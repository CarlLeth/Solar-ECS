using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Construction;

namespace SolarEcs.Common
{
    public class CommonComponentPackage : IComponentPackage
    {
        public void Register(IComponentRegistration register)
        {
            register.Component<ChangeTracking.EntityChangeEvent>();
            register.Component<ChangeTracking.EntityCreationEvent>();
            register.Component<ChangeTracking.EntityModificationEvent>();

            register.Component<Comments.TextComment>();

            register.Component<Contacts.ContactInfo>();

            register.Component<Identification.Abbreviation>();
            register.Component<Identification.Alias>();
            register.Component<Identification.CodeName>();
            register.Component<Identification.IdentificationNumber>();
            register.Component<Identification.StaticName>();
            register.Component<Identification.StaticText>();

            register.Component<Hierarchy.HierarchyPosition>();

            register.Component<Lists.OrderedListMembership>();
            register.Component<Lists.UnorderedListMembership>();

            register.ValueType<Time.TimePeriod>();

            register.Component<Versioning.EntityVersion>();
        }
    }
}
