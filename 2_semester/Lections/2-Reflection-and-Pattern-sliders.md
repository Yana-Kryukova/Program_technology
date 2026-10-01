# Лекции 5

## Слайд 1. 
**Фреймворки и паттерны, использующие рефлексию**
DI · ORM · Сериализация · AOP · ASP.NET Core


---

## Слайд 2. План лекции
- Рефлексия как «клей» фреймворков
- DI-контейнеры
- ORM и маппинг
- Сериализация
- AOP и прокси
- ASP.NET Core: маршрутизация, model binding, фильтры, валидация
- HTTP-статусы: 200, 201, 204, 400, 401, 403, 404, 409, 500
- Итоги и практика

---

## Слайд 3. Рефлексия — невидимый позвоночник
- Фреймворк не знает ваш код на этапе компиляции
- Он **читает метаданные** во время выполнения
- Атрибуты — «стикеры», которые фреймворк считывает через рефлексию
- Пример: `[HttpGet]` не меняет метод — он сообщает ASP.NET Core, что делать

```csharp
var attr = method.GetCustomAttribute<HttpGetAttribute>();
if (attr != null) { /* зарегистрировать маршрут */ }
```

---

## Слайд 4. Dependency Injection: зачем
- Ручное создание графа зависимостей — боль
- DI-контейнер строит объекты **автоматически**
- Использует рефлексию для поиска конструктора

```csharp
var ctor = typeof(Service).GetConstructors()
    .OrderByDescending(c => c.GetParameters().Length)
    .First();
var args = ctor.GetParameters()
    .Select(p => Resolve(p.ParameterType))
    .ToArray();
return ctor.Invoke(args);
```

---

## Слайд 5. DI: жизненные циклы
- **Singleton** — один на всё приложение
- **Scoped** — один на HTTP-запрос
- **Transient** — новый каждый раз

```csharp
services.AddSingleton<ICache, MemoryCache>();
services.AddScoped<IUserService, UserService>();
services.AddTransient<IEmailSender, SmtpSender>();
```

---

## Слайд 6. ORM: рефлексия для маппинга
- Объект ↔ таблица
- Атрибуты описывают схему
- Рефлексия создаёт объекты из строк

```csharp
[Table("Users")]
public class User
{
    [Key] public int Id { get; set; }
    [Column("user_name")] public string Name { get; set; }
}
```

```csharp
var user = new User();
user.Name = reader["user_name"].ToString();
```

---

## Слайд 7. Сериализация: рефлексия и атрибуты
- Обход свойств через `PropertyInfo`
- Атрибуты управляют поведением
- Source generators заменяют рефлексию для AOT

```csharp
public class Product
{
    [JsonPropertyName("product_name")]
    public string Name { get; set; }
    [JsonIgnore] public decimal InternalCost { get; set; }
}
```

---

## Слайд 8. AOP: перехват вызовов
- Сквозные concerns: логирование, кэш, транзакции
- Прокси перехватывает вызов через рефлексию

```csharp
public class LoggingProxy<T> : DispatchProxy
{
    protected override object Invoke(MethodInfo m, object[] args)
    {
        Console.WriteLine($"Вызов {m.Name}");
        return m.Invoke(_inner, args);
    }
}
```

---

## Слайд 9. ASP.NET Core: где рефлексия
- Маршрутизация — чтение `[Route]`, `[HttpGet]`
- Model binding — чтение `[FromBody]`, `[FromQuery]`
- Фильтры — чтение атрибутов-фильтров
- Валидация — чтение `[Required]`, `[Range]`
- DI — построение контроллеров

**Один контроллер = десятки рефлексивных вызовов при старте**

---

## Слайд 10. Маршрутизация через атрибуты

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]                    // GET api/products
    public IActionResult GetAll() => Ok(_products);

    [HttpGet("{id}")]            // GET api/products/5
    public IActionResult GetById(int id) => Ok(_products.Find(id));

    [HttpPost]                   // POST api/products
    public IActionResult Create([FromBody] Product p) 
        => CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
}
```

---

## Слайд 11. Атрибуты маршрутизации
- `[Route]` — шаблон URL
- `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`, `[HttpPatch]`
- Комбинируются на контроллере и действии
- Конфликт маршрутов → `AmbiguousMatchException`

```csharp
[Route("api/[controller]/[action]")]
public class ReportsController : ControllerBase
{
    [HttpGet("daily")] public IActionResult Daily() => Ok();
}
```

---

## Слайд 12. Model binding: атрибуты источника (теория)
- `[FromBody]` — тело запроса (JSON)
- `[FromQuery]` — `?id=5`
- `[FromRoute]` — `/{id}`
- `[FromForm]` — form-data
- `[FromHeader]` — заголовки
- `[FromServices]` — сервис из DI

---

## Слайд 13. Model binding: `[FromQuery]` на практике

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // GET api/products?category=books&minPrice=100&page=2
    [HttpGet]
    public IActionResult Search(
        [FromQuery] string category,
        [FromQuery] decimal minPrice = 0,
        [FromQuery] int page = 1)
    {
        var result = _repo.Search(category, minPrice, page);
        return Ok(result);
    }
}
```

**Применение:** фильтры каталога, поиск, пагинация, сортировка.

---

## Слайд 14. Model binding: `[FromRoute]` на практике

```csharp
// GET api/orders/42/items/7
[HttpGet("{orderId}/items/{itemId}")]
public IActionResult GetItem(
    [FromRoute] int orderId,
    [FromRoute] int itemId)
{
    var item = _repo.GetItem(orderId, itemId);
    if (item == null) return NotFound();
    return Ok(item);
}
```

**Применение:** RESTful-идентификация ресурсов вложенных сущностей.

---

## Слайд 15. Model binding: `[FromHeader]` на практике

```csharp
// Клиент отправляет: X-Api-Version: 2
[HttpGet("data")]
public IActionResult GetData(
    [FromHeader(Name = "X-Api-Version")] int version)
{
    if (version < 1 || version > 2)
        return BadRequest("Неподдерживаемая версия API");
    return Ok(_service.GetForVersion(version));
}
```

**Применение:** версионирование API, tenant-id, correlation-id для трейсинга.

---

## Слайд 16. Model binding: `[FromServices]` на практике

```csharp
[HttpPost("send")]
public IActionResult Send(
    [FromBody] EmailDto dto,
    [FromServices] IEmailSender sender,
    [FromServices] ILogger<NotificationController> logger)
{
    sender.Send(dto.To, dto.Subject, dto.Body);
    logger.LogInformation("Email отправлен на {To}", dto.To);
    return Ok();
}
```

**Применение:** точечное внедрение зависимости без полей класса — удобно для редких сервисов.

---

## Слайд 17. Типичная ошибка: забыли `[FromBody]`

```csharp
// ❌ model == null, хотя клиент отправил JSON
[HttpPost]
public IActionResult Create(Product model) { ... }

// ✅ правильно
[HttpPost]
public IActionResult Create([FromBody] Product model) { ... }
```

**Причина:** без `[ApiController]` complex-типы по умолчанию биндятся из формы, не из тела.
**С `[ApiController]`** inference сам подставит `[FromBody]` для complex-типа — но только для одного параметра.

---

## Слайд 18. Атрибуты `[Bind*]` на практике

```csharp
public class OrderDto
{
    [BindRequired] public int CustomerId { get; set; }
    [BindNever] public decimal Total { get; set; }
    [BindNever] public DateTime CreatedAt { get; set; }
}

// POST api/orders
// Клиент шлёт { "customerId": 5, "total": 99999 }
// Total игнорируется — защита от подмены цены клиентом
[HttpPost]
public IActionResult Create([FromBody] OrderDto dto)
{
    dto.Total = _priceService.Calculate(dto.CustomerId);
    return Ok(dto);
}
```

**Применение:** защита от Mass Assignment уязвимостей.

---

## Слайд 19. `[ApiController]` — что включает
- **Требует attribute routing** для всех действий
- **Автоматические HTTP 400** при ошибках валидации
- **Inference источников** привязки
- **Problem Details** (RFC 7807) для ошибок

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase { ... }
```

Больше не нужно писать `if (!ModelState.IsValid) return BadRequest(...)`.

---

## Слайд 20. `[ApiController]` на практике: без него и с ним

**Без `[ApiController]`:**
```csharp
[HttpPost]
public IActionResult Create(Product p)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // пишем вручную
    ...
}
```

**С `[ApiController]`:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] Product p)
    {
        // ModelState уже проверен — 400 придёт автоматически
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }
}
```

---

## Слайд 21. Фильтры как атрибуты
- **Authorization** — `[Authorize]`, `[AllowAnonymous]`
- **Resource** — до/после model binding
- **Action** — до/после действия
- **Exception** — обработка исключений
- **Result** — до/после результата

```csharp
[Authorize(Roles = "Admin")]
[HttpGet("secret")]
public IActionResult Secret() => Ok("data");
```

---

## Слайд 22. Кастомный фильтр-атрибут

```csharp
public class ResponseHeaderAttribute : ActionFilterAttribute
{
    private readonly string _name, _value;
    public ResponseHeaderAttribute(string name, string value)
        => (_name, _value) = (name, value);

    public override void OnResultExecuting(ResultExecutingContext ctx)
    {
        ctx.HttpContext.Response.Headers.Add(_name, _value);
        base.OnResultExecuting(ctx);
    }
}

[ResponseHeader("X-App", "Demo")]
[HttpGet] public IActionResult Get() => Ok();
```

---

## Слайд 23. Кастомный фильтр: логирование времени выполнения

```csharp
public class TimingAttribute : ActionFilterAttribute
{
    private Stopwatch _sw;
    private readonly ILogger<TimingAttribute> _logger;

    public TimingAttribute(ILogger<TimingAttribute> logger) 
        => _logger = logger;

    public override void OnActionExecuting(ActionExecutingContext ctx)
        => _sw = Stopwatch.StartNew();

    public override void OnActionExecuted(ActionExecutedContext ctx)
    {
        _sw.Stop();
        _logger.LogInformation(
            "Действие {Action} выполнено за {Ms} мс",
            ctx.ActionDescriptor.DisplayName, _sw.ElapsedMilliseconds);
    }
}

[ServiceFilter(typeof(TimingAttribute))]
[HttpGet] public IActionResult Heavy() => Ok(_service.Compute());
```

**Применение:** профилирование «тяжёлых» эндпоинтов, мониторинг SLA.

---

## Слайд 24. Кастомный фильтр: проверка прав на ресурс

```csharp
public class OwnResourceAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var userId = ctx.HttpContext.User.FindFirst("sub")?.Value;
        var resourceOwnerId = (string)ctx.RouteData.Values["userId"];

        if (userId != resourceOwnerId)
            ctx.Result = new ForbidResult(); // 403
    }
}

// GET api/users/42/orders — только сам пользователь 42
[OwnResource]
[HttpGet("users/{userId}/orders")]
public IActionResult GetOrders(string userId) => Ok(_orders);
```

**Применение:** защита чужих данных в multi-tenant SaaS.

---

## Слайд 25. Exception filter: единый формат ошибок

```csharp
public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext ctx)
    {
        var status = ctx.Exception switch
        {
            NotFoundException => 404,
            ValidationException => 400,
            UnauthorizedAccessException => 401,
            _ => 500
        };

        ctx.Result = new ObjectResult(new
        {
            Error = ctx.Exception.Message,
            Status = status
        })
        { StatusCode = status };

        ctx.ExceptionHandled = true;
    }
}

[ApiController]
[ApiExceptionFilter]
[Route("api/[controller]")]
public class ProductsController : ControllerBase { ... }
```

**Применение:** клиент всегда получает единый JSON-формат ошибки.

---

## Слайд 26. Валидация через атрибуты

```csharp
public class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }
}
```

ASP.NET Core читает эти атрибуты рефлексией и наполняет `ModelState`.

---

## Слайд 27. Валидация на практике: ответ 400

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        // При невалидных данных ASP.NET Core сам вернёт 400:
        // {
        //   "errors": {
        //     "Email": ["The Email field is not a valid e-mail address."],
        //     "Age": ["The field Age must be between 18 and 120."]
        //   }
        // }
        return Ok("Зарегистрирован");
    }
}
```

**Применение:** единая точка валидации — не нужно писать проверки руками.

---

## Слайд 28. Кастомный валидационный атрибут

```csharp
public class NoProfanityAttribute : ValidationAttribute
{
    private static readonly string[] Bad = { "spam", "xxx" };

    protected override ValidationResult IsValid(
        object value, ValidationContext ctx)
    {
        if (value is string s && Bad.Any(b => s.Contains(b)))
            return new ValidationResult("Недопустимые слова");
        return ValidationResult.Success;
    }
}

public class CommentDto
{
    [NoProfanity, StringLength(500)]
    public string Text { get; set; }
}
```

**Применение:** бизнес-правила прямо в модели.

---

## Слайд 29. HTTP-статусы: зачем они нужны
- Это **язык общения** между клиентом и сервером
- Клиент по коду понимает: успех, ошибка клиента, ошибка сервера
- Правильные коды = предсказуемый API
- Группы:
  - **2xx** — успех
  - **4xx** — ошибка клиента
  - **5xx** — ошибка сервера

---

## Слайд 30. 200 OK

```csharp
[HttpGet]
public IActionResult GetAll()
    => Ok(_products); // 200 + JSON
```

**Когда:** GET, успешный PUT/PATCH с телом, любой успешный запрос с данными.
**Пример:** клиент загружает список товаров, обновляет профиль и получает обновлённый объект.

---

## Слайд 31. 201 Created

```csharp
[HttpPost]
public IActionResult Create([FromBody] Product p)
{
    _repo.Add(p);
    return CreatedAtAction(
        nameof(GetById), 
        new { id = p.Id }, 
        p); // 201 + Location: /api/products/42
}
```

**Когда:** успешный POST, создающий ресурс.
**Пример:** создание заказа — клиент сразу знает URL заказа из заголовка `Location`.

---

## Слайд 32. 204 No Content

```csharp
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    _repo.Remove(id);
    return NoContent(); // 204
}
```

**Когда:** DELETE, PUT/PATCH без тела ответа.
**Пример:** удаление комментария — клиенту нечего читать в ответе.

---

## Слайд 33. 400 Bad Request

```csharp
[HttpPost]
public IActionResult Create([FromBody] RegisterDto dto)
{
    if (dto.Age < 18)
        return BadRequest("Возраст < 18");
    return Ok();
}
```

**Когда:** провал валидации, кривой JSON, неверные параметры.
**Пример:** клиент отправил `age: 12` — сервер отвечает 400 и перечисляет ошибки.

---

## Слайд 34. 401 Unauthorized и 403 Forbidden

```csharp
[Authorize]                    // 401, если нет токена
[Authorize(Roles = "Admin")]   // 403, если роль не Admin
[HttpGet] public IActionResult AdminOnly() => Ok();
```

**Разница:**
- **401** — «кто ты?» — нет или просрочен токен
- **403** — «я знаю, кто ты, но нельзя» — прав не хватает

**Пример:** обычный пользователь пытается удалить чужой пост → 403.

---

## Слайд 35. 404 Not Found

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var p = _repo.Find(id);
    if (p == null) return NotFound(); // 404
    return Ok(p);
}
```

**Когда:** запись удалена, URL неверный, id не существует.
**Пример:** клиент запрашивает `/api/products/999`, товара нет → 404.

---

## Слайд 36. 409 Conflict

```csharp
[HttpPost]
public IActionResult Register([FromBody] RegisterDto dto)
{
    if (_users.EmailExists(dto.Email))
        return Conflict("Email уже занят"); // 409
    // ...
}
```

**Когда:** дубликат уникального поля, конфликт версий (optimistic concurrency).
**Пример:** регистрация с занятым email; обновление записи, которую кто-то изменил параллельно.

---

## Слайд 37. 500 Internal Server Error

```csharp
[HttpGet] public IActionResult Crash() 
    => throw new Exception("boom"); // 500
```

**Когда:** необработанные исключения, падение БД, баги.
**Пример:** NullReferenceException в сервисе — клиент получает 500 без деталей.
**Важно:** 500 — всегда вина сервера, не клиента.

---

## Слайд 38. Сводная таблица кодов

| Код | Значение | Типичное использование |
|-----|----------|------------------------|
| 200 | OK | GET, успешный PUT |
| 201 | Created | POST с созданием ресурса |
| 204 | No Content | DELETE, PUT без тела |
| 400 | Bad Request | Ошибка валидации |
| 401 | Unauthorized | Нет аутентификации |
| 403 | Forbidden | Нет прав |
| 404 | Not Found | Ресурс не существует |
| 409 | Conflict | Дубликат, конкурентный доступ |
| 500 | Internal Server Error | Необработанное исключение |
| 503 | Service Unavailable | Сервис перегружен / на обслуживании |

---

## Слайд 39. Итоги
- Рефлексия — фундамент DI, ORM, сериализации, AOP
- ASP.NET Core — центральный практический пример
- Атрибуты контроллеров = декларативное программирование поверх рефлексии
- HTTP-статусы — язык общения клиента и сервера
- Правильные коды делают API предсказуемым

**Практика: свой DI-контейнер, ORM или кастомный фильтр.**

---

## Слайд 40. Вопросы для самопроверки
1. Как DI-контейнер находит конструктор?
2. Чем `[FromBody]` отличается от `[FromQuery]`?
3. Что делает `[ApiController]`?
4. Разница между 401 и 403?
5. Когда 201, а когда 200?
6. Когда 409, а когда 400?
7. Почему 500 — всегда вина сервера?

---

## Слайд 41. Спасибо!
Вопросы?
Практическое задание — в LMS.
