using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.ViewModels;
using SimpleLogisticSystem.ViewModels.Warehouses;
using System.Security.Claims;

namespace SimpleLogisticSystem.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemRepository _itemRepository;

        public WarehouseController(IWarehouseRepository warehouseRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IItemRepository itemRepository)
        {
            _warehouseRepository = warehouseRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _itemRepository = itemRepository;
        }

        // GET: /Warehouse/Index
        // Retrieves al warehouses.
        public async Task<IActionResult> Index()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseRepository.GetAll();
            return View(warehouses);
        }

        // GET: /Warehouse/Detail/{id}
        // Retrieves details of a specific warehouse by ID.
        public async Task<IActionResult> Detail(int id)
        {
            Warehouse warehouse = await _warehouseRepository.GetbyIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            warehouse.WarehouseItems = (await _warehouseRepository.GetWarehouseItemsByWarehouseId(id)).ToList();

            var totalQuantity = warehouse.WarehouseItems.Sum(i => i.Quantity);
            var totalWeight = warehouse.WarehouseItems.Sum(i => i.Weight);

            var viewModel = new WarehouseDetailViewModel
            {
                Warehouse = warehouse,
                TotalQuantity = totalQuantity,
                TotalWeight = totalWeight
            };

            return View(viewModel);
        }

        // GET: /Warehouse/Create
        // Displays the create warehouse form.
        public async Task<IActionResult> Create()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var currentUser = await _userRepository.GetUserById(userId);

            var createdBy = $"{currentUser.FirstName} {currentUser.LastName}";

            var warehouseVM = new CreateWarehouseViewModel
            {
                CreatedBy = createdBy,
                AppUserId = userId
            };
            return View(warehouseVM);
        }

        // POST: /Warehouse/Create
        // Handles the submission of the create warehouse form.
        [HttpPost]
        public IActionResult Create(CreateWarehouseViewModel warehouseVM)
        {
            if (ModelState.IsValid)
            {
                var warehouse = new Warehouse()
                {
                    Id = warehouseVM.Id,
                    WarehouseName = warehouseVM.WarehouseName,
                    WarehouseCompany = warehouseVM.WarehouseCompany,
                    QuantityCapacity = warehouseVM.QuantityCapacity,
                    WeightCapacity = warehouseVM.WeightCapacity,
                    AppUserId = warehouseVM.AppUserId,
                    CreatedBy = warehouseVM.CreatedBy,
                    Address = new Address
                    {
                        Street = warehouseVM.Address.Street,
                        City = warehouseVM.Address.City,
                        PostalCode = warehouseVM.Address.PostalCode,
                        Country = warehouseVM.Address.Country
                    }
                };
                _warehouseRepository.Add(warehouse);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong while saving the warehouse. Please try again.");
            }

            return View(warehouseVM);
        }

        // POST: /Warehouse/Delete/{id}
        // Deletes a specific warehouse by ID.
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _warehouseRepository.GetbyIdAsync(id);
            if (warehouse == null) return View("Error");

            _warehouseRepository.Delete(warehouse);

            return RedirectToAction("Index");
        }

        // GET: /Warehouse/Edit/{id}
        // Dusplays the edit warehouse form.
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseRepository.GetbyIdAsync(id);
            if (warehouse == null) return View("Error");
            var warehouseVM = new EditWarehouseViewModel
            {
                Id = warehouse.Id,
                WarehouseName = warehouse.WarehouseName,
                WarehouseCompany = warehouse.WarehouseCompany,
                Address = warehouse.Address,
                QuantityCapacity = warehouse.QuantityCapacity,
                WeightCapacity = warehouse.WeightCapacity,
                CreatedBy = warehouse.CreatedBy,
                AppUserId = warehouse.AppUserId
            };
            return View(warehouseVM);
        }

        // POST: /Warehouse/Edit
        // Handles the submission of the edit warehouse form.
        [HttpPost]
        public IActionResult Edit(int id, EditWarehouseViewModel warehouseVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit warehouse");
                return View("Edit", warehouseVM);
            }

            var warehouse = new Warehouse
            {
                Id = id,
                WarehouseName = warehouseVM.WarehouseName,
                WarehouseCompany = warehouseVM.WarehouseCompany,
                Address = warehouseVM.Address,
                QuantityCapacity = warehouseVM.QuantityCapacity,
                WeightCapacity = warehouseVM.WeightCapacity,
                CreatedBy = warehouseVM.CreatedBy,
                AppUserId = warehouseVM.AppUserId
            };

            _warehouseRepository.Update(warehouse);

            return RedirectToAction("Index");
        }

        // POST: /Warehouse/AddItemToWarehouse
        // Adds and item to a specific warehouse.
        [HttpPost]
        public async Task<IActionResult> AddItemToWarehouse(int warehouseId, int itemId, int quantity)
        {
            // Get warehouse and items
            var warehouse = await _warehouseRepository.GetbyIdAsync(warehouseId);
            if (warehouse == null) return NotFound();

            var item = await _itemRepository.GetbyIdAsync(itemId);
            if (item == null) return NotFound();

            // Load WarehouseItems
            warehouse.WarehouseItems = (await _warehouseRepository.GetWarehouseItemsByWarehouseId(warehouseId)).ToList();

            if (warehouse.CurrentQuantity + quantity > warehouse.QuantityCapacity || 
                warehouse.CurrentWeight + (item.Weight * quantity) > warehouse.WeightCapacity || 
                quantity > item.Quantity)
            {
                ModelState.AddModelError("", "Cannot add item to warehouse. Capacity exceeded.");

                return RedirectToAction("Index", "Item");
            }

            // Check if item already exists in warehouse
            var existingWarehouseItem = warehouse.WarehouseItems.FirstOrDefault(wi => wi.ItemId == itemId);
            if (existingWarehouseItem != null)
            {
                existingWarehouseItem.Quantity += quantity;
                existingWarehouseItem.Weight += item.Weight * quantity;
            }
            else
            {
                var warehouseItem = new WarehouseItem
                {
                    WarehouseId = warehouseId,
                    ItemId = itemId,
                    Quantity = quantity,
                    Weight = item.Weight * quantity
                };
                warehouse.WarehouseItems.Add(warehouseItem);
            }

            // Update warehouse and item
            warehouse.CurrentQuantity += quantity;
            warehouse.CurrentWeight += item.Weight * quantity;

            warehouse.CurrentWeight = Math.Round(warehouse.CurrentWeight, 2);

            item.Quantity -= quantity;
            _itemRepository.Update(item);
            _itemRepository.Save();

            _warehouseRepository.Update(warehouse);
            _warehouseRepository.Save();

            return RedirectToAction("Index", "Item");
        }

        // POST: /Warehouse/RemoveItemFromWarehouse
        // Removes an item from a specific warehouse.
        [HttpPost]
        public async Task<IActionResult> RemoveItemFromWarehouse(int warehouseId, int itemId, int quantity)
        {
            // Get warehouse and items
            var warehouse = await _warehouseRepository.GetbyIdAsync(warehouseId);
            if (warehouse == null) return NotFound();

            var item = await _itemRepository.GetbyIdAsync(itemId);
            if (item == null) return NotFound();

            warehouse.WarehouseItems = (await _warehouseRepository.GetWarehouseItemsByWarehouseId(warehouseId)).ToList();

            // Check if item exists in warehouse
            var existingWarehouseItem = warehouse.WarehouseItems.FirstOrDefault(wi => wi.ItemId == itemId);
            if (quantity > warehouse.CurrentQuantity)
            {
                ModelState.AddModelError("", "Cannot remove item from warehouse. Quantity exceeded.");
                return View("Detail", warehouse);
            }

            // Update warehouse and item
            existingWarehouseItem.Quantity -= quantity;
            existingWarehouseItem.Weight -= item.Weight * quantity;

            if (existingWarehouseItem.Quantity == 0)
            {
                warehouse.WarehouseItems.Remove(existingWarehouseItem);
            }

            warehouse.CurrentQuantity -= quantity;
            warehouse.CurrentWeight -= item.Weight * quantity;

            // Round to avoid precision issues
            warehouse.CurrentWeight = Math.Round(warehouse.CurrentWeight, 2);

            item.Quantity += quantity;
            _itemRepository.Update(item);
            _itemRepository.Save();

            _warehouseRepository.Update(warehouse);
            _warehouseRepository.Save();

            return RedirectToAction("Detail", new { id = warehouseId });
        }
    }
}