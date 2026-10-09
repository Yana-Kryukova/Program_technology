# ДОМАШНЕЕ ЗАДАНИЕ: Наследование, интерфейсы, полиморфизм

**Дисциплина:** Технологии программирования (язык C#)

**Тема:** Наследование, абстрактные классы, интерфейсы, полиморфизм, перечисления, перегрузка конструкторов, свойства

**Форма сдачи:** проект C# (Console Application) + ссылка на репозиторий с историей коммитов

**Срок сдачи:** согласно расписанию

---

## Чему научится студент

1. **Проектировать иерархию наследования** от абстрактного базового класса.
2. **Реализовывать интерфейсы** только там, где это семантически оправдано.
3. **Использовать приватные поля** для инкапсуляции.
4. **Проектировать автосвойства и вычисляемые свойства** (get-only, без backing-поля).
5. **Перегружать конструкторы** — предоставлять удобные варианты создания объектов.
6. **Применять перечисления (`enum`)** для типизированных состояний/категорий и **задавать значение по умолчанию** для свойства типа перечисления.
7. **Переопределять** абстрактные методы и `ToString()` в наследниках.
8. **Использовать полиморфизм** — хранить объекты наследников в коллекции базового типа.
9. **Проверять тип во время выполнения** через `is` и pattern matching.
10. **Работать с Git**: ветки, коммиты по подзадачам, осмысленные сообщения.

---

## Соглашения по оформлению кода

### Именование

| Элемент | Стиль | Пример |
|---------|-------|--------|
| Класс | PascalCase | `Transport`, `Amphibious` |
| Интерфейс | PascalCase с префиксом `I` | `ISwimmable` |
| Перечисление | PascalCase | `TransportState` |
| Элемент перечисления | PascalCase | `TransportState.OnWater` |
| Свойство | PascalCase | `SpeedInMetersPerSecond` |
| Метод | PascalCase | `Move()`, `Swim()` |
| Приватное поле | `_camelCase` | `_fleet`, `_id` |
| Локальная переменная | camelCase | `currentVehicle` |
| Параметр метода | camelCase | `wheelCount` |

### Файлы

- **Каждый класс, интерфейс и перечисление — в отдельном файле** с именем, совпадающим с именем типа.
- Один файл — один публичный тип.

### Прочее

- **Запрещено:** LINQ (`Where`, `Select`, `First`, `OfType`, `Cast`).
- **Разрешено:** `List<T>`, `foreach`, `if`, `switch`, `is`, `as`, pattern matching.
- **Комментарии:** `/// <summary>` для классов, интерфейсов, перечислений и публичных методов.
- **Язык идентификаторов:** только английский.

---

## Обязательные требования к коду (для каждого варианта)

1. **Приватные поля** — минимум одно поле на класс, закрытое от внешнего доступа.
2. **Автосвойства** — публичные данные класса (например, `Name`, `Speed`) оформлены как автосвойства.
3. **Вычисляемые свойства** — минимум одно get-only свойство без backing-поля в каждом классе.
4. **Перегрузка конструкторов** — минимум два конструктора в базовом классе, вызывающих друг друга через `this(...)`.
5. **Перечисление (`enum`)** — собственный тип для категории/состояния, объявлен в отдельном файле. Значения перечисления указаны в варианте.
6. **Свойство типа перечисления — со значением по умолчанию.** Объявляется как автосвойство с инициализатором:
   ```csharp
   public TransportState State { get; set; } = TransportState.Parked;
   ```
   Значение по умолчанию для каждого варианта указано ниже.
7. **Абстрактный метод** — базовый класс объявляет минимум один метод, который наследники должны переопределить.
8. **`ToString()`** — переопределён во всех наследниках, использует специфичные поля класса.
9. **Интерфейс** — реализуют **только те наследники, для которых это семантически верно**.

---

## Работа с Git (обязательная часть задания)

1. **Ветка `main`** содержит только стартовый шаблон.
2. **Создать новую ветку** от `main`:
3. **Создать глобальную задачу** в трекере (GitHub Projects) с заголовком:

   > **Домашнее задание: Наследование и интерфейсы**

4. Внутри задачи создать **три подзадачи**:

   - **Подзадача 1 — Скелет проекта, перечисление, базовый класс**
   - **Подзадача 2 — Иерархия, интерфейс, вычисляемые свойства, конструкторы**
   - **Подзадача 3 — `Main` и демонстрация полиморфизма**

5. **Первый коммит** делается сразу после создания проекта в ветке.
6. **По завершении каждой подзадачи** — отдельный коммит:
```
Subtask #15: создан проект, перечисление и базовый класс
Subtask #16: реализованы наследники, интерфейс и ToString
Subtask #17: добавлен Main с перебором List<Base> и проверкой интерфейса
```
### ⚠️ Важно: указывайте номер подзадачи в сообщении коммита

Чтобы коммит **автоматически отобразился внутри соответствующей подзадачи** в трекере, в сообщении коммита обязательно указывайте **номер подзадачи из проекта**.

| Трекер | Формат ссылки в коммите | Пример |
|--------|-------------------------|--------|
| GitHub Issues | `#<номер>` | `Subtask #12: создан проект, перечисление и базовый класс` |

> **Без номера подзадачи** коммит не привяжется к карточке в трекере, и проверяющий не увидит прогресс внутри задачи. Считается ошибкой оформления.
---

## Шаблон оформления варианта

Каждый вариант содержит:

1. **Перечисление** — имя, список значений, значение по умолчанию для свойства.
2. **Интерфейс** — 1 метод.
3. **Базовый абстрактный класс** — приватное поле, автосвойства, свойство-перечисление со значением по умолчанию, вычисляемое свойство, два конструктора, абстрактный метод.
4. **Три класса-наследника** — собственные автосвойства, вычисляемые свойства, конструкторы, переопределённые `Move()` и `ToString()`.
5. **Пример вывода**.

---

## Вариант 1. Транспортный парк

**Перечисление `TransportState`:**
- Значения: `Parked`, `Moving`, `OnWater`
- **Значение по умолчанию:** `TransportState.Parked`

**Интерфейс:** `ISwimmable { void Swim(); }`

**Базовый класс `Transport`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Speed`
- **Свойство-перечисление:** `public TransportState State { get; set; } = TransportState.Parked;`
- Вычисляемое: `SpeedInMetersPerSecond => Speed / 3.6`
- Конструкторы: `Transport(int id, string name, double speed)`, `Transport(string name) : this(0, name, 0)`
- Абстрактный метод `Move()`

**Наследники:**
- `Car` — автосвойство `WheelCount`, вычисляемое `IsFourWheeler`, `ToString` вида `"[Car] Lada, 120 km/h, wheels: 4"`
- `Boat : Transport, ISwimmable` — автосвойство `HasMotor`, вычисляемое `IsMotorBoat`, `Swim()`
- `Amphibious : Transport, ISwimmable` — автосвойство `Mode`, вычисляемое `IsOnWater`, `Swim()`

**Пример вывода:**
```
[Car] Lada, 120 km/h, wheels: 4
  state: Parked, speed in m/s: 33.33
Lada drives on the road on 4 wheels.
Lada cannot swim.
----------------------------------------
[Boat] Progress, 40 km/h, motor: yes
  state: Parked, speed in m/s: 11.11
Progress sails on water.
Progress stays on water and swims.
----------------------------------------
```

---

## Вариант 2. Зоопарк

**Перечисление `AnimalClass`:**
- Значения: `Mammal`, `Bird`, `Reptile`
- **Значение по умолчанию:** `AnimalClass.Mammal`

**Интерфейс:** `IFlyable { void Fly(); }`

**Базовый класс `Animal`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Age`
- **Свойство-перечисление:** `public AnimalClass Class { get; set; } = AnimalClass.Mammal;`
- Вычисляемое: `AgeInMonths => Age * 12`
- Конструкторы: `Animal(int id, string name, int age)`, `Animal(string name) : this(0, name, 0)`
- Абстрактный метод `MakeSound()`

**Наследники:**
- `Mammal` — автосвойство `FurColor`, вычисляемое `IsAdult`
- `Bird : Animal, IFlyable` — автосвойство `WingSpan`, вычисляемое `IsBigBird`, `Fly()`
- `Reptile` — автосвойство `IsVenomous`, вычисляемое `IsDangerous`

**Пример вывода:**
```
[Mammal] Barsik, 3 years, class: Mammal
  age in months: 36
Barsik says: meow.
Barsik cannot fly.
----------------------------------------
[Bird] Kesha, 2 years, class: Bird, wingspan: 120
  age in months: 24
Kesha says: chirp.
Kesha flaps wings and flies.
----------------------------------------
```

---

## Вариант 3. Сотрудники компании

**Перечисление `EmployeeLevel`:**
- Значения: `Junior`, `Middle`, `Senior`
- **Значение по умолчанию:** `EmployeeLevel.Junior`

**Интерфейс:** `IManageable { void Manage(); }`

**Базовый класс `Employee`:**
- Приватное поле: `_id`
- Автосвойства: `FullName`, `Salary`
- **Свойство-перечисление:** `public EmployeeLevel Level { get; set; } = EmployeeLevel.Junior;`
- Вычисляемое: `AnnualSalary => Salary * 12`
- Конструкторы: `Employee(int id, string fullName, decimal salary)`, `Employee(string fullName) : this(0, fullName, 0)`
- Абстрактный метод `Work()`

**Наследники:**
- `Developer` — автосвойство `PrimaryLanguage`, вычисляемое `IsSenior`
- `Manager : Employee, IManageable` — автосвойство `TeamSize`, вычисляемое `IsBigTeam`, `Manage()`
- `Intern` — автосвойство `University`, вычисляемое `IsPaid`

**Пример вывода:**
```
[Developer] Ivanov, 150000, level: Junior
  annual: 1800000
Ivanov writes code.
Ivanov cannot manage.
----------------------------------------
[Manager] Petrov, 200000, level: Senior, team: 8
  annual: 2400000
Petrov works on tasks.
Petrov manages the team of 8.
----------------------------------------
```

---

## Вариант 4. Банковские счета

**Перечисление `AccountType`:**
- Значения: `Debit`, `Credit`, `Deposit`
- **Значение по умолчанию:** `AccountType.Debit`

**Интерфейс:** `IWithdrawable { void Withdraw(decimal amount); }`

**Базовый класс `Account`:**
- Приватное поле: `_number`
- Автосвойства: `Owner`, `Balance`
- **Свойство-перечисление:** `public AccountType Type { get; set; } = AccountType.Debit;`
- Вычисляемое: `IsVip => Balance > 1_000_000`
- Конструкторы: `Account(string number, string owner, decimal balance)`, `Account(string owner) : this("N/A", owner, 0)`
- Абстрактный метод `DisplayInfo()`

**Наследники:**
- `DebitAccount : Account, IWithdrawable` — автосвойство `DailyLimit`, вычисляемое `CanWithdrawMore`, `Withdraw()`
- `CreditAccount` — автосвойство `CreditLimit`, вычисляемое `AvailableCredit`
- `DepositAccount` — автосвойство `InterestRate`, вычисляемое `ProjectedAnnualYield`

**Пример вывода:**
```
[DebitAccount] 40817810001, Ivanov, 250000, type: Debit
  vip: False
40817810001 (Debit) displayed.
40817810001 withdrawal of 1000 processed.
----------------------------------------
[CreditAccount] 40817810002, Petrov, -50000, type: Credit, limit: 200000
  vip: False
40817810002 (Credit) displayed.
40817810002 cannot withdraw.
----------------------------------------
```

---

## Вариант 5. Геометрические фигуры

**Перечисление `ShapeKind`:**
- Значения: `Circle`, `Rectangle`, `Triangle`
- **Значение по умолчанию:** `ShapeKind.Circle`

**Интерфейс:** `IScalable { void Scale(double factor); }`

**Базовый класс `Shape`:**
- Приватное поле: `_name`
- Автосвойство: `Color`
- **Свойство-перечисление:** `public ShapeKind Kind { get; set; } = ShapeKind.Circle;`
- Вычисляемое: `Info => $"{Kind} ({Color})"`
- Конструкторы: `Shape(string name, string color)`, `Shape(string name) : this(name, "black")`
- Абстрактный метод `double GetArea()`

**Наследники:**
- `Circle : Shape, IScalable` — автосвойство `Radius`, вычисляемое `Diameter`, `Scale()`
- `Rectangle` — автосвойства `Width`, `Height`, вычисляемое `IsSquare`
- `Triangle` — автосвойства `Base`, `Height`, вычисляемое `IsRight`

**Пример вывода:**
```
[Circle] red, kind: Circle, r=5
  diameter: 10, area: 78.54
Circle scaled by 2.
----------------------------------------
[Rectangle] blue, kind: Rectangle, 4x6
  is square: False, area: 24
Rectangle cannot scale.
----------------------------------------
```

---

## Вариант 6. Оружие в игре

**Перечисление `WeaponType`:**
- Значения: `Melee`, `Ranged`, `Magic`
- **Значение по умолчанию:** `WeaponType.Melee`

**Интерфейс:** `IReloadable { void Reload(); }`

**Базовый класс `Weapon`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Damage`, `AttackSpeed`
- **Свойство-перечисление:** `public WeaponType Type { get; set; } = WeaponType.Melee;`
- Вычисляемое: `DamagePerSecond => Damage * AttackSpeed`
- Конструкторы: `Weapon(int id, string name, int damage)`, `Weapon(string name) : this(0, name, 0)`
- Абстрактный метод `Attack()`

**Наследники:**
- `Sword` — автосвойство `BladeLength`, вычисляемое `IsTwoHanded`
- `Bow : Weapon, IReloadable` — автосвойство `ArrowCount`, вычисляемое `IsEmpty`, `Reload()`
- `Staff` — автосвойство `ManaCost`, вычисляемое `IsPowerful`

**Пример вывода:**
```
[Sword] Excalibur, 50 dmg, type: Melee
  dps: 100, two-handed: True
Excalibur swings and strikes.
Excalibur cannot reload.
----------------------------------------
[Bow] Longbow, 30 dmg, type: Ranged, arrows: 0
  dps: 45, empty: True
Longbow fires an arrow.
Longbow reloads arrows.
----------------------------------------
```

---

## Вариант 7. Мебель

**Перечисление `Material`:**
- Значения: `Wood`, `Metal`, `Plastic`
- **Значение по умолчанию:** `Material.Wood`

**Интерфейс:** `IAssemblable { void Assemble(); }`

**Базовый класс `Furniture`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public Material Material { get; set; } = Material.Wood;`
- Вычисляемое: `PriceWithVat => Price * 1.2m`
- Конструкторы: `Furniture(int id, string name, decimal price)`, `Furniture(string name) : this(0, name, 0)`
- Абстрактный метод `Describe()`

**Наследники:**
- `Chair` — автосвойство `LegsCount`, вычисляемое `IsStool`
- `Table : Furniture, IAssemblable` — автосвойство `SeatsCount`, вычисляемое `IsBigTable`, `Assemble()`
- `Wardrobe` — автосвойство `ShelvesCount`, вычисляемое `IsSpacious`

**Пример вывода:**
```
[Chair] Stool, 1500, material: Wood, legs: 3
  price with VAT: 1800
Chair is a stool.
Chair cannot be assembled.
----------------------------------------
[Table] Dining, 8000, material: Wood, seats: 6
  price with VAT: 9600
Table seats 6 people.
Table assembled successfully.
----------------------------------------
```

---

## Вариант 8. Продукты

**Перечисление `StorageType`:**
- Значения: `Fridge`, `Freezer`, `Room`
- **Значение по умолчанию:** `StorageType.Room`

**Интерфейс:** `IExpirable { bool IsExpired(); }`

**Базовый класс `Food`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Calories`
- **Свойство-перечисление:** `public StorageType Storage { get; set; } = StorageType.Room;`
- Вычисляемое: `CaloriesPer100g => Calories / 100.0`
- Конструкторы: `Food(int id, string name, int calories)`, `Food(string name) : this(0, name, 0)`
- Абстрактный метод `Consume()`

**Наследники:**
- `Vegetable` — автосвойство `IsOrganic`, вычисляемое `IsLowCalorie`
- `Dairy : Food, IExpirable` — автосвойство `ExpirationDate`, вычисляемое `DaysLeft`, `IsExpired()`
- `Meat` — автосвойство `IsFrozen`, вычисляемое `IsSafeToEat`

**Пример вывода:**
```
[Vegetable] Cucumber, 15 cal, storage: Room
  cal/100g: 0.15
Cucumber consumed.
Cucumber is not expirable.
----------------------------------------
[Dairy] Milk, 60 cal, storage: Fridge
  cal/100g: 0.6
Milk consumed.
Milk expired: True
----------------------------------------
```

---

## Вариант 9. Устройства

**Перечисление `PowerSource`:**
- Значения: `Battery`, `Mains`, `Solar`
- **Значение по умолчанию:** `PowerSource.Mains`

**Интерфейс:** `IChargeable { void Charge(); }`

**Базовый класс `Device`:**
- Приватное поле: `_serial`
- Автосвойства: `Model`, `Power`
- **Свойство-перечисление:** `public PowerSource Source { get; set; } = PowerSource.Mains;`
- Вычисляемое: `PowerInWatts => Power * 1000`
- Конструкторы: `Device(string serial, string model, double power)`, `Device(string model) : this("N/A", model, 0)`
- Абстрактный метод `TurnOn()`

**Наследники:**
- `Laptop : Device, IChargeable` — автосвойство `BatteryCapacity`, вычисляемое `IsLongLasting`, `Charge()`
- `Phone : Device, IChargeable` — автосвойство `ScreenSize`, вычисляемое `IsPhablet`, `Charge()`
- `Desktop` — автосвойство `TowerSize`, вычисляемое `IsGaming`

**Пример вывода:**
```
[Laptop] ThinkPad, 65W, source: Mains
  watts: 65000
ThinkPad turns on.
ThinkPad charging...
----------------------------------------
[Desktop] Gaming PC, 500W, source: Mains
  watts: 500000
Gaming PC turns on.
Gaming PC cannot be charged.
----------------------------------------
```

---

## Вариант 10. Музыкальные инструменты

**Перечисление `InstrumentFamily`:**
- Значения: `Strings`, `Winds`, `Percussion`
- **Значение по умолчанию:** `InstrumentFamily.Strings`

**Интерфейс:** `ITunable { void Tune(); }`

**Базовый класс `Instrument`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public InstrumentFamily Family { get; set; } = InstrumentFamily.Strings;`
- Вычисляемое: `PriceInUsd => Price / 90.0`
- Конструкторы: `Instrument(int id, string name, decimal price)`, `Instrument(string name) : this(0, name, 0)`
- Абстрактный метод `Play()`

**Наследники:**
- `Guitar : Instrument, ITunable` — автосвойство `StringsCount`, вычисляемое `IsBass`, `Tune()`
- `Piano` — автосвойство `KeysCount`, вычисляемое `IsGrand`
- `Drum` — автосвойство `Diameter`, вычисляемое `IsBigDrum`

**Пример вывода:**
```
[Guitar] Fender, 80000, family: Strings
  price in USD: 888.89
Fender plays a chord.
Fender tuned.
----------------------------------------
[Piano] Yamaha, 500000, family: Percussion
  price in USD: 5555.56
Yamaha plays a melody.
Yamaha cannot be tuned.
----------------------------------------
```

---

## Вариант 11. Космические объекты

**Перечисление `ObjectClass`:**
- Значения: `Planet`, `Star`, `Asteroid`
- **Значение по умолчанию:** `ObjectClass.Asteroid`

**Интерфейс:** `IScannable { void Scan(); }`

**Базовый класс `SpaceObject`:**
- Приватное поле: `_catalogId`
- Автосвойства: `Name`, `Mass`
- **Свойство-перечисление:** `public ObjectClass Class { get; set; } = ObjectClass.Asteroid;`
- Вычисляемое: `MassInMillions => Mass / 1_000_000`
- Конструкторы: `SpaceObject(string catalogId, string name, double mass)`, `SpaceObject(string name) : this("N/A", name, 0)`
- Абстрактный метод `Describe()`

**Наследники:**
- `Planet` — автосвойство `HasRings`, вычисляемое `IsGasGiant`
- `Star : SpaceObject, IScannable` — автосвойство `Temperature`, вычисляемое `IsHot`, `Scan()`
- `Asteroid : SpaceObject, IScannable` — автосвойство `Diameter`, вычисляемое `IsDangerous`, `Scan()`

**Пример вывода:**
```
[Planet] Earth, mass: 5.97e24, class: Asteroid
  mass in millions: 5.97e18
Earth is a planet.
Earth cannot be scanned.
----------------------------------------
[Star] Sun, temp: 5500, class: Star
  mass in millions: 1.99e24
Sun shines.
Sun scanned: temperature 5500K.
----------------------------------------
```

---

## Вариант 12. Одежда

**Перечисление `Season`:**
- Значения: `Winter`, `Summer`, `AllSeason`
- **Значение по умолчанию:** `Season.AllSeason`

**Интерфейс:** `IWashable { void Wash(); }`

**Базовый класс `Clothing`:**
- Приватное поле: `_sku`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public Season Season { get; set; } = Season.AllSeason;`
- Вычисляемое: `PriceWithDiscount => Price * 0.9m`
- Конструкторы: `Clothing(string sku, string name, decimal price)`, `Clothing(string name) : this("N/A", name, 0)`
- Абстрактный метод `Wear()`

**Наследники:**
- `Jacket : Clothing, IWashable` — автосвойство `Insulation`, вычисляемое `IsWarm`, `Wash()`
- `TShirt` — автосвойство `Size`, вычисляемое `IsBigSize`
- `Shoes` — автосвойство `SoleThickness`, вычисляемое `IsSporty`

**Пример вывода:**
```
[Jacket] North Face, 25000, season: AllSeason
  price with discount: 22500
North Face worn.
North Face washed.
----------------------------------------
[TShirt] Adidas, 3000, season: AllSeason
  price with discount: 2700
Adidas worn.
Adidas cannot be washed.
----------------------------------------
```

---

## Вариант 13. Напитки в кофейне

**Перечисление `DrinkTemperature`:**
- Значения: `Hot`, `Cold`, `Room`
- **Значение по умолчанию:** `DrinkTemperature.Hot`

**Интерфейс:** `IWarmable { void Warm(); }`

**Базовый класс `Beverage`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Volume`
- **Свойство-перечисление:** `public DrinkTemperature Temperature { get; set; } = DrinkTemperature.Hot;`
- Вычисляемое: `VolumeInLiters => Volume / 1000.0`
- Конструкторы: `Beverage(int id, string name, int volume)`, `Beverage(string name) : this(0, name, 0)`
- Абстрактный метод `Serve()`

**Наследники:**
- `Coffee : Beverage, IWarmable` — автосвойство `HasMilk`, вычисляемое `IsLatte`, `Warm()`
- `Tea` — автосвойство `TeaType`, вычисляемое `IsGreen`
- `Juice` — автосвойство `IsFresh`, вычисляемое `IsExpensive`

**Пример вывода:**
```
[Coffee] Latte, 300 ml, temperature: Hot
  liters: 0.3
Latte served.
Latte warmed.
----------------------------------------
[Juice] Orange, 500 ml, temperature: Hot
  liters: 0.5
Orange served.
Orange cannot be warmed.
----------------------------------------
```

---

## Вариант 14. Роботы

**Перечисление `RobotState`:**
- Значения: `Idle`, `Working`, `Charging`
- **Значение по умолчанию:** `RobotState.Idle`

**Интерфейс:** `IProgrammable { void Program(string task); }`

**Базовый класс `Robot`:**
- Приватное поле: `_serial`
- Автосвойства: `Model`, `BatteryLevel`
- **Свойство-перечисление:** `public RobotState State { get; set; } = RobotState.Idle;`
- Вычисляемое: `NeedsCharge => BatteryLevel < 20`
- Конструкторы: `Robot(string serial, string model, int battery)`, `Robot(string model) : this("N/A", model, 100)`
- Абстрактный метод `PerformTask()`

**Наследники:**
- `IndustrialRobot : Robot, IProgrammable` — автосвойство `Payload`, вычисляемое `IsHeavyDuty`, `Program()`
- `DomesticRobot` — автосвойство `HasVacuum`, вычисляемое `IsCleaner`
- `CombatRobot` — автосвойство `WeaponType`, вычисляемое `IsArmed`

**Пример вывода:**
```
[IndustrialRobot] KUKA, battery: 80, state: Idle
  needs charge: False
KUKA performs industrial task.
KUKA programmed for welding.
----------------------------------------
[DomesticRobot] Roomba, battery: 15, state: Idle
  needs charge: True
Roomba performs cleaning.
Roomba cannot be programmed.
----------------------------------------
```

---

## Вариант 15. Растения в оранжерее

**Перечисление `PlantType`:**
- Значения: `Flower`, `Tree`, `Cactus`
- **Значение по умолчанию:** `PlantType.Flower`

**Интерфейс:** `IWaterable { void Water(); }`

**Базовый класс `Plant`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Height`
- **Свойство-перечисление:** `public PlantType Type { get; set; } = PlantType.Flower;`
- Вычисляемое: `HeightInMeters => Height / 100.0`
- Конструкторы: `Plant(int id, string name, int height)`, `Plant(string name) : this(0, name, 0)`
- Абстрактный метод `Grow()`

**Наследники:**
- `Flower : Plant, IWaterable` — автосвойство `PetalColor`, вычисляемое `IsBright`, `Water()`
- `Tree : Plant, IWaterable` — автосвойство `TrunkDiameter`, вычисляемое `IsOld`, `Water()`
- `Cactus` — автосвойство `SpineCount`, вычисляемое `IsPrickly`

**Пример вывода:**
```
[Flower] Rose, 50 cm, type: Flower
  height in m: 0.5
Rose grows.
Rose watered.
----------------------------------------
[Cactus] Saguaro, 300 cm, type: Flower
  height in m: 3.0
Saguaro grows.
Saguaro cannot be watered.
----------------------------------------
```

---

## Вариант 16. Птицы

**Перечисление `Habitat`:**
- Значения: `Forest`, `Water`, `Mountain`
- **Значение по умолчанию:** `Habitat.Forest`

**Интерфейс:** `IFlyable { void Fly(); }`

**Базовый класс `Bird`:**
- Приватное поле: `_ringId`
- Автосвойства: `Species`, `Weight`
- **Свойство-перечисление:** `public Habitat Habitat { get; set; } = Habitat.Forest;`
- Вычисляемое: `WeightInGrams => Weight * 1000`
- Конструкторы: `Bird(string ringId, string species, double weight)`, `Bird(string species) : this("N/A", species, 0)`
- Абстрактный метод `Sing()`

**Наследники:**
- `Eagle : Bird, IFlyable` — автосвойство `WingSpan`, вычисляемое `IsPredator`, `Fly()`
- `Penguin` — автосвойство `SwimSpeed`, вычисляемое `IsFastSwimmer`
- `Parrot : Bird, IFlyable` — автосвойство `VocabularySize`, вычисляемое `CanTalk`, `Fly()`

**Пример вывода:**
```
[Eagle] Golden, 5 kg, habitat: Forest
  weight in grams: 5000
Golden sings: screech.
Golden soars high and flies.
----------------------------------------
[Penguin] Emperor, 30 kg, habitat: Forest
  weight in grams: 30000
Emperor sings: squawk.
Emperor cannot fly.
----------------------------------------
```

---

## Вариант 17. Игровые персонажи

**Перечисление `CharacterClass`:**
- Значения: `Warrior`, `Mage`, `Archer`
- **Значение по умолчанию:** `CharacterClass.Warrior`

**Интерфейс:** `IHealable { void Heal(int amount); }`

**Базовый класс `Character`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Health`
- **Свойство-перечисление:** `public CharacterClass Class { get; set; } = CharacterClass.Warrior;`
- Вычисляемое: `IsAlive => Health > 0`
- Конструкторы: `Character(int id, string name, int health)`, `Character(string name) : this(0, name, 100)`
- Абстрактный метод `Attack()`

**Наследники:**
- `Warrior : Character, IHealable` — автосвойство `Armor`, вычисляемое `IsTank`, `Heal()`
- `Mage` — автосвойство `Mana`, вычисляемое `IsPowerful`
- `Archer : Character, IHealable` — автосвойство `ArrowCount`, вычисляемое `IsOutOfArrows`, `Heal()`

**Пример вывода:**
```
[Warrior] Conan, HP=200, class: Warrior
  alive: True
Conan swings sword.
Conan healed by 50.
----------------------------------------
[Mage] Merlin, HP=80, class: Warrior, mana=150
  alive: True
Merlin casts a fireball.
Merlin cannot be healed.
----------------------------------------
```

---

## Вариант 18. Блюда в ресторане

**Перечисление `Course`:**
- Значения: `Starter`, `Main`, `Dessert`
- **Значение по умолчанию:** `Course.Main`

**Интерфейс:** `IServable { void Serve(); }`

**Базовый класс `Dish`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public Course Course { get; set; } = Course.Main;`
- Вычисляемое: `PriceWithService => Price * 1.1m`
- Конструкторы: `Dish(int id, string name, decimal price)`, `Dish(string name) : this(0, name, 0)`
- Абстрактный метод `Cook()`

**Наследники:**
- `Soup : Dish, IServable` — автосвойство `IsVegetarian`, вычисляемое `IsLight`, `Serve()`
- `Steak` — автосвойство `Doneness`, вычисляемое `IsRare`
- `Cake : Dish, IServable` — автосвойство `Layers`, вычисляемое `IsBigCake`, `Serve()`

**Пример вывода:**
```
[Soup] Borscht, 350, course: Main
  price with service: 385
Borscht cooked.
Borscht served.
----------------------------------------
[Steak] Ribeye, 1500, course: Main, doneness: medium
  price with service: 1650
Ribeye cooked.
Ribeye cannot be served.
----------------------------------------
```

---

## Вариант 19. Компьютерные комплектующие

**Перечисление `ComponentType`:**
- Значения: `CPU`, `GPU`, `RAM`
- **Значение по умолчанию:** `ComponentType.CPU`

**Интерфейс:** `IOverclockable { void Overclock(); }`

**Базовый класс `Component`:**
- Приватное поле: `_sku`
- Автосвойства: `Model`, `Price`
- **Свойство-перечисление:** `public ComponentType Type { get; set; } = ComponentType.CPU;`
- Вычисляемое: `PriceWithTax => Price * 1.2m`
- Конструкторы: `Component(string sku, string model, decimal price)`, `Component(string model) : this("N/A", model, 0)`
- Абстрактный метод `Install()`

**Наследники:**
- `CPU : Component, IOverclockable` — автосвойство `Cores`, вычисляемое `IsMulticore`, `Overclock()`
- `GPU : Component, IOverclockable` — автосвойство `Vram`, вычисляемое `IsHighEnd`, `Overclock()`
- `RAM` — автосвойство `Capacity`, вычисляемое `IsBig`

**Пример вывода:**
```
[CPU] Intel i9, 50000, type: CPU
  price with tax: 60000
Intel i9 installed.
Intel i9 overclocked to 5 GHz.
----------------------------------------
[RAM] Kingston 32GB, 12000, type: CPU, capacity: 32
  price with tax: 14400
Kingston 32GB installed.
Kingston 32GB cannot be overclocked.
----------------------------------------
```

---

## Вариант 20. Лабораторное оборудование

**Перечисление `EquipmentStatus`:**
- Значения: `Ready`, `Calibrating`, `Broken`
- **Значение по умолчанию:** `EquipmentStatus.Ready`

**Интерфейс:** `ICalibratable { void Calibrate(); }`

**Базовый класс `LabEquipment`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Accuracy`
- **Свойство-перечисление:** `public EquipmentStatus Status { get; set; } = EquipmentStatus.Ready;`
- Вычисляемое: `IsPrecise => Accuracy < 0.01`
- Конструкторы: `LabEquipment(int id, string name, double accuracy)`, `LabEquipment(string name) : this(0, name, 1.0)`
- Абстрактный метод `Measure()`

**Наследники:**
- `Microscope : LabEquipment, ICalibratable` — автосвойство `Magnification`, вычисляемое `IsHighPower`, `Calibrate()`
- `Centrifuge` — автосвойство `Rpm`, вычисляемое `IsFast`
- `Spectrometer : LabEquipment, ICalibratable` — автосвойство `Wavelength`, вычисляемое `IsUv`, `Calibrate()`

**Пример вывода:**
```
[Microscope] Olympus, accuracy=0.005, status: Ready
  precise: True
Olympus measures sample.
Olympus calibrated.
----------------------------------------
[Centrifuge] Eppendorf, rpm=15000, status: Ready
  precise: False
Eppendorf measures sample.
Eppendorf cannot be calibrated.
----------------------------------------
```

---

## Вариант 21. Строительная техника

**Перечисление `VehicleType`:**
- Значения: `Excavator`, `Crane`, `Bulldozer`
- **Значение по умолчанию:** `VehicleType.Excavator`

**Интерфейс:** `ILiftable { void Lift(int weight); }`

**Базовый класс `ConstructionVehicle`:**
- Приватное поле: `_vin`
- Автосвойства: `Model`, `EnginePower`, `Weight`
- **Свойство-перечисление:** `public VehicleType Type { get; set; } = VehicleType.Excavator;`
- Вычисляемое: `PowerPerTon => EnginePower / Weight`
- Конструкторы: `ConstructionVehicle(string vin, string model, int power, int weight)`, `ConstructionVehicle(string model) : this("N/A", model, 0, 0)`
- Абстрактный метод `Operate()`

**Наследники:**
- `Excavator` — автосвойство `BucketVolume`, вычисляемое `IsBigBucket`
- `Crane : ConstructionVehicle, ILiftable` — автосвойство `MaxLoad`, вычисляемое `IsHeavyLifter`, `Lift()`
- `Bulldozer : ConstructionVehicle, ILiftable` — автосвойство `BladeWidth`, вычисляемое `IsWide`, `Lift()`

**Пример вывода:**
```
[Excavator] CAT 320, power=150, weight=20, type: Excavator
  power per ton: 7.5
CAT 320 digs.
CAT 320 cannot lift.
----------------------------------------
[Crane] Liebherr, max load=50, type: Excavator
  power per ton: 4.0
Liebherr lifts 30 tons.
Liebherr lifted 30 tons.
----------------------------------------
```

---

## Вариант 22. Напитки в баре

**Перечисление `AlcoholLevel`:**
- Значения: `None`, `Low`, `High`
- **Значение по умолчанию:** `AlcoholLevel.None`

**Интерфейс:** `IMixable { void Mix(string ingredient); }`

**Базовый класс `Cocktail`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Volume`
- **Свойство-перечисление:** `public AlcoholLevel Alcohol { get; set; } = AlcoholLevel.None;`
- Вычисляемое: `VolumeInOz => Volume / 30.0`
- Конструкторы: `Cocktail(int id, string name, int volume)`, `Cocktail(string name) : this(0, name, 0)`
- Абстрактный метод `Prepare()`

**Наследники:**
- `Mocktail : Cocktail, IMixable` — автосвойство `MainFlavor`, вычисляемое `IsFruity`, `Mix()`
- `ClassicCocktail` — автосвойство `BaseSpirit`, вычисляемое `IsStrong`
- `SignatureCocktail : Cocktail, IMixable` — автосвойство `ChefName`, вычисляемое `IsAuthorial`, `Mix()`

**Пример вывода:**
```
[Mocktail] Virgin Mojito, 300 ml, alcohol: None
  volume in oz: 10.0
Virgin Mojito prepared.
Virgin Mojito mixed with lime.
----------------------------------------
[ClassicCocktail] Martini, 100 ml, alcohol: None, base: gin
  volume in oz: 3.33
Martini prepared.
Martini cannot be mixed.
----------------------------------------
```

---

## Вариант 23. Космонавты и экипаж

**Перечисление `CrewRole`:**
- Значения: `Commander`, `Pilot`, `Engineer`
- **Значение по умолчанию:** `CrewRole.Engineer`

**Интерфейс:** `ITrainable { void Train(string program); }`

**Базовый класс `CrewMember`:**
- Приватное поле: `_id`
- Автосвойства: `FullName`, `Experience`
- **Свойство-перечисление:** `public CrewRole Role { get; set; } = CrewRole.Engineer;`
- Вычисляемое: `IsVeteran => Experience > 10`
- Конструкторы: `CrewMember(int id, string fullName, int experience)`, `CrewMember(string fullName) : this(0, fullName, 0)`
- Абстрактный метод `PerformDuty()`

**Наследники:**
- `Commander : CrewMember, ITrainable` — автосвойство `MissionsCount`, вычисляемое `IsExperienced`, `Train()`
- `Pilot : CrewMember, ITrainable` — автосвойство `FlightHours`, вычисляемое `IsAcePilot`, `Train()`
- `Engineer` — автосвойство `Specialization`, вычисляемое `IsTechnical`

**Пример вывода:**
```
[Commander] Ivanov, exp=15, role: Engineer
  veteran: True
Ivanov commands the mission.
Ivanov trained for EVAs.
----------------------------------------
[Engineer] Petrov, exp=5, role: Engineer
  veteran: False
Petrov repairs the station.
Petrov cannot train.
----------------------------------------
```

---

## Вариант 24. Файлы и папки

**Перечисление `ItemType`:**
- Значения: `File`, `Folder`, `Shortcut`
- **Значение по умолчанию:** `ItemType.File`

**Интерфейс:** `ICompressible { void Compress(); }`

**Базовый класс `FileSystemItem`:**
- Приватное поле: `_path`
- Автосвойства: `Name`, `Size`
- **Свойство-перечисление:** `public ItemType Type { get; set; } = ItemType.File;`
- Вычисляемое: `SizeInKb => Size / 1024.0`
- Конструкторы: `FileSystemItem(string path, string name, long size)`, `FileSystemItem(string name) : this("N/A", name, 0)`
- Абстрактный метод `Open()`

**Наследники:**
- `TextFile : FileSystemItem, ICompressible` — автосвойство `Encoding`, вычисляемое `IsSmall`, `Compress()`
- `ImageFile : FileSystemItem, ICompressible` — автосвойство `Resolution`, вычисляемое `IsHighRes`, `Compress()`
- `Shortcut` — автосвойство `TargetPath`, вычисляемое `IsValid`

**Пример вывода:**
```
[TextFile] readme.txt, size=4096, type: File
  size in KB: 4.0
readme.txt opened.
readme.txt compressed.
----------------------------------------
[Shortcut] Chrome.lnk, size=512, type: File, target: chrome.exe
  size in KB: 0.5
Chrome.lnk opened.
Chrome.lnk cannot be compressed.
----------------------------------------
```

---

## Вариант 25. Домашние питомцы

**Перечисление `PetKind`:**
- Значения: `Cat`, `Dog`, `Bird`
- **Значение по умолчанию:** `PetKind.Cat`

**Интерфейс:** `ITrainable { void Train(); }`

**Базовый класс `Pet`:**
- Приватное поле: `_chipId`
- Автосвойства: `Name`, `Age`
- **Свойство-перечисление:** `public PetKind Kind { get; set; } = PetKind.Cat;`
- Вычисляемое: `IsAdult => Age > 1`
- Конструкторы: `Pet(string chipId, string name, int age)`, `Pet(string name) : this("N/A", name, 0)`
- Абстрактный метод `Play()`

**Наследники:**
- `Cat` — автосвойство `IsIndoor`, вычисляемое `IsQuiet`
- `Dog : Pet, ITrainable` — автосвойство `Breed`, вычисляемое `IsLarge`, `Train()`
- `Bird : Pet, ITrainable` — автосвойство `CanSpeak`, вычисляемое `IsTalkative`, `Train()`

**Пример вывода:**
```
[Cat] Barsik, age=3, kind: Cat
  adult: True
Barsik plays with a ball.
Barsik cannot be trained.
----------------------------------------
[Dog] Rex, age=2, kind: Cat, breed: Labrador
  adult: True
Rex plays fetch.
Rex trained to sit.
----------------------------------------
```

---

## Вариант 26. Медицинское оборудование

**Перечисление `DeviceClass`:**
- Значения: `Diagnostic`, `Therapeutic`, `Surgical`
- **Значение по умолчанию:** `DeviceClass.Diagnostic`

**Интерфейс:** `ISterilizable { void Sterilize(); }`

**Базовый класс `MedicalDevice`:**
- Приватное поле: `_serial`
- Автосвойства: `Name`, `PowerConsumption`
- **Свойство-перечисление:** `public DeviceClass Class { get; set; } = DeviceClass.Diagnostic;`
- Вычисляемое: `IsPowerHungry => PowerConsumption > 1000`
- Конструкторы: `MedicalDevice(string serial, string name, int power)`, `MedicalDevice(string name) : this("N/A", name, 0)`
- Абстрактный метод `Operate()`

**Наследники:**
- `MRI : MedicalDevice, ISterilizable` — автосвойство `TeslaStrength`, вычисляемое `IsPowerful`, `Sterilize()`
- `XRay : MedicalDevice, ISterilizable` — автосвойство `RadiationLevel`, вычисляемое `IsDangerous`, `Sterilize()`
- `Defibrillator` — автосвойство `MaxEnergy`, вычисляемое `IsPortable`

**Пример вывода:**
```
[MRI] Siemens, tesla=3.0, class: Diagnostic
  power hungry: True
Siemens scans patient.
Siemens sterilized.
----------------------------------------
[Defibrillator] Zoll, max energy=360 J, class: Diagnostic
  power hungry: False
Zoll defibrillates.
Zoll cannot be sterilized.
----------------------------------------
```

---

## Вариант 27. Спортивный инвентарь

**Перечисление `SportType`:**
- Значения: `Football`, `Tennis`, `Swimming`
- **Значение по умолчанию:** `SportType.Football`

**Интерфейс:** `IInflatable { void Inflate(); }`

**Базовый класс `SportsItem`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public SportType Sport { get; set; } = SportType.Football;`
- Вычисляемое: `PriceWithVat => Price * 1.2m`
- Конструкторы: `SportsItem(int id, string name, decimal price)`, `SportsItem(string name) : this(0, name, 0)`
- Абстрактный метод `Use()`

**Наследники:**
- `Ball : SportsItem, IInflatable` — автосвойство `Diameter`, вычисляемое `IsBig`, `Inflate()`
- `Racket` — автосвойство `StringTension`, вычисляемое `IsTight`
- `SwimRing : SportsItem, IInflatable` — автосвойство `MaxWeight`, вычисляемое `IsAdult`, `Inflate()`

**Пример вывода:**
```
[Ball] Soccer ball, 2000, sport: Football
  price with VAT: 2400
Soccer ball used.
Soccer ball inflated.
----------------------------------------
[Racket] Wilson, 15000, sport: Football, tension: 25
  price with VAT: 18000
Wilson used.
Wilson cannot be inflated.
----------------------------------------
```

---

## Вариант 28. Здания и аренда

**Перечисление `BuildingPurpose`:**
- Значения: `Residential`, `Office`, `Industrial`
- **Значение по умолчанию:** `BuildingPurpose.Residential`

**Интерфейс:** `IRentable { decimal GetMonthlyRent(); }`

**Базовый класс `Building`:**
- Приватное поле: `_cadastralNumber`
- Автосвойства: `Address`, `Area`, `Price`
- **Свойство-перечисление:** `public BuildingPurpose Purpose { get; set; } = BuildingPurpose.Residential;`
- Вычисляемое: `PricePerSqm => Price / Area`
- Конструкторы: `Building(string cadastral, string address, double area, decimal price)`, `Building(string address) : this("N/A", address, 0, 0)`
- Абстрактный метод `Describe()`

**Наследники:**
- `Apartment : Building, IRentable` — автосвойство `Floor`, вычисляемое `IsPenthouse`, `GetMonthlyRent()`
- `Office : Building, IRentable` — автосвойство `Workplaces`, вычисляемое `IsBigOffice`, `GetMonthlyRent()`
- `Warehouse` — автосвойство `CeilingHeight`, вычисляемое `IsTall`

**Пример вывода:**
```
[Apartment] Lenina 5, area=80, purpose: Residential
  price per sqm: 125000
Apartment described.
Monthly rent: 30000.
----------------------------------------
[Warehouse] Prom 1, height=12, purpose: Residential
  price per sqm: 25000
Warehouse described.
Warehouse cannot be rented.
----------------------------------------
```

---

## Вариант 29. Цветы в магазине

**Перечисление `FlowerColor`:**
- Значения: `Red`, `White`, `Yellow`, `Mixed`
- **Значение по умолчанию:** `FlowerColor.Red`

**Интерфейс:** `IFragrant { void Smell(); }`

**Базовый класс `Flower`:**
- Приватное поле: `_id`
- Автосвойства: `Name`, `Price`
- **Свойство-перечисление:** `public FlowerColor Color { get; set; } = FlowerColor.Red;`
- Вычисляемое: `PriceWithDelivery => Price + 300`
- Конструкторы: `Flower(int id, string name, decimal price)`, `Flower(string name) : this(0, name, 0)`
- Абстрактный метод `Present()`

**Наследники:**
- `Rose : Flower, IFragrant` — автосвойство `ThornCount`, вычисляемое `IsPrickly`, `Smell()`
- `Tulip` — автосвойство `StemLength`, вычисляемое `IsLong`
- `Lily : Flower, IFragrant` — автосвойство `PetalCount`, вычисляемое `IsBig`, `Smell()`

**Пример вывода:**
```
[Rose] Red Rose, 250, color: Red
  price with delivery: 550
Red Rose presented.
Red Rose smells sweet.
----------------------------------------
[Tulip] Holland, 150, color: Red, stem: 40
  price with delivery: 450
Holland presented.
Holland cannot smell.
----------------------------------------
```

---

## Вариант 30. Доставка еды

**Перечисление `DeliveryStatus`:**
- Значения: `Pending`, `OnTheWay`, `Delivered`
- **Значение по умолчанию:** `DeliveryStatus.Pending`

**Интерфейс:** `ITrackable { void Track(); }`

**Базовый класс `Delivery`:**
- Приватное поле: `_orderNumber`
- Автосвойства: `CustomerName`, `Price`
- **Свойство-перечисление:** `public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;`
- Вычисляемое: `PriceWithFee => Price + 200`
- Конструкторы: `Delivery(string orderNumber, string customerName, decimal price)`, `Delivery(string customerName) : this("N/A", customerName, 0)`
- Абстрактный метод `Deliver()`

**Наследники:**
- `PizzaDelivery : Delivery, ITrackable` — автосвойство `PizzaSize`, вычисляемое `IsLargePizza`, `Track()`
- `SushiDelivery : Delivery, ITrackable` — автосвойство `SetSize`, вычисляемое `IsFamilySet`, `Track()`
- `GroceryDelivery` — автосвойство `ItemCount`, вычисляемое `IsBigOrder`

**Пример вывода:**
```
[PizzaDelivery] Order #001, pizza size: 40, status: Pending
  price with fee: 1000
Order delivered.
Order #001 tracked: on the way.
----------------------------------------
[GroceryDelivery] Order #002, items: 5, status: Pending
  price with fee: 2500
Order delivered.
Order #002 cannot be tracked.
----------------------------------------
```

---

## Критерии оценки

| Балл | Что проверяется |
|------|-----------------|
| 1 | Ветка создана от `main`, история коммитов читаемая |
| 1 | В трекере создана глобальная задача с тремя подзадачами |
| 1 | Первый коммит сделан после создания проекта в ветке (Subtask 1) |
| 1 | Коммиты по подзадачам имеют корректный формат `Subtask N: ...` |
| 1 | Иерархия: три наследника, вызов `base(...)` |
| 1 | Интерфейс реализован только там, где нужно |
| 1 | Приватное поле есть минимум в базовом классе и используется |
| 1 | Автосвойства и вычисляемые свойства (get-only) присутствуют |
| 1 | Перегрузка конструкторов (минимум два в базовом классе, `this(...)`) |
| 1 | Перечисление объявлено в отдельном файле и используется |
| 1 | Свойство типа перечисления имеет **значение по умолчанию** |
| 1 | Абстрактный метод переопределён во всех наследниках |
| 1 | `ToString()` переопределён и использует специфичные поля |
| 1 | В `Main` есть `List<Base>`, цикл и проверка `is Interface` |
| 1 | Программа компилируется и выдаёт корректный вывод |
| 1 | Все идентификаторы — на английском языке |

**Дополнительно (на «отлично»):** добавить четвёртый наследник и продемонстрировать, что добавление нового класса в список не требует изменений в цикле `Main`.

---

## Вопросы для подготовки к сдаче

### Теория ООП
1. Чем абстрактный класс отличается от обычного?
2. Что такое полиморфизм и как он проявляется в цикле `foreach`?
3. Зачем нужен `override`?
4. Что произойдёт, если в наследнике не переопределить абстрактный метод?

### Интерфейсы
5. Чем интерфейс отличается от абстрактного класса?
6. Почему интерфейс реализуют не все наследники, а только часть?
7. Что вернёт `t is IInterface`, если `t` не реализует интерфейс?
8. В чём разница между `is` и `as`?

### Инкапсуляция
9. Зачем делать поля приватными?
10. Как приватное поле может использоваться в вычисляемом свойстве?

### Свойства
11. Чем автосвойство отличается от обычного свойства с полем?
12. Что такое вычисляемое свойство? Почему у него нет backing-поля?
13. В чём разница между `SpeedInMetersPerSecond => Speed / 3.6` и методом `GetSpeedInMetersPerSecond()`?

### Конструкторы
14. Зачем перегружать конструкторы?
15. Что делает `: this(...)` в объявлении конструктора?
16. Можно ли вызвать `base(...)` и `this(...)` в одном конструкторе?

### Перечисления
17. Зачем использовать `enum` вместо строк или чисел?
18. Как объявить `enum` в отдельном файле?
19. Какое значение по умолчанию получает `enum`-свойство без явной инициализации?
20. Как задать значение по умолчанию для автосвойства типа `enum`?
21. Что произойдёт, если присвоить `enum`-свойству число, не входящее в список значений?

### Git
22. Зачем создавать отдельную ветку для домашнего задания?
23. Что такое осмысленное сообщение коммита?
24. Как связать коммит с номером подзадачи в трекере?
25. Что произойдёт, если сделать все три подзадачи одним коммитом?
