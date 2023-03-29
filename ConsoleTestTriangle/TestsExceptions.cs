using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangle
{
    public class IncorrectCountArgumentException : Exception
    {
        public IncorrectCountArgumentException(string message = "Неверное количество аргументов") : base(message) { }
    }
    public class InvalidFilePathException : Exception
    {
        public InvalidFilePathException(string message = "Файла нет или он не доступен") : base(message) { }
    }
    public class InvalidExpectedResponseException : Exception
    {
        public InvalidExpectedResponseException(string message = "Неизвесный ожидаемый ответ") : base(message) { }
    }
}