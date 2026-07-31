using E_Commerce.Application.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder
        (
        StoreDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<IdentityDataSeeder> logger
        )


        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var pendingMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);

                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (!await _roleManager.Roles.AnyAsync(cancellationToken))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                if (!await _userManager.Users.AnyAsync(cancellationToken))
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "Ahmed Samy",
                        Email = "AhmedSamy@gmail.com",
                        UserName = "AhmedSamy",
                        PhoneNumber = "01111075920"
                    };

                    var createResult = await _userManager.CreateAsync(admin, "P@ssw0rd");
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "SuperAdmin");
                    }
                    else
                    {
                        var Errors = string.Join(';', createResult.Errors.Select(e => e.Description));

                        _logger.LogWarning($"Cannot Seed Default Admin {Errors}");
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Identity Data Seeding Failed");
                return;
            }
        }
    }
}
