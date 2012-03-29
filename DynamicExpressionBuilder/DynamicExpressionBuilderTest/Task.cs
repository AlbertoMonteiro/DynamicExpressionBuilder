using System;

namespace DynamicExpressionBuilderTest
{
    public class Task
    {
        public Task(string title, DateTime when)
        {
            Title = title;
            When = when;
        }

        public string Title { get; set; }
        public DateTime When { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Task && Equals((Task)obj);
        }

        public bool Equals(Task task)
        {
            return Title == task.Title && When == task.When;
        }
    }
}
