using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Task.Data;

namespace Linq.Tests
{
    [TestClass]
    public class LinqTests
    {
        private readonly DataSource dataSource = new DataSource();

        [TestMethod]
        public void Linq_Task1()
        {
            int lowerLimit = 21000;

            System.Collections.Generic.IEnumerable<Customer> result = dataSource.Customers.Where(customer => customer.Orders.Sum(order => order.Total) > lowerLimit);

            result.ToList().ForEach(customer => Console.WriteLine($@"{customer.CustomerID} - {customer.Orders.Sum(x => x.Total)}"));


        }

        [TestMethod]
        public void Linq_Task2()
        {
            var groupJoinResult = dataSource.Customers.GroupJoin(dataSource.Suppliers,
            customer => new { customer.City, customer.Country },
            supplier => new { supplier.City, supplier.Country },
            (customer, suppliers) => new
            {
                customer.CustomerID,
                Suppliers = suppliers.ToList()
            }).Where(r => r.Suppliers.Any()).ToList();

            var resultWithoutGroup = dataSource.Customers.Select(customer => new
            {
                customer.CustomerID,
                Suppliers = dataSource.Suppliers
                    .Where(supplier => supplier.City == customer.City &&
                    supplier.Country == customer.Country).ToList()
            }).Where(x => x.Suppliers.Any()).ToList();

            var joinWithGroupResult = dataSource.Customers.Join(dataSource.Suppliers,
            customer => new { customer.City, customer.Country },
            supplier => new { supplier.City, supplier.Country },
            (customer, supplier) => new
            {
                customer.CustomerID,
                Supplier = supplier
            }).GroupBy(r => r.CustomerID).Select(x => new
            {
                CustomerID = x.Key,
                Suppliers = x.Select(y => y.Supplier).ToList()
            }).ToList();

            foreach (var customer in groupJoinResult)
            {
                Console.WriteLine(customer.CustomerID);
                foreach (Supplier supplier in customer.Suppliers)
                {
                    Console.WriteLine($"{supplier.SupplierName} : {supplier.Country}, {supplier.City}");
                }
            }
        }



        [TestMethod]
        public void Linq_Task3()
        {
            int sum = 10000;
            var result = 
                dataSource.Customers.Where(
                    customer => customer.Orders.Any(order => order.Total > sum)
                    ).ToList();
            foreach (var customer in result)
            {
                foreach (var order in customer.Orders.Where(order => order.Total > sum))
                {
                    Console.WriteLine($"{customer.CompanyName}  -  {order.Total}");
                }
            }


        }


        [TestMethod]
        public void Linq_Task4()
        {

            var customers = dataSource.Customers.Where(customer => customer.Orders.Any()).Select(customer => new
            {
                customer.CompanyName,
                DataStart = customer.Orders.Min(order => order.OrderDate)
            }
            ).ToList();

            foreach (var cust in customers)
            {
                Console.WriteLine($"{cust.CompanyName} -  {cust.DataStart}");
            }



        }

        [TestMethod]
        public void Linq_Task5()
        {

            var customers = dataSource.Customers.Where(customer => customer.Orders.Any()).Select(customer => new
            {
                customer.CompanyName,
                OrdersTotal = customer.Orders.Sum(order => order.Total),
                DataStart = customer.Orders.Min(order => order.OrderDate)
            }
            ).ToList()
            .OrderBy(customer => customer.DataStart.Year)
            .ThenBy(customer => customer.DataStart.Month)
            .ThenByDescending(customer => customer.OrdersTotal)
            .ThenBy(customer => customer.CompanyName);

            foreach (var item in customers)
            {
                Console.WriteLine($"{item.CompanyName} : {item.OrdersTotal} - {item.DataStart}");
            }



        }

        private bool isRequirmentsTrue(Customer customer)
        {
            bool isPostalCodeContainsOnlyDigit = customer.PostalCode?.All(char.IsDigit) ?? false;
            bool isRegionEmpty = string.IsNullOrEmpty(customer.Region);
            bool isPhoneContainsOperatorCode = customer.Phone.First() == '(';

            return !isPhoneContainsOperatorCode || !isRegionEmpty || !isPostalCodeContainsOnlyDigit;
        }

        [TestMethod]
        public void Linq_Task6()
        {

            var customers = dataSource.Customers.Where(customer => isRequirmentsTrue(customer));
            foreach (var item in customers)
            {
                Console.WriteLine($"{item.CompanyName}: {item.Region = "No region" } {item.PostalCode}, {item.Phone} ");
            }

        }

        [TestMethod]
        public void Linq_Task7()
        {

            var result = dataSource.Products.GroupBy(product => product.Category, (category, item) => new
            {
                Category = category,
                UnitsInStock = item.GroupBy(product => product.UnitsInStock, (unitsCount, products) => new
                {
                    Count = unitsCount,
                    Products = products.OrderByDescending(p => p.UnitPrice).ToList()
                }).ToList()
            });

            foreach (var item in result)
            {
                Console.WriteLine($"Category: {item.Category}");
                foreach (var unit in item.UnitsInStock)
                {
                    Console.WriteLine($"Count: {unit.Count}");
                    foreach (Product product in unit.Products)
                    {
                        Console.WriteLine(product.ProductName);
                    }
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void Linq_Task8()
        {
            int averageLimit = 20;
            int expensiveLimit = 50;

            var result = dataSource.Products.GroupBy(
                product => product.UnitPrice < averageLimit ? "Cheap products" :
                product.UnitPrice >= averageLimit &&
                product.UnitPrice < expensiveLimit ? "Average products" : "Expensive products",
                (category, products) => new
                {
                    Category = category,
                    Products = products.OrderBy(product => product.UnitPrice).ToList()
                });

            foreach (var item in result)
            {
                Console.WriteLine(item.Category);
                Console.WriteLine();
                foreach (var product in item.Products)
                {
                    Console.WriteLine($"{product.ProductName} -   {product.UnitPrice}");
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void Linq_Task9()
        {

            var result = dataSource.Customers.GroupBy(customer => customer.City, (city, customers) => new
            {
                City = city,
                averageProfability = customers.Average(customer => customer.Orders.Sum(order => order.Total)),
                averageIntensivity = customers.Average(customer => customer.Orders.Length)
            });

            foreach(var item in result)
            {
                Console.WriteLine($"city: {item.City}  average profitability: {item.averageProfability}   average intensivity:  {item.averageIntensivity}");
            }
        }


        [TestMethod]
        public void Linq_Task10()
        {
            var orders = dataSource.Customers.SelectMany(customer => customer.Orders).ToList();


            var statsByYearAndMonth = orders.GroupBy(order => new
            {
                Year = order.OrderDate.ToString("yyyy"),
                Month = order.OrderDate.ToString("MM")
            },
            (date,count) => new
            {
                Date=$"{ date.Year}-{date.Month}",
                CountOrders=count.Count()
            }).OrderBy(r=>r.Date).ToList();

            Console.WriteLine("List by year and month:");
            foreach(var item in statsByYearAndMonth)
            {
                Console.WriteLine($"{item.Date}   {item.CountOrders}");
            }


            var statsByYear = orders.GroupBy(order => order.OrderDate.ToString("yyyy"), 
            (year,count) => new
            {
                Year = year,
                Count = count.Count()
            }).OrderBy(r => r.Year).ToList();

            Console.WriteLine("List by year");
            foreach (var item in statsByYear)
            {
                Console.WriteLine($"{item.Year}   {item.Count}");
            }


            var statsByMonth = orders.GroupBy(order => order.OrderDate.ToString("MMMMMM"),
           (month, count) => new
           {
               Month = month,
               Count = count.Count()
           }).OrderBy(r => r.Month).ToList();

            Console.WriteLine("List by month");
            foreach (var item in statsByMonth)
            {
                Console.WriteLine($"{item.Month}   {item.Count}");
            }

        }

    }
}


