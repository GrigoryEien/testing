using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{

    public class NumberValidatorTests
    {
        [TestCase("0.0",17, 2, true, ExpectedResult = true, TestName = "0.0")]
        [TestCase("0", 17, 2, true, ExpectedResult = true, TestName = "Zero")]
        [TestCase("00.00", 3, 2, true, ExpectedResult = false, TestName = "Leading zeros are accounted")]
        [TestCase("-0.00", 3, 2, true, ExpectedResult = false, TestName = "Leading minus before zero is accounted")]
        [TestCase("+0.00", 3, 2, true, ExpectedResult = false, TestName = "Leading plus before zero is accounted")]
        [TestCase("+1.23", 4, 2, true, ExpectedResult = true, TestName = "Leading plus, length is suitable")]
        [TestCase("+1.23", 3, 2, true, ExpectedResult = false, TestName = "Leading plus before non-zero digit is accounted")]
        [TestCase("0.000", 17, 2, true, ExpectedResult = false, TestName = "Several zeros in fraction, length is suitable")]
        [TestCase("-1.23", 3, 2, true, ExpectedResult = false, TestName = "Leading minus before non-zero digit is accounted")]
        [TestCase("a.sd", 3, 2, true, ExpectedResult = false, TestName = "Letters instead of numbers")]

        public bool isValidNumber(string value, int precision, int scale = 0, bool onlyPositive = false)
        {
            return new NumberValidator(precision,scale,onlyPositive).IsValidNumber(value);
        }


        [TestCase(-1,2,true,TestName = "Negative length is not accepted when numbers should be positive only")]
        [TestCase(-1,2,false, TestName = "Negatie length is not accepted even when negative numbers allowed")]
        public void ThrowsExceptionsWhenNumverValidatorInitializedWithNegativeNumberLength(int precision, int scale, bool onlyPositive)
        {
            Assert.That(() => new NumberValidator(precision,scale,onlyPositive), Throws.TypeOf<ArgumentException>());

        }

        [TestCase(1, 0, true,TestName = "Validator with integer only numbers is accepted")]
        public void DoesNotTrhowExceptionWhenNumverValidatorInitializedWithValidData(int precision, int scale, bool onlyPositive)
        {
            new NumberValidator(precision, scale, onlyPositive);

        }

    }

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
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