﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillUp.DAL.Context;
using SkillUp.Entity.Entities;

namespace SkillUp.Web.Areas.InstructorPanel.ViewComponents
{
    public class InstructorInfoViewComponent:ViewComponent
    {
        readonly UserManager<Instructor> _userManager;
        readonly AppDbContext _context;

        public InstructorInfoViewComponent(UserManager<Instructor> userManageer, AppDbContext context)
        {
            _userManager = userManageer;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string id = _userManager.GetUserId(HttpContext.User);
            Instructor instructor = await _context.Instructors.FindAsync(id);
            return View(instructor);
        }
    }
}