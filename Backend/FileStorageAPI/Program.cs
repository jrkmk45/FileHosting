using Domain.Models;
using ForumAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Repositories.Implementations;
using Repository.Repositories.Interfaces;
using Services.Services.Implementations;
using Services.Services.Inerfaces;
using Services.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:4200",
                "http://www.contoso.com",
                "http://filehosting-frontend.s3-website.eu-central-1.amazonaws.com")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services to the container.

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql("Server=my-apps-db.ccckezizvzsu.eu-central-1.rds.amazonaws.com;" +
        $"Port=5432;Database=my-apps-db;UserId=postgres;Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}; Include Error Detail=true;"));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IFileMetadataRepository, FileMetadataRepository>();

builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = true;
}).AddEntityFrameworkStores<AppDbContext>().AddRoles<IdentityRole<int>>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        ValidateAudience = false,
        RequireExpirationTime = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileStorageAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization\n Input: Bearer Token",
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

app.UseCors();

app.UseStaticFiles();

/*
var rolesService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IRoleService>();

await rolesService.CreateInitialRoles();
await rolesService.CreateInitialAdminUser();
*/
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorageAPI v1");
    });
}


app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.MapControllers();

app.Run();
