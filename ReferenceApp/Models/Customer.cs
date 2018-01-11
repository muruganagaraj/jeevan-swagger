using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ReferenceApp.Models
{
    public class Customer
    {
        private readonly List<Order> _orders = new List<Order>();

        [Required]
        public string Code { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public decimal Credit { get; set; }

        public CustomerType CustomerType { get; set; }

        public IList<Order> Orders
        {
            [DebuggerStepThrough]
            get { return _orders; }
        }
    }

    public enum CustomerType
    {
        Regular,
        Premium,
        SpecialDiscount,
    }
    public sealed class Order
    {
        private readonly List<Product> _products = new List<Product>();

        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }

        public IList<Product> Products
        {
            [DebuggerStepThrough]
            get { return _products; }
        }
    }

    public sealed class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}