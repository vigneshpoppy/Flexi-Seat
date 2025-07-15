using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.Models;
using FlexiSeat.Services;
using FlexiSeat.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.SpaServices;

var builder = WebApplication.CreateBuilder(args);
var secretString = builder.Configuration.GetValue<string>("TokenKey");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FlexiSeatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<WhatsAppMessageService>();
builder.Services.AddScoped<SeatService>();
builder.Services.Configure<TwilioSettings>(
builder.Configuration.GetSection("Twilio"));

builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString!)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();
// Serve static files
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot"; // Or wherever your Angular build output is
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseSpaStaticFiles();


// Redirect unknown paths to Angular
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot"; // Or wherever your Angular project is

    //if (app.Environment.IsDevelopment())
    //{
    //    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    //}
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlexiSeatDbContext>();

    // Option 1: Create database and tables if they don't exist
    //dbContext.Database.EnsureCreated();

    // Option 2: Apply pending migrations (recommended for production)
    dbContext.Database.Migrate();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
