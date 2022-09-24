using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Construction;
using SolarEcs.Common.ChangeTracking;
using SolarEcs.Common.Lists;
using SolarEcs.Common.Identification;
using SolarEcs.Common.Hierarchy;
using SolarEcs.Common.Comments;
using SolarEcs.Common.Time;
using SolarEcs.Common.Contacts;

namespace SolarEcs.Common
{
    public class CommonComponentPackage : IComponentPackage
    {
        public void Register(IComponentRegistration register)
        {
            register.ValueType<TimePeriod>();

            register.Component<Abbreviation>();
            register.Component<Alias>();
            register.Component<CodeName>();
            register.Component<ContactInfo>();
            register.Component<IdentificationNumber>();
            register.Component<StaticName>();
            register.Component<StaticText>();

            register.Component<OrderedListMembership>();
            register.Component<UnorderedListMembership>();

            register.Component<TextComment>();

            register.Component<HierarchyPosition>();

            register.Component<EntityCreationEvent>();
            register.Component<EntityModificationEvent>();
        }
    }
}
