using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Program
{
    public class Logic
    {
        public static XElement TaskA(IEnumerable<XElement> automobiles, IEnumerable<XElement> clients, IEnumerable<XElement> categorys, IEnumerable<XElement> infos)
        {
            var data = (from i in infos
                        join a in automobiles on (int)i.Element("AutomobileId") equals (int)a.Element("Id")
                        join cl in clients on (int)i.Element("ClientId") equals (int)cl.Element("Id")
                        join ct in categorys on (int)a.Element("CategoryId") equals (int)ct.Element("Id")
                        select new
                        {
                            Mark = (string)a.Element("Mark"),
                            Surname = (string)cl.Element("Surname"),
                        });

            return new XElement("TaskA",
                from d in data
                group d by d.Mark into gMark
                orderby gMark.Key
                select new XElement("Auto",
                    new XAttribute("Mark", gMark.Key),
                    from mark in gMark
                    group mark by mark.Surname into gSurname
                    orderby gSurname.Key
                    select new XElement("Renter",
                        new XAttribute("Surname", gSurname.Key)
                    )
                )
            );
        }

        public static XElement TaskB(IEnumerable<XElement> automobiles, IEnumerable<XElement> clients, IEnumerable<XElement> categorys, IEnumerable<XElement> infos)
        {
            var data = (from i in infos
                        join a in automobiles on (int)i.Element("AutomobileId") equals (int)a.Element("Id")
                        join cl in clients on (int)i.Element("ClientId") equals (int)cl.Element("Id")
                        join ct in categorys on (int)a.Element("CategoryId") equals (int)ct.Element("Id")

                        // ВИПРАВЛЕНО: Ціну беремо з категорії (ct) через decimal, щоб уникнути проблем з крапкою
                        let total = (int)i.Element("Days") * (decimal)ct.Element("Price")
                        select new
                        {
                            Month = DateTime.Parse((string)i.Element("Date")).Month,
                            Category = (string)ct.Element("Name"), // ВИПРАВЛЕНО: ct.Element("Name") замість відсутнього a.Element("Category")
                            Surname = (string)cl.Element("Surname"),
                            Total = total
                        });

            return new XElement("TaskB",
                from d in data
                group d by d.Month into gMonth
                orderby gMonth.Key
                select new XElement("Month",
                    new XAttribute("Month", gMonth.Key),

                    from month in gMonth
                    group month by month.Category into gCategory
                    orderby gCategory.Key

                    let sum = (from category in gCategory
                               group category by category.Surname into gSurname
                               select new { Surname = gSurname.Key, Sum = gSurname.Sum(x => x.Total) }
                              )

                    // Захист: якщо раптом група пуста, повернемо 0, щоб уникнути Exception
                    let max = sum.Any() ? sum.Max(x => x.Sum) : 0

                    let topClient = (from c in sum
                                     where c.Sum == max
                                     orderby c.Surname
                                     select c.Surname
                                    )

                    select new XElement("Category",
                        new XAttribute("Category", gCategory.Key),
                        new XAttribute("MaxCost", max),
                        from c in topClient
                        select new XElement("TopClient",
                            new XAttribute("Surname", c)
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
            // Переконайся, що файли лежать поруч з файлом .exe програми (в папці bin/Debug/net...)
            var automobiles = XDocument.Load("Automobiles.xml").Descendants("Automobile");
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var categorys = XDocument.Load("categorys.xml").Descendants("Category");

            var info = XDocument.Load("info1.xml").Descendants("Info")
                       .Concat(XDocument.Load("info2.xml").Descendants("Info"));

            var AResult = Logic.TaskA(automobiles, clients, categorys, info);
            var BResult = Logic.TaskB(automobiles, clients, categorys, info);

            AResult.Save("TaskAResult.xml");
            BResult.Save("TaskBResult.xml");
        }
    }
}