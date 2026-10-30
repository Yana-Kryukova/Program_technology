using BankAccounts.Core.Accounts;

namespace BankAccounts.Core.MonthEnd;

//Ежемесячное пополнение (для подарочной карты)
public class MonthlyDepositeRule : IMonthEndRule
{
    private readonly decimal _amount;
    public MonthlyDepositeRule(decimal amount)
    {
        _amount = amount;
    }

    public void Apply(IAccountOperations account, DateTime asOf)
    {
        if (_amount != 0)
            account.Deposit(_amount, asOf, "Add monthly deposit");
    }
}
