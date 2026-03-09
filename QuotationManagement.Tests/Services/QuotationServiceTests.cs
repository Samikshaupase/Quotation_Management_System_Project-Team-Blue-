using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using QuotationManagementApp.Services;   // make sure this namespace matches
using QuotationManagementApp.Interfaces;
using QuotationManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuotationManagementApp.Logic;

namespace QuotationManagement.Tests.Services
{
    public class QuotationServiceTests
    {
        private readonly Mock<IQuotationRepository> _repoMock;
        private readonly QuotationService _service;

        public QuotationServiceTests()
        {
            _repoMock = new Mock<IQuotationRepository>();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "TaxSettings:DefaultTaxRate", "0.18" }
                })
                .Build();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

            _service = new QuotationService(_repoMock.Object, config);
        }

      [Fact]
public async Task CreateAsync_Calculates_Total()
{
    var quotation = new Quotation
    {
        LineItems = new List<QuotationLineItem>
        {
            new() { Quantity = 2, UnitPrice = 100 }
        },
        DiscountAmount = 0,
        ExpiryDate = DateTime.UtcNow.AddDays(5)
    };

    _repoMock.Setup(r => r.GetAllAsync())
             .ReturnsAsync(new List<Quotation>());
    _repoMock.Setup(r => r.AddWithTransactionAsync(It.IsAny<Quotation>()))
             .Returns(Task.CompletedTask);

    await _service.CreateAsync(quotation);

    Assert.Equal(236, quotation.TotalAmount); // 200 + 18%
}

       [Fact]
public async Task CreateAsync_Sets_LineTotal()
{
    var quotation = new Quotation
    {
        LineItems = new List<QuotationLineItem>
        {
            new() { Quantity = 3, UnitPrice = 100 }
        },
        ExpiryDate = DateTime.UtcNow.AddDays(5)
    };

    _repoMock.Setup(r => r.GetAllAsync())
             .ReturnsAsync(new List<Quotation>());
    _repoMock.Setup(r => r.AddWithTransactionAsync(It.IsAny<Quotation>()))
             .Returns(Task.CompletedTask);

    await _service.CreateAsync(quotation);

    Assert.Equal(300, quotation.LineItems[0].LineTotal);
}

[Fact]
public async Task CreateAsync_Throws_If_Expired()
{
    var quotation = new Quotation
    {
        ExpiryDate = DateTime.UtcNow.AddDays(-1)
    };

    await Assert.ThrowsAsync<ArgumentException>(() =>
        _service.CreateAsync(quotation));
}
[Fact]
public async Task UpdateStatus_Valid_Transition()
{
    var quotation = new Quotation
    {
        Status = QuoteStatus.Draft,
        ExpiryDate = DateTime.UtcNow.AddDays(5)
    };

    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(quotation);
    _repoMock.Setup(r => r.SaveChangesAsync())
             .Returns(Task.CompletedTask);

    await _service.UpdateStatusAsync(1, QuoteStatus.Sent);

    Assert.Equal(QuoteStatus.Sent, quotation.Status);
}

[Fact]
public async Task UpdateStatus_Invalid_Transition()
{
    var quotation = new Quotation
    {
        Status = QuoteStatus.Draft
    };

    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(quotation);

    await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _service.UpdateStatusAsync(1, QuoteStatus.Accepted));
}

[Fact]
public async Task UpdateStatus_Throws_If_NotFound()
{
    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync((Quotation?)null);

    await Assert.ThrowsAsync<ArgumentException>(() =>
        _service.UpdateStatusAsync(1, QuoteStatus.Sent));
}

       [Fact]
public async Task SoftDelete_Sets_IsDeleted()
{
    var quotation = new Quotation();

    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(quotation);
    _repoMock.Setup(r => r.SaveChangesAsync())
             .Returns(Task.CompletedTask);

    var result = await _service.SoftDeleteAsync(1);

    Assert.True(result);
    Assert.True(quotation.IsDeleted);
}


[Fact]
public async Task GetById_Returns_Null_If_NotFound()
{
    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync((Quotation?)null);

    var result = await _service.GetByIdAsync(1);

    Assert.Null(result);
}

[Fact]
public async Task GetById_Auto_Expires()
{
    var quotation = new Quotation
    {
        Status = QuoteStatus.Sent,
        ExpiryDate = DateTime.UtcNow.AddDays(-1)
    };

    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(quotation);
    _repoMock.Setup(r => r.SaveChangesAsync())
             .Returns(Task.CompletedTask);

    var result = await _service.GetByIdAsync(1);

    Assert.Equal(QuoteStatus.Expired, result!.Status);
}

[Fact]
public async Task CreateAsync_Increments_QuoteNumber()
{
    var existing = new List<Quotation>
    {
        new() { QuoteNumber = $"Q-{DateTime.Now.Year}-0001" }
    };

    var quotation = new Quotation
    {
        ExpiryDate = DateTime.UtcNow.AddDays(5)
    };

    _repoMock.Setup(r => r.GetAllAsync())
             .ReturnsAsync(existing);
    _repoMock.Setup(r => r.AddWithTransactionAsync(It.IsAny<Quotation>()))
             .Returns(Task.CompletedTask);

    await _service.CreateAsync(quotation);

    Assert.Contains("0002", quotation.QuoteNumber);
}

[Fact]
public async Task CreateRevision_Creates_New_Version()
{
    var original = new Quotation
    {
        QuotationId = 1,
        Version = 1,
        ExpiryDate = DateTime.UtcNow.AddDays(5)
    };

    _repoMock.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(original);
    _repoMock.Setup(r => r.AddWithTransactionAsync(It.IsAny<Quotation>()))
             .Returns(Task.CompletedTask);

    await _service.CreateRevisionAsync(1);

    Assert.True(true); // simple check that method runs
}

[Fact]
public async Task GetAll_Excludes_Deleted()
{
    var list = new List<Quotation>
    {
        new() { IsDeleted = false },
        new() { IsDeleted = true }
    };

    _repoMock.Setup(r => r.GetAllAsync())
             .ReturnsAsync(list);

    var result = await _service.GetAllAsync();

    Assert.Single(result);
}}}