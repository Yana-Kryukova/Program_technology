# События в C#: теория и практическая лабораторная

## Содержание

1. [Введение](#введение)
2. [Теоретическая часть: что такое события](#теоретическая-часть)
3. [Синтаксис событий](#синтаксис-событий)
4. [Лабораторная работа: реализация событий](#лабораторная-работа)
5. [Замечания к лабораторной](#замечания-к-лабораторной)
6. [Полный код программы с комментариями](#полный-код-программы)

---

## Введение

Данный документ объединяет теоретическую базу по событиям в C# и практическую лабораторную работу от Microsoft Learning. Материал построен так, чтобы сначала дать понятия и синтаксис, а затем показать их применение в реальном коде.

**Цель обучения:** научиться объявлять события, поднимать их, создавать собственные классы данных события, подписываться на события и отписываться от них.

**Требования:** базовое знание C#, понимание делегатов.

---

## Теоретическая часть

### Что такое событие?

**Событие (`event`)** — это член класса, который позволяет объекту (издателю) уведомлять другие объекты (подписчиков) о том, что произошло что-то важное. Событие основано на **делегате** и реализует шаблон проектирования **«издатель — подписчик»** (publisher-subscriber) или **«наблюдатель»** (Observer).

**Ключевые роли:**

- **Издатель** (publisher) — объект, который поднимает событие. Он не знает, кто на него подписан.
- **Подписчик** (subscriber) — объект, который предоставляет метод-обработчик и подписывается на событие.
- **Обработчик** (event handler) — метод, который вызывается, когда событие происходит.
- **Данные события** (`EventArgs`) — объект, содержащий дополнительную информацию о произошедшем.

Событие — это обёртка над делегатом с ограничениями доступа: снаружи класса можно только подписываться (`+=`) и отписываться (`-=`). Нельзя напрямую присвоить событию `null` или вызвать его как обычный делегат. Это защищает инкапсуляцию.

### Зачем нужны события?

1. **Слабая связанность (loose coupling).** Издатель не зависит от конкретных классов подписчиков. Он просто сообщает: «событие произошло».
2. **Реакция на изменения.** Классический пример — UI: кнопка не знает, что делать при нажатии; она лишь поднимает `Click`, а обработчики решают.
3. **Реализация Observer.** Позволяет множеству объектов следить за состоянием одного объекта без жёстких ссылок.
4. **Отказ от опроса (polling).** Вместо постоянной проверки «не случилось ли чего?» подписчик получает уведомление только когда событие реально произошло.
5. **Расширяемость.** Новых подписчиков можно добавлять без изменения кода издателя.
6. **Асинхронные и отложенные уведомления.** События можно обрабатывать в другом потоке, через очередь и т.п.

---

## Синтаксис событий

### Объявление делегата (если не используется стандартный)

```csharp
public delegate void MyEventHandler(object sender, MyEventArgs e);
```

### Объявление события

```csharp
// Свой делегат:
public event MyEventHandler MyEvent;

// Стандартный делегат без данных:
public event EventHandler MyEvent;

// Стандартный обобщённый делегат с данными:
public event EventHandler<MyEventArgs> MyEvent;
```

`EventHandler` — встроенный делегат с сигнатурой:

```csharp
public delegate void EventHandler(object? sender, EventArgs e);
```

`EventHandler<TEventArgs>` — обобщённая версия:

```csharp
public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);
```

### Поднятие события (только внутри класса-издателя)

```csharp
protected virtual void OnMyEvent(MyEventArgs e)
{
    MyEvent?.Invoke(this, e);
}
```

`?.Invoke` защищает от `NullReferenceException`, если подписчиков нет. Альтернатива — инициализировать событие пустым делегатом:

```csharp
public event EventHandler MyEvent = delegate { };
```

### Подписка и отписка (снаружи класса)

```csharp
obj.MyEvent += Handler;   // подписка
obj.MyEvent -= Handler;   // отписка
```

Извне **нельзя**:

```csharp
obj.MyEvent = null;       // ошибка компиляции
obj.MyEvent();            // ошибка компиляции
```

### Метод-обработчик

```csharp
void Handler(object? sender, MyEventArgs e)
{
    // реакция на событие
}
```

`sender` — источник события (издатель), `e` — данные события.

### Кастомные данные события

```csharp
public class MyEventArgs : EventArgs
{
    public int Value { get; set; }
    public DateTime Time { get; set; }
}
```

Наследование от `EventArgs` — конвенция .NET.

### Полный скелет события

```csharp
// 1. Данные события
public class MyEventArgs : EventArgs
{
    public int Value { get; set; }
}

// 2. Издатель
public class Publisher
{
    // 3. Событие
    public event EventHandler<MyEventArgs> MyEvent;

    // 4. Метод поднятия
    protected virtual void OnMyEvent(MyEventArgs e)
    {
        MyEvent?.Invoke(this, e);
    }

    // 5. Что-то, что вызывает событие
    public void DoSomething(int value)
    {
        OnMyEvent(new MyEventArgs { Value = value });
    }
}

// 6. Подписчик
class Program
{
    static void Main()
    {
        var pub = new Publisher();
        pub.MyEvent += HandleMyEvent; // подписка
        pub.DoSomething(42);
        pub.MyEvent -= HandleMyEvent; // отписка
    }

    static void HandleMyEvent(object? sender, MyEventArgs e)
    {
        Console.WriteLine($"Произошло событие со значением {e.Value}");
    }
}
```

---

## Лабораторная работа

### Сценарий

Вы создаёте простое консольное приложение, которое отслеживает значение счётчика и **поднимает событие**, когда счётчик превышает заданный порог. Это демонстрация событийно-ориентированной модели .NET.

---

### Task 1. Создание проекта и класса `Counter`

**Цель:** завести новый консольный проект и определить класс `Counter` со свойствами для хранения текущего значения и порога.

**Пошагово:**

1. Создайте новый консольный проект:
   - Visual Studio: **File → New → Project → Console App (.NET Core)**.
   - VS Code / терминал:
     ```bash
     dotnet new console -n CounterApp
     cd CounterApp
     ```

2. Добавьте файл `Counter.cs` и определите класс:

```csharp
public class Counter
{
    public int Total { get; private set; }
    public int Threshold { get; set; }

    public Counter(int threshold)
    {
        Threshold = threshold;
        Total = 0;
    }

    public void Add(int value)
    {
        Total += value;
        Console.WriteLine($"Current Total: {Total}");
    }
}
```

**Пояснения:**

- `Total` — текущее значение счётчика. Сеттер `private`, потому что менять его снаружи напрямую нельзя — только через `Add`.
- `Threshold` — порог, при достижении которого должно сработать событие.
- `Add` — единственный публичный способ увеличить счётчик.

**Проверка:** класс компилируется, `Add` увеличивает `Total` и печатает его.

---

### Task 2. Объявление и поднятие события `ThresholdReached`

**Цель:** объявить событие `ThresholdReached` через делегат `EventHandler` и реализовать метод, который поднимает это событие при превышении порога.

**Пошагово:**

1. В классе `Counter` объявите событие:

```csharp
public event EventHandler ThresholdReached = delegate { };
```

2. Добавьте `protected virtual` метод `OnThresholdReached`:

```csharp
protected virtual void OnThresholdReached(EventArgs e)
{
    ThresholdReached?.Invoke(this, e);
}
```

3. Измените `Add`, чтобы проверять превышение порога и вызывать `OnThresholdReached`:

```csharp
public void Add(int value)
{
    Total += value;
    Console.WriteLine($"Current Total: {Total}");

    if (Total >= Threshold)
    {
        OnThresholdReached(EventArgs.Empty);
    }
}
```

4. В `Program.cs` подпишитесь на событие и проверьте работу:

```csharp
var counter = new Counter(5);

counter.ThresholdReached += (sender, e) =>
{
    Console.WriteLine("Threshold reached!");
};

counter.Add(3);
counter.Add(2); // здесь должно сработать событие
```

**Пояснения:**

- `public event EventHandler ThresholdReached` — стандартный шаблон .NET. `EventHandler` — встроенный делегат с сигнатурой `(object? sender, EventArgs e)`.
- `= delegate { };` — инициализация «пустым» делегатом, чтобы избежать `NullReferenceException` при вызове без подписчиков.
- `OnThresholdReached` — метод-подниматель. Единая точка, где событие вызывается. Это стандартная практика.
- `Total >= Threshold` — срабатывает **каждый раз**, когда порог достигнут или превышен. В текущем коде нет защиты от повторного срабатывания.

**Ожидаемый вывод:**

```
Current Total: 3
Current Total: 5
Threshold reached!
```

---

### Task 3. Создание класса данных события

**Цель:** создать класс `ThresholdReachedEventArgs`, чтобы передать в событии дополнительную информацию — значение порога и время его достижения.

**Пошагово:**

1. Добавьте файл `ThresholdReachedEventArgs.cs`:

```csharp
public class ThresholdReachedEventArgs : EventArgs
{
    public int Threshold { get; set; }
    public DateTime TimeReached { get; set; }
}
```

2. Обновите объявление события в `Counter`:

```csharp
public event EventHandler ThresholdReached;
```

> Примечание: корректнее было бы `public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;` — тогда сигнатура обработчика была бы типизированной. Но в исходной лабораторной используется необобщённый `EventHandler`.

3. Обновите `OnThresholdReached`:

```csharp
protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
{
    ThresholdReached?.Invoke(this, e);
}
```

4. Обновите `Add`:

```csharp
public void Add(int value)
{
    Total += value;
    Console.WriteLine($"Current Total: {Total}");

    if (Total >= Threshold)
    {
        var args = new ThresholdReachedEventArgs
        {
            Threshold = Threshold,
            TimeReached = DateTime.Now
        };
        OnThresholdReached(args);
    }
}
```

**Пояснения:**

- Наследование от `EventArgs` — конвенция .NET для классов данных события.
- `Threshold` и `TimeReached` — та информация, которую подписчик получит в обработчике.
- `OnThresholdReached` теперь принимает `ThresholdReachedEventArgs`, а не `EventArgs.Empty`.

**Проверка:** класс `ThresholdReachedEventArgs` определён, событие передаёт дополнительные данные.

---

### Task 4. Написание метода-обработчика

**Цель:** написать метод в `Program.cs`, который реагирует на `ThresholdReached` и выводит сообщение.

**Пошагово:**

1. В `Program.cs` создайте счётчик:

```csharp
var counter = new Counter(10);
```

2. Подпишитесь на событие через `+=`:

```csharp
counter.ThresholdReached += Counter_ThresholdReached;
```

3. Определите обработчик:

```csharp
static void Counter_ThresholdReached(object? sender, ThresholdReachedEventArgs e)
{
    Console.WriteLine($"Threshold of {e.Threshold} reached at {e.TimeReached}.");
}
```

**Пояснения:**

- `sender` — тот, кто поднял событие (`Counter`). Может быть `null`, поэтому `object?`.
- `e` — типизированный `ThresholdReachedEventArgs`. Именно здесь видно, зачем нужен кастомный класс: обработчик получает `Threshold` и `TimeReached` без дополнительных запросов к источнику.
- Имя обработчика `Counter_ThresholdReached` — конвенция `Sender_EventName`.

**Ожидаемый вывод при достижении порога:**

```
Threshold of 10 reached at 22.04.2025 8:54:42.
```

---

### Task 5. Подписка и отписка от события

**Цель:** программно подписаться и отписаться от `ThresholdReached` через `+=` и `-=`, проверить работу в цикле.

**Пошагово:**

1. В `Main` подпишитесь (как в Task 4).

2. Добавьте цикл чтения клавиш:

```csharp
Console.WriteLine("Press 'a' to add 1 to the counter or 'q' to quit.");
while (true)
{
    var key = Console.ReadKey(true).KeyChar;
    if (key == 'a')
    {
        counter.Add(1);
    }
    else if (key == 'q')
    {
        break;
    }
}
```

3. Перед выходом отпишитесь:

```csharp
counter.ThresholdReached -= Counter_ThresholdReached;
```

**Пояснения:**

- `Console.ReadKey(true)` — не отображает нажатую клавишу в консоли.
- `+=` и `-=` — те же операторы, что и для делегатов. Событие — это обёртка над делегатом, и подписка/отписка работает так же.
- Отписка важна для долгоживущих объектов: если подписчик не отпишется, издатель будет держать ссылку на него и не даст сборщику мусора его удалить. Для консольного приложения это не критично, но привычку стоит формировать.
- В исходном задании цикл `while (true)` с `break` — простой способ дать пользователю управлять счётчиком вручную.

**Проверка:** нажимая `a`, вы видите `Current Total`. При достижении порога срабатывает событие. После `q` программа выходит, и отписка выполнена.

---

## Замечания к лабораторной

1. **Несогласованность `EventHandler` vs `EventHandler<T>`.** В Task 2 объявляется `EventHandler`, в Task 3 — тоже `EventHandler`, но передаётся `ThresholdReachedEventArgs`. Это работает, но обработчик в Task 4 ожидает `ThresholdReachedEventArgs`, а не `EventArgs`. В реальном коде лучше с самого начала использовать `EventHandler<ThresholdReachedEventArgs>`.

2. **Событие срабатывает многократно.** Если после достижения порога продолжать вызывать `Add`, событие будет срабатывать на каждое добавление. В задании это не оговаривается, но на практике часто нужно однократное срабатывание или сброс флага.

3. **`delegate { }` vs `?.Invoke`.** В Task 2 используются оба подхода: инициализация пустым делегатом и `?.Invoke`. Достаточно одного. `?.Invoke` — более современный и идиоматичный вариант.

4. **Отписка в конце.** В консольном приложении это не обязательно, но в долгоживущих приложениях (WPF, ASP.NET) — критично. Задание правильно показывает `-=`, но не объясняет, зачем.

---

## Полный код программы

Ниже — полный, рабочий вариант программы с комментариями. Код исправляет несогласованность `EventHandler` / `EventHandler<T>` и объединяет все пять задач в единое решение.

### Файл `ThresholdReachedEventArgs.cs`

```csharp
using System;

/// <summary>
/// Класс данных события, которое поднимается при достижении порога счётчиком.
/// Наследуется от EventArgs согласно конвенции .NET для классов данных события.
/// </summary>
public class ThresholdReachedEventArgs : EventArgs
{
    /// <summary>
    /// Значение порога, при достижении которого сработало событие.
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// Момент времени, когда порог был достигнут.
    /// </summary>
    public DateTime TimeReached { get; set; }
}
```

### Файл `Counter.cs`

```csharp
using System;

/// <summary>
/// Издатель (publisher). Класс-счётчик, который поднимает событие
/// ThresholdReached, когда накопленное значение достигает заданного порога.
/// </summary>
public class Counter
{
    /// <summary>
    /// Текущее накопленное значение счётчика.
    /// Сеттер закрыт (private), изменить значение можно только через метод Add.
    /// </summary>
    public int Total { get; private set; }

    /// <summary>
    /// Порог, при достижении которого поднимается событие.
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// Событие, уведомляющее подписчиков о достижении порога.
    /// Типизированная версия EventHandler&lt;TEventArgs&gt; обеспечивает
    /// строгую типизацию аргумента в обработчике.
    /// </summary>
    public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

    /// <summary>
    /// Конструктор счётчика.
    /// </summary>
    /// <param name="threshold">Порог, при котором должно сработать событие.</param>
    public Counter(int threshold)
    {
        Threshold = threshold;
        Total = 0;
    }

    /// <summary>
    /// Увеличивает значение счётчика на заданную величину.
    /// Если новое значение достигает или превышает порог — поднимает событие.
    /// </summary>
    /// <param name="value">Величина, на которую увеличивается счётчик.</param>
    public void Add(int value)
    {
        // Увеличиваем накопленное значение.
        Total += value;

        // Отладочный вывод текущего значения.
        Console.WriteLine($"Current Total: {Total}");

        // Проверяем достижение порога.
        if (Total >= Threshold)
        {
            // Формируем объект данных события.
            var args = new ThresholdReachedEventArgs
            {
                Threshold = Threshold,
                TimeReached = DateTime.Now
            };

            // Поднимаем событие через защищённый метод-подниматель.
            OnThresholdReached(args);
        }
    }

    /// <summary>
    /// Метод-подниматель события. Единая точка, где событие ThresholdReached
    /// вызывается. Объявлен protected virtual, чтобы наследники могли
    /// переопределить логику поднятия события.
    /// </summary>
    /// <param name="e">Аргументы события.</param>
    protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
    {
        // ?.Invoke защищает от NullReferenceException, если подписчиков нет.
        ThresholdReached?.Invoke(this, e);
    }
}
```

### Файл `Program.cs`

```csharp
using System;

/// <summary>
/// Подписчик (subscriber). Точка входа в приложение.
/// Создаёт счётчик, подписывается на событие ThresholdReached,
/// даёт пользователю возможность увеличивать счётчик нажатием клавиши 'a'
/// и корректно отписывается перед выходом.
/// </summary>
class Program
{
    static void Main()
    {
        // 1. Создаём экземпляр издателя (счётчик) с порогом 10.
        var counter = new Counter(10);

        // 2. Подписываемся на событие ThresholdReached.
        //    Обработчик — отдельный метод Counter_ThresholdReached.
        counter.ThresholdReached += Counter_ThresholdReached;

        // 3. Информируем пользователя о правилах управления.
        Console.WriteLine("Press 'a' to add 1 to the counter or 'q' to quit.");

        // 4. Основной цикл: читаем клавиши без эха в консоли.
        while (true)
        {
            var key = Console.ReadKey(true).KeyChar;

            if (key == 'a')
            {
                // Увеличиваем счётчик. При достижении порога сработает событие.
                counter.Add(1);
            }
            else if (key == 'q')
            {
                // Пользователь решил выйти.
                break;
            }
        }

        // 5. Отписываемся от события перед завершением работы.
        //    Это освобождает ссылку издателя на подписчика и позволяет
        //    сборщику мусора удалить подписчика при необходимости.
        counter.ThresholdReached -= Counter_ThresholdReached;

        Console.WriteLine("Goodbye!");
    }

    /// <summary>
    /// Обработчик события ThresholdReached.
    /// Вызывается издателем, когда накопленное значение счётчика достигает порога.
    /// </summary>
    /// <param name="sender">Источник события (экземпляр Counter).</param>
    /// <param name="e">Данные события: значение порога и время достижения.</param>
    static void Counter_ThresholdReached(object? sender, ThresholdReachedEventArgs e)
    {
        Console.WriteLine($"Threshold of {e.Threshold} reached at {e.TimeReached}.");
    }
}
```

### Как запустить

1. Создайте новый консольный проект:
   ```bash
   dotnet new console -n CounterApp
   cd CounterApp
   ```
2. Замените содержимое `Program.cs` на код выше.
3. Создайте файлы `Counter.cs` и `ThresholdReachedEventArgs.cs` в том же каталоге.
4. Запустите:
   ```bash
   dotnet run
   ```
5. Нажимайте `a`, пока значение не дойдёт до 10 — сработает событие. Нажмите `q` для выхода.

### Ожидаемое поведение

```
Press 'a' to add 1 to the counter or 'q' to quit.
Current Total: 1
Current Total: 2
...
Current Total: 10
Threshold of 10 reached at 22.04.2025 8:54:42.
Current Total: 11
Threshold of 10 reached at 22.04.2025 8:54:45.
...
Goodbye!
```

Обратите внимание: событие срабатывает **при каждом** последующем увеличении после достижения порога. Если нужно однократное срабатывание — добавьте в класс `Counter` приватный флаг `bool _thresholdNotified;` и проверяйте его в `Add`.

---

## Итог

Документ объединяет теорию и практику:

1. **Определения** — что такое событие, издатель, подписчик, обработчик, данные события.
2. **Назначение** — слабая связанность, реакция на изменения, отказ от опроса.
3. **Синтаксис** — объявление, поднятие, подписка, отписка, кастомные `EventArgs`.
4. **Лабораторная работа** — пять задач, последовательно строящих событийную архитектуру.
5. **Полный код** — готовое к запуску решение с документированными комментариями.

Этого достаточно, чтобы осознанно читать и писать код с событиями в C#, а также применять шаблон «издатель — подписчик» в реальных проектах.
