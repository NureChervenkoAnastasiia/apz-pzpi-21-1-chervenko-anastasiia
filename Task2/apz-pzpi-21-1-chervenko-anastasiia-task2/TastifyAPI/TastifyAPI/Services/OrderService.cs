using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using TastifyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TastifyAPI.Services
{

    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orderCollection;
        //private readonly IMongoCollection<Menu> _menuCollection;
       // private readonly IMongoCollection<OrderItem> _orderItemCollection;

        public OrderService(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<Order>("Orders");
        }

        public async Task<List<Order>> GetAsync() =>
            await _orderCollection.Find(_ => true).ToListAsync();

        public async Task<Order?> GetByIdAsync(string id) =>
            await _orderCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Order newOrder) =>
            await _orderCollection.InsertOneAsync(newOrder);

        public async Task UpdateAsync(string id, Order updatedOrder) =>
            await _orderCollection.ReplaceOneAsync(x => x.Id == id, updatedOrder);

        public async Task RemoveAsync(string id) =>
            await _orderCollection.DeleteOneAsync(x => x.Id == id);

        /*
        public async Task<List<Order>> GetAllRestaurantOrdersAsync(string restaurantId)
        {
            var menu = await _menuCollection.Find(menu => menu.RestaurantId == restaurantId).FirstOrDefaultAsync();
            var orderItemsFromMenu = await _orderItemCollection.Find(orderItem => orderItem.MenuId == menu.Id).ToListAsync();
            var orderIds = orderItemsFromMenu.Select(order => order.OrderId);
            var orders = await _orderCollection.Find(order => orderIds.Contains(order.Id)).ToListAsync();
            return orders;
        }
        */
        
    }
}
