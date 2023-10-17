using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewShoppingCart.Models;

namespace NewShoppingCart.Data
{
    public class NewShoppingCartContext : IdentityDbContext
    {
        public NewShoppingCartContext (DbContextOptions<NewShoppingCartContext> options)
            : base(options)
        {
        }

        public DbSet<NewShoppingCart.Models.User> User { get; set; } = default!;

        public DbSet<NewShoppingCart.Models.ItemModel> ItemModel { get; set; } = default!;

    

        public DbSet<NewShoppingCart.Models.UserCartItem> UserCartItem { get; set; } = default!;
    }
}
