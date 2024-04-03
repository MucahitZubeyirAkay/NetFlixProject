using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Models;
using System.Security.Claims;

namespace SoftitoFlix.Data
{
    public class DbInitializer
    {
        public DbInitializer(SoftitoFlixContext? context, UserManager<ApplicationUser>? userManager)
        {
            Restriction restriction;
            ApplicationUser applicationUser;

            if( context != null )
            {
                context.Database.Migrate();
                
                if(context.Restrictions!.Count()==0)
                {
                    restriction = new Restriction();
                    restriction.Id = 0;
                    restriction.Name = "Genel İzleyici";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 7;
                    restriction.Name = "7 yaş ve üzeri";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 14;
                    restriction.Name = "14 yaş ve üzeri";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 18;
                    restriction.Name = "18 yaş ve üzeri";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 19;
                    restriction.Name = "Korku ve şiddet içerir";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 20;
                    restriction.Name = "Cinsellik barındırır";
                    context.Restrictions.Add(restriction);

                    restriction = new Restriction();
                    restriction.Id = 21;
                    restriction.Name = "Olumsuz örnek barındırır";
                    context.Restrictions.Add(restriction);
                }

                context.SaveChanges();

                if(userManager != null)
                {
                    if(userManager.Users.Count() == 0)
                    {
                        applicationUser = new ApplicationUser();
                        applicationUser.BirthDate = new DateTime(2024, 01, 01);
                        applicationUser.Email = "admin@gmail.com";
                        applicationUser.PhoneNumber = "2122121212";
                        applicationUser.UserName = "admin";
                        applicationUser.FullName = "admin";
                        applicationUser.Passive = false;

                        userManager.CreateAsync(applicationUser, "Admin123!").Wait();

                        Claim claim;

                        claim = new Claim("BirthDate", applicationUser.BirthDate.ToString(), ClaimValueTypes.Date);
                        userManager.AddClaimAsync(applicationUser, claim).Wait();
                    }
                }
            }
        }
    }
}
