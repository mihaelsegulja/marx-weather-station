# Marx Weather Station

**MARX: Weather Station** is an IoT project for monitoring air quality using an ESP32 and BME680 sensor. It collects sensor data, sends it via MQTT to a backend service, stores it in a SQL Server database, and allows data visualization for a given time period.

## Project structure

```
MARX-Weather-station/
│── backend/             # ASP.NET Web API (C#)
│── database/            # SQL script (MSSQL)
│── frontend/            # HTML, CSS, JS, Chart.js, jQuery...
│── firmware/            # ESP32 code (C++)
│── docker-compose.yml   # RabbitMQ container
│── README.md            # Project description
│── ...
```

## Prerequisites

- Docker and Docker Compose
- .NET SDK 8.0 or later
- SQL Server instance
- Visual Studio or any other IDE for editing/running backend
- Visual Studio Code (optional, for editing/running frontend)
	- Live Server VS Code extension (for serving the frontend locally)
- Arduino IDE or PlatformIO (for uploading firmware to ESP32)

## Setup

### 1. Clone this repo

```shell
git clone https://github.com/LovricA10/MARX-Weather-station.git
cd MARX-Weather-station
```

### 2. Start and configure RabbitMQ 

**2.1.** Make sure you're in the same directory as the `docker-compose.yml` file, then run:

```shell
docker compose up -d
```

- Management UI: [http://localhost:15672](http://localhost:15672)
- Default credentials:
    - Username: `admin`
    - Password: `admin`
- MQTT port: `1883`
- AMQP port: `5672`

**2.2.** From the management UI, go to *Queues and streams* tab, and from there you need to manually add a queue named `weather/data` (set Durability: Durable, leave everything else default). 
Then go to *Exchanges* tab, click `amq.topic` and add a binding from this exchange (To queue: `weather/data`, Routing key: `weather.data`, leave everything else default).

### 3. ESP32 setup

Go to `firmware/src/main.cpp` and change the following parameters:

```cpp
// WiFi credentials
const char* ssid = "some ssid";
const char* password = "some pass";

// RabbitMQ (MQTT) config
const char* mqtt_server = "some ip";
// ...
```

Replace these values with your actual access point credentials and server IP address your ESP32 will connect to.

> **Tip:** For local testing, it's recommended to create a WiFi hotspot on your computer and connect the ESP32 to that network.

Flash the firmware to your ESP32 using the Arduino IDE or PlatformIO.

### 4. SQL Server setup

Open SQL Server Management Studio or your preferred SQL tool and execute the SQL script from the `database/` directory.

### 5. Configure and run the Backend

**5.1.** Go to `backend/WebApp/appsettings.json` and modify the database connection string if neccessary:

```json
"ConnectionStrings": {
    "Default": "Server=.;User Id=sa;Password=SQL;Database=dbMarxWeatherStation;TrustServerCertificate=True;trusted_connection=true;MultipleActiveResultSets=True"
}
```

> **Note:** Replace `sa` and `SQL` with your actual SQL Server credentials.

**5.2.** Run the backend from your shell (or via your desired IDE):

```shell
cd backend/WebApp
dotnet run
```

The backend should now be running at [http://localhost:8080](http://localhost:8080)

### 6. Run the Frontend

> **Important:** The frontend must be served from `http://localhost` (not opened as a file) due to browser security restrictions (CORS).

#### Recommended: Use Live Server + VS Code

1. Open the `frontend/` folder in Visual Studio Code
2. Install the Live Server extension (if not already installed)
3. Right-click on `index.html` -> **"Open with Live Server"**

This will serve the page at something like [http://127.0.0.1:5500](http://127.0.0.1:5500), allowing it to access the backend safely.

> **Additional notes:** By default, the backend whitelists `http://127.0.0.1:5500` and `http://localhost:5500`, but that can be easily changed if needed from the `backend/WebApp/appsettings.json`, under the section `CorsOriginsWhitelist`.
