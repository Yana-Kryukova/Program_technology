using BankAccounts.Core.AccountNumbers;
using BankAccounts.Core.MonthEnd;
using BankAccounts.Core.Time;
using BankAccounts.Core.Transactions;
using BankAccounts.Core.Withdrawals;

namespace BankAccounts.Core.Accounts;
// Тонкий класс - ядро: хранит транзакции, применяет политики и правила
public class BankAccount : IAccountOperations, IAccountStatement
{
    private readonly List<Transaction> _transactions = new();
    private readonly List<IMonthEndRule> _monthEndRules;
    private readonly IClock _clock;
    private readonly IWithdrawalPolicy _withdrawalPolicy;

    public string Number { get; }
    public string Owner { get; }
    public decimal MinimumBalance { get; }
    public IReadOnlyList<Transaction> Transactions => _transactions;
    public decimal Balance => _transactions.Sum(t => t.Amount);

    public BankAccount(
        IClock clock,
        string owner,
        decimal initialBalance,
        decimal minimumBalance,
        IAccountNumberGenerator numberGenerator,
        IWithdrawalPolicy withdrawalPolicy,
        IEnumerable<IMonthEndRule>? monthEndRules = null)
    {
        _monthEndRules = monthEndRules?.ToList() ?? null;
        //if (_monthEndRules is null)
        //    _monthEndRules = null;
        //else _monthEndRules = monthEndRules!.ToList();
        _clock = clock;
        _withdrawalPolicy = withdrawalPolicy;
        Number = numberGenerator.Next();
        Owner = owner;
        MinimumBalance = minimumBalance;

        if (initialBalance > 0)
            Deposit(initialBalance, _clock.Now, "Initial balance");
    }

    public void Deposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException
                (nameof(amount), "Amount of deposit must be positive");
        }

        var deposit = new Transaction(amount, date, note);
        _transactions.Add(deposit);
    }

    public void Withdraw(decimal amount, DateTime date, string note)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        // Применяем политику снятия: получаем решение, а не исключение
        var decision = _withdrawalPolicy.Evaluate(Balance, amount, MinimumBalance);

        if (!decision.IsAllowed)
            throw new InvalidOperationException(decision.RejectionReason);

        _transactions.Add(new Transaction(-amount, date, note));

        // Если есть комиссия - добавляем отдельной транзакцией
        if (decision.Fee > 0)
            _transactions.Add(
                new Transaction(-decision.Fee, date, "Apply overdraft fee"));
    }

    public virtual void PerforMonthEndTransactions()
    {
        var asOf = _clock.Now;
        foreach (var rule in _monthEndRules)
            rule.Apply(this, asOf);
    }
}
