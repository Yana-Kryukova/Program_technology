### Последовательность показа

1. `appsettings.json` — откуда берём настройки (облако CloudAMQP).
2. `Consumer.csproj` — пакеты и подключение конфига (с уточнением про `Always` vs `PreserveNewest`).
3. `MessageDto.cs` — структура сообщения (должна совпадать с Producer).
4. `ApplicationSettings.cs` + `RmqSettings.cs` — маппинг JSON в C#-объекты.
5. `Consumer.cs` — регистрация очереди, привязка к обменнику, обработка с ручным `Ack` (с уточнениями про `Qos`, `autoAck`, `BasicAck`, `Thread.Sleep`).
6. `Program.cs` — сборка: аргумент командной строки, конфиг, соединение, регистрация консьюмера (с уточнениями про direct/topic и типы обменников).

---

### 1. `appsettings.json`

```json
{
  "RMQSettings": {
    "Host": "hawk.rmq.cloudamqp.com",   // Адрес облачного брокера RabbitMQ (CloudAMQP)
    "VHost": "iatvfquz",                // Виртуальный хост — изолированное пространство внутри брокера
    "Login": "iatvfquz",                // Логин (в CloudAMQP совпадает с VHost)
    "Password": "G68bk0zxzH0ncOvMlmfyYapLaCqwjiRi" // Пароль, сгенерированный CloudAMQP
  },

  "RMQSettings_": {                     // Альтернативная конфигурация для локального RabbitMQ (не используется)
    "Host": "localhost",                // Локальный хост
    "VHost": "/",                       // Стандартный виртуальный хост
    "Login": "guest",                   // Логин по умолчанию
    "Password": "guest"                 // Пароль по умолчанию
  }
}
```

> **Уточнение:** Имя секции в JSON — `"RMQSettings"` (все буквы заглавные), а свойство в C#-классе — `RmqSettings` (только первая буква). Биндинг конфигурации в .NET **нечувствителен к регистру**, поэтому маппинг сработает. Но если бы ключи отличались не только регистром, а по сути (`"RabbitSettings"` vs `RmqSettings`), свойство осталось бы `null`.

---

### 2. `Consumer.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>                 <!-- Тип проекта — консольное приложение -->
    <TargetFramework>net9.0</TargetFramework>    <!-- Целевая платформа .NET 9 -->
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.6" /> <!-- Пакет для IConfiguration -->
    <PackageReference Include="RabbitMQ.Client" Version="7.1.2" />               <!-- Официальный клиент RabbitMQ (асинхронный API) -->
  </ItemGroup>

  <ItemGroup>
    <Content Include="appsettings.json">          <!-- Включаем файл конфигурации в выходную папку -->
      <CopyToOutputDirectory>Always</CopyToOutputDirectory> <!-- Всегда копировать при сборке -->
    </Content>
  </ItemGroup>

</Project>
```

> **Уточнение:** Здесь `CopyToOutputDirectory` стоит `Always`, а в Producer-проекте был `PreserveNewest`. Разница: `PreserveNewest` копирует файл, только если он изменился с момента последней сборки; `Always` копирует при каждой сборке. На поведение приложения это не влияет — оба варианта гарантируют, что `appsettings.json` окажется рядом с `.exe`. `Always` чаще выбирают в учебных примерах, чтобы исключить ситуации «поменял конфиг — а в bin/ остался старый».

---

### 3. `MessageDto.cs`

```csharp
namespace Consumer
{
    public class MessageDto          // Модель сообщения — должна совпадать по структуре с той, что отправляет Producer
    {
        public string Content { get; set; } // Содержимое сообщения
    }
}
```

> **Уточнение:** Класс лежит в пространстве имён `Consumer`, а у Producer — в `Producer`. Имена и структура полей (`Content`) должны **совпадать**, иначе при десериализации через `JsonSerializer.Deserialize<MessageDto>` поле останется `null`. Пространство имён на JSON-десериализацию не влияет — важно только имя свойства.

---

### 4. `ApplicationSettings.cs` и `RmqSettings.cs`

**ApplicationSettings.cs**
```csharp
namespace Consumer.Settings
{
    public class ApplicationSettings
    {
        public RmqSettings RmqSettings { get; set; } // Соответствует секции "RMQSettings" в JSON (регистр не важен)
    }
}
```

**RmqSettings.cs**
```csharp
namespace Consumer.Settings
{    
    public class RmqSettings
    {
        public string Host { get; set; }      // Адрес брокера
        public string VHost { get; set; }     // Виртуальный хост
        public string Login { get; set; }     // Логин
        public string Password { get; set; }  // Пароль
    }
}
```

> **Уточнение:** Как и в Producer, имена свойств должны совпадать с ключами JSON (с учётом нечувствительности к регистру). Иначе `configuration.Get<ApplicationSettings>().RmqSettings` вернёт объект с `null`-полями, и `ConnectionFactory` не сможет подключиться.

---

### 5. `Consumer.cs`

```csharp
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Consumers
{
    public static class Consumer
    {
        // Регистрация консьюмера: привязка очереди к обменнику и запуск прослушивания
        public static async Task Register(IChannel channel, string exchangeName, string queueName, string routingKey)
        {
                // Prefetch: брокер не отдаст консьюмеру больше 10 неподтверждённых сообщений одновременно
                await channel.BasicQosAsync(0, 10, false);
                // Объявляем очередь: durable=false, exclusive=false, autoDelete=false, arguments=null
                await channel.QueueDeclareAsync(queueName, false, false, false, null);
                // Привязываем очередь к обменнику с указанным ключом маршрутизации
                await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);

                // Асинхронный консьюмер — обработчик получает сообщения через событие ReceivedAsync
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, e) =>
                {
                    //throw new Exception("Error has occured"); // Раскомментировать для демонстрации _error-очереди
                    var body = e.Body;                                    // Тело сообщения — ReadOnlyMemory<byte>
                    // Десериализуем JSON обратно в объект MessageDto
                    var message = JsonSerializer.Deserialize<MessageDto>(Encoding.UTF8.GetString(body.ToArray()));
                    Console.WriteLine($"{DateTime.Now} Received message: {message.Content}");
                    Thread.Sleep(TimeSpan.FromSeconds(2)); // Имитация долгой обработки (блокирует поток!)
                    // Подтверждаем обработку: deliveryTag — идентификатор сообщения, false — не батчем
                    await channel.BasicAckAsync(e.DeliveryTag, false);
                };

                // Запускаем прослушивание: autoAck=false — подтверждать нужно вручную
                await channel.BasicConsumeAsync(queueName, false, consumer);
                Console.WriteLine($"Subscribed to the queue with key {routingKey} (exchange name: {exchangeName})");
                // Блокируем основной поток, чтобы приложение не завершилось
                Console.ReadLine();
        }
    }
}
```

> **Уточнение 1 — `BasicQosAsync(0, 10, false)`:**
> Параметры: `prefetchSize = 0` (без ограничения по размеру), `prefetchCount = 10` (максимум 10 неподтверждённых сообщений), `global = false` (ограничение применяется к текущему каналу). Если консьюмер медленный, брокер не «завалит» его сообщениями — он будет отдавать новую пачку только после подтверждений (`BasicAckAsync`). Это защита от перегрузки.
>
> **Уточнение 2 — `QueueDeclareAsync(queueName, false, false, false, null)`:**
> Аргументы: `durable = false` (очередь не переживёт перезапуск брокера), `exclusive = false` (доступна нескольким соединениям), `autoDelete = false` (не удаляется автоматически), `arguments = null`. Для учебного примера durable-очередь не нужна; в продакшене обычно ставят `durable = true`, чтобы сообщения не терялись.
>
> **Уточнение 3 — `Thread.Sleep` внутри `async`-обработчика:**
> `Thread.Sleep` блокирует поток на 2 секунды. Так как обработчик асинхронный, это плохая практика — лучше `await Task.Delay(...)`. Но в учебном примере `Thread.Sleep` наглядно показывает «долгую обработку»: в веб-панели CloudAMQP видно, как сообщения накапливаются в очереди, а консьюмер забирает их по одному.
>
> **Уточнение 4 — `BasicAckAsync(e.DeliveryTag, false)`:**
> `deliveryTag` — уникальный идентификатор сообщения в канале. Второй параметр `multiple = false` — подтверждаем только это сообщение (а не все предыдущие). Если консьюмер упадёт до вызова `BasicAckAsync`, брокер вернёт сообщение в очередь и отдаст другому консьюмеру (или этому же после переподключения).
>
> **Уточнение 5 — `autoAck = false` в `BasicConsumeAsync`:**
> Это критически важно: при `autoAck = true` брокер считает сообщение доставленным сразу после отправки, ещё до обработки. Если консьюмер упадёт — сообщение потеряно. С `autoAck = false` подтверждение отправляется вручную через `BasicAckAsync`, и только после этого брокер удаляет сообщение из очереди.
>
> **Уточнение 6 — `Console.ReadLine()` в конце:**
> Без этой строки метод `Register` завершится, `Main` выйдет, и приложение закроется, не успев получить ни одного сообщения. `Console.ReadLine()` блокирует выполнение до нажатия Enter — консьюмер продолжает работать в фоне.

---

### 6. `Program.cs`

```csharp
using System.Threading.Tasks;
using Consumer.Settings;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Загружаем конфигурацию из appsettings.json
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Номер консьюмера передаётся аргументом командной строки: dotnet run -- 1
            var consumerNumber = int.Parse(args[0]);

            // Устанавливаем соединение и создаём канал
            var connection = await GetRabbitConnection(configuration);
            var channel = await connection.CreateChannelAsync();
            
            // Регистрируем консьюмер для direct-обменника.
            // Очередь: queue.direct_{N}, ключ маршрутизации: cars.{N}
            await Consumers.Consumer.Register(channel, $"exchange.direct", $"queue.direct_{consumerNumber}", $"cars.{consumerNumber}");
            // Альтернативы для других типов обменников (закомментированы):
            //await Consumers.Consumer.Register(channel, $"exchange.fanout", $"queue.fanout_{consumerNumber}", $"cars.{consumerNumber}");
            //await Consumers.Consumer.Register(channel, $"exchange.topic", $"queue.topic_{consumerNumber}",  consumerNumber > 2 ? "*.1": $"cars.{consumerNumber}");
        }
        
        // Вспомогательный метод: читает настройки и создаёт соединение
        private static async Task<IConnection> GetRabbitConnection(IConfiguration configuration)
        {
            // Маппим секцию "RMQSettings" в объект RmqSettings
            var rmqSettings = configuration.Get<ApplicationSettings>().RmqSettings;
            // Фабрика соединений — точка входа в RabbitMQ.Client
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = rmqSettings.Host,         // Адрес брокера
                VirtualHost = rmqSettings.VHost,     // Виртуальный хост
                UserName = rmqSettings.Login,        // Логин
                Password = rmqSettings.Password,     // Пароль
            };
            IConnection conn = await factory.CreateConnectionAsync();
            return conn;
        }
    }
}
```

> **Уточнение 1 — `args[0]` (номер консьюмера):**
> Приложение запускается с аргументом, например: `dotnet run -- 1` или `Consumer.exe 2`. Номер используется и в имени очереди (`queue.direct_1`), и в ключе маршрутизации (`cars.1`). Благодаря этому можно запустить несколько экземпляров, и каждый будет слушать свою очередь. `int.Parse` упадёт с исключением, если аргумент не передан или не является числом — в учебном примере обработка ошибок опущена.
>
> **Уточнение 2 — как работает связка «обменник + очередь + ключ»:**
> В `Program.cs` для direct-обменника: обменник `exchange.direct`, очередь `queue.direct_1`, ключ `cars.1`. Producer отправляет сообщение в `exchange.direct` с ключом `cars.1` — оно попадёт именно в `queue.direct_1`, потому что ключи совпадают. Если Producer отправит с ключом `cars.2`, сообщение уйдёт в `queue.direct_2` (если такой консьюмер запущен), а `queue.direct_1` его не получит.
>
> **Уточнение 3 — закомментированная строка для topic:**
> `consumerNumber > 2 ? "*.1" : $"cars.{consumerNumber}"` — если номер консьюмера больше 2, ключ привязки будет `*.1` (шаблон «любое слово + `.1`»). Это значит, что консьюмер №3 получит сообщения с ключами `cars.1`, `trucks.1`, `bikes.1` и т.д. Консьюмеры №1 и №2 привязываются точными ключами `cars.1` и `cars.2`. Так на практике видно разницу между direct и topic: topic-шаблон ловит несколько ключей.
>
> **Уточнение 4 — почему Producer и Consumer должны использовать одинаковый тип обменника:**
> В `Producer.cs` конструктор вызывает `ExchangeDeclareAsync` с типом `direct`. Если Consumer попытается объявить тот же обменник с типом `fanout`, RabbitMQ вернёт ошибку `PRECONDITION_FAILED` и закроет канал. Поэтому при переключении на fanout/topic нужно раскомментировать соответствующую строку **и в Producer, и в Consumer** — и удалить старый обменник в веб-панели CloudAMQP (или использовать другое имя обменника, что и сделано: `exchange.direct`, `exchange.fanout`, `exchange.topic` — три разных обменника).

---
