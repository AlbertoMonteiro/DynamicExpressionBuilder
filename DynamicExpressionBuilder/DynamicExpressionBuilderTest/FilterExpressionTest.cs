using System;
using System.Collections.Generic;
using System.Linq;
using DynamicExpressionBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicExpressionBuilderTest
{
    [TestClass]
    public class FilterExpressionTest
    {
        private List<Person> _peoples;

        [TestInitialize]
        public void StartTest()
        {
            _peoples = new List<Person>
                          {
                              new Person("Alberto", 22, true, new Task("Teste 2", DateTime.Today)),
                              new Person("Fabio", 24, false, new Task("Teste 2", DateTime.Today)),
                              new Person("Abraão", 23, false, new Task("Teste 2", DateTime.Today)),
                              new Person("Pedro", 26, false, new Task("Teste 2", DateTime.Today))
                          };
        }

        [TestMethod]
        public void ResultExpressionTestHelper()
        {
            var target = new FilterExpression<Person>();

            var filterExpression = target.Start(p => p.Name.Contains("o"));
            filterExpression = filterExpression.And(p => p.Age <= 25);
            filterExpression = filterExpression.And(p => p.Tasks.Any(x => x.When == DateTime.Today));
            filterExpression = filterExpression.And(p => p.Working);

            var persons = _peoples.Where(filterExpression.ResultExpression);
            var expected = _peoples.Where(p1 => p1.Name.Contains("o") &&
                                                p1.Age <= 25 &&
                                                p1.Tasks.Any(x => x.When == DateTime.Today) &&
                                                p1.Working);
            var result = AreCollectionsEquals(persons.ToList(), expected.ToList());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResultExpressionTestHelper2()
        {
            var target = new FilterExpression<Person>();

            var filterExpression = target.Start(p => p.Name.Contains("berto")).And(p => p.Age <= 25);

            var persons = _peoples.Where(filterExpression.ResultExpression);
            var expected = _peoples.Where(p => p.Name.Contains("berto") && p.Age <= 25);
            bool result = AreCollectionsEquals(persons.ToList(), expected.ToList());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResultExpressionTestHelper3()
        {
            var target = new FilterExpression<Person>();

            var persons = _peoples.Where(target.Start(p => p.Working).ResultExpression);
            var expected = _peoples.Where(p => p.Working);
            bool result = AreCollectionsEquals(persons.ToList(), expected.ToList());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResultExpressionTestHelper4()
        {
            var target = new FilterExpression<Person>();

            var persons = _peoples.Where(target.Start(p => p is Person).ResultExpression);
            var expected = _peoples.Where(p => p is Person);
            bool result = AreCollectionsEquals(persons.ToList(), expected.ToList());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResultExpressionTestHelper5()
        {
            var target = new FilterExpression<Person>();

            var persons = _peoples.Where(target.Start(p => p is Int32).ResultExpression);
            var expected = _peoples.Where(p => p is Person);
            bool result = AreCollectionsEquals(persons.ToList(), expected.ToList());
            Assert.IsFalse(result);
        }

        private bool AreCollectionsEquals(List<Person> persons, IList<Person> expected)
        {
            if (persons.Count() != expected.Count())
                return false;

            return !persons.Where((person, i) => !person.Equals(expected[i])).Any();
        }
    }
}
