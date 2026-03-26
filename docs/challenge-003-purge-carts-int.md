---
title: Challenge 003 Purge Carts Intermediate
description: >-
  Intermediate screening challenge for rewriting the cart cleanup process
  as an automated background service while preserving existing functionality.
author: StoreTech Evaluation Team
ms.date: 2026-03-26
ms.topic: how-to
keywords:
  - asp.net core
  - cart cleanup
  - background service
  - refactoring
estimated_reading_time: 8
---

## Overview

This challenge evaluates your ability to improve an existing feature that is
correct in places, but poorly implemented and difficult to maintain.

You are given a user story to improve the cart cleanup process:

> Barb has been complaining about the cart cleanup job that Ron, our junior
> developer, worked on last year. It takes a long time to run, you have to call
> a web endpoint manually to execute it, it only cleans up carts from 24 hours
> ago, and it deletes carts even if the cart was updated within the last 24
> hours -- this should be fixed if we're already going into the code. We need
> changes to the functionality as soon as possible.
>
> Jerry, one of our intermediate developers, says the current implementation is
> **throw away**, but we still need to preserve the functionality.

## Acceptance Criteria

1. Required: The cart cleanup functionality runs as an automated process every
   5 minutes.
2. Required: The implementation uses modern patterns and coding standards.
3. Required: The old purge endpoint is no longer accessible.
4. Required: Existing rules are preserved exactly after the rewrite.
5. Required: The implementation does not rely on a manual HTTP request to
   execute cleanup.
6. Required: The cleanup logic is covered by automated tests.
7. **Strongly Recommended**: Carts that have been updated (added/removed, etc.)
   within the cutoff period should not be deleted. The create date of the cart
   should not drive the functionality. (Note: the `Cart` entity currently uses
   `CreatedAtUtc`; you may need to add a `LastModifiedAtUtc` property via an
   EF Core migration.)
8. **Recommended**: **ALL** schedule and cleanup cutoff settings are easy to
   change later. (Note: `CartOptions` with `StaleCartDays` and
   `RewardEligibleRetentionDays` already uses the `IOptions<T>` pattern with
   `ValidateOnStart`.)
9. **Recommended**: The rewrite avoids loading unnecessary data into memory and
   is **measurably cleaner** than the current implementation.
10. **Nice to have**: Clean as you code.

## What To Implement

Rewrite the cart cleanup flow as an automated process using ASP.NET Core's
built-in `BackgroundService`.

## Candidate Tasks

1. Replace the current manual cleanup trigger with a `BackgroundService`
   implementing `ExecuteAsync`.
2. The job runs every 5 minutes using `PeriodicTimer`.
3. Preserve the existing cleanup business rules while rewriting the
   implementation, and **refactor where appropriate**. Remember, Barb wants
   carts updated within the cutoff to be exempt from the purge.
4. Remove or disable the old purge-stale endpoint from `CartController` so it
   is no longer accessible.
5. Use `IServiceScopeFactory` to resolve scoped services
   (`StoreTechDbContext`, `ICartService`) from the singleton hosted service.
6. Add or update tests to prove the cleanup behavior still works.

## Implementation Expectations

Review the current behavior before changing anything. The screening goal is not
to invent new cleanup rules. It is to modernize the implementation without
breaking the rules that already exist.

Use ASP.NET Core's built-in `BackgroundService` and `PeriodicTimer` for the
baseline solution. If you think a dedicated scheduling framework (Hangfire,
Quartz.NET) should be introduced, explain why.

The expected direction is:

1. Move cleanup execution away from the controller layer into a hosted service.
2. **Keep responsibilities clear between scheduling, orchestration, and
   database operations**, adhering to Single Responsibility Principle.
3. Rewrite the current implementation rather than layering new behavior on top
   of the existing flow.

## Hints

1. `PurgeStaleCartsAsync` in `CartService.cs` contains the existing cleanup
   logic. Unlike the Java project where this was intentionally a poor
   implementation, the .NET version works correctly. Focus on automation and
   endpoint removal.
2. There is currently no `BackgroundService` or `IHostedService` in the
   application. You will create one.
3. `BackgroundService` is a singleton; `StoreTechDbContext` and `ICartService`
   are scoped. Use `IServiceScopeFactory.CreateAsyncScope()` to obtain scoped
   services in each timer tick.
4. Existing test coverage in
   `tests/StoreTech.Eval.Tests/Data/CartPersistenceTests.cs` validates purge
   behavior. Build on these tests rather than starting from scratch.
5. `CartOptions` already supports configurable retention via
   `IOptions<CartOptions>` with `StaleCartDays` and
   `RewardEligibleRetentionDays`. Introducing schedule configuration (purge
   interval) into `CartOptions` is a natural extension.
6. If adding `LastModifiedAtUtc` to `Cart`, create a new EF Core migration
   using `dotnet ef migrations add AddCartLastModifiedAt`.

## Files To Review

Before implementing, review the existing purge-related code and tests.

```text
Services/CartService.cs                                  (PurgeStaleCartsAsync method)
Services/ICartService.cs                                 (service interface)
Controllers/CartController.cs                            (purge-stale endpoint to remove)
Options/CartOptions.cs                                   (retention configuration)
Domain/Cart.cs                                           (cart entity)
Domain/CartType.cs                                       (cart type enum)
tests/StoreTech.Eval.Tests/Data/CartPersistenceTests.cs  (existing purge tests)
```

## What We Are Evaluating

This challenge shows whether you can:

* Recognize when existing code should be rewritten rather than incrementally
  patched
* Apply automation in a clean, performant, production-appropriate way
* Preserve logic during refactoring to prevent escaping defects
* Improve maintainability, readability, and execution flow
* Write tests that protect the cleanup behavior during change

## Deliverable Guidance

A strong solution will:

* Schedule the cleanup automatically using `BackgroundService` with
  `PeriodicTimer`
* Remove the manual trigger path from normal usage
* Keep the business logic intact
* Make future schedule changes straightforward
* Leave the code in better shape than it was found
