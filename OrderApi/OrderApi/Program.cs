using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Database;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain.Entities;
using OrderApi.Messaging.Receive.Options;
using OrderApi.Messaging.Receive.Receiver;
using OrderApi.Models.v1;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Query;
using OrderApi.Service.v1.Services;
using OrderApi.Validators.v1;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var serviceClientSettingsConfig = builder.Configuration.GetSection("RabbitMq");
var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();
builder.Services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);

//DB 

builder.Services.AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

//Info

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMvc().AddFluentValidation();



// Add services to the container.

builder.Services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(ICustomerNameUpdateService).Assembly);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IValidator<OrderModel>, OrderModelValidator>();

builder.Services.AddTransient<IRequestHandler<GetPaidOrderQuery, List<Order>>, GetPaidOrderQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetOrderByIdQuery, Order>, GetOrderByIdQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetOrderByCustomerGuidQuery, List<Order>>, GetOrderByCustomerGuidQueryHandler>();
builder.Services.AddTransient<IRequestHandler<CreateOrderCommand, Order>, CreateOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<PayOrderCommand, Order>, PayOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateOrderCommand>, UpdateOrderCommandHandler>();
builder.Services.AddTransient<ICustomerNameUpdateService, CustomerNameUpdateService>();

if (serviceClientSettings.Enabled)
{
    builder.Services.AddHostedService<CustomerFullNameUpdateReceiver>();
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
