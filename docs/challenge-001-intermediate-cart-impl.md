---
title: Challenge 001 Cart Implementation
description: Screening instructions for implementing the CartService.SaveAsync method in an ASP.NET Core shopping cart system using EF Core and domain-driven design
author: StoreTech Evaluation Team
ms.date: 2026-03-26
ms.topic: how-to
keywords:
  - asp.net core
  - entity framework core
  - shopping cart
  - implementation
estimated_reading_time: 8
---

## Overview

This project evaluates C# and ASP.NET Core proficiency through practical implementation of a domain-driven shopping cart system.

## Pre-configured Components

You have a working ASP.NET Core application with:

* ✅ Product entity with PriceInfo (one-to-many) relationship (`Domain/Product.cs`, `Domain/PriceInfo.cs`)
* ✅ TaxInfo representing Canadian provincial and federal taxes (`Domain/TaxInfo.cs`)
* ✅ Cart entity as the shopping cart aggregate root (`Domain/Cart.cs`, `Domain/CartItem.cs`)
* ✅ EF Core migrations and SQLite database schema
* ✅ Seed data for StoreTech products and Canadian tax rates (`Data/Seed/StoreTechSeedData.cs`)
* ✅ Request/response contracts prepared (`Contracts/Cart/CartSaveRequest.cs`, `Contracts/Cart/CartSaveResponse.cs`)
* ✅ xUnit test suite with test cases ready for validation (`tests/StoreTech.Eval.Tests/Services/CartServiceTests.cs`)

## Your Tasks

### 1. Implement CartService.SaveAsync()

**Location:** `Services/CartService.cs`

**Method signature:**

```csharp
public async Task<CartSaveResponse> SaveAsync(
    CartSaveRequest request,
    CancellationToken cancellationToken = default)
```

**Requirements:**

1. **Cart Creation/Retrieval**
   * If `CartId` is null, create a new `Cart` with the provided region and currency code
   * If `CartId` has a value, retrieve the persisted cart by querying `dbContext.Carts.Include(c => c.Items)` and load the existing items

2. **Product Validation**
   * Call `IProductService.GetProductPricingAsync(request.ProductId, request.CurrencyCode, DateTimeOffset.UtcNow)` to validate and resolve the product
   * This method throws `ProductNotFoundException` when the product ID is invalid

3. **Add Product to Cart**
   * Use an upsert pattern on the cart's `Items` collection, matching by `ProductId`
   * If the product already exists in the cart, increment the quantity and update the unit price
   * Otherwise, add a new `CartItem`

4. **Calculate Subtotal**
   * Sum all `CartItem.LineSubtotal` values across the cart's items

5. **Retrieve Applicable Taxes**
   * Call `ITaxService.GetTaxesForRegionAsync(region, currencyCode, date)` to get all applicable tax rates for the cart's region

6. **Persist and Respond**
   * Call `dbContext.SaveChangesAsync()` to persist all tracked changes
   * Use `CartResponseMapper.ToResponse()` to build the `CartSaveResponse`
   * Return the response

### 2. Consider Edge Cases

* Invalid or non-existent product IDs
* Currency codes with no matching price information
* Invalid region codes
* Null or zero quantities

## Project Structure

```text
Contracts/
    Cart/
        CartItemDto.cs
        CartSaveRequest.cs
        CartSaveResponse.cs
        CartSummaryDto.cs
        TaxBreakdownDto.cs
    Product/
        ProductDto.cs
Controllers/
    CartController.cs                     (POST /api/carts)
    ProductController.cs                  (GET /api/products)
Data/
    StoreTechDbContext.cs
    Configurations/
        CartConfiguration.cs
        CartItemConfiguration.cs
        PriceInfoConfiguration.cs
        ProductConfiguration.cs
        TaxInfoConfiguration.cs
    Seed/
        StoreTechSeedData.cs
Domain/
    Cart.cs                               (Aggregate Root)
    CartItem.cs
    CartType.cs
    PriceInfo.cs                          (Value Object)
    Product.cs                            (Aggregate Root)
    TaxInfo.cs
Services/
    CartResponseMapper.cs
    CartService.cs                        ⭐ IMPLEMENT THIS
    CartWorkflowNormalization.cs
    ICartService.cs
    IProductService.cs
    ITaxService.cs
    ProductPricingResult.cs
    ProductService.cs
    ServiceExceptions.cs
    TaxLookupResult.cs
    TaxService.cs
tests/
    StoreTech.Eval.Tests/
        Services/
            CartServiceTests.cs           ⭐ TEST YOUR IMPLEMENTATION
```

## Database Schema

### Tables

* **Carts**: shopping cart aggregate with region, currency code, subtotal, tax total, and total
* **CartItems**: line items linking a cart to a product with quantity, unit price, and line subtotal
* **Products**: product catalog with SKU and name
* **PriceInfos**: product pricing in different currencies with effective dates (one-to-many with Products)
* **TaxInfos**: Canadian tax rates by region with effective dates

### Sample Data

**Products (5 StoreTech items, all CAD):**

| Product | SKU | Price (CAD) |
|---|---|---|
| StoreTech Coffee Beans | SKU-COFFEE-001 | $18.99 |
| StoreTech Ceramic Mug | SKU-MUG-001 | $12.49 |
| StoreTech Claw Hammer | SKU-HAMMER-001 | $24.99 |
| StoreTech Screwdriver Set | SKU-SCREWDRIVER-001 | $19.99 |
| StoreTech Hand Saw | SKU-SAW-001 | $29.99 |

**Taxes (Canadian, effective 2026-01-01 to open-ended):**

| Region | Tax | Rate |
|---|---|---|
| CA-ON (Ontario) | HST | 13% |
| CA-AB (Alberta) | GST | 5% |

## API Endpoints

### Get Products with Pricing

```text
GET /api/products
Response: List of ProductDto with current prices
```

### Add Product to Cart

```text
POST /api/carts
```

**Request body (`CartSaveRequest`):**

```json
{
  "cartId": null,
  "productId": 1,
  "quantity": 2,
  "region": "ON",
  "currencyCode": "CAD"
}
```

**Response body (`CartSaveResponse`):**

```json
{
  "cartId": "a1b2c3d4-...",
  "totalItems": 2,
  "subtotal": 37.98,
  "cartType": "Regular",
  "createdAt": "2026-03-26T...",
  "currencyCode": "CAD",
  "region": "CA-ON",
  "items": [...],
  "taxBreakdown": [...],
  "success": true,
  "message": "Cart created successfully."
}
```

> [!NOTE]
> The `cartId` field is a `Guid`. Pass `null` for new carts. The `region` field accepts short codes like `ON` or full codes like `CA-ON`; normalization is handled by `CartWorkflowNormalization`.

## Testing

Run the test suite to verify your implementation:

```bash
dotnet test --filter "FullyQualifiedName~CartServiceTests"
```

**Test cases:**

1. ✅ Add a valid product to a new cart (no cart ID)
2. ✅ Add a valid product to an existing cart (increments quantity)
3. ✅ Add a second distinct product to an existing cart
4. ✅ Reject region changes for an existing cart (should throw `CartScopeMismatchException`)

## Domain-Driven Design Notes

* `PriceInfo` is a value object accessible through its aggregate root (`Product`) via navigation properties
* `Cart` is an aggregate root that manages its own `CartItems` collection
* `TaxInfo` is immutable reference data representing tax rules
* EF Core handles persistence through change tracking: all modifications are persisted with a single `SaveChangesAsync()` call
* Entities validate their own invariants at the domain boundary

## Key Classes to Review

Before implementing, review these files:

1. `Domain/Product.cs`: the `PriceInfo` navigation property relationship
2. `Services/CartService.cs`: helper methods (`GetOrCreateCartAsync`, `UpsertItem`, `RecalculateTotals`) and their contracts
3. `Contracts/Cart/CartSaveRequest.cs` and `Contracts/Cart/CartSaveResponse.cs`: the request and response DTOs
4. `tests/StoreTech.Eval.Tests/Services/CartServiceTests.cs`: expected test behavior and assertions

## Database Connection

The project uses SQLite with EF Core. Configuration lives in `appsettings.json`:

* Connection string: `Data Source=storetech.db`
* EF Core migrations are applied on startup
* Seed data is loaded via EF Core `HasData` in the model configuration (`Data/Seed/StoreTechSeedData.cs`)

## Hints

1. `StoreTechDbContext` provides `Carts`, `Products`, `Prices`, and `Taxes` DbSet properties
2. Use LINQ `.Include()` and `.ThenInclude()` for eager loading related entities
3. `IProductService.GetProductPricingAsync` handles product lookup and price resolution for a given currency
4. `ITaxService.GetTaxesForRegionAsync` handles tax lookup for a given region and currency
5. `CartResponseMapper.ToResponse()` builds the response DTO from a fully loaded cart and tax results
6. `CartWorkflowNormalization` handles input normalization for currency codes and region identifiers
7. EF Core change tracking persists all modifications via a single `SaveChangesAsync()` call; no explicit repository save is needed

## Success Criteria

* ✅ All 4 test cases pass
* ✅ Code follows ASP.NET Core best practices
* ✅ Proper exception handling for invalid inputs
* ✅ Code is readable and maintainable
* ✅ Domain model is respected (DDD principles)
