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

            var customers= dataSource.Customers.Where(customer => customer.Orders.Sum(order => order.Total) > lowerLimit);

            customers.ToList().ForEach(cust => Console.WriteLine($"{cust.CustomerID} - {cust.Orders.Sum(ord => ord.Total)}"));


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
                Suppliers = suppliers
            }).Where(r => r.Suppliers.Any()).ToList();

            var resultWithoutGroup = dataSource.Customers
                .Select(customer => new
                {
                customer.CustomerID,
                Suppliers = dataSource.Suppliers.Where(supplier => 
                supplier.City == customer.City && supplier.Country == customer.Country)
                })
                .Where(x => x.Suppliers.Any()).ToList();

                var joinWithGroupResult = dataSource.Customers.Join(dataSource.Suppliers,
                customer => new { customer.City, customer.Country },
                supplier => new { supplier.City, supplier.Country },
                (customer, supplier) => new
                {
                    customer.CustomerID,
                    supplier = supplier
                })
                .GroupBy(r => r.CustomerID).Select(
                x => new
                {
                CustomerID = x.Key,
                Suppliers = x.Select(y => y.supplier)
            }).ToList();

            foreach (var customer in groupJoinResult)
            {
                Console.WriteLine(customer.CustomerID);
                foreach (var supplier in customer.Suppliers)
                {
                    Console.WriteLine($"{supplier.SupplierName} : {supplier.Country}, {supplier.City}");
                }
            }
        }



        [TestMethod]
        public void Linq_Task3()   
        {
            int sum = 10000;
            var customers =
                dataSource.Customers.Where(
                    customer => customer.Orders.Any(order => order.Total > sum)
                    ).ToList();
            foreach (var customer in customers)
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
                dataStart = customer.Orders.Min(order => order.OrderDate)
            }
            ).ToList();

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CompanyName} -  {customer.dataStart}");
            }



        }

        [TestMethod]
        public void Linq_Task5()
        {

            var customers = dataSource.Customers.Where(customer => customer.Orders.Any()).Select(customer => new
            {
                customer.CompanyName,
                ordersTotal = customer.Orders.Sum(order => order.Total),
                dataStart = customer.Orders.Min(order => order.OrderDate)
            }
            ).ToList()
            .OrderBy(customer => customer.dataStart.Year)
            .ThenBy(customer => customer.dataStart.Month)
            .ThenByDescending(customer => customer.ordersTotal)
            .ThenBy(customer => customer.CompanyName);

            foreach (var item in customers)
            {
                Console.WriteLine($"{item.CompanyName} : {item.ordersTotal} - {item.dataStart}");
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
            string region = String.Empty;
            foreach (var item in customers)
            {
                if (string.IsNullOrEmpty(item.Region)) region = "No region";
                else region = item.Region;
                Console.WriteLine($"{item.CompanyName}:  {region}  {item.PostalCode}, {item.Phone} ");
            }

        }

        [TestMethod]
        public void Linq_Task7()
        {

            var products = dataSource.Products.GroupBy(product => product.Category, (category, item) => new
            {
                category,
                unitsInStock = item.GroupBy(product => product.UnitsInStock, (count, prod) => new
                {
                    count,
                    products = prod.OrderByDescending(p => p.UnitPrice)
                }).ToList()
            });

            foreach (var product in products)
            {
                Console.WriteLine($"Category: {product.category}");
                foreach (var unit in product.unitsInStock)
                {
                    Console.WriteLine($"Count: {unit.count}");
                    foreach (var item in unit.products)
                    {
                        Console.WriteLine(item.ProductName);
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
            var products = dataSource.Products.GroupBy(
                product => product.UnitPrice < averageLimit ? "Cheap products" :
                product.UnitPrice >= averageLimit &&
                product.UnitPrice < expensiveLimit ? "Average products" : "Expensive products",
                (category, prod) => new
                {
                    Category = category,
                    Products = prod.OrderBy(product => product.UnitPrice).ToList()
                });

            foreach (var item in products)
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

            var averageEffectvityEachCity = dataSource.Customers.GroupBy(customer => customer.City, (city, customers) => new
            {
                city,
                averageProfability = customers.Average(customer => customer.Orders.Sum(order => order.Total)),
                averageIntensivity = customers.Average(customer => customer.Orders.Length)
            });

            foreach(var item in averageEffectvityEachCity)
            {
                Console.WriteLine($"city: {item.city}  average profitability: {item.averageProfability}   average intensivity:  {item.averageIntensivity}");
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


