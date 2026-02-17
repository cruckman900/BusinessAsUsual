setInterval(refreshMonitoring, 10000);

function refreshMonitoring() {
    fetch('/api/monitoring/platform-health')
        .then(r => r.json())
        .then(data => updateUI(data));
}

refreshMonitoring(); // Initial load

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
    alert("hello world");
    let title = $(this).data("title");

    $("#drawer-title").text(title);
    $("#drawer-body").html("Loading...");

    $.ajax({
        url: "/Admin/Monitoring/AlarmDetail",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(title),
        success: html => {
            $("#drawer-body").html(html);
            $("#alarm-detail-drawer").addClass("open");
        }
    });
});

$("#close-drawer").on("click", function () {
    $("#alarm-detail-drawer").removeClass("open");
});