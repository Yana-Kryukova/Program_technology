namespace BankAccounts.Core.Accounts;

public interface IAccountOperations
{
    decimal Balance { get; }
    void Deposit(decimal amount, DateTime date, string note);
    void Withdraw(decimal amount, DateTime date, string note);
}
