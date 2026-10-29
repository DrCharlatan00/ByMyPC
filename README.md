# ByMyPC

**ByMyPC** is a backend REST API for convenient storage, management, and retrieval of PC hardware components data (RAM, CPU, GPU, PSU, and more). It exposes Prometheus metrics, streams events to **Apache Kafka**, and uses **Redis** for caching to speed up responses.

## 📖 About

ByMyPC helps PC builders, hardware enthusiasts, and small businesses keep their component inventory organized. Instead of scattered spreadsheets, you get a structured database with a clean API.

Beyond storage, the service acts as a **metrics collector** and **event producer**: it gathers hardware metrics, exposes them in Prometheus format, and publishes events to Kafka for monitoring, analytics, or alerting pipelines.

### Features

- **Store & Manage:** Keep records of PC components (CPU, GPU, RAM, PSU, Motherboard, etc.).
- **Edit Data:** Update component specifications and details easily.
- **Search & Filter:** Find components by name with built-in filtering.
- **Pagination:** Retrieve large lists of components in manageable pages.
- **Prometheus Metrics:** Expose metrics at the `/metrics` endpoint for scraping.
- **Kafka Integration:** Stream metrics and events to Apache Kafka for downstream consumers.
- **Redis Caching:** Cache frequently requested data to reduce database load and speed up responses.
- **Real-time Updates:** Powered by SignalR (Hubs) for instant data updates.
- **PostgreSQL Storage:** Reliable and scalable data persistence.

## 🛠 Tech Stack

- **Language:** C# / .NET 10
- **Framework:** ASP.NET Core Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Caching:** Redis
- **Message Broker:** Apache Kafka
- **Metrics:** Prometheus (`/metrics`)
- **Real-time:** SignalR
- **Testing:** xUnit (Integration Tests)
- **Containerization:** Docker

## 📂 Solution Structure
ByMyPC.sln
├── ByMyPC/ # Main Web API (Controllers, Hubs, Metrics, Middlewares, Services)
├── ByMyPC_Postgresql/ # Data Access Layer (EF Core, Migrations, Models)
└── ByMyPC.IntegrationTesting/ # Integration tests for hardware endpoints & CRUD


## 📊 Metrics & Kafka Pipeline

ByMyPC collects hardware metrics and exposes them at the **`/metrics`** endpoint in Prometheus format. You can point Prometheus to scrape this endpoint and visualize data in Grafana. Metrics and events are also published to Kafka, enabling:

- **Monitoring:** Feed metrics into dashboards (Grafana, Kibana, etc.).
- **Analytics:** Store and analyze historical hardware data.
- **Alerting:** Trigger alerts based on metric thresholds.

## ⚡ Caching with Redis

Redis is used to cache frequently requested data (e.g., component lists, search results) to reduce load on PostgreSQL and improve response times. Ensure Redis is running and configured before starting the application.

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download)
- [Apache Kafka](https://kafka.apache.org/downloads)

  
### Database Setup (from backup)

The repository includes a database backup file (`backup.sql`). You do **not** need to run migrations manually — simply restore the backup into your PostgreSQL instance, use Release backup [Releases](https://github.com/DrCharlatan00/ByMyPC/releases).

1. Create an empty database:
   ```bash
   createdb -U postgres bymypc
2. Restore the backup:
   ```bash
   psql -U postgres -d bymypc -f backup.sql


You can run ByMyPC in two ways: using the pre-built binaries from the **Releases** page, or by building the project from source.

### Option 1: Download  Release (Recommended)

The easiest way to run ByMyPC is to download the ready-to-use archive for your operating system from the [Releases](https://github.com/DrCharlatan00/ByMyPC/releases) page.

1. Go to the [Releases](https://github.com/DrCharlatan00/ByMyPC/releases) section and download the archive for your OS:
   - `win64.zip` (Windows 64-bit)
   - `win32.zip` (Windows 32-bit)
   - `linux64.zip` (Linux 64-bit)
2. Extract the archive to a folder of your choice.
3. Download the database backup (`bymypcbackup.29102026`) from the same release page and restore it to your PostgreSQL instance (see **Database Setup** above).
4. Configure the database connection in the `.env` file (see **Configuration** above) for db and configure file appsettings.json for db redis and kafka connections.
5. Run the executable file `ByMyPC.exe` (on Windows) or `./ByMyPC` (on Linux).

### Option 2: Build from Source (For Developers)

If you want to build the project yourself, ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.

1. Clone the repository:
   ```bash
   git clone https://github.com/DrCharlatan00/ByMyPC.git
   cd ByMyPC
2. Build App
   Build and run the application:
    ```bash
   dotnet run --project ByMyPC
