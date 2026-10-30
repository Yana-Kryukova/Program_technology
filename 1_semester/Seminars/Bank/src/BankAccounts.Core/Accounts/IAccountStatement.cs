using BankAccounts.Core.Transactions;

namespace BankAccounts.Core.Accounts;

// ISP: клиенты зависят только от того, что им реально нужно
public interface IAccountStatement
{
    IReadOnlyList<Transaction> Transactions { get; }
}
