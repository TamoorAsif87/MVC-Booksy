using Microsoft.AspNetCore.Http;
using Stripe.Checkout;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IMapper _mapper;
    private readonly IBookService _bookService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICart _cartService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRecommender _recommender;
    public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, IBookService bookService, IPublishEndpoint publishEndpoint, ICart cartService, IHttpContextAccessor httpContextAccessor, IRecommender recommender)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _mapper = mapper;
        _bookService = bookService;
        _publishEndpoint = publishEndpoint;
        _cartService = cartService;
        _httpContextAccessor = httpContextAccessor;
        _recommender = recommender;
    }

    public async Task CreateOrderAsync(OrderDto orderDto)
    {
        var order = _mapper.Map<Order>(orderDto);
        order.Status = OrderStatus.Processing;
        order.ProcessedAt = DateTime.Now;
        await _orderRepository.AddAsync(order);

        var cartItems = _cartService.GetCartItems() ?? throw new Exception("Must have cart Items for order creation");

        foreach (var item in cartItems)
        {
            var book = await _bookService.GetByIdAsync(item.BookId);

            // Ensure the book exists before adding to order
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {item.BookId} not found.");

            // Ensure quantity is valid
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(item.Quantity));

            //Map item to OrderItem
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BookName = book.Title,
                BookCover = book.BookCover,
                Price = book.Price - (book.Price * book.Discount / 100), 
                Quantity = item.Quantity,
                OrderId = order.Id
            });
        

           
        }

        order.TotalPrice = order.Items.Sum(x => x.Price);
        await _orderItemRepository.SaveChanges();

        _ = _publishEndpoint.Publish<OrderCreated>(new OrderCreated
        {
            OrderId = order.Id
        });

        _cartService.ClearCart();

        _httpContextAccessor.HttpContext?.Session.SetString("order:Id", order.Id.ToString());
    }

    public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.FindOneAsync(o => o.Id == orderId, "Items");

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync(null, "Items");
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task UpdateOrderAsync(Guid orderId, OrderDto orderDto)
    {
        var existingOrder = await _orderRepository.FindOneAsync(o => o.Id == orderId, "Items");

        if (existingOrder == null)
            throw new KeyNotFoundException("Order not found.");

        // Map updated fields (but preserve ID, status, etc.)
        _mapper.Map(orderDto, existingOrder);

        await _orderRepository.Update(existingOrder);
        await _orderRepository.SaveChanges();
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        await _orderRepository.Remove(order);
        await _orderRepository.SaveChanges();
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(string customerId)
    {
        // NOTE: Adjust if Order has a CustomerId property
        var orders = await _orderRepository.GetAllAsync(o => o.CustomerId == customerId, "Items");
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var orders = await _orderRepository.GetAllAsync(o => o.Status == status, "Items");
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task CancelOrder(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if(order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing)
        {
            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.TotalPrice = 0;

            await _orderRepository.Update(order);
            await _orderRepository.SaveChanges();
            _httpContextAccessor.HttpContext?.Session.Remove("order:Id");
        }
        else
        {
            throw new Exception("Order can not cancelled because it is neither pending nor processing only pending or processing can be canceled");
        }
      
    }

    public async Task<string> CheckOut()
    {
        var orderId = Guid.Parse(_httpContextAccessor.HttpContext?.Session.GetString("order:Id")!);

        var order = await GetOrderByIdAsync(orderId) ?? throw new Exception($"Order not Found with Id {orderId}");

        var domain = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";

        var lineItems = order.Items.Select(item =>
        {
            return new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.BookName,
                    }
                },
                Quantity = item.Quantity
            };
        }).ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> {"card"},
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = domain + $"/checkout/success",
            CancelUrl = domain + $"/checkout/failure",
            ClientReferenceId = orderId.ToString()
        };

        var service = new SessionService();
        Session session = service.Create(options);
        return session.Url;

    }

    public async Task<Guid> PaymentSuccessful()
    {
        var orderId = Guid.Parse(_httpContextAccessor.HttpContext?.Session.GetString("order:Id")!);

        var order = await _orderRepository.FindOneAsync(o => o.Id == orderId, "Items") ?? throw new Exception($"Order not Found with Id {orderId}");

        order.Status = OrderStatus.Approved;
        order.ProcessedAt = DateTime.Now;
        order.Paid = true;

        await _orderRepository.SaveChanges();

        _ =  _publishEndpoint.Publish<OrderInvoice>(new OrderInvoice { OrderId = order.Id});

        await _recommender.BoughtTogether(order.Items.Select(item => item.BookId).ToList());
        _httpContextAccessor.HttpContext?.Session.Remove("order:Id");
        return orderId;
    }

    public async Task<OrderDto> GetOrderDetailsOfCustomer(string customerId, Guid orderId)
    {
        var order = await _orderRepository.FindOneAsync(o => o.CustomerId == customerId && o.Id == orderId, "Items");
        return _mapper.Map<OrderDto>(order);
    }

    public async Task MarkAsShipped(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null) throw new KeyNotFoundException("Order not Found");

        order.Status = OrderStatus.Shipped;
        order.ShippedAt = DateTime.Now;

        await _orderRepository.SaveChanges();
    }

    public async Task MarkAsDelivered(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null) throw new KeyNotFoundException("Order not Found");

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.Now;

        await _orderRepository.SaveChanges();
    }
}
