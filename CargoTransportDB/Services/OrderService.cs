using CargoTransportation.Data;
using CargoTransportation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CargoTransportation.Services
{
    public class OrderService
    {
        private readonly CargoDbContext _context;

        public OrderService()
        {
            _context = new CargoDbContext();
        }

        public List<Order> GetAllOrders()
        {
            try
            {
                return _context.Orders
                    .Include(x => x.Client)
                    .Include(x => x.Driver)
                    .Include(x => x.Vehicle)
                    .Include(x => x.Cargo)
                    .Include(x => x.Status)
                    .OrderByDescending(x => x.OrderDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllOrders Error: {ex.Message}");
                return new List<Order>();
            }
        }

        public bool AddOrder(Order order)
        {
            try
            {
                order.OrderNumber = GenerateOrderNumber();
                order.OrderDate = DateTime.Now;
                order.StatusID = 1;
                _context.Orders.Add(order);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddOrder Error: {ex.Message}");
                return false;
            }
        }

        public bool DeleteOrder(int orderId)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteOrder Error: {ex.Message}");
                return false;
            }
        }

        public List<Order> FilterOrders(DateTime? fromDate, DateTime? toDate, int? statusId, string searchText)
        {
            try
            {
                var query = _context.Orders
                    .Include(x => x.Client)
                    .Include(x => x.Driver)
                    .Include(x => x.Vehicle)
                    .Include(x => x.Cargo)
                    .Include(x => x.Status)
                    .AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(x => x.OrderDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(x => x.OrderDate <= toDate.Value);

                if (statusId.HasValue && statusId.Value > 0)
                    query = query.Where(x => x.StatusID == statusId.Value);

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.OrderNumber.Contains(searchText) ||
                                            x.PickupAddress.Contains(searchText) ||
                                            x.DeliveryAddress.Contains(searchText));

                return query.OrderByDescending(x => x.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FilterOrders Error: {ex.Message}");
                return new List<Order>();
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }
}