using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises {
	public class NumberValidatorTests {
		[TestCase("",17,2,true,ExpectedResult = false,TestName = "Should return false when value is empty")]
		[TestCase(null, 17, 2, true, ExpectedResult = false, TestName = "Should return false when value is null")]
		[TestCase("++1", 17, 2, true, ExpectedResult = false, TestName = "Should return false when value contains two pluses on beginning")]
		[TestCase("1,2.3", 17, 2, true, ExpectedResult = false, TestName = "Should return false when value is contains several separators")]
		[TestCase("1.2+", 17, 2, true, ExpectedResult = false, TestName = "Should return false when value contains non-digit on the end ")]
		[TestCase("1.345", 17, 2, true, ExpectedResult = false, TestName = "Should return false when fraction is longer than scale")]
		[TestCase("12345,6", 5, 2, true, ExpectedResult = false, TestName = "Should return false when value is longer than precision")]
		[TestCase("-12,3", 17, 2, true, ExpectedResult = false, TestName = "Should return false when onlyPositive but value is negative")]
		[TestCase("a.sd", 3, 2, true, ExpectedResult = false, TestName = "Should return false when there are letters instead of numbers")]
		[TestCase("0.000", 17, 2, true, ExpectedResult = false, TestName = "Should return false when number of zeros if fraction exceeds scale")]
		[TestCase("00.00", 3, 2, true, ExpectedResult = false, TestName = "Should return false when all digits are zeros, but their number exceeds precision")]
		[TestCase("-0.00", 3, 2, true, ExpectedResult = false, TestName = "Should return false when number is negative zero and it's length is greater than precision")]
		[TestCase("+0.00", 3, 2, true, ExpectedResult = false, TestName = "Should return false when number is positive zero and it's length is greater than precision")]
		[TestCase("+1.23", 3, 2, true, ExpectedResult = false, TestName = "Should return false when number is negative and it's length is greater than precision")]
		[TestCase("-1.23", 3, 2, true, ExpectedResult = false, TestName = "Should return false when number is positive and it's length is greater than precision")]

		[TestCase("12", 17, 2, true, ExpectedResult = true, TestName = "Should return true when value is integer")]
		[TestCase("1.2", 17, 2, true, ExpectedResult = true, TestName = "Should return true when value has dot separator")]
		[TestCase("1,2", 17, 2, true, ExpectedResult = true, TestName = "Should return true when value has comma separator")]
		[TestCase("0.0", 17, 2, true, ExpectedResult = true, TestName = "Should return true when zero integer with zero fraction")]
		[TestCase("0", 17, 2, true, ExpectedResult = true, TestName = "Should return true when zero")]
		[TestCase("+1.23", 4, 2, true, ExpectedResult = true, TestName = "Should return true when number has plus")]


		public bool isValidNumber(string value, int precision, int scale = 0, bool onlyPositive = false) {
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}


		[TestCase(-1, 2, false, TestName = "Should throw when precision is negative")]
		[TestCase(0, 1, true, TestName = "Should throw when precision is zero")]
		[TestCase(1, -1, true, TestName = "Should throw when scale is negative")]
		[TestCase(2, 2, true, TestName = "Should throw when scale is equal to precision")]
		[TestCase(1, 2, true, TestName = "Should throw when scalse is greater than precision")]
		public void ShouldThrowWhenIntializedWithInvalidData(int precision, int scale, bool onlyPositive) {
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void ShouldNotThrowWhenInitializedWithValidData() {
			Action act = () => new NumberValidator(1, 0, true);

			act.ShouldNotThrow();
		}
	}

	public class NumberValidator {
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false) {
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value) {
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}