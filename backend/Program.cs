using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Interfaces;
using backend.Repository;
using backend.Services;
using backend.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using backend.Helpers;
using Microsoft.Extensions.Options;

namespace backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["GoogleAuth:ClientId"];
                    googleOptions.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
                });

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthTokenHandler, AuthTokenHandler>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


            builder.Services.AddIdentityCore<User>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 7;
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<Role>()
                .AddRoleManager<RoleManager<Role>>()
                .AddEntityFrameworkStores<DataContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataContext>();
                var userManager = services.GetService<UserManager<User>>();
                var roleManager = services.GetService<RoleManager<Role>>();
                await context.Database.MigrateAsync();
                await Seed.SeedAdminAndRoles(userManager, roleManager, builder.Configuration);
            }

            catch (Exception ex)
            {
                var logger = services.GetService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            app.Run();

        }
    }
}
