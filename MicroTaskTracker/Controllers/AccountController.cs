using Microsoft.AspNetCore.Mvc;
using MicroTaskTracker.Models.ViewModels.Authentication;

namespace MicroTaskTracker.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register(SignInViewModel signInViewModel)
        {
            // Registration logic here
            return View();
        }
        [HttpPost]
        public IActionResult Register(SignInViewModel signInViewModel,string email, string password)
        {
            // Registration logic here
            return View();
        }
        [HttpGet]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            // Login logic here
            return View();
        }
        [HttpGet]
        public IActionResult Login(LoginViewModel loginViewModel,string email, string password)
        {
            // Login logic here
            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Logout logic here
            return View();
        }
    }
}
