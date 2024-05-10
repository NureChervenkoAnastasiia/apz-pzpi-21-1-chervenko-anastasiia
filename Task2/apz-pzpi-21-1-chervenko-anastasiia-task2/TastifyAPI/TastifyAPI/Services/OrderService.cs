using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TastifyAPI.Services
{

    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly IMongoCollection<Menu> _menuCollection;
        private readonly IMongoCollection<OrderItem> _orderItemCollection;
        private readonly ILogger<Order> _logger;

        public OrderService(IMongoDatabase database, IMongoCollection<Menu> menuCollection, IMongoCollection<OrderItem> orderItemCollection, ILogger<Order> logger)
        {
            _orderCollection = database.GetCollection<Order>("Orders");
            _menuCollection = menuCollection;
            _orderItemCollection = orderItemCollection;
            _logger = logger;
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


        public async Task<List<Order>> GetAllRestaurantOrdersAsync(string restaurantId)
        {
            var menu = await _menuCollection.Find(menu => menu.RestaurantId == restaurantId).FirstOrDefaultAsync();
            var orderItemsFromMenu = await _orderItemCollection.Find(orderItem => orderItem.MenuId == menu.Id).ToListAsync();
            var orderIds = orderItemsFromMenu.Select(order => order.OrderId);
            var orders = await _orderCollection.Find(order => orderIds.Contains(order.Id)).ToListAsync();
            return orders;
        }
        
        public async Task<List<Order>> GetAllRestaurantOrders1Async(string restaurantId)
        {
            try
            {
                _logger.LogInformation("Restaurant Id: {RestaurantId}", restaurantId);

                _logger.LogInformation("Fetching menus for restaurant {RestaurantId}", restaurantId);
                var menus = await _menuCollection.Find(menu => menu.RestaurantId == restaurantId).ToListAsync();

                _logger.LogInformation("Found {Count} menus for restaurant {RestaurantId}", menus.Count, restaurantId);

                var orderItemsFromMenus = new List<OrderItem>();

                foreach (var menu in menus)
                {
                    _logger.LogInformation("Fetching order items for menu {MenuId}", menu.Id);
                    var orderItems = await _orderItemCollection.Find(orderItem => orderItem.MenuId == menu.Id).ToListAsync();
                    orderItemsFromMenus.AddRange(orderItems);
                    _logger.LogInformation("Found {Count} order items for menu {MenuId}", orderItems.Count, menu.Id);
                }

                var orderIds = orderItemsFromMenus.Select(order => order.OrderId);

                _logger.LogInformation("Fetching orders for order ids: {OrderIds}", string.Join(", ", orderIds));

                var orders = await _orderCollection.Find(order => orderIds.Contains(order.Id)).ToListAsync();
                _logger.LogInformation("Found {Count} orders for restaurant {RestaurantId}", orders.Count, restaurantId);

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }
}
}
