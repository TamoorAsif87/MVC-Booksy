using UI.Exceptions;

var dllPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dll");
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(dllPath);

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");

var supportedCultures = new[] { "en-US", "ur-Pk" }
    .Select(c => new CultureInfo(c)).ToList();

builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    opts.DefaultRequestCulture = new RequestCulture("en-US");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
    opts.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CustomExceptionHandlerFilter>();
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
})
.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRequestLocalization();

app.UseRouting();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/error/{0}");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseInfrastructureModule();
app.Run();
