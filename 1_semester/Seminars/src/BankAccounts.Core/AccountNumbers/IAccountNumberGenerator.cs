namespace BankAccounts.Core.AccountNumbers;

// Генерация номеров - отдельная ответственность, не должна быть внутри счета (SRP)
public interface IAccountNumberGenerator
{
    string Next();
}
