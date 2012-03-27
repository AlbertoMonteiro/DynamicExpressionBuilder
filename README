## Dynamic Lambda Functions Builder

Help you to create functions when you have optional parameters

Example:

```csharp
	
	List<Person> _peoples = new List<Person>
              {
                  new Person("Alberto", 22, true),
                  new Person("Fabio", 24, false),
                  new Person("Abraão", 23, false),
                  new Person("Pedro", 26, false)
              };
	
	var target = new FilterExpression<Person>();

	var filterExpression = target
		.Start(p => p.Name.Contains("o"))
		.And(p => p.Age <= 25, false);
		.And(p => p.Age >= 26, true);

	_peoples.Where(filterExpression.ResultExpression) == _peoples.Where(p1 => p1.Name.Contains("o") && p.Age >= 26);
	
```