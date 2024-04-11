using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext.Entities;
using System.Security.Cryptography;

namespace DataAccess.DataContext
{
    public static class ModelBuilderExtensions
    {
       
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //Appmodule
            Guid ModuleAdministratorId = Guid.NewGuid();
            Guid ModuleSystemManageId = Guid.NewGuid();
            Guid ModuleUsersId = Guid.NewGuid();
            Guid ModuleRolesId = Guid.NewGuid();
            Guid ModuleModulesId = Guid.NewGuid();
            
            modelBuilder.Entity<Appmodule>()
                .HasData(
                   new Appmodule
                   {
                       Id = ModuleAdministratorId,
                       Title = "Administrator",
                       Subtitle = "Application Management",
                       Type = "group",
                       IsActive = true,
                       Sequence = 0
                   },
                   new Appmodule
                   {
                       Id = ModuleSystemManageId,
                       Title = "System Management",
                       Type = "collapsable",
                       Icon = "feather:settings",
                       IsActive = true,
                       Sequence = 10,
                       ParentId = ModuleAdministratorId
                   },
                   new Appmodule
                   {
                       Id = ModuleUsersId,
                       Title = "Users",
                       Subtitle = "Application Users",
                       Type = "basic",
                       Icon = "feather:users",
                       Path = "/app-user/users",
                       IsActive = true,
                       Sequence = 20,
                       ParentId = ModuleSystemManageId
                   },
                   new Appmodule
                   {
                       Id = ModuleRolesId,
                       Title = "Roles",
                       Subtitle = "Application Roles",
                       Type = "basic",
                       Icon = "heroicons_outline:user-group",
                       Path = "/app-role/roles",
                       IsActive = true,
                       Sequence = 30,
                       ParentId = ModuleSystemManageId
                   },
                   new Appmodule
                   {
                       Id = ModuleModulesId,
                       Title = "Modules",
                       Subtitle = "Application Module",
                       Type = "basic",
                       Icon = "feather:list",
                       Path = "/app-module/modules",
                       IsActive = true,
                       Sequence = 40,
                       ParentId = ModuleSystemManageId
                   }
                );

            // Approles
            Guid RolesRoleId = Guid.NewGuid();
            modelBuilder.Entity<Approles>()
                .HasData(
                    new Approles { Id = RolesRoleId, Name = "SUPERADMIN", Description = "Super Admin", CreatedBy = "System", CreatedDate = DateTime.Now }
                );

            // Apppermission
            modelBuilder.Entity<Apppermission>()
                .HasData(
                    new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleUsersId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now },
                    new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleRolesId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now },
                    new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleModulesId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now }
                );

            // Appusers
            string username = "admin";
            string password = "P@ssw0rd";
            Guid UserUserId = Guid.NewGuid();
            CreatePasswordHash(username, password, out byte[] passwordHash, out byte[] passwordSalt);
            modelBuilder.Entity<Appusers>()
                .HasData(
                    new Appusers { Id = UserUserId, Username = username, PasswordHash = passwordHash, PasswordSalt = passwordSalt, IsActive = true, LoginAttemptCount = 0, CreatedBy = "System", CreatedDate = DateTime.Now }
                );
        }

        //for sql server
        public static async Task SeedData(DatabaseContext context)
        {
            //Appmodule
            Guid ModuleAdministratorId = Guid.NewGuid();
            Guid ModuleSystemManageId = Guid.NewGuid();
            Guid ModuleUsersId = Guid.NewGuid();
            Guid ModuleRolesId = Guid.NewGuid();
            Guid ModuleModulesId = Guid.NewGuid();

            Guid RolesRoleId = Guid.NewGuid();
            Guid UserUserId = Guid.NewGuid();

            using (var transaction = context.Database.BeginTransaction())
            {
                if (!context.Appmodule.Any())
                {
                    context.Appmodule.AddRange(
                        new Appmodule { Id = ModuleAdministratorId, Title = "Administrator", Subtitle = "Application Management", Type = "group", IsActive = true, Sequence = 100 },
                        new Appmodule { Id = ModuleSystemManageId, Title = "System Management", Type = "collapsable", Icon = "feather:settings", IsActive = true, Sequence = 110, ParentId = ModuleAdministratorId },
                        new Appmodule { Id = ModuleUsersId, AuthCode = "USERS", Title = "Users", Subtitle = "Application Users", Type = "basic", Icon = "feather:users", Path = "/app-user/users", IsActive = true, Sequence = 120, ParentId = ModuleSystemManageId },
                        new Appmodule { Id = ModuleRolesId, AuthCode = "ROLES", Title = "Roles", Subtitle = "Application Roles", Type = "basic", Icon = "heroicons_outline:user-group", Path = "/app-role/roles", IsActive = true, Sequence = 130, ParentId = ModuleSystemManageId },
                        new Appmodule { Id = ModuleModulesId, AuthCode = "MODULES", Title = "Modules", Subtitle = "Application Module", Type = "basic", Icon = "feather:list", Path = "/app-module/modules", IsActive = true, Sequence = 140, ParentId = ModuleSystemManageId }
                        // new Appmodule { Id = 6, Title = "BackOffice", Subtitle = "Back-office Applications", Type = "group", Icon = "feather:list", IsActive = true, Sequence = 0 },
                        // new Appmodule { Id = 7, Title = "Home", Subtitle = "หน้าหลัก", Type = "basic", Icon = null, Path = "/home", IsActive = true, Sequence = 5, ParentId = 6 }
                        );

                    //for sql server
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.appmodule ON");
                    await context.SaveChangesAsync();
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.appmodule OFF");

                }


                if (!context.Approles.Any())
                {
                    context.Approles.Add(new Approles { Id = RolesRoleId, Name = "SUPERADMIN", Description = "Super Admin", CreatedBy = "System", CreatedDate = DateTime.Now });

                    //for sql server
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.approles ON");
                    await context.SaveChangesAsync();
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.approles OFF");

                }

                if (!context.Apppermission.Any())
                {
                    context.Apppermission.AddRange(
                            new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleUsersId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now },
                            new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleRolesId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now },
                            new Apppermission { RoleId = RolesRoleId, ModuleId = ModuleModulesId, IsAccess = true, IsCreate = true, IsEdit = true, IsView = true, IsDelete = true, ModifiedBy = "System", ModifiedDate = DateTime.Now }
                    );

                    await context.SaveChangesAsync();
                }

                if (!context.Appusers.Any())
                {
                    string username = "admin";
                    string password = "Widely123!";

                    CreatePasswordHash(username, password, out byte[] passwordHash, out byte[] passwordSalt);

                    context.Appusers.AddRange(
                            new Appusers { Id = UserUserId, RoleId = RolesRoleId, Username = username, PasswordHash = passwordHash, PasswordSalt = passwordSalt, IsActive = true, LoginAttemptCount = 0, CreatedBy = "System", CreatedDate = DateTime.Now }
                    );

                    //for sql server
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.appusers ON");
                    await context.SaveChangesAsync();
                    // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.appusers OFF");

                }

                transaction.Commit();

            }

        }

        public static void CreatePasswordHash(string username, string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(username + password));
            }
        }
    }
}
