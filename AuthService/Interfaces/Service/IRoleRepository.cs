using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AuthService.Interfaces.Service
{
    public interface IRoleRepository<T>
        where T: IdentityRole
    {
        DbSet<T> DbSet { get; }
        bool Add(T model);
        bool Delete(T model);
        bool Delete(int id);
        bool Update(T model);
        T Get(int id);
        T GetFirst(Expression<Func<T, bool>> expression);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        List<T> GetList(List<int> roles);
    }
}
