using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretBusiness.Concrete;
using eTicaretDal.Abstract;
using eTicaretData.Context;
using eTicaretData.Entities;
using Microsoft.EntityFrameworkCore;

namespace eTicaretDal.Concrete
{
    public class OrderDal : GenericRepository<Order, ETicaretContext>, IOrderDal
    {
        private readonly ETicaretContext _context;

        public OrderDal(ETicaretContext context)
        {
            _context = context;
        }

        public Order GetOrderWithDetails(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderLines) // Include OrderLines
                .ThenInclude(ol => ol.Product) // Include Product in OrderLine
                .FirstOrDefault(o => o.OrderId == orderId);
        }
    }
}
