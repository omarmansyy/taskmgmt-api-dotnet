using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Mapping;
using TaskMgmt.Api.Middleware;
using TaskMgmt.Api.Services.Implementations;
using TaskMgmt.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddScoped<IEmailService, EmailService>();

// Quartz @ 9:00 every day
builder.Services.AddQuartz(q =>
{
    var jobKey = new Quartz.JobKey("OverdueTaskNotifier");
    q.AddJob< TaskMgmt.Api.Jobs.OverdueTaskNotifierJob >(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(t => t.ForJob(jobKey)
        .WithIdentity("OverdueTaskNotifier-trigger")
        .WithCronSchedule("0 0 9 * * ?")); // 09:00 daily
});
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        // useful for local HTTP testing
        o.RequireHttpsMetadata = false;

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// DI
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskMgmt API", Version = "v1" });

    // Define the scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here (without the word Bearer)."
    });

    // Require the scheme (this is what makes Swagger send the header)
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Global error handler
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you're only running HTTP locally, you can comment the next line.
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// migrate + seed
await Seed.RunAsync(app.Services);

app.Run();
