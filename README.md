# 📚 MVC-Booksy

**MVC-Booksy** is a modern e-commerce application built using **ASP.NET Core MVC**. This project demonstrates clean architecture, advanced .NET practices, localization, distributed caching, messaging, and dynamic PDF generation.

---

## ✨ Key Features

- **Modular Architecture**  
  Clean separation of concerns: `Core`, `Infrastructure`, and `UI` layers.

- **Multilingual Support**  
  Supports **English** and **Urdu** via ASP.NET Core localization (resource-based).

- **Distributed Messaging**  
  Uses **MassTransit** with **RabbitMQ** for asynchronous background processing.

- **Caching**  
  Integrated **Redis** for distributed caching and performance optimization.

- **PDF Generation**  
  Generates dynamic PDFs using `wkhtmltopdf`.

- **Custom Exception Handling**  
  Global error handling with user-friendly error pages.

- **Session Management**  
  Secure and persistent user sessions.

- **Design Patterns Implemented**
  - **Repository Pattern** – Abstracts data access.
  - **Service Layer Pattern** – Encapsulates business logic.
  - **Builder Pattern** – Builds complex objects step-by-step.
  - **Specification Pattern** – Reusable and composable business rules.
  - **Decorator Pattern** – Add behavior dynamically to services.

---

## 📁 Project Structure

MVC-Booksy/
├── src/
│ ├── UI/ → ASP.NET Core MVC web app
│ ├── Core/ → Domain models, interfaces, specifications
│ └── Infrastructure/ → EF Core, services, RabbitMQ, Redis

├── EcomerceApp.sln → Solution file



---

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [wkhtmltopdf](https://wkhtmltopdf.org/downloads.html) native library
- [Redis](https://redis.io/docs/getting-started/installation/) server
- [RabbitMQ](https://www.rabbitmq.com/download.html) server

---

### 🛠 Running Locally

1. **Clone the repository**  
   ```bash
   git clone https://github.com/TamoorAsif87/MVC-Booksy.git
   cd MVC-Booksy

2. **Restore dependencies and build**
   dotnet restore
   dotnet build
3. **Ensure infrastructure is running**

  Redis and RabbitMQ should be running (you can use Docker or install locally)
4. **Run the applicatio**
    dotnet run --project src/UI/UI
5. **Access the app**
    Visit: https://localhost:5001 (or configured port)


🌐 Localization
Supports:

English (en-US)

Urdu (ur-PK)

Language can be set via:

Browser settings

Query string

Cookies


📬 Messaging
Uses MassTransit with RabbitMQ for background job processing and service communication.

Messages are handled asynchronously with retry, timeout, and fault policies.


🧠 Caching
Redis is used to cache:

Product data

User sessions

Frequently accessed queries


🧾 PDF Export
Powered by wkhtmltopdf

Used to export invoices, order summaries, or reports


🚨 Error Handling
Centralized ExceptionMiddleware

Friendly error pages (404, 500, etc.)

Logs to console and file (can extend to use Serilog, etc.)


🪪 License
This project is licensed under the MIT License.
See LICENSE file for details.





