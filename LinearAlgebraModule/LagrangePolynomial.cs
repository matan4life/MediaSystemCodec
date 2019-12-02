using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearAlgebraModule
{
    public static class LagrangePolynomial
    {
        public static double CalculateLagrangePolynomialValue(double argument, IDictionary<double, double> values)
        {
            return values
                .Select(x => x.Value * EvaluateBasicPolynom(argument, values
                    .Keys
                    .OrderBy(y => y)
                    .ToList(), values
                    .Keys
                    .OrderBy(y => y)
                    .ToList()
                    .IndexOf(x.Key)))
                .Sum();
        }

        private static double EvaluateBasicPolynom(double argument, List<double> arguments, int index)
        {
            return arguments.Select(x => arguments.IndexOf(x) != index ? (argument - x) / (arguments[index] - x) : 1).Aggregate((x, y) => x * y);
        }
    }
}
