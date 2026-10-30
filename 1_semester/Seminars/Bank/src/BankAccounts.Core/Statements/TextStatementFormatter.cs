using BankAccounts.Core.Transactions;
using System.Text;

namespace BankAccounts.Core.Statements;

public sealed class TextStatementFormatter : IStatementFormatter
{
    public string Format(IEnumerable<Transaction> transactions)
    {
        var report = new StringBuilder();

        decimal balance = 0;
        report.AppendLine("Data\t\tAmount\tBalance\tNote");
        foreach (var item in transactions)
        {
            balance += item.Amount;
            report.AppendLine($"" +
                $"{item.Date.ToShortDateString()}\t" +
                $"{item.Amount}\t{balance}\t{item.Note}");
        }
        return report.ToString();
    }
}
