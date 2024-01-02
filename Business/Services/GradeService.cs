using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IGradeService
    {
        IQueryable<GradeModel> Query();
        Result Add(GradeModel model);
        Result Update(GradeModel model);
        Result Delete(int id);
    }

    public class GradeService : IGradeService
    {
        private readonly Db _db;

        public GradeService(Db db)
        {
            _db = db;
        }

        public IQueryable<GradeModel> Query()
        {
            return _db.Grades.Include(r => r.Students).OrderBy(r => r.Name).Select(r => new GradeModel()
            {
                // model - entity property assignments
                Id = r.Id,
                Name = r.Name,

                // modified model - entity property assignments for displaying in views
                StudentCountOutput = r.Students.Count // display the user count for each role
            });
        }

        public Result Add(GradeModel model)
        {
            // Way 1: may cause problems for Turkish characters such as İ, i, I and ı
            //if (_db.Roles.Any(r => r.Name.ToLower() == model.Name.ToLower().Trim()))
            //    return new ErrorResult("Role with the same name already exists!");
            // Way 2: one of the correct ways for checking string data without any problems for Turkish characters,
            // works correct for English culture,
            // since Entity Framework is built on ADO.NET, we can run SQL commands directly as below in the database
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim()); // using a parameter prevents SQL Injection
            // we provide SQL parameters to the SQL query as the second and rest parameters for the FromSqlRaw method
            // according to their usage order in the SQL query
            var query = _db.Grades.FromSqlRaw("select * from Grade where LOWER(Name) = LOWER(@name)", nameSqlParameter);
            if (query.Any()) // if there are any results for the query above
                return new ErrorResult("Grade with the same name already exists!");

            var entity = new Grade()
            {
                Name = model.Name.Trim()
            };
            _db.Grades.Add(entity);
            _db.SaveChanges();
            return new SuccessResult("Grade added successfully.");
        }

        public Result Update(GradeModel model)
        {
            // Way 1: may cause problems for Turkish characters such as İ, i, I and ı
            //if (_db.Roles.Any(r => r.Name.ToLower() == model.Name.ToLower().Trim() && r.Id != model.Id))
            //    return new ErrorResult("Role with the same name already exists!");
            // Way 2: one of the correct ways for checking string data without any problems for Turkish characters,
            // works correct for English culture,
            // since Entity Framework is built on ADO.NET, we can run SQL commands directly as below in the database
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim()); // using a parameter prevents SQL Injection
            var idSqlParameter = new SqlParameter("id", model.Id);
            // we provide SQL parameters to the SQL query as the second and rest parameters for the FromSqlRaw method
            // according to their usage order in the SQL query
            var query = _db.Grades.FromSqlRaw("select * from Grades where LOWER(Name) = LOWER(@name) and Id != @id", nameSqlParameter, idSqlParameter);
            if (query.Any()) // if there are any results for the query above
                return new ErrorResult("Grade with the same name already exists!");

            // Way 1: retreiving entity from the related database table and updating its properties
            //var entity = _db.Roles.Find(model.Id); // SingleOrDefault can also be used
            //if (entity is null)
            //    return new ErrorResult("Role not found!");
            //entity.Name = model.Name.Trim();
            // Way 2: creating an entity with the model id and setting its properties
            var entity = new Grade()
            {
                Id = model.Id, // must be set
                Name = model.Name.Trim()
            };

            // then updating the entity in the related database table
            _db.Grades.Update(entity);
            _db.SaveChanges();
            return new SuccessResult("Grade updated successfully.");
        }

        public Result Delete(int id)
        {
            // getting the role entity with relational user entities by role id from the related database table
            var existingEntity = _db.Grades.Include(r => r.Students).SingleOrDefault(r => r.Id == id);
            if (existingEntity is null)
                return new ErrorResult("Grade not found!");

            // checking if the role entity has relational users, if it has, we should not delete the role entity
            // Way 1:
            //if (existingEntity.Users.Count > 0)
            //    return new ErrorResult("Role can't be deleted because it has users!");
            // Way 2:
            if (existingEntity.Students.Any())
                return new ErrorResult("Grade can't be deleted because it has users!");

            // since there is no relational user entities of the role entity, we can delete it
            _db.Grades.Remove(existingEntity);
            _db.SaveChanges();
            return new SuccessResult("Grade deleted successfully.");
        }





    }
}
