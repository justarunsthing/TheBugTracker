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
    // Swagger doc details
    options.SwaggerDoc("v1", new()
    {
        Title = "The Bug Tracker API",
        Version = "v1",
        Description = """
                      <img src="/img/app-logo.svg" height="120" />

                      This API is used by The Bug Tracker application when executing 
                      in WebAssembly to interact with the server.

                      This API uses cookie-based authentication. To test the requests
                      below, you must first log in through the application to set a 
                      cookie in your browser and receive a valid response from the 
                      "Test Request" buttons below.
                      """,
        Contact = new()
        {
            Name = "Arun Pun",
            Email = "test@email.com",
            Url = new("https://github.com/justarunsthing")
        }
    });

    // Show cookies as the authentication scheme
    options.AddSecurityDefinition("cookie", new OpenApiSecurityScheme
    {
        Name = ".AspNetCore.Identity.Application",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.Http,
        Scheme = "cookie"
    });

    // Show which endpoints require login
    options.OperationFilter<SecurityRequirementsOperationFilter>();

    // Generate documentation from XML comments in the code
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

    // Exclude documentation for the built-in Identity endpoints
    options.DocInclusionPredicate((_, description) =>
        description.RelativePath is null || !description.RelativePath.StartsWith("Account"));
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
    .AddIdentityCookies(cookieBuilder =>
    {
        // Override the default behavior of redirecting to the login/access denied page when an unauthorized API request is made
        cookieBuilder.ApplicationCookie!.Configure(config =>
        {
            config.Events.OnRedirectToLogin += (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api") || context.Request.HasJsonContentType())
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }

                return Task.CompletedTask;
            };

            config.Events.OnRedirectToAccessDenied += (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api") || context.Request.HasJsonContentType())
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                }

                return Task.CompletedTask;
            };
        });
    });

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

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyDTOService, CompanyDTOService>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketDTOService, TicketDTOService>();

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
app.MapScalarApiReference(options =>
{
    options.WithFavicon("/img/arunpun-favicon.png")
           .WithTitle("API Specification | The Bug Tracker")
           .WithTheme(ScalarTheme.BluePlanet);
}); // URL: /scalar/v1
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseOutputCache();

app.MapStaticAssets();

app.Use(async (ctx, next) =>
{
    await next(ctx);

    if (ctx.Request.Path.StartsWithSegments("/images/png") && ctx.Response.StatusCode == 404)
    {
        ctx.Response.Redirect("/images/png/default.png");
    }
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TheBugTracker.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();