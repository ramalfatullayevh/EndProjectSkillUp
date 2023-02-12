﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkillUp.Entity.Entities;
using SkillUp.Entity.Entities.Relations.CourseExtraProperities;
using SkillUp.Entity.ViewModels;
using SkillUp.Service.Helpers;
using SkillUp.Service.Services.Abstractions;
using SkillUp.Service.Services.Concretes;

namespace SkillUp.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CourseController : Controller
    {
        readonly ICourseService _courseService;
        readonly ICategoryService _categoryService;
        readonly IInstructorService _instructorService;

        public CourseController(ICourseService courseService, ICategoryService categoryService, IInstructorService instructorService)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _instructorService = instructorService;
        }

        public async Task<IActionResult> ManageCourses()
        {
            var courses = await _courseService.GetAllCourseAsync();
            return View(courses);
        }

        public async  Task<IActionResult> AddNewCourse()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoryAsync(), nameof(Category.Id), nameof(Category.Name));
            ViewBag.Instructor = new SelectList(await _instructorService.GetAllInstructorAsync(), nameof(Instructor.Id), nameof(Instructor.Name));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCourse(CreateCourseVM courseVM)
        {
            if (courseVM.Image != null)
            {
                string result = courseVM.Image.CheckValidate("image/", 500);
                if (result.Length > 0)
                {
                    ModelState.AddModelError("Image", result);
                }
            }
            if (courseVM.Preview != null)
            {
                string result = courseVM.Preview.CheckValidate("video/", 50000);
                if (result.Length > 0)
                {
                    ModelState.AddModelError("Preview", result);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoryAsync(), nameof(Category.Id), nameof(Category.Name));
                ViewBag.Instructor = new SelectList(await _instructorService.GetAllInstructorAsync(), nameof(Instructor.Id), nameof(Instructor.Name));
                return View(courseVM);
            }
            await _courseService.CreateCourseAsync(courseVM);
            return View();
        }


        public IActionResult Coupons()
        {
            return View();
        }
    }
}
