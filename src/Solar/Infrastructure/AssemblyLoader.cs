using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Infrastructure
{
    public static class AssemblyLoader
    {
        public static void LoadAllLocalAssemblies()
        {
            var loadedAssemblyNames = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName);
            TryLoadFromDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"), loadedAssemblyNames);
            TryLoadFromDirectory(AppDomain.CurrentDomain.BaseDirectory, loadedAssemblyNames);
        }

        private static void TryLoadFromDirectory(string directory, IEnumerable<string> loadedAssemblyNames)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var missingLocalAssemblies = Directory.GetFiles(directory, @"*.dll", SearchOption.AllDirectories)
                .Where(dll => !loadedAssemblyNames.Contains(Path.GetFileNameWithoutExtension(dll)));

            missingLocalAssemblies.ForEach(assemblyFile => System.Reflection.Assembly.LoadFrom(assemblyFile));
        }
    }
}
