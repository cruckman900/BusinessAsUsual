console.log("[SmartCommit] LegacyDataGrid.js loaded");

globalThis.initLegacyDataGrid = function (gridId, data, rowActionsFnName, columnDisplayNames = {}) {
    const container = document.getElementById(gridId);
    const table = container.querySelector("table");
    const thead = table.querySelector("thead");
    const tbody = table.querySelector("tbody");
    const pageSizeSelect = container.querySelector(".grid-page-size");
    const pageInfo = container.querySelector(".grid-page-info");
    const prevBtn = container.querySelector(".grid-prev");
    const nextBtn = container.querySelector(".grid-next");

    let currentPage = 1;
    let pageSize = parseInt(pageSizeSelect.value);
    let sortColumn = null;
    let sortDirection = "asc";

    console.log("header names", columnDisplayNames);
    function renderHeaders() {
        const keys = Object.keys(data[0] || {});
        thead.innerHTML = "<tr>" + keys.map(key => `
            <th data-key="${key}" class="sortable">${columnDisplayNames[key.toLowerCase()] || key}</th>
         `).join("") + "<th>Actions</th></tr>";

        thead.querySelectorAll("th.sortable").forEach(th => {
            th.addEventListener("click", () => {
                const key = th.dataset.key;
                sortDirection = (sortColumn === key && sortDirection === "asc") ? "desc" : "asc";
                sortColumn = key;
                renderRows();
            });
        });
    }

    function renderRows() {
        let sorted = [...data];
        if (sortColumn) {
            sorted.sort((a, b) => {
                const valA = a[sortColumn];
                const valB = b[sortColumn];
                return sortDirection === "asc"
                    ? (valA > valB ? 1 : -1)
                    : (valA < valB ? 1 : -1);
            });
        }

        const start = (currentPage - 1) * pageSize;
        const paged = sorted.slice(start, start + pageSize);

        tbody.innerHTML = paged.map(row => {
            const actionsHtml = typeof globalThis[rowActionsFnName] === "function"
                ? globalThis[rowActionsFnName](row)
                : "";

            return `
                <tr>
                    ${Object.values(row).map(val => `<td>${val}</td>`).join("")}
                    <td>${actionsHtml}</td>
                </tr>
            `;
        }).join("");

        pageInfo.textContent = `Page ${currentPage} of ${Math.ceil(sorted.length / pageSize)}`;
        prevBtn.disabled = currentPage === 1;
        nextBtn.disabled = currentPage >= Math.ceil(sorted.length / pageSize);
    }

    pageSizeSelect.addEventListener("change", () => {
        pageSize = parseInt(pageSizeSelect.value);
        currentPage = 1;
        renderRows();
    });

    prevBtn.addEventListener("click", () => {
        if (currentPage > 1) {
            currentPage--;
            renderRows();
        }
    });

    nextBtn.addEventListener("click", () => {
        currentPage++;
        renderRows();
    });

    renderHeaders();
    renderRows();
};