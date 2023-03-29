using System;

namespace Triangle // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(CheckTriangle(args));
        }

        public static string CheckTriangle(string[] sides)
        {
            try
            {
                if (sides.Length != 3) throw new NotTriangleException();
                double a = TryParseStringToDouble(sides[0]);
                double b = TryParseStringToDouble(sides[1]);
                double c = TryParseStringToDouble(sides[2]);

                if (a < 0 || b < 0 || c < 0) throw new UncnownException();
                if (a == 0 || b == 0 || c == 0) throw new NotTriangleException();
                if ((a >= b + c && a != b && a != c) ||
                    (b >= a + c && b != a && b != c) ||
                    (c >= a + b && c != a && c != b)) throw new NotTriangleException();
                if (a == b && b == c) return "Равносторонний";
                else if (a == b || b == c || a == c) return "Равнобедренный";
                else return "Обычный";

            }
            catch (NotTriangleException exception) { return exception.Message; }
            catch (UncnownException exception) { return exception.Message; }
            catch (Exception) { return "Error"; }
        }

        public static double TryParseStringToDouble(string arg)
        {
            if (double.TryParse(arg, out double result)) return result;

            throw new UncnownException();
        }
    }
}