---
title: "CTC Store Tech - Technical Interview"
description: "Technical evaluation project for CTC Store Tech candidates using ASP.NET Core, Entity Framework Core, and SQLite"
ms.date: 2026-03-26
---

## Overview

This is a technical evaluation project for CTC Store Tech candidates. The project is an ASP.NET Core application with Entity Framework Core, and SQLite.

## Technical Interview Setup Instructions

### Prerequisites

Before the interview, complete the following steps to set up your development environment.

### Step 1: Create a GitHub Account

If you don't already have a GitHub account:

1. Go to [github.com](https://github.com)
2. Click **Sign up** in the top right corner
3. Follow the registration process (provide email, create password, choose username)
4. Verify your email address

This should take about 2-3 minutes.

### Step 2: Fork the Repository

1. Navigate to the interview repository: [https://github.com/denlabctire/storetech-eval-dotnet](https://github.com/denlabctire/storetech-eval-dotnet)
2. Click the **Fork** button in the top right corner
3. Select your account as the destination for the fork
4. Wait for GitHub to create your fork (this takes a few seconds)

### Step 3: Clone Your Fork Locally

Open your terminal and run:

```bash
git clone https://github.com/YOUR_USERNAME/storetech-eval-dotnet.git
cd storetech-eval-dotnet
```

Replace `YOUR_USERNAME` with your actual GitHub username.

### Step 4: Set Up Your Development Environment

**Requirements:**

* .NET 10 SDK

**Verify your setup:**

```bash
dotnet --version
```

**Build the project:**

```bash
dotnet build
```

**Run the application:**

```bash
dotnet run
```

The application will start on `http://localhost:5136`

**Run the tests:**

```bash
dotnet test
```

The test project is located at `tests/StoreTech.Eval.Tests/`.

### Step 5: Make Your Changes

1. Work on the coding challenge as described in the `docs/` directory
2. Test your changes locally
3. Commit your changes:

```bash
git add .
git commit -m "Implement solution for technical challenge"
```

### Step 6: Push to Your Fork

```bash
git push origin main
```

If this is your first push, you will be prompted to authenticate.
The easiest way to authenticate is to use a Personal Access Token (PAT):

1. Go to [GitHub Settings](https://github.com/settings/profile)
2. Click on **Developer settings** in the left sidebar (at the bottom)
3. Click on **Personal access tokens** in the left sidebar
4. Click on **Generate new classic token**
5. Give your token a name (e.g., "Store Tech Interview Token")
6. Select the scopes you need (for pushing code, you typically need `repo` scope)
7. Click **Generate token** at the bottom of the page
8. Save the generated token somewhere secure (you won't be able to see it again)
9. Use this token when you authenticate in the terminal. Your username is your GitHub username, and the password is the Personal Access Token you generated.

### Step 7: Create a Pull Request

1. Go to your forked repository on GitHub: `https://github.com/YOUR_USERNAME/storetech-eval-dotnet`
2. You should see a banner saying **"This branch is X commits ahead of denlabctire:main"**
3. Click the **Contribute** button, then **Open pull request**
4. Add a title and description explaining your changes
5. Click **Create pull request**

## Project Structure

```text
storetech-eval-dotnet/
├── README.md
├── Program.cs
├── storetech-eval-dotnet.csproj
├── appsettings.json
├── appsettings.Development.json
├── Contracts/
│   ├── Cart/
│   │   ├── CartItemDto.cs
│   │   ├── CartSaveRequest.cs
│   │   ├── CartSaveResponse.cs
│   │   ├── CartSummaryDto.cs
│   │   └── TaxBreakdownDto.cs
│   └── Product/
│       └── ProductDto.cs
├── Controllers/
│   ├── CartController.cs
│   ├── HomeController.cs
│   └── ProductController.cs
├── Data/
│   ├── StoreTechDbContext.cs
│   ├── Configurations/
│   └── Seed/
├── Domain/
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── CartType.cs
│   ├── PriceInfo.cs
│   ├── Product.cs
│   └── TaxInfo.cs
├── Infrastructure/
│   └── GlobalExceptionHandler.cs
├── Migrations/
├── Options/
│   └── CartOptions.cs
├── Services/
│   ├── CartService.cs
│   ├── ICartService.cs
│   ├── IProductService.cs
│   ├── ITaxService.cs
│   ├── ProductService.cs
│   └── TaxService.cs
├── docs/
└── tests/
    └── StoreTech.Eval.Tests/
```

## Technologies Used

* ASP.NET Core (.NET 10)
* Entity Framework Core
* SQLite
* xUnit
* Microsoft.AspNetCore.Mvc.Testing

## Database

This project uses SQLite as a file-based database. No separate database server is required.

**Connection strings:**

| Environment | Connection String |
|---|---|
| Production | `Data Source=storetech.db` |
| Development | `Data Source=storetech.development.db` |

The database file is created automatically when the application runs. To browse the database, use [DB Browser for SQLite](https://sqlitebrowser.org/) or the `sqlite3` CLI:

```bash
sqlite3 storetech.development.db
```

## Troubleshooting

### .NET SDK not found

Verify the SDK is installed and available on your PATH:

```bash
dotnet --list-sdks
```

If no .NET 10 SDK appears, download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

### EF Core migration issues

If the database is in a bad state, delete the SQLite file and let migrations recreate it:

```bash
rm storetech.development.db
dotnet run
```

### Test failures

Run tests with detailed output to diagnose failures:

```bash
dotnet test --verbosity detailed
```

Ensure the main project builds successfully before running tests:

```bash
dotnet build
dotnet test
```
