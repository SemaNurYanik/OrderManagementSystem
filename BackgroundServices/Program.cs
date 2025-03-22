using BackgroundServices;
using Microsoft.EntityFrameworkCore;
using OrderManagementData;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHostedService<OrderProcessingServices>(); 

var host = builder.Build();
host.Run();
