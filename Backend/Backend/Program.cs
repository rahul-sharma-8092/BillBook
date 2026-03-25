using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.Configure<Backend.Infrastructure.Db.DbOptions>(builder.Configuration.GetSection("Db"));
builder.Services.AddSingleton<Backend.Infrastructure.Db.IDbConnectionFactory, Backend.Infrastructure.Db.SqlConnectionFactory>();

builder.Services.AddScoped<Backend.Infrastructure.Repositories.ICategoriesRepository, Backend.Infrastructure.Repositories.CategoriesRepository>();
builder.Services.AddScoped<Backend.Infrastructure.Repositories.IProductsRepository, Backend.Infrastructure.Repositories.ProductsRepository>();
builder.Services.AddScoped<Backend.Infrastructure.Repositories.ICustomersRepository, Backend.Infrastructure.Repositories.CustomersRepository>();
builder.Services.AddScoped<Backend.Infrastructure.Repositories.IInvoicesRepository, Backend.Infrastructure.Repositories.InvoicesRepository>();
builder.Services.AddScoped<Backend.Infrastructure.Repositories.IDashboardRepository, Backend.Infrastructure.Repositories.DashboardRepository>();
builder.Services.AddScoped<Backend.Infrastructure.Repositories.ISettingsRepository, Backend.Infrastructure.Repositories.SettingsRepository>();
builder.Services.AddSingleton<Backend.Infrastructure.Pdf.IPdfService, Backend.Infrastructure.Pdf.PdfService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Backend.Application.Invoices.InvoiceCreateDtoValidator>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<Backend.Infrastructure.Api.ApiExceptionMiddleware>();

//app.UseSwagger();
//app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
