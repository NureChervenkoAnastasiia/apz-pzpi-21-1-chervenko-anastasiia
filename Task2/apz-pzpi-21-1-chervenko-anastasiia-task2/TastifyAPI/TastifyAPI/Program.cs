using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Data;
using TastifyAPI.Entities;
using TastifyAPI.Models;
using TastifyAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using TastifyAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TastifyAPI.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<TastifyDbSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TastifyDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TastifyDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSingleton<BookingService>();
builder.Services.AddSingleton<GuestService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<RestaurantService>();
builder.Services.AddSingleton<ScheduleService>();
builder.Services.AddSingleton<StaffService>();
builder.Services.AddSingleton<TableService>();
builder.Services.AddSingleton<OrderService>();
//builder.Services.AddSingleton<IMenuService, MenuService>();
//builder.Services.AddSingleton<IPositionProductService, PositionProductService>();
//


//builder.Services.AddScoped<IPasswordHasher<Staff>, PasswordHasher<Staff>>();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddJWTTokenAuthentication(builder.Configuration);
//builder.Services.AddScoped<IMenuService, MenuService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
