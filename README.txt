# Finance Tracker API

![CI](https://github.com/AyaAhmed18/FinanceTracker/actions/workflows/ci.yml/badge.svg)

A RESTful API for tracking personal income and expenses, with JWT authentication, per-user data isolation, and monthly financial reports.

<!-- After deploying (Step 7), add: **Live Swagger:** https://your-app-name.azurewebsites.net/swagger -->

## Features

- User registration and login with **JWT** authentication
- Passwords hashed with ASP.NET Core `PasswordHasher` (PBKDF2 with salt)
- **Categories** CRUD (Income / Expense)
- **Transactions** CRUD with filtering by date range, category, and type
- **Data isolation:** every query is scoped to the authenticated user
- **Monthly report:** total income, total expenses, balance, and a breakdown by category (computed in the database with LINQ `GroupBy`)
- Input validation and consistent error responses
- Swagger UI with JWT support
- Unit tests with **xUnit**, **Moq**, and **FluentAssertions**
- Automated build and test pipeline with **GitHub Actions**

## Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Data access | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | JWT Bearer |
| Testing | xUnit, Moq, FluentAssertions, EF Core InMemory |
| CI | GitHub Actions |
| Docs | Swagger / OpenAPI |

## Project Structure

```
FinanceTracker/
├── FinanceTracker.Api/
│   ├── Controllers/    # Auth, Categories, Transactions, Reports
│   ├── Data/           # AppDbContext
│   ├── Dtos/           # Request and response models
│   ├── Entities/       # User, Category, Transaction
│   ├── Services/       # TokenService, ReportService
│   └── Program.cs
├── FinanceTracker.Tests/
│   ├── ReportServiceTests.cs
│   ├── TokenServiceTests.cs
│   └── ReportsControllerTests.cs
└── .github/workflows/ci.yml
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Create an account | No |
| POST | `/api/auth/login` | Log in and receive a JWT | No |
| GET | `/api/categories` | List categories | Yes |
| GET | `/api/categories/{id}` | Get a category | Yes |
| POST | `/api/categories` | Create a category | Yes |
| PUT | `/api/categories/{id}` | Update a category | Yes |
| DELETE | `/api/categories/{id}` | Delete a category (only if it has no transactions) | Yes |
| GET | `/api/transactions` | List transactions (filters: `from`, `to`, `categoryId`, `type`) | Yes |
| GET | `/api/transactions/{id}` | Get a transaction | Yes |
| POST | `/api/transactions` | Create a transaction | Yes |
| PUT | `/api/transactions/{id}` | Update a transaction | Yes |
| DELETE | `/api/transactions/{id}` | Delete a transaction | Yes |
| GET | `/api/reports/monthly?year=2026&month=10` | Monthly income, expenses, balance, and category breakdown | Yes |

### Example: monthly report response

```json
{
  "year": 2026,
  "month": 10,
  "totalIncome": 5000,
  "totalExpenses": 500,
  "balance": 4500,
  "byCategory": [
    { "categoryId": 1, "categoryName": "Salary", "type": 1, "total": 5000, "count": 1 },
    { "categoryId": 2, "categoryName": "Food", "type": 2, "total": 500, "count": 2 }
  ]
}
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### Run locally

1. Clone the repository:
   ```bash
   git clone https://github.com/AyaAhmed18/FinanceTracker.git
   cd FinanceTracker
   ```
2. Update the connection string and JWT settings in `FinanceTracker.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=FinanceTrackerDb;Trusted_Connection=True;TrustServerCertificate=True"
   },
   "Jwt": {
     "Key": "use-a-long-random-secret-at-least-32-characters",
     "Issuer": "FinanceTracker",
     "Audience": "FinanceTrackerUsers",
     "ExpiryMinutes": 60
   }
   ```
3. Create the database:
   ```bash
   dotnet ef database update --project FinanceTracker.Api
   ```
4. Run the API:
   ```bash
   dotnet run --project FinanceTracker.Api
   ```
5. Open Swagger at `https://localhost:<port>/swagger`.

### Using the API in Swagger

1. Call `POST /api/auth/register` (or `/login`) and copy the `token` value.
2. Click **Authorize** and paste the token (without the `Bearer` prefix).
3. Create categories, add transactions, and call the monthly report.

> **Security note:** never commit real secrets. In production, set `Jwt__Key` and the connection string through environment variables or your hosting platform's configuration.

## Running the Tests

```bash
dotnet test
```

The tests cover:

- **ReportService:** income, expenses, and balance calculation; grouping by category; month boundaries; isolation between users; empty months
- **TokenService:** token claims and expiry
- **ReportsController:** input validation and correct user resolution, using Moq

Service tests use the EF Core InMemory provider, so they run without SQL Server.

## CI/CD

Every push and pull request to `main` triggers a GitHub Actions workflow that restores, builds, and runs all tests (`.github/workflows/ci.yml`).

## Roadmap

- [ ] Deploy to Azure App Service with Application Insights
- [ ] Add pagination to the transactions endpoint
- [ ] Add global exception-handling middleware and FluentValidation
- [ ] Add integration tests

## Author

**Aya Ahmed**, Full Stack .NET Developer
[GitHub](https://github.com/AyaAhmed18) | [LinkedIn](https://www.linkedin.com/in/aya-ahmed-a71a57213)
