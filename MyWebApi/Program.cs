
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.Middlewares;
using Serilog;
using System.Text;

namespace MyWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddDbContext<Data.DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy => policy.WithOrigins("http://localhost:4200") 
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());
            });

            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Paste Token here:",
                    In = Microsoft.OpenApi.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            builder.Services.AddScoped<Repositories.Interfaces.IStudentRepository, Repositories.Implementations.StudentRepository>();
            builder.Services.AddScoped<Services.Interfaces.IStudentService, Services.Implementations.StudentService>();

            builder.Services.AddScoped<Repositories.Interfaces.ITeacherRepository, Repositories.Implementations.TeacherRepository>();
            builder.Services.AddScoped<Services.Interfaces.ITeacherService, Services.Implementations.TeacherService>();

            builder.Services.AddScoped<Repositories.Interfaces.ISubjectRepository, Repositories.Implementations.SubjectRepository>();
            builder.Services.AddScoped<Services.Interfaces.ISubjectService, Services.Implementations.SubjectService>();

            builder.Services.AddScoped<Repositories.Interfaces.IClassRepository, Repositories.Implementations.ClassRepository>();
            builder.Services.AddScoped<Services.Interfaces.IClassService, Services.Implementations.ClassService>();

            builder.Services.AddScoped<Repositories.Interfaces.ICourseRepository, Repositories.Implementations.CourseRepository>();
            builder.Services.AddScoped<Services.Interfaces.ICourseService, Services.Implementations.CourseService>();

            builder.Services.AddScoped<Services.Interfaces.IFileService, Services.Implementations.FileService>();
            builder.Services.AddScoped<Services.Interfaces.IAuthService, Services.Implementations.AuthService>();
            builder.Services.AddScoped<Services.Interfaces.IUserAccessor, Services.Implementations.UserAccessor>();
            builder.Services.AddHttpContextAccessor();

            var tokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;
            if (!string.IsNullOrEmpty(tokenKey))
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,
                        };
                    });
            }

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    SeedData.SeedDataAsync(services).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Có lỗi xảy ra khi seed dữ liệu!");
                }
            }

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<Middlewares.LoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();
            app.Run();
        }
    }
}
