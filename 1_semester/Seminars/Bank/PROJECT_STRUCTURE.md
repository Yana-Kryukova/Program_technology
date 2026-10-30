# Структура проекта

## Дерево

```
BankAccounts.sln
├── src/
│   ├── BankAccounts.Core/          # библиотека с предметной логикой
│   │   ├── Time/                   # IClock + SystemClock
│   │   ├── AccountNumbers/         # генерация номеров счетов
│   │   ├── Transactions/           # DTO транзакции
│   │   ├── Accounts/               # BankAccount + узкие интерфейсы
│   │   ├── Withdrawals/            # политики снятия
│   │   ├── MonthEnd/               # правила конца месяца
│   │   ├── Statements/             # форматирование выписок
│   │   └── Factories/              # AccountFactory
│   └── BankAccounts.Console/       # точка входа (демо-сценарии)
│       └── Program.cs
└── tests/
    └── ...

```

src/                                  ← «здесь продакшн-код»
├── BankAccounts.Core/                ← переиспользуемая библиотека, без консоли
│   └── Time/IClock.cs + SystemClock.cs   ← интерфейс рядом с реализацией
└── BankAccounts.Console/             ← exe, который использует Core
    └── Program.cs                    ← только сценарии запуска

tests/                                ← «здесь проверочный код»
└── BankAccounts.Tests/               ← ссылается только на Core, не на Console


## Принципы раскладки

- `src/` — код, который попадает в артефакт (`.dll`/`.exe`).
- `tests/` — проверочный код, в артефакт не входит.
- Внутри `Core` — по предметным областям, а не по типам файлов.
- Интерфейс лежит рядом с реализацией (`IClock.cs` + `SystemClock.cs`).
- Namespace = путь папки: `BankAccounts.Core.Withdrawals` и т.д.

## Проекты

| Проект | Назначение | Тип |
|---|---|---|
| `BankAccounts.Core` | предметная логика | Library |
| `BankAccounts.Console` | демо-сценарии | Exe |
| `BankAccounts.Tests` | модульные тесты | Library (test SDK) |