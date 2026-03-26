using Microsoft.EntityFrameworkCore;
using storetech_eval_dotnet.Data;
using storetech_eval_dotnet.Infrastructure;
using storetech_eval_dotnet.Options;
using storetech_eval_dotnet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var connectionString = builder.Configuration.GetConnectionString("StoreTech")
    ?? throw new InvalidOperationException("Connection string 'StoreTech' was not found.");

builder.Services.AddDbContext<StoreTechDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddOptions<CartOptions>()
    .Bind(builder.Configuration.GetSection(CartOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(options => options.RewardEligibleRetentionDays >= options.StaleCartDays,
        "RewardEligibleRetentionDays must be greater than or equal to StaleCartDays.")
    .ValidateOnStart();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITaxService, TaxService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
