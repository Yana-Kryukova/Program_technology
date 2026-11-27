# Задание 1. Заказы

**Время:** 40-50 минут. 

## Что нужно сделать

1. **Разобраться.** Над классом и над каждым его методом напишите XML-комментарий (`/// <summary>`, при необходимости `<param>` и `<returns>`). Опишите, **что** делает код и зачем, а не **как** (не «проходит циклом по списку»).
2. **Доработать** код под требования заказчика (ниже). Названия приёмов и типов в требованиях не указаны, выбирайте сами.
3. **Показать в `Main`**, что всё работает (сценарий ниже). Тестовые данные создайте сами.
4. **Ответить письменно на 3 вопроса** в блоке `#region Ответы на вопросы` в конце класса `OrderService`: под каждым вопросом, после слова «Ответ:». Блок уже есть в заготовке. Если вы переименовали или заменили класс, перенесите блок в итоговый класс.

## Требования заказчика

- Завтра попросят отбирать заказы по городу и статусу, клиентов по email и т. д. Копировать метод под каждое условие нельзя.
- Способ уведомления выбирает вызывающий код. Позже добавятся push и Telegram, `OrderService` при этом менять нельзя.
- Статистика и аудит должны узнавать об обработанном заказе, но `OrderService` про них знать не должен.

## Что показать в `Main`

- заказы дороже 5000 из Берлина;
- клиентов с почтой на `@gmail.com`;
- обработку заказов с уведомлением в консоль и отдельно с уведомлением другим способом, который вы придумаете;
- подсчёт обработанных заказов подключённым снаружи модулем и ещё один независимый модуль, который записывает номера обработанных заказов в список.

## Заготовка кода

```csharp
using System;
using System.Collections.Generic;

public class Order { public int Id; public decimal Price; public string City; public string Status; }
public class Customer { public string Name; public int Age; public string Email; }

public class OrderService
{
    public List<Order> GetExpensive(List<Order> orders)
    {
        var r = new List<Order>();
        foreach (var o in orders) if (o.Price > 1000) r.Add(o);
        return r;
    }

    public List<Customer> GetAdults(List<Customer> customers)
    {
        var r = new List<Customer>();
        foreach (var c in customers) if (c.Age >= 18) r.Add(c);
        return r;
    }

    public void Process(List<Order> orders, string mode)
    {
        foreach (var o in orders)
        {
            if (mode == "email") Console.WriteLine("Email: " + o.Id);
            else if (mode == "sms") Console.WriteLine("SMS: " + o.Id);
            o.Status = "Processed";
        }
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Func, Action и Predicate<T> отличаются друг от друга?
    // Ответ:
    //
    // Вопрос 2. Чем event отличается от публичного поля-делегата?
    // Ответ:
    //
    // Вопрос 3. Зачем при вызове события пишут ?.Invoke(...), а не вызывают его напрямую?
    // Ответ:
    //
    #endregion
}

public class Program
{
    public static void Main()
    {
        // Ваш сценарий
    }
}
```

## Что сдавать

Один файл `Program.cs` (или проект целиком): доработанные классы с комментариями, `Main` и заполненный блок с ответами.
