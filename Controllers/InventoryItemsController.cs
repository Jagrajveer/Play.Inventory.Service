using Microsoft.AspNetCore.Mvc;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class InventoryItemsController : Controller
{
    private readonly IRepository<InventoryItem> _inventoryItemsRepository;
    private readonly IRepository<CatalogItem> _catalogItemsRepository;

    public InventoryItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
    {
        _inventoryItemsRepository = inventoryItemsRepository;
        _catalogItemsRepository = catalogItemsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
        var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
        var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));
        
        var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
        {
            var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
        });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(GrantItemDto grantItemDto)
    {
        var inventoryItem = await _inventoryItemsRepository.GetAsync(item =>
            item.UserId == grantItemDto.UserId && item.CatalogItemId == grantItemDto.CatalogItemId);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemDto.CatalogItemId,
                UserId = grantItemDto.UserId,
                Quantity = grantItemDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryItemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemDto.Quantity;
            await _inventoryItemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}
