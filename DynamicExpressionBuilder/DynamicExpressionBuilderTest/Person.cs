namespace DynamicExpressionBuilderTest
{
    public class Person
    {
        public Person(string name, int age, bool working)
        {
            Name = name;
            Age = age;
            Working = working;
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public bool Working { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Person)) return false;
            return Equals((Person) obj);
        }

        public override string ToString()
        {
            return string.Format("Name: {0} - Age: {1} - Working: {2}", Name, Age, Working);
        }

        public bool Equals(Person other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Age == Age && other.Working.Equals(Working);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Age;
                result = (result*397) ^ Working.GetHashCode();
                return result;
            }
        }
    }
}