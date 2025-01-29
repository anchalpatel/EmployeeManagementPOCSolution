using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeManagement.MVCFramework.Models.View_Model;
using EmployeeManagement.MVCFramework.Models;
using EmployeeManagement.MVCFramework.Helpers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Security;

namespace EmployeeManagement.MVCFramework.Controllers
{
    public class HRController : Controller
    {
        private readonly HttpClient _httpClient;

        public HRController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7057/");

        }
        [HttpGet]
        public async Task<ActionResult> ManageHR()
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var organizationId = TokenHelper.GetClaimFromToken(token, "OrganizationId");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Admin/GetAllHR/{organizationId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsAsync<ApiResponse>();
                if (data?.Success == true)
                {
                    var adminList = new List<EmployeeViewModel>();
                    var employeeData = data.Data;

                    foreach (var employee in employeeData)
                    {
                        var employeeViewModel = new EmployeeViewModel
                        {
                            Id = employee?.id,
                            FirstName = employee?.firstName,
                            Address = employee?.address,
                            LastName = employee?.lastName,
                            PhoneNumber = employee?.phoneNumber,
                            Email = employee?.email,
                            CreatedAt = employee?.createdAt,
                            CreatedBy = employee?.createdBy,
                            OrganizationId = employee?.organizationId
                        };
                        adminList.Add(employeeViewModel);
                    }

                    return View(adminList);
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to retrieve HR data.";
                    return View("Error");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
                ViewBag.ErrorMessage = problemDetails?.Title ?? "An error occurred while fetching HR data.";

                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult CreateHR()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateHR(EmployeeViewModel model)
        {
            if (model.Password == null)
            {
                ModelState.AddModelError("", "Password is required.");
            }
            if (ModelState.IsValid)
            {
                var token = Session["AuthToken"]?.ToString();

                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var organizationId = TokenHelper.GetClaimFromToken(token, "OrganizationId");

                if (organizationId == null)
                {
                    
                    ViewBag.ErrorMessage = "Organization ID is required.";
                    return View(model);
                }
                model.OrganizationId = organizationId;
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var jsonData = JsonContent.Create(model);
                try
                {
                    var response = await _httpClient.PostAsync($"api/Admin/CreateHR", jsonData);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<ApiResponse>();
                        if (data?.Success == true)
                        {
                            return RedirectToAction("ManageHR", "HR");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = data?.Message ?? "Failed to create HR.";
                            return View("Error");
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
                        ViewBag.ErrorMessage = "Error: " + errorContent;
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while processing your request: " + ex.Message;
                    return View("Error");
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteHR(int employeeId)
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/Admin/removeHR/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response?.Content?.ReadAsAsync<ApiResponse>();
                if (data?.Success == true)
                {
                    return RedirectToAction("ManageHR", "HR");
                }
                else
                {
                    ViewBag.ErrorMessage = "HR can not be deleted " + data.Message;
                    return View("Error");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
                ViewBag.ErrorMessage = "Error: " + problemDetails;
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult> UpdateHR(int employeeId)
        {
            var token = Session["AuthToken"]?.ToString();
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Employee/GetEmployeeDetails/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsAsync<ApiResponse>();
                if (responseData.Success)
                {
                    var data = responseData?.Data;

                    var admin = new EmployeeUpdateModel()
                    {
                        Id = data?.id,
                        FirstName = data?.firstName,
                        Address = data?.address,
                        LastName = data?.lastName,
                        PhoneNumber = data?.phoneNumber,
                        Email = data?.email,
                        OrganizationId = data?.organizationId,
                        
                    };
                    return View(admin);
                }
                else
                {
                    ViewBag.ErrorMessage = "Could not get employee data: " + responseData.Message;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Could not get employee data";
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateHR(EmployeeUpdateModel model, int organizationId)
        {
            if (ModelState.IsValid)
            {
                var token = Session["AuthToken"]?.ToString();
                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonData = JsonContent.Create(new
                {
                    model.Id,
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.PhoneNumber,
                    model.Address,
                    Roles = model.Role,
                });
                var response = await _httpClient.PutAsync($"api/Employee/UpdateEmployee/{model.Id}", jsonData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                        if (apiResponse?.Success == true)
                        {
                            return RedirectToAction("ManageHR", "HR");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = apiResponse?.Message ?? "An unknown error occurred.";
                            return View("Error");
                        }
                    }
                    catch (JsonSerializationException ex)
                    {
                        ViewBag.ErrorMessage = $"Error deserializing API response: {ex.Message}";
                        return View("Error");
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Error: {response.StatusCode} - {content}";
                    return View("Error");
                }
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine(error.ErrorMessage);
            }

            return View(model);
        }

    }
}