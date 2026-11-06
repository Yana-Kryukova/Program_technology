using BankAccounts.Core.AccountNumbers;
using BankAccounts.Core.Accounts;
using BankAccounts.Core.MonthEnd;
using BankAccounts.Core.Time;
using BankAccounts.Core.Withdrawals;

namespace BankAccounts.Core.Factories;

public class AccountFactory
{
    private readonly IClock _clock;
    private readonly IAccountNumberGenerator _numbers;
    public AccountFactory(IClock clock, IAccountNumberGenerator numbers)
    {
        _clock = clock;
        _numbers = numbers;
    }

    public BankAccount CreateInterestEarning(string owner, decimal initialBalance)
        => new BankAccount(
            _clock,
            owner,
            initialBalance,
            0m,
            _numbers,
            new StrictWithdrawalPolicy(),
            new IMonthEndRule[] { new InterestOnPositiveBalanceRule(0.2m, 500m) });

    public BankAccount CreateDebitAccount(string owner, decimal initialBalance)
    => new BankAccount(
        _clock,
        owner,
        initialBalance,
        0m,
        _numbers,
        new StrictWithdrawalPolicy());

    public BankAccount CreateLineOfCredit(string owner, decimal creditLimit)
    => new BankAccount(
        _clock,
        owner,
        0m,
        -creditLimit,
        _numbers,
        new OverdraftFeeWithdrawalPolicy(20m),
        new IMonthEndRule[] { new InterestNegativeBalanceRule(0.07m) });

    public BankAccount CreateGiftCard(string owner, decimal initialBalance, decimal monthlyDeposit)
    => new BankAccount(
        _clock,
    owner,
    initialBalance,
    0m,
    _numbers,
    new StrictWithdrawalPolicy(),
    new IMonthEndRule[] { new MonthlyDepositeRule(monthlyDeposit) });
}
