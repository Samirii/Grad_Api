using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Grad_Api.Data; // Where your ApiUser is defined
using Microsoft.Extensions.Logging;

namespace Grad_Api.Initializers
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<GradProjDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApiUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = services.GetRequiredService<ILogger<DbInitializer>>();

                // Apply pending migrations
                await context.Database.MigrateAsync();

                await SeedRoles(roleManager, logger);
                await SeedUsers(userManager, logger);
                await SeedUserRoles(context, logger);
                await SeedCourseCategories(context, logger);
                await SeedSubjects(context, logger);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database");
            }
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            // Student Role
            if (!await roleManager.RoleExistsAsync("Student"))
            {
                var result = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Student",
                    NormalizedName = "STUDENT",
                    Id = "0cfbe64f-8d26-4b72-8a66-4655477ffdb1",
                    ConcurrencyStamp = "0cfbe64f-8d26-4b72-8a66-4655477ffdb1"
                });
                if (result.Succeeded) logger.LogInformation("Created Student role");
            }

            // Administrator Role
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var result = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = "40247282-b48e-4140-a75c-48a104e3ba27",
                    ConcurrencyStamp = "40247282-b48e-4140-a75c-48a104e3ba27"
                });
                if (result.Succeeded) logger.LogInformation("Created Administrator role");
            }

            // Teacher Role
            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                var result = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Teacher",
                    NormalizedName = "TEACHER",
                    Id = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3",
                    ConcurrencyStamp = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3"
                });
                if (result.Succeeded) logger.LogInformation("Created Teacher role");
            }
        }

        private static async Task SeedUsers(UserManager<ApiUser> userManager, ILogger logger)
        {
            // Admin User
            if (await userManager.FindByIdAsync("a566bd1e-536d-4a83-96d4-ac3824012fc3") == null)
            {
                var admin = new ApiUser
                {
                    Id = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
                    Email = "admin@samir.com",
                    NormalizedEmail = "ADMIN@SAMIR.COM",
                    UserName = "admin@samir.com",
                    NormalizedUserName = "ADMIN@SAMIR.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    SecurityStamp = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
                    ConcurrencyStamp = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
                    LockoutEnabled = true
                };

                var result = await userManager.CreateAsync(admin, "P@ssword1");
                if (result.Succeeded) logger.LogInformation("Created Admin user");
            }

            // Teacher User
            if (await userManager.FindByIdAsync("0cdff28c-16b3-406c-8660-7deac47f3545") == null)
            {
                var teacher = new ApiUser
                {
                    Id = "0cdff28c-16b3-406c-8660-7deac47f3545",
                    Email = "teacher@samir.com",
                    NormalizedEmail = "TEACHER@SAMIR.COM",
                    UserName = "teacher@samir.com",
                    NormalizedUserName = "TEACHER@SAMIR.COM",
                    FirstName = "System",
                    LastName = "Teacher",
                    EmailConfirmed = true,
                    SecurityStamp = "0cdff28c-16b3-406c-8660-7deac47f3545",
                    ConcurrencyStamp = "0cdff28c-16b3-406c-8660-7deac47f3545",
                    LockoutEnabled = true
                };

                var result = await userManager.CreateAsync(teacher, "P@ssword1");
                if (result.Succeeded) logger.LogInformation("Created Teacher user");
            }

            // Student User
            if (await userManager.FindByIdAsync("26d2fc9b-2420-4bd4-bc15-402364f3c489") == null)
            {
                var student = new ApiUser
                {
                    Id = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
                    Email = "student@samir.com",
                    NormalizedEmail = "STUDENT@SAMIR.COM",
                    UserName = "student@samir.com",
                    NormalizedUserName = "STUDENT@SAMIR.COM",
                    FirstName = "System",
                    LastName = "Student",
                    EmailConfirmed = true,
                    SecurityStamp = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
                    ConcurrencyStamp = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
                    LockoutEnabled = true
                };

                var result = await userManager.CreateAsync(student, "P@ssword1");
                if (result.Succeeded) logger.LogInformation("Created Student user");
            }
        }

        private static async Task SeedUserRoles(GradProjDbContext context, ILogger logger)
        {
            // Admin role assignment
            if (!await context.UserRoles.AnyAsync(ur =>
                ur.UserId == "a566bd1e-536d-4a83-96d4-ac3824012fc3" &&
                ur.RoleId == "40247282-b48e-4140-a75c-48a104e3ba27"))
            {
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
                    RoleId = "40247282-b48e-4140-a75c-48a104e3ba27"
                });
                await context.SaveChangesAsync();
                logger.LogInformation("Assigned Admin role to Admin user");
            }

            // Teacher role assignment
            if (!await context.UserRoles.AnyAsync(ur =>
                ur.UserId == "0cdff28c-16b3-406c-8660-7deac47f3545" &&
                ur.RoleId == "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3"))
            {
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = "0cdff28c-16b3-406c-8660-7deac47f3545",
                    RoleId = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3"
                });
                await context.SaveChangesAsync();
                logger.LogInformation("Assigned Teacher role to Teacher user");
            }

            // Student role assignment
            if (!await context.UserRoles.AnyAsync(ur =>
                ur.UserId == "26d2fc9b-2420-4bd4-bc15-402364f3c489" &&
                ur.RoleId == "0cfbe64f-8d26-4b72-8a66-4655477ffdb1"))
            {
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
                    RoleId = "0cfbe64f-8d26-4b72-8a66-4655477ffdb1"
                });
                await context.SaveChangesAsync();
                logger.LogInformation("Assigned Student role to Student user");
            }
        }

        private static async Task SeedCourseCategories(GradProjDbContext context, ILogger logger)
        {
            if (!context.CourseCategories.Any())
            {
                var categories = new List<CourseCategory>
                {
                    new CourseCategory { Name = "prep1" },
                    new CourseCategory {  Name = "prep2" },
                    new CourseCategory {Name = "prep3" }
                };

                await context.CourseCategories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
                logger.LogInformation("Created course categories");
            }
        }

        private static async Task SeedSubjects(GradProjDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Starting to seed subjects...");
                
                if (!context.Subjects.Any())
                {
                    logger.LogInformation("No subjects found in database. Seeding subjects...");
                    
                    var subjects = new List<Subject>
                    {
                        new Subject {  Name = "Math" },
                        new Subject {  Name = "Science" },
                        new Subject { Name = "English" },
                      
                    };

                    await context.Subjects.AddRangeAsync(subjects);
                    var result = await context.SaveChangesAsync();
                    
                    logger.LogInformation($"Successfully seeded {result} subjects");
                }
                else
                {
                    logger.LogInformation("Subjects already exist in database. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding subjects");
                throw;
            }
        }
    }
}
