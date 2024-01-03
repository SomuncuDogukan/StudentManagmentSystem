using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace Business.Services;

/// <summary>
/// Performs user CRUD operations.
/// </summary>
public interface IStudentService
{
    // method definitions: method definitions must be created here in order to be used in the related controller

    /// <summary>
    /// Queries the records in the Users table.
    /// </summary>
    /// <returns></returns>
    IQueryable<StudentModel> Query();

    Results.Bases.Result Add(StudentModel model);
    Results.Bases.Result Update(StudentModel model);

    [Obsolete("Do not use this method anymore, use DeleteUser method instead!")]
    Results.Bases.Result Delete(int id);

    Results.Bases.Result DeleteStudent(int id);
}

public class StudentService : IStudentService // UserService is a IUserService (UserService implements IUserService)
{
    #region Db Constructor Injection
    private readonly Db _db;

    // An object of type Db which inherits from DbContext class is
    // injected to this class through the constructor therefore
    // user CRUD and other operations can be performed with this object.
    public StudentService(Db db)
    {
        _db = db;
    }
    #endregion

    // method implementations of the method definitions in the interface
    public IQueryable<StudentModel> Query()
    {
        // Query method will be used for generating SQL queries without executing them.
        // From the Users DbSet first order records by IsActive data descending
        // then for records with same IsActive data order UserName ascending
        // then for each element in the User entity collection map user entity
        // properties to the desired user model properties (projection) and return the query.
        // In Entity Framework Core, lazy loading (loading related data automatically without the need to include it) 
        // is not active by default if projection is not used. To use eager loading (loading related data 
        // on-demand with include), you can write the desired related entity property on the DbSet retrieved from 
        // the _db using the Include method either through a lambda expression or a string. If you want to include 
        // the related entity property of the included entity, you should write it through a delegate of type
        // included entity in the ThenInclude method. However, if the ThenInclude method is to be used, 
        // a lambda expression should be used in the Include method.
        return _db.Students.Include(e => e.Grade).OrderByDescending(e => e.StudentName)
            .Select(e => new StudentModel()
            {
                // model - entity property assignments
                Id = e.Id,
                IsActive = e.IsActive,
                Password = e.Password,
                GradeId = e.GradeId,
                StudentName = e.StudentName,

                // modified model - entity property assignments for displaying in views
                IsActiveOutput = e.IsActive ? "Yes" : "No",
                GradeNameOutput = e.Grade.Name,
            });
    }

    public Results.Bases.Result Add(StudentModel model)
    {

        List<Student> existingUsers = _db.Students.ToList();
        if (existingUsers.Any(u => u.StudentName.Equals(model.StudentName.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("User with the same user name already exists!");

        // entity creation from the model
        Student studentEntity = new Student()
        {
            IsActive = model.IsActive,
            StudentName = model.StudentName.Trim(),
            Password = model.Password.Trim(),


            GradeId = model.GradeId

        };

        // adding entity to the related db set
        _db.Students.Add(studentEntity);

        // changes in all of the db sets are commited to the database with Unit of Work
        _db.SaveChanges();

        return new SuccessResult("Student added successfully.");
    }

    public Results.Bases.Result Update(StudentModel model)
    {
        // Checking if the user other than the user to be updated (checking by id) with the same user name exists in the database table:
        // Way 1: may cause problems for Turkish characters such as İ, i, I and ı
        //if (_db.Users.Any(u => u.UserName.ToLower() == model.UserName.ToLower().Trim() && u.Id != model.Id))
        //    return new ErrorResult("User with the same user name already exists!");
        // Way 2: not effective since we are getting all the users other than the user with the model id from the database table
        // but we can use StringComparison for the correct case insensitive data equality, StringComparison can also be used in
        // Contains, StartsWith and EndsWith methods
        var existingUsers = _db.Students.Where(u => u.Id != model.Id).ToList();
        if (existingUsers.Any(u => u.StudentName.Equals(model.StudentName.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("Student with the same user name already exists!");

        // first getting the user entity to be updated from the db set
        var userEntity = _db.Students.SingleOrDefault(u => u.Id == model.Id);

        // then checking if the user entity exists
        if (userEntity is null)
            return new ErrorResult("Student not found!");

        // then updating the user entity properties
        userEntity.IsActive = model.IsActive;
        userEntity.StudentName = model.StudentName.Trim();
        userEntity.Password = model.Password.Trim();
        userEntity.GradeId = model.GradeId;
        //userEntity.Status = model.Status;

        // updating the user entity in the related db set
        _db.Students.Update(userEntity);

        // changes in all of the db sets are commited to the database with Unit of Work
        _db.SaveChanges();

        return new SuccessResult("Student updated successfully.");
    }

    // Way 1:
    public Results.Bases.Result Delete(int id)
    {
       
        var userResourceEntities = _db.StudentCourses.Where(ur => ur.StudentId == id).ToList();

        _db.StudentCourses.RemoveRange(userResourceEntities);

        // deleting the user record:
        var userEntity = _db.Students.SingleOrDefault(u => u.Id == id);
        if (userEntity is null)
            return new ErrorResult("User not found!");
        _db.Students.Remove(userEntity);

        _db.SaveChanges(); 

        return new SuccessResult("User deleted successfully.");
    }

 
    public Results.Bases.Result DeleteStudent(int id)
    {
        // getting the user record joined with the user resources records
        var userEntity = _db.Students.Include(u => u.StudentCourses).SingleOrDefault(u => u.Id == id);
        if (userEntity is null)
            return new ErrorResult("User not found!");

       
        _db.StudentCourses.RemoveRange(userEntity.StudentCourses);

       
        _db.Students.Remove(userEntity);

        _db.SaveChanges(); 

        return new SuccessResult("Student deleted successfully.");
    }
}
