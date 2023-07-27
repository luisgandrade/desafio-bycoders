using Microsoft.EntityFrameworkCore;
using TransactionsLog.DataLayer.Abstractions;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.DataLayer.EF;
using TransactionsLog.DataLayer.EF.Repositories;
using TransactionsLog.Models.Entities;
using TransactionsLog.Services.TransactionParser;
using TransactionsLog.Services.TransactionsLogger;
using TransactionsLog.Services.TransactionsUploader;
using TransactionsLog.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<TransactionsLogContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITransactionsLogger, DefaultTransactionsLogger>();
builder.Services.AddScoped<ITransactionParser, CNABTransactionParser>();
builder.Services.AddScoped<IBaseRepository<Transaction>, EFBaseRepository<Transaction>>();
builder.Services.AddScoped<IBaseRepository<TransactionType>, EFBaseRepository<TransactionType>>();
builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();

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

app.BootstrapDb();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
