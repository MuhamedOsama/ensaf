﻿using Ensaf.Domain.Enums;
using Ensaf.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Ensaf.Persistence
{
    public class ApplicationDbContextSeed
    {
        public static void Seed(ApplicationDbContext context)
        {

            context.Database.EnsureCreated();

            if (context.Roles.Count() == 0)
            {
                var staticRoles = Enum.GetNames(typeof(Roles)).ToList();
                var roles = new List<Role>();
                staticRoles.ForEach(r => roles.Add(new Role { Name = r }));
                context.Roles.AddRange(roles);
                context.SaveChanges();

            }

            if (context.Users.Count() == 0)
            {
                var users = new List<User>
                {
                    new User {
                        Name = "admin",
                        Email = "admin@admin.com",
                        PasswordHash =  BC.HashPassword("123456"),
                        Verified = DateTime.Now,
                        Created = DateTime.Now,
                        AcceptTerms = true,
                        IdType = IdentificationType.KsA
                    },
                };
                context.Users.AddRange(users);
                int r = context.SaveChanges();

                users[0].UserRoles.Add(new UserRole
                {
                    RoleId = context.Roles.SingleOrDefault(r => r.Name == nameof(Roles.SysAdmin)).Id,
                    UserId = users[0].Id
                });

                context.SaveChanges();


            }
        }



    }
}
