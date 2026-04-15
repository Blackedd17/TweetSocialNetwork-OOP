using System;
using System.Collections.Generic;

namespace Social.Core.Repositories
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Delete(Guid id);
        List<T> GetAll();
        T GetById(Guid id);
    }
}