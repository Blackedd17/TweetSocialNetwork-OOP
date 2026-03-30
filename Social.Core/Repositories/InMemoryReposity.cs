using System;
using System.Collections.Generic;

namespace Social.Core.Repositories
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> items = new List<T>();
        public void Add(T item)
        {
            items.Add(item);
        }
        public List<T> GetAll()
        {
            return items;
        }
        public void Remove(T item)
        {
            items.Remove(item);
        }
    }
}
