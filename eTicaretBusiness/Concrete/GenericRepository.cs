using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTicaretBusiness.Abstract;
using eTicaretData.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eTicaretBusiness.Concrete
{
    public class GenericRepository<T, Tcontext> : IGenericRepository<T> where T : class, new() where Tcontext : IdentityDbContext<AppUser, AppRole, int>, new()
    {
        public void add(T entity)
        {
            using (var context = new Tcontext())
            {
                context.Set<T>().Add(entity);
                context.SaveChanges();
            }
        }

        public void delete(T entity)
        {
            using (var context = new Tcontext())
            {
                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
                //context.Set<T>().Remove(entity);    
                //context.SaveChanges();
            }
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            using (var context = new Tcontext())
            {
               return filter==null?context.Set<T>().ToList():context.Set<T>().Where(filter).ToList();
            }
        }

        public T GetById(int id)
        {
            using (var context = new Tcontext())
            {
                var nesne = context.Set<T>().Find(id);
                return nesne;
            }
        }

        public T GetById(Expression<Func<T, bool>> filter)
        {
            using (var context = new Tcontext())
            {
                var nesne = context.Set<T>().Find(filter);
                return nesne;
            }
        }

        public void update(T entity)
        {
            using (var context = new Tcontext())
            {
                context.Entry(entity).State= EntityState.Modified;
                context.SaveChanges();
            }
        }
    }


}
