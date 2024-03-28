using System.Text;
using backend.Context;
using backend.Helper;
using backend.Models;
using backend.Repositories;
using backend.Repositories.IRepositories;
using backend.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Khanh API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddIdentity<AppUser,IdentityRole>(options=>{
    options.Password.RequiredLength = 7;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric=true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options=>{
     
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>{
    options.SaveToken=true;
    options.RequireHttpsMetadata=false;
    options.TokenValidationParameters=new Microsoft.IdentityModel.Tokens.TokenValidationParameters{
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidAudience=builder.Configuration["Jwt:ValidAudience"],
        ValidIssuer=builder.Configuration["Jwt:ValidIssuer"],
        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]??"baba"))
    };
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectString") + "Encrypt=True;");
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();



builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountService>();


var app = builder.Build();

// Khởi tạo dữ liệu mẫu
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<AppUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
await SeedData.InitializeAsync(userManager, roleManager);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

 app.UseHttpsRedirection();


    app.UseAuthentication();

    app.UseAuthorization();

 app.MapControllers();
    app.AddGlobalErrorHandler();
      app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/account"), builder =>
    {
        builder.UseMiddleware<CustomAuthorizationMiddleware>();
    });

    // app.AddCustomAuthorizationHandler();
       

    


    app.Run();

