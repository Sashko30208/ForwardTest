using System;
using testPr.Engine;
using testPr.Engine.Impl;

namespace testPr
{
    class Program
    {
        static int Main(string[] args)
        {
            int _hasError = 0;
            try
            {
                Console.Write("Введите температуру воздуха: ");
                string input = Console.ReadLine();
                double roomTemp = 10;
                if (!Double.TryParse(input.Replace(".", ","), out roomTemp))
                { throw new Exception($"Не удалось перевести {input} в число"); }

                IEngine engine = new DVSEngine(roomTemp);
                engine.Simulate();
            }
            catch (Exception e)
            {
                _hasError = 1;
                Console.WriteLine(e.Message);
            }
            Console.Write("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
            return _hasError;
        }
    }
}