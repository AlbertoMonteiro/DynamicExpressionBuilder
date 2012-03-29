using System.Collections.Generic;

namespace DynamicExpressionBuilderTest
{
    public class Person
    {
        public Person(string name, int age, bool working, params Task[] tasks)
        {
            Name = name;
            Age = age;
            Working = working;
            Tasks = new List<Task>(tasks);
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public bool Working { get; set; }
        public List<Task> Tasks { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Person && Equals((Person) obj);
        }

        public bool Equals(Person person)
        {
            return person.Name.Equals(Name) && person.Age.Equals(Age) && person.Working.Equals(Working);
        }

        public override string ToString()
        {
            return string.Format("Name: {0} - Age: {1} - Working: {2}", Name, Age, Working);
        }
    }
}