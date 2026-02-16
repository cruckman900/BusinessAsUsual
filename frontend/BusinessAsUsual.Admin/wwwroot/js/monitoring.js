setInterval(refreshMonitoring, 10000);

function refreshMonitoring() {
    fetch('/api/monitoring/platform-health')
        .then(r => r.json())
        .then(data => updateUI(data));
}

function updateUI(data) {

    $.ajax({
        url: "/Admin/Monitoring/PlatformStatus",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: html => $("#platform-status").html(html)
    });

    $.ajax({
        url: "/Admin/Monitoring/ServiceHealth",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: html => $("#service-health").html(html)
    });

    $.ajax({
        url: "/Admin/Monitoring/InfrastructureHealth",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: html => $("#infrastructure-health").html(html)
    });

    $("#last-updated-time").text(new Date().toLocaleTimeString());
}

// Drawer logic
$(document).on("click", ".open-detail", function () {
    let title = $(this).data("title");
    $("#drawer-title").text(title);
    $("#drawer-body").text("Loading details...");
    $("#alarm-detail-drawer").addClass("open");
});

$("#close-drawer").on("click", function () {
    $("#alarm-detail-drawer").removeClass("open");
});