using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Identification;
using SolarEcs.Common.Comments;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class CommentTests : SolarTestsBase
    {
        [TestMethod]
        public void CanCommentOnEntity()
        {
            Guid cookieMonster = Guid.NewGuid();

            Assign(cookieMonster, new StaticName("Cookie Monster"));

            Guid me = Guid.NewGuid();
            Add(new TextComment(cookieMonster, me, new DateTime(2015, 5, 13), "What starts with the letter 'C'?"));

            var comments = Query<TextComment>();
            var cookieMonsterComments = comments
                .ShiftKey(o => o.Model.Entity)
                .For(cookieMonster)
                .ExecuteAll();

            Assert.AreEqual(1, cookieMonsterComments.Count());

            var first = cookieMonsterComments.First().Model;
            Assert.AreEqual(cookieMonster, first.Entity);
            Assert.AreEqual(me, first.Commenter);
            Assert.AreEqual(new DateTime(2015, 5, 13), first.TimeUtc);
            Assert.AreEqual("What starts with the letter 'C'?", first.Text);
        }
    }
}
