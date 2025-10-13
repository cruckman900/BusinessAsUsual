// wwwroot/js/menuSystem.js

console.log("[SmartCommit] menuSystem.js loaded");

globalThis.menuState = {
    data: null,
    activeCategory: null, // ✅ new
    sidebarWasManuallyCollapsed: false,
    sidebarIsExpanded: false,
    sidebarWasTriggeredByNavbar: false,
};

globalThis.menuData = globalThis.menuData || {};

globalThis.loadMenuJson = async function () {
    if (!globalThis.menuData || Object.keys(globalThis.menuData).length === 0) {
        try {
            const response = await fetch('/config/menu.json');
            if (!response.ok) throw new Error(`HTTP ${response.status}`);
            const json = await response.json();
            globalThis.menuData = json;
            globalThis.menuState.data = json; // ✅ assign here
            console.log("[SmartCommit] menuData loaded from /config/menu.json");
        } catch (err) {
            console.error("[SmartCommit] Failed to load menu.json", err);
            globalThis.menuData = {};
            globalThis.menuState.data = {}; // ✅ fallback
        }
    } else {
        globalThis.menuState.data = globalThis.menuData; // ✅ assign if already loaded
    }
};

globalThis.resolveSidebarSection = function () {
    const path = globalThis.location.pathname.toLowerCase();

    if (path.includes("/admin/company")) return "Companies";
    if (path.includes("/admin/provision")) return "Provisioning";
    if (path.includes("/admin/user")) return "Users";
    if (path.includes("/admin/settings")) return "Settings";

    return "Home";
};

globalThis.renderNavbar = async function () {
    await globalThis.loadMenuJson();

    const nav = document.getElementById("nav-links");
    if (!nav) return;

    if (!globalThis.menuState.data || typeof globalThis.menuState.data !== "object") {
        console.warn("[SmartCommit] menuState.data is missing or invalid");
        return;
    }

    nav.innerHTML = "";
    const activeCategory = globalThis.menuState.activeCategory;

    Object.entries(globalThis.menuState.data).forEach(([category, config]) => {
        const icon = config.icon || "fas fa-folder";
        const route = config.route || "#";
        const id = config.id || `nav-${category.toLowerCase()}`;
        const isActive = category === activeCategory; // ✅ use category match

        const li = document.createElement("li");
        li.className = isActive ? "active" : "";

        const link = document.createElement("a");
        link.href = route;
        link.id = id;
        link.setAttribute("aria-label", category);
        link.innerHTML = `<i class="${icon}" aria-hidden="true"></i> ${category}`;
        link.classList.add("nav-item");
        if (isActive) link.classList.add("nav-item-active");

        link.addEventListener("click", function (e) {
            if (globalThis.location.pathname === route) {
                e.preventDefault();
                console.log(`[SmartCommit] Already on ${id}`);
                globalThis.menuState.sidebarWasTriggeredByNavbar = true;
                globalThis.renderSidebar(category);
                globalThis.renderNavbar(); // ✅ re-render navbar to reflect active state
            } else {
                console.log(`[SmartCommit] Redirecting to ${route} via ${id}`);
                globalThis.menuState.sidebarWasTriggeredByNavbar = true;
                globalThis.menuState.activeCategory = category; // ✅ set before reload
                globalThis.renderSidebar(category);
                globalThis.renderNavbar(); // ✅ re-render before navigation
                globalThis.location.href = route;
            }
        });

        li.appendChild(link);
        nav.appendChild(li);
    });
};

globalThis.renderSidebar = function (category) {
    const sidebar = document.getElementById("sidebar");
    const sidebarTitle = document.getElementById("sidebar-title");
    const sidebarLinks = document.getElementById("sidebar-links");

    if (!sidebar || !sidebarTitle || !sidebarLinks) return;

    const isHome = category === "Home" || category === "/";

    if (isHome) {
        sidebar.classList.add("collapsed");
        sidebar.classList.remove("expanded");
        sidebarTitle.textContent = "";
        sidebarLinks.innerHTML = "";
        globalThis.menuState.sidebarWasManuallyCollapsed = false;
        globalThis.menuState.sidebarWasTriggeredByNavbar = false;
        globalThis.menuState.sidebarIsExpanded = false;
        return;
    }

    if (globalThis.menuState.sidebarWasTriggeredByNavbar && !globalThis.menuState.sidebarWasManuallyCollapsed) {
        sidebar.classList.remove("collapsed");
        sidebar.classList.add("expanded");
        globalThis.menuState.sidebarIsExpanded = true;
    }

    if (globalThis.menuState.sidebarIsExpanded) {
        sidebar.classList.remove("collapsed");
        sidebar.classList.add("expanded");
    }

    sidebarTitle.textContent = category;
    sidebarLinks.innerHTML = "";
    globalThis.menuState.activeCategory = category; // ✅ track active category

    const items = globalThis.menuState.data?.[category]?.items || [];
    items.forEach(item => {
        const isActive = globalThis.location.pathname === item.route;
        sidebarLinks.innerHTML += `
          <li class="list-group-item ${isActive ? 'active' : ''}">
            <a id="${item.id}" href="${item.route}" title="${item.description}" aria-label="${item.description}" class="${isActive ? 'sidebar-item-active' : ''}">
              <i class="${item.icon}" aria-hidden="true"></i>
              <span>${item.label}</span>
            </a>
          </li>`;
    });
    globalThis.menuState.sidebarWasTriggeredByNavbar = false;
};

globalThis.initializeSidebar = async function () {
    await globalThis.loadMenuJson();
    const section = globalThis.resolveSidebarSection();
    console.log(`[SmartCommit] Initializing sidebar for: ${section}`);
    console.log("[SmartCommit] Sidebar items:", globalThis.menuState.data?.[section]?.items);
    globalThis.renderSidebar(section);
};