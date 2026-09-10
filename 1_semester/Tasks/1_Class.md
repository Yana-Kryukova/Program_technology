# ДОМАШНЕЕ ЗАДАНИЕ: Работа с классами, коллекциями и источниками данных

**Дисциплина:** Технологии программирования  (язык C#)  
**Тема:** Классы, коллекции, работа с CSV  
**Форма сдачи:** проект C# (Console Application) 
**Срок сдачи:** согласно расписанию

---

## Чему научится студент

Выполнив это задание, студент научится:

1. **Проектировать классы** предметной области на основе ERD-диаграммы: выделять сущности, их свойства и связи.
2. **Использовать автосвойства и вычисляемые свойства** — различать, когда нужна простая автосвойства, а когда — вычисляемое свойство с логикой.
3. **Реализовывать методы класса** — там, где нужны параметры или сложная логика, а не просто возврат значения.
4. **Работать с коллекциями** — `List<T>`, `Dictionary<TKey, TValue>`, массивами — без использования LINQ.
5. **Организовывать загрузку данных** из двух источников: в памяти (`InMemoryRepository`) и из CSV-файлов (`CsvRepository`).
6. **Строить связи между сущностями** через внешние ключи и находить связанные объекты вручную (цикл + `if`).
7. **Реализовывать аналитические методы** — группировку, сортировку, поиск максимума/минимума, подсчёт статистики.
8. **Оформлять код по стандартам C#**: правильное именование, разбиение по файлам, разделение ответственности.

---

## Соглашения по оформлению кода

### Именование

| Элемент | Стиль | Пример |
|---------|-------|--------|
| Класс | PascalCase | `Publisher`, `InMemoryRepository` |
| Свойство | PascalCase | `FullName`, `PublisherId` |
| Метод | PascalCase | `GetBooks()`, `FindAuthor()` |
| Приватное поле | _camelCase | `_publishers`, `_basePath` |
| Локальная переменная | camelCase | `foundAuthor`, `totalPages` |
| Параметр метода | camelCase | `bookTitle`, `minScholarship` |

### Файлы

- **Каждый класс — в отдельном файле** с именем, совпадающим с именем класса: `Publisher.cs`, `Book.cs`, `InMemoryRepository.cs`, `CsvRepository.cs`, `Program.cs`.
- Один файл — один публичный класс.

### Прочее

- **Запрещено:** LINQ (`Where`, `Select`, `First`, `Sum`, `Average`, `OrderBy`).
- **Разрешено:** `List<T>`, `Dictionary<TKey, TValue>`, массивы, циклы `for`/`foreach`, `if`, `switch`.
- **ВАЖНО!!!! Комментарии:** `/// <summary>` для классов и публичных методов.

---

## Создание классов InMemory для тестирования

Для **каждого** варианта создаётся класс `InMemoryRepository`, который хранит тестовые данные в памяти.

### Требования

1. **Приватные поля** — `List<T>` для каждой сущности варианта.
2. **Конструктор** — заполняет списки тестовыми данными (не менее 5 записей на сущность).
3. **Публичные методы** `Get<Сущность>()` — возвращают соответствующий список.
4. **Без LINQ** — только циклы и `if`.
5. **Данные должны быть согласованы** — внешние ключи должны ссылаться на существующие записи.

### Пример (вариант «Библиотека»)

```csharp
/// <summary>
/// Хранилище тестовых данных в памяти
/// </summary>
public class InMemoryRepository
{
    private List<Publisher> _publishers;
    private List<Author> _authors;
    private List<Book> _books;

    public InMemoryRepository()
    {
        _publishers = new List<Publisher>
        {
            new Publisher { Id = 1, Name = "Эксмо", City = "Москва" },
            new Publisher { Id = 2, Name = "Питер", City = "Санкт-Петербург" },
            new Publisher { Id = 3, Name = "АСТ", City = "Москва" }
        };

        _authors = new List<Author>
        {
            new Author { Id = 1, FullName = "Лев Толстой", Country = "Россия" },
            new Author { Id = 2, FullName = "Фёдор Достоевский", Country = "Россия" },
            new Author { Id = 3, FullName = "Антон Чехов", Country = "Россия" }
        };

        _books = new List<Book>
        {
            new Book { Id = 1, Title = "Война и мир", Year = 1869, PublisherId = 1, AuthorId = 1, Pages = 1225 },
            new Book { Id = 2, Title = "Анна Каренина", Year = 1877, PublisherId = 1, AuthorId = 1, Pages = 864 },
            new Book { Id = 3, Title = "Преступление и наказание", Year = 1866, PublisherId = 2, AuthorId = 2, Pages = 671 },
            new Book { Id = 4, Title = "Идиот", Year = 1869, PublisherId = 2, AuthorId = 2, Pages = 640 },
            new Book { Id = 5, Title = "Вишнёвый сад", Year = 1904, PublisherId = 3, AuthorId = 3, Pages = 96 }
        };
    }

    public List<Publisher> GetPublishers() { return _publishers; }
    public List<Author> GetAuthors() { return _authors; }
    public List<Book> GetBooks() { return _books; }
}
```

---

## Считывание информации из CSV

Для **каждого** варианта создаётся класс `CsvRepository`, который читает данные из CSV-файлов в папке `data`.

### Требования

1. **Отдельный CSV-файл** на каждую сущность: например, `publishers.csv`, `authors.csv`, `books.csv`.
2. **Первая строка** — заголовки (пропускается при чтении).
3. **Разделитель** — запятая.
4. **Парсинг** — `int.Parse`, `double.Parse`.
5. **Без LINQ** — `File.ReadAllLines`, `Split`, циклы, `if`.
6. **Обработка ошибок** — проверка, что файл не пустой.
7. **Возвращаемый тип** — `List<T>`.

### Пример CSV-файлов (вариант «Библиотека»)

`data/publishers.csv`:
```
Id,Name,City
1,Эксмо,Москва
2,Питер,Санкт-Петербург
3,АСТ,Москва
```

`data/authors.csv`:
```
Id,FullName,Country
1,Лев Толстой,Россия
2,Фёдор Достоевский,Россия
3,Антон Чехов,Россия
```

`data/books.csv`:
```
Id,Title,Year,PublisherId,AuthorId,Pages
1,Война и мир,1869,1,1,1225
2,Анна Каренина,1877,1,1,864
3,Преступление и наказание,1866,2,2,671
4,Идиот,1869,2,2,640
5,Вишнёвый сад,1904,3,3,96
```

### Пример класса CsvRepository

```csharp
/// <summary>
/// Чтение данных из CSV-файлов
/// </summary>
public class CsvRepository
{
    private string _basePath;

    public CsvRepository(string basePath)
    {
        _basePath = basePath;
    }

    public List<Publisher> GetPublishers()
    {
        List<Publisher> result = new List<Publisher>();
        string[] lines = File.ReadAllLines(Path.Combine(_basePath, "publishers.csv"));

        if (lines.Length < 2) return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length < 3) continue;

            Publisher p = new Publisher();
            p.Id = int.Parse(parts[0]);
            p.Name = parts[1];
            p.City = parts[2];

            result.Add(p);
        }

        return result;
    }

    public List<Author> GetAuthors()
    {
        List<Author> result = new List<Author>();
        string[] lines = File.ReadAllLines(Path.Combine(_basePath, "authors.csv"));

        if (lines.Length < 2) return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length < 3) continue;

            Author a = new Author();
            a.Id = int.Parse(parts[0]);
            a.FullName = parts[1];
            a.Country = parts[2];

            result.Add(a);
        }

        return result;
    }

    public List<Book> GetBooks()
    {
        List<Book> result = new List<Book>();
        string[] lines = File.ReadAllLines(Path.Combine(_basePath, "books.csv"));

        if (lines.Length < 2) return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length < 6) continue;

            Book b = new Book();
            b.Id = int.Parse(parts[0]);
            b.Title = parts[1];
            b.Year = int.Parse(parts[2]);
            b.PublisherId = int.Parse(parts[3]);
            b.AuthorId = int.Parse(parts[4]);
            b.Pages = int.Parse(parts[5]);

            result.Add(b);
        }

        return result;
    }
}
```

### Выбор источника в Main

```csharp
Console.WriteLine("Выберите источник данных:");
Console.WriteLine("1 - InMemory");
Console.WriteLine("2 - CSV");
int choice = int.Parse(Console.ReadLine());

List<Publisher> publishers;
List<Author> authors;
List<Book> books;

switch (choice)
{
    case 1:
        var mem = new InMemoryRepository();
        publishers = mem.GetPublishers();
        authors = mem.GetAuthors();
        books = mem.GetBooks();
        break;
    case 2:
        var csv = new CsvRepository("data");
        publishers = csv.GetPublishers();
        authors = csv.GetAuthors();
        books = csv.GetBooks();
        break;
    default:
        Console.WriteLine("Неверный выбор");
        return;
}
```

---

## Вариант 1. Космодром

**ERD:**
```
Mission (1) ────< Rocket >──── (1) Cosmonaut
  Id              Id             Id
  Name            Model          FullName
  Date            MissionId      RocketId
  Target          CosmonautId    Experience
                  Fuel           Rank
                  Payload
```

**Классы, свойства и методы:**

**`Mission`**
- Свойства: `Id`, `Name`, `Date`, `Target`
- Вычисляемое свойство `Info` — `"Луна-25 (01.09.2025, Луна)"`

**`Cosmonaut`**
- Свойства: `Id`, `FullName`, `RocketId`, `Experience`, `Rank`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 5`
- Метод `GetInfo()` — `"Иванов И.И. (10 лет, капитан)"`

**`Rocket`**
- Свойства: `Id`, `Model`, `MissionId`, `CosmonautId`, `Fuel`, `Payload`
- Вычисляемое свойство `IsHeavy` — `true`, если `Payload > 5000`
- Метод `GetInfo()` — `"Союз-2 (12000 кг полезной нагрузки)"`

**Репозитории:** `GetMissions()`, `GetCosmonauts()`, `GetRockets()`

**Методы программы:**
- `FindCosmonaut(rockets, cosmonauts, model)` — космонавт ракеты.
- `FindMission(missions, rocket)` — миссия ракеты.
- `GetTotalPayload(rockets)` — общая полезная нагрузка.
- `GetCosmonautsByRank(cosmonauts)` — `Dictionary<string, List<Cosmonaut>>`: группировка по званию.
- `PrintAllRockets(rockets, cosmonauts, missions)` — вывод ракет.

**Вывод:**
```
Количество ракет: 5, космонавтов: 6
"Союз-2" — космонавт Иванов И.И., миссия "Луна-25"
Общая полезная нагрузка: 12000 кг
Космонавты по званиям: Капитан — 2, Майор — 3, Лейтенант — 1
```

---

## Вариант 2. Кинотеатр

**ERD:**
```
Genre (1) ────< Movie >──── (1) Session
  Id              Id              Id
  Name            Title           MovieId
                  GenreId         Time
                  Duration        Hall
                  Year            Price
```

**Классы, свойства и методы:**

**`Genre`**
- Свойства: `Id`, `Name`
- Вычисляемое свойство `Info` — название жанра

**`Movie`**
- Свойства: `Id`, `Title`, `GenreId`, `Duration`, `Year`
- Вычисляемое свойство `IsLong` — `true`, если `Duration > 120`
- Метод `GetInfo()` — `"Интерстеллар (2014, 169 мин)"`

**`Session`**
- Свойства: `Id`, `MovieId`, `Time`, `Hall`, `Price`
- Вычисляемое свойство `IsEvening` — `true`, если `Time >= 18:00`
- Метод `GetInfo()` — `"18:30, зал 3, 450 руб."`

**Репозитории:** `GetGenres()`, `GetMovies()`, `GetSessions()`

**Методы программы:**
- `FindGenre(movies, genres, title)` — по названию фильма находит жанр.
- `FindSession(sessions, movie)` — первый сеанс для фильма.
- `GetTotalDuration(movies)` — суммарная длительность всех фильмов.
- `GroupMoviesByGenre(movies, genres)` — `Dictionary<string, List<Movie>>`: фильмы, сгруппированные по жанрам.
- `PrintAllMovies(movies, genres)` — вывод фильмов с жанром.

**Вывод:**
```
Количество фильмов: 4, сеансов: 6
Фильм "Интерстеллар" жанра "Фантастика", сеанс в 18:30
Общая длительность: 520 минут
Фантастика: Интерстеллар, Начало
Драма: Зелёная миля
```

---

## Вариант 3. Университет

**ERD:**
```
Faculty (1) ────< Group >──── (1) Student
  Id               Id              Id
  Name             Name            FullName
  Dean             FacultyId       GroupId
                   Course          Age
                                   Scholarship
```

**Классы, свойства и методы:**

**`Faculty`**
- Свойства: `Id`, `Name`, `Dean`
- Вычисляемое свойство `Info` — `"Информатики (декан: Иванов И.И.)"`

**`Group`**
- Свойства: `Id`, `Name`, `FacultyId`, `Course`
- Вычисляемое свойство `IsSenior` — `true`, если `Course >= 4`
- Метод `GetInfo()` — `"ИС-21 (2 курс)"`

**`Student`**
- Свойства: `Id`, `FullName`, `GroupId`, `Age`, `Scholarship`
- Вычисляемое свойство `HasScholarship` — `true`, если `Scholarship > 0`
- Метод `GetInfo()` — `"Иванов И.И. (20 лет, стипендия 8000)"`

**Репозитории:** `GetFaculties()`, `GetGroups()`, `GetStudents()`

**Методы программы:**
- `FindGroup(students, groups, name)` — группа студента по имени.
- `FindFaculty(faculties, group)` — факультет группы.
- `GetTotalScholarship(students)` — суммарная стипендия.
- `GetStudentsWithHighScholarship(students, min)` — `List<Student>` со стипендией выше порога, отсортированный по убыванию (пузырьковая сортировка).
- `PrintAllStudents(students, groups, faculties)` — вывод студентов с группой и факультетом.

**Вывод:**
```
Количество студентов: 10, групп: 4
Студент Иванов И.И. учится в группе ИС-21 факультета Информатики
Общая стипендия: 45000 руб.
Студенты со стипендией > 5000: Иванов (8000), Петров (6500)
```

---

## Вариант 4. Больница

**ERD:**
```
Department (1) ────< Doctor >──── (1) Patient
  Id                  Id              Id
  Name                FullName        FullName
  Head                DepartmentId    DoctorId
                      Specialty       Diagnosis
                                      Age
```

**Классы, свойства и методы:**

**`Department`**
- Свойства: `Id`, `Name`, `Head`
- Вычисляемое свойство `Info` — `"Терапия (зав.: Сидоров С.С.)"`

**`Doctor`**
- Свойства: `Id`, `FullName`, `DepartmentId`, `Specialty`
- Вычисляемое свойство `IsSurgeon` — `true`, если `Specialty == "Хирург"`
- Метод `GetInfo()` — `"Сидоров С.С. (терапевт)"`

**`Patient`**
- Свойства: `Id`, `FullName`, `DoctorId`, `Diagnosis`, `Age`
- Вычисляемое свойство `IsElderly` — `true`, если `Age > 60`
- Метод `GetInfo()` — `"Петров П.П. (58 лет, грипп)"`

**Репозитории:** `GetDepartments()`, `GetDoctors()`, `GetPatients()`

**Методы программы:**
- `FindDoctor(patients, doctors, name)` — врач пациента.
- `FindDepartment(departments, doctor)` — отделение врача.
- `GetAverageAge(patients)` — средний возраст (проверка деления на 0).
- `CountPatientsByDiagnosis(patients)` — `Dictionary<string, int>`: количество пациентов по диагнозам.
- `PrintAllPatients(patients, doctors, departments)` — вывод пациентов с врачом и отделением.

**Вывод:**
```
Количество пациентов: 8, врачей: 5
Пациент Петров П.П. у врача Сидорова С.С. (терапевт), отделение Терапии
Средний возраст: 47 лет
Диагнозы: Грипп — 3, Ангина — 2, Бронхит — 1
```

---

## Вариант 5. Магазин

**ERD:**
```
Supplier (1) ────< Product >──── (1) Category
  Id                Id               Id
  Name              Name             Name
  Country           Price            Description
                    SupplierId
                    CategoryId
                    Quantity
```

**Классы, свойства и методы:**

**`Supplier`**
- Свойства: `Id`, `Name`, `Country`
- Вычисляемое свойство `IsForeign` — `true`, если `Country != "Россия"`
- Метод `GetInfo()` — `"Samsung (Южная Корея)"`

**`Category`**
- Свойства: `Id`, `Name`, `Description`
- Вычисляемое свойство `Info` — `"Электроника — бытовая техника"`

**`Product`**
- Свойства: `Id`, `Name`, `Price`, `SupplierId`, `CategoryId`, `Quantity`
- Вычисляемое свойство `TotalPrice` — `Price * Quantity`
- Вычисляемое свойство `IsExpensive` — `true`, если `Price > 10000`
- Метод `GetInfo()` — `"Ноутбук (75000 руб., 10 шт.)"`

**Репозитории:** `GetSuppliers()`, `GetCategories()`, `GetProducts()`

**Методы программы:**
- `FindCategory(products, categories, name)` — категория товара по названию.
- `FindSupplier(suppliers, product)` — поставщик товара.
- `GetTotalPrice(products)` — суммарная стоимость (`Price * Quantity` по всем товарам).
- `GetMostExpensiveProductPerCategory(products, categories)` — `Dictionary<string, Product>`: самый дорогой товар в каждой категории.
- `PrintAllProducts(products, categories, suppliers)` — вывод товаров с категорией и поставщиком.

**Вывод:**
```
Количество товаров: 7, категорий: 3
Товар "Ноутбук" категории "Электроника" от "Samsung"
Общая стоимость: 1250000 руб.
Электроника: Ноутбук (75000), Одежда: Куртка (12000)
```

---

## Вариант 6. Автопарк

**ERD:**
```
Route (1) ────< Car >──── (1) Driver
  Id             Id            Id
  Name           Model         FullName
  Distance       RouteId       CarId
                 Year          Experience
                 Number        License
```

**Классы, свойства и методы:**

**`Route`**
- Свойства: `Id`, `Name`, `Distance`
- Вычисляемое свойство `IsLong` — `true`, если `Distance > 50`
- Метод `GetInfo()` — `"Городской (25 км)"`

**`Car`**
- Свойства: `Id`, `Model`, `RouteId`, `Year`, `Number`
- Вычисляемое свойство `IsNew` — `true`, если `Year >= 2020`
- Метод `GetInfo()` — `"Toyota Camry (2021, А123БВ)"`

**`Driver`**
- Свойства: `Id`, `FullName`, `CarId`, `Experience`, `License`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 5`
- Метод `GetInfo()` — `"Иванов И.И. (10 лет стажа)"`

**Репозитории:** `GetRoutes()`, `GetCars()`, `GetDrivers()`

**Методы программы:**
- `FindDriver(cars, drivers, number)` — водитель по номеру машины.
- `FindRoute(routes, car)` — маршрут машины.
- `GetTotalDistance(routes)` — суммарное расстояние всех маршрутов.
- `GetDriversWithMultipleCars(cars, drivers)` — `List<Driver>`, у которых больше одной машины.
- `PrintAllCars(cars, drivers, routes)` — вывод машин с водителем и маршрутом.

**Вывод:**
```
Количество машин: 6, водителей: 5
Машина А123БВ у водителя Иванова И.И., маршрут "Городской" (25 км)
Общая протяжённость: 180 км
Водители с 2+ машинами: Иванов (2), Петров (3)
```

---

## Вариант 7. Ресторан

**ERD:**
```
Chef (1) ────< Dish >──── (1) Category
  Id            Id            Id
  FullName      Name          Name
  Specialty     ChefId        Type
                CategoryId    Description
                Price
                Weight
```

**Классы, свойства и методы:**

**`Chef`**
- Свойства: `Id`, `FullName`, `Specialty`
- Вычисляемое свойство `IsChef` — `true`, если `Specialty == "Шеф-повар"`
- Метод `GetInfo()` — `"Петрова А.А. (шеф-повар)"`

**`Category`**
- Свойства: `Id`, `Name`, `Type`
- Вычисляемое свойство `Info` — `"Супы — горячие блюда"`

**`Dish`**
- Свойства: `Id`, `Name`, `ChefId`, `CategoryId`, `Price`, `Weight`
- Вычисляемое свойство `PricePerGram` — `Price / Weight`
- Вычисляемое свойство `IsHeavy` — `true`, если `Weight > 500`
- Метод `GetInfo()` — `"Борщ (350 руб., 400 г)"`

**Репозитории:** `GetChefs()`, `GetCategories()`, `GetDishes()`

**Методы программы:**
- `FindChef(dishes, chefs, name)` — повар, готовящий блюдо.
- `FindCategory(categories, dish)` — категория блюда.
- `GetTotalWeight(dishes)` — общий вес всех блюд.
- `GetDishesByChefSortedByPrice(dishes, chefs, chefName)` — `List<Dish>` указанного повара, отсортированный по цене.
- `PrintAllDishes(dishes, chefs, categories)` — вывод блюд с поваром и категорией.

**Вывод:**
```
Количество блюд: 9, поваров: 4
Блюдо "Борщ" готовит Петрова А.А., категория "Супы"
Общий вес: 3200 г
Блюда Петровой по цене: Борщ (350), Солянка (400), Окрошка (250)
```

---

## Вариант 8. Аэропорт

**ERD:**
```
Airline (1) ────< Plane >──── (1) Flight
  Id              Id              Id
  Name            Model           PlaneId
  Country         AirlineId       Destination
                  Capacity        Departure
                                  Price
```

**Классы, свойства и методы:**

**`Airline`**
- Свойства: `Id`, `Name`, `Country`
- Вычисляемое свойство `IsInternational` — `true`, если `Country != "Россия"`
- Метод `GetInfo()` — `"Аэрофлот (Россия)"`

**`Plane`**
- Свойства: `Id`, `Model`, `AirlineId`, `Capacity`
- Вычисляемое свойство `IsBig` — `true`, если `Capacity > 200`
- Метод `GetInfo()` — `"Boeing 737 (180 мест)"`

**`Flight`**
- Свойства: `Id`, `PlaneId`, `Destination`, `Departure`, `Price`
- Вычисляемое свойство `IsMorning` — `true`, если `Departure < 12:00`
- Метод `GetInfo()` — `"Москва, 08:30, 5500 руб."`

**Репозитории:** `GetAirlines()`, `GetPlanes()`, `GetFlights()`

**Методы программы:**
- `FindPlane(flights, planes, destination)` — самолёт по направлению.
- `FindAirline(airlines, plane)` — авиакомпания самолёта.
- `GetTotalCapacity(planes)` — суммарная вместимость всех самолётов.
- `GetBusiestAirline(flights, planes, airlines)` — авиакомпания с наибольшим числом рейсов.
- `PrintAllFlights(flights, planes, airlines)` — вывод рейсов с самолётом и авиакомпанией.

**Вывод:**
```
Количество рейсов: 8, самолётов: 5
Рейс в "Москва" — Boeing 737, авиакомпания "Аэрофлот"
Общая вместимость: 1200 пассажиров
Самая загруженная авиакомпания: Аэрофлот (4 рейса)
```

---

## Вариант 9. Музей

**ERD:**
```
Curator (1) ────< Exhibit >──── (1) Hall
  Id              Id               Id
  FullName        Name             Name
  Specialty       CuratorId        Floor
                  HallId           Area
                  Year
                  Price
```

**Классы, свойства и методы:**

**`Curator`**
- Свойства: `Id`, `FullName`, `Specialty`
- Вычисляемое свойство `IsRestorer` — `true`, если `Specialty == "Реставратор"`
- Метод `GetInfo()` — `"Смирнова Е.В. (реставратор)"`

**`Hall`**
- Свойства: `Id`, `Name`, `Floor`, `Area`
- Вычисляемое свойство `IsUpper` — `true`, если `Floor > 1`
- Метод `GetInfo()` — `"Античность (2 этаж, 200 м²)"`

**`Exhibit`**
- Свойства: `Id`, `Name`, `CuratorId`, `HallId`, `Year`, `Price`
- Вычисляемое свойство `IsAncient` — `true`, если `Year < 1000`
- Вычисляемое свойство `IsValuable` — `true`, если `Price > 1000000`
- Метод `GetInfo()` — `"Амфора (500 до н.э., 50000 руб.)"`

**Репозитории:** `GetCurators()`, `GetHalls()`, `GetExhibits()`

**Методы программы:**
- `FindCurator(exhibits, curators, name)` — куратор экспоната.
- `FindHall(halls, exhibit)` — зал экспоната.
- `GetTotalPrice(exhibits)` — общая стоимость экспонатов.
- `GetExhibitsByHallSortedByYear(halls, exhibits, hallName)` — экспонаты зала, отсортированные по году.
- `PrintAllExhibits(exhibits, curators, halls)` — вывод экспонатов с куратором и залом.

**Вывод:**
```
Количество экспонатов: 10, залов: 4
Экспонат "Древняя амфора" курирует Смирнова Е.В., зал "Античность" (2 этаж)
Общая стоимость: 8500000 руб.
Экспонаты зала Античность: Амфора (500 до н.э.), Статуя (200 до н.э.)
```

---

## Вариант 10. Спортзал

**ERD:**
```
Trainer (1) ────< Workout >──── (1) Client
  Id               Id              Id
  FullName         Name            FullName
  Specialization   TrainerId       WorkoutId
                   Duration        Age
                   Price           Phone
```

**Классы, свойства и методы:**

**`Trainer`**
- Свойства: `Id`, `FullName`, `Specialization`
- Вычисляемое свойство `IsYogaTrainer` — `true`, если `Specialization == "Йога"`
- Метод `GetInfo()` — `"Орлова М.И. (йога)"`

**`Workout`**
- Свойства: `Id`, `Name`, `TrainerId`, `Duration`, `Price`
- Вычисляемое свойство `PricePerMinute` — `Price / Duration`
- Вычисляемое свойство `IsLong` — `true`, если `Duration > 60`
- Метод `GetInfo()` — `"Йога (60 мин, 800 руб.)"`

**`Client`**
- Свойства: `Id`, `FullName`, `WorkoutId`, `Age`, `Phone`
- Вычисляемое свойство `IsYoung` — `true`, если `Age < 25`
- Метод `GetInfo()` — `"Сергеева О.П. (22 года)"`

**Репозитории:** `GetTrainers()`, `GetWorkouts()`, `GetClients()`

**Методы программы:**
- `FindTrainer(workouts, trainers, name)` — тренер тренировки.
- `FindClient(clients, workout)` — клиент тренировки.
- `GetTotalDuration(workouts)` — суммарная длительность тренировок.
- `GetTrainerWorkload(workouts, trainers)` — `Dictionary<string, int>`: суммарная длительность по тренерам.
- `PrintAllWorkouts(workouts, trainers, clients)` — вывод тренировок.

**Вывод:**
```
Количество тренировок: 6, тренеров: 3
Тренировка "Йога" проводит Орлова М.И., клиент Сергеева О.П.
Общая длительность: 360 минут
Загрузка тренеров: Орлова — 120 мин, Иванов — 150 мин
```

---

## Вариант 11. Банк

**ERD:**
```
Branch (1) ────< Account >──── (1) Client
  Id              Id               Id
  Name            Number           FullName
  Address         BranchId         AccountId
                  Balance          Passport
                  Type             Phone
```

**Классы, свойства и методы:**

**`Branch`**
- Свойства: `Id`, `Name`, `Address`
- Вычисляемое свойство `IsCentral` — `true`, если `Name == "Центральное"`
- Метод `GetInfo()` — `"Центральное (ул. Ленина, 1)"`

**`Client`**
- Свойства: `Id`, `FullName`, `AccountId`, `Passport`, `Phone`
- Метод `GetInfo()` — `"Иванов И.И. (паспорт 1234 567890)"`

**`Account`**
- Свойства: `Id`, `Number`, `BranchId`, `Balance`, `Type`
- Вычисляемое свойство `IsVip` — `true`, если `Balance > 1000000`
- Метод `GetBalanceInUsd(double rate)` — баланс в долларах по курсу
- Метод `GetInfo()` — `"40817810001 (дебетовый, 250000 руб.)"`

**Репозитории:** `GetBranches()`, `GetClients()`, `GetAccounts()`

**Методы программы:**
- `FindClient(accounts, clients, number)` — клиент по номеру счёта.
- `FindBranch(branches, account)` — отделение счёта.
- `GetTotalBalance(accounts)` — суммарный баланс.
- `GetClientsWithMultipleAccounts(accounts, clients)` — клиенты с более чем одним счётом.
- `PrintAllAccounts(accounts, clients, branches)` — вывод счетов с клиентом и отделением.

**Вывод:**
```
Количество счетов: 7, клиентов: 5
Счёт 40817810001 у Иванова И.И., отделение "Центральное"
Общий баланс: 1250000 руб.
Клиенты с 2+ счетами: Иванов (2), Петров (3)
```

---

## Вариант 12. Онлайн-курсы

**ERD:**
```
Teacher (1) ────< Course >──── (1) Student
  Id              Id              Id
  FullName        Title           FullName
  Subject         TeacherId       CourseId
                  Duration        Email
                  Price           Progress
```

**Классы, свойства и методы:**

**`Teacher`**
- Свойства: `Id`, `FullName`, `Subject`
- Метод `GetInfo()` — `"Петров П.П. (программирование)"`

**`Course`**
- Свойства: `Id`, `Title`, `TeacherId`, `Duration`, `Price`
- Вычисляемое свойство `PricePerHour` — `Price / Duration`
- Вычисляемое свойство `IsLong` — `true`, если `Duration > 40`
- Метод `GetInfo()` — `"C# для начинающих (40 ч, 15000 руб.)"`

**`Student`**
- Свойства: `Id`, `FullName`, `CourseId`, `Email`, `Progress`
- Вычисляемое свойство `IsExcellent` — `true`, если `Progress >= 90`
- Вычисляемое свойство `IsFailing` — `true`, если `Progress < 50`
- Метод `GetInfo()` — `"Сидоров С.С. (прогресс 75%)"`

**Репозитории:** `GetTeachers()`, `GetCourses()`, `GetStudents()`

**Методы программы:**
- `FindTeacher(courses, teachers, title)` — преподаватель курса.
- `FindStudent(students, course)` — студент курса.
- `GetTotalDuration(courses)` — суммарная длительность курсов.
- `GetTopStudents(students, topN)` — топ-N студентов по прогрессу.
- `PrintAllCourses(courses, teachers, students)` — вывод курсов.

**Вывод:**
```
Количество курсов: 5, преподавателей: 3
Курс "C# для начинающих" ведёт Петров П.П., студент Сидоров С.С. (75%)
Общая длительность: 120 часов
Топ-3 студента: Иванов (95%), Петров (88%), Сидоров (75%)
```

---

## Вариант 13. Ферма

**ERD:**
```
Farmer (1) ────< Animal >──── (1) Pen
  Id              Id              Id
  FullName        Name            Number
  Experience      FarmerId        PenId
                  PenId           Area
                  Age             Type
                  Weight
```

**Классы, свойства и методы:**

**`Farmer`**
- Свойства: `Id`, `FullName`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 5`
- Метод `GetInfo()` — `"Иванов И.И. (10 лет опыта)"`

**`Pen`**
- Свойства: `Id`, `Number`, `Area`, `Type`
- Вычисляемое свойство `IsBig` — `true`, если `Area > 100`
- Метод `GetInfo()` — `"Загон №3 (150 м², коровник)"`

**`Animal`**
- Свойства: `Id`, `Name`, `FarmerId`, `PenId`, `Age`, `Weight`
- Вычисляемое свойство `IsAdult` — `true`, если `Age > 2`
- Вычисляемое свойство `IsHeavy` — `true`, если `Weight > 500`
- Метод `GetInfo()` — `"Бурёнка (3 года, 600 кг)"`

**Репозитории:** `GetFarmers()`, `GetPens()`, `GetAnimals()`

**Методы программы:**
- `FindFarmer(animals, farmers, name)` — фермер животного.
- `FindPen(pens, animal)` — загон животного.
- `GetTotalWeight(animals)` — общий вес.
- `GetHeaviestAnimalPerPen(animals, pens)` — самое тяжёлое животное в каждом загоне.
- `PrintAllAnimals(animals, farmers, pens)` — вывод животных.

**Вывод:**
```
Количество животных: 12, фермеров: 3
Животное "Бурёнка" у Иванова И.И., загон №3
Общий вес: 4500 кг
Самые тяжёлые в загонах: Загон 1 — Бык (800), Загон 2 — Корова (600)
```

---

## Вариант 14. IT-компания

**ERD:**
```
Project (1) ────< Employee >──── (1) Department
  Id               Id               Id
  Name             FullName         Name
  Budget           ProjectId        Head
  Deadline         DepartmentId     Floor
                   Position
                   Salary
```

**Классы, свойства и методы:**

**`Project`**
- Свойства: `Id`, `Name`, `Budget`, `Deadline`
- Вычисляемое свойство `IsExpensive` — `true`, если `Budget > 1000000`
- Метод `GetInfo()` — `"CRM-система (2 000 000 руб.)"`

**`Department`**
- Свойства: `Id`, `Name`, `Head`, `Floor`
- Метод `IsOnFloor(int floor)` — `true`, если отдел на указанном этаже
- Метод `GetInfo()` — `"Разработка (зав.: Петров П.П., 3 этаж)"`

**`Employee`**
- Свойства: `Id`, `FullName`, `ProjectId`, `DepartmentId`, `Position`, `Salary`
- Вычисляемое свойство `IsSenior` — `true`, если `Position == "Senior"`
- Вычисляемое свойство `AnnualSalary` — `Salary * 12`
- Метод `GetInfo()` — `"Петров П.П. (Senior, 150000 руб.)"`

**Репозитории:** `GetProjects()`, `GetDepartments()`, `GetEmployees()`

**Методы программы:**
- `FindDepartment(employees, departments, name)` — отдел сотрудника.
- `FindProject(projects, employee)` — проект сотрудника.
- `GetTotalSalary(employees)` — фонд зарплат.
- `GetAverageSalaryByDepartment(employees, departments)` — средняя зарплата по отделам.
- `PrintAllEmployees(employees, departments, projects)` — вывод сотрудников.

**Вывод:**
```
Количество сотрудников: 15, отделов: 4
Петров П.П. в отделе "Разработка", проект "CRM-система"
Общий фонд зарплат: 2500000 руб.
Средняя зарплата: Разработка — 150000, Тестирование — 90000
```

---

## Вариант 15. Гостиница

**ERD:**
```
Floor (1) ────< Room >──── (1) Guest
  Id            Id            Id
  Number        Number        FullName
  Description   FloorId       RoomId
                Type          Passport
                Price         CheckIn
                Capacity      CheckOut
```

**Классы, свойства и методы:**

**`Floor`**
- Свойства: `Id`, `Number`, `Description`
- Вычисляемое свойство `IsUpper` — `true`, если `Number > 5`
- Метод `GetInfo()` — `"3 этаж (стандарт)"`

**`Room`**
- Свойства: `Id`, `Number`, `FloorId`, `Type`, `Price`, `Capacity`
- Вычисляемое свойство `IsLux` — `true`, если `Type == "Люкс"`
- Метод `GetTotalPrice(int days)` — стоимость проживания за N дней
- Метод `GetInfo()` — `"305 (люкс, 5000 руб./сутки)"`

**`Guest`**
- Свойства: `Id`, `FullName`, `RoomId`, `Passport`, `CheckIn`, `CheckOut`
- Вычисляемое свойство `StayDays` — количество дней проживания
- Метод `GetInfo()` — `"Иванов И.И. (3 дня)"`

**Репозитории:** `GetFloors()`, `GetRooms()`, `GetGuests()`

**Методы программы:**
- `FindGuest(rooms, guests, number)` — гость по номеру комнаты.
- `FindFloor(floors, room)` — этаж номера.
- `GetTotalPrice(rooms)` — суммарная стоимость номеров.
- `GetRoomsWithGuests(rooms, guests)` — `Dictionary<string, int>`: число гостей в каждом номере.
- `PrintAllRooms(rooms, floors, guests)` — вывод номеров.

**Вывод:**
```
Количество номеров: 10, постояльцев: 6
Номер 305 занимает Иванов И.И., этаж 3
Общая стоимость: 45000 руб./сутки
Номера с гостями: 305 — 2, 401 — 1, 502 — 3
```

---

## Вариант 16. Парк аттракционов

**ERD:**
```
Zone (1) ────< Attraction >──── (1) Operator
  Id            Id                 Id
  Name          Name               FullName
  Area          ZoneId             AttractionId
                OperatorId         Shift
                Price              Experience
                Capacity
```

**Классы, свойства и методы:**

**`Zone`**
- Свойства: `Id`, `Name`, `Area`
- Вычисляемое свойство `IsFamily` — `true`, если `Name == "Семейная"`
- Метод `GetInfo()` — `"Семейная (500 м²)"`

**`Operator`**
- Свойства: `Id`, `FullName`, `AttractionId`, `Shift`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 3`
- Метод `GetInfo()` — `"Сидоров С.С. (5 лет опыта)"`

**`Attraction`**
- Свойства: `Id`, `Name`, `ZoneId`, `OperatorId`, `Price`, `Capacity`
- Вычисляемое свойство `IsExtreme` — `true`, если в названии есть "Экстрим"
- Метод `GetTotalRevenue(int visitors)` — выручка при N посетителях
- Метод `GetInfo()` — `"Колесо обозрения (300 руб., 50 мест)"`

**Репозитории:** `GetZones()`, `GetOperators()`, `GetAttractions()`

**Методы программы:**
- `FindOperator(attractions, operators, name)` — оператор аттракциона.
- `FindZone(zones, attraction)` — зона аттракциона.
- `GetTotalCapacity(attractions)` — суммарная вместимость.
- `GetZoneWithMaxCapacity(attractions, zones)` — зона с макс. вместимостью.
- `PrintAllAttractions(attractions, operators, zones)` — вывод аттракционов.

**Вывод:**
```
Количество аттракционов: 8, операторов: 5
"Колесо обозрения" обслуживает Сидоров С.С., зона "Семейная"
Общая вместимость: 400 человек
Зона с макс. вместимостью: Экстрим (200)
```

---

## Вариант 17. Пекарня

**ERD:**
```
Workshop (1) ────< Bakery >──── (1) Baker
  Id               Id              Id
  Name             Name            FullName
  Head             WorkshopId      BakeryId
                   BakerId         Experience
                   Price           Shift
                   Weight
```

**Классы, свойства и методы:**

**`Workshop`**
- Свойства: `Id`, `Name`, `Head`
- Вычисляемое свойство `IsConfectionery` — `true`, если `Name == "Кондитерский"`
- Метод `GetInfo()` — `"Кондитерский (зав.: Петрова А.А.)"`

**`Baker`**
- Свойства: `Id`, `FullName`, `BakeryId`, `Experience`, `Shift`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 3`
- Метод `GetInfo()` — `"Петрова А.А. (5 лет опыта)"`

**`Bakery`**
- Свойства: `Id`, `Name`, `WorkshopId`, `BakerId`, `Price`, `Weight`
- Вычисляемое свойство `PricePerGram` — `Price / Weight`
- Вычисляемое свойство `IsHeavy` — `true`, если `Weight > 300`
- Метод `GetInfo()` — `"Круассан (80 руб., 100 г)"`

**Репозитории:** `GetWorkshops()`, `GetBakers()`, `GetBakery()`

**Методы программы:**
- `FindBaker(bakery, bakers, name)` — пекарь изделия.
- `FindWorkshop(workshops, item)` — цех изделия.
- `GetTotalWeight(bakery)` — общий вес.
- `GetBakerWithMaxWeight(bakery, bakers)` — пекарь с максимальным весом.
- `PrintAllBakery(bakery, bakers, workshops)` — вывод изделий.

**Вывод:**
```
Количество изделий: 12, пекарей: 4
"Круассан" печёт Петрова А.А., цех "Кондитерский"
Общий вес: 5600 г
Пекарь с макс. весом: Петрова А.А. (1800 г)
```

---

## Вариант 18. Зоопарк

**ERD:**
```
Keeper (1) ────< Animal >──── (1) Aviary
  Id             Id              Id
  FullName       Name            Number
  Experience     KeeperId        AviaryId
                 AviaryId        Type
                 Species         Area
                 Age
```

**Классы, свойства и методы:**

**`Keeper`**
- Свойства: `Id`, `FullName`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 5`
- Метод `GetInfo()` — `"Иванов И.И. (10 лет опыта)"`

**`Aviary`**
- Свойства: `Id`, `Number`, `Type`, `Area`
- Вычисляемое свойство `IsBig` — `true`, если `Area > 50`
- Метод `GetInfo()` — `"Вольер №7 (100 м², хищники)"`

**`Animal`**
- Свойства: `Id`, `Name`, `KeeperId`, `AviaryId`, `Species`, `Age`
- Вычисляемое свойство `IsPredator` — `true`, если `Species == "Хищник"`
- Вычисляемое свойство `IsOld` — `true`, если `Age > 10`
- Метод `GetInfo()` — `"Лев (5 лет, хищник)"`

**Репозитории:** `GetKeepers()`, `GetAviaries()`, `GetAnimals()`

**Методы программы:**
- `FindKeeper(animals, keepers, name)` — смотритель животного.
- `FindAviary(aviaries, animal)` — вольер животного.
- `GetAverageAge(animals)` — средний возраст.
- `CountAnimalsBySpecies(animals)` — количество по видам.
- `PrintAllAnimals(animals, keepers, aviaries)` — вывод животных.

**Вывод:**
```
Количество животных: 15, смотрителей: 5
"Лев" обслуживает Иванов И.И., вольер №7
Средний возраст: 8 лет
Виды: Хищник — 6, Травоядный — 9
```

---

## Вариант 19. Типография

**ERD:**
```
Department (1) ────< Edition >──── (1) Editor
  Id                 Id              Id
  Name               Title           FullName
  Head               DepartmentId    EditionId
                     EditorId        Experience
                     Pages           Specialty
                     Price
```

**Классы, свойства и методы:**

**`Department`**
- Свойства: `Id`, `Name`, `Head`
- Вычисляемое свойство `IsFiction` — `true`, если `Name == "Художественная литература"`
- Метод `GetInfo()` — `"Художественная литература (зав.: Смирнова Е.В.)"`

**`Editor`**
- Свойства: `Id`, `FullName`, `EditionId`, `Experience`, `Specialty`
- Вычисляемое свойство `IsSenior` — `true`, если `Experience > 10`
- Метод `GetInfo()` — `"Смирнова Е.В. (15 лет опыта)"`

**`Edition`**
- Свойства: `Id`, `Title`, `DepartmentId`, `EditorId`, `Pages`, `Price`
- Вычисляемое свойство `IsThick` — `true`, если `Pages > 500`
- Вычисляемое свойство `PricePerPage` — `Price / Pages`
- Метод `GetInfo()` — `"Тихий Дон (600 стр., 1200 руб.)"`

**Репозитории:** `GetDepartments()`, `GetEditors()`, `GetEditions()`

**Методы программы:**
- `FindEditor(editions, editors, title)` — редактор издания.
- `FindDepartment(departments, edition)` — отдел издания.
- `GetTotalPages(editions)` — суммарное число страниц.
- `GetEditorWithMostPages(editions, editors)` — редактор с макс. страниц.
- `PrintAllEditions(editions, editors, departments)` — вывод изданий.

**Вывод:**
```
Количество изданий: 9, редакторов: 4
"Тихий Дон" редактирует Смирнова Е.В., отдел "Художественная литература"
Общее количество страниц: 3200
Редактор с макс. страниц: Смирнова Е.В. (1200)
```

---

## Вариант 20. Склад

**ERD:**
```
Section (1) ────< Item >──── (1) Storekeeper
  Id              Id             Id
  Name            Name           FullName
  Area            SectionId      ItemId
                  StorekeeperId  Shift
                  Quantity       Experience
                  Price
```

**Классы, свойства и методы:**

**`Section`**
- Свойства: `Id`, `Name`, `Area`
- Вычисляемое свойство `IsBig` — `true`, если `Area > 200`
- Метод `GetInfo()` — `"Метизы (300 м²)"`

**`Storekeeper`**
- Свойства: `Id`, `FullName`, `ItemId`, `Shift`, `Experience`
- Вычисляемое свойство `IsMorningShift` — `true`, если `Shift == "Утренняя"`
- Метод `GetInfo()` — `"Петров П.П. (5 лет опыта)"`

**`Item`**
- Свойства: `Id`, `Name`, `SectionId`, `StorekeeperId`, `Quantity`, `Price`
- Вычисляемое свойство `TotalValue` — `Quantity * Price`
- Метод `IsLowStock(int threshold)` — `true`, если `Quantity < threshold`
- Метод `GetInfo()` — `"Болты М8 (1000 шт., 5 руб.)"`

**Репозитории:** `GetSections()`, `GetStorekeepers()`, `GetItems()`

**Методы программы:**
- `FindStorekeeper(items, storekeepers, name)` — кладовщик товара.
- `FindSection(sections, item)` — секция товара.
- `GetTotalQuantity(items)` — общее количество.
- `GetItemsBelowThreshold(items, threshold)` — товары ниже порога.
- `PrintAllItems(items, storekeepers, sections)` — вывод товаров.

**Вывод:**
```
Количество товаров: 20, кладовщиков: 3
"Болты М8" у Петрова П.П., секция "Метизы"
Общее количество: 15000 шт.
Товары ниже порога 100: Гайки (50), Шайбы (80)
```

---

## Вариант 21. Турагентство

**ERD:**
```
Country (1) ────< Tour >──── (1) Manager
  Id              Id            Id
  Name            Name          FullName
  Continent       CountryId     TourId
                  ManagerId     Phone
                  Price         Experience
                  Days
```

**Классы, свойства и методы:**

**`Country`**
- Свойства: `Id`, `Name`, `Continent`
- Вычисляемое свойство `IsEurope` — `true`, если `Continent == "Европа"`
- Метод `GetInfo()` — `"Турция (Азия)"`

**`Manager`**
- Свойства: `Id`, `FullName`, `TourId`, `Phone`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 3`
- Метод `GetInfo()` — `"Иванова А.А. (5 лет опыта)"`

**`Tour`**
- Свойства: `Id`, `Name`, `CountryId`, `ManagerId`, `Price`, `Days`
- Вычисляемое свойство `PricePerDay` — `Price / Days`
- Вычисляемое свойство `IsLong` — `true`, если `Days > 10`
- Метод `GetInfo()` — `"Пляжный отдых (7 дней, 50000 руб.)"`

**Репозитории:** `GetCountries()`, `GetManagers()`, `GetTours()`

**Методы программы:**
- `FindManager(tours, managers, name)` — менеджер тура.
- `FindCountry(countries, tour)` — страна тура.
- `GetTotalDays(tours)` — суммарное число дней.
- `GetMostPopularCountry(tours, countries)` — страна с макс. туров.
- `PrintAllTours(tours, managers, countries)` — вывод туров.

**Вывод:**
```
Количество туров: 7, менеджеров: 3
"Пляжный отдых" ведёт Иванова А.А., страна "Турция"
Общее количество дней: 56
Самая популярная страна: Турция (3 тура)
```

---

## Вариант 22. Кофейня

**ERD:**
```
Shift (1) ────< Drink >──── (1) Barista
  Id            Id             Id
  Time          Name           FullName
  Date          ShiftId        DrinkId
                BaristaId      Experience
                Price          Rating
                Volume
```

**Классы, свойства и методы:**

**`Shift`**
- Свойства: `Id`, `Time`, `Date`
- Вычисляемое свойство `IsMorning` — `true`, если `Time == "Утренняя"`
- Метод `GetInfo()` — `"Утренняя (01.09.2025)"`

**`Barista`**
- Свойства: `Id`, `FullName`, `DrinkId`, `Experience`, `Rating`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 2`
- Метод `GetInfo()` — `"Петров П.П. (3 года, рейтинг 4.8)"`

**`Drink`**
- Свойства: `Id`, `Name`, `ShiftId`, `BaristaId`, `Price`, `Volume`
- Вычисляемое свойство `PricePerMl` — `Price / Volume`
- Вычисляемое свойство `IsHot` — `true`, если `Name` содержит "Кофе" или "Чай"
- Метод `GetInfo()` — `"Латте (250 руб., 300 мл)"`

**Репозитории:** `GetShifts()`, `GetBaristas()`, `GetDrinks()`

**Методы программы:**
- `FindBarista(drinks, baristas, name)` — бариста напитка.
- `FindShift(shifts, drink)` — смена напитка.
- `GetTotalVolume(drinks)` — общий объём.
- `GetBaristaRating(baristas, drinks)` — средний рейтинг по бариста.
- `PrintAllDrinks(drinks, baristas, shifts)` — вывод напитков.

**Вывод:**
```
Количество напитков: 10, бариста: 4
"Латте" готовит Петров П.П., смена "Утренняя"
Общий объём: 3500 мл
Рейтинг бариста: Петров — 4.8, Иванов — 4.5
```

---

## Вариант 23. Фотостудия

**ERD:**
```
Photographer (1) ────< PhotoSession >──── (1) Client
  Id                   Id                   Id
  FullName             Name                 FullName
  Experience           PhotographerId       SessionId
                       ClientId             Phone
                       Date                 Email
                       Price
                       Duration
```

**Классы, свойства и методы:**

**`Photographer`**
- Свойства: `Id`, `FullName`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 5`
- Метод `GetInfo()` — `"Сидоров С.С. (8 лет опыта)"`

**`Client`**
- Свойства: `Id`, `FullName`, `SessionId`, `Phone`, `Email`
- Метод `HasEmail()` — `true`, если `Email` не пустой
- Метод `GetInfo()` — `"Иванова А.А. (ivanova@mail.ru)"`

**`PhotoSession`**
- Свойства: `Id`, `Name`, `PhotographerId`, `ClientId`, `Date`, `Price`, `Duration`
- Вычисляемое свойство `PricePerHour` — `Price / Duration`
- Вычисляемое свойство `IsLong` — `true`, если `Duration > 3`
- Метод `GetInfo()` — `"Свадебная (3 ч, 15000 руб.)"`

**Репозитории:** `GetPhotographers()`, `GetClients()`, `GetSessions()`

**Методы программы:**
- `FindPhotographer(sessions, photographers, name)` — фотограф сессии.
- `FindClient(clients, session)` — клиент сессии.
- `GetTotalDuration(sessions)` — общая длительность.
- `GetPhotographerWithMaxRevenue(sessions, photographers)` — фотограф с макс. выручкой.
- `PrintAllSessions(sessions, photographers, clients)` — вывод сессий.

**Вывод:**
```
Количество фотосессий: 6, фотографов: 3
"Свадебная" проводит Сидоров С.С., клиент Иванова А.А.
Общая длительность: 12 часов
Макс. выручка: Сидоров С.С. (45000 руб.)
```

---

## Вариант 24. Автосалон

**ERD:**
```
Brand (1) ────< Car >──── (1) Manager
  Id            Id           Id
  Name          Model        FullName
  Country       BrandId      CarId
                ManagerId    Phone
                Price        Experience
                Year
```

**Классы, свойства и методы:**

**`Brand`**
- Свойства: `Id`, `Name`, `Country`
- Вычисляемое свойство `IsForeign` — `true`, если `Country != "Россия"`
- Метод `GetInfo()` — `"Toyota (Япония)"`

**`Manager`**
- Свойства: `Id`, `FullName`, `CarId`, `Phone`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 3`
- Метод `GetInfo()` — `"Петров П.П. (5 лет опыта)"`

**`Car`**
- Свойства: `Id`, `Model`, `BrandId`, `ManagerId`, `Price`, `Year`
- Вычисляемое свойство `IsNew` — `true`, если `Year >= 2023`
- Метод `GetDepreciation(int years)` — остаточная стоимость через N лет
- Метод `GetInfo()` — `"Camry (2023, 3000000 руб.)"`

**Репозитории:** `GetBrands()`, `GetManagers()`, `GetCars()`

**Методы программы:**
- `FindManager(cars, managers, model)` — менеджер автомобиля.
- `FindBrand(brands, car)` — марка автомобиля.
- `GetTotalPrice(cars)` — общая стоимость.
- `GetCarsByBrandSortedByPrice(cars, brands, brandName)` — машины марки по цене.
- `PrintAllCars(cars, managers, brands)` — вывод автомобилей.

**Вывод:**
```
Количество автомобилей: 8, менеджеров: 3
"Camry" продаёт Петров П.П., марка "Toyota"
Общая стоимость: 24000000 руб.
Toyota по цене: Camry (3000000), Corolla (2000000)
```

---

## Вариант 25. Аптека

**ERD:**
```
Category (1) ────< Medicine >──── (1) Pharmacist
  Id               Id               Id
  Name             Name             FullName
  Description      CategoryId       MedicineId
                   PharmacistId     Shift
                   Price            Experience
                   Quantity
```

**Классы, свойства и методы:**

**`Category`**
- Свойства: `Id`, `Name`, `Description`
- Вычисляемое свойство `Info` — `"Обезболивающие — от боли"`

**`Pharmacist`**
- Свойства: `Id`, `FullName`, `MedicineId`, `Shift`, `Experience`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 3`
- Метод `GetInfo()` — `"Иванова А.А. (5 лет опыта)"`

**`Medicine`**
- Свойства: `Id`, `Name`, `CategoryId`, `PharmacistId`, `Price`, `Quantity`
- Вычисляемое свойство `TotalValue` — `Price * Quantity`
- Метод `IsLowStock(int threshold)` — `true`, если `Quantity < threshold`
- Метод `GetInfo()` — `"Аспирин (50 руб., 100 уп.)"`

**Репозитории:** `GetCategories()`, `GetPharmacists()`, `GetMedicines()`

**Методы программы:**
- `FindPharmacist(medicines, pharmacists, name)` — фармацевт лекарства.
- `FindCategory(categories, medicine)` — категория лекарства.
- `GetTotalQuantity(medicines)` — общее количество.
- `GetLowStockMedicines(medicines, threshold)` — лекарства ниже порога.
- `PrintAllMedicines(medicines, pharmacists, categories)` — вывод лекарств.

**Вывод:**
```
Количество лекарств: 15, фармацевтов: 4
"Аспирин" отпускает Иванова А.А., категория "Обезболивающие"
Общее количество: 500 упаковок
Лекарства ниже порога 20: Аспирин (10), Анальгин (15)
```

---

## Вариант 26. Киностудия

**ERD:**
```
Studio (1) ────< Film >──── (1) Director
  Id             Id            Id
  Name           Title         FullName
  Country        StudioId      FilmId
                 DirectorId    Experience
                 Year          Awards
                 Budget
```

**Классы, свойства и методы:**

**`Studio`**
- Свойства: `Id`, `Name`, `Country`
- Вычисляемое свойство `IsForeign` — `true`, если `Country != "Россия"`
- Метод `GetInfo()` — `"Warner Bros (США)"`

**`Director`**
- Свойства: `Id`, `FullName`, `FilmId`, `Experience`, `Awards`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 10`
- Метод `GetInfo()` — `"Нолан К. (20 лет, 5 наград)"`

**`Film`**
- Свойства: `Id`, `Title`, `StudioId`, `DirectorId`, `Year`, `Budget`
- Вычисляемое свойство `IsExpensive` — `true`, если `Budget > 100000000`
- Метод `GetInfo()` — `"Начало (2010, 160000000 руб.)"`

**Репозитории:** `GetStudios()`, `GetDirectors()`, `GetFilms()`

**Методы программы:**
- `FindDirector(films, directors, title)` — режиссёр фильма.
- `FindStudio(studios, film)` — студия фильма.
- `GetTotalBudget(films)` — общий бюджет.
- `GetDirectorWithMaxBudget(films, directors)` — режиссёр с макс. бюджетом.
- `PrintAllFilms(films, directors, studios)` — вывод фильмов.

**Вывод:**
```
Количество фильмов: 6, режиссёров: 3
"Начало" снял Нолан К., студия "Warner Bros"
Общий бюджет: 500000000 руб.
Режиссёр с макс. бюджетом: Нолан К. (300000000)
```

---

## Вариант 27. Вокзал

**ERD:**
```
Direction (1) ────< Train >──── (1) Driver
  Id                Id            Id
  Name              Number        FullName
  Distance          DirectionId   TrainId
                    DriverId      Experience
                    Capacity      License
                    Type
```

**Классы, свойства и методы:**

**`Direction`**
- Свойства: `Id`, `Name`, `Distance`
- Вычисляемое свойство `IsLong` — `true`, если `Distance > 500`
- Метод `GetInfo()` — `"Москва-Питер (650 км)"`

**`Driver`**
- Свойства: `Id`, `FullName`, `TrainId`, `Experience`, `License`
- Вычисляемое свойство `IsExperienced` — `true`, если `Experience > 10`
- Метод `GetInfo()` — `"Иванов И.И. (15 лет стажа)"`

**`Train`**
- Свойства: `Id`, `Number`, `DirectionId`, `DriverId`, `Capacity`, `Type`
- Вычисляемое свойство `IsFast` — `true`, если `Type == "Скоростной"`
- Метод `GetInfo()` — `"№123 (Скоростной, 500 мест)"`

**Репозитории:** `GetDirections()`, `GetDrivers()`, `GetTrains()`

**Методы программы:**
- `FindDriver(trains, drivers, number)` — машинист поезда.
- `FindDirection(directions, train)` — направление поезда.
- `GetTotalCapacity(trains)` — суммарная вместимость.
- `GetDirectionsByTrainCount(trains, directions)` — число поездов по направлениям.
- `PrintAllTrains(trains, drivers, directions)` — вывод поездов.

**Вывод:**
```
Количество поездов: 10, машинистов: 5
Поезд №123 — машинист Иванов И.И., направление "Москва-Питер"
Общая вместимость: 5000 пассажиров
Поездов по направлениям: Москва-Питер — 3, Москва-Казань — 2
```

---

## Вариант 28. Галерея

**ERD:**
```
Exhibition (1) ────< Painting >──── (1) Artist
  Id                 Id               Id
  Name               Title            FullName
  Date               ExhibitionId     PaintingId
                     ArtistId         Country
                     Year             Style
                     Price
```

**Классы, свойства и методы:**

**`Exhibition`**
- Свойства: `Id`, `Name`, `Date`
- Вычисляемое свойство `Info` — `"Постимпрессионизм (01.09.2025)"`

**`Artist`**
- Свойства: `Id`, `FullName`, `PaintingId`, `Country`, `Style`
- Вычисляемое свойство `IsForeign` — `true`, если `Country != "Россия"`
- Метод `GetInfo()` — `"Ван Гог (Нидерланды, постимпрессионизм)"`

**`Painting`**
- Свойства: `Id`, `Title`, `ExhibitionId`, `ArtistId`, `Year`, `Price`
- Вычисляемое свойство `IsValuable` — `true`, если `Price > 1000000`
- Метод `GetInfo()` — `"Подсолнухи (1888, 5000000 руб.)"`

**Репозитории:** `GetExhibitions()`, `GetArtists()`, `GetPaintings()`

**Методы программы:**
- `FindArtist(paintings, artists, title)` — художник картины.
- `FindExhibition(exhibitions, painting)` — выставка картины.
- `GetTotalPrice(paintings)` — общая стоимость.
- `GetArtistWithMostPaintings(paintings, artists)` — художник с макс. картин.
- `PrintAllPaintings(paintings, artists, exhibitions)` — вывод картин.

**Вывод:**
```
Количество картин: 12, художников: 5
"Подсолнухи" написал Ван Гог, выставка "Постимпрессионизм"
Общая стоимость: 15000000 руб.
Художник с макс. картин: Ван Гог (3)
```

---

## Вариант 29. Стадион

**ERD:**
```
Coach (1) ────< Team >──── (1) Match
  Id            Id            Id
  FullName      Name          TeamId
  TeamId        CoachId       Opponent
                City          Date
                Budget        Score
                              Stadium
```

**Классы, свойства и методы:**

**`Coach`**
- Свойства: `Id`, `FullName`, `TeamId`
- Метод `GetInfo()` — `"Иванов И.И."`

**`Team`**
- Свойства: `Id`, `Name`, `CoachId`, `City`, `Budget`
- Вычисляемое свойство `IsRich` — `true`, если `Budget > 100000000`
- Метод `GetInfo()` — `"Спартак (Москва, 500000000 руб.)"`

**`Match`**
- Свойства: `Id`, `TeamId`, `Opponent`, `Date`, `Score`, `Stadium`
- Метод `GetGoals()` — сумма голов из строки `Score` (парсинг через `Split(':')`)
- Метод `GetInfo()` — `"Спартак — Зенит (2:1, 01.09.2025)"`

**Репозитории:** `GetCoaches()`, `GetTeams()`, `GetMatches()`

**Методы программы:**
- `FindCoach(teams, coaches, name)` — тренер команды.
- `FindTeam(teams, match)` — команда матча.
- `GetTotalGoals(matches)` — суммарное число голов.
- `GetTeamStats(matches, teams)` — очки команд (победа +3, ничья +1).
- `PrintAllMatches(matches, teams, coaches)` — вывод матчей.

**Вывод:**
```
Количество матчей: 8, команд: 4
Команда "Спартак" — тренер Иванов И.И., матч против "Зенит" (2:1)
Общее количество голов: 24
Турнирная таблица: Спартак — 15, Зенит — 12, ЦСКА — 9
```

---

## Вопросы для подготовки к сдаче

### Теория ООП

1. Что такое класс и объект? В чём разница?
2. Что такое автосвойство? Чем оно отличается от обычного свойства с полем?
3. Что такое вычисляемое свойство? Когда его использовать вместо метода?
4. В чём разница между свойством и методом? Когда что выбирать?
5. Что такое конструктор? Какие виды конструкторов бывают?
6. Что такое инкапсуляция? Как она реализуется в C#?
7. Что такое внешний ключ в контексте классов? Как организовать связь между классами?
8. Что такое `null`? Как правильно обрабатывать `null` при поиске?

### Коллекции

9. Чем отличается `List<T>` от массива `T[]`? Когда что использовать?
10. Что такое `Dictionary<TKey, TValue>`? Когда его применять?
11. Как добавить элемент в `List<T>`? Как проверить наличие ключа в `Dictionary`?
12. Как перебрать элементы `Dictionary` без LINQ?
13. Как отсортировать `List<T>` без LINQ? Опишите алгоритм пузырьковой сортировки.

### Работа с CSV

14. Как прочитать файл в C#? Какой метод использовать?
15. Как разбить строку CSV на части? Какой метод использовать?
16. Как преобразовать строку в число? Какие методы использовать?
17. Как обработать ситуацию, когда файл пустой или содержит меньше строк, чем ожидается?
18. Почему важно проверять `parts.Length` перед доступом к элементам массива?

### Архитектура программы

19. Зачем разделять `InMemoryRepository` и `CsvRepository`? Что это даёт?
20. Почему каждый класс должен быть в отдельном файле?
21. Как выбрать источник данных через `switch`? Опишите структуру.
22. Что такое разделение ответственности (Single Responsibility)? Как оно применяется в задании?

### Методы поиска и аналитики

23. Как найти связанный объект по внешнему ключу без LINQ?
24. Как найти максимальное значение в `Dictionary` без LINQ?
25. Как сгруппировать объекты по какому-либо признаку без LINQ?
26. Как подсчитать количество объектов с одинаковым значением поля?
27. Как реализовать поиск по вводу пользователя через `Console.ReadLine()`?

### Именование и оформление

28. Как правильно именовать классы, свойства и методы в C#?
29. Как правильно именовать приватные поля?
30. Зачем нужны XML-комментарии (`/// <summary>`)?

### Практические вопросы

31. Что произойдёт, если внешний ключ ссылается на несуществующую запись?
32. Как проверить, что поиск не нашёл результат? Что вернуть в этом случае?
33. Как вывести данные в формате `"Книга "X" написана Y, издательство Z"`?
34. Как избежать деления на ноль при вычислении среднего?
35. Как протестировать программу с обоими источниками данных и убедиться, что результаты совпадают?

---
