using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public MenuService(IMongoDatabase database)
        {
            _menuCollection = database.GetCollection<Menu>("Menus");
        }

        public async Task<List<Menu>> GetAsync() =>
            await _menuCollection.Find(_ => true).ToListAsync();

        public async Task<Menu?> GetByIdAsync(string id) =>
            await _menuCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Menu newMenu) =>
            await _menuCollection.InsertOneAsync(newMenu);

        public async Task UpdateAsync(string id, Menu updatedMenu) =>
            await _menuCollection.ReplaceOneAsync(x => x.Id == id, updatedMenu);

        public async Task RemoveAsync(string id) =>
            await _menuCollection.DeleteOneAsync(x => x.Id == id);
    }
}
