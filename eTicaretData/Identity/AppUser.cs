using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretData.Entities;
using Microsoft.AspNetCore.Identity;

namespace eTicaretData.Identity
{
    public class AppUser : IdentityUser<int>
    {

        public AppUser() : base()
        {

        }

        public AppUser(string userName) : base(userName)
        {

        }
        public string Name { get; set; }
        public string SurName { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

    }
}
