using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretBusiness.Abstract
{
    public interface IGenericRepository<T> where T : class, new()
    {
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T GetById(int id);
        T GetById(Expression<Func<T, bool>> filter);
        void add(T entity);
        void update(T entity);
        void delete(T entity); // sadece varlık nesnesiyle silme işlemi
    }

}
