using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeManagement.MVCFramework.Models;
using EmployeeManagement.MVCFramework.Models.View_Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmployeeManagement.MVCFramework.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrganizationController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7057/");

        }
        [HttpGet]
        public async Task<ActionResult> GetAllOrganizations()
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Organization/GetAllOrganizations");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsAsync<ApiResponse>();

                if (responseData?.Success == true)
                {
                    // Transform response into a structured list of organizations
                    var organizations = responseData?.Data?.result;

                    var organizationList = new List<OrganizationViewModel>();

                    foreach (var organization in organizations)
                    {
                        
                        var organizationViewModel = new OrganizationViewModel
                        {
                            Id = organization.id,
                            Name = organization.name,
                            Address = organization.address,
                            CreatedAt = organization.createdAt,
                            Employees = new List<EmployeeViewModel>()
                        };

                        // Map the employees
                        foreach (var employee in organization.employees ?? Enumerable.Empty<dynamic>())
                        {
                            var employeeViewModel = new EmployeeViewModel
                            {
                                Id = employee.id,
                                FirstName = employee.firstName,
                                LastName = employee.lastName,
                                Email = employee.email,
                                Address = employee.address,
                                PhoneNumber = employee.phoneNumber,
                                CreatedAt = employee.createdAt
                            };

                            organizationViewModel.Employees.Add(employeeViewModel);
                        }

                        organizationList.Add(organizationViewModel);
                    }

                    return View(organizationList);
                }
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOrganization(int organizationId)
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/Organization/DeleteOrganization/{organizationId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllOrganizations", "Organization");
            }

            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                // Optionally, you could also include the status code for more detail
                ViewBag.ErrorMessage = $"Error occurred while deleting the organization. Status code: {response.StatusCode}. Message: {errorMessage}";

                return View("Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult> UpdateOrganization(int organizationId)
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Organization/GetOrganizationDetails/{organizationId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsAsync<ApiResponse>();
                if (responseData.Success)
                {
                    var data = responseData?.Data?.result;

                    // Map the data to the view model
                    var organizationViewModel = new OrganizationViewModel
                    {
                        Name = data?.name,
                        Address = data?.address,
                    };
                    return View(organizationViewModel);
                }
            }
            
            var errorMessage = await response.Content.ReadAsStringAsync();

            // Optionally, you could also include the status code for more detail
            ViewBag.ErrorMessage = $"Error occurred while deleting the organization. Status code: {response.StatusCode}. Message: {errorMessage}";

            return View("Error");
            
        }
        [HttpPost]
        public async Task<ActionResult> UpdateOrganization(OrganizationViewModel model, int organizationId)
        {
            if (ModelState.IsValid)
            {
                var token = Session["AuthToken"]?.ToString();
                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var orgObj = new { Name = model.Name, Address = model.Address };
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonContent.Create(model);
                var response = await _httpClient.PutAsync($"api/Organization/UpdateOrganization/{organizationId}", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrganizations", "Organization");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = "Error: " + errorMessage;
                    return View("Error");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateOrganization()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrganization(OrganizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = Session["AuthToken"]?.ToString();
                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var orgObj = new { Name = model.Name, Address = model.Address };
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonContent.Create(model);
                var response = await _httpClient.PostAsync("api/Organization/CreateOrganization", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrganizations", "Organization");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = "Error: " + errorMessage;
                    return View("Error");
                }
            }
            return View(model);
        }

    }
}