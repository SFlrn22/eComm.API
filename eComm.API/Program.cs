using eComm.API.TokenHandlerMiddleware;
using eComm.APPLICATION;
using eComm.DOMAIN.Utilities;
using eComm.INFRASTRUCTURE;
using eComm.PERSISTENCE;
using eComm.PERSISTENCE.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>()
    {
        new SqlColumn("Username", SqlDbType.VarChar),
        new SqlColumn("SessionIdentifier", SqlDbType.UniqueIdentifier)
    }
};

var logger = new LoggerConfiguration().WriteTo
    .MSSqlServer(EncryptionHelper.Decrypt(config.GetSection("ConnectionStrings:DefaultConnection").Value!, AesKeyConfiguration.Key, AesKeyConfiguration.IV),
        new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            SchemaName = "dbo",
            AutoCreateSqlTable = true,
        }, columnOptions: columnOptions)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services
  .AddOptions<AppSettings>()
  .Bind(builder.Configuration.GetSection(AppSettings.Name));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddPersistenceServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var origins = builder.Configuration.GetSection("CorsOrigins").GetChildren().Select(x => x.Value).ToArray();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        });
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Auth with JWT",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:JwtConfiguration:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:JwtConfiguration:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JwtConfiguration:Key"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseMiddleware<TokenHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
