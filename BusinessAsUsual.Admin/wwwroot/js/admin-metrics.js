async function pollMetrics() {
    const res = await fetch("/admin/metrics/system");
    const data = await res.json();

    updateCpu(data.cpu.percent);
    updateMemory(data.memory);
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
    value.textContent = `${used.toFixed(1)} / ${total.toFixed(i)} GB`;
}

setInterval(pollMetrics, 5000);
pollMetrics();