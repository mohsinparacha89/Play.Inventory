using Play.Inventory.Models;
using Entitity = Play.Inventory.Data.Entity;
namespace Play.Inventory.Services
{
    public static class Extensions
    {
        public static InventoryItem AsDto(this Entitity.InventoryItem item, string name, string description)
        {
            return new InventoryItem()
            {
                AcquiredDate = item.AcquiredDate,
                CatalogItemId = item.CatalogItemId,
                Quantity = item.Quantity,
                Description = description,
                Name = name

            };
        }
    }
}
