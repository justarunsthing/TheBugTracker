using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Helpers;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using Microsoft.AspNetCore.Identity;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class CompanyDTOService(ICompanyRepository repository, UserManager<ApplicationUser> userManager) : ICompanyDTOService
    {
        public async Task<IEnumerable<UserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            IEnumerable<ApplicationUser> users = await repository.GetUsersAsync(userInfo);
            List<UserDTO> dtos = [];

            foreach (ApplicationUser user in users)
            {
                UserDTO dto = await user.ToDTOWithRole(userManager);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            var usersInRole = await repository.GetUsersInRoleAsync(role, userInfo);
            List<UserDTO> dtos = [];

            foreach (ApplicationUser user in usersInRole)
            {
                UserDTO dto = user.ToDTO();
                dto.Role = role;

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<CompanyDTO> GetCompanyAsync(UserInfo userInfo)
        {
            Company company = await repository.GetCompanyAsync(userInfo);
            CompanyDTO dto = company.ToDTO();

            dto.Members.Clear();

            foreach (ApplicationUser user in company.Members)
            {
                UserDTO userWithRole = await user.ToDTOWithRole(userManager);
                dto.Members.Add(userWithRole);
            }

            return dto;
        }

        public async Task UpdateCompanyAsync(CompanyDTO company, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin))
            {
                return;
            }

            Company dbCompany = await repository.GetCompanyAsync(userInfo);

            // Clear navigational properties to avoid EF Core tracking issues
            dbCompany.Projects.Clear();
            dbCompany.Members.Clear();
            dbCompany.Invites.Clear();

            dbCompany.Name = company.Name;
            dbCompany.Description = company.Description;

            if (company.ImageUrl.StartsWith("data:"))
            {
                try
                {
                    dbCompany.Image = UploadHelper.GetFileUpload(company.ImageUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }            
            }

            await repository.UpdateCompanyAsync(dbCompany, userInfo);
        }
    }
}