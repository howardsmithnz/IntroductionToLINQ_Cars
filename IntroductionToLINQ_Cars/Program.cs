using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroductionToLINQ_Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var records = ProcessCars("fuel.csv");

            var document = new XDocument();
            var cars = new XElement("Cars");

            foreach (var record in records)
            {
                var name = new XAttribute("Name", record.Name);
                var combined = new XAttribute("Combined", record.Combined);
                var car = new XElement("Car", name, combined);

                cars.Add(car);
            }

            document.Add(cars);
            document.Save("fuel.xml");

            //var manufacturers = ProcessManufacturers("manufacturers.csv");

            //var query = cars.OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name);

            //var query =
            //    from car in cars
            //    //where car.Manufacturer == "BMW" && car.Year == 2016
            //    join manufacturer in manufacturers 
            //        on car.Manufacturer equals manufacturer.Name
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        manufacturer.Headquarters,
            //        car.Name,
            //        car.Combined
            //    };

            //var query2 =
            //    cars.Join(manufacturers,
            //                c => c.Manufacturer,
            //                m => m.Name, 
            //                (c, m) => new
            //                {
            //                    m.Headquarters,
            //                    c.Name,
            //                    c.Combined
            //                })
            //        .OrderByDescending(c => c.Combined)
            //        .ThenBy(c => c.Name);

            //var query3 =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper() into manufacturer
            //    orderby manufacturer.Key
            //    select manufacturer;

            //var query4 =
            //    cars.GroupBy(c => c.Manufacturer.ToUpper())
            //        .OrderBy(g => g.Key);

            //var query5 =
            //    from manufacturer in manufacturers
            //    join car in cars on manufacturer.Name equals car.Manufacturer
            //        into carGroup
            //    select new
            //    {
            //        Manufacturer = manufacturer,
            //        Cars = carGroup
            //    };

            //var query6 =
            //    manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, 
            //        (m, g) =>
            //        new
            //        {
            //          Manufacturer = m,
            //          Cars = g
            //        })
            //    .OrderBy(m => m.Manufacturer.Name);

            //var query7 =
            //    from car in cars
            //    group car by car.Manufacturer into carGroup
            //    select new
            //    {
            //        Name = carGroup.Key,
            //        Max = carGroup.Max(c => c.Combined),
            //        Min = carGroup.Min(c => c.Combined),
            //        Avg = carGroup.Average(c => c.Combined)
            //    };

            //var query8 =
            //    cars.GroupBy(c => c.Manufacturer)
            //        .Select(g => 
            //        {
            //            var results = g.Aggregate(new CarStatistics(),
            //                                (acc, c) => acc.Accumulate(c),
            //                                acc => acc.Compute());
            //            return new
            //            {
            //                Name = g.Key,
            //                Avg = results.Average,
            //                Min = results.Min,
            //                Max = results.Max
            //            };
            //        })
            //        .OrderByDescending(r => r.Max);

            //foreach (var result in query8)
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\t Max: {result.Max}");
            //    Console.WriteLine($"\t Min: {result.Min}");
            //    Console.WriteLine($"\t Avg: {result.Avg}");
            //}

            //foreach (var group in query6)
            //{
            //    Console.WriteLine($"{group.Manufacturer.Name}:{group.Manufacturer.Headquarters}");
            //    foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}

            //foreach (var group in query4)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}


            //var top =
            //    cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
            //        .OrderByDescending(c => c.Combined)
            //        .ThenBy(c => c.Name)
            //        .Select(c => c)
            //        .First();

           // Console.WriteLine(top.Name);

            //foreach (var car in query2.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                File.ReadAllLines(path)
                    .Where(l => l.Length > 1)
                    .Select(l =>
                    {
                        var columns = l.Split(',');
                        return new Manufacturer
                        {
                            Name = columns[0],
                            Headquarters = columns[1],
                            Year = int.Parse(columns[2])
                        };
                    });
            return query.ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            //return 
            //    File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(Car.ParseFromCsv)
            //    .ToList();

            var query =
                File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToCar();

            return query.ToList();
        }
    }

    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }

        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);

            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;

            return this;
        }

        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
