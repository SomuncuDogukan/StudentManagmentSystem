using Business.Services;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region IoC (Inversion of Control) Container
// IoC Container manages the initialization operations of the objects which are
// injected to classes by Constructor Injection. Alternatively Autofac or Ninject
// libraries can also be used under the Business layer.
// "Unable to reslove service..." exceptions should be resolved here.
builder.Services.AddDbContext<Db>(options => options // options used in the AddDbContext method is a delegate
                                                     // of type DbContextOptionsBuilder. This delegate
                                                     // is also called an Action which doesn't return anything.
                                                     // Actions are generally used for configurations.
                                                     // Through the Actions properties or methods
                                                     // (such as UseMySql method) can be used therefore
                                                     // the Actions can provide these to the method
                                                     // which they are used in.
                                                     // We are saying that use MySQL with the provided
                                                     // connection string through the options Action
                                                     // to the AddDbContext method which uses the type of Db,
                                                     // therefore we should provide the type of our class
                                                     // inherited from the DbContext as the generic type
                                                     // for AddDbContext method.
                                                     //.UseMySQL("server=127.0.0.1;database=test;user id=std;password=;")); // // we are going to use Microsoft SQL Server LocalDB from now on
                                                     //.UseSqlServer("server=(localdb)\\mssqllocaldb;database=RMSCTISDB;trusted_connection=true;"));
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // appsettings.json -> ConnectionString section
                                                                                    // Connection string should be read from the ConnectionString section of the appsettings.json file by using the
                                                                                    // connection string name (DefaultConnection).

// AddScoped: The object's reference (usually an interface or abstract class) is used to instantiate an object
// through constructor injection when a request is received and the object lives until the response is returned.
// AddDbContext is a scoped method by default.
// AddSingleton: The object's reference (usually an interface or abstract class) is used to instantiate an object
// through constructor injection when a request is received and the only one object lives throughout
// the application's lifetime (as long as the application is running and not stopped or restarted for example
// via IIS: Internet Information Services or Kestrel web server applications).
// AddTransient: The object is instantiated every time whenever a constructor injection through the
// object's reference (usually an interface or abstract class) is used, independent from the request.
// Generally the AddScoped method is used.
// Way 1:
//builder.Services.AddSingleton<IUserService, UserService>();
// Way 2:
// builder.Services.AddTransient<IUserService, UserService>();
// Way 3:
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IGradeService, GradeService>();
//builder.Services.AddScoped<IResourceService, ResourceService>();
#endregion



var app = builder.Build();














// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
