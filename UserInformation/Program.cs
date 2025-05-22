using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserInformation.Model;
using UserInformation.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITreeRepository, TreeRepository>();
builder.Services.AddScoped<IAmChart,  AmChart>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)                    //JwtBearerDefaults.AuthenticationScheme is just a constant "Bearer", which tells ASP.NET that tokens will be sent like this:Authorization: Bearer <token>

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                                                 //Ensures the issuer (who created the token) matches the expected one (in your config):
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,                               //validate the token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],                                    
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), //"Use this secret key (from config) to sign and validate JWT tokens to ensure they haven't been tampered with."
            ClockSkew = TimeSpan.Zero
        };
    });



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        );



}
);

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseStaticFiles();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
