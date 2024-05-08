using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class MenuService
    {
        private readonly IMongoCollection<Menu> _menuItems;
        private readonly IMapper _mapper;

        public MenuService(IMongoDatabase database, IMapper mapper)
        {
            _menuItems = database.GetCollection<Menu>("MenuItems");
            _mapper = mapper;
        }

        public async Task AddMenuItemAsync(MenuDto menuItemDTO)
        {
            var menuItem = _mapper.Map<Menu>(menuItemDTO);
            await _menuItems.InsertOneAsync(menuItem);
        }

        public async Task<bool> UpdateMenuItemAsync(string id, MenuDto menuItemDTO)
        {
            var menuItem = _mapper.Map<Menu>(menuItemDTO);

            var result = await _menuItems.ReplaceOneAsync(item => item.Id == id, menuItem);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteMenuItemAsync(string id)
        {
            var result = await _menuItems.DeleteOneAsync(item => item.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<MenuDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuItems.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<MenuDto>>(menuItems);
        }

        public async Task<MenuDto> GetMenuItemByIdAsync(string id)
        {
            var menuItem = await _menuItems.Find(item => item.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<MenuDto>(menuItem);
        }
    }
}
