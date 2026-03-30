using System;

namespace Social.Core.Interfaces
{
    public interface ICommentable
    {
        void AddComment(Guid userId, string text);
    }
}
