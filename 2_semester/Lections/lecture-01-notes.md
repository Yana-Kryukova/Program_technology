# ЛЕКЦИЯ. Часть 1. ASP.NET Core, архитектура и основы EF Core

**Продолжительность:** 90 минут (первое занятие из двух).  
**Тема:** Инфраструктурный слой микросервиса NotesService на EF Core и ASP.NET Core.

«Сегодня мы с вами соберём инфраструктурный слой микросервиса NotesService. Это сервис заметок: пользователь регистрируется, создаёт заметки, редактирует, удаляет. Звучит просто, но за этой простотой стоит полноценная архитектура: Domain, Application, Infrastructure и Presentation. Сегодня мы разберём Infrastructure и Presentation, а Application оставим на следующую лекцию — но я покажу, где он находится и зачем он вообще нужен.

Мы не просто напишем код — мы поймём, **почему** каждая строчка написана именно так. Почему `SaveChanges` вызывается в репозитории, а не в контроллере. Почему `DbContext` — это Unit of Work, а `DbSet` — репозиторий. Почему value objects нужно конвертировать.

Если по ходу лекции встретится что-то незнакомое — остановите меня, я объясню. Мы будем разбирать все термины с нуля. Асинхронность мы уже проходили на прошлой лекции, поэтому `async/await` я буду использовать без подробных объяснений — если что-то забылось, поднимите руку.

В конце каждой крупной темы я буду задавать вопросы. Это не экзамен, а способ убедиться, что мы движемся вместе.

К концу лекции вы сможете самостоятельно построить такой же слой для любого домена.»

---

## 1. Что такое ASP.NET Core

«ASP.NET Core — это фреймворк для построения веб-приложений и HTTP API на .NET. Он кроссплатформенный, модульный и имеет встроенный DI-контейнер.

Прежде чем идти дальше, давайте разберёмся с базовыми терминами, потому что без них ничего не будет понятно.

**Веб-приложение** — это программа, которая принимает запросы по сети и возвращает ответы. Обычно работает на сервере и доступна клиентам через интернет или локальную сеть.

**HTTP** — протокол, по которому клиент и сервер общаются. Клиент отправляет запрос, сервер возвращает ответ. Это как почта: вы отправляете письмо, получаете ответ.

**HTTP-запрос** — сообщение от клиента к серверу. Содержит метод, URL, заголовки, возможно — тело. **HTTP-ответ** — сообщение от сервера. Содержит код (200, 404, 500), заголовки, тело.

**HTTP-метод** — глагол, указывающий, что клиент хочет сделать: GET (прочитать), POST (создать), PUT (обновить), DELETE (удалить).

**JSON** — текстовый формат обмена данными. Выглядит как объект JavaScript: `{ "id": 1, "title": "Note" }`. В REST API чаще всего используется для тел запросов и ответов.

**REST API** — стиль построения HTTP API, где каждый URL соответствует ресурсу. `GET /api/notes` — получить все заметки. `POST /api/notes` — создать новую. `PUT /api/notes/123` — обновить. `DELETE /api/notes/123` — удалить.

**Микросервис** — небольшое приложение, решающее одну задачу. NotesService — микросервис, который занимается только заметками. Он может общаться с другими микросервисами — например, с сервисом аутентификации — по сети.

**Kestrel** — встроенный кроссплатформенный веб-сервер ASP.NET Core. Он принимает TCP-соединения, разбирает HTTP-запросы и передаёт их в приложение. Может работать самостоятельно или за обратным прокси — Nginx, IIS.

**HttpContext** — объект, содержащий всю информацию о текущем HTTP-запросе и ответе: метод, URL, заголовки, тело, cookies, пользователя. Создаётся Kestrel для каждого запроса.

**Endpoint** — обработчик конкретного URL. Например, `GET /api/notes` — это endpoint, который возвращает список заметок.

Теперь давайте посмотрим на минимальное приложение:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
```

Что здесь происходит?

- `WebApplication.CreateBuilder(args)` — создаёт **билдер**. Это объект, который собирает приложение по частям. Внутри у него уже преднастроены конфигурация, логирование и DI.
- `builder.Services` — это **коллекция сервисов**, которые мы регистрируем в DI-контейнере.
- `builder.Configuration` — доступ к настройкам.
- `builder.Build()` — создаёт приложение.
- `app.MapGet("/", ...)` — регистрирует endpoint.
- `app.Run()` — запускает Kestrel и блокирует поток.

### Middleware

**Middleware** — это компоненты, которые обрабатывают HTTP-запрос по очереди.

Как это работает: запрос входит в приложение и проходит через конвейер middleware. Каждый компонент может обработать запрос, изменить его, передать дальше или прервать.

**Аналогия:** представьте конвейер на заводе. Деталь — запрос — проходит через несколько станций. Каждая станция что-то делает: красит, шлифует, проверяет. В конце — упаковка, endpoint. Ответ идёт обратно по тому же конвейеру.

**Примеры middleware:**
- **Логирование** — записывает в лог метод, URL, время обработки.
- **Аутентификация** — проверяет, кто отправил запрос — по токену, cookie.
- **Авторизация** — проверяет, есть ли у пользователя права на этот URL.
- **Маршрутизация** — определяет, какой endpoint должен обработать запрос.
- **Обработка ошибок** — перехватывает исключения и формирует ответ с кодом 500.
- **CORS** — управляет кросс-доменными запросами.
- **Сжатие** — сжимает ответ перед отправкой.

Порядок middleware критичен. `UseRouting` должен идти до `MapControllers`, `UseAuthentication` — до `UseAuthorization`. Если перепутать — авторизация не сработает.

**Почему порядок важен?** Представьте, что вы сначала проверяете права — авторизация, — а потом узнаёте, кто пришёл — аутентификация. Логически это невозможно: нельзя проверить права, не зная пользователя. Поэтому `UseAuthentication` всегда идёт до `UseAuthorization`.

### Dependency Injection

**Dependency Injection (DI)** — паттерн, при котором объект не создаёт свои зависимости сам, а получает их извне.

**Зависимость** — это объект, который нужен другому объекту для работы. Контроллеру нужен репозиторий — репозиторий и есть зависимость контроллера.

Без DI контроллер сам создавал бы репозиторий, а тот — контекст. Это жёсткая связанность, трудно тестировать. С DI контроллер просто говорит: «мне нужен `IUserRepository`», и контейнер сам его подставляет.

**DI-контейнер** — это объект, который хранит регистрации «интерфейс → реализация» и умеет создавать объекты, автоматически подставляя их зависимости. В ASP.NET Core он встроен.

**Простейший пример регистрации:**

```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

Мы говорим контейнеру: «Когда кто-то попросит `IMyService`, создай `MyService`».

**Три времени жизни сервисов:**

- **Transient** — новый экземпляр при каждом запросе. Подходит для лёгких stateless-сервисов.
- **Scoped** — один экземпляр на HTTP-запрос.
- **Singleton** — один экземпляр на всё приложение.

`DbContext` регистрируется как **Scoped**. Почему?

- Если сделать его **Singleton** — два параллельных запроса будут писать в один контекст. Трекер изменений перемешается, `SaveChanges` одного запроса сохранит изменения другого. Это баг.
- Если сделать **Transient** — каждый репозиторий получит свой контекст. Изменения, сделанные одним репозиторием, не будут видны другому. Unit of Work перестанет работать.
- **Scoped** — идеальный компромисс.

**Внедрение через конструктор:**

```csharp
public class MyController(IMyService service) : ControllerBase
{
    // service доступен внутри класса
}
```

Здесь используется **primary constructor** — синтаксис из C# 12. Раньше пришлось бы писать отдельное поле и конструктор.

**Чек-поинт.** Прежде чем идти дальше, ответьте: что такое DI-контейнер и зачем он нужен? Почему `DbContext` регистрируется как Scoped?

### Конфигурация

**Конфигурация** — это настройки приложения: строка подключения к БД, уровни логирования, ключи API.

Источники конфигурации в порядке приоритета, от слабого к сильному:
1. `appsettings.json` — базовый файл.
2. `appsettings.{Environment}.json` — настройки для конкретного окружения.
3. **Секреты пользователя** — только Development.
4. **Переменные среды** — для Production и CI/CD.
5. **Аргументы командной строки**.

Каждый следующий источник переопределяет предыдущий.

**Окружение** — режим работы приложения: `Development`, `Staging`, `Production`. Определяется переменной среды `ASPNETCORE_ENVIRONMENT`.

**Чтение строки подключения:**

```csharp
var connectionString = builder.Configuration.GetConnectionString("Default");
```

`GetConnectionString` ищет значение в секции `ConnectionStrings:Default`.

### Swagger

**Swagger** — это инструмент для документирования HTTP API. Он генерирует интерактивную страницу, где можно:
- Посмотреть список всех endpoint'ов.
- Увидеть ожидаемые параметры и формат ответа.
- Отправить тестовый запрос прямо из браузера.

**OpenAPI** — это спецификация, то есть формат описания API. Swagger — инструмент, который умеет читать и отображать эту спецификацию.

**Зачем Swagger:**
- **Документация.** Разработчики видят, какие endpoint'ы есть и как их вызывать.
- **Тестирование.** Не нужно писать curl-команды вручную.
- **Контракт.** Клиенты знают, что ожидать.

**Как подключить:**

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Notes service API",
        Description = "API for creating, viewing, storing, modifying, and deleting notes."
    });
});

builder.Services.AddEndpointsApiExplorer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**Что здесь происходит:**
- `AddSwaggerGen` — регистрирует генератор OpenAPI-документа.
- `AddEndpointsApiExplorer` — позволяет Swagger находить все endpoint'ы.
- `UseSwagger` — middleware, отдающий JSON-спецификацию.
- `UseSwaggerUI` — middleware, отдающий интерактивную HTML-страницу.

После запуска Swagger доступен по адресу `https://localhost:{port}/swagger`.

### Контроллеры

**Контроллер** — это класс, который обрабатывает HTTP-запросы. Каждый публичный метод контроллера — **action** — привязан к определённому URL и HTTP-методу.

**Простейший контроллер:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new[] { "note1", "note2" });
}
```

**Разбор:**
- `[ApiController]` — атрибут, включающий автоматическую валидацию модели и привязку параметров.
- `[Route("api/[controller]")]` — маршрут. `[controller]` заменяется на имя контроллера без суффикса `Controller`. То есть `/api/notes`.
- `ControllerBase` — базовый класс для API-контроллеров. Предоставляет методы `Ok`, `NotFound`, `BadRequest` и другие.
- `[HttpGet]` — атрибут, указывающий, что метод обрабатывает GET-запросы.
- `Ok(...)` — возвращает HTTP 200 с переданным объектом.

**Атрибут** — специальная конструкция в C#, которая добавляет метаданные к классу, методу или свойству. Начинается с `[`. ASP.NET Core и EF Core активно используют атрибуты для настройки.

**Маршрутизация** — процесс сопоставления URL с конкретным action. `GET /api/notes` → `NotesController.GetAll`. `GET /api/notes/123` → `NotesController.GetById(123)`.

**Модель** — в контексте ASP.NET Core это объект, который приходит в action или возвращается из него. Например, `[FromBody] User user` — модель, полученная из тела запроса.

### Program.cs NotesService

```csharp
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string for NotesServiceDbContext is not configured.");

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("NotesService.Infrastructure.EntityFramework");
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Notes service API", ... });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MigrateDatabase<ApplicationDbContext>();
app.Run();
```

**Пошаговый разбор:**

1. Создаём билдер.
2. Читаем строку подключения из конфигурации. Если её нет — падаем с понятной ошибкой.
3. `AddNpgsql` регистрирует `DbContext` с провайдером PostgreSQL и указывает сборку миграций.
4. Настраиваем Swagger: версия, заголовок, описание.
5. `AddDbContext` — дополнительная регистрация.
6. `AddControllers` — регистрирует контроллеры.
7. `AddEndpointsApiExplorer` — нужен для Swagger.
8. Строим приложение.
9. В Development включаем Swagger UI.
10. `UseAuthorization` — middleware авторизации.
11. `MapControllers` — маршрутизация.
12. `MigrateDatabase` — применяем миграции при старте.
13. `Run` — запускаем.»

---

## 2. Место инфраструктурного слоя в архитектуре

«NotesService построен по принципам **чистой архитектуры**. Это подход, при котором приложение делится на слои, и зависимости направлены строго внутрь.

**Слой** — это набор классов, объединённых общей ответственностью.

**Слой Presentation** — взаимодействует с внешним миром: HTTP-запросы, CLI, gRPC. В нашем случае — ASP.NET Core Web API с контроллерами.

**Слой Infrastructure** — реализует доступ к внешним ресурсам: БД, файлы, сторонние сервисы. В нашем случае — EF Core и репозитории.

**Слой Application** — содержит use cases, сценарии использования. Это бизнес-логика приложения. Будет рассмотрен в следующей лекции.

**Слой Domain** — ядро приложения. Сущности, value objects, интерфейсы репозиториев, доменные правила.

У нас четыре слоя:

```
        ┌─────────────────────┐
        │    Presentation     │
        └──────────┬──────────┘
                   │ зависит от
        ┌──────────▼──────────┐
        │    Infrastructure   │
        └──────────┬──────────┘
                   │ зависит от
        ┌──────────▼──────────┐
        │     Application     │
        └──────────┬──────────┘
                   │ зависит от
        ┌──────────▼──────────┐
        │        Domain       │
        └─────────────────────┘
```

**Стрелки направлены внутрь.** Domain не знает ни о ком. Application знает только о Domain. Infrastructure знает об Application и Domain. Presentation знает обо всех, но используется только как точка входа.

**Зависимость** — это когда один класс использует другой. В чистой архитектуре правило: внутренние слои не должны знать о внешних.

**Почему это важно?**

Первое — **домен можно тестировать без БД**, без HTTP, без всего. У нас чистая логика, которую мы проверяем за миллисекунды.

Второе — **можно заменить EF Core на Dapper** или на что угодно, не меняя Domain.

Третье — **команда понимает, куда что писать**. Если это бизнес-правило — в Domain. Если это use case — в Application. Если это работа с БД — в Infrastructure.

**В NotesService:**
- **Domain:** `Entity<TId>`, `IRepository<TEntity, TId>`, `IUserRepository`, `User`, `Note`, `Username`, `Title`, `Thesis`.
- **Application:** будет в следующей лекции — сервисы, use cases.
- **Infrastructure:** `ApplicationDbContext`, `UserConfiguration`, `NoteConfiguration`, `EfRepository`, `EfUserRepository`.
- **Presentation:** `Program.cs`, `appsettings.json`, контроллеры.

**Application не будет сегодня.** Это тема следующей лекции.»

---

## 3. Доменные абстракции

«Прежде чем писать инфраструктуру, нам нужно понять, что мы реализуем.

**Сущность (Entity)** — это объект с уникальным идентификатором и жизненным циклом. Два пользователя с одинаковым именем — разные сущности, если у них разные id.

**`Entity<TId>`** — базовый класс сущности с идентификатором. Все сущности наследуются от него. `TId` — тип идентификатора, обычно `Guid`, `int`, `long`.

**`IRepository<TEntity, TId>`** — обобщённый интерфейс:

```csharp
public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct, bool asNoTracking = false);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct);
    Task<TEntity?> AddAsync(TEntity entity, CancellationToken ct);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken ct);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken ct);
    Task<bool> DeleteAsync(TId id, CancellationToken ct);
}
```

**Ограничения generic** — правила, какие типы можно подставить. `where TEntity : Entity<TId>` — `TEntity` должен быть наследником `Entity<TId>`. `where TId : struct, IEquatable<TId>` — `TId` — значимый тип, реализующий `IEquatable<TId>`.

Без `where TEntity : Entity<TId>` мы не смогли бы обращаться к `entity.Id` внутри репозитория — компилятор не знал бы, что у `TEntity` есть `Id`.

**`IUserRepository`** — специализированный интерфейс для `User`:

```csharp
public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct);
}
```

Он наследуется от `IRepository<User, Guid>` и добавляет метод поиска по username.

**Value objects: `Username`, `Title`, `Thesis`.**

**Value object** — это объект, определяемый своими значениями, а не идентификатором. Два `Username("alice")` равны. `Username("alice")` и `Username("bob")` — разные. Value objects иммутабельны: их нельзя изменить после создания.

**Зачем value objects:**
- **Валидация в конструкторе.** `new Username("")` бросает исключение. Невалидный username невозможно создать.
- **Типобезопасность.** Нельзя случайно передать `Title` туда, где ожидается `Username`.
- **Инкапсуляция правил.** Правила о максимальной длине — внутри value object.

В нашем случае `Username` валидирует длину через `UsernameValidator.MAX_LENGTH`, `Title` — через `TitleValidator.MAX_LENGTH`. `Thesis` — просто обёртка над непустой строкой.»

---

## 4. Основы EF Core

### Переход от ASP.NET Core к EF Core

«Мы разобрали, как ASP.NET Core принимает HTTP-запрос и передаёт его контроллеру. Но что делает контроллер? Он вызывает репозиторий, который обращается к базе данных. Именно для работы с БД нам нужен EF Core.

### Что такое EF Core и зачем он нужен

**Entity Framework Core** — это ORM, Object-Relational Mapping. Он позволяет работать с БД через объекты C#, а не через SQL-запросы.

**ORM** — это прослойка между объектами в коде и таблицами в БД. Вы работаете с классами `User`, `Note`, а ORM сам генерирует SQL, конвертирует строки в объекты, обрабатывает NULL, следит за SQL-инъекциями.

**Зачем это нужно?** Представьте, что вы пишете SQL вручную для каждой сущности. Нужно вставить пользователя — пишете INSERT. Нужно обновить — UPDATE. Нужно загрузить с заметками — JOIN. При этом вы вручную конвертируете строки в объекты. Это долго и чревато ошибками. EF Core берёт это на себя.

**Основные понятия:**
- **DbContext** — сессия работы с БД. Это Unit of Work.
- **DbSet<T>** — коллекция сущностей. Это Repository.
- **Entity** — класс, маппящийся на таблицу.
- **Change Tracker** — отслеживает изменения.
- **Migration** — код, описывающий изменение схемы.

**EF Core — не единственный ORM для .NET.** Есть Dapper — микро-ORM: он быстрее, но требует больше ручного кода. Есть ADO.NET — чистый SQL без абстракций. Есть NHibernate — старший брат EF с другой философией.

EF Core выбран для NotesService, потому что он даёт баланс между удобством и производительностью, а также поддерживает миграции.

### DbContext — Unit of Work

**DbContext** — это Unit of Work. Он управляет сессией работы с БД и отслеживает все изменения.

**Unit of Work** — это паттерн, при котором все изменения в рамках одной операции накапливаются и сохраняются вместе, одной транзакцией.

**Зачем Unit of Work?** Представьте, что вы переводите деньги с одного счёта на другой. Это две операции: списать с одного, зачислить на другой. Если после первой операции что-то упадёт — деньги пропадут. Unit of Work решает эту проблему: он накапливает изменения и сохраняет их все вместе в одной транзакции.

**Транзакция** — это набор операций, которые выполняются как единое целое. Либо все успешно, либо ни одна. Транзакции обеспечивают ACID: атомарность, согласованность, изоляцию, долговечность.

В EF Core это работает так: вы загружаете сущности, меняете их свойства, добавляете новые. Все изменения накапливаются в трекере. Когда вы вызываете `SaveChanges`, EF Core генерирует SQL-запросы для всех изменений и выполняет их в одной транзакции.

**Change Tracker** — это внутренний механизм EF Core, который запоминает, какие сущности загружены и какие свойства у них менялись. На основе этого он решает, какие SQL-запросы генерировать при `SaveChanges`.

### DbSet — Repository

**DbSet<T>** — это репозиторий для сущностей типа T.

```csharp
public DbSet<Product> Products { get; set; }
```

**DbSet на уровне SQL.** `DbSet<User> Users` соответствует таблице `Users` в БД. Каждое свойство `User` — колонка. Каждый экземпляр `User` — строка.

Когда вы пишете `context.Users.Add(user)`, EF Core готовит INSERT. Когда `context.Users.Where(u => u.Id == id)`, EF Core готовит SELECT с WHERE. Но реальный SQL генерируется только при `SaveChanges` или при материализации запроса.

**Что умеет DbSet:**
- **LINQ-запросы:** `context.Products.Where(p => p.Price > 100).ToList()`.
- **Add / Update / Remove.**
- **FindAsync:** ищет в трекере, потом в БД.
- **Include:** eager loading связанных данных.

**LINQ** (Language Integrated Query) — это способ писать запросы прямо в C#. Вместо SQL-строки `SELECT * FROM Products WHERE Price > 100` вы пишете `context.Products.Where(p => p.Price > 100)`. EF Core транслирует это в SQL.

**Eager loading** — это стратегия загрузки связанных данных сразу. `Include` выполняет JOIN и загружает всё одним запросом.

**Lazy loading** — стратегия, при которой связанные данные загружаются при первом обращении. Отключён по умолчанию и может привести к N+1 проблеме.

**N+1 проблема** — если вы загружаете 100 пользователей и для каждого обращаетесь к его заметкам, будет 1 запрос на пользователей + 100 запросов на заметки. Итого 101 запрос. Это медленно. Eager loading через `Include` решает проблему.

**Почему DbSet — это репозиторий?** Потому что он предоставляет интерфейс, похожий на коллекцию: можно добавлять, удалять, искать. Но под капотом — запросы к БД.

### Трекинг изменений

«EF Core по умолчанию отслеживает все загруженные сущности. Это значит: вы загрузили пользователя, поменяли ему `Username`, вызвали `SaveChanges` — EF сам сгенерирует `UPDATE`.

**Что такое трекинг?** Это механизм, при котором EF Core запоминает состояние каждой загруженной сущности и сравнивает его с текущим при `SaveChanges`.

**У трекинга есть цена:**
- Загруженные сущности остаются в памяти до конца контекста.
- Если загрузить одну и ту же сущность дважды — вернётся один и тот же объект.
- Для read-only запросов трекинг — лишняя нагрузка.

**Решение — `AsNoTracking()`:**

```csharp
var products = await context.Products.AsNoTracking().ToListAsync();
```

Запрос быстрее, но изменения не отслеживаются. Используйте для отчётов, списков, справочников.

**Разница между `FindAsync` и `FirstOrDefaultAsync`:**
- `FindAsync(id)` сначала ищет сущность в трекере. Если её нет — делает запрос к БД.
- `FirstOrDefaultAsync` всегда идёт в БД.

**Кэш запросов** — EF Core кэширует план выполнения запросов, чтобы не компилировать их каждый раз. Это не то же самое, что кэш результатов — сами данные не кэшируются.

В `EfRepository.GetByIdAsync` мы используем `FindAsync`. Если один и тот же id запросить дважды за один запрос — второй раз вернётся из трекера.

### ApplicationDbContext из NotesService

```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
```

**Разбор:**

1. **Primary constructor** принимает `DbContextOptions<ApplicationDbContext>`. Это нужно, чтобы `DbContext` регистрировался через DI.

**DbContextOptions<T>** — это объект с настройками контекста. В нём хранится строка подключения, провайдер — PostgreSQL, SQL Server, — параметры логирования. Когда вы регистрируете `DbContext` через `AddDbContext`, вы передаёте лямбду, которая настраивает эти опции. Контейнер DI создаёт `DbContextOptions` и передаёт их в конструктор контекста.

**Зачем это нужно?** Потому что `DbContext` не должен сам знать, к какой БД подключаться. Он получает это извне. Это позволяет в тестах подставить InMemory-провайдер, а в production — PostgreSQL.

2. **`DbSet<User> Users` и `DbSet<Note> Notes`** — таблицы.

3. **`OnConfiguring`** — вызывается при создании контекста. Здесь включаем `EnableSensitiveDataLogging()`. Это позволяет видеть значения параметров в логах. **Важно:** в production это небезопасно — в логи могут попасть пароли и персональные данные.

4. **`OnModelCreating`** — вызывается при построении модели, один раз за время жизни приложения. `ApplyConfigurationsFromAssembly` автоматически находит все классы `IEntityTypeConfiguration<T>` в текущей сборке.

**Модель** — это описание того, как классы C# маппятся на таблицы БД. EF Core строит модель один раз, чтобы знать, какие SQL-запросы генерировать.

**Почему `ApplyConfigurationsFromAssembly` — это хорошо?** Потому что иначе пришлось бы вручную перечислять все конфигурации. А так — добавили новый класс, он автоматически подхватился.

**Почему `DbContext` не должен быть Singleton?** Потому что он не потокобезопасен. Если два запроса одновременно будут писать в один контекст — трекер изменений перемешается.

**Почему `DbContext` не должен быть Transient?** Потому что тогда каждый репозиторий получит свой контекст. Unit of Work перестанет работать.

**Чек-поинт.** Ответьте: в чём разница между `FindAsync` и `FirstOrDefaultAsync`? Что будет, если `DbContext` сделать Singleton?»

---

## 5. Конфигурация сущностей через IEntityTypeConfiguration

### Data Annotations vs Fluent API

«Есть два подхода к настройке модели.

**Data Annotations** — атрибуты прямо на свойствах:

```csharp
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
}
```

Плюсы: просто, всё в одном месте. Минусы: засоряет доменные классы инфраструктурными деталями.

**Fluent API** — отдельные классы:

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
    }
}
```

Плюсы: чисто, гибко, Domain не знает о EF. Минусы: конфигурация в отдельном файле.

В чистой архитектуре предпочтителен Fluent API.

**Fluent API** — это стиль программирования, при котором методы вызываются цепочкой: `builder.Property(p => p.Name).IsRequired().HasMaxLength(100)`. Читается как предложение.

### Простейшая конфигурация

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
    }
}
```

- `HasKey` — первичный ключ.
- `Property` — доступ к свойству.
- `IsRequired` — NOT NULL.
- `HasMaxLength` — максимальная длина.
- `HasColumnType` — тип колонки в БД.

**EntityTypeBuilder** — это объект, который предоставляет методы для настройки сущности.

### NoteConfiguration

```csharp
public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Title)
            .IsRequired(false)
            .HasConversion(title => title!.Value, str => new Title(str))
            .HasMaxLength(TitleValidator.MAX_LENGTH);
        builder.Property(x => x.Thesis)
            .IsRequired()
            .HasConversion(thesis => thesis.Value, str => new Thesis(str));
        builder.Property(x => x.CreationData).IsRequired().HasConversion(
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );
        builder.Property(x => x.ModificationData).IsRequired(false).HasConversion(
            src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
            dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc)
        );
        builder.HasOne(x => x.User).WithMany("_notes");
    }
}
```

**Разбор:**

- `HasKey(x => x.Id)` — первичный ключ.
- `Property(x => x.Id).IsRequired()` — колонка NOT NULL.
- **Title:** nullable-свойство. Конвертер `Title` ↔ `string`. `title!.Value` — `!` подавляет предупреждение компилятора.

**Null-forgiving operator (`!`)** — это способ сказать компилятору: «Я знаю, что здесь не null, не предупреждай». В нашем случае `Title` может быть null, но мы знаем, что если он не null — у него есть `.Value`.

- **Thesis:** обязательное. Простой конвертер.
- **CreationData:** конвертер для `DateTime`. Гарантирует UTC. Npgsql требует UTC для `timestamp with time zone`.
- **ModificationData:** nullable-версия конвертера.
- **Связь:** `HasOne(x => x.User).WithMany("_notes")`.

**DateTimeKind** — перечисление `Utc`, `Local`, `Unspecified`. Оно говорит, в каком часовом поясе дата.

**Почему конвертеры для `DateTime` такие сложные?** Потому что нужно гарантировать: что бы ни пришло из кода, в БД уйдёт UTC. А что бы ни пришло из БД — оно будет помечено как UTC.

### UserConfiguration

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Username)
            .IsRequired()
            .HasConversion(username => username.Value, str => new Username(str))
            .HasMaxLength(UsernameValidator.MAX_LENGTH);
        builder.HasMany<Note>("_notes")
            .WithOne(x => x.User)
            .HasForeignKey("UserId")
            .HasPrincipalKey(x => x.Id);
        builder.Ignore(x => x.Notes);
    }
}
```

**Разбор:**
- `HasMany<Note>("_notes")` — коллекция заметок в приватном поле.
- `.WithOne(x => x.User)` — у каждой `Note` один `User`.
- `.HasForeignKey("UserId")` — внешний ключ как теневое свойство.
- `.HasPrincipalKey(x => x.Id)` — ссылка на первичный ключ `User`.
- `Ignore(x => x.Notes)` — публичное свойство не маппится.

**Теневое свойство** — это свойство, которое существует только в модели EF Core, но не в классе C#. EF Core использует его для хранения внешнего ключа.

**Backing field** — это приватное поле, в котором хранятся данные. EF Core работает с ним, а домен контролирует изменения через методы. Это инкапсуляция.»

---

## 6. Конвертеры значений

«EF Core не умеет хранить пользовательские типы напрямую. Он знает, как хранить `string`, `int`, `DateTime`. А как хранить `Username`? Через Value Converter.

```csharp
.HasConversion(
    v => v.Value,          // в БД
    v => new Username(v)   // из БД
)
```

**Зачем?**
- Хранить value objects в БД как примитивы.
- Сохранять инкапсуляцию домена.
- При чтении — восстанавливать value object с валидацией.

**Ограничения:**
- Нельзя использовать с навигационными свойствами.
- Иногда нельзя сравнивать сконвертированные свойства в LINQ напрямую.

**Пример:** в `EfUserRepository.GetUserByUsernameAsync` мы пишем:

```csharp
_users.FirstOrDefaultAsync(u => u.Username.Equals(new Username(username)), cancellationToken);
```

Мы не можем написать `u.Username.Value == username`, потому что в БД `Username` хранится как строка. EF Core не умеет транслировать `.Value` в SQL. Но `.Equals(new Username(username))` транслирует в `=`. Почему? Потому что он видит, что `Username` сконвертирован, и применяет конвертер к `new Username(username)` — получается `username`.

**Почему `==` не работает, а `.Equals` работает?** Это особенность трансляции выражений EF Core. Для сконвертированных свойств он поддерживает `.Equals`, но не всегда — оператор `==`.»

---

## 7. Настройка связей

«**Связь** — это отношение между двумя сущностями. В нашем случае `User` и `Note` связаны: у одного пользователя много заметок.

В EF Core три типа связей:
- **One-to-Many** — один `User` — много `Note`.
- **One-to-One** — один `User` — один `Profile`.
- **Many-to-Many** — `Book` — `Author`.

**One-to-Many — простейший пример:**

```csharp
builder.Entity<Order>()
    .HasOne(o => o.Customer)
    .WithMany(c => c.Orders)
    .HasForeignKey(o => o.CustomerId);
```

Читается так: у `Order` один `Customer`, у `Customer` много `Orders`, внешний ключ — `CustomerId` в `Order`.

**One-to-One — простейший пример:**

```csharp
builder.Entity<User>()
    .HasOne(u => u.Profile)
    .WithOne(p => p.User)
    .HasForeignKey<Profile>(p => p.UserId);
```

**Many-to-Many — простейший пример:**

```csharp
builder.Entity<Book>()
    .HasMany(b => b.Authors)
    .WithMany(a => a.Books)
    .UsingEntity(j => j.ToTable("BookAuthors"));
```

EF Core создаст промежуточную таблицу `BookAuthors`.

**Пример из NotesService:**

```csharp
builder.HasMany<Note>("_notes")
    .WithOne(x => x.User)
    .HasForeignKey("UserId")
    .HasPrincipalKey(x => x.Id);
```

**Методы:**
- `HasMany` / `HasOne` — сторона связи.
- `WithOne` / `WithMany` — противоположная сторона.
- `HasForeignKey` — внешний ключ.
- `HasPrincipalKey` — ключ на главной стороне.
- `OnDelete(DeleteBehavior.Cascade)` — поведение при удалении.

**Cascade delete:** при удалении `User` удаляются все его `Note`. Это удобно, но нужно быть осторожным.

**DeleteBehavior** — это перечисление, определяющее, что делать с зависимыми сущностями при удалении главной:
- `Cascade` — удалить всех зависимых.
- `Restrict` — запретить удаление, если есть зависимые.
- `SetNull` — установить внешний ключ в NULL.
- `NoAction` — оставить как есть.

**Чек-поинт.** Ответьте: зачем нужен конвертер значений? Как настроить связь один-ко-многим?

---

## Итоги части 1

«Сегодня мы разобрали первую половину инфраструктурного слоя NotesService. Мы поняли:
- Как устроен ASP.NET Core: DI, конфигурация, middleware, контроллеры, Swagger.
- Что такое `DbContext` и `DbSet`, почему `DbContext` — Unit of Work, а `DbSet` — Repository.
- Зачем нужен трекинг и `AsNoTracking`.
- Как конфигурировать сущности через Fluent API.
- Как работают конвертеры значений для value objects.
- Как настраивать связи один-ко-многим.

На следующем занятии мы разберём паттерн Repository, миграции, подключение к БД, безопасное хранение строки подключения и слой Presentation. А затем — Application.

Если что-то осталось непонятным — не стесняйтесь спрашивать сейчас или в начале следующего занятия.

Вопросы?»
