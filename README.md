# MVC-Booksy

MVC-Booksy is a modern e-commerce application built using ASP.NET Core MVC. This project demonstrates advanced .NET development practices, modular design, and integration with distributed systems.

## Key Features

- **Modular Architecture:** Clear separation of concerns with core, infrastructure, and UI layers.
- **Multilingual Support:** English and Urdu languages (localization via ASP.NET Core).
- **Distributed Messaging:** Integration with **MassTransit** and **RabbitMQ** for robust asynchronous processing.
- **Caching:** Utilizes **Redis** for distributed caching and improved performance.
- **PDF Generation:** Uses `wkhtmltopdf` for dynamic PDF exports.
- **Custom Exception Handling:** User-friendly error pages and centralized error management.
- **Session Management:** Secure user sessions.
- **Docker Support:** Containerized deployment with Docker and Docker Compose.
- **Design Patterns:** Implements several design patterns for maintainable and scalable code:
  - **Repository Pattern:** Abstracts data access logic.
  - **Service Layer Pattern:** Encapsulates business logic.
  - **Builder Pattern:** For constructing complex objects.
  - **Specification Pattern:** For flexible and reusable business rules.
  - **Decorator Pattern:** For adding responsibilities dynamically.

## Project Structure

- `/src` - Source code
  - `/UI` - ASP.NET Core MVC web application
  - Other layers (Core, Infrastructure, etc.)
- `EcomerceApp.sln` - Solution file
- `docker-compose.yml` - Docker configuration

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started) (optional)
- **wkhtmltopdf** library
- **Redis** server
- **RabbitMQ** server

### Running Locally

1. **Clone the repository:**
    ```sh
    git clone https://github.com/TamoorAsif87/MVC-Booksy.git
    cd MVC-Booksy
    ```

2. **Restore dependencies and build:**
    ```sh
    dotnet restore
    dotnet build
    ```

3. **Setup infrastructure:**  
   Ensure Redis and RabbitMQ are running (use Docker for convenience).

4. **Run the application:**
    ```sh
    dotnet run --project src/UI/UI
    ```

5. **Access the app:**  
   Visit `https://localhost:5001` (or as configured).

### Using Docker

1. **Build and run all services:**
    ```sh
    docker-compose up --build
    ```

## Localization

Supports English (en-US) and Urdu (ur-PK). Select language via browser, query string, or cookies.

## Distributed Systems

- **MassTransit** is used with **RabbitMQ** to enable asynchronous messaging between services.
- **Redis** is configured for distributed caching.

## Error Handling

Centralized custom exception handling and user-friendly error pages.

## License

MIT License. See [`LICENSE`](LICENSE).

---

For more details, explore the [repository files](https://github.com/TamoorAsif87/MVC-Booksy/tree/main).
