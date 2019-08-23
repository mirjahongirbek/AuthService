
using AuthService.Interfaces.Service;
using AuthService.Models;
using EntityRepository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AuthService.Service
{
    public class RoleRepository<TRole> : IRoleRepository<TRole, int>
        where TRole : EntityRole
    {
        private DbSet<TRole> _dbSet;
        public DbSet<TRole> DbSet { get => _dbSet; set => _dbSet = value; }

        private DbContext _context;
        public RoleRepository(IDbContext context)
        {
            _context = context.DataContext;
            DbSet = _context.Set<TRole>();

        }
        public virtual bool Add(TRole model)
        {
            var role = _dbSet.FirstOrDefault(m => m.Name == model.Name);
            if (role != null)
            {
                return false;
            }
            _dbSet.Add(model);
            return true;
        }

        public virtual bool Delete(TRole model)
        {
            var role = _dbSet.Remove(model);
            if (role == null)
            {
                return false;
            }
            return true;
        }

        public virtual bool Delete(int id)
        {
           var role= Get(id);
            if(role== null)
            {
                return false;
            }
            _dbSet.Remove(role);
            return true;
        }

        public virtual IEnumerable<TRole> Find(Expression<Func<TRole, bool>> expression)
        {
           return _dbSet.Where(expression);
        }

        public virtual TRole Get(int id)
        {
           return _dbSet.FirstOrDefault(m=>m.Id==id);
        }

        public virtual TRole GetFirst(Expression<Func<TRole, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(TRole model)
        {
            throw new NotImplementedException();
        }
    }
}
