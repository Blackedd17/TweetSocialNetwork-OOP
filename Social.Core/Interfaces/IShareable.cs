using System;

namespace Social.Core.Interfaces
{
    public interface IShareable
    {
        void Share(Guid userId);
    }
}
