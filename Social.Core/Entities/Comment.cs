using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Core.Entities
{
    namespace Social.Core.Entities
    {
        public class Comment : BaseEntity
        {
            public Guid UserId { get; set; }
            public string Text { get; set; }
            public Comment(Guid userId, string text)
            {
                UserId = userId;
                Text = text;
            }
        }
    }
}
