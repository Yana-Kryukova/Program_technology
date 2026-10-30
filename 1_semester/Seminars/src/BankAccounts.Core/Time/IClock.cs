namespace BankAccounts.Core.Time;

// Выносим зависимость от времени в интерфейс,
// чтобы можно было подменить его в тестах (DIP)
public interface IClock
{
    DateTime Now { get; }
}
