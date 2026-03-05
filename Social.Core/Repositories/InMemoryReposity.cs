using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
