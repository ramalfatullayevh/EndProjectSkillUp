﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillUp.DAL.Context;
using SkillUp.Entity.Entities;
using SkillUp.Entity.ViewModels;
using Stripe;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SkillUp.Web.Controllers
{
    public class InstructorController : Controller
    {
        readonly AppDbContext appDbContext;

        public InstructorController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IActionResult> FindInstructor( string? query, int page = 1)
        {
                var instructors = await appDbContext.Instructors.Include(iu => iu.AppUserInstructors).ThenInclude(u => u.AppUser)
                .Include(ip => ip.InstructorProfessions).ThenInclude(p => p.Profession).ToListAsync();
            if (query != null)
            {
                var search = instructors.Where(c => c.Name.ToLower().Trim().Contains(query.ToLower().Trim())).ToList();
                IEnumerable<Instructor> pagination = search.Skip((page - 1) * 2).Take(2);
                PaginationVM<Instructor> searchpaginationVM = new PaginationVM<Instructor>
                {
                    MaxPageCount = (int)Math.Ceiling((decimal)search.Count / 2),
                    CurrentPage = page,
                    Items = pagination,
                    Query = query
                };
                return View(searchpaginationVM);
            }
            else
            {
                IEnumerable<Instructor> pagination = instructors.Skip((page - 1) * 2).Take(2);
                PaginationVM<Instructor> paginationVM = new PaginationVM<Instructor>
                {
                    MaxPageCount = (int)Math.Ceiling((decimal) instructors.Count / 2),
                    CurrentPage = page,
                    Items = pagination
                };
                return View(paginationVM);
            }
        }


        public IActionResult InstructorDetail(string id)
        {
            var instructor = appDbContext.Instructors.Include(ip=>ip.InstructorProfessions)
                .ThenInclude(p=>p.Profession).Include(ai=>ai.AppUserInstructors).ThenInclude(a=>a.AppUser)
                .Include(c=>c.Courses).ThenInclude(p=>p.Paragraphs).ThenInclude(l=>l.Lectures).
                Include(c=>c.Courses).ThenInclude(cc=>cc.CourseCategories).ThenInclude(ctg=>ctg.Category).
                Include(c=>c.Courses).ThenInclude(ac=>ac.AppUserCourses).FirstOrDefault(i=>i.Id == id);
            return View(instructor);
        }


    }
}
