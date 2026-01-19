let metricsInterval = null;

function applySettingsToMetrics(settings) {
    if (metricsInterval) clearInterval(metricsInterval);

    if (settings.autoRefreshMetrics) {
        metricsInterval = setInterval(pollMetrics, settings.metricsPollingIntervalSeconds * 1000);
    }
}

window.addEventListener("settingsLoaded", (e) => {
    window.currentSettings = e.detail;
    applySettingsToMetrics(e.detail);
}

// ===============================
//  GLOBAL CHART REFERENCES
// ===============================
let cpuChart, memoryChart, diskChart, networkChart, uptimeChart;

// ===============================
//  INITIALIZE ALL CHARTS
// ===============================
function initCharts() {

    // CPU Line Chart
    const cpuCtx = document.getElementById("cpuChart");
    cpuChart = new Chart(cpuCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'CPU %',
                data: [],
                borderColor: '#38bdf8',
                backgroundColor: 'rgba(56, 189, 248, 0.2)',
                tension: 0.3
            }]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: false,
            scales: { y: { min: 0, max: 100 } }
        }
    });

    // Memory Doughnut
    const memCtx = document.getElementById("memoryChart");
    memoryChart = new Chart(memCtx, {
        type: 'doughnut',
        data: {
            labels: ['Used', 'Free'],
            datasets: [{
                data: [0, 1],
                backgroundColor: ['#f97316', '#22c55e']
            }]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: false,
            cutout: '60%'
        }
    });

    // Disk Doughnut
    const diskCtx = document.getElementById("diskChart");
    diskChart = new Chart(diskCtx, {
        type: 'doughnut',
        data: {
            labels: ['Used', 'Free'],
            datasets: [{
                data: [0, 1],
                backgroundColor: ['#3b82f6', '#1e293b']
            }]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: false,
            cutout: '60%'
        }
    });

    // Network Line Chart
    const netCtx = document.getElementById("networkChart");
    networkChart = new Chart(netCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [
                {
                    label: 'Inbound (bytes/s)',
                    data: [],
                    borderColor: '#f97316',
                    backgroundColor: 'rgba(249, 115, 22, 0.2)',
                    tension: 0.3
                },
                {
                    label: 'Outbound (bytes/s)',
                    data: [],
                    borderColor: '#22c55e',
                    backgroundColor: 'rgba(34, 197, 94, 0.2)',
                    tension: 0.3
                }
            ]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: false,
            scales: { y: { beginAtZero: true } }
        }
    });

    // Uptime Doughnut
    const upCtx = document.getElementById("uptimeChart");
    uptimeChart = new Chart(upCtx, {
        type: 'doughnut',
        data: {
            labels: ['Uptime', ''],
            datasets: [{
                data: [1, 0],
                backgroundColor: ['#a855f7', '#1e293b']
            }]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: false,
            cutout: '75%',
            plugins: { tooltip: { enabled: false } }
        }
    });
}

// ===============================
//  UPDATE FUNCTIONS
// ===============================
function updateCpu(percent) {
    const now = new Date().toLocaleTimeString();

    cpuChart.data.labels.push(now);
    cpuChart.data.datasets[0].data.push(percent);

    const max = window.currentSettings?.chartHistoryLength || 20;

    if (cpuChart.data.labels.length > max) {
        cpuChart.data.labels.shift();
        cpuChart.data.datasets[0].data.shift();
    }

    cpuChart.update();
}

function updateMemory(memory) {
    const used = memory.used;
    const total = memory.total;
    const free = total - used;

    memoryChart.data.datasets[0].data = [used, free];
    memoryChart.update();
}

function updateDisk(disk) {
    const used = disk.used;
    const total = disk.total;
    const free = total - used;

    diskChart.data.datasets[0].data = [used, free];
    diskChart.update();
}

function updateNetwork(network) {
    const now = new Date().toLocaleTimeString();

    networkChart.data.labels.push(now);
    networkChart.data.datasets[0].data.push(network.bytesIn);
    networkChart.data.datasets[1].data.push(network.bytesOut);

    const max = window.currentSettings?.chartHistoryLength || 20;

    if (networkChart.data.labels.length > max) {
        networkChart.data.labels.shift();
        networkChart.data.datasets[0].data.shift();
        networkChart.data.datasets[1].data.shift();
    }

    networkChart.update();
}

function updateUptime(uptime) {
    const label = document.getElementById("uptimeLabel");
    label.textContent = uptime.humanReadable;
}

// ===============================
//  POLLING LOOP
// ===============================
async function pollMetrics() {
    const res = await fetch("/Admin/MetricsApi/system");
    const data = await res.json();

    updateCpu(data.cpu.percent);
    updateMemory(data.memory);
    updateDisk(data.disk);
    updateNetwork(data.network);
    updateUptime(data.uptime);
}

// ===============================
//  BOOTSTRAP
// ===============================

document.addEventListener("DOMContentLoaded", () => {
    initCharts();
    pollMetrics();
    setInterval(pollMetrics, 5000);
});