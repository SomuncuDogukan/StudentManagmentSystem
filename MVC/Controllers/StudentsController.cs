﻿#nullable disable
using Business;
using Business.Models;
using Business.Results.Bases;
using Business.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using DataAccess.Entities;

//Generated from Custom Template.
namespace MVC.Controllers
{
    public class StudentsController : Controller
    {
        // Add service injections here
        #region Students and Grade Service Constructor Injections
        private readonly IStudentService _studentService;
        private readonly IGradeService _gradeService;

        // Objects of type UserService and RoleService which are implemented from the IUserService
        // and IRoleService interfaces are injected to this class through the constructor therefore
        // user and role CRUD and other operations can be performed with these objects.
        public StudentsController(IStudentService studentService, IGradeService gradeService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
        }
        #endregion

        // GET: Users/GetList
        public IActionResult Index()
        {

            if (!User.HasClaim("Student", "true"))
            {
                // If the user is not a director, redirect to the index page
                return RedirectToAction("Login", "Students"); // Change "Home" to the appropriate controller
            }
            List<StudentModel> studentList = _studentService.Query().ToList();
            // return View("Index", studentList); // model will be passed to the List view under Views/Users folder
            return View(studentList); 
        }

        // Returning user list in JSON format:
        // GET: Users/GetListJson
        public JsonResult GetListJson()
        {


            var studentList = _studentService.Query().ToList();
            return Json(studentList);
        }

        // GET: Users/Details/5
        public IActionResult Details(int id)
        {
            // Way 1:
            //UserModel user = _userService.Query().FirstOrDefault(u => u.Id == id);
            // Way 2:
            //UserModel user = _userService.Query().LastOrDefault(u => u.Id == id);
            // Way 3:
            StudentModel student = _studentService.Query().SingleOrDefault(u => u.Id == id);
            // The SingleOrDefault method, when used with a lambda expression, returns a single element (record) 
            // based on the specified condition. If the query returns multiple elements, it throws an exception, 
            // and if no elements match the condition, it returns a null reference.
            // You can use Single instead of SingleOrDefault and it throws an exception if multiple elements 
            // match the condition or if no elements are found.
            // Similarly, you can use FirstOrDefault instead of SingleOrDefault. When using a lambda expression, 
            // it returns the first element that matches the condition whether there are multiple matching elements or not. 
            // If no elements are found, it returns a null reference.
            // You can also use First instead of FirstOrDefault, and it throws an exception if no elements match the condition.
            // The LastOrDefault and Last methods perform operations on the last element found based on the specified condition, 
            // which can be considered as the reverse of FirstOrDefault and First.
            // Generally, methods ending with OrDefault that return a null result when no elements are found are used 
            // when dealing with a situation where no match is expected.

            if (student == null)
            {
                return NotFound(); // returns 404 HTTP not found status code
            }

            return View(student); // send user of type UserModel to the Views/Users/Details view
        }

        // GET: Users/Create
        [HttpGet] // action method which is get by default when not written
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items

            // Creation of a SelectList object with parameters in order as role list, value member of each element
            // to be used in the background through related model property name (Id) and display member of each element
            // to be shown to the user through related model property name (Name) and assignment to the
            // ViewData through the Roles key.
            // Way 1 ViewData:
            //ViewData["Roles"] = new SelectList(_roleService.Query().ToList(), "Id", "Name");
            // Way 2: ViewBag which is the same object as ViewData
            ViewBag.Grades = new SelectList(_gradeService.Query().ToList(), "Id", "Name");

            return View(); // returning Views/Users/Create view with no model data
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // action method which is used for processing request data sent by a form or AJAX
        [ValidateAntiForgeryToken] 


       
        public IActionResult Create(StudentModel student) // since UserModel has properties for above parameters, it should be used as action parameter
        {
            if (ModelState.IsValid) // validates the user action parameter (model) through data annotations above its properties
            {
                // If model data is valid, insert service logic should be written here.
                Result result = _studentService.Add(student); // result referenced object can be of type SuccessResult or ErrorResult
                if (result.IsSuccessful)
                {
                    // Way 1:
                    //return RedirectToAction("GetList");
                    // Way 2:
                    TempData["Message"] = result.Message; // if there is a redirection, the data should be carried with TempData to the redirected action's view
                    return RedirectToAction(nameof(Index)); // redirection to the action specified of this controller to get the updated list from database
                }

                // Way 1:  carrying data from the action with ViewData
                //ViewBag.Message = result.Message; // ViewData["Message"] = result.Message;
                // Way 2: sends data to view's validation summary
                ModelState.AddModelError("", result.Message);

            }

            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            // Way 1: SelectList constructor last parameter is the selected value
            //ViewBag.Roles = new SelectList(_roleService.Query().ToList(), "Id", "Name", user.RoleId);
            // Way 2:
            ViewBag.Grades = new SelectList(_gradeService.Query().ToList(), "Id", "Name");

            // Returning the model containing the data entered by the user to the view therefore
            // the user can correct validation errors without losing data of the form input elements.
            return View(student);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int id)
        {
            StudentModel student = _studentService.Query().SingleOrDefault(u => u.Id == id); // getting the model from the service
            if (student == null)
            {
                return NotFound(); // 404 HTTP Status Code
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewBag.GradeId = new SelectList(_gradeService.Query().ToList(), "Id", "Name"); // filling the roles
            return View(student); // returning the model to the view so that user can see the data to edit
        }

        // POST: Users/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentModel student)
        {
            if (ModelState.IsValid) // if no validation errors through data annotations of the model
            {
                var result = _studentService.Update(student); // update the user in the service
                if (result.IsSuccessful)
                {
                    // if update operation result is successful, carry successful result message to the List view through the GetList action
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message); // if unsuccessful, carry error result message to the view's validation summary
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewBag.GradeId = new SelectList(_gradeService.Query().ToList(), "Id", "Name"); // filling the roles
            return View(student); // returning the model sent by application user to the view so he/she can correct the validation errors and try again
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int id)
        {
            StudentModel student = _studentService.Query().SingleOrDefault(u => u.Id == id); // getting the model from the service
            if (student == null)
            {
                return NotFound();
            }
            return View(student); // sending the model to the view so application user can see the details of the user
        }

        // POST: Users/Delete
        // ActionName attribute (Delete) renames and overrides the action method name (DeleteConfirmed) 
        // for the route so that it can be requested as not Users/DeleteConfirmed but as Users/Delete. 
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _studentService.DeleteStudent(id);

            // carrying the service result message to the List view through GetList action
            TempData["Message"] = result.Message;

            return RedirectToAction(nameof(Index));
        }


        #region User Authentication
        public IActionResult Login()
        {
            return View(); // returning the Login view to the user for entering the user name and password
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(StudentModel director)
        {
            // checking the active user from the database table by the user name and password
            var existingUser = _studentService.Query().SingleOrDefault(u => u.StudentName == director.StudentName);
            if (existingUser is null) // if an active user with the entered user name and password can't be found in the database table
            {
                ModelState.AddModelError("", "Invalid user name"); // send the invalid message to the view's validation summary 
                return View(); // returning the Login view
            }

            // Creating the claim list that will be hashed in the authentication cookie which will be sent with each request to the web application.
            // Only non-critical user data, which will be generally used in the web application such as user name to show in the views or user role
            // to check if the user is authorized to perform specific actions, should be put in the claim list.
            // Critical data such as password must never be put in the claim list!
            List<Claim> userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, existingUser.StudentName),
                //new Claim(ClaimTypes.Surname, existingUser.Surname),
                new Claim("Student", "true")
            };

            // creating an identity by the claim list and default cookie authentication
            var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            // creating a principal by the identity
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // signing the user in to the MVC web application and returning the hashed authentication cookie to the client
            HttpContext.SignInAsync(userPrincipal);

            // redirecting user to the home page
            return RedirectToAction("Index", "Home");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign the user out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home"); // Redirect to the home page or any other page after logout
        }
        #endregion
    }





}

