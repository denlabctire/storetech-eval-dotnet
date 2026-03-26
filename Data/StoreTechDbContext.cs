using Microsoft.EntityFrameworkCore;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data;

public sealed class StoreTechDbContext(DbContextOptions<StoreTechDbContext> options) : DbContext(options)
{
	public DbSet<Cart> Carts => Set<Cart>();

	public DbSet<CartItem> CartItems => Set<CartItem>();

	public DbSet<Product> Products => Set<Product>();

	public DbSet<PriceInfo> Prices => Set<PriceInfo>();

	public DbSet<TaxInfo> Taxes => Set<TaxInfo>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreTechDbContext).Assembly);
	}
}