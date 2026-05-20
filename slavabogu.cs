using System;
using Xunit;
using System.Collections.Generic;
using System.Xml.Linq;
using SalonTask;

namespace Salon_xUnit
{
    public class SalonDataFixture
    {
        public IEnumerable<XElement> Cars { get; private set; }
        public IEnumerable<XElement> Brands { get; private set; }
        public IEnumerable<XElement> Menegers { get; private set; }
        public IEnumerable<XElement> Buyments { get; private set; }

        public SalonDataFixture()
        {
            Menegers = XElement.Parse(@"
<Menegers>
  <Meneger>
    <Id>1</Id>
    <LastName>Trukhan</LastName>
    <Stage>5</Stage>
  </Meneger>
  <Meneger>
    <Id>2</Id>
    <LastName>Kruvano</LastName>
    <Stage>7</Stage>
  </Meneger>
  <Meneger>
    <Id>3</Id>
    <LastName>Yaremko</LastName>
    <Stage>5</Stage>
  </Meneger>
</Menegers>
            ").Descendants("Meneger");
            Brands = XDocument.Load("brands.xml").Descendants("Brand");
            Cars = XDocument.Load("cars.xml").Descendants("Car");
            Buyments = XDocument.Load("buyments.xml").Descendants("Buyment");
        }
    }
    public class UnitTest1 : IClassFixture<SalonDataFixture>
    {
        private readonly SalonDataFixture _fixture;
        public UnitTest1(SalonDataFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(3, "Kruvano", 4)]
        [InlineData(1, "Trukhan", 5)]
        public void Test1(int expc, string last, int month)
        {
            var result = SalonLogic.CreateMonthReport(_fixture.Menegers, _fixture.Brands, _fixture.Cars, _fixture.Buyments, month);
            var count = result.Elements("Meneger").Count();
            Assert.Equal(expc, count);
            var surname = (string)result.Elements("Meneger").First().Attribute("LastName");
            Assert.Equal(last, surname);
        }
        [Theory]
        [InlineData(3, "Italy", 5000)]
        [InlineData(1, "Italy", 10000)]
        public void Test2(int expc, string country, int minrev)
        {
            var result = SalonLogic.CreateCountryStatistic(_fixture.Brands, _fixture.Cars, _fixture.Buyments, minrev);
            var count = result.Elements("Country").Count();
            Assert.Equal(expc, count);
            var countryname = (string)result.Elements("Country").First().Attribute("Name");
            Assert.Equal(country, countryname);
        }
    }
    public class MenegerTest
    {
        [Theory]
        [InlineData(10, "Blabla", 10)]
        [InlineData(15, "Blblb", 45)]
        public void Correct_data_setter_test(int id, string ln, int st)
        {
            var obj = new Meneger(1, "test1", 5);
            obj.Id = id;
            obj.LastName = ln;
            obj.Stage = st;
            Assert.Equal(id, obj.Id);
            Assert.Equal(ln, obj.LastName);
            Assert.Equal(st, obj.Stage);
        }
        [Fact]
        public void InCorrect_LastName_setter_test()
        {
            var ob2 = new Meneger(12, "test2", 11);
            Assert.Throws<ArgumentException>(() => ob2.LastName = "");
        }
        [Fact]
        public void InCorresct_Stage_setter_test()
        {
            var ob3 = new Meneger(9, "test3", 9);
            Assert.Throws<ArgumentException>(() => ob3.Stage = -19);
        }
    }
}
 
using System;
using Xunit;
using System.Collections.Generic;
using System.Xml.Linq;
using SalonTask;
 
namespace Salon_xUnit
{
    public class SalonDataFixture
    {
        public IEnumerable<XElement> Cars { get; private set; }
        public IEnumerable<XElement> Brands { get; private set; }
        public IEnumerable<XElement> Menegers { get; private set; }
        public IEnumerable<XElement> Buyments { get; private set; }

        public SalonDataFixture()
        {
            Menegers = XElement.Parse(@"
<Menegers>
 <Meneger>
 <Id>1</Id>
 <LastName>Trukhan</LastName>
 <Stage>5</Stage>
 </Meneger>
 <Meneger>
 <Id>2</Id>
 <LastName>Kruvano</LastName>
 <Stage>7</Stage>
 </Meneger>
 <Meneger>
 <Id>3</Id>
 <LastName>Yaremko</LastName>
 <Stage>5</Stage>
 </Meneger>
</Menegers>
 ").Descendants("Meneger");
            Brands = XDocument.Load("brands.xml").Descendants("Brand");
            Cars = XDocument.Load("cars.xml").Descendants("Car");
            Buyments = XDocument.Load("buyments.xml").Descendants("Buyment");
        }
    }
    public class UnitTest1 : IClassFixture<SalonDataFixture>
    {
        private readonly SalonDataFixture _fixture;
        public UnitTest1(SalonDataFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(3, "Kruvano", 4)]
        [InlineData(1, "Trukhan", 5)]
        public void Test1(int expc, string last, int month)
        {
            var result = SalonLogic.CreateMonthReport(_fixture.Menegers, _fixture.Brands, _fixture.Cars, _fixture.Buyments, month);
            var count = result.Elements("Meneger").Count();
            Assert.Equal(expc, count);
            var surname = (string)result.Elements("Meneger").First().Attribute("LastName");
            Assert.Equal(last, surname);
        }
        [Theory]
        [InlineData(3, "Italy", 5000)]
        [InlineData(1, "Italy", 10000)]
        public void Test2(int expc, string country, int minrev)
        {
            var result = SalonLogic.CreateCountryStatistic(_fixture.Brands, _fixture.Cars, _fixture.Buyments, minrev);
            var count = result.Elements("Country").Count();
            Assert.Equal(expc, count);
            var countryname = (string)result.Elements("Country").First().Attribute("Name");
            Assert.Equal(country, countryname);
        }
    }
    public class MenegerTest
    {
        [Theory]
        [InlineData(10, "Blabla", 10)]
        [InlineData(15, "Blblb", 45)]
        public void Correct_data_setter_test(int id, string ln, int st)
        {
            var obj = new Meneger(1, "test1", 5);
            obj.Id = id;
            obj.LastName = ln;
            obj.Stage = st;
            Assert.Equal(id, obj.Id);
            Assert.Equal(ln, obj.LastName);
            Assert.Equal(st, obj.Stage);
        }
        [Fact]
        public void InCorrect_LastName_setter_test()
        {
            var ob2 = new Meneger(12, "test2", 11);
            Assert.Throws<ArgumentException>(() => ob2.LastName = "");
        }
        [Fact]
        public void InCorresct_Stage_setter_test()
        {
            var ob3 = new Meneger(9, "test3", 9);
            Assert.Throws<ArgumentException>(() => ob3.Stage = -19);
        }
    }
}

Ось детальний розбір тих частин коду, про які ви запитали, а також список інших корисних методів LINQ.
### 1. Рядки, де обчислюється знижка k
Знижка обчислюється під час формування проміжної таблиці в методі GeneratePatientCostsReport.
```csharp
let age = opDate.Year - birthDate.Year - (opDate.DayOfYear < birthDate.DayOfYear ? 1 : 0)
let isMinor = age < 18
            
let price = decimal.Parse(med.Attribute("Price").Value, CultureInfo.InvariantCulture)
            
// ОСЬ РЯДОК ЗІ ЗНИЖКОЮ:
let cost = qty * price * (isMinor ? (1m - discountK / 100m) : 1m)
 
```
**Як це працює:**Ми використовуємо тернарний оператор умова ? дія_якщо_так : дія_якщо_ні.Якщо пацієнт неповнолітній (isMinor == true), ми беремо його знижку (наприклад, якщо discountK = 15, то 1m - 15/100 = 0.85, тобто пацієнт платить 85% вартості). Якщо повнолітній — множимо на 1m (тобто повна вартість).
### 2. Рядки, де використовується Sum
Метод Sum() використовується для знаходження загальної суми елементів у згрупованих даних (після використання group ... by).
У завданні **(а)** (вартість для кожного пацієнта):
```csharp
group cost by pat.Attribute("LastName").Value into patientGroup
orderby patientGroup.Sum() descending // Використання Sum для сортування за спаданням загальної вартості
 
select new
       {
           LastName = patientGroup.Key,
           TotalCost = patientGroup.Sum()    // Використання Sum для запису загальної вартості в таблицю
       }
 
```
У завданні **(в)** (щоденний відпуск):
```csharp
group qty by new { Date = opDate.ToString("yyyy-MM-dd"), MedicineName = med.Attribute("Name").Value } into dailyGroup
// ...
select new
       {
           Date = dailyGroup.Key.Date,
           Medicine = dailyGroup.Key.MedicineName,
           TotalQuantity = dailyGroup.Sum() // Використання Sum для підрахунку загальної кількості проданих одиниць препарату
       }
 
```
### 3. Які ще методи агрегації (LINQ) можуть бути використані?
Окрім Sum(), у LINQ є багато інших методів для роботи з колекціями чи групами (patientGroup або dailyGroup). Ось основні з них із прикладами, як би їх можна було застосувати у вашій аптеці:
 ***Count() * *: Повертає кількість елементів у групі.
   * *Приклад:*patientGroup.Count() — скільки всього разів пацієнт купував ліки (кількість операцій).
 * **Average()**: Вираховує середнє арифметичне значення.
   * *Приклад:*patientGroup.Average() — середній чек пацієнта за одну операцію.
 * **Max()**: Знаходить максимальне значення.
   * *Приклад:*patientGroup.Max() — найдорожча покупка пацієнта.
 * **Min()**: Знаходить мінімальне значення.
   * *Приклад:*dailyGroup.Min() — найменша кількість проданого препарату за одну транзакцію в цей день.
 * **Aggregate()**: Дозволяє застосувати власну логіку для об'єднання елементів (наприклад, перемножити всі числа замість додавання, або з'єднати рядки).
   * *Приклад:*patientGroup.Aggregate((x, y) => x + y) — це ручний аналог методу Sum().
 