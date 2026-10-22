### Последовательность показа

1. `appsettings.json` — откуда берём настройки (облако CloudAMQP).
2. `MessageDto.cs` — что отправляем (структура JSON).
3. `ApplicationSettings.cs` + `RmqSettings.cs` — как JSON маппится в C#-объекты.
4. `Producer.csproj` — какие пакеты нужны и как подключается конфиг.
5. `Producer.cs` — логика: объявление exchange, сериализация, публикация (с уточнением про routing key).
6. `Program.cs` — сборка всего: конфиг → соединение → канал → producer → отправка (с уточнениями про IConnection/IChannel и ExchangeDeclare).

### 1. `appsettings.json`

```json
{
  "RmqSettings": {
    "Host": "hawk.rmq.cloudamqp.com",   // Адрес облачного брокера RabbitMQ (CloudAMQP)
    "VHost": "iatvfquz",                // Виртуальный хост (изолированное пространство внутри брокера)
    "Login": "iatvfquz",                // Логин для подключения (в CloudAMQP совпадает с VHost)
    "Password": "G68bk0zxzH0ncOvMlmfyYapLaCqwjiRi" // Пароль, сгенерированный CloudAMQP
  },

  "RmqSettings_": {                     // Альтернативная конфигурация для локального RabbitMQ (не используется)
    "Host": "localhost",                // Локальный хост
    "VHost": "/",                       // Стандартный виртуальный хост
    "Login": "guest",                   // Логин по умолчанию
    "Password": "guest"                 // Пароль по умолчанию
  }
}
```

> **Уточнение:** В CloudAMQP `Host` — это полный домен (`hawk.rmq.cloudamqp.com`), а `VHost` и `Login` совпадают. Это нормально для managed-сервиса. Для локального RabbitMQ используется секция `RmqSettings_`, но в коде она не читается — это запасной вариант для переключения вручную.

---

### 2. `MessageDto.cs`

```csharp
namespace Producer
{
    public class MessageDto          // Основная модель сообщения
    {
        public string Content { get; set; } // Содержимое сообщения (строка)
    }
    
    public class MessageDto2         // Вторая модель (не используется в примере, оставлена для демонстрации)
    {
        public string Content { get; set; } // Аналогичное поле
    }
}
```

> **Уточнение:** Наличие `MessageDto2` показывает, что сообщения разных типов можно сериализовать по-разному. В текущем `Producer.cs` используется только `MessageDto`.

---

### 3. `ApplicationSettings.cs` и `RmqSettings.cs`

**ApplicationSettings.cs**
```csharp
namespace Producer.Settings
{
    public class ApplicationSettings
    {
        public RmqSettings RmqSettings { get; set; } // Свойство соответствует секции "RmqSettings" в JSON
    }
}
```

**RmqSettings.cs**
```csharp
namespace Producer.Settings
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

> **Уточнение:** Имена свойств (`Host`, `VHost`, `Login`, `Password`) должны **точно** совпадать с ключами в JSON-секции `RmqSettings`, иначе при `configuration.Get<ApplicationSettings>()` они останутся `null`. Регистр имеет значение.

---

### 4. `Producer.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>                 <!-- Тип проекта — консольное приложение -->
    <TargetFramework>net9.0</TargetFramework>    <!-- Целевая платформа .NET 9 -->
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.6" /> <!-- Пакет для IConfiguration и хостинга -->
    <PackageReference Include="RabbitMQ.Client" Version="7.1.2" />               <!-- Официальный клиент RabbitMQ (асинхронный API) -->
  </ItemGroup>

  <ItemGroup>
    <Content Include="appsettings.json">          <!-- Включаем файл конфигурации в выходную папку -->
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory> <!-- Копировать при сборке, если файл новее -->
    </Content>
  </ItemGroup>

</Project>
```

> **Уточнение:** Версия `RabbitMQ.Client 7.x` использует **асинхронные** методы (`CreateConnectionAsync`, `CreateChannelAsync`, `BasicPublishAsync`). В версиях 6.x API был синхронным — код не совместим без изменений. `CopyToOutputDirectory` обязателен, иначе `appsettings.json` не попадёт в папку сборки и конфигурация не загрузится.

---

### 5. `Producer.cs`

```csharp
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Producer.Producers
{
    public class Producer
    {
        private readonly string _exchangeName;   // Имя обменника (exchange)
        private readonly string _routingKey;     // Ключ маршрутизации, прикрепляемый к сообщению
        private readonly IChannel _channel;      // Канал — лёгкое виртуальное соединение внутри IConnection

        // Конструктор: принимает тип обменника, имя, ключ маршрутизации и канал
        public Producer(string exchangeType, string exchangeName,
            string routingKey, IChannel channel)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _channel = channel;
            // Объявляем обменник. Если он уже существует с тем же типом — ошибки не будет.
            // Если существует с другим типом — RabbitMQ вернёт ошибку PRECONDITION_FAILED.
            _channel.ExchangeDeclareAsync(_exchangeName, exchangeType);
        }
        
        // Метод отправки одного сообщения
        public async Task Produce(string messageContent)
        {
            // Формируем объект сообщения
            var message = new MessageDto()
            {
                Content = $"{messageContent}"
            };
            // Сериализуем в JSON (например: {"Content":"42"})
            var messageSerialized = JsonSerializer.Serialize(message);
            // Преобразуем строку в байты UTF-8 — RabbitMQ передаёт тело сообщения как byte[]
            var body = Encoding.UTF8.GetBytes(messageSerialized);
            
            // Публикуем сообщение в обменник с указанным ключом маршрутизации
            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: _routingKey,
                body: body);

            // Логируем факт отправки (не подтверждение доставки!)
            Console.WriteLine($"Message {messageSerialized} is sent into exchange: {_exchangeName}");
        }
    }
}
```

> **Уточнение 1 — что такое ключ маршрутизации (Routing Key):**
> Ключ маршрутизации — это строка, которую продюсер прикрепляет к сообщению. Обменник анализирует её и решает, в какую очередь направить сообщение. Для **direct exchange** очередь получит сообщение только при **точном совпадении** её binding-ключа с `routingKey`. В вашем примере ключ — `"cars.1"`. Если очередь привязана с ключом `"cars.2"`, сообщение до неё не дойдёт. Для **fanout** ключ игнорируется, для **topic** — сравнивается с шаблоном (`cars.*`, `cars.#`).
>
> **Уточнение 2 — `Console.WriteLine` не гарантирует доставку:**
> `BasicPublishAsync` завершается, как только сообщение передано в сокет. Это не значит, что RabbitMQ сохранил его на диск или доставил в очередь. Для гарантированной доставки нужны **publisher confirms** (подтверждения от брокера) и/или **mandatory flag** с обработкой возвратов. В учебном примере это опущено намеренно.

---

### 6. `Program.cs`

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Producer.Settings;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Загружаем конфигурацию из appsettings.json.
            // optional: true — не падать, если файла нет; reloadOnChange — перечитывать при изменении.
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Устанавливаем соединение с RabbitMQ (тяжёлое TCP-соединение, создаётся один раз)
            var connection = await GetRabbitConnection(configuration);
            // Создаём канал — лёгкий виртуальный канал внутри соединения (можно создавать много)
            var channel = await connection.CreateChannelAsync();
            
            // Создаём продюсера для direct-обменника "exchange.direct" с ключом "cars.1"
            var producer = new Producers.Producer("direct", "exchange.direct", "cars.1", channel);
            // Альтернативы для демонстрации других типов обменников:
            //var producer = new Producers.Producer("fanout", "exchange.fanout", "cars.1", channel);
            //var producer = new Producers.Producer("topic", "exchange.topic", "cars.1", channel);
            
            // Отправляем одно сообщение со случайным числом от 1 до 100
            await producer.Produce($"{new Random().Next(1,100)}");
            
            /* Закомментированный бесконечный цикл для постоянной отправки:
            while (true)
            {
                await producer.Produce($"{new Random().Next(1,100)}");
                Thread.Sleep(TimeSpan.FromSeconds(1)); // Имитация долгой обработки
            }
            */
            
            // Корректно закрываем канал и соединение
            await channel.CloseAsync();
            await connection.CloseAsync();
        }
        
        // Вспомогательный метод: читает настройки и создаёт соединение
        private static async Task<IConnection> GetRabbitConnection(IConfiguration configuration)
        {
            // Маппим секцию "RmqSettings" из JSON в объект RmqSettings
            var rmqSettings = configuration.Get<ApplicationSettings>().RmqSettings;
            // Фабрика соединений — точка входа в RabbitMQ.Client
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = rmqSettings.Host,         // Адрес брокера
                VirtualHost = rmqSettings.VHost,     // Виртуальный хост
                UserName = rmqSettings.Login,        // Логин
                Password = rmqSettings.Password      // Пароль
            };

            // Асинхронно устанавливаем соединение
            return await factory.CreateConnectionAsync();
        }
    }
}
```

> **Уточнение 1 — почему `IConnection` и `IChannel` разные:**
> `IConnection` — это физическое TCP-соединение с брокером. Оно дорогое по ресурсам, его создают один раз на приложение. `IChannel` — виртуальный канал внутри соединения; каналов можно создавать много, и именно через них выполняется публикация/подписка. Закрывать нужно оба: сначала каналы, потом соединение.
>
> **Уточнение 2 — `Producer` объявляет exchange в конструкторе:**
> Строка `_channel.ExchangeDeclareAsync(_exchangeName, exchangeType)` выполняется при создании объекта. Если обменник уже существует с тем же типом — RabbitMQ просто подтвердит. Если с другим типом — вернёт ошибку `PRECONDITION_FAILED` и канал закроется. Поэтому важно, чтобы тип (`direct`/`fanout`/`topic`) совпадал с тем, что уже создано в облаке.
>
> **Уточнение 3 — закомментированный `while (true)`:**
> Бесконечный цикл полезен для демонстрации в веб-панели CloudAMQP: сообщения будут появляться в очереди каждую секунду, и их можно наблюдать в реальном времени. В текущем виде отправляется одно сообщение — удобно для показа «вот оно появилось».

---

