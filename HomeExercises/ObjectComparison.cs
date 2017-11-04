using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			actualTsar.ShouldBeEquivalentTo(expectedTsar,options => options
			.Excluding(o => o.Id)
			.Excluding( o => o.Parent.Id));


		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));

			//Метод AreEqual в его текущей реализации имеет две проблемы: он нерасширяем и
			//у него нет понятных сообщений об ошибках. Для решения проблемы с расширяемостью
			//скорее всего пришлось бы использовать рефлексию, но тогда тест будет слишком сложным
			//для понимания и, кроме того, его вероятно тоже пришлось бы тестировать на ошибки.

			//В моем решении я использую метод ShouldBeEquivalentTo() из Fluent Assertions.
			//Он умеет итерироваться по полям класса, следовательно его скорее всего не понадобится
			//переписывать при добавлении новых полей объекту (исключение составляют поля типа Id, они
			//отличаются у любых объектов, даже тех, которые нам удобно считать одинаковыми)

			//Кроме того, в случае ошибки этот метод покажет, какие именно поля отличались, этим
			//он лучше, чем тест основанный на AreEquals, который лишь говорил что метод вернул false вместо true
		}

		private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
			actual.Name == expected.Name
			&& actual.Age == expected.Age
			&& actual.Height == expected.Height
			&& actual.Weight == expected.Weight
			&& AreEqual(actual.Parent, expected.Parent);
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}
