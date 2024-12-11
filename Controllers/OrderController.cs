using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.ViewModels.Orders;
using SimpleLogisticSystem.ViewModels.Warehouses;
using System.Security.Claims;

namespace SimpleLogisticSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusUpdater _orderStatusUpdaterRepository;

        public OrderController(ApplicationDbContext context, IWarehouseRepository warehouseRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IOrderRepository orderRepository, IOrderStatusUpdater orderStatusUpdaterRepository)
        {
            _context = context;
            _warehouseRepository = warehouseRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderStatusUpdaterRepository = orderStatusUpdaterRepository;
        }

        // GET: /Order/Index
        // Retrieves all orders for the current user or all orders if the user is an admin or inventory manager.
        public async Task<IActionResult> Index()
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

        // GET: /Order/CreateOrder
        // Displays the create order form with available warehouse items.
        public IActionResult CreateOrder()
        {
            var warehouseItems = _context.WarehouseItems.Include(w => w.Item).ToList();
            ViewBag.WarehouseItems = warehouseItems;
            return View();
        }

        // POST: /Order/AddToOrder
        // Adds an item to the current order stored in TempData.
        [HttpPost]
        public IActionResult AddToOrder(int itemId, int quantity)
        {
            var warehouseItem = _context.WarehouseItems
                .Where(wi => wi.ItemId == itemId)
                .Select(wi => new OrderItem
                {
                    Item = wi.Item,
                    Quantity = quantity
                })
                .FirstOrDefault();

            if (warehouseItem == null) return NotFound();

            // Retrieve the current order from TempData or create a new one
            var order = TempData["Order"] as Order ?? new Order
            {
                OrderItems = new List<OrderItem>()
            };

            // Add the new item to the order
            order.OrderItems.Add(warehouseItem);

            // Store the updated order in TempData
            TempData["Order"] = order;

            var warehouseItems = _context.WarehouseItems
                .Select(wi => new
                {
                    wi.ItemId,
                    wi.Item.ItemName,
                    Item = new
                    {
                        wi.Item.Price,
                        wi.Item.ItemCategory
                    },
                    wi.Quantity
                })
                .ToList();

            ViewBag.WarehouseItems = warehouseItems;

            return View("Create", order);
        }

        // GET: /Order/Create
        // Displays the create order form with user and warehouse item details.
        public async Task<IActionResult> Create()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var currentUser = await _userRepository.GetUserById(userId);

            var createdBy = $"{currentUser.FirstName} {currentUser.LastName}";

            var warehouseItems = await _context.WarehouseItems.Include(w => w.Item).ToListAsync();

            var createOrderVM = new CreateOrderViewModel
            {
                CreatedBy = createdBy,
                AppUserId = userId,
                WarehouseItems = warehouseItems.Select(wi => new WarehouseItemViewModel
                {
                    ItemId = wi.ItemId,
                    ItemName = wi.Item.ItemName,
                    Price = wi.Item.Price,
                    Quantity = wi.Quantity,
                    Weight = wi.Item.Weight,
                    ItemCategory = wi.Item.ItemCategory
                }).ToList()
            };

            return View(createOrderVM);
        }

        // POST: /Order/Create
        // Handles the submission of the create order form.
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel createOrderVM)
        {
            if (createOrderVM.OrderItems == null || !createOrderVM.OrderItems.Any())
            {
                return RedirectToAction("CreateOrder");
            }

            // Populate order items with details from the database
            var populatedOrderItems = createOrderVM.OrderItems.Select(oi =>
            {
                var item = _context.Items.FirstOrDefault(i => i.ItemId == oi.ItemId);
                if (item != null)
                {
                    return new OrderItem
                    {
                        ItemId = item.ItemId,
                        Item = item,
                        Quantity = oi.Quantity,
                        Weight = item.Weight
                    };
                }
                return null;
            }).Where(oi => oi != null).ToList();

            // If any order items could not be populated
            if (populatedOrderItems.Count != createOrderVM.OrderItems.Count)
            {
                return RedirectToAction("CreateOrder");
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userRepository.GetUserById(userId);
            var createdBy = $"{currentUser.FirstName} {currentUser.LastName}";

            // Create the order
            var order = new Order
            {
                OrderItems = populatedOrderItems,
                CreatedBy = createdBy,
                AppUserId = userId,
                Address = currentUser.Address,
                AddressId = currentUser.Address.Id
            };

            // Update the warehouse item quantities and weights
            foreach (var orderItem in order.OrderItems)
            {
                var warehouseItem = await _context.WarehouseItems
                    .Include(wi => wi.Warehouse)
                    .FirstOrDefaultAsync(wi => wi.ItemId == orderItem.ItemId);

                if (warehouseItem != null)
                {
                    warehouseItem.Quantity -= orderItem.Quantity;
                    warehouseItem.Warehouse.CurrentWeight -= orderItem.Quantity * orderItem.Weight;
                    warehouseItem.Warehouse.CurrentQuantity -= orderItem.Quantity;

                    _context.WarehouseItems.Update(warehouseItem);
                    _context.Warehouses.Update(warehouseItem.Warehouse);
                }
            }

            _orderRepository.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("PreviewOrder", new { orderId = order.OrderId });
        }

        // GET: /Order/PreviewOrder
        // Displays the preview of a specific order.
        public async Task<IActionResult> PreviewOrder(string orderId)
        {
            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("CreateOrder");
            }

            //if (order.Address)

            return View(order);
        }

        public async Task<IActionResult> Delete(string orderId)
        {
            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            _orderRepository.Delete(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Order ID cannot be null or empty.");
            }

            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.OrderStatus = OrderStatus.Confirmed;
            _orderRepository.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendOutOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Order ID cannot be null or empty.");
            }

            var order = await _orderRepository.GetbyIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.OrderStatus = OrderStatus.Shipped;
            await _context.SaveChangesAsync();

            _ = _orderStatusUpdaterRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered, TimeSpan.FromMinutes(1));

            return RedirectToAction("Index");
        }
    }
}