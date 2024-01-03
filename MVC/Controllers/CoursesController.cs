#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using DataAccess.Entities;
using Business.Models;
using Business.Services;
using Humanizer.Localisation;

//Generated from Custom Template.
namespace MVC.Controllers
{
    public class CoursesController : Controller
    {
        // TODO: Add service injections here
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        public CoursesController(ICourseService courseService, IStudentService studentService)
        {
            _courseService = courseService;

            _studentService = studentService;

        }

        // GET: Courses
        public IActionResult Index()
        {
            List<CourseModel> courseList = _courseService.GetList(); ; // TODO: Add get list service logic here
            return View(courseList);
        }

        // GET: Courses/Details/5
        public IActionResult Details(int id)
        {
            CourseModel course = _courseService.GetItem(id); // TODO: Add get item service logic here
            if (course == null)
            {
                return View("_Error", "Course not found!");
            }
            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {

            ViewBag.Student = new MultiSelectList(_studentService.Query().ToList(), "Id", "StudentName");
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CourseModel course)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert service logic here
                var result = _courseService.Add(course);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }

            ViewBag.Student = new MultiSelectList(_studentService.Query().ToList(), "Id", "StudentName");
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(course);
        }

        // GET: Courses/Edit/5
        public IActionResult Edit(int id)
        {
            CourseModel course = _courseService.GetItem(id); // TODO: Add get item service logic here
            if (course == null)
            {
                return View("_Error", "Course not found!");
            }
            ViewBag.Student = new MultiSelectList(_studentService.Query().ToList(), "Id", "StudentName");

            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(course);
        }

        // POST: Courses/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CourseModel course)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add update service logic here
                // TODO: Add update service logic here
                var result = _courseService.Update(course);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    // Way 1: 
                    //return RedirectToAction(nameof(Index));
                    // Way 2: redirection with route values
                    return RedirectToAction(nameof(Details), new { id = course.Id });
                }
                ModelState.AddModelError("", result.Message);
            }
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(course);
        }

        // GET: Courses/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _courseService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }



    }
}
