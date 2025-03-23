using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretBusiness.Concrete;
using eTicaretDal.Abstract;
using eTicaretData.Context;
using eTicaretData.Entities;

namespace eTicaretDal.Concrete
{
    public class CategoryDal : GenericRepository<Category, ETicaretContext>, ICategoryDal
    {
    }
}
