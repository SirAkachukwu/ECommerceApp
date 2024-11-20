﻿using CommerceApp.Configurations;
using CommerceApp.Models.AuthModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CommerceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApiUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration (new RoleConfiguration());
        }

    }
}