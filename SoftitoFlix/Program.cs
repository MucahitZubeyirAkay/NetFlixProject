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

        builder.Services.AddDbContext<SoftitoFlixContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationWindowsDatabase")));
        builder.Services.AddDefaultIdentity<ApplicationUser>()
            .AddDefaultTokenProviders()
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
        UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
        DbInitializer dBInitializer = new DbInitializer(context, userManager);

        app.Run();
    }
}

