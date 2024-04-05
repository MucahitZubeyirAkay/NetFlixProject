using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Data;
using SoftitoFlix.Models;

namespace SoftitoFlix;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<SoftitoFlixContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<SoftitoFlixContext>();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);//Program çalışıtğında loopa girmesini engelliyor.
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();



        app.MapControllers();

        SoftitoFlixContext? context = app.Services.CreateScope().ServiceProvider.GetService<SoftitoFlixContext>();
        RoleManager<ApplicationRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<ApplicationRole>>();
        UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
        SignInManager<ApplicationUser>? signInManager = app.Services.CreateScope().ServiceProvider.GetService<SignInManager<ApplicationUser>>();
        DbInitializer dBInitializer = new DbInitializer(context, roleManager, userManager);

        app.Run();
    }
}

