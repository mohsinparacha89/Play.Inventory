using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Data.Entity;
using Play.Inventory.Models;
using Play.Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItemDto = Play.Inventory.Models.InventoryItem;
using InventoryItem = Play.Inventory.Data.Entity.InventoryItem;
using Play.Inventory.Clients;

namespace Play.Inventory.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _itemsRepository;
        private readonly CatalogClient _catalogClient;
        public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
        {
            _itemsRepository = itemsRepository;
            _catalogClient = catalogClient;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var catalogItems = await _catalogClient.GetCatalogItemsAsync();
            var inventoryItems = await _itemsRepository.GetAllAsync(item => item.UserId == userId);

            var items = inventoryItems.Select(s =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == s.CatalogItemId);
                return s.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItems grantItem)
        {
            var inventoryItem = await _itemsRepository.GetAsync(
                item => item.UserId == grantItem.UserId && item.CatalogItemId == grantItem.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItem.CatalogItemId,
                    UserId = grantItem.UserId,
                    Quantity = grantItem.Quantity,
                    AcquiredDate = DateTimeOffset.Now
                };
                await _itemsRepository.CreateAsync(inventoryItem);
            }

            else
            {
                inventoryItem.Quantity += grantItem.Quantity;
                await _itemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}
