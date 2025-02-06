using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeManagement.MVCFramework.CustomAttributes;
using EmployeeManagement.MVCFramework.Models;
using EmployeeManagement.MVCFramework.Models.View_Model;
using Newtonsoft.Json;

namespace EmployeeManagement.MVCFramework.Controllers
{
    [TokenValidation]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString);

        }
        [HttpGet]
      
        public async Task<ActionResult> ManageAdmin(int organizationId, string organizationName)
        {
            var token = Session["AuthToken"]?.ToString();
           // if (token == null) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Organization/GetAdmin/{organizationId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsAsync<ApiResponse>();
                if (responseData.Success)
                {
                    var data = responseData?.Data;

                    var adminList = new List<EmployeeViewModel>();  
                    foreach (var employee in data)
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


                    ViewBag.OrganizationName = organizationName;
                    return View(adminList);
                }
            }

            var errorMessage = await response.Content.ReadAsStringAsync();

            
            ViewBag.ErrorMessage = $"Error occurred while deleting the organization. Status code: {response.StatusCode}. Message: {errorMessage}";

            return View("Error");
        }

        [HttpGet]
   
        public ActionResult CreateAdmin(int organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateAdmin(EmployeeViewModel model, int organizationId)
        {
            if (model.Password == null) ModelState.AddModelError("", "Paswword is required");

            if (ModelState.IsValid)
            {
               
                var token = Session["AuthToken"]?.ToString();
                //if (token == null) return RedirectToAction("Login", "Account");
              
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var jsonData = JsonContent.Create(model);
                var response = await _httpClient.PostAsync($"api/Organization/addAdmin/{organizationId}", jsonData);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response?.Content?.ReadAsAsync<ApiResponse>();
                    if(data?.Success == true)
                    {
                        return RedirectToAction("ManageAdmin", "Admin", new {organizationId = organizationId});
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
            return View(model);
        }
      
        public async Task<ActionResult> DeleteAdmin(int employeeId, int organizationId)
        {
            var token = Session["AuthToken"]?.ToString();
            //if (token == null) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.DeleteAsync($"api/Organization/removeAdmin/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response?.Content?.ReadAsAsync<ApiResponse>();
                if (data?.Success == true)
                {
                    return RedirectToAction("ManageAdmin", "Admin", new { organizationId = organizationId });
                }
                else
                {
                    ViewBag.ErrorMessage = "Admin can not be deleted " + data.Message;
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

        [HttpGet]
        public async Task<ActionResult> UpdateAdmin(int employeeId, int organizationId)
        {
            var token = Session["AuthToken"]?.ToString();
            //if (token == null) return RedirectToAction("Login", "Account");

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

                    // Pass organizationId to the view
                    ViewBag.OrganizationId = organizationId;

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
        public async Task<ActionResult> UpdateAdmin(EmployeeUpdateModel model, int organizationId)
        {
            if (ModelState.IsValid)
            {
                var token = Session["AuthToken"]?.ToString();
                //if (token == null)
                //{
                //    return RedirectToAction("Login", "Account");
                //}

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonData = JsonContent.Create(new
                {
                    model.Id,
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.PhoneNumber,
                    model.Address,
                    
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
                            return RedirectToAction("ManageAdmin", "Admin", new { organizationId = organizationId });
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