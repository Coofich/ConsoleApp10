using System;

using System.Collections.Generic;

using System.Linq;

using System.Security.Cryptography;

using System.Text.RegularExpressions;

using System.Xml.Linq;

namespace DiagnosticCentre

{

    public static class CentreLogic

    {

        public static XElement TaskA(IEnumerable<XElement> obstezhs, IEnumerable<XElement> doctors, IEnumerable<XElement> pacients, IEnumerable<XElement> categorys, IEnumerable<XElement> info)

        {

            var data = (from i in info

                        join o in obstezhs on (int)i.Element("ObstezhId") equals (int)o.Element("Id")

                        join p in pacients on (int)i.Element("PacientId") equals (int)p.Element("Id")

                        join c in categorys on (int)o.Element("CategoryId") equals (int)c.Element("Id")

                        join d in doctors on (int)o.Element("DoctorId") equals (int)d.Element("Id")

                        select new

                        {

                            DoctorLastname = (string)d.Element("Lastname"),

                            Category = (string)c.Element("Name")

                        });

            return new XElement("TaskA",

                from d in data

                group d by d.DoctorLastname into dgroup

                orderby dgroup.Key ascending

                select new XElement("Doctor", new XAttribute("Lastname", dgroup.Key),

                    from c in dgroup

                    group c by c.Category into g

                    orderby g.Key ascending

                    select new XElement("Category", new XAttribute("Name", g.Key), new XAttribute("Count", g.Count())

                    )

                    )

                );

        }

        public static XElement TaskB(IEnumerable<XElement> obstezhs, IEnumerable<XElement> doctors, IEnumerable<XElement> pacients, IEnumerable<XElement> categorys, IEnumerable<XElement> info)

        {

            var data = (from i in info

                        join o in obstezhs on (int)i.Element("ObstezhId") equals (int)o.Element("Id")

                        join p in pacients on (int)i.Element("PacientId") equals (int)p.Element("Id")

                        join c in categorys on (int)o.Element("CategoryId") equals (int)c.Element("Id")

                        join d in doctors on (int)o.Element("DoctorId") equals (int)d.Element("Id")

                        select new

                        {

                            Month = DateTime.Parse((string)i.Element("Date")).Month,

                            Category = (string)c.Element("Name"),

                            Pacient = (string)p.Element("Lastname"),

                            Cost = (float)c.Element("Price")

                        });

            return new XElement("TaskB",

                from d in data

                group d by d.Month into gMonth

                orderby gMonth.Key

                select new XElement("Number",

                    new XAttribute("Month", gMonth.Key),

                    from m in gMonth

                    group m by m.Category into gCategory

                    orderby gCategory.Key

                    let PatientsTotal = (from c in gCategory

                                         group c by c.Pacient into gPacient

                                         select new { Patient = gPacient.Key, Total = gPacient.Sum(x => x.Cost) })

                    let maxCost = PatientsTotal.Max(x => x.Total)

                    let TopPacient = from p in PatientsTotal

                                     where p.Total == maxCost

                                     orderby p.Patient

                                     select p.Patient

                    select new XElement("Category",

                        new XAttribute("Name", gCategory.Key),

                        new XAttribute("MaxTotal", maxCost),

                        from p in TopPacient

                        select new XElement("TopPacient", new XAttribute("Lastname", p))

                        )

)

            );


        }



        class Program

        {

            static void Main(string[] args)

            {

                var obstezhs = XDocument.Load("obstezhs.xml").Descendants("Obstezh");

                var doctors = XDocument.Load("doctors.xml").Descendants("Doctor");

                var pacients = XDocument.Load("pacients.xml").Descendants("Pacient");

                var categorys = XDocument.Load("categorys.xml").Descendants("Category");

                var infos1 = XDocument.Load("infos1.xml").Descendants("Info");

                var infos2 = XDocument.Load("infos2.xml").Descendants("Info");

                var info = infos1.Concat(infos2);

                var result1 = CentreLogic.TaskA(obstezhs, doctors, pacients, categorys, info);

                result1.Save("TaskA.xml");

                var result2 = CentreLogic.TaskB(obstezhs, doctors, pacients, categorys, info);

                result2.Save("TaskB.xml");


            }

        }

    }

}
