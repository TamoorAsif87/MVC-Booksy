using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seed;

public class SeedData()
{
    public static async Task Seed(StoreContext context,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
    {
        if(!context.Database.GetPendingMigrations().Any())
        {
            if(!context.ApplicationUsers.Any())
            {
                //serialize json of users
                var contentFile = await File.ReadAllTextAsync("../../Infrastructure/Infrastructure/Data/Seed/SeedFiles/users.json");
                var users = JsonSerializer.Deserialize<List<ApplicationUser>>(contentFile);

                // create role user if not exist
                if (!await roleManager.RoleExistsAsync("user"))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = "user"});
                }

                if (!await roleManager.RoleExistsAsync("admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = "admin" });
                }

                // password

                var password = "Sahiwal90@";

               

                // add user to database

                foreach (var user in users!)
                {
                    var result =  await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        if(user.UserName == "tamoor.asif@example.com")
                        {
                            await userManager.AddToRoleAsync(user, "admin");
                        }
                        else
                        {
                            await userManager.AddToRoleAsync(user, "user");
                        }

                            
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error creating user {user.UserName}: {error.Description}");
                        }
                    }
                }

                
            }

            if (!context.Categories.Any())
            {
                var contentFile = await File.ReadAllTextAsync("../../Infrastructure/Infrastructure/Data/Seed/SeedFiles/category.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(contentFile);

                await context.Categories.AddRangeAsync(categories!);
                await context.SaveChangesAsync();
            }

            if (!context.CategoryTranslations.Any())
            {
                var contentFile = await File.ReadAllTextAsync("../../Infrastructure/Infrastructure/Data/Seed/SeedFiles/category-translation.json");
                var categoriesTranslations = JsonSerializer.Deserialize<List<CategoryTranslation>>(contentFile);

                if (context.Categories.Any())
                {
                    await context.CategoryTranslations.AddRangeAsync(categoriesTranslations!);
                    await context.SaveChangesAsync();
                }
            }


            if (!context.Books.Any())
            {
                var contentFile = await File.ReadAllTextAsync("../../Infrastructure/Infrastructure/Data/Seed/SeedFiles/books.json");
                var books = JsonSerializer.Deserialize<List<Book>>(contentFile);

                await context.Books.AddRangeAsync(books!);
                await context.SaveChangesAsync();
            }


            if (!context.UserProfiles.Any() && context.ApplicationUsers.Any())
            {
                var contentFile = await File.ReadAllTextAsync("../../Infrastructure/Infrastructure/Data/Seed/SeedFiles/userProfiles.json");
                var userProfiles = JsonSerializer.Deserialize<List<UserProfile>>(contentFile);
                await context.UserProfiles.AddRangeAsync(userProfiles!);
                await context.SaveChangesAsync();
            }
        }  
    }
}
