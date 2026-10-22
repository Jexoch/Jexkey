using Jexkey.Application.Interfaces;
using Jexkey.Application.Services;
using Jexkey.Domain.Interfaces;
using Jexkey.Infrastructure.Context;
using Jexkey.Infrastructure.Repositories;
using Jexkey.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Jexkey.WebAPI {
    public class Program {
        public static void Main(string[] args) {

            var builder = WebApplication.CreateBuilder(args);

            var conn = builder.Configuration.GetConnectionString("PGConn");
            var key = builder.Configuration["Jwt:Key"]!;
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddDbContext<JexkeyDbContext>(options => options.UseNpgsql(conn));

            builder.Services.AddControllers();

            var tokenRules = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = tokenRules;
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IVaultRepository, VaultRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IVaultService, VaultService>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
