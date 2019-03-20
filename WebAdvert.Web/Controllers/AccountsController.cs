using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Runtime.Internal.Transform;
using System.Threading;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager,UserManager<CognitoUser> userManager,CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }
        // GET: Accounts
        public async Task<IActionResult> Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignUpModel model)
        {
            if(ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if(user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User Already Exists");
                    return View(model);
                }
                //var nameAttribute = new Amazon.CognitoIdentityProvider.Model.AttributeType
                //{
                //    Name = "name",
                //    Value = model.Email
                //};
                user.Attributes.Add("name", model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password);

                if (createdUser.Succeeded)
                    RedirectToAction(nameof(Confirm));
            }
            return View();
        }

        public async Task<IActionResult> Confirm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Not Found", "User with email doesn't exist");
                    return View(model);
                }
                var confirmResult = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);
                
                if(confirmResult.Succeeded)
                    return RedirectToAction("Index", "Home");

                else
                {
                    foreach(var item in confirmResult.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }

                    return View(model);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                //var user = await _userManager.FindByEmailAsync(model.Email);
                ////var code = await _userManager.GenerateTwoFactorTokenAsync(user,"");
                //var isValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                //if(isValid.Succeeded)
                //{
                //    var result = await _signInManager.sig(user);
                //    return RedirectToAction("Index", "Home");
                //}
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                else
                {
                    ModelState.AddModelError("LoginError", "Email or password do not match");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Not Found", "User with email doesn't exist");
                    return View(model);
                }
                await user.ForgotPasswordAsync();
                return RedirectToAction(nameof(ConfirmForgotPassword));
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmForgotPassword(ConfirmForgotPassword model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Not Found", "User with email doesn't exist");
                    return View(model);
                }
                await user.ConfirmForgotPasswordAsync(model.Code, model.Password);
                //await _pool.ConfirmForgotPassword(model.Email, model.Code, model.Password, default(CancellationToken));

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View(model);
        }

        public async Task<IActionResult> Signout()
        {
            if (User.Identity.IsAuthenticated) await _signInManager.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Login");
        }
    }
}