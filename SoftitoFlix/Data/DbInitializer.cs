using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Models;
using System.Security.Claims;

namespace SoftitoFlix.Data
{
    public class DbInitializer
    {
        public DbInitializer(SoftitoFlixContext? context, RoleManager<ApplicationRole>? roleManager  , UserManager<ApplicationUser>? userManager)
        {
            Restriction restriction;
            ApplicationUser applicationUser;
            ApplicationRole applicationRole;
            Plan plan;
            UserPlan userPlan;

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

                if(context.Plans!.Count()==0)
                {
                    plan = new Plan();
                    plan.Name = "Admin";
                    plan.Price = 999999999999999999;
                    plan.Passive = true;
                    context.Plans.Add(plan);

                    plan = new Plan();
                    plan.Name = "VisitorPlan";
                    plan.Price = 0;
                    plan.Passive = false;
                    context.Plans.Add(plan);

                    plan = new Plan();
                    plan.Name = "SmallPlan";
                    plan.Price = 100;
                    plan.Passive = false;
                    context.Plans.Add(plan);

                    plan = new Plan();
                    plan.Name = "MediumPlan";
                    plan.Price = 200;
                    plan.Passive = false;
                    context.Plans.Add(plan);

                    plan = new Plan();
                    plan.Name = "BigPlan";
                    plan.Price = 300;
                    plan.Passive = false;
                    context.Plans.Add(plan);
                }

                context.SaveChanges();

                if(roleManager != null)
                {
                    if(roleManager.Roles.Count() == 0)
                    {
                        applicationRole = new ApplicationRole("Administrator");
                        roleManager.CreateAsync(applicationRole).Wait();

                        applicationRole = new ApplicationRole("VisitorMember");
                        roleManager.CreateAsync(applicationRole).Wait();

                        applicationRole = new ApplicationRole("SmallPartner");
                        roleManager.CreateAsync(applicationRole).Wait();

                        applicationRole = new ApplicationRole("MediumPartner");
                        roleManager.CreateAsync(applicationRole).Wait();

                        applicationRole = new ApplicationRole("BigPartner");
                        roleManager.CreateAsync(applicationRole).Wait();
                    }
                    
                }

                if(userManager != null)
                {
                    if(userManager.Users.Count() == 0)
                    {
                        applicationUser = new ApplicationUser();
                        applicationUser.BirthDate = new DateTime(1900, 01, 01);
                        applicationUser.Email = "admin@gmail.com";
                        applicationUser.PhoneNumber = "2122121212";
                        applicationUser.UserName = "admin";
                        applicationUser.FullName = "admin";
                        applicationUser.Passive = false;

                        userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                        userManager.AddToRoleAsync(applicationUser,"Administrator").Wait();

                        Claim claim;

                        claim = new Claim("BirthDate", applicationUser.BirthDate.ToString(), ClaimValueTypes.Date);
                        userManager.AddClaimAsync(applicationUser, claim).Wait();

                        claim = new Claim("Id", applicationUser.Id.ToString());
                        userManager.AddClaimAsync(applicationUser, claim).Wait();
                    }
                }

                if (context.UserPlans.Count() == 0)
                {
                    userPlan = new UserPlan();
                    userPlan.PlanId = 1;
                    userPlan.UserId = 1;
                    userPlan.StartDate = DateTime.Now;
                    userPlan.EndDate = DateTime.MaxValue;

                    context.UserPlans.Add(userPlan);
                }

                context.SaveChanges();
            }
        }
    }
}
