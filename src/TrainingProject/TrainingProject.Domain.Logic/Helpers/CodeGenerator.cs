using System;
using System.Globalization;

namespace TrainingProject.DomainLogic.Helpers
{
    public static class CodeGenerator
    {
        private static readonly Random Random = new Random();

        public static string GenerateCode(int numbers)
        {
            if (numbers < 1)
            {
                throw new ArgumentException(nameof(numbers));
            }

            var divider = (int) Math.Pow(10, numbers);
            var number = Random.Next() % divider;
            var stringNumber = number.ToString(CultureInfo.InvariantCulture).PadLeft(numbers, '0');

            return stringNumber;
        }
    }
}