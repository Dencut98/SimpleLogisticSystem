using Microsoft.AspNetCore.Mvc;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.Repository;
using SimpleLogisticSystem.ViewModels.Orders;
using SimpleLogisticSystem.ViewModels.Report;
using SimpleLogisticSystem.ViewModels.Users;
using SimpleLogisticSystem.ViewModels.Warehouses;
using System.Security.Claims;
using System.Text;

namespace SimpleLogisticSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public ReportController(IItemRepository itemRepository, IWarehouseRepository warehouseRepository, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Item/InventoryReport
        // Retrieves all items and their warehouse information for the inventory report
        public async Task<IActionResult> InventoryReport()
        {
            var items = await _itemRepository.GetAll();
            var warehouses = await _warehouseRepository.GetAll();
            var warehouseItems = await _warehouseRepository.GetAllWarehouseItems(); // Assuming you have a method to get all WarehouseItems

            var inventoryReportViewModel = items.GroupBy(item => item.ItemName)
                .Select(group => new InventoryReportViewModel
                {
                    ItemName = group.Key,
                    QuantityNotStored = group.Sum(item => item.Quantity),
                    QuantityInWarehouse = group.Sum(item => warehouseItems.Where(wi => group.Any(i => i.ItemId == wi.ItemId)).Sum(wi => wi.Quantity)),
                    WarehouseDetails = warehouseItems
                        .Where(wi => group.Any(item => item.ItemId == wi.ItemId))
                        .Select(wi => new WarehouseDetailViewModel
                        {
                            Warehouse = warehouses.FirstOrDefault(w => w.Id == wi.WarehouseId),
                            TotalQuantity = wi.Quantity,
                            TotalWeight = wi.Weight * wi.Quantity
                        }).ToList()
                }).ToList();

            return View(inventoryReportViewModel);
        }

        // Action to return the warehouse report index view
        public async Task<IActionResult> WarehouseReportIndex()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseRepository.GetAll();
            return View(warehouses);
        }

        // Action to return a detailed report of a specific warehouse
        public async Task<IActionResult> WarehouseReport(int id)
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

        // Action to return the orders report index view
        public async Task<IActionResult> OrdersReportIndex()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            IEnumerable<Order> orders;

            if (User.IsInRole("admin") || User.IsInRole("inventory_manager"))
            {
                orders = await _orderRepository.GetAllOrdersAsync();
            }
            else
            {
                orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            }

            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId.ToString(),
                OrderType = order.OrderType,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems,
                OrderStatus = order.OrderStatus,
                Address = order.Address
            }).ToList();

            return View(orderViewModels);
        }

        // Action to return a detailed report of a specific order
        public async Task<IActionResult> OrderReport(string orderId)
        {
            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("OrdersReportIndex");
            }

            return View(order);
        }

        // Action to return a report of all orders
        public async Task<IActionResult> OrderReportAll()
         {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId.ToString(),
                OrderType = order.OrderType,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems,
                OrderStatus = order.OrderStatus,
                Address = order.Address
            }).ToList();

            return View(orderViewModels);
        }

        // Action to return a report of all user accounts
        public async Task<IActionResult> AccountsReport()
        {
            var users = await _userRepository.GetAllUsers();
            var userViewModels = users.Select(user => new UserViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.AccountType
            }).ToList();

            return View(userViewModels);
        }

        // Action to export user accounts report to CSV
        [HttpPost]
        public async Task<IActionResult> ExportAccountsToCsv()
        {
            var users = await _userRepository.GetAllUsers();
            var userViewModels = users.Select(user => new UserViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.AccountType
            }).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Username,FirstName,LastName,Email,UserRole");

            foreach (var user in userViewModels)
            {
                csv.AppendLine($"{user.Username},{user.FirstName},{user.LastName},{user.Email},{user.UserRole}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "AccountsReport.csv");
        }

        // Action to export a specific order report to CSV
        [HttpPost]
        public async Task<IActionResult> ExportOrderToCsv(string orderId)
        {
            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var csv = new StringBuilder();
            csv.AppendLine("Order ID,Created By,Time of Order,Street,City,Postal Code,Country,Item ID,Item Name,Price,Weight,Quantity");

            foreach (var item in order.OrderItems)
            {
                csv.AppendLine($"{order.OrderId},{order.CreatedBy},{order.CreatedAt},{order.Address.Street},{order.Address.City},{order.Address.PostalCode},{order.Address.Country},{item.Item.ItemId},{item.Item.ItemName},{item.Item.Price},{item.Weight},{item.Quantity}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"{orderId}.csv";
            return File(bytes, "text/csv", fileName);
        }

        // Action to export all orders report to CSV
        [HttpPost]
        public async Task<IActionResult> ExportAllOrdersToCsv()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId.ToString(),
                OrderType = order.OrderType,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems,
                OrderStatus = order.OrderStatus,
                Address = order.Address
            }).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Order ID,Created By,Created At,Total Price,Total Weight,Items");

            foreach (var order in orderViewModels)
            {
                var items = string.Join(", ", order.OrderItems.Select(item => item.Item.ItemName));
                csv.AppendLine($"{order.OrderId},{order.CreatedBy},{order.CreatedAt},{order.TotalPrice.ToString("C")},{order.TotalWeight.ToString("F2")} Kg,\"{items}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "AllOrdersReport.csv");
        }

        // Action to export all warehouses report to CSV
        public async Task<IActionResult> ExportWarehouseReportAll()
        {
            var warehouses = await _warehouseRepository.GetAllIncludingAddressAndItems();
            var warehouseItems = await _warehouseRepository.GetAllWarehouseItems();

            var csv = new StringBuilder();
            csv.AppendLine("Warehouse ID,Warehouse Company,Warehouse Name,Street,City,Postal Code,Country,Quantity Capacity,Weight Capacity,Created By,Created By (ID),Item ID,Item Name,Quantity,Weight");

            foreach (var warehouse in warehouses)
            {
                var items = warehouseItems.Where(wi => wi.WarehouseId == warehouse.Id);
                foreach (var item in items)
                {
                    csv.AppendLine($"{warehouse.Id},{warehouse.WarehouseCompany},{warehouse.WarehouseName},{warehouse.Address?.Street},{warehouse.Address?.City},{warehouse.Address?.PostalCode},{warehouse.Address?.Country},{warehouse.QuantityCapacity},{warehouse.WeightCapacity.ToString("F1")},{warehouse.CreatedBy},{warehouse.AppUserId},{item.ItemId},{item.Item.ItemName},{item.Quantity},{item.Weight.ToString("F1")}");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "AllWarehousesReport.csv");
        }

        // Action to export a specific warehouse report to CSV
        public async Task<IActionResult> ExportWarehouseToCsv(int id)
        {
            Warehouse warehouse = await _warehouseRepository.GetbyIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            warehouse.WarehouseItems = (await _warehouseRepository.GetWarehouseItemsByWarehouseId(id)).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Warehouse ID,Warehouse Company,Warehouse Name,Street,City,Postal Code,Country,Quantity Capacity,Weight Capacity,Created By,Created By (ID),Item ID,Item Name,Quantity,Weight");

            foreach (var item in warehouse.WarehouseItems)
            {
                csv.AppendLine($"{warehouse.Id},{warehouse.WarehouseCompany},{warehouse.WarehouseName},{warehouse.Address?.Street},{warehouse.Address?.City},{warehouse.Address?.PostalCode},{warehouse.Address?.Country},{warehouse.QuantityCapacity},{warehouse.WeightCapacity.ToString("F1")},{warehouse.CreatedBy},{warehouse.AppUserId},{item.ItemId},{item.Item.ItemName},{item.Quantity},{item.Weight.ToString("F1")}");
            }

            var fileName = $"{warehouse.WarehouseCompany}_{warehouse.WarehouseName}.csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", fileName);
        }

        // Action to export the inventory report to CSV
        public async Task<IActionResult> ExportInventoryReportToCsv()
        {
            var items = await _itemRepository.GetAll();
            var warehouses = await _warehouseRepository.GetAll();
            var warehouseItems = await _warehouseRepository.GetAllWarehouseItems();

            var inventoryReportViewModel = items.GroupBy(item => item.ItemName)
                .Select(group => new InventoryReportViewModel
                {
                    ItemName = group.Key,
                    QuantityNotStored = group.Sum(item => item.Quantity),
                    QuantityInWarehouse = group.Sum(item => warehouseItems.Where(wi => group.Any(i => i.ItemId == wi.ItemId)).Sum(wi => wi.Quantity)),
                    WarehouseDetails = warehouseItems
                        .Where(wi => group.Any(item => item.ItemId == wi.ItemId))
                        .Select(wi => new WarehouseDetailViewModel
                        {
                            Warehouse = warehouses.FirstOrDefault(w => w.Id == wi.WarehouseId),
                            TotalQuantity = wi.Quantity,
                            TotalWeight = wi.Weight * wi.Quantity
                        }).ToList()
                }).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Item Name,Quantity not yet in warehouses,Warehouse Name,Warehouse Quantity");

            foreach (var item in inventoryReportViewModel)
            {
                foreach (var detail in item.WarehouseDetails)
                {
                    csv.AppendLine($"{item.ItemName},{item.QuantityNotStored},{detail.Warehouse.WarehouseName},{detail.TotalQuantity}");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "InventoryReport.csv");
        }
    }
}
