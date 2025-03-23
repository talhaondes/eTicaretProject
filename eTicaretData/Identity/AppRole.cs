﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace eTicaretData.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole():base()
        {
        }

        public AppRole(string roleName) : base(roleName)
        {
        }
    }
}
