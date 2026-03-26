namespace StoreTech.Eval.Tests.TestInfrastructure;

internal sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<StoreTechDbContext> _options;

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<StoreTechDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new StoreTechDbContext(_options);
        context.Database.EnsureCreated();
    }

    public StoreTechDbContext CreateDbContext()
    {
        return new StoreTechDbContext(_options);
    }

    public static CartOptions CreateCartOptions(
        int staleCartDays = 7,
        int rewardEligibleRetentionDays = 14)
    {
        return new CartOptions
        {
            StaleCartDays = staleCartDays,
            RewardEligibleRetentionDays = rewardEligibleRetentionDays
        };
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}