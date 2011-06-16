using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DynamicExpressionBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicExpressionBuilderTest
{
    [TestClass]
    public class FilterExpressionTest
    {
        private List<Person> peoples;

        [TestInitialize]
        public void StartTest()
        {
            peoples = new List<Person>();
            for (long i = 0; i < 1000000; i++)
            {
                peoples.Add(new Person(Guid.NewGuid().ToString(), (int)i, i % 2 == 0));
            }
        }

        [TestMethod]
        public void ReturnSameValueWhenUseExpressionAnd()
        {
            var target = new FilterExpression<Person>();

            target.Start(p => p.Name.Contains("o")).And(p => p.Age <= 25);

            var persons = peoples.Where(target.ResultExpression);

            var expected = peoples.Where(p => p.Name.Contains("o") && p.Age <= 25);

            bool result = AreCollectionsEquals(persons, expected);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnSameValueWhenUseExpressionOr()
        {
            var target = new FilterExpression<Person>();

            target.Start(p => p.Name.Contains("o")).Or(p => p.Age <= 25);

            var persons = peoples.Where(target.ResultExpression);

            var expected = peoples.Where(p => p.Name.Contains("o") || p.Age <= 25);

            bool result = AreCollectionsEquals(persons, expected);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldBeEqualsWithChainedWheres()
        {
            var target = new FilterExpression<Person>();

            target.Start(p => p.Name.Contains("o")).And(p => p.Age <= 25);

            var persons = peoples.Where(target.ResultExpression);

            var expected = peoples.Where(p => p.Name.Contains("o"));
            expected = expected.Where(p => p.Age <= 25);

            bool result = AreCollectionsEquals(persons, expected);
            Assert.IsTrue(result);
        }

        private bool AreCollectionsEquals(IEnumerable<Person> persons, IEnumerable<Person> expected)
        {
            if (persons.Count() != expected.Count()) return false;
            for (int i = 0; i < persons.Count(); i++)
            {
                if (!persons.ElementAt(i).Equals(expected.ElementAt(i)))
                    return false;
            }
            return true;
        }
    }
}
