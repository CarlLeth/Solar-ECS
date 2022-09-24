using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework
{
    public class PersistenceTypeLibrary : IPersistenceTypeLibrary
    {
        private ModuleBuilder ModuleBuilder { get; set; }
        private Dictionary<Type, Type> PersistenceTypes { get; set; }

        public PersistenceTypeLibrary()
        {
            ModuleBuilder = CreateModuleBuilder();
            PersistenceTypes = new Dictionary<Type, Type>();
        }

        private ModuleBuilder CreateModuleBuilder()
        {
            var dynamicAssemblyName = new AssemblyName("Solar.Ecs.DynamicAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule("SolarEcsDynamicModule");
        }

        public Type GetPersistedType(Type componentType)
        {
            if (PersistenceTypes.ContainsKey(componentType))
            {
                return PersistenceTypes[componentType];
            }
            else
            {
                throw new KeyNotFoundException(string.Format("The type '{0}' does not have an associated persistence type.  If this is a persisted component type, make sure it has been registered in a component package.", componentType));
            }
        }

        public void CreatePersistenceType(Type componentType)
        {
            if (PersistenceTypes.ContainsKey(componentType))
            {
                return;
            }

            try
            {
                var typeBuilder = ModuleBuilder.DefineType(string.Format("{0}.DynamicProxies.{1}Component", componentType.Namespace, componentType.Name), TypeAttributes.Class | TypeAttributes.Public);
                Type baseType = typeof(EntityWith<>).MakeGenericType(componentType);
                typeBuilder.SetParent(baseType);

                PersistenceTypes.Add(componentType, typeBuilder.CreateType());
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "Duplicate type name within an assembly.")
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
