using BankAccounts.Core.Transactions;

namespace BankAccounts.Core.Statements;

// Форматирование - отдельная ответственность. 
// Можно добавить JSON/CSV без правки счета (SRP)
public interface IStatementFormatter
{
    string Format(IEnumerable<Transaction> transactions);
}

