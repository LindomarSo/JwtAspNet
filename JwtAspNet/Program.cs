using JwtAspNet;
using JwtAspNet.Extensions;
using JwtAspNet.Models;
using JwtAspNet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // É a forma como ele vai interrogar a requisição para saber de onde tá vindo o token, por exemplo.
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("teste"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", (TokenService service) =>
{
    return service.Create(new User(1, "Teste", "lindomar@teste.com", "xyz", "https://google.com/me", new[] { "Admin", "Student" }));
});

app.Map("/restrito", (ClaimsPrincipal user) =>
{
    var usuario = new
    {
        Id = user.Id(),
        Name = user.Name(),
        Email = user.Email(),
        GivenName = user.GivenName(),
        Image = user.Image()
    };

    return Results.Ok(usuario);
}).RequireAuthorization();

app.Map("/admin", () => "Você tem acesso ao sistema").RequireAuthorization("admin");

app.Run();
