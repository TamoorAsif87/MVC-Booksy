using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace Core.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly IOrderService _orderService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;
    public OrderCreatedConsumer(IOrderService orderService, IWebHostEnvironment webHostEnvironment, IEmailService emailService)
    {
        _orderService = orderService;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var order = await _orderService.GetOrderByIdAsync(context.Message.OrderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {context.Message.OrderId} not found.");
        }

        var htmlTemplate = await File.ReadAllTextAsync(Path.Combine(_webHostEnvironment.WebRootPath, "templates", "order-details.html"));

       

        StringBuilder itemsBuilder = new StringBuilder();
        foreach (var item in order.Items)
        {
            itemsBuilder.Append($"<tr><td>{item.BookName}</td><td>{item.Quantity}</td><td>{item.Price:C}</td><td>{(item.Quantity * item.Price):C}</td></tr>");
        }

        var totalPrice  = order.Items.Sum(i => i.Price * i.Quantity);

        htmlTemplate = htmlTemplate.Replace("{{CustomerName}}", order.Name)
            .Replace("{{OrderItems}}", itemsBuilder.ToString())
            .Replace("{{OrderTotal}}", totalPrice.ToString("c"))
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

        await _emailService.SendOrderDetails(order.Email, htmlTemplate, "Order Creation", "Your order details", order.Name);
    }
}
