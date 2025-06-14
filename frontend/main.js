document.getElementById("loadBtn").addEventListener("click", loadData);

let chart;

async function loadData() {
  try {
    const response = await fetch("http://localhost:8080/api/WeatherData");

    if (!response.ok) {
      throw new Error(`Status: ${response.status}`);
    }

    const data = await response.json();
    console.log("Primljeni podaci:", data);

    const filtered = filterByTimeRange(data);
    updateTable(filtered);
    updateChart(filtered);
  } catch (err) {
    console.error("Greška:", err);
    alert("Ne mogu dohvatiti podatke s API-ja.");
  }
}

function filterByTimeRange(data) {
  const timeRange = document.getElementById("timeRange").value;
  const now = new Date();
  let rangeStart = null;

  switch (timeRange) {
    case "5min":
      rangeStart = new Date(now - 5 * 60 * 1000);
      break;
    case "1h":
      rangeStart = new Date(now - 60 * 60 * 1000);
      break;
    case "24h":
      rangeStart = new Date(now - 24 * 60 * 60 * 1000);
      break;
    case "1mj":
      rangeStart = new Date();
      rangeStart.setMonth(now.getMonth() - 1);
      break;
    case "1g":
      rangeStart = new Date();
      rangeStart.setFullYear(now.getFullYear() - 1);
      break;
    case "all":
    default:
      return data;
  }

  return data.filter((d) => new Date(d.timestamp) >= rangeStart);
}

function updateTable(data) {
  const tbody = document.querySelector("#dataTable tbody");
  tbody.innerHTML = "";

  data.forEach((entry) => {
    const row = document.createElement("tr");
    row.innerHTML = `
            <td>${formatTimestamp(entry.timestamp)}</td>
            <td>${formatValue(entry.compensatedTemperature)}°C</td>
            <td>${formatValue(entry.compensatedHumidity)}%</td>
            <td>${formatValue(entry.breathVocEquivalent)}</td>
            <td>${formatValue(entry.iaq)}</td>
            <td>${formatValue(entry.pressure)} hPa</td>
        `;
    tbody.appendChild(row);
  });
}

function updateChart(data) {
  const ctx = document.getElementById("sensorChart").getContext("2d");

  const labels = data.map((d) => formatTime(new Date(d.timestamp)));
  const temp = data.map((d) => d.compensatedTemperature);
  const humidity = data.map((d) => d.compensatedHumidity);
  const pressure = data.map((d) => d.pressure);

  if (chart) chart.destroy();

  chart = new Chart(ctx, {
    type: "line",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Temperature (°C)",
          data: temp,
          borderWidth: 2,
          fill: false,
          tension: 0.3,
        },
        {
          label: "Humidity (%)",
          data: humidity,
          borderWidth: 2,
          fill: false,
          tension: 0.3,
        },
        {
          label: "Pressure (hPa)",
          data: pressure,
          borderWidth: 2,
          fill: false,
          tension: 0.3,
        },
      ],
    },
    options: {
      responsive: true,
      scales: {
        x: {
          title: { display: true, text: "Time" },
        },
        y: {
          title: { display: true, text: "Value" },
        },
      },
    },
  });
}

function formatValue(val) {
  return val?.toFixed ? val.toFixed(2) : "-";
}

function formatTime(date) {
  return `${date.getHours()}:${String(date.getMinutes()).padStart(2, "0")}`;
}

function formatTimestamp(ts) {
  const date = new Date(ts);
  return date.toLocaleString();
}
