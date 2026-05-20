using Program;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ObsTest
{
    public class ObsFixture
    {
        public IEnumerable<XElement> Automobiles { get; private set; }
        public IEnumerable<XElement> Clients { get; private set; }
        public IEnumerable<XElement> Categorys { get; private set; }
        public IEnumerable<XElement> Infos { get; private set; }

        public ObsFixture()
        {
            Automobiles = XElement.Parse(@"
<Automobiles>
	<Automobile>
		<Id>1</Id>
		<Mark>Mark1</Mark>
		<CategoryId>1</CategoryId>
	</Automobile>
</Automobiles>").Descendants("Automobile");
            Clients = XElement.Parse(@"<Clients>
	<Client>
		<Id>1</Id>
		<Surname>Surname1</Surname>
	</Client>
</Clients>").Descendants("Client");
            Categorys = XElement.Parse(@"<Categorys>
	<Category>
		<Id>1</Id>
		<Name>Name1</Name>
		<Price>50.0</Price>
	</Category>
</Categorys>").Descendants("Category");
            Infos = XElement.Parse(@"<Infos>
	<Info>
		<Date>26.05.2025</Date>
		<AutomobileId>1</AutomobileId>
		<ClientId>1</ClientId>
		<Days>5</Days>
	</Info>
</Infos>").Descendants("Info").Concat(XElement.Parse(@"<Infos>
	<Info>
		<Date>27.05.2025</Date>
		<AutomobileId>1</AutomobileId>
		<ClientId>1</ClientId>
		<Days>7</Days>
	</Info>
</Infos>").Descendants("Info"));
        }
    }
    public class UnitTest1 : IClassFixture<ObsFixture>
    {
        private readonly ObsFixture _fixture;
        public UnitTest1(ObsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TaskATest()
        {
     
            var exptree = XElement.Parse(@"<TaskA><Auto Mark=""Mark1""><Renter Surname=""Surname1"" /></Auto></TaskA>");

            // ВИПРАВЛЕНО: Правильний порядок: спочатку Клієнти, потім Категорії
            var result = Logic.TaskA(_fixture.Automobiles, _fixture.Clients, _fixture.Categorys, _fixture.Infos);

            // Тепер це порівняння спрацює ідеально
            Assert.True(XNode.DeepEquals(exptree, result));
        }
        
    }
}