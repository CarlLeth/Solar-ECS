using SolarEcs.Common.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.NetCore.Infrastructure
{
    public class TestDataAgent : IDataAgent
    {
        public static readonly Guid DefaultTestAgent = new Guid("e63068f8-94cb-4d04-b0bd-7b29a74c9006");

        public Guid Id { get; set; } = DefaultTestAgent;
    }
}
