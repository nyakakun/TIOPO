using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangle
{
    public class NotTriangleException : Exception
    {
        public NotTriangleException(string message = "Не треугольник") : base(message) { }
    }
    public class UncnownException : Exception
    {
        public UncnownException(string message = "Неизвестная ошибка") : base(message) { }
    }
}