# TurnDigitalTask

This repository contains a .NET-based application developed as a task submission for Turn Digital, following Clean Architecture principles for maintainability and separation of concerns.

## Project Structure

- **Domain**: Core business entities and logic.
- **Application**: Use cases, services, and validation.
- **Infrastructure**: Data access and external service integration.
- **Web**: ASP.NET Core API & Server side rendered razor views.
- **UnitTest & IntegrationTests**: Testing projects.

## Technologies & Packages Used

- .NET Core
- ASP.NET Core
- Entity Framework Core (In-Memory Database)
- MediatR
- Serilog (Logging)
- Mapster (For object to object mapping)
- FluentValidation
- Bogus (Fake data generation)
- Moq (Unit testing)
- XUnit (Unit & Integration Testing)
- Language.Ext (For functional types as the Either monad)
- Mini-Profiler (For profiling the request-response) visit /profiler/results-index
- Swagger (For api documentation) visit /swagger
- Microsoft Identity For Authentication & Authorization

## Features

- **CQRS Pattern**: Separate repository interfaces for read and write operations, with read repository disabling EF Core change tracking.
- **Authentication**: Supports both cookie and JWT authentication.
- **Logging**: Uses Serilog to log information to the console and warnings to files in the `Web/logs` folder.
- **Action Auditing**: Implements action auditing via EF Core's `SaveChanges` interceptor.
- **Domain Events**: Processes domain events using EF Core's `SaveChanges` interceptor.
- **API Versioning**: Supports versioning for product APIs with differences in validation for creating and updating products.
- **Response Mapping**: Implements proper application layer response to http response mapping using the Result pattern

### API Versions

- **Version 1**:
  - Create Product: No validation for unique product name or valid category.
  - Update Product: Similar to Create, lacks validation checks.
- **Version 2**:
  - Create and Update Product: Validates that the product name is unique and the associated category is valid.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/kirollos-hany/TurnDigitalTask.git
