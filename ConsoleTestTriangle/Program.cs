using System.Diagnostics;
using System.Text;
using System.Linq;
using Triangle;
using static System.Net.Mime.MediaTypeNames;
using System;

namespace ConsoleTestTriangle
{
    internal class Program
    {
        class TestCaseTriangle
        {
            string _args;
            string _expectedResponse;

            public string Args {get {return _args;}}
            public TestCaseTriangle(string testString)
            {
                string[] strings = testString.Split(' ');
                switch (strings[^1])
                {
                    case "Обычный":
                    case "Равнобедренный":
                    case "Равносторонний":
                        _expectedResponse = strings[^1].ToLower();
                        _args = string.Join(' ', strings[0..^1]);
                        break;
                    case "треугольник":
                        _expectedResponse = string.Join(' ', strings[^2..^0]).ToLower();
                        if (_expectedResponse != "не треугольник".ToLower()) throw new InvalidExpectedResponseException();
                        _args = string.Join(' ', strings[0..^2]);
                        break;
                    case "ошибка":
                        _expectedResponse = string.Join(' ', strings[^2..^0]).ToLower();
                        if (_expectedResponse != "неизвестная ошибка".ToLower()) throw new InvalidExpectedResponseException();
                        _args = string.Join(' ', strings[0..^2]);
                        break;
                    default:
                        throw new InvalidExpectedResponseException();
                }
            }
            public string CheckResponse(string response) => response.Trim().ToLower() == _expectedResponse ? "Secusses" : string.Format("Error:\n\tExpected: {0};\n\tResponse: {1};", _expectedResponse, response);
            //public override string ToString() => String.Format("args: {0}\nexpected: {1}", _args, _expectedResponse);
        }

        class ExecuterTestCase
        {
            private List<TestCaseTriangle> _testCases;
            private Process _process;
            private StreamWriter _output;

            public ExecuterTestCase(StreamWriter output)
            {
                _testCases ??= new(); //if (_testCases == null) _testCases = new(); - составной оператор назначения
                _process = new();
                _process.StartInfo.FileName = "triangle.exe";
                _process.StartInfo.RedirectStandardOutput = true;
                _output = output;
            }

            ~ExecuterTestCase()
            {
                _process.Close();
            }

            /*public void AddTests(string[] newTests)
            {
                foreach(string newTest in newTests) AddTest(newTest);
            }*/

            public void AddTest(string newTest)
            {
                _testCases.Add(new(newTest));
            }

            public void Execute()
            {
                while (_testCases.Count > 0)
                {
                    TestCaseTriangle testCase = _testCases[0];
                    _testCases.RemoveAt(0);

                    //Console.WriteLine(testCase.Args);

                    _process.StartInfo.Arguments = testCase.Args;
                    _process.Start();
                    _process.WaitForExit();
                    string response = _process.StandardOutput.ReadToEnd();
                    //Console.WriteLine(response);

                    _output.WriteLine(testCase.CheckResponse(response));
                }
            }
        }

        static void Main(string[] args)
        {
            StreamReader? tests = null;
            StreamWriter? output = null;
            ExecuterTestCase? executerTestCase = null;
            try
            {
                tests = GetTests(args);
                //output = new(DateTime.Now.ToString().Replace(':', '-') + " test.txt");
                output = new("test.txt");

                executerTestCase = new(output);

                string? test = "";

                while ((test = tests.ReadLine()) != null)
                {
                    if (test.Trim() == "") continue;
                    executerTestCase.AddTest(test);
                }

                executerTestCase.Execute();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                if (output != null) output.Close();
                if (tests != null) tests.Close();
            }
        }

        static StreamReader GetTests(string[] args)
        {
            if (args.Length != 1) throw new IncorrectCountArgumentException();
            if (!File.Exists(args[0])) throw new InvalidFilePathException();
            return new StreamReader(args[0]);
        }
    }
}