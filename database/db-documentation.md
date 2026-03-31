# 🗄️ Database Documentation

## Overview

This project uses Entity Framework Core (Code-First).

## Tables

### Quotations

Stores quotation details.

### QuotationLineItems

Stores products for each quotation.

### QuotationTemplates

Stores reusable templates.

## Relationships

* One quotation → multiple line items

## Notes

* Enum stored as integer
* Soft delete using IsDeleted
