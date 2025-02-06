using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using EmployeeManagement.MVCFramework.Models;
using EmployeeManagement.MVCFramework.Models.View_Model;

namespace EmployeeManagement.MVCFramework.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        

        public AccountController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString);
            
        }
        public ActionResult Login()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var loginObj = new { Email = model.Email, Password = model.Password, RememberMe = model.RememberMe };
                var response = await _httpClient.PostAsJsonAsync("/api/account/login", loginObj);
                
                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content.ReadAsAsync<AuthApiResponse>().Result.Token;
                    Session["AuthToken"] = token;
                    Console.WriteLine("TOKEN INSIDE SESSION ---> " + token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var registerObj = new { Email = model.Email, Password = model.Password, UserName = model.Email };
                var response = await _httpClient.PostAsJsonAsync("api/account/register", registerObj);
                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content.ReadAsAsync<AuthApiResponse>().Result.Token;
                    Session["AuthToken"] = token;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Register Failed");
                }
            }
            return View(model) ;
        }
        [HttpGet]
        
        public ActionResult Logout()
        {
            Session["AuthToken"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}