using Microsoft.AspNetCore.Mvc;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.ViewModels;
using SimpleLogisticSystem.ViewModels.Items;
using SimpleLogisticSystem.ViewModels.Report;
using SimpleLogisticSystem.ViewModels.Warehouses;
using System.Security.Claims;

namespace SimpleLogisticSystem.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public ItemController(IItemRepository itemRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IWarehouseRepository warehouseRepository)
        {
            _itemRepository = itemRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _warehouseRepository = warehouseRepository;
        }

        // GET: /Item/Index
        // Retrieves all items and warehouses from the database and displays them on the index page
        public async Task<IActionResult> Index()
        {
            IEnumerable<Item> items = await _itemRepository.GetAll();
            var itemViewModels = items.Select(item => new ItemViewModel
            {
                Id = item.ItemId,
                ItemName = item.ItemName,
                ItemCategory = item.ItemCategory,
                Weight = item.Weight,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();

            IEnumerable<Warehouse> warehouses = await _warehouseRepository.GetAll();
            var warehouseViewModels = warehouses.Select(warehouse => new WarehouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.WarehouseName,
                Company = warehouse.WarehouseCompany
            }).ToList();

            var viewModel = new ItemIndexViewModel
            {
                Items = itemViewModels,
                Warehouses = warehouseViewModels
            };

            return View(viewModel);
        }

        // GET: /Item/Create
        // Displays the create item form
        public async Task<IActionResult> Create()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var currentUser = await _userRepository.GetUserById(userId);
            var createdBy = $"{currentUser.FirstName} {currentUser.LastName}";

            var warehouses = await _warehouseRepository.GetAll();
            var warehouseViewModels = warehouses.Select(warehouse => new WarehouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.WarehouseName
            }).ToList();

            var itemVM = new CreateItemViewModel
            {
                CreatedBy = createdBy,
                AppUserId = userId
            };
            return View(itemVM);
        }

        // POST: /Item/Create
        // Handles the submission of the create item form
        [HttpPost]
        public IActionResult Create(CreateItemViewModel itemVM)
        {
            if (ModelState.IsValid)
            {
                var item = new Item()
                {
                    ItemId = itemVM.ItemId,
                    ItemName = itemVM.ItemName,
                    Weight = itemVM.Weight,
                    ItemCategory = itemVM.ItemCategory,
                    Price = itemVM.Price,
                    Quantity = itemVM.Quantity,
                    CreatedBy = itemVM.CreatedBy,
                    AppUserId = itemVM.AppUserId
                };
                _itemRepository.Add(item);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong while saving the item. Please try again.");
            }

            return View(itemVM);
        }

        // Get: /Item/Edit/{id}
        // Displays the edit item form for a specific item
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _itemRepository.GetbyIdAsync(id);
            if (item == null) return View("Error");
            var itemVM = new EditItemViewModel
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                ItemCategory = item.ItemCategory,
                Weight = item.Weight,
                Quantity = item.Quantity,
                Price = item.Price,
                CreatedBy = item.CreatedBy,
                AppUserId = item.AppUserId
            };
            return View(itemVM);
        }

        // POST: /Item/Edit/{id}
        // Handles the submission of the edit item form.
        [HttpPost]
        public IActionResult Edit(int id, EditItemViewModel itemVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit warehouse");
                return View("Edit", itemVM);
            }

            var item = new Item
            {
                ItemId = id,
                ItemName = itemVM.ItemName,
                ItemCategory = itemVM.ItemCategory,
                Weight = itemVM.Weight,
                Quantity = itemVM.Quantity,
                Price = itemVM.Price,
                CreatedBy = itemVM.CreatedBy,
                AppUserId = itemVM.AppUserId
            };

            _itemRepository.Update(item);
            return RedirectToAction("Index");
        }

        // POST: /Item/Delete/{id}
        // Handles the deletion of a specific item.
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemRepository.GetbyIdAsync(id);
            if (item == null) return View("Error");

            _itemRepository.Delete(item);

            var itemVM = await _itemRepository.GetAll();
            return RedirectToAction("Index");
        }
    }
}
