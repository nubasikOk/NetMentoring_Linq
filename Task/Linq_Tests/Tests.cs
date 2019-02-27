using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task;
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
            var lowerLimit = 21000;

            var result = dataSource.Customers.Where(customer=> customer.Orders.Sum(order=>order.Total) > lowerLimit);

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

            foreach (var customer in resultWithoutGroup)
            {
                Console.WriteLine(customer.CustomerID);
                foreach (var supplier in customer.Suppliers)
                {
                    Console.WriteLine($@"{supplier.SupplierName} : {supplier.Country}, {supplier.City}");
                }
            }
        }

        
    }
}
