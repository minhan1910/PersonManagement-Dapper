using DotnetAPI_Project.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services
builder.Services.AddScoped<IUserRepository, UserRepository>();

string? tokenKey = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenKey ?? string.Empty)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // cookies, ...
    });

    options.AddPolicy("ProdCors", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://myProductionSide.com")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // cookies, ...
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
} 
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

