using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Business.Services
{
    public interface ICourseService
    {
        IQueryable<CourseModel> Query();
        Result Add(CourseModel model);
        Result Update(CourseModel model);
        Result Delete(int id);

        // extra method definitions can be added to use in the related controller and
        // to implement to the related classes
        List<CourseModel> GetList();
        CourseModel GetItem(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly Db _db;

        public CourseService(Db db)
        {
            _db = db;
        }

        public IQueryable<CourseModel> Query()
        {
            return _db.Courses.Include(r => r.StudentCourses).Select(r => new CourseModel()
            {
                Course_Content = r.Course_Content,
                Id = r.Id,
                Course_Title = r.Course_Title,

                StudentCountOutput = r.StudentCourses.Count,

                // querying over many to many relationship
                StudentNamesOutput = string.Join("<br />", r.StudentCourses.Select(ur => ur.Student.StudentName)), 
                StudentIdsInput = r.StudentCourses.Select(ur => ur.StudentId).ToList() 
            }).OrderByDescending(r => r.Id).ThenByDescending(r => r.Course_Title);
        }

        public Result Add(CourseModel model)
        {
            
            if (
                _db.Courses.Any(r => ( r.Course_Title.ToUpper() == model.Course_Title.ToUpper().Trim())))
                return new ErrorResult("Course with the same title");

            var entity = new Course()
            {
                // ? can be used if the value of a property can be null,
                // if model.Content is null, Content is set to null, else Content is set to
                // model.Content's trimmed value
                Course_Content = model.Course_Content?.Trim(),

             
                Course_Title = model.Course_Title.Trim(), // since Title is required in the model, therefore can't be null,
                                            // we don't need to use ?

                // inserting many to many relational entity,

                StudentCourses = model.StudentIdsInput?.Select(studentId => new StudentCourse()
                {
                    StudentId = studentId
                }).ToList()
            };

            _db.Courses.Add(entity);
            _db.SaveChanges();

            return new SuccessResult("Course added successfully.");
        }

        public Result Update(CourseModel model)
        {
            if (
                _db.Courses.Any(r => (r.Course_Title.ToUpper() == model.Course_Title.ToUpper().Trim())))
                return new ErrorResult("Course with the same title");

            // deleting many to many relational entity
            var existingEntity = _db.Courses.Include(r => r.StudentCourses).SingleOrDefault(r => r.Id == model.Id);
            if (existingEntity is not null && existingEntity.StudentCourses is not null)
                _db.StudentCourses.RemoveRange(existingEntity.StudentCourses);

            existingEntity.Course_Content = model.Course_Content?.Trim();
            existingEntity.Course_Title = model.Course_Title.Trim();

            // inserting many to many relational entity
            existingEntity.StudentCourses = model.StudentIdsInput?.Select(studentId => new StudentCourse()
            {
                StudentId = studentId
            }).ToList();

            _db.Courses.Update(existingEntity);
            _db.SaveChanges(); // changes in all DbSets are commited to the database by Unit of Work

            return new SuccessResult("Course updated successfully.");
        }

        public Result Delete(int id)
        {
            var entity = _db.Courses.Include(r => r.StudentCourses).SingleOrDefault(r => r.Id == id);
            if (entity is null)
                return new ErrorResult("Course not found!");

            // deleting many to many relational entity:
            _db.StudentCourses.RemoveRange(entity.StudentCourses);

            // then deleting the Course entity
            _db.Courses.Remove(entity);

            _db.SaveChanges();

            return new SuccessResult("Course deleted successfully.");
        }

        public List<CourseModel> GetList()
        {
            // since we wrote the Query method above, we should call it
            // and return the result as a list by calling ToList method
            return Query().ToList();
        }


        public CourseModel GetItem(int id)
        {
            return Query().SingleOrDefault(r => r.Id == id);
        }
    }
}
