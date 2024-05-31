using AgriEnergy.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgriEnergy.Controllers
{
    public class AuthController : Controller
    {
        private readonly AgriEnergyContext _db;
        private readonly HttpClient _httpClient;
        private readonly FirebaseAuthProvider _auth;

        public AuthController(HttpClient httpClient, AgriEnergyContext db)
        {
            _httpClient = httpClient;
            _db = db;
            _auth = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("AgriEnergyFirebase")));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCredentials login)
        {
            try
            {
                var fbAuthLink = await _auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                string currentUserId = fbAuthLink.User.LocalId;

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    HttpContext.Session.SetString("currentUser", currentUserId);
                    return RedirectToAction("Index", "Product");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                ModelState.AddModelError(string.Empty, firebaseEx.error.message);
                return View(login);
            }

            return View(login);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginCredentials login, Models.User user)
        {
            try
            {
                await _auth.CreateUserWithEmailAndPasswordAsync(login.Email, login.Password);

                var fbAuthLink = await _auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                string currentUserId = fbAuthLink.User.LocalId;

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    user.UserUid = currentUserId;
                    user.Name = login.Name;
                    user.Surname = login.Surname;
                    user.UserRole = 1;
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    HttpContext.Session.SetString("currentUser", currentUserId);
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                ModelState.AddModelError(string.Empty, firebaseEx.error.message);
                return View(login);
            }

            return View();
        }
    }
}
