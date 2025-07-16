using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services.Redis;


namespace Core;

public static class CoreExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services,IConfiguration configuration)
    
    {
        services.AddAutoMapper(typeof(CoreExtensions).Assembly);

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUserActions, UserActionsService>();
        services.AddScoped<IFileUpload, FileUploadService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRecommender, Recommender>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICart, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserCreateConsumer>();
            x.AddConsumer<ResetPasswordEmailConsumer>();
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderInvoiceConsumer>();
            x.AddConsumer<UpdateTotalReviewsAndRatingOfBookConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {


                cfg.Host(configuration.GetConnectionString("RabbitMQ")!);

                cfg.ReceiveEndpoint("user-created-event", e =>
                {
                    e.ConfigureConsumer<UserCreateConsumer>(context);
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("reset-password-email-event", e =>
                {
                    e.ConfigureConsumer<ResetPasswordEmailConsumer>(context);
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("order-create-email", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("order-invoice", e =>
                {
                    e.ConfigureConsumer<OrderInvoiceConsumer>(context);
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("update-book-totalReviews-rating", e =>
                {
                    e.ConfigureConsumer<OrderInvoiceConsumer>(context);
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ConfigureEndpoints(context);
            });
        });



        Stripe.StripeConfiguration.ApiKey = configuration["Stripe:Secretkey"];

        return services;
    }
}
