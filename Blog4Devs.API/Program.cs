using Blog4Devs.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog4Devs.API;
using NetDevPack.Identity.Jwt;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Model;
using MiniValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

//Add Swagger Settings
builder.Services.AddSwaggerGen(c =>
  {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
          Title = "Blogs4Devs",
          Description = "Developed by Leandro Andreotti",
          Contact = new OpenApiContact { Name = "Leandro Andreotti", Email="landreotti91@gmail.com"},
          License = new OpenApiLicense { Name = "Mit", Url = new Uri("https://opensource.org/license/mit-0") }
      });

      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
          Description = "Insira o token JWT desta maneira: Bearer {Seu Token}",
          Name = "Authorization",
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey
      });

      c.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  }
              },
              new string[]{}
          }
      });
  }
);

// Add Conection Services / DBContext.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// Some Tests
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true);
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add Configuration
builder.Services.AddControllersWithViews();

// Configuration NetDevPack.Identity
builder.Services.AddIdentityEntityFrameworkContextConfiguration(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Blog4Devs.API")));
builder.Services.AddIdentityConfiguration().AddEntityFrameworkStores<ApplicationDbContext>(); ;
builder.Services.AddJwtConfiguration(builder.Configuration, "AppSettings");

// Configuration for Policys
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExcluirPosts", policy => policy.RequireClaim("ExcluirPosts"));
});


var app = builder.Build();

#endregion

#region Configure Pipeline
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHttpsRedirection();
#endregion

#region Actions
app.MapPost("/registro", [AllowAnonymous] async (
    SignInManager<IdentityUser> signInManager,
    Microsoft.AspNetCore.Identity.UserManager<IdentityUser> UserManager,
    IOptions<AppJwtSettings> AppJwtSettings,
    RegisterUser registerUser) =>
    {
        if (registerUser == null)
            return Results.BadRequest("Usu�rio n�o informado.");
        if (!MiniValidator.TryValidate(registerUser, out var errors))
            return Results.ValidationProblem(errors);
        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await UserManager.CreateAsync(user, registerUser.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        var jwt = new JwtBuilder()
                        .WithUserManager(UserManager)
                        .WithJwtSettings(AppJwtSettings.Value)
                        .WithEmail(user.Email)
                        .WithJwtClaims()
                        .WithUserClaims()
                        .WithUserRoles()
                        .BuildUserResponse();

        return Results.Ok(jwt);
    }).Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("RegistroUsuario")
.WithTags("Usuario");

app.MapPost("/login", [AllowAnonymous] async (
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> UserManager,
    IOptions<AppJwtSettings> AppJwtSettings,
    LoginUser loginUser) =>
{
    if (loginUser == null)
        return Results.BadRequest("Usu�rio n�o informado");
    if (!MiniValidator.TryValidate(loginUser, out var errors))
        return Results.ValidationProblem(errors);

    var results = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, isPersistent: false, lockoutOnFailure: false);


    if (results.IsLockedOut)
        return Results.BadRequest("Usu�rio bloqueado");
    if (!results.Succeeded)
        return Results.BadRequest("Usu�rio ou senha inv�lidos");

    var jwt = new JwtBuilder()
    .WithUserManager(UserManager)
    .WithJwtSettings(AppJwtSettings.Value)
    .WithEmail(loginUser.Email)
    .WithJwtClaims()
    .WithUserClaims()
    .WithUserRoles()
    .BuildUserResponse();

    return Results.Ok(jwt);
}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("LoginUsuario")
.WithTags("Usuario");

app.MapPostsEndpoints();

app.MapCommentEndpoints();

app.Run();
#endregion