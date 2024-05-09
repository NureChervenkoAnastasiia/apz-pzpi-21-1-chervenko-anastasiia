using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMongoCollection<Menu> _menuCollection;
       // private readonly IMongoCollection<Product> _productCollection;

        public MenuService(IMongoDatabase database)
        {
            _menuCollection = database.GetCollection<Menu>("Menu");
        //    _productCollection = productCollection;
        }

        public async Task<List<Menu>> GetAsync() =>
            await _menuCollection.Find(_ => true).ToListAsync();

        public async Task<List<Menu>> GetRestaurantMenuAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Id == restaurantId).ToListAsync();

        public async Task<Menu?> GetByIdAsync(string id) =>
            await _menuCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Menu newMenu) =>
            await _menuCollection.InsertOneAsync(newMenu);

        public async Task UpdateAsync(string id, Menu updatedMenu) =>
            await _menuCollection.ReplaceOneAsync(x => x.Id == id, updatedMenu);

        public async Task RemoveAsync(string id) =>
            await _menuCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Menu>> GetFirstDishesForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Перші страви" && x.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<Menu>> GetSecondDishesForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Другі страви" && x.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<Menu>> GetDrinksForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Напій" && x.RestaurantId == restaurantId).ToListAsync();

       /* public async Task<List<Product>> GetPositionIngredientsAsync(string menuId)
        {
            var positionProductsCollection = _menuCollection.Database.GetCollection<PositionProduct>("PositionProducts");

            var positionProducts = await positionProductsCollection.Find(x => x.MenuId == menuId).ToListAsync();
            if (positionProducts == null || !positionProducts.Any())
            {
                return null; // Если нет продуктов для указанного блюда, возвращаем null или пустой список
            }

            var productIds = positionProducts.Select(x => x.ProductId);
            var ingredients = await _productCollection.Find(x => productIds.Contains(x.Id)).ToListAsync();

            return ingredients;
        }*/
    }
}

