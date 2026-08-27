using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TheBugTracker.Repository
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager) : ICompanyRepository
    {
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<ApplicationUser> users = await context.Users
                .Where(u => u.CompanyId == userInfo.CompanyId)
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            IEnumerable<ApplicationUser> usersInRole = await userManager.GetUsersInRoleAsync(Enum.GetName(role)!);
            usersInRole = usersInRole.Where(u => u.CompanyId == userInfo.CompanyId);

            return usersInRole;
        }

        public async Task<Company> GetCompanyAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Company? company = await context.Companies
                .Include(c => c.Members)
                .Include(c => c.Invites)
                .FirstAsync(c => c.Id == userInfo.CompanyId);

            return company;
        }

        public async Task UpdateCompanyAsync(Company company, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin) || company.Id != userInfo.CompanyId)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            FileUpload? oldImage = null;

            if (company.Image is not null && company.Image.Id != company.ImageId)
            {
                oldImage = await context.Companies
                    .Where(c => c.Id == userInfo.CompanyId)
                    .Select(c => c.Image)
                    .FirstOrDefaultAsync();

                context.Add(company.Image);
                company.ImageId = company.Image.Id;
            }

            context.Update(company);
            await context.SaveChangesAsync();

            if (oldImage is not null)
            {
                context.Remove(oldImage);
                await context.SaveChangesAsync();
            }
        }

        public async Task AssignUserRoleAsync(string userId, Role newRole, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin) || newRole == Role.DemoUser || userId == userInfo.UserId)
            {
                return;
            }

            ApplicationUser? userToAssign = await userManager.FindByIdAsync(userId);

            if (userToAssign?.CompanyId != userInfo.CompanyId)
            {
                return;
            }

            var originalRoles = await userManager.GetRolesAsync(userToAssign);

            if (originalRoles.Any(roleName => roleName == nameof(Role.DemoUser) || roleName == Enum.GetName(newRole)))
            {
                return;
            }

            try
            {
                var removedResult = await userManager.RemoveFromRolesAsync(userToAssign, originalRoles);

                if (!removedResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(", ", removedResult.Errors.Select(e => e.Description)));
                }

                var addedResult = await userManager.AddToRoleAsync(userToAssign, Enum.GetName(newRole)!);

                if (!addedResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(", ", removedResult.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                await userManager.AddToRolesAsync(userToAssign, originalRoles);

                throw;
            }
        }
    }
}