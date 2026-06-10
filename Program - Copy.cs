using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiagnosticCenterApp
{
    public class Logic
    {
        public static XElement TaskA(IEnumerable<XElement> analyses, IEnumerable<XElement> patients, IEnumerable<XElement> categories, IEnumerable<XElement> records)
        {
            var data = (from r in records
                        join a in analyses on (int)r.Element("AnalysisId") equals (int)a.Element("Id")
                        join p in patients on (int)r.Element("PatientId") equals (int)p.Element("Id")
                        join c in categories on (int)a.Element("CategoryId") equals (int)c.Element("Id")
                        select new
                        {
                            CategoryName = (string)c.Element("Name"),
                            Surname = (string)p.Element("Surname")
                        });

            return new XElement("TaskAResult",
                from d in data
                group d by d.CategoryName into gCategory
                orderby gCategory.Key
                select new XElement("Category",
                    new XAttribute("Name", gCategory.Key),
                    from cat in gCategory
                    group cat by cat.Surname into gSurname
                    orderby gSurname.Key
                    select new XElement("Patient",
                        new XAttribute("Surname", gSurname.Key)
                    )
                )
            );
        }

        public static XElement TaskB(IEnumerable<XElement> analyses, IEnumerable<XElement> patients, IEnumerable<XElement> categories, IEnumerable<XElement> records)
        {
            var data = (from r in records
                        join a in analyses on (int)r.Element("AnalysisId") equals (int)a.Element("Id")
                        join p in patients on (int)r.Element("PatientId") equals (int)p.Element("Id")
                        join c in categories on (int)a.Element("CategoryId") equals (int)c.Element("Id")

                        // РОБОТА З КОМБІНОВАНИМ РЯДКОМ: "Phone: +380...; Email: test@gmail.com"
                        let contacts = (string)p.Element("Contacts")
                        let emailIndex = contacts.IndexOf("Email: ")
                        let email = emailIndex != -1 ? contacts.Substring(emailIndex + 7).Trim() : ""

                        // Обчислення знижки на основі розпарсеного емейлу
                        let basePrice = (decimal)c.Element("Price")
                        let quantity = (int)r.Element("Quantity")
                        let discount = email.EndsWith("@gmail.com") ? 0.15m : 0.0m
                        let totalCost = (basePrice * quantity) * (1m - discount)

                        select new
                        {
                            Month = DateTime.Parse((string)r.Element("Date")).Month,
                            CategoryName = (string)c.Element("Name"),
                            Surname = (string)p.Element("Surname"),
                            Cost = totalCost
                        });

            return new XElement("TaskBResult",
                from d in data
                group d by d.Month into gMonth
                orderby gMonth.Key
                select new XElement("MonthData",
                    new XAttribute("Month", gMonth.Key),

                    from m in gMonth
                    group m by m.CategoryName into gCategory
                    orderby gCategory.Key

                    let patientSums = (from item in gCategory
                                       group item by item.Surname into gSurname
                                       select new
                                       {
                                           Surname = gSurname.Key,
                                           TotalSum = gSurname.Sum(x => x.Cost)
                                       })

                    let maxCost = patientSums.Any() ? patientSums.Max(x => x.TotalSum) : 0m

                    let topPatients = (from ps in patientSums
                                       where ps.TotalSum == maxCost
                                       orderby ps.Surname
                                       select ps.Surname)

                    select new XElement("CategoryData",
                        new XAttribute("Category", gCategory.Key),
                        new XAttribute("MaxCost", Math.Round(maxCost, 2)),
                        from tp in topPatients
                        select new XElement("TopPatient",
                            new XAttribute("Surname", tp)
                        )
                    )
                )
            );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Завантаження основних файлів та об'єднання двох файлів з інформацією про записи
            var analyses = XDocument.Load("Analyses.xml").Descendants("Analysis");
            var patients = XDocument.Load("Patients.xml").Descendants("Patient");
            var categories = XDocument.Load("Categories.xml").Descendants("Category");

            var records = XDocument.Load("records1.xml").Descendants("Record")
                          .Concat(XDocument.Load("records2.xml").Descendants("Record"));

            var resultA = Logic.TaskA(analyses, patients, categories, records);
            var resultB = Logic.TaskB(analyses, patients, categories, records);

            resultA.Save("TaskAResult.xml");
            resultB.Save("TaskBResult.xml");

            Console.WriteLine("Файли успішно збережено!");
        }
    }
}

/*
 * public static XElement TaskB(IEnumerable<XElement> analyses, IEnumerable<XElement> patients, IEnumerable<XElement> categories, IEnumerable<XElement> records)
{
    var data = (from r in records
                join a in analyses on (int)r.Element("AnalysisId") equals (int)a.Element("Id")
                join p in patients on (int)r.Element("PatientId") equals (int)p.Element("Id")
                join c in categories on (int)a.Element("CategoryId") equals (int)c.Element("Id")
                
                // 1. Беремо рядок контактів (захищаємо від null через ??)
                let contactInfo = (string)p.Element("Contacts") ?? ""
                
                // 2. Визначаємо, що саме нам прийшло (якщо є '@' — це пошта, інакше пустий рядок)
                let email = contactInfo.Contains("@") ? contactInfo.Trim() : ""
                
                // 3. Рахуємо гроші та знижку
                let basePrice = (decimal)c.Element("Price")
                let quantity = (int)r.Element("Quantity")
                let discount = email.EndsWith("@gmail.com") ? 0.15m : 0.0m
                let totalCost = (basePrice * quantity) * (1m - discount)
                
                select new
                {
                    Month = DateTime.Parse((string)r.Element("Date")).Month,
                    CategoryName = (string)c.Element("Name"),
                    Surname = (string)p.Element("Surname"),
                    Cost = totalCost
                });

    return new XElement("TaskBResult",
        from d in data
        group d by d.Month into gMonth
        orderby gMonth.Key
        select new XElement("MonthData",
            new XAttribute("Month", gMonth.Key),
            
            from m in gMonth
            group m by m.CategoryName into gCategory
            orderby gCategory.Key
            
            let patientSums = (from item in gCategory
                               group item by item.Surname into gSurname
                               select new { Surname = gSurname.Key, TotalSum = gSurname.Sum(x => x.Cost) })
                               
            let maxCost = patientSums.Any() ? patientSums.Max(x => x.TotalSum) : 0m
            
            let topPatients = (from ps in patientSums
                               where ps.TotalSum == maxCost
                               orderby ps.Surname
                               select ps.Surname)
                               
            select new XElement("CategoryData",
                new XAttribute("Category", gCategory.Key),
                new XAttribute("MaxCost", Math.Round(maxCost, 2)),
                from tp in topPatients
                select new XElement("TopPatient",
                    new XAttribute("Surname", tp)
                )
            )
        )
    );
}
*/
/*
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DiagnosticCenterApp;
using Xunit;

namespace DiagnosticCenterTests
{
    public class DiagnosticFixture
    {
        public IEnumerable<XElement> Analyses { get; private set; }
        public IEnumerable<XElement> Patients { get; private set; }
        public IEnumerable<XElement> Categories { get; private set; }
        public IEnumerable<XElement> Records { get; private set; }

        public DiagnosticFixture()
        {
            Categories = XElement.Parse(@"
            <Categories>
                <Category><Id>1</Id><Name>Біохімія</Name><Price>200.0</Price></Category>
            </Categories>").Descendants("Category");

            Analyses = XElement.Parse(@"
            <Analyses>
                <Analysis><Id>10</Id><Name>Глюкоза</Name><CategoryId>1</CategoryId></Analysis>
            </Analyses>").Descendants("Analysis");

            // Один користувач має пошту на gmail (отримає знижку 15%), інший — ні.
            Patients = XElement.Parse(@"
            <Patients>
                <Patient><Id>100</Id><Surname>Шевченко</Surname><Contacts>Phone: +380111; Email: sheva@gmail.com</Contacts></Patient>
                <Patient><Id>200</Id><Surname>Коваленко</Surname><Contacts>Phone: +380222; Email: koval@ukr.net</Contacts></Patient>
            </Patients>").Descendants("Patient");

            // Об'єднуємо два джерела записів за допомогою Concat
            var part1 = XElement.Parse(@"
            <Records>
                <Record><Date>15.05.2026</Date><AnalysisId>10</AnalysisId><PatientId>100</PatientId><Quantity>1</Quantity></Record>
            </Records>").Descendants("Record");

            var part2 = XElement.Parse(@"
            <Records>
                <Record><Date>16.05.2026</Date><AnalysisId>10</AnalysisId><PatientId>200</PatientId><Quantity>1</Quantity></Record>
            </Records>").Descendants("Record");

            Records = part1.Concat(part2);
        }
    }

    public class DiagnosticTests : IClassFixture<DiagnosticFixture>
    {
        private readonly DiagnosticFixture _fixture;

        public DiagnosticTests(DiagnosticFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestTaskA()
        {
            // Очікуване дерево: Категорії -> Пацієнти за алфавітом без повторень
            var expectedXml = XElement.Parse(@"
            <TaskAResult>
                <Category Name=""Біохімія"">
                    <Patient Surname=""Коваленко"" />
                    <Patient Surname=""Шевченко"" />
                </Category>
            </TaskAResult>");

            var realXml = Logic.TaskA(_fixture.Analyses, _fixture.Patients, _fixture.Categories, _fixture.Records);

            Assert.True(XNode.DeepEquals(expectedXml, realXml));
        }

        [Fact]
        public void TestTaskB_WithComplexStringParsing()
        {
            /*
              Розрахунок вартості:
              Базова ціна = 200. Кількість = 1.
              1) Шевченко (gmail.com) -> Знижка 15% -> 200 * 0.85 = 170.
              2) Коваленко (ukr.net) -> Без знижки -> 200 * 1.00 = 200.
              Максимальна сума у категорії за Травень (місяць 5) складає 200 (Коваленко).

            var expectedXml = XElement.Parse(@"
            <TaskBResult>
                <MonthData Month=""5"">
                    <CategoryData Category=""Біохімія"" MaxCost=""200"">
                        <TopPatient Surname=""Коваленко"" />
                    </CategoryData>
                </MonthData>
            </TaskBResult>");

            var realXml = Logic.TaskB(_fixture.Analyses, _fixture.Patients, _fixture.Categories, _fixture.Records);

            Assert.True(XNode.DeepEquals(expectedXml, realXml));
        }
    }
}
public static XElement Task1_PromoReport(IEnumerable<XElement> datas, IEnumerable<XElement> ofises, IEnumerable<XElement> avtos, string targetClass, int minRentals)
{
    // Крок 1. Збираємо плоску таблицю даних, фільтруємо за класом і рахуємо вартість
    var prepData = from d in datas
                   join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                   where (string)a.Element("Class") == targetClass
                   join o in ofises on (int)a.Element("O_id") equals (int)o.Element("O_id")
                   
                   let days = (int)d.Element("Days")
                   let price = days * (int)a.Element("BasePrice")
                   let paid = days > 7 ? price * 0.85 : price
                   
                   select new
                   {
                       OfficeName = (string)o.Element("Name"),
                       City = (string)o.Element("City"),
                       Paid = paid
                   };

    // Крок 2. Групуємо, застосовуємо умови (minRentals) та формуємо XML
    return new XElement("PromoReport",
        new XAttribute("TargetClass", targetClass),
        from p in prepData
        group p by new { p.OfficeName, p.City } into g
        
        let rentalsCount = g.Count()
        let totalRevenue = g.Sum(x => x.Paid)
        
        where rentalsCount >= minRentals
        orderby totalRevenue descending
        
        select new XElement("Office",
            new XAttribute("Name", g.Key.OfficeName),
            new XAttribute("City", g.Key.City),
            new XAttribute("TotalRentals", rentalsCount),
            new XAttribute("Revenue", totalRevenue)
        )
    );
}

public static XElement Task2_ClientPreferences(IEnumerable<XElement> datas, IEnumerable<XElement> clients, IEnumerable<XElement> avtos)
{
    // Крок 1. Збираємо загальну таблицю зі знижками
    var prepData = from d in datas
                   join c in clients on (int)d.Element("C_id") equals (int)c.Element("C_id")
                   join a in avtos on (int)d.Element("A_id") equals (int)a.Element("A_id")
                   
                   let days = (int)d.Element("Days")
                   let price = days * (int)a.Element("BasePrice")
                   let paid = days > 7 ? price * 0.85 : price
                   
                   select new
                   {
                       Surname = (string)c.Element("Sur"),
                       Marka = (string)a.Element("Marka"),
                       Paid = paid
                   };

    // Крок 2. Формуємо багаторівневий XML
    return new XElement("ClientPreferences",
        from p in prepData
        group p by p.Surname into clientGroup
        
        let totalSpent = clientGroup.Sum(x => x.Paid)
        orderby totalSpent descending
        
        // Внутрішня статистика марок для КОЖНОГО клієнта
        let carsStats = from cg in clientGroup
                        group cg by cg.Marka into markaGroup
                        select new 
                        { 
                            Marka = markaGroup.Key, 
                            RentCount = markaGroup.Count() 
                        }
        
        // Шукаємо максимальну кількість оренд серед марок цього клієнта
        let maxRentals = carsStats.Max(x => x.RentCount)
        
        select new XElement("Client",
            new XAttribute("Surname", clientGroup.Key),
            new XAttribute("TotalSpent", totalSpent),
            
            new XElement("FavoriteCars",
                from cs in carsStats
                where cs.RentCount == maxRentals
                select new XElement("Car",
                    new XAttribute("Marka", cs.Marka),
                    new XAttribute("RentCount", cs.RentCount)
                )
            )
        )
    );
}
*/