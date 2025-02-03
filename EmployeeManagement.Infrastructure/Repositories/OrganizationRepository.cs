using EmployeeManagement.Core.DTO;
using EmployeeManagement.Infrastructure.Interfaces.Repositories;
using EmployeeManagement.Core.Entites;
using EmployeeManagement.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleRepository _roleRepository;
       

        public OrganizationRepository(ApplicationDbContext context, IRoleRepository roleRepository)
        {
            _context = context;
            _roleRepository = roleRepository;
        }

        //public async Task<Employee> AddAdmin(EmployeeDTO employeeDTO, int organizationId)
        //{
        //    var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);
        //    if(organization == null)
        //    {
        //        throw new ArgumentException("Organization not found");
        //    }


        //}

        public async Task<Organization> CreateOrganization(OrganizationDTO organizationDTO)
        {
            try
            {
                var organization = new Organization()
                {
                    Name = organizationDTO.Name,
                    Address = organizationDTO.Address,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                var result = await _context.Organizations.AddAsync(organization);
                if (result == null)
                {
                    throw new Exception("Organization can not be created");
                }
                await _context.SaveChangesAsync();
                return organization;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while creating organization " + ex.Message);
            }
        }

        public async Task<bool> DeleteOrganization(int organizationId)
        {
            try
            {
                var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == organizationId && o.IsDeleted == false);

                if (organization == null)
                {
                    throw new ArgumentException("Organization does not exist or has already been deleted");
                }

                organization.IsDeleted = true;
                var employees = await _context.Employees
                    .Where(e => e.OrganizationId == organization.Id && e.IsDeleted == false)
                    .ToListAsync();

                foreach (var employee in employees)
                {
                    employee.IsDeleted = true;

                    var removeUserFromAllRoles = await _roleRepository.RemoveAllRoles(employee.UserId);
                    if (!removeUserFromAllRoles)
                    {
                        throw new Exception($"Employee {employee.FirstName} {employee.LastName} could not be removed from roles");
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while deleting organization " + ex.Message);
            }
        }


        public async Task<OrganizationDTO> GetOrganizationsDetials(int organizationId)
        {
            try
            {
                var organizationDetails = await (from org in _context.Organizations
                                                 where org.IsDeleted == false
                                                 && org.Id == organizationId
                                                 join emp in _context.Employees
                                                 on org.Id equals emp.OrganizationId into empGroup
                                                 from emp in empGroup.DefaultIfEmpty()
                                                 where emp == null || emp.IsDeleted == false
                                                 select new { org, emp }).ToListAsync();

                if (organizationDetails == null)
                {
                    throw new Exception("Organization not found");
                }

                var organizationDTO = organizationDetails
                                     .GroupBy(x => x.org.Id)
                                     .Select(group => new OrganizationDTO
                                     {
                                         Id = group.Key,
                                         Name = group.FirstOrDefault().org.Name,
                                         Address = group.FirstOrDefault().org.Address,
                                         CreatedAt = group.FirstOrDefault().org.CreatedAt,
                                         Employees = group.Where(x => x.emp != null).Select(x => new EmployeeDTO
                                         {
                                             Id = x.emp.Id,
                                             FirstName = x.emp.FirstName,
                                             LastName = x.emp.LastName,
                                             Email = x.emp.Email,
                                             CreatedBy = x.emp.CreatedBy,
                                             OrganizationId = x.emp.OrganizationId,
                                             CreareAt = x.emp.CreatedAt
                                         }).ToList()
                                     }).FirstOrDefault();

                //List<EmployeeDTO> employees = new List<EmployeeDTO>();
                //foreach (var emp in organizationDetails.Employees)
                //{
                //    employees.Add(new EmployeeDTO()
                //    {
                //        FirstName = emp.FirstName,
                //        LastName = emp.LastName,
                //        Email = emp.Email,
                //        Id = emp.Id,
                //        CreatedBy = emp.CreatedBy
                //    });
                //}

                //return (new OrganizationDTO()
                //{
                //    Name = organizationDetails.Name,
                //    Address = organizationDetails.Address,
                //    CreatedAt = organizationDetails.CreatedAt,
                //    Id = organizationDetails.Id,
                //    Employees = employees
                //}); ;
                return organizationDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while fetching organization data " + ex.Message);
            }
        }

        public async Task<List<Organization>> GetAllOrganizationDetails()
        {
            try
            {
                var organizationDetails = await (from org in _context.Organizations
                                                 where org.IsDeleted == false
                                                 join emp in _context.Employees
                                                 on org.Id equals emp.OrganizationId into empGroup
                                                 from emp in empGroup.DefaultIfEmpty() 
                                                 where emp == null || emp.IsDeleted == false 
                                                 select new { org, emp }).ToListAsync();

                var orgList = organizationDetails
                                .GroupBy(x => x.org.Id) 
                                .Select(group => new Organization
                                {
                                    Id = group.Key,
                                    Name = group.FirstOrDefault().org.Name,
                                    Address = group.FirstOrDefault().org.Address,
                                    CreatedAt = group.FirstOrDefault().org.CreatedAt,
                                    Employees = group
                                                .Where(x => x.emp != null) 
                                                .Select(x => new Employee
                                                {
                                                    Id = x.emp.Id,
                                                    FirstName = x.emp.FirstName,
                                                    LastName = x.emp.LastName,
                                                    Email = x.emp.Email,
                                                    CreatedBy = x.emp.CreatedBy,
                                                    OrganizationId = x.emp.OrganizationId,
                                                    CreatedAt = x.emp.CreatedAt
                                                }).ToList()
                                }).ToList();

                return orgList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching organization list: " + ex.Message);
            }
        }


        public async Task<bool> IsOrganizationExisits(int organizationId)
        {
            try
            {
                return await _context.Organizations.AnyAsync(o => o.Id == organizationId);
            }
            catch(Exception ex){
                throw new Exception("Error occured while checking organization existance " + ex.Message);
            }
        }

        public async Task<Organization> UpdateOrganization(OrganizationDTO organizationDTO, int organizationId)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId && o.IsDeleted == false);

            if (organization == null)
            {
                throw new ArgumentException("Organization Do not exists");
            }

            else
            {
                organization.Name = organizationDTO.Name;
                organization.Address = organizationDTO.Address;
                organization.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();  
                return organization;
            }
        }

        public async Task<List<EmployeeDTO>> GetAdmin(int organizationId)
        {
            try
            {
                return await (from emp in _context.Employees
                              join userRole in _context.UserRoles
                              on emp.UserId equals userRole.UserId
                              join role in _context.Roles
                              on userRole.RoleId equals role.Id
                              where emp.OrganizationId == organizationId
                              && emp.IsDeleted == false
                              && role.IsDeleted == false
                              && userRole.IsDeleted == false
                              && role.Name == "Admin"
                              select new EmployeeDTO
                              {
                                  Id = emp.Id,
                                  FirstName = emp.FirstName,
                                  LastName = emp.LastName,
                                  Email = emp.Email,
                                  PhoneNumber = emp.PhoneNumber,
                                  Address = emp.Address,
                                  userId = emp.UserId,
                                  CreatedBy = emp.CreatedBy,
                                  CreareAt = emp.CreatedAt,
                              }).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while fetching admin list of organization " + ex.Message);
            }
        }
    }
}
