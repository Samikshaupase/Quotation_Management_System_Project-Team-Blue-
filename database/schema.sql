-- Quotations Table
CREATE TABLE Quotations (
    QuotationId INT PRIMARY KEY,
    QuoteNumber VARCHAR(100) NOT NULL,
    QuoteDate DATETIME NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    Status INT NOT NULL,
    DiscountAmount DECIMAL(10,2),
    TotalAmount DECIMAL(10,2),
    IsDeleted BIT DEFAULT 0,
    Version INT,
    ParentQuotationId INT,
    LeadId INT,
    CustomerId INT
);

-- Quotation Line Items Table
CREATE TABLE QuotationLineItems (
    LineItemId INT PRIMARY KEY,
    ProductName VARCHAR(100),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    LineTotal DECIMAL(10,2),
    IsDeleted BIT DEFAULT 0,
    QuotationId INT,
    FOREIGN KEY (QuotationId) REFERENCES Quotations(QuotationId)
);

-- Quotation Templates Table
CREATE TABLE QuotationTemplates (
    TemplateId INT PRIMARY KEY,
    TemplateName VARCHAR(100),
    TemplateContent TEXT,
    CreatedDate DATETIME
);