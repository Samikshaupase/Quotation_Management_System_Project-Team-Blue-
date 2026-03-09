using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuotationManagementApp.Data;
using QuotationManagementApp.Interfaces;
using QuotationManagementApp.Logic;
using QuotationManagementApp.Models;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString =
            context.Configuration["ConnectionStrings:DefaultConnection"];

        services.AddDbContext<CRMDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IQuotationRepository, QuotationRepository>();
        services.AddScoped<IQuotationService, QuotationService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<IQuotationService>();

await RunMenu(service);


// MENU
static async Task RunMenu(IQuotationService service)
{
    while (true)
    {
        Console.WriteLine("\n==================================");
        Console.WriteLine("   QUOTATION MANAGEMENT SYSTEM   ");
        Console.WriteLine("==================================");
        Console.WriteLine("1. Create Quotation");
        Console.WriteLine("2. View All Quotations");
        Console.WriteLine("3. View Quotation By Id");
        Console.WriteLine("4. Update Status");
        Console.WriteLine("5. Soft Delete");
        Console.WriteLine("0. Exit");
        Console.Write("Select Option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await CreateQuotation(service);
                break;

            case "2":
                await ViewAll(service);
                break;

            case "3":
                await ViewById(service);
                break;

            case "4":
                await UpdateStatus(service);
                break;

            case "5":
                await SoftDelete(service);
                break;

            case "0":
                Console.WriteLine("Exiting Application...");
                return;

            default:
                Console.WriteLine("Invalid choice! Try again.");
                break;
        }
    }
}


// CREATE

static async Task CreateQuotation(IQuotationService service)
{
    Console.Write("Enter Quote Number: ");
    var quoteNumber = Console.ReadLine();

    Console.WriteLine("Select Status:");
Console.WriteLine("1. Draft");
Console.WriteLine("2. Sent");
Console.WriteLine("3. Viewed");
Console.WriteLine("4. Accepted");
Console.WriteLine("5. Rejected");
Console.WriteLine("6. Expired");

int.TryParse(Console.ReadLine(), out int statusChoice);

QuoteStatus status = statusChoice switch
{
    1 => QuoteStatus.Draft,
    2 => QuoteStatus.Sent,
    3 => QuoteStatus.Viewed,
    4 => QuoteStatus.Accepted,
    5 => QuoteStatus.Rejected,
    6 => QuoteStatus.Expired,
    _ => QuoteStatus.Draft
};

    Console.Write("Enter Discount Amount: ");
    decimal.TryParse(Console.ReadLine(), out decimal discount);

    var quotation = new Quotation
{
    Status = status,
    DiscountAmount = discount,
    ExpiryDate = DateTime.Now.AddDays(7),
    LineItems = new List<QuotationLineItem>()
};

    while (true)
    {
        Console.Write("Enter Product Name (or 'done'): ");
        var product = Console.ReadLine();

        if (product?.ToLower() == "done")
            break;

        Console.Write("Enter Quantity: ");
        int.TryParse(Console.ReadLine(), out int qty);

        Console.Write("Enter Unit Price: ");
        decimal.TryParse(Console.ReadLine(), out decimal price);

        quotation.LineItems.Add(new QuotationLineItem
        {
            ProductName = product ?? string.Empty,
            Quantity = qty,
            UnitPrice = price
        });
    }

    await service.CreateAsync(quotation);
    Console.WriteLine("Quotation Created Successfully!");
}


// VIEW ALL
static async Task ViewAll(IQuotationService service)
{
    var quotations = await service.GetAllAsync();

    if (!quotations.Any())
    {
        Console.WriteLine("No quotations found.");
        return;
    }

    foreach (var q in quotations)
    {
        Console.WriteLine("----------------------------------");
        Console.WriteLine($"ID: {q.QuotationId}");
        Console.WriteLine($"Quote Number: {q.QuoteNumber}");
        Console.WriteLine($"Status: {q.Status}");
        Console.WriteLine($"Total Amount: {q.TotalAmount}");
    }
}


//VIEW BY ID

static async Task ViewById(IQuotationService service)
{
    Console.Write("Enter Quotation ID: ");
    int.TryParse(Console.ReadLine(), out int id);

    var q = await service.GetByIdAsync(id);

    if (q == null)
    {
        Console.WriteLine("Quotation not found.");
        return;
    }

    Console.WriteLine("\nQuotation Details:");
    Console.WriteLine($"Quote Number: {q.QuoteNumber}");
    Console.WriteLine($"Status: {q.Status}");
    Console.WriteLine($"Total: {q.TotalAmount}");

    Console.WriteLine("\nLine Items:");
    foreach (var item in q.LineItems)
    {
        Console.WriteLine($"- {item.ProductName} | Qty: {item.Quantity} | Price: {item.UnitPrice}");
    }
}


//UPDATE

static async Task UpdateStatus(IQuotationService service)
{
    Console.Write("Enter Quotation ID: ");
    int.TryParse(Console.ReadLine(), out int id);

    Console.WriteLine("Select New Status:");
Console.WriteLine("1. Sent");
Console.WriteLine("2. Viewed");
Console.WriteLine("3. Accepted");
Console.WriteLine("4. Rejected");
Console.WriteLine("5. Expired");

int.TryParse(Console.ReadLine(), out int statusChoice);

QuoteStatus newStatus = statusChoice switch
{
    1 => QuoteStatus.Sent,
    2 => QuoteStatus.Viewed,
    3 => QuoteStatus.Accepted,
    4 => QuoteStatus.Rejected,
    5 => QuoteStatus.Expired,
    _ => QuoteStatus.Draft
};

await service.UpdateStatusAsync(id, newStatus);
   
    Console.WriteLine("Quotation Updated Successfully!");
}


// DELETE

static async Task SoftDelete(IQuotationService service)
{
    Console.Write("Enter Quotation ID: ");
    int.TryParse(Console.ReadLine(), out int id);

    await service.SoftDeleteAsync(id);
    Console.WriteLine("Quotation Soft Deleted Successfully!");
}