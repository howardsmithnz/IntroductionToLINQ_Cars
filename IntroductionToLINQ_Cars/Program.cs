using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroductionToLINQ_Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFiles("fuel.csv");
            foreach (var car in cars)
            {
                Console.WriteLine(car.Name);
            }
        }

        private static List<Car> ProcessFiles(string path)
        {
            return 
                File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .Select(Car.ParseFromCsv)
                .ToList();
        }
    }
}
