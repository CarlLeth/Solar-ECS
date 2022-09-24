using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solar.Ecs.Construction;
using Solar.Common.ChangeTracking;
using Solar.Common.Lists;
using Solar.Common.Identification;
using Solar.Common.Hierarchy;
using Solar.Common.Comments;
using Solar.Common.Time;
using Solar.Common.Contacts;

namespace Solar.Common
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
