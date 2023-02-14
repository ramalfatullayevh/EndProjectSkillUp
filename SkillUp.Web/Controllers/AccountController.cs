﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillUp.Entity.Entities;
using SkillUp.Entity.ViewModels;

namespace SkillUp.Web.Controllers
{
    public class AccountController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterVM register)
        {
            if(!ModelState.IsValid) return View(register);  
            AppUser user = await _userManager.FindByNameAsync(register.UserName);
            if(user is not null)
            {
                ModelState.AddModelError("UserName", "UserName already exist");
                return View(register);
            }
            user = new AppUser
            {
                Name = register.Name,
                Surname = register.Surname,
                UserName = register.UserName,
                Email = register.Email,
            };

            var result = await _userManager.CreateAsync(user,register.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return RedirectToAction(nameof(SignIn));
        }


        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginVM login, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(login);
            AppUser user = await _userManager.FindByNameAsync(login.UserName);
            if(user is null)
            {
                ModelState.AddModelError("", "Login or Password is wrong");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login or Password is wrong");
                return View();
            }
            if (returnUrl is not null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();    
            return RedirectToAction("Index", "Home");
        }


        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}
    }
}