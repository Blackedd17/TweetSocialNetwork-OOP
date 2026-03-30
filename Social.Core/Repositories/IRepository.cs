using System;
using System.Collections.Generic;

namespace Social.Core.Repositories
{
    public interface IRepository<T>
    {
        void Add(T item);
        List<T> GetAll();
        void Remove(T item);
    }
}
