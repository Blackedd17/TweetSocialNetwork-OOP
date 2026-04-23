using System;

namespace Social.Core.Entities
{
    /// <summary>
    /// Бүх entity-д нийтлэг хэрэгтэй үндсэн талбаруудыг агуулна.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Entity-ийн давтагдашгүй ID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Entity үүссэн огноо, цаг.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}