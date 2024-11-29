using InventarioAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;




var builder = WebApplication.CreateBuilder(args);

//// Configurar el manejo de ciclos de referencia
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    });


//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
//    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;       
        options.JsonSerializerOptions.PropertyNamingPolicy = null;        
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;       
        options.JsonSerializerOptions.WriteIndented = true;
    });


// Add services to the container.

builder.Services.AddDbContext<InventarioContex>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlLocal")));

builder.Configuration.AddJsonFile("appsettings.json");
var clavesecreta = builder.Configuration.GetSection("settings").GetSection("clavesecreta").ToString();// "=Em1l1an0=";
var keyBytes = Encoding.UTF8.GetBytes(clavesecreta);

builder.Services.AddAuthentication(config => {

    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => {
    config.RequireHttpsMetadata = false;
    config.SaveToken = false;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



//Configuro swagger para que aparezca para autenticar
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventarioAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });
});

var app = builder.Build();

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

app.Run();
