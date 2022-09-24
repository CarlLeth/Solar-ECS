using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Comments
{
    public class TextComment
    {
        public Guid Entity { get; private set; }
        public Guid Commenter { get; private set; }
        public DateTime TimeUtc { get; private set; }
        public string Text { get; private set; }

        public TextComment(Guid entity, Guid commenter, DateTime timeUtc, string text)
        {
            this.Entity = entity;
            this.Commenter = commenter;
            this.TimeUtc = timeUtc;
            this.Text = text;
        }

        private TextComment() { }
    }
}
