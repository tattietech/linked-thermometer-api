# Linked Thermometer API

A .NET 8.0 ASP.NET Core Web API for collecting and managing temperature and humidity readings from IoT thermometer devices.

## Overview

This API provides endpoints for IoT devices to submit temperature and humidity readings and for clients to retrieve the latest readings. All data is stored in Azure Cosmos DB and secured with API key authentication.

## Features

- Collect temperature and humidity readings from multiple devices
- Store historical readings for analytics
- Query latest readings per device
- API key authentication
- Swagger/OpenAPI documentation
- Integration with Azure Cosmos DB

## Prerequisites

- .NET 8.0 SDK or later
- Azure Cosmos DB account
- Visual Studio 2022 or Visual Studio Code (optional)

## Configuration

The application requires the following configuration settings:

### appsettings.json

Add the following configuration to your `appsettings.json` file:

```json
{
  "Cosmos": {
    "Endpoint": "your-cosmos-db-endpoint",
    "Database": "your-database-name",
    "Container": "your-container-name"
  },
  "ApiKey": "your-api-key",
  "DeviceId1": "Device 1 Name",
  "DeviceId2": "Device 2 Name"
}
```

Configuration details:
- `Cosmos:Endpoint`: Your Azure Cosmos DB endpoint URL
- `Cosmos:Database`: The name of your Cosmos DB database
- `Cosmos:Container`: The name of your Cosmos DB container
- `ApiKey`: Authentication key for API requests
- Device mappings: Map device IDs to friendly names (e.g., "DeviceId1": "Living Room")

## Installation

1. Clone the repository:
```bash
git clone https://github.com/tattietech/linked-thermometer-api.git
cd linked-thermometer-api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Configure your settings in `appsettings.json`

4. Build the project:
```bash
dotnet build
```

## Running the Application

### Development Mode

Run with HTTP:
```bash
dotnet run --project linked-thermometer-api
```

The API will be available at `http://localhost:5246`

Run with HTTPS:
```bash
dotnet run --project linked-thermometer-api --launch-profile https
```

The API will be available at `https://localhost:7208`

### Swagger UI

When running in development mode, access the Swagger UI at:
- HTTP: `http://localhost:5246/swagger`
- HTTPS: `https://localhost:7208/swagger`

## API Endpoints

### POST /Readings

Submit a new temperature and humidity reading from a device.

**Request Headers:**
- `x-api-key`: Your API key

**Request Body:**
```json
{
  "deviceId": "device-123",
  "deviceName": "Living Room",
  "temperature": 22.5,
  "humidity": 45.0,
  "timeStamp": "2024-11-06T12:00:00Z"
}
```

**Response:**
- `200 OK`: Reading stored successfully
- `401 Unauthorized`: Invalid or missing API key

### GET /Readings

Retrieve the latest readings from all devices.

**Request Headers:**
- `x-api-key`: Your API key

**Response:**
```json
[
  {
    "id": "device-123",
    "deviceId": "device-123",
    "deviceName": "Living Room",
    "temperature": 22.5,
    "humidity": 45.0,
    "timeStamp": "2024-11-06T12:00:00Z",
    "partitionKey": "latest"
  }
]
```

**Response Codes:**
- `200 OK`: Successfully retrieved readings
- `401 Unauthorized`: Invalid or missing API key

## Data Storage

The API uses a dual-storage approach in Cosmos DB:

1. **Historical Data**: Each reading is stored with a unique ID and partitioned by device ID for historical tracking and analytics.

2. **Latest Readings**: The most recent reading for each device is stored in a "latest" partition with the device ID as the document ID, enabling efficient queries for current device states.

## Authentication

All API endpoints require authentication using an API key. Include the key in the `x-api-key` header with each request:

```bash
curl -H "x-api-key: your-api-key" https://localhost:7208/readings
```

## Project Structure

```
linked-thermometer-api/
├── Controllers/
│   └── ReadingsController.cs    # API endpoints
├── Interfaces/
│   └── IReadingService.cs       # Service interface
├── Models/
│   └── Reading.cs                # Data model
├── Services/
│   └── ReadingService.cs        # Business logic
├── Program.cs                    # Application entry point
└── appsettings.json             # Configuration
```

## Dependencies

- **Microsoft.Azure.Cosmos** (3.54.0): Azure Cosmos DB client
- **Azure.Identity** (1.17.0): Azure authentication
- **Newtonsoft.Json** (13.0.4): JSON serialization
- **Swashbuckle.AspNetCore** (6.6.2): Swagger/OpenAPI documentation

## Building for Production

To build for production:

```bash
dotnet publish -c Release -o ./publish
```

The compiled application will be in the `./publish` directory.

## License

This project is available for use and modification as needed.
