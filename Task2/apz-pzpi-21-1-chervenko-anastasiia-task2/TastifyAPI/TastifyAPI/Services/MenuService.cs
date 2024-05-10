using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMongoCollection<Menu> _menuCollection;

        public MenuService(IMongoDatabase database)
        {
            _menuCollection = database.GetCollection<Menu>("Menu");
        }

        public async Task<List<Menu>> GetAsync() =>
            await _menuCollection.Find(_ => true).ToListAsync();

        public async Task<List<Menu>> GetRestaurantMenuAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.RestaurantId == restaurantId).ToListAsync();


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


    }
}

