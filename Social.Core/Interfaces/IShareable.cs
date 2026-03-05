using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Core.Interfaces
{
    public interface IShareable
    {
        void Share(Guid userId);
    }
}
