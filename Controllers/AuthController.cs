using AgriEnergy.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                HttpContext.Session.SetString("currentUser", currentUserId);


                if (!string.IsNullOrEmpty(currentUserId))
                {
                    // Get the user from the database based on UserUid
                    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserUid == currentUserId);

                    if (user != null)
                    {
                        if (user.UserRole == 0)
                        {
                            // Redirect to Employee action in Product controller
                            return RedirectToAction("Employee", "Product");
                        }
                        else if (user.UserRole == 1)
                        {
                            // Redirect to Index action in Product controller
                            return RedirectToAction("Index", "Product");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                ModelState.AddModelError(string.Empty, firebaseEx.error.message);
            }

            // If user role or login fails, return to login view
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
                    return RedirectToAction("Employee", "Product");
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
