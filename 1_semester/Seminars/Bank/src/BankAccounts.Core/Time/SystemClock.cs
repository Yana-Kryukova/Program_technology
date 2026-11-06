namespace BankAccounts.Core.Time;

// Реализация для продакшена: берет реальное время
public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}

public sealed class TestClock : IClock
{
    public DateTime Now => new DateTime(2000,12,12);
}
