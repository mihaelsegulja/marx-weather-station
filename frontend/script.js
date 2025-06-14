const chartCtx = document.getElementById("sensorChart").getContext("2d");
let sensorChart;
let currentPage = 1;
let pageSize = parseInt(document.getElementById("pageSizeSelect").value);

let currentLang = document.getElementById("languageSelector").value;

const savedLang = localStorage.getItem("lang");
const savedPageSize = localStorage.getItem("pageSize");
const savedTimeRange = localStorage.getItem("timeRange");

if (savedLang) {
  document.getElementById("languageSelector").value = savedLang;
  currentLang = savedLang;
}

if (savedPageSize) {
  document.getElementById("pageSizeSelect").value = savedPageSize;
  pageSize = parseInt(savedPageSize);
}

if (savedTimeRange) {
  document.getElementById("timeRange").value = savedTimeRange;
}

document.getElementById("languageSelector").addEventListener("change", (e) => {
  currentLang = e.target.value;
  localStorage.setItem("lang", currentLang);
  loadChart();
  loadTable();
  localize();
});

const localizationLabels = {
  en: {
    title: "MARX Weather Station",
    iaq: "IAQ",
    staticIaq: "Static IAQ",
    co2: "CO₂ Equivalent",
    voc: "Breath VOC Equivalent",
    temp: "Temperature (°C)",
    pressure: "Pressure (Pa)",
    humidity: "Humidity (%)",
    gas: "Gas Resistance (Ohm)",
    next: "Next",
    prev: "Previous",
    rowsPerPage: "Rows per page:",
    timestamp: "Timestamp",
    timeRange: "Date/Time range:",
    load: "Load",
    min5: "Last 5 minutes",
    h1: "Last 1 hour",
    h24: "Last 24 hours",
    mo1: "Last 1 month",
    y1: "Last 1 year",
    all: "All time",
    page: "Page",
    of: "of",
  },
  hr: {
    title: "MARX Meteorološka Stanica",
    iaq: "IAQ",
    staticIaq: "Statički IAQ",
    co2: "Ekvivalent CO₂",
    voc: "VOC udisanja",
    temp: "Temperatura (°C)",
    pressure: "Tlak (Pa)",
    humidity: "Vlaga (%)",
    gas: "Otpornost plina (Ohm)",
    next: "Slijedeća",
    prev: "Prethodna",
    rowsPerPage: "Redaka po stranici:",
    timestamp: "Vremenska oznaka",
    timeRange: "Raspon datuma/vremena:",
    load: "Učitaj",
    min5: "Zadnjih 5 minuta",
    h1: "Zadnji sat",
    h24: "Zadnja 24 sata",
    mo1: "Zadnji mjesec",
    y1: "Zadnja godina",
    all: "Sve",
    page: "Stranica",
    of: "od",
  },
};

function localize() {
  document.querySelectorAll("[data-i18n]").forEach((el) => {
    const key = el.getAttribute("data-i18n");
    el.textContent = localizationLabels[currentLang][key];
  });
}

document.getElementById("pageSizeSelect").addEventListener("change", () => {
  pageSize = parseInt(document.getElementById("pageSizeSelect").value);
  currentPage = 1;
  localStorage.setItem("pageSize", pageSize);
  loadTable();
});

document.getElementById("loadBtn").addEventListener("click", () => {
  const timeRange = getSelectedTimeRange();
  localStorage.setItem("timeRange", timeRange);
  currentPage = 1;
  loadChart();
  loadTable();
});

document.getElementById("prevPage").addEventListener("click", () => {
  if (currentPage > 1) {
    currentPage--;
    loadTable();
  }
});

document.getElementById("nextPage").addEventListener("click", () => {
  currentPage++;
  loadTable();
});

function getSelectedTimeRange() {
  return document.getElementById("timeRange").value;
}

async function loadChart() {
  const timeRange = getSelectedTimeRange();
  const response = await fetch(
    `http://localhost:8080/api/WeatherData/chart?timeRange=${timeRange}`
  );
  const result = await response.json();

  const data = result.data.map((d) => ({
    timestamp: new Date(d.timestamp),
    iaq: d.iaq,
    staticIaq: d.staticIaq,
    co2equivalent: d.co2equivalent,
    breathVocEquivalent: d.breathVocEquivalent,
    compensatedTemperature: d.compensatedTemperature,
    pressure: d.pressure,
    compensatedHumidity: d.compensatedHumidity,
    gasResistance: d.gasResistance,
  }));

  const labels = data.map((d) => d.timestamp.toLocaleString());

  if (sensorChart) sensorChart.destroy();

  sensorChart = new Chart(chartCtx, {
    type: "line",
    data: {
      labels,
      datasets: [
        {
          label: localizationLabels[currentLang].iaq,
          data: data.map((d) => ({ x: d.timestamp, y: d.iaq })),
          borderWidth: 2,
          hidden: true,
        },
        {
          label: localizationLabels[currentLang].staticIaq,
          data: data.map((d) => ({ x: d.timestamp, y: d.staticIaq })),
          borderWidth: 2,
        },
        {
          label: localizationLabels[currentLang].co2,
          data: data.map((d) => ({ x: d.timestamp, y: d.co2equivalent })),
          borderWidth: 2,
          hidden: true,
        },
        {
          label: localizationLabels[currentLang].voc,
          data: data.map((d) => ({ x: d.timestamp, y: d.breathVocEquivalent })),
          borderWidth: 2,
        },
        {
          label: localizationLabels[currentLang].temp,
          data: data.map((d) => ({ x: d.timestamp, y: d.compensatedTemperature })),
          borderWidth: 2,
        },
        {
          label: localizationLabels[currentLang].pressure,
          data: data.map((d) => ({ x: d.timestamp, y: d.pressure })),
          borderWidth: 2,
          hidden: true,
        },
        {
          label: localizationLabels[currentLang].humidity,
          data: data.map((d) => ({ x: d.timestamp, y: d.compensatedHumidity })),
          borderWidth: 2,
        },
        {
          label: localizationLabels[currentLang].gas,
          data: data.map((d) => ({ x: d.timestamp, y: d.gasResistance })),
          borderWidth: 2,
          hidden: true,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: true,
          position: "top",
        },
      },
      scales: {
        x: {
          ticks: { maxRotation: 45, minRotation: 45 },
        },
      },
    },
  });
}

const formatDate = (date) =>
  new Intl.DateTimeFormat(currentLang === "hr" ? "hr-HR" : "en-US", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);

async function loadTable() {
  const timeRange = getSelectedTimeRange();
  const response = await fetch(
    `http://localhost:8080/api/WeatherData/table?timeRange=${timeRange}&page=${currentPage}&pageSize=${pageSize}`
  );
  const result = await response.json();

  const tbody = document.querySelector("#dataTable tbody");
  tbody.innerHTML = "";

  result.data.forEach((row) => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${formatDate(new Date(row.timestamp))}</td>
      <td>${row.compensatedTemperature ?? "-"}</td>
      <td>${row.compensatedHumidity ?? "-"}</td>
      <td>${row.breathVocEquivalent ?? "-"}</td>
      <td>${row.iaq ?? "-"}</td>
      <td>${row.pressure ?? "-"}</td>
    `;
    tbody.appendChild(tr);
  });

  document.getElementById(
    "pageInfo"
  ).innerText = `${localizationLabels[currentLang].page} ${result.currentPage} ${localizationLabels[currentLang].of} ${result.totalPages}`;
  document.getElementById("prevPage").disabled = currentPage <= 1;
  document.getElementById("nextPage").disabled =
    currentPage >= result.totalPages;
}

loadChart();
loadTable();
localize();
