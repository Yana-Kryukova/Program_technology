# Задание: Обмен сообщениями через облачный RabbitMQ

**Дисциплина:** Технологии программирования
**Технологии:** C#, .NET, RabbitMQ (CloudAMQP)
**Тема:** Очереди сообщений, паттерны Point-to-Point и Work Queue

---

## 📌 Общее описание

Задание состоит из двух частей:

- **Часть 1 — Практика (на занятии):** регистрация в облачном брокере и разработка приложения-**публикатора** сообщений.
- **Часть 2 — Домашнее задание:** разработка приложения-**потребителя**, которое читает сообщения из очереди. По сценарию «заказ такси» запускаются **два** потребителя, и сообщения распределяются между ними.

Итоговый проект — это **система из трёх приложений**:

```mermaid
flowchart LR
    Client["🚕 TaxiClient<br/><i>(публикатор)</i><br/>Часть 1"]
    Queue[["📬 Очередь<br/><b>taxi_orders</b><br/>CloudAMQP"]]
    D1["🚖 TaxiDriver #1<br/><i>(потребитель)</i>"]
    D2["🚖 TaxiDriver #2<br/><i>(потребитель)</i>"]

    Client -- "заказ (BasicPublish)" --> Queue
    Queue -- "заказ (BasicConsume)" --> D1
    Queue -- "заказ (BasicConsume)" --> D2

    D1 -. "BasicAck" .-> Queue
    D2 -. "BasicAck" .-> Queue

    classDef publisher fill:#dbeafe,stroke:#1e40af,stroke-width:2px,color:#1e3a8a
    classDef broker    fill:#fef3c7,stroke:#b45309,stroke-width:2px,color:#78350f
    classDef consumer  fill:#dcfce7,stroke:#15803d,stroke-width:2px,color:#14532d

    class Client publisher
    class Queue broker
    class D1,D2 consumer
```

---

## 🎯 Цели

1. Научиться работать с брокером сообщений в облаке без локальной инфраструктуры.
2. Освоить паттерн **публикатор–очередь–потребитель**.
3. Понять распределение сообщений между несколькими потребителями (паттерн **Work Queue**).
4. Разобраться в подтверждениях (`ack`) и справедливой раздаче (`prefetch`).

---

## Часть 1. Практика (на занятии) — Клиент-публикатор

### 1.1. Регистрация в облачном RabbitMQ

1. Зарегистрироваться на [customer.cloudamqp.com/signup](https://customer.cloudamqp.com/signup) (бесплатно).
2. Создать экземпляр: **Create New Instance**, план **Little Lemur (Free)**, выбрать ближайший регион.
3. Дождаться запуска (1–2 минуты).
4. Открыть экземпляр и скопировать **URL для подключения**, вида:
   ```
   amqps://user:pass@hostname/vhost
   ```
   Этот URL понадобится во всех трёх приложениях.

### 1.2. Создание проекта «Клиент такси»

```bash
dotnet new console -n TaxiClient
cd TaxiClient
dotnet add package RabbitMQ.Client --version 6.8.1
```

> ⚠️ Версия пакета фиксируется на **6.8.1**, чтобы код ниже компилировался без правок (в версии 7.x API стал асинхронным).

### 1.3. Код публикатора (`Program.cs`)

```csharp
using RabbitMQ.Client;
using System.Text;

string url = "amqps://ВАШ_ПОЛЬЗОВАТЕЛЬ:ВАШ_ПАРОЛЬ@ХОСТ/ВАШ_VHOST";
var factory = new ConnectionFactory { Uri = new Uri(url) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string queueName = "taxi_orders";

channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, null);

var props = channel.CreateBasicProperties();
props.Persistent = true;

Console.WriteLine("Клиент такси. Введите адрес (или 'exit'):");
while (true)
{
    var order = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(order) || order.ToLower() == "exit") break;

    var body = Encoding.UTF8.GetBytes(order);
    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: props, body: body);
    Console.WriteLine($" [x] Заказ отправлен: {order}");
}
```

### 1.4. Проверка на занятии

1. Запустить `dotnet run`.
2. Отправить 2–3 сообщения (адреса).
3. В панели CloudAMQP → вкладка **Queues** → очередь `taxi_orders` — убедиться, что сообщения накопились (счётчик **Ready**).
4. Открыть очередь → **Get messages** — увидеть текст отправленных сообщений.

✅ **Результат практики:** клиент отправляет сообщения, они видны в брокере.

---

## Часть 2. Домашнее задание — Два потребителя «Такси»

### 2.1. Задача

Реализовать **второе приложение** — `TaxiDriver`, которое:

- подключается к той же очереди `taxi_orders` и **читает** сообщения;
- обрабатывает каждое сообщение (имитация поездки — задержка 2 секунды);
- подтверждает выполнение через `BasicAck`;
- при запуске **двух экземпляров** приложения заказы распределяются между ними (round-robin).

### 2.2. Создание проекта

```bash
dotnet new console -n TaxiDriver
cd TaxiDriver
dotnet add package RabbitMQ.Client --version 6.8.1
```

### 2.3. Код потребителя (`Program.cs`)

```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

string taxiName = args.Length > 0 ? args[0] : "Такси-1";

string url = "amqps://ВАШ_ПОЛЬЗОВАТЕЛЬ:ВАШ_ПАРОЛЬ@ХОСТ/ВАШ_VHOST";
var factory = new ConnectionFactory { Uri = new Uri(url) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string queueName = "taxi_orders";

channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine($" [{taxiName}] На линии, жду заказы...");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, ea) =>
{
    var order = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($" [{taxiName}] Принял заказ: {order}");

    Thread.Sleep(2000); // имитация поездки

    Console.WriteLine($" [{taxiName}] Завершил заказ: {order}");
    channel.BasicAck(ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

Console.WriteLine("Enter — выйти");
Console.ReadLine();
```

### 2.4. Что нужно продемонстрировать

1. **Два таксиста в двух терминалах:**
   ```bash
   dotnet run --project TaxiDriver -- "Такси-1"
   dotnet run --project TaxiDriver -- "Такси-2"
   ```
2. **Клиент в третьем терминале** — отправляет 4–6 заказов.
3. В консолях таксистов видно, что заказы распределились между ними попеременно.
4. В панели CloudAMQP у очереди `taxi_orders`:
   - пока таксист «едет», у очереди **Unacked = 1**;
   - после `BasicAck` сообщение удаляется, **Ready** уменьшается.

---

## 🧠 Вопросы для защиты

Ответы на эти вопросы нужно кратко (2–3 предложения) написать в отчёте:

1. Чем отличается `autoAck: true` от `autoAck: false` в `BasicConsume`? Что произойдёт, если таксист упадёт, не вызвав `BasicAck`?
2. Зачем нужен `channel.BasicQos(prefetchCount: 1)`? Что изменится, если убрать эту строку и запустить двух таксистов с разной скоростью обработки?
3. Почему в `QueueDeclare` указано `durable: true`, а у сообщений `props.Persistent = true`? Что произойдёт с очередью и сообщениями при перезапуске брокера, если эти флаги убрать?
4. Зачем в задании две отдельные программы (`TaxiClient` и `TaxiDriver`), а не одна?
5. Что такое `routingKey` в вызове `BasicPublish` и почему он совпадает с именем очереди?

---

## 📦 Что сдавать

1. **Исходный код** трёх приложений (`TaxiClient`, `TaxiDriver`) — архив или ссылка на репозиторий (GitHub/GitLab).
2. **Скриншоты:**
   - панель CloudAMQP с созданным экземпляром;
   - вкладка **Queues** с очередью `taxi_orders` и сообщениями (**Ready** и **Unacked**);
   - два терминала с таксистами и третий с клиентом — одновременно, с видимым распределением заказов.
3. **Короткий отчёт** (1–2 страницы):
   - описание архитектуры (схема из трёх приложений и очереди);
   - ответы на 5 вопросов для защиты;
   - описание одного эксперимента на выбор: удаление `BasicQos`, удаление `autoAck:false`, убийство таксиста посреди поездки — что наблюдали.

---

## 📊 Критерии оценивания

| Критерий | Баллы |
|---|---|
| Регистрация и рабочий публикатор (часть 1) | 20 |
| Корректный потребитель с `BasicAck` (часть 2) | 30 |
| Демонстрация распределения между двумя таксистами | 20 |
| Ответы на вопросы для защиты | 20 |
| Отчёт с экспериментом и схемой | 10 |
| **Итого** | **100** |

---

## ⏱️ Тайминг

| Этап | Время |
|---|---|
| Регистрация и создание экземпляра (на занятии) | 5–7 мин |
| Разработка и запуск клиента (на занятии) | 10–15 мин |
| Разработка потребителя (дома) | 10–15 мин |
| Запуск и проверка двух таксистов (дома) | 5 мин |
| Эксперименты и отчёт (дома) | 10–15 мин |

---

## 🔧 Частые проблемы и решения

| Проблема | Решение |
|---|---|
| `BrokerUnreachableException` при подключении | Проверить URL, что порт **5671** (amqps, TLS), а не 5672. На бесплатном тарифе CloudAMQP только TLS. |
| `QueueDeclare` кидает `PRECONDITION_FAILED` | Очередь уже создана с другими параметрами (`durable`, `exclusive` и т.д.). Удалить очередь в панели CloudAMQP и создать заново с теми же параметрами, что в коде. |
| Код не компилируется с `EventingBasicConsumer` | Проверить версию пакета: `dotnet list package`. Должна быть `6.8.1`. В 7.x другой API. |
| Один таксист забирает все заказы | Забыли `channel.BasicQos(prefetchCount: 1)` или стоит `autoAck: true`. |
| Сообщения не появляются в панели | Возможно, потребитель их сразу съедает. Остановить таксистов и отправить заново. |

---

## 💡 Рекомендуемая литература

- Официальный туториал RabbitMQ: [rabbitmq.com/tutorials/tutorial-two-dotnet](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html) — паттерн Work Queue на C#.
- Документация CloudAMQP: [cloudamqp.com/docs](https://www.cloudamqp.com/docs/index.html).
- Про `BasicQos` и справедливую раздачу — в том же туториале, раздел «Fair dispatch».

Удачи! 🚕
