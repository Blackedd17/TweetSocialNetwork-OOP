using System;

namespace Social.Core.Repositories
{
    /// <summary>
    /// Post дээр хэрэглэгчийн reaction-ийг удирдах repository interface.
    /// 
    /// Энэ interface нь:
    /// - Reaction нэмэх эсвэл солих
    /// - Нэг reaction-ийг устгах
    /// - Тодорхой төрлийн reaction count авах
    /// - Хэрэглэгчийн одоогийн reaction-г авах
    /// - Toggle reaction хийх
    /// боломжийг тодорхойлно.
    /// </summary>
    public interface IReactionRepository
    {
        /// <summary>
        /// Тухайн post дээр хэрэглэгчийн reaction-ийг нэмэх эсвэл шинэчлэх.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="type">Reaction төрөл</param>
        void React(Guid postId, Guid userId, string type);

        /// <summary>
        /// Тухайн post дээрх хэрэглэгчийн reaction-ийг устгах.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        void RemoveReaction(Guid postId, Guid userId);

        /// <summary>
        /// Тодорхой reaction төрлийн нийт тоог авна.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="type">Reaction төрөл</param>
        /// <returns>Reaction count</returns>
        int GetCount(Guid postId, string type);

        /// <summary>
        /// Тухайн хэрэглэгчийн тухайн post дээр өгсөн reaction-г авна.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Reaction төрөл, байхгүй бол null</returns>
        string GetUserReaction(Guid postId, Guid userId);

        /// <summary>
        /// Хэрэв ижил reaction байвал устгана, өөр reaction байвал солино.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="type">Reaction төрөл</param>
        void ToggleReaction(Guid postId, Guid userId, string type);
    }
}