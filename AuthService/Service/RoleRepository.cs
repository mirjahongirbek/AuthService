using AuthService.Interfaces.Models;
using AuthService.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AuthService.Service
{
    public class RoleRepository<TRole> : IRoleRepository<TRole, int>
        where TRole : class, IUserRole<int>
    {

        public void Add(TRole model)
        {
            throw new NotImplementedException();
        }

        public void Delete(TRole model)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TRole> Find(Expression<Func<TRole, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public TRole Get(int id)
        {
            throw new NotImplementedException();
        }

        public TRole GetFirst(Expression<Func<TRole, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(TRole model)
        {
            throw new NotImplementedException();
        }
    }
}
