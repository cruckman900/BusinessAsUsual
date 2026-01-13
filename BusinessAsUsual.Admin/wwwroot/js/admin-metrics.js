async function pollMetrics() {
    console.log("hello world");
    const res = await fetch("/Admin/MetricsApi/system");
    const data = await res.json();

    console.log("data", data);

    updateCpu(data.cpu.percent);
    updateMemory(data.memory);
    updateDisk(data.disk);
}

function updateCpu(percent) {
    const gauge = document.getElementById("cpuGauge");
    const value = document.getElementById("cpuValue");

    value.textContent = percent.toFixed(1) + "%";

    gauge.style.background = `conic-gradient(
        #4caf50 ${percent * 3.6}deg,
        #222 ${percent * 3.6}deg
    )`;
}

function updateMemory(memory) {
    const bar = document.getElementById("memoryBar");
    const value = document.getElementById("memoryValue");

    const used = memory.used;
    const total = memory.total;

    const percent = (used / total) * 100;

    bar.style.width = percent + "%";
    value.textContent = `${used.toFixed(1)} / ${total.toFixed(1)} GB`;
}

function updateDisk(disk) {
    const bar = document.getElementById("diskBar");
    const value = document.getElementById("diskValue");

    const used = disk.used;
    const total = disk.total;

    if (total === 0) {
        bar.style.width = "0%";
        value.textContent = "-- / -- GB";
        return;
    }

    const percent = (used / total) * 100;

    bar.style.width = percent + "%";
    value.textContent = `${used.toFixed(1)} / ${total.toFixed(1)} GB`;
}

setInterval(pollMetrics, 5000);
pollMetrics();