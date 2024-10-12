using System.Reflection;
using System.Text;
using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MyCSharp.HttpUserAgentParser.DependencyInjection;
using TurnDigital.Application;
using TurnDigital.Application.Common;
using TurnDigital.Infrastructure;
using TurnDigital.Web;
using TurnDigital.Web.Configurations;
using TurnDigital.Web.Extensions;
using TurnDigital.Web.Filters;
using TurnDigital.Web.Middlewares;
using TurnDigital.Web.Security;
using TurnDigital.Web.Services;
using TurnDigital.Web.Utilities;
using Newtonsoft.Json.Converters;
using Serilog;
using Serilog.Events;
using StackExchange.Profiling;
using Swashbuckle.AspNetCore.SwaggerGen;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Security.Configuration;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Interfaces;
using TurnDigital.Domain.Web.Interfaces;
using TurnDigital.Infrastructure.DataAccess;
using TurnDigital.Infrastructure.DataAccess.Seed;
using TurnDigital.Web.ActionFilters;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

var builder = WebApplication.CreateBuilder(args);

#region logging

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Set default minimum level
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .Filter.ByExcluding(e =>
        e.MessageTemplate.Text == "@mt = 'An unhandled exception has occurred while executing the request.'")
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Identity Configuration

builder.Services.AddIdentity<User, Role>(options =>
    {
        //identity configuration goes here
    })
    .AddSignInManager()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<TurnDigitalDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region JWT

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped(services => services.GetRequiredService<IOptions<JwtConfig>>().Value);

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped(typeof(IClaimsProvider), typeof(ClaimsProvider));

builder
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Constants.AuthenticationSchemes.JwtOrCookies;
        options.DefaultChallengeScheme = Constants.AuthenticationSchemes.JwtOrCookies;
        options.DefaultSignOutScheme = Constants.AuthenticationSchemes.JwtOrCookies;
    })
    .AddCookie(options =>
    {
        //cookie authentication options goes here
        //login, logout paths, and cookie expiration options
        options.LoginPath = "/account/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secrets"]!)),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(builder.Configuration["JWT:ClockSkewInMinutes"]))
        };
    })
    .AddPolicyScheme(Constants.AuthenticationSchemes.JwtOrCookies, Constants.AuthenticationSchemes.JwtOrCookies,
        options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authorization = context.Request.Headers[HeaderNames.Authorization];
                if (!string.IsNullOrEmpty(authorization) && authorization.ToString().StartsWith("Bearer "))
                    return JwtBearerDefaults.AuthenticationScheme;

                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });

builder.Services.AddScoped<ICookieAuthenticationService, CookieAuthenticationService>();

#endregion


#region authorization

builder.Services.AddAuthorization(options => { options.AddPolicies(); });

builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

#endregion

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

#region ef core config

builder.Services.AddDbContext(builder.Environment.IsDevelopment());

#endregion

#region fluent validation

builder.Services.AddValidatorsFromAssemblyContaining<ApplicationModule>();

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

#endregion

#region mapster

TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);

var config = TypeAdapterConfig.GlobalSettings;

config.Scan(Assembly.GetExecutingAssembly());

#endregion

builder.Services.AddScoped<IUserDeviceDetector, UserDeviceDetector>();
builder.Services.AddHttpUserAgentParser();

var corsConfiguration = builder.Configuration.GetSection("CorsConfiguration").Get<CorsConfiguration>();

builder.Services
    .AddControllersWithViews(opts => { opts.Filters.Add<RequireBaseUrlFilter>(); })
    .AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddScoped<JsonSerializerConfigProvider>();

builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AuthorizeOperationFilter>();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apis v1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "apis v2", Version = "v2" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.DocInclusionPredicate((version, apiDescription) =>
    {
        if (!apiDescription.TryGetMethodInfo(out var methodInfo)) return false;

        var versions = methodInfo.DeclaringType!
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .ToList();

        return versions.Any(v => $"v{v.MajorVersion}" == version);
    });
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new InfrastructureModule());
    containerBuilder.RegisterModule(new ApplicationModule());
    containerBuilder.RegisterModule(new WebModule());
});

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddExceptionHandler<ExceptionLogger>();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddScoped<LoggingMiddleware>();

#region Memory caching

builder.Services.AddMemoryCache();

#endregion

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

builder.Services.AddScoped<IBaseUrlProvider, BaseUrlProvider>();
builder.Services.AddScoped<IUserIpAddressProvider, UserIpAddressProvider>();

builder.Services.Configure<ProxyConfig>(builder.Configuration.GetSection("ProxyConfig"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<ProxyConfig>>().Value);

#region miniprofiler

builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";

    options.ColorScheme = ColorScheme.Dark;

    options.IgnorePath("/swagger");
}).AddEntityFramework();

#endregion

builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV"; // 'v1', 'v2', etc.
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
        c.SwaggerEndpoint($"/swagger/v2/swagger.json", "v2");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiniProfiler();
app.UseMiddleware<LoggingMiddleware>();
app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.UseCors(
        options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
    );
}
else
{
    app.UseCors(options =>
    {
        options.WithOrigins(corsConfiguration!.AllowedOrigins.ToArray()).AllowAnyMethod().AllowAnyHeader()
            .AllowCredentials();
    });
}


app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{slug?}");
app.MapRazorPages();

//migrate db context
var scope = app.Services.CreateAsyncScope();

await using (scope)
{
    await scope.ServiceProvider.SeedRoles();
    await scope.ServiceProvider.CreateTurnDigitalAdmin();
    var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
    await repository.SeedFakeData();
}

app.Run();

public partial class Program
{
}