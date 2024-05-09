using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
/*using System.Threading.Tasks;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class PositionProductService : IPositionProductService
    {
        private readonly IMongoCollection<PositionProduct> _positionProductCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public PositionProductService(IMongoDatabase database, IMongoCollection<Product> productCollection)
        {
            _positionProductCollection = database.GetCollection<PositionProduct>("PositionProducts");
            _productCollection = productCollection;
        }

        public async Task<List<Product>> GetProductsByMenuIdAsync(string menuId)
        {
            var positionProducts = await _positionProductCollection.Find(x => x.MenuId == menuId).ToListAsync();
            if (positionProducts == null || !positionProducts.Any())
            {
                return null; // Если нет продуктов для указанного блюда, возвращаем null или пустой список
            }

            var productIds = positionProducts.Select(x => x.ProductId);
            var products = await _productCollection.Find(x => productIds.Contains(x.Id)).ToListAsync();

            return products;
        }
    }
}
*/

