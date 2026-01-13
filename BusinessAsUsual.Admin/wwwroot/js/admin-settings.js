async function loadSettings() {
    const res = await fetch("/Admin/SettingsApi");
    if (!res.ok) return;
    const s = await res.json();

    document.getElementById("metricsPollingInterval").value = s.metricsPollingIntervalSeconds;
    document.getElementById("autoRefreshMetrics").checked = s.autoRefreshMetrics;
    document.getElementById("autoRefreshLogs").checked = s.autoRefreshLogs;

    document.getElementById("enableCpu").checked = s.enableCpu;
    document.getElementById("enableMemory").checked = s.enableMemory;
    document.getElementById("enableDisk").checked = s.enableDisk;
    document.getElementById("enableNetwork").checked = s.enableNetwork;
    document.getElementById("chartHistoryLength").value = s.chartHistoryLength;

    document.getElementById("logRetentionDays").value = s.logRetentionDays;
    document.getElementById("defaultLogLevel").value = s.defaultLogLevel;
    document.getElementById("enableCloudWatch").checked = s.enableCloudWatch;
    document.getElementById("cloudWatchLogGroup").value = s.cloudWatchLogGroup;

    document.getElementById("themeMode").value = s.themeMode;
    document.getElementById("rememberSidebarState").checked = s.rememberSidebarState;
    document.getElementById("enableDebugMode").checked = s.enableDebugMode;
    document.getElementById("showDangerTools").checked = s.showDangerTools;
}

async function saveSettings() {
    const payload = {
        metricsPollingIntervalSeconds: parseInt(document.getElementById("metricsPollingInterval").value || "5"),
        autoRefreshMetrics: document.getElementById("autoRefreshMetrics").checked,
        autoRefreshLogs: document.getElementById("autoRefreshLogs").checked,

        enableCpu: document.getElementById("enableCpu").checked,
        enableMemory: document.getElementById("enableMemory").checked,
        enableDisk: document.getElementById("enableDisk").checked,
        enableNetwork: document.getElementById("enableNetwork").checked,
        chartHistoryLength: parseInt(document.getElementById("chartHistoryLength").value || "20"),

        logRetentionDays: parseInt(document.getElementById("logRetentionDays").value || "7"),
        defaultLogLevel: document.getElementById("defaultLogLevel").value,
        enableCloudWatch: document.getElementById("enableCloudWatch").checked,
        cloudWatchLogGroup: document.getElementById("cloudWatchLogGroup").value,

        themeMode: document.getElementById("themeMode").value,
        rememberSidebarState: document.getElementById("rememberSidebarState").checked,
        enableDebugMode: document.getElementById("enableDebugMode").checked,
        showDangerTools: document.getElementById("showDangerTools").checked
    };

    await fetch("/Admin/SettingsApi", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    const toastEl = document.getElementById("settingsToast");
    if (toastEl && bootstrap?.Toast) {
        new bootstrap.Toast(toastEl).show();
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadSettings();

    document.getElementById("saveSettingsBtn")
        .addEventListener("click", saveSettings);

    document.getElementById("resetSettingsBtn")
        .addEventListener("click", () => loadSettings());
});