#nullable disable
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

//Generated from Custom Template.
namespace MVC.Controllers
{
    public class GradesController : Controller
    {
        // Add service injections here
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        // GET: Grades

        public IActionResult Index()
        {
            if (!User.HasClaim("Student", "true"))
            {
                // If the user is not a student, redirect to the index page
                return RedirectToAction("Login", "Students"); // Change "Home" to the appropriate controller
            }

            List<GradeModel> gradeList = _gradeService.Query().ToList();
            return View(gradeList);
        }

        // GET: Grades/Details/5
        public IActionResult Details(int id)
        {
            GradeModel grade = _gradeService.Query().SingleOrDefault(r => r.Id == id);
            if (grade == null)
            {
                return NotFound();
            }
            return View(grade);
        }

        // GET: Grades/Create
        public IActionResult Create()
        {


            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GradeModel grade)
        {
            if (ModelState.IsValid)
            {
                var result = _gradeService.Add(grade);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(grade);
        }

        // GET: Grades/Edit/5
        public IActionResult Edit(int id)
        {
            GradeModel grade = _gradeService.Query().SingleOrDefault(r => r.Id == id);
            if (grade == null)
            {
                return NotFound();
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(grade);
        }

        // POST: Grades/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GradeModel grade)
        {
            if (ModelState.IsValid)
            {
                var result = _gradeService.Update(grade);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(grade);
        }

        // GET: Grades/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _gradeService.Delete(id);
            TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
            return RedirectToAction(nameof(Index));
        }


    }
}
