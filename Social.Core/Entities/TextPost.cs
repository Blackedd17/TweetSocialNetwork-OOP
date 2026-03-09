using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Core.Entities
{
    public class TextPost : Post
    {
        public TextPost(Guid authorId, string content)
           : base(authorId, content)
        {
        }
    }
}
