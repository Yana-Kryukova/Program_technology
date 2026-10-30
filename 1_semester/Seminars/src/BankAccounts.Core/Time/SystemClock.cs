namespace BankAccounts.Core.Time;

// Реализация для продакшена: берет реальное время
public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}
