using BankAccounts.Core.Accounts;

namespace BankAccounts.Core.MonthEnd;

// Проценты на положительный баланс выше порога (для процентного счета)
public class InterestOnPositiveBalanceRule : IMonthEndRule
{
    private readonly decimal _rate;
    private readonly decimal _threshold;

    public InterestOnPositiveBalanceRule(decimal rate, decimal threshold)
    {
        _rate = rate;
        _threshold = threshold;
    }

    public void Apply(IAccountOperations account, DateTime asOf)
    {
        if (account.Balance > _threshold)
            account.Deposit(account.Balance * _rate, asOf, "apply monthly interest");
    }
}
