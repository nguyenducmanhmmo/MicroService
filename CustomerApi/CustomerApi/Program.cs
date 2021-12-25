using CustomerApi.Data;
using CustomerApi.Domain.Database;
using CustomerApi.Domain.NewFolder.Repository.v1;
using CustomerApi.Messaging.Send.Options.v1;
using CustomerApi.Messaging.Send.Sender;
using CustomerApi.Models.v1;
using CustomerApi.Service.Command;
using CustomerApi.Service.Query;
using CustomerApi.Validators.v1;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();

var serviceClientSettingsConfig = builder.Configuration.GetSection("RabbitMq");
builder.Services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);

// DB Connection
builder.Services.AddDbContext<CustomerContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

// Autmap
builder.Services.AddAutoMapper(typeof(Program));

// Validator

builder.Services.AddMvc().AddFluentValidation();

// Add services to the container.

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

builder.Services.AddTransient<IValidator<CreateCustomerModel>, CreateCustomerModelValidator>();
builder.Services.AddTransient<IValidator<UpdateCustomerModel>, UpdateCustomerModelValidator>();

builder.Services.AddSingleton<ICustomerUpdateSender, CustomerUpdateSender>();

builder.Services.AddTransient<IRequestHandler<CreateCustomerCommand, Customer>, CreateCustomerCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateCustomerCommand, Customer>, UpdateCustomerCommandHandler>();
builder.Services.AddTransient<IRequestHandler<GetCustomerByIdQuery, Customer>, GetCustomerByIdQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetCustomersQuery, List<Customer>>, GetCustomersQueryHandler>();

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
