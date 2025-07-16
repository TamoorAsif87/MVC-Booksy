
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Consumers;

public class OrderInvoiceConsumer : IConsumer<OrderInvoice>
{
    private readonly IOrderService _orderService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    public OrderInvoiceConsumer(IOrderService orderService, IWebHostEnvironment webHostEnvironment, IEmailService emailService, IConfiguration configuration)
    {
        _orderService = orderService;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<OrderInvoice> context)
    {
        var order = await _orderService.GetOrderByIdAsync(context.Message.OrderId);
        if (order == null) return;

        var htmlTemplate = await File.ReadAllTextAsync(Path.Combine(_webHostEnvironment.WebRootPath, "templates", "order-invoice.html"));
        
        var pdfUrl = $"{_configuration["App:PublicBaseUrl"]}invoices/{order.Id}.pdf";

        var itemRowsHtml = string.Join("", order.Items.Select(item => $@"
            <tr>
                <td style='padding:10px; border:1px solid #eee;'>{item.BookName}</td>
                <td align='center' style='padding:10px; border:1px solid #eee;'>{item.Quantity}</td>
                <td align='right' style='padding:10px; border:1px solid #eee;'>{item.Price:C}</td>
                <td align='right' style='padding:10px; border:1px solid #eee;'>{item.ItemCost:C}</td>
            </tr>"));



        var htmlContent = htmlTemplate
            .Replace("{{ItemRowsHtml}}", itemRowsHtml)
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{OrderDate}}", DateTime.UtcNow.ToString("dd MMM yyyy"))
            .Replace("{{CustomerName}}", order.Name)
            .Replace("{{Address}}", order.Address)
            .Replace("{{City}}", order.City)
            .Replace("{{Country}}", order.Country)
            .Replace("{{PostCode}}", order.PostCode.ToString())
            .Replace("{{Email}}", order.Email)
            .Replace("{{Phone}}", order.Phone)
            .Replace("{{TotalPrice}}", order.TotalSum().ToString("C"))
            .Replace("{{urlPdf}}", pdfUrl);

        await _emailService.SendOrderInvoice(order.Email, htmlContent, "Order-Invoice", "Order-details", order.Name);

    }
}
