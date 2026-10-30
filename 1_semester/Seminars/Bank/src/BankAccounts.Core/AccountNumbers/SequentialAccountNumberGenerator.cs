namespace BankAccounts.Core.AccountNumbers;

// Простая последовательная нумерация. Можно заменить на GUID или другую стратегию (SRP)
public sealed class SequentialAccountNumberGenerator : IAccountNumberGenerator
{
    private int _seed;
    public SequentialAccountNumberGenerator(int seed = 1234567890)
        => _seed = seed;

    public string Next()
    {
        return (_seed++).ToString();
    }
}
