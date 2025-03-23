using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretBusiness.Abstract;
using eTicaretData.Entities;

namespace eTicaretDal.Abstract
{
    public interface IAddressDal : IGenericRepository<Address>
    {
        List<Address> GetAddressesByUserId(int userId);

    }
}
