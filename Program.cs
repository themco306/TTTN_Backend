using System.Text;
using backend.Context;
using backend.DTOs;
using backend.Helper;
using backend.Hubs;
using backend.Models;
using backend.Payment.Momo;
using backend.Payment.Momo.Config;
using backend.Repositories;
using backend.Repositories.IRepositories;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
string Example07JSDomain="AllowLocalhost3000";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:Example07JSDomain,
        builder => builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .AllowAnyMethod());
});
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
builder.Services.AddHttpContextAccessor();



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
    //
      options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken)
                        && path.StartsWithSegments("/chatHub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
            //
});
builder.Services.AddAuthorization(APolicyBuilder.Build);
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectString") + "Encrypt=True;");
// });
// builder.Services.AddHostedService<UpdateDatabaseService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<UserTracker>();
builder.Services.Configure<MomoConfig>(
    builder.Configuration.GetSection("Momo")
);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<BrandService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<Generate>();



builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<TagService>();

builder.Services.AddScoped<IWebInfoRepository, WebInfoRepository>();
builder.Services.AddScoped<WebInfoService>();

builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<GalleryService>();

builder.Services.AddScoped<ISilderRepository, SilderRepository>();
builder.Services.AddScoped<SliderService>();

builder.Services.AddScoped<IOrderInfoRepository, OrderInfoRepository>();
builder.Services.AddScoped<OrderInfoService>();

builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<ICartItemRepository,CartItemRepository>();

builder.Services.AddScoped<ICartRepository,CartRepository>();
builder.Services.AddScoped<CartService>();

builder.Services.AddScoped<ICouponRepository,CouponRepository>();
builder.Services.AddScoped<CouponService>();

builder.Services.AddScoped<ICouponUsageRepository,CouponUsageRepository>();

builder.Services.AddScoped<IProductTagRepository,ProductTagRepository>();

builder.Services.AddScoped<IPaidOrderRepository,PaidOrderRepository>();
builder.Services.AddScoped<PaymentMomoService>();

builder.Services.AddScoped<IMenuRepository,MenuRepository>();
builder.Services.AddScoped<MenuService>();

builder.Services.AddScoped<ITopicRepository,TopicRepository>();
builder.Services.AddScoped<TopicService>();

builder.Services.AddScoped<IPostRepository,PostRepository>();
builder.Services.AddScoped<PostService>();

builder.Services.AddScoped<IContactRepository,ContactRepository>();
builder.Services.AddScoped<ContactService>();

builder.Services.AddScoped<IRateLikeRepository,RateLikeRepository>();

builder.Services.AddScoped<IRateRepository,RateRepository>();
builder.Services.AddScoped<RateService>();

builder.Services.AddScoped<IMessageRepository,MessageRepository>();
builder.Services.AddScoped<IMessageReadStatusRepository,MessageReadStatusRepository>();

var app = builder.Build();
app.UseCors(Example07JSDomain);
// Khởi tạo dữ liệu mẫu
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<AppUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
var dbContext = services.GetRequiredService<AppDbContext>();
await SeedData.InitializeAsync(userManager, roleManager, dbContext);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
 app.UseHttpsRedirection();


    app.UseAuthentication();

    app.UseAuthorization();

 app.MapControllers();
 app.MapHub<ChatHub>("/chatHub");
    app.AddGlobalErrorHandler();
    //   app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/account"), builder =>
    // {
    //     builder.UseMiddleware<CustomAuthorizationMiddleware>();
    // });

    // app.AddCustomAuthorizationHandler();
       

    


    app.Run();

