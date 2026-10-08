using System.Text;

namespace Bank;

// BankAccount - потомок класс object
public class BankAccount
{
    private readonly decimal _minimumBalance;
    static private int s_accountNuberSeed = 1000000000;
    public string Number { get; }
    public string Owner { get; private set; }
    public decimal Balance
    {
        get
        {
            decimal balance = 0;
            foreach (var item in _allTransactions)
            {
                balance += item.Amount;
            }

            return balance;
        }
    }

    private List<Transaction> _allTransactions = new List<Transaction>();

    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0)
    {

    }

    public BankAccount(string name, decimal initialBalance, decimal minimumBalance)
    {
        Owner = name; // this.Owner = name

        Number = s_accountNuberSeed.ToString();
        s_accountNuberSeed++;

        _minimumBalance = minimumBalance;

        if (initialBalance > 0)
            MakeDeposit(initialBalance, DateTime.UtcNow, "Initial balance");
    }
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
        }

        var deposit = new Transaction(amount, date, note);
        _allTransactions.Add(deposit);
    }

    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Transaction? overdraftTransaction 
            = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);

        _allTransactions.Add(withdrawal);

        if (overdraftTransaction is not null)
            _allTransactions.Add(overdraftTransaction);
    }


    // protected - модификатор доступа, который означает,
    // что это метод можно вызвать только из текущего и дочернего класса
    // Клиент (внешний код) данный метод вызвать не может
    protected virtual Transaction? CheckWithdrawalLimit(bool isOverdrawn)
    {
        if (isOverdrawn)
        {
            throw new InvalidOperationException("Not sufficient rubls for this withdrawal");
        }
        else 
        {
            // default - содержит значение по умолчаю,
            // так как тип возвращаемого значения - ссылочный, то 
            // default = null
            return default; // == return null;
        }
    }

    public string GetAccountHistory()
    {
        var report = new StringBuilder();

        decimal balance = 0;
        report.AppendLine("Data\t\tAmount\tBalance\tNote");
        foreach (var item in _allTransactions)
        {
            balance += item.Amount;
            report.AppendLine($"" +
                $"{item.Date.ToShortDateString()}\t" +
                $"{item.Amount}\t{balance}\t{item.Note}");
        }
        return report.ToString();
    }


    // Ключевое слово virtual позволяет в дочернем классе предоставить другую реализацию
    // метода PerformMonthAndTransactions
    public virtual void PerformMonthAndTransactions()
    {

    }

    // переопределяем метод базового класса - класса object
    // ToString - возвращает строку и информацией об объекте
    public override string ToString()
    {
        return $"Owner: {Owner}\taccount number: {Number} (тип счета {GetType()})";
    }

}
