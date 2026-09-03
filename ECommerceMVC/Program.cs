using ECommerceMVC.Handlers;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// MVC
// ==========================================================

builder.Services.AddControllersWithViews();

// ==========================================================
// HTTP CONTEXT ACCESSOR
// ==========================================================

builder.Services.AddHttpContextAccessor();

// ==========================================================
// SESSION
// ==========================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromHours(2);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;
});

// ==========================================================
// COOKIE AUTHENTICATION
// ==========================================================

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath =
            "/Account/Login";

        options.LogoutPath =
            "/Account/Logout";

        options.AccessDeniedPath =
            "/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    });

// ==========================================================
// JWT AUTHORIZATION HANDLER
// ==========================================================

builder.Services.AddTransient<
    JwtAuthorizationHandler>();

// ==========================================================
// API BASE URL
// ==========================================================

var apiBaseUrl =
    builder.Configuration[
        "ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "ApiSettings:BaseUrl is not configured.");
}

// ==========================================================
// GENERAL ECOMMERCE API CLIENT
// Used by PaymentController
// ==========================================================

builder.Services.AddHttpClient(
    "ECommerceApi",
    client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);

        client.DefaultRequestHeaders.Accept
            .Add(
                new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue(
                        "application/json"));
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// PRODUCT API
// ==========================================================

builder.Services.AddHttpClient<
    IProductApiService,
    ProductApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// CATEGORY API
// ==========================================================

builder.Services.AddHttpClient<
    ICategoryApiService,
    CategoryApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// CART API
// ==========================================================

builder.Services.AddHttpClient<
    ICartApiService,
    CartApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// ORDER API
// ==========================================================

builder.Services.AddHttpClient<
    IOrderApiService,
    OrderApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// PAYMENT API
// ==========================================================

builder.Services.AddHttpClient<
    IPaymentApiService,
    PaymentApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<
        JwtAuthorizationHandler>();

// ==========================================================
// AUTH API
// ==========================================================

builder.Services.AddHttpClient<
    IAuthApiService,
    AuthApiService>(client =>
    {
        client.BaseAddress =
            new Uri(apiBaseUrl);

        client.DefaultRequestHeaders.Accept
            .Add(
                new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue(
                        "application/json"));
    });


// ==========================================================
// BUILD
// ==========================================================

var app = builder.Build();

// ==========================================================
// HTTP PIPELINE
// ==========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ==========================================================
// SESSION
// Must come before Authentication because
// JWT fallback may restore Session.
// ==========================================================

app.UseSession();

// ==========================================================
// AUTHENTICATION
// ==========================================================

app.UseAuthentication();

// ==========================================================
// AUTHORIZATION
// ==========================================================

app.UseAuthorization();

// ==========================================================
// ROUTING
// ==========================================================

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");

app.Run();