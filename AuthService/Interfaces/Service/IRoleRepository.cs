using AuthService.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AuthService.Interfaces.Service
{
    public interface IRoleRepository<T, TKey>
        where T:class, IUserRole<TKey>
    {
        void Add(T model);
        void Delete(T model);
        void Delete(TKey id);
        void Update(T model);
        T Get(TKey id);
        T GetFirst(Expression<Func<T, bool>> expression);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);

    }
}
