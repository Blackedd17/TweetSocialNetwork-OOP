using Social.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Social.Core.Repositories
{
    /// <summary>
    /// Generic in-memory repository (RAM дээр хадгална)
    /// </summary>
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly List<T> data = new List<T>();

        /// <summary>
        /// Entity нэмнэ
        /// </summary>
        public void Add(T entity)
        {
            data.Add(entity);
        }

        /// <summary>
        /// ID-р устгана
        /// </summary>
        public void Delete(Guid id)
        {
            var item = data.FirstOrDefault(x => x.Id == id);
            if (item != null)
                data.Remove(item);
        }

        /// <summary>
        /// Бүгдийг авна
        /// </summary>
        public List<T> GetAll()
        {
            return data;
        }

        /// <summary>
        /// ID-р хайна
        /// </summary>
        public T GetById(Guid id)
        {
            return data.FirstOrDefault(x => x.Id == id);
        }
    }
}