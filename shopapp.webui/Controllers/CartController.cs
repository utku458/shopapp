using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.entity;
using shopapp.webui.Identity;
using shopapp.webui.Models;
using Microsoft.Extensions.Options;
using shopapp.business.Abstract;

namespace shopapp.webui.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly UserManager<User> _userManager;
    private readonly IyzipaySettings _iyzipaySettings;

    public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService, IOptions<IyzipaySettings> iyzipaySettings)
    {
        _cartService = cartService;
        _userManager = userManager;
        _orderService = orderService;
        _iyzipaySettings = iyzipaySettings.Value;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart(_userManager.GetUserId(User));

        return View(new CartModel
        {
            CartId = cart.Id,
            CartItems = cart.CartItems.Select(i => new CartItemModel
            {
                CartItemId = i.Id,
                ProductId = i.ProductId,
                Name = i.Product.Name,
                Price = (double)i.Product.Price,
                ImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity
            }).ToList()
        });
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        _cartService.AddToCart(_userManager.GetUserId(User), productId, quantity);
        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        var userId = _userManager.GetUserId(User);
        _cartService.DeleteFromCart(userId, productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var userId = _userManager.GetUserId(User);
        _cartService.UpdateQuantity(userId, productId, quantity);
        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        var cart = _cartService.GetCart(_userManager.GetUserId(User));

        var orderModel = new OrderModel
        {
            CartModel = new CartModel
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product?.Name ?? "Unknown",
                    Price = i.Product?.Price ?? 0,
                    ImageUrl = i.Product?.ImageUrl ?? "",
                    Quantity = i.Quantity
                }).ToList()
            }
        };

        return View(orderModel);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(OrderModel orderModel)
    {
        try
        {
            Console.WriteLine("Checkout başladı");
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"User ID: {userId}");
            
            var cart = _cartService.GetCart(userId);
            Console.WriteLine($"Cart: {(cart == null ? "null" : "exists")}");
            if (cart != null)
            {
                Console.WriteLine($"Cart Items Count: {cart.CartItems?.Count ?? 0}");
                if (cart.CartItems != null)
                {
                    foreach (var item in cart.CartItems)
                    {
                        Console.WriteLine($"Cart Item - ProductId: {item.ProductId}, Quantity: {item.Quantity}");
                    }
                }
            }

            if (cart == null || !cart.CartItems.Any())
            {
                Console.WriteLine("Sepet boş hatası");
                ViewBag.ErrorMessage = "Sepetiniz boş. Lütfen ürün ekleyin.";
                return View("Error");
            }

            var cartModel = new CartModel
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product?.Name ?? "Unknown",
                    Price = i.Product?.Price ?? 0,
                    ImageUrl = i.Product?.ImageUrl ?? "",
                    Quantity = i.Quantity
                }).ToList()
            };

            Console.WriteLine($"CartModel CartItems Count: {cartModel.CartItems.Count}");
            Console.WriteLine($"Toplam Tutar: {cartModel.TotalPrice()}");

            if (cartModel.TotalPrice() <= 0)
            {
                Console.WriteLine("Geçersiz tutar hatası");
                ViewBag.ErrorMessage = "Sepet tutarı geçerli değil.";
                return View("Error");
            }

            ModelState.Remove("CartModel");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validasyon hatası");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                ViewBag.ErrorMessage = "Lütfen gerekli alanları doldurun.";
                return View("Error");
            }

            orderModel.CartModel = cartModel;

            Console.WriteLine("Ödeme işlemi başlatılıyor");
            var result = await PaymentProcess(orderModel);
            Console.WriteLine($"Ödeme sonucu: {result.Status}");

            if (result.Status.ToLower() == "success")
            {
                Console.WriteLine("Sipariş kaydediliyor");
                SaveOrder(orderModel, result, userId);
                ClearCart(orderModel.CartModel.CartId);
                return View("success");
            }

            Console.WriteLine($"Ödeme hatası: {result.ErrorMessage}");
            ViewBag.ErrorMessage = result.ErrorMessage ?? "Ödeme sırasında bir hata oluştu.";
            return View("Error");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Checkout hatası: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ViewBag.ErrorMessage = "Beklenmeyen bir hata oluştu.";
            return View("Error");
        }
    }

    private void ClearCart(int cartId)
    {
        _cartService.ClearCart(cartId);
    }

    private void SaveOrder(OrderModel model, Payment payment, string userId)
    {
        var order = new Order
        {
            OrderNumber = new Random().Next(111111, 999999).ToString(),
            OrderStatus = EnumOrderStatus.completed,
            PaymentTypes = EnumPaymentTypes.CreditCart,
            PaymentId = payment.PaymentId,
            ConversationId = payment.ConversationId,
            OrderDate = DateTime.Now,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Adress = model.Address,
            City = model.City,
            UserId = userId,
            Note = model.Note,
            OrderItems = model.CartModel.CartItems.Select(item => new shopapp.entity.OrderItem
            {
                Price = item.Price,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        _orderService.Create(order);
    }

    private async Task<Payment> PaymentProcess(OrderModel orderModel)
    {
        try
        {
            Console.WriteLine("PaymentProcess başladı");
            Console.WriteLine($"Toplam Tutar: {orderModel.CartModel.TotalPrice()}");
            
            Iyzipay.Options options = new Iyzipay.Options
            {
                ApiKey = _iyzipaySettings.ApiKey,
                SecretKey = _iyzipaySettings.SecretKey,
                BaseUrl = _iyzipaySettings.BaseUrl
            };

            var request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = new Random().Next(111111111, 999999999).ToString(),
                Price = orderModel.CartModel.TotalPrice().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                PaidPrice = orderModel.CartModel.TotalPrice().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                Currency = Currency.TRY.ToString(),
                Installment = 1,
                BasketId = "B67832",
                PaymentChannel = PaymentChannel.WEB.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                PaymentCard = new PaymentCard
                {
                    CardHolderName = orderModel.CardName,
                    CardNumber = orderModel.CardNumber.Replace(" ", ""),
                    ExpireMonth = orderModel.ExpirationMonth,
                    ExpireYear = orderModel.ExpirationYear,
                    Cvc = orderModel.Cvc,
                    RegisterCard = 0
                },
                Buyer = new Buyer
                {
                    Id = "BY789",
                    Name = orderModel.FirstName,
                    Surname = orderModel.LastName,
                    GsmNumber = orderModel.Phone,
                    Email = orderModel.Email,
                    IdentityNumber = "74300864791",
                    LastLoginDate = "2015-10-05 12:43:35",
                    RegistrationDate = "2013-04-21 15:12:09",
                    RegistrationAddress = orderModel.Address,
                    Ip = "85.34.78.112",
                    City = orderModel.City,
                    Country = "Turkey",
                    ZipCode = "34732"
                },
                ShippingAddress = new Address
                {
                    ContactName = $"{orderModel.FirstName} {orderModel.LastName}",
                    City = orderModel.City,
                    Country = "Turkey",
                    Description = orderModel.Address,
                    ZipCode = "34742"
                },
                BillingAddress = new Address
                {
                    ContactName = $"{orderModel.FirstName} {orderModel.LastName}",
                    City = orderModel.City,
                    Country = "Turkey",
                    Description = orderModel.Address,
                    ZipCode = "34742"
                },
                BasketItems = orderModel.CartModel.CartItems.Select(item => new BasketItem
                {
                    Id = item.ProductId.ToString(),
                    Name = item.Name,
                    Category1 = "Telefon",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Quantity).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                }).ToList()
            };

            Console.WriteLine("Payment isteği oluşturuldu");
            Console.WriteLine($"Price: {request.Price}");
            Console.WriteLine($"PaidPrice: {request.PaidPrice}");
            Console.WriteLine($"CardNumber: {request.PaymentCard.CardNumber}");
            Console.WriteLine($"ExpireMonth: {request.PaymentCard.ExpireMonth}");
            Console.WriteLine($"ExpireYear: {request.PaymentCard.ExpireYear}");
            Console.WriteLine($"Cvc: {request.PaymentCard.Cvc}");
            
            var payment = await Payment.Create(request, options);
            Console.WriteLine($"Payment durumu: {payment.Status}");
            Console.WriteLine($"Payment hata mesajı: {payment.ErrorMessage}");
            
            return payment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PaymentProcess hatası: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }


    public IActionResult GetOrders()
    {
        var userId = _userManager.GetUserId(User);
        var orders = _orderService.GetOrders(userId);
        var orderlistModel = new List<OrderListModel>();
        OrderListModel orderModel; 
        foreach (var order in orders)
        {
            orderModel = new OrderListModel();
            
            orderModel.OrderId = order.Id;
            orderModel.OrderNumber = order.OrderNumber;
            orderModel.OrderDate = order.OrderDate;
            orderModel.OrderStatus = order.OrderStatus;
            orderModel.Phone = order.Phone;
            orderModel.FirstName = order.FirstName;
            orderModel.LastName = order.LastName;
            orderModel.Email = order.Email;
            orderModel.City = order.City;

            orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
            {
                OrderItemId = i.id,
                Name = i.Product.Name,
                Quantity = i.Quantity,
                Price = (double)i.Price,
                ImageUrl = i.Product.ImageUrl,
                
                
            }).ToList();
            
            orderlistModel.Add(orderModel);


        }
        
        return View("Orders", orderlistModel);
    }

    public IActionResult OrderDetails(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = _orderService.GetOrders(userId).FirstOrDefault(o => o.Id == id);
        
        if (order == null)
        {
            return NotFound();
        }

        var orderModel = new OrderListModel
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus,
            Phone = order.Phone,
            FirstName = order.FirstName,
            LastName = order.LastName,
            Email = order.Email,
            City = order.City,
            Adress = order.Adress,
            Note = order.Note,
            OrderItems = order.OrderItems.Select(i => new OrderItemModel()
            {
                OrderItemId = i.id,
                Name = i.Product.Name,
                Quantity = i.Quantity,
                Price = (double)i.Price,
                ImageUrl = i.Product.ImageUrl,
            }).ToList()
        };

        return View(orderModel);
    }
}