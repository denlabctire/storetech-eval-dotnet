---
title: Challenge 002 Concurrency Screening
description: >-
  Screening instructions for implementing a concurrency-safe inventory
  reservation flow with a required serializable transaction approach
author: StoreTech Evaluation Team
ms.date: 2026-03-26
ms.topic: how-to
keywords:
  - asp.net core
  - entity framework core
  - sqlite
  - concurrency
  - reservation
  - serializable transactions
estimated_reading_time: 7
---

## Overview

This challenge evaluates your ability to implement a concurrency-safe reservation
workflow in ASP.NET Core with EF Core and SQLite.

You will complete the missing reservation flow so concurrent requests cannot
oversell inventory. The required baseline solution uses serializable
transactions.

## Why This Challenge Uses Serializable Transactions

The exercise simulates 50 concurrent reservation attempts for a single SKU with
only 10 units available.

SQLite does not support row-level locks or `SELECT FOR UPDATE` syntax. Instead,
SQLite uses file-level locking with three transaction modes: `DEFERRED`,
`IMMEDIATE`, and `EXCLUSIVE`.

When EF Core opens a transaction with `IsolationLevel.Serializable`, the SQLite
provider issues `BEGIN IMMEDIATE`. This acquires a reserved lock on the database
file at the start of the transaction, preventing other writers from beginning a
transaction until the first one commits or rolls back. The result is
database-level write exclusion: concurrent requests block rather than fail with
conflicts.

Serializable transactions are required for the screening baseline because they
produce deterministic outcomes under contention and keep the exercise focused on
transaction correctness, service design, and API behavior.

Optimistic locking (EF Core concurrency tokens with `[ConcurrencyCheck]` or
`[Timestamp]`) can also solve this class of problem, but it adds retry and
conflict-handling concerns that are not the primary goal of this screening.

## What To Implement

### Candidate Tasks

1. Create the controller endpoint: `POST /api/reservations/reserve` in a new
   `ReservationController`
2. Create domain entities: `InventoryItem` (Id, Sku, QuantityAvailable,
   ProductId), `Reservation` (Id, Sku, Quantity, CustomerId, Status,
   CreatedAtUtc), and `ReservationStatus` enum (Confirmed, Failed)
3. Create `IReservationService` interface and `ReservationService`
   implementation
4. Implement `ReserveAsync()` using an `IsolationLevel.Serializable` transaction
   for inventory access
5. Create EF Core configuration and migration for the new entities
6. Return a successful reservation response when inventory is available
7. Return the correct error response when inventory is not available

## API Contract

### Request

```json
{
  "sku": "ABC",
  "quantity": 1,
  "customerId": 123
}
```

### Success Response

- HTTP `200`
- Response includes `id`, `sku`, `quantity`, `customerId`, `status`, and
  `createdAt`

### Failure Response

- Out-of-stock returns HTTP `409` (Conflict) with ProblemDetails (RFC 9457)
- Validation failure or malformed request returns HTTP `400` with ProblemDetails

## Acceptance Criteria

1. Use ASP.NET Core, EF Core, and SQLite
2. Use serializable transactions in the inventory workflow code
3. Ensure concurrency safety so parallel requests cannot oversell inventory
4. Prove the behavior with an integration test using `WebApplicationFactory`
5. Demonstrate this exact result for 50 parallel attempts against inventory
   of 10:
   - 10 successful reservations
   - 40 failures due to out-of-stock
   - Final inventory of 0

## Implementation Expectations

Keep the implementation aligned with the current project layering:

`Controller -> Service -> DbContext -> Domain`

Follow the existing DTO and mapper patterns already used elsewhere in the
project.

Use transaction boundaries deliberately. The critical path is:

```text
1. Open the database connection explicitly
2. Begin a transaction with IsolationLevel.Serializable
3. Attach the transaction to DbContext via Database.UseTransaction()
4. Load the inventory row within the transaction
5. Check available quantity
6. Decrement inventory when sufficient stock exists
7. Create the reservation entity and persist changes via SaveChangesAsync
8. Commit the transaction
```

## What We Are Evaluating

This challenge is not only about choosing a transaction isolation level. It is
intended to show whether you can:

- Apply EF Core transactions correctly under concurrent load
- Keep controller, service, and persistence responsibilities clear
- Handle validation and failure conditions predictably
- Write a concurrency test that proves the required behavior instead of relying
  on assumptions

## Notes

Serializable transactions are the required baseline for this exercise.

If you want to explore a concurrency token variant (optimistic locking) after the
baseline is working, treat that as an optional extension rather than the primary
solution.
