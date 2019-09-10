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
    public class IdentityRoleService<TRole> : IRoleRepository<TRole>
        where TRole : IdentityRole
    {
        public DbSet<TRole> DbSet { get => _dbSet; }
        private DbSet<TRole> _dbSet;
        private DbContext _context;
        public IdentityRoleService(IDbContext context)
        {
            _dbSet=context.DataContext.Set<TRole>();
            _context = context.DataContext;
        }
        private void Save()
        {
            _context.SaveChanges();
        }
        public bool Add(TRole model)
        {
            _dbSet.Add(model);
            Save();
            return true;
        }

        public bool Delete(TRole model)
        {
            _dbSet.Remove(model);
            Save();
            return true;
        }

        public bool Delete(int id)
        {
           var model= Get(id);
           return Delete(model);
        }

        public IEnumerable<TRole> Find(Expression<Func<TRole, bool>> expression)
        {
           return _dbSet.Where(expression);
        }

        public TRole Get(int id)
        {
           return GetFirst(m => m.Id == id);
        }

        public TRole GetFirst(Expression<Func<TRole, bool>> expression)
        {
            return _dbSet.FirstOrDefault(expression);
        }

        public List<TRole> GetList(List<int> roles)
        {
            var list =new  List<TRole>();
           foreach(var i in roles)
            {
                list.Add(Get(i));
            }
            return list;
        }
        public bool Update(TRole model)
        {
            _dbSet.Update(model);
            return true;
        }
    }
}

