using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretBusiness.Abstract;
using eTicaretBusiness.Concrete;
using eTicaretDal.Abstract;
using eTicaretData.Context;
using eTicaretData.Entities;

namespace eTicaretDal.Concrete
{
    public class AddressDal : GenericRepository<Address, ETicaretContext>, IAddressDal
    {
        public List<Address> GetAddressesByUserId(int userId)
        {
            return GetAll(a => a.UserId == userId);
        }
    }
}
