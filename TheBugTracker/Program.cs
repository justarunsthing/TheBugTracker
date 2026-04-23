using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using TheBugTracker.Client.Interfaces;
using TheBugTracker.Components;
using TheBugTracker.Components.Account;
using TheBugTracker.Data;
using TheBugTracker.Interfaces;
using TheBugTracker.Models;
using TheBugTracker.Repository;
using TheBugTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("cookie", new OpenApiSecurityScheme
    {
        Name = ".AspNetCore.Identity.Application",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.Http,
        Scheme = "cookie"
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddOutputCache();
builder.Services.AddMudServices();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = DataUtility.GetConnectionString(builder.Configuration) ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectDTOService, ProjectDTOService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DataUtility.ManageDataAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
app.MapScalarApiReference(); // URL: /scalar/v1
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseOutputCache();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TheBugTracker.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();