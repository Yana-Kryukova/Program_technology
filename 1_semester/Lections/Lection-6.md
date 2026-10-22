# Презентация к лекции: Value Object и перегрузка операций в C#

---

## Слайд 1. Титульный

**Value Object и перегрузка операций в C#**

Практика на примере доменной модели `AuctionTrading`

- Синтаксис перегрузки операций
- Зачем нужна перегрузка: простые примеры
- Какие операции можно определять и правила
- Value Object: `Equals`, `GetHashCode`, `==`, `!=`
- Практика: `Money`, `Description`, валидаторы

---

## Слайд 2. Проблема: примитивы небезопасны

**Плохо:**
```csharp
decimal amount = -100.50m;          // отрицательная сумма — ошибка не поймана
decimal price  = 99.999m;           // три знака после запятой — ошибка не поймана
```

**Проблемы:**
- Валидация размазана по всему коду
- Компилятор не мешает передать описание туда, где ждут деньги
- Невозможно гарантировать инварианты

**Решение:** инкапсулировать значение и его правила в отдельный тип — **Value Object**

---

## Слайд 3. Value Object vs Entity

| Признак | Entity (Сущность) | Value Object (Объект-значение) |
|---|---|---|
| Идентичность | Есть ID | Нет ID |
| Определяется | Идентификатором | Значением |
| Изменяемость | Может меняться | **Неизменяем** |
| Равенство | По ID | По значению |
| Пример | `Order`, `User` | `Money`, `Description`, `Address` |

**Ключевое свойство Value Object — неизменяемость.** Любая «модификация» возвращает новый экземпляр.

---

## Слайд 4. Зачем нужен Value Object

- **Инкапсуляция валидации** — правила в одном месте
- **Самодокументируемый код** — `Transfer(Money from, Money to)` понятнее, чем `Transfer(decimal, decimal)`
- **Безопасность типов** — нельзя случайно передать `Description` вместо `Money`
- **Невозможность недопустимого состояния** — объект с плохим значением просто не создастся

**Вывод:** Value Object — это тип, который «защищает» своё значение.

---

## Слайд 5. Что такое перегрузка операций: синтаксис

**Определение:** перегрузка операций — это возможность задать собственное поведение для стандартных операторов (`+`, `-`, `==`, `>`, `<` и др.) применительно к вашему типу.

**Общий синтаксис:**

```csharp
public static <ВозвращаемыйТип> operator <Оператор>(<Параметр1>, <Параметр2>)
{
    // логика операции
}
```

**Требования к синтаксису:**
- метод **обязательно** `public static`
- ключевое слово `operator`, затем сам оператор
- хотя бы один параметр должен быть **того же типа**, что и содержащий класс
- возвращаемый тип — произвольный (часто тот же тип, что и класс)

**Простой пример:**

```csharp
public class Vector
{
    public int X { get; }
    public int Y { get; }

    public Vector(int x, int y) { X = x; Y = y; }

// Перегружаем оператор +: возвращаем новый Vector со сложенными координатами
    public static Vector operator +(Vector a, Vector b)
        => new Vector(a.X + b.X, a.Y + b.Y);
}
```

Использование — как с обычными числами:

```csharp
var sum = new Vector(1, 2) + new Vector(3, 4);   // sum = Vector(4, 6)
```

---

## Слайд 6. Зачем нужна перегрузка: простое сравнение

**Без перегрузки** — сравнение ссылок, а не значений:

```csharp
public class Point
{
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) { X = x; Y = y; }
}

var a = new Point(1, 2);
var b = new Point(1, 2);
a == b;   // false — сравниваются ссылки, а не координаты!
```

**С перегрузкой** — сравнение по значению:

```csharp
public class Point
{
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) { X = x; Y = y; }

// Перегружаем ==, чтобы сравнивать координаты, а не ссылки
    public static bool operator ==(Point a, Point b)
        => a.X == b.X && a.Y == b.Y;

// Правило C#: перегрузили == → обязаны перегрузить !=
    public static bool operator !=(Point a, Point b)
        => !(a == b);
}

var a = new Point(1, 2);
var b = new Point(1, 2);
a == b;   // true — координаты совпадают
```

**Вывод:** перегрузка делает поведение типа **интуитивным**. Клиентский код читается как обычная математика.

---

## Слайд 7. Зачем нужна перегрузка: арифметика

**Без перегрузки** — громоздкий код с методами:

```csharp
var result = Money.Add(Money.Subtract(a, b), c);
```

**С перегрузкой** — естественная запись:

```csharp
var result = a - b + c;
```

**Правило неизменяемости:** если тип неизменяемый (Value Object), оператор должен возвращать **новый экземпляр**, а не менять существующий:

```csharp
// ПЛОХО для Value Object: меняет исходный объект
public static Vector operator +(Vector a, Vector b)
{
    a.X += b.X;   // мутация!
    a.Y += b.Y;
    return a;
}

// ХОРОШО: создаёт новый объект
public static Vector operator +(Vector a, Vector b)
    => new Vector(a.X + b.X, a.Y + b.Y);
```

---

## Слайд 8. Какие операции можно переопределять

**Можно перегружать:**

| Категория | Операторы |
|---|---|
| Арифметические | `+`, `-`, `*`, `/`, `%` |
| Инкремент/декремент | `++`, `--` |
| Сравнения | `==`, `!=`, `<`, `>`, `<=`, `>=` |
| Логические | `!`, `&`, `|`, `^`, `true`, `false` |
| Побитовые | `&`, `|`, `^`, `~`, `<<`, `>>` |
| Приведения типов | `implicit`, `explicit` |

**Нельзя перегружать:**

- `=` (присваивание) — всегда ссылочное для ссылочных типов
- `&&`, `||` (логическое И/ИЛИ с коротким замыканием) — вычисляются через `true`/`false` и `&`/`|`
- `?:` (тернарный оператор)
- `.`, `->`, `new`, `is`, `as`, `sizeof`, `typeof`
- `[]` (индексатор) — это не оператор, а свойство

---

## Слайд 9. Правила перегрузки операций

1. **Метод обязан быть `public static`.**
2. **Хотя бы один параметр должен совпадать с типом класса** (или быть его наследником). Нельзя перегрузить `+` для двух `int`.
3. **Парные операторы:**
   - `==` и `!=` — перегружаются **только вместе**
   - `<` и `>` — только вместе
   - `<=` и `>=` — только вместе
   - Если перегрузили `==`, C# выдаст предупреждение, пока не переопределите `Equals` и `GetHashCode`
4. **`++` и `--`** перегружаются вместе (хотя можно и по отдельности, но это редкость).
5. **`true` и `false`** перегружаются вместе.
6. **Операторы `&&` и `||` не перегружаются напрямую**, но их поведение можно задать через `&`, `|` и `true`/`false`.
7. **Нельзя менять приоритет** операторов — он остаётся стандартным.

**Главное правило качества:** перегружайте оператор только там, где его смысл **очевиден** для типа. `+` для денег и векторов — да. `+` для `User` — нет.

---

## Слайд 10. Как это связано с Value Object

Value Object **идеально подходит** для перегрузки операций, потому что:

- **Неизменяем** → операторы возвращают новый экземпляр, а не мутируют
- **Определяется значением** → `==` и `!=` должны сравнивать по значению, а не по ссылке
- **Имеет предметный смысл** → `+` для `Money`, `>` для сравнения сумм

В нашем проекте это выражается в двух уровнях:

1. **Базовый класс `ValueObject<T>`** — перегружает `==` и `!=` один раз, для всех наследников
2. **Класс `Money`** — добавляет предметные `+`, `-`, `>`, `<`, `>=`, `<=`

Дальше — код.

---

## Слайд 11. Базовый класс `ValueObject<T>` — код (строки 1–18)

```csharp
// Подключаем исключения доменных объектов-значений
 1  using AuctionTrading.Domain.ValueObjects.Exceptions;
 2
// Пространство имён базовых классов
 3  namespace AuctionTrading.Domain.ValueObjects.Base
 4  {
// Абстрактный базовый класс; IEquatable<T> — типизированное сравнение без упаковки
 5      public abstract class ValueObject<T> : IEquatable<ValueObject<T>>
 6      {
// Хранимое значение; только get — неизменяемость
 7          public T Value { get; }
 8
// Внедрение валидатора через конструктор (Dependency Injection)
 9          protected ValueObject(IValidator<T> validator, T value)
10          {
// Защита от null-валидатора — раньше, чем что-либо ещё
11              if (validator == null)
12                  throw new ValidatorNullException(GetType().FullName ?? String.Empty, ExceptionMessages.VALIDATOR_MUST_BE_SPECIFIED);
// Валидация выполняется ДО сохранения значения
13              validator.Validate(value);
// Присваиваем только после успешной проверки — инвариант гарантирован
14              Value = value;
15          }
16
// Человекочитаемое представление: значение, либо имя типа
17          public override string ToString()
18              => Value!.ToString() ?? GetType().ToString();
```

---

## Слайд 12. Базовый класс `ValueObject<T>` — код (строки 19–35)

```csharp
19
// Хеш-код вычисляется по значению — согласован с Equals
20          public override int GetHashCode()
21              => Value!.GetHashCode();
22
// Перегрузка object.Equals: приводим к ValueObject<T> и делегируем
23          public override bool Equals(object? other)
24              => Equals(other as ValueObject<T>);
25
// Типизированное сравнение, реализующее IEquatable<ValueObject<T>>
26          public bool Equals(ValueObject<T>? other)
27          {
// null — не равны
28              if (other is null)
29                  return false;
// Тот же самый объект в памяти — равны (быстрая проверка + рефлексивность)
30              if (ReferenceEquals(this, other))
31                  return true;
// КРИТИЧНО: разные типы не равны (Money ≠ Description), даже если значения совпали
32              if (GetType() != other.GetType())
33                  return false;
// Финальное сравнение по внутреннему значению
34              return other.Value!.Equals(Value);
35          }
```

---

## Слайд 13. Базовый класс `ValueObject<T>` — код (строки 36–43)

```csharp
36
// operator == делегирует статическому Equals(object, object), который сам обрабатывает null
37          public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right)
38              => Equals(left, right);
39
// operator != выражается через ==, чтобы не дублировать логику
40          public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right)
41              => !(left == right);
42      }
43  }
```

**Правило C#:** перегрузили `==` → обязаны перегрузить `!=` (и наоборот).

---

## Слайд 14. Зачем переопределять `Equals` и `GetHashCode` вместе

**По умолчанию** для ссылочных типов:
```csharp
var a = new Money(100);
var b = new Money(100);
a == b;   // false — сравнение ссылок, а не значений!
```

**Мы переопределяем `Equals`,** чтобы сравнивать по значению.

**Контракт `GetHashCode` (обязателен!):**

> Если `a.Equals(b) == true`, то `a.GetHashCode() == b.GetHashCode()`

**Почему это критично для хеш-коллекций:**

```
Dictionary / HashSet:
  1) вычисляет хеш → находит «корзину»
  2) внутри корзины сравнивает через Equals
```

Если хеши разные, объект попадёт в **другую корзину** — коллекция его «не найдёт», хотя `Equals` вернёт `true`. **Коллекция сломается.**

```
хорошо:  a.Equals(b) → true   →   a.GetHashCode() == b.GetHashCode()
плохо:   a.Equals(b) → true   →   a.GetHashCode() != b.GetHashCode()   // BUG
```

**Обратное неверно:** одинаковый хеш ≠ равенство (возможны коллизии).

---

## Слайд 15. `GetHashCode` и `Equals`: почему вместе (схема)

```
┌─────────────────────────────────────────────────────┐
│   Dictionary<Money, string>                         │
│                                                     │
│   хеш-корзина #42  →  [Money(100), "ставка"]        │
│                                                     │
│   Поиск Money(100):                                 │
│     1. хеш = 42  →  корзина #42                     │
│     2. внутри корзины: Equals(Money(100), key)      │
│                                                     │
│   Если GetHashCode вернёт другое число —            │
│   объект пойдёт в другую корзину → НЕ НАЙДЁТСЯ      │
└─────────────────────────────────────────────────────┘
```

**Правило платформы .NET:** переопределил `Equals` — переопредели `GetHashCode`. Иначе — непредсказуемое поведение хеш-коллекций.

---

## Слайд 16. `Money` — арифметические операторы (строки 1–14)

```csharp
// Базовый класс и интерфейс валидатора
 1  using AuctionTrading.Domain.ValueObjects.Base;
// Пространство валидаторов
 2  using AuctionTrading.Domain.ValueObjects.Validators;
 3
// Пространство доменных объектов-значений
 4  namespace AuctionTrading.Domain.ValueObjects
 5  {
// Конструктор: принимает сумму в рублях, наследует ValueObject<decimal>
// и передаёт в base валидатор и уже округлённое значение
 6      public class Money(decimal amountInRub) : ValueObject<decimal>(
// Внедряем валидатор денежной суммы
 7          new MoneyAmountValidator(),
// Округляем ДО валидации — проверяем именно хранимое значение
 8          Math.Round(amountInRub, 2, MidpointRounding.AwayFromZero))
 9      {
// operator + возвращает НОВЫЙ объект (принцип неизменяемости)
10          public static Money operator +(Money m1, Money m2)
11              => new(m1.Value + m2.Value);
12
// operator - тоже возвращает новый объект; если результат ≤ 0 — конструктор выбросит исключение
13          public static Money operator -(Money m1, Money m2)
14              => new(m1.Value - m2.Value);
```

---

## Слайд 17. `Money` — операторы сравнения (строки 15–27)

```csharp
15
// operator > сравнивает внутренние значения
16          public static bool operator >(Money m1, Money m2)
17              => m1.Value > m2.Value;
18
// operator < симметричен
19          public static bool operator <(Money m1, Money m2)
20              => m1.Value < m2.Value;
21
// Правило C#: перегрузили < и > → обязаны перегрузить <= и >=
22          public static bool operator >=(Money m1, Money m2)
23              => m1.Value >= m2.Value;
24
// <= завершает набор; теперь естественно писать if (currentBid > minimumBid)
25          public static bool operator <=(Money m1, Money m2)
26              => m1.Value <= m2.Value;
27      }
```

---

## Слайд 18. Валидатор `MoneyAmountValidator` (строки 1–20)

```csharp
// Валидатор для decimal, реализующий IValidator<decimal>
 1  public class MoneyAmountValidator : IValidator<decimal>
 2  {
// Метод валидации вызывается из конструктора ValueObject<T>
 3      public void Validate(decimal value)
 4      {
// Сумма должна быть строго больше нуля
 5          if (value <= 0)
 6              throw new MoneyAmountNonPositiveException(
 7                  ExceptionMessages.MONEY_AMOUNT_NON_POSITIVE, nameof(value), value);
 8
// Проверяем, что знаков после запятой не больше двух
 9          if (!IsValidAmount(value))
10              throw new MoneyAmountHasMoreThanTwoDecimalPlacesException(
11                  ExceptionMessages.MONEY_AMOUNT_HAS_NOT_MORE_THEN_TWO_DECIMAL_PLACES, nameof(value), value);
12      }
13
// Вспомогательный метод проверки количества знаков после запятой
14      private bool IsValidAmount(decimal value)
15      {
// Умножаем на 100 — сдвигаем запятую на две позиции вправо
16          value = value * 100;
// Отбрасываем целую часть — остаётся только дробная
17          value -= (int)value;
// Если дробной части нет — знаков после запятой не больше двух
18          return value == 0m;
19      }
20  }
```

---

## Слайд 19. Валидатор `DescriptionValidator` и класс `Description` (строки 1–15)

```csharp
// Валидатор для строк, реализующий IValidator<string>
 1  public class DescriptionValidator : IValidator<string>
 2  {
// Метод валидации вызывается из конструктора ValueObject<string>
 3      public void Validate(string value)
 4      {
// IsNullOrWhiteSpace ловит null, "", и строки из одних пробелов
 5          if (string.IsNullOrWhiteSpace(value))
 6              throw new ArgumentNullOrWhiteSpaceException(
 7                  nameof(value), ExceptionMessages.TITLE_NOT_NULL_OR_WHITE_SPACE);
 8      }
 9  }
10
// Description — тоже Value Object, но для строки; свой валидатор
11  public class Description(string description)
12      : ValueObject<string>(new DescriptionValidator(), description)
13  {
14  }
```

**Вывод:** базовый класс универсален — логика сравнения/хеша переиспользуется.

---

## Слайд 20. Как это работает вместе (схема)

```
        IValidator<T>                       ValueObject<T>
        ┌──────────────┐                   ┌──────────────────────────┐
        │ Validate(T)  │◄──────────────────│ + Value                  │
        └──────────────┘                   │ + Equals / GetHashCode   │
               ▲                           │ + operator == / !=       │
               │                           └──────────────────────────┘
               │                                       ▲
     ┌─────────┴─────────┐                             │
     │                   │                             │
MoneyAmountValidator  DescriptionValidator      Money / Description
 (для decimal)         (для string)          (+ operator + - > < >= <=)
```

- **Валидаторы** — отвечают только за правила
- **`ValueObject<T>`** — за равенство, хеш, `ToString`
- **`Money`** — добавляет предметные операции

---

## Слайд 21. Итоги и best practices

1. **Перегрузка операций** — это `public static` метод с ключевым словом `operator`; хотя бы один параметр — тип класса
2. **Парные операторы** перегружаются вместе: `==`/`!=`, `<`/`>`, `<=`/`>=`, `++`/`--`, `true`/`false`
3. **Перегрузили `==` → переопределите `Equals` и `GetHashCode`** (контракт .NET)
4. **Value Object — неизменяемый объект без ID**, определяемый значением
5. **В `Equals` проверяйте `GetType()`** — защита от сравнения разнородных типов
6. **Операторы возвращают новый экземпляр** — сохраняем неизменяемость
7. **Валидация до присваивания** — недопустимое состояние невозможно
8. **Рассмотрите `record`** для новых Value Objects (но не для Entity EF Core)

---

## Слайд 22. Вопросы аудитории

1. Что произойдёт, если в `Money` добавить `Currency` и забыть обновить `Equals`?
   *→ Две суммы в разных валютах с одинаковым числом будут считаться равными. `record` решил бы это автоматически.*

2. Почему `operator +` возвращает `new Money(...)`, а не меняет `m1.Value`?
   *→ Принцип неизменяемости Value Object.*

3. Почему нельзя переопределить только `Equals` без `GetHashCode`?
   *→ Сломается контракт хеш-коллекций: равные объекты должны иметь одинаковый хеш.*

4. Зачем проверка `GetType() != other.GetType()` в `Equals`?
   *→ Чтобы `Money(100)` и `Description("100")` никогда не были равны.*

5. Почему нельзя перегрузить `&&` и `||` напрямую?
   *→ Они вычисляются через `&`, `|` и `true`/`false` с коротким замыканием.*

6. Почему нельзя использовать `record` для сущностей EF Core?
   *→ EF Core опирается на ссылочное равенство для отслеживания изменений.*

---

## Слайд 23. Спасибо за внимание!

**Ключевая мысль:**

> Перегрузка операций делает тип естественным в использовании,
> а Value Object делает недопустимое состояние невозможным.

**Контакты / вопросы:** ___
