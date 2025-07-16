using Infrastructure.Data.Seed;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        

        services.AddDbContext<StoreContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser,IdentityRole>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
        }).AddEntityFrameworkStores<StoreContext>().AddDefaultTokenProviders();

        services.AddSingleton<IConnectionMultiplexer>(c =>
        {
            var configString = configuration.GetSection("Redis")["Configuration"];
            var options = ConfigurationOptions.Parse(configString!, true);
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.Decorate<IProfileRepository, CacheProfileRepository>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.Decorate<IBookRepository, BookCacheRepository>();

        services.AddScoped<ICategoryRepository,CategoryRepository>();
        services.Decorate<ICategoryRepository, CategoryCacheRepository>();


        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.Decorate<IReviewRepository, ReviewCacheRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructureModule(this IApplicationBuilder app)
    {
        InitializeDatabase(app).GetAwaiter().GetResult();

        return app;
    }

    private static async Task InitializeDatabase(IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        var service = scope.ServiceProvider;
        var context = service.GetRequiredService<StoreContext>();
        var roleManagerServices = service.GetRequiredService<RoleManager<IdentityRole>>();
        var userMangerServices = service.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();
        await SeedData.Seed(context,roleManagerServices,userMangerServices);
    }
}
