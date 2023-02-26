﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillUp.Entity.Entities;
using SkillUp.Service.Services.Abstractions;
using SkillUp.Service.Services.Concretes;

namespace SkillUp.Web.Areas.InstructorPanel.Controllers
{
    [Area("InstructorPanel")]
    public class ReportController : Controller
    {
        
        readonly UserManager<Instructor> _userManager;
        readonly IInstructorService _instructorService;

        public ReportController(UserManager<Instructor> userManager, IInstructorService instructorService)
        {
            _userManager = userManager;
            _instructorService = instructorService;
        }


        public async Task<IActionResult> MyCourseRevenue()
        {
            string id = _userManager.GetUserId(HttpContext.User);
            var instructor = await _instructorService.GetInstructorById(id);
            return View(instructor);
        }


        public async Task<IActionResult> MyProductRevenue()
        {
            string id = _userManager.GetUserId(HttpContext.User);
            var instructor = await _instructorService.GetInstructorById(id);
            return View(instructor);
        }
    }
}
