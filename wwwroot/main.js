// ========== CHART ==========


let chart;

async function loadBillsChart() {

    try {

        const response = await fetch("/api/Banking/transactions", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (!response.ok) return;

        const transactions = await response.json();

        const bills = transactions.filter(t => t.type === "BillPayment");

        const months = [
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        ];

        const electricity = new Array(12).fill(0);
        const water = new Array(12).fill(0);
        const internet = new Array(12).fill(0);
        const gas = new Array(12).fill(0);
        const phone = new Array(12).fill(0);
        const tv = new Array(12).fill(0);

        bills.forEach(bill => {

            const month = new Date(bill.createdAt).getMonth();

            const description = (bill.description || "").toLowerCase();

            if (description.includes("electricity"))
                electricity[month] += bill.amount;

            else if (description.includes("water"))
                water[month] += bill.amount;

            else if (description.includes("internet"))
                internet[month] += bill.amount;

            else if (description.includes("gas"))
                gas[month] += bill.amount;

            else if (description.includes("phone"))
                phone[month] += bill.amount;

            else if (description.includes("tv"))
                tv[month] += bill.amount;

        });

        const ctx = document.getElementById("chart").getContext("2d");

        if (chart)
            chart.destroy();

        chart = new Chart(ctx, {

            type: "bar",

            data: {

                labels: months,

                datasets: [

                    {
                        label: "Electricity",
                        data: electricity,
                        backgroundColor: "#f39c12"
                    },

                    {
                        label: "Water",
                        data: water,
                        backgroundColor: "#3498db"
                    },

                    {
                        label: "Internet",
                        data: internet,
                        backgroundColor: "#2ecc71"
                    },

                    {
                        label: "Gas",
                        data: gas,
                        backgroundColor: "#e74c3c"
                    },

                    {
                        label: "Phone",
                        data: phone,
                        backgroundColor: "#9b59b6"
                    },

                    {
                        label: "TV",
                        data: tv,
                        backgroundColor: "#1abc9c"
                    }

                ]

            },

            options: {

                responsive: true,

                plugins: {

                    legend: {
                        position: "top"
                    },

                    title: {
                        display: true,
                        text: "Monthly Bills Payments"
                    }

                },

                scales: {

                    y: {
                        beginAtZero: true
                    }

                }

            }

        });

    }

    catch (err) {

        console.error("Bills Chart Error:", err);

    }

}

// ========== SMART SEARCH ==========
const searchInput = document.querySelector(".search-bar input");

const searchDropdown = document.createElement("div");
searchDropdown.className = "search-dropdown";
searchDropdown.style.cssText = `
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: white;
    border-radius: 10px;
    box-shadow: 0 4px 20px rgba(0,0,0,0.15);
    z-index: 1000;
    display: none;
    max-height: 300px;
    overflow-y: auto;
`;

const searchBarContainer = document.querySelector(".search-bar");
searchBarContainer.style.position = "relative";
searchBarContainer.appendChild(searchDropdown);

const searchItems = [
    { name: "Dashboard", icon: "dashboard", url: "index.html" },
    { name: "Transfer Money", icon: "send", url: "transfer.html" },
    { name: "Transfer History", icon: "history", url: "transfer_his.html" },
    { name: "Beneficiaries", icon: "group", url: "Beneficiaries.HTML" },
    { name: "Bills Payment", icon: "payment", url: "bills.html" },
    { name: "Admin Panel", icon: "admin_panel_settings", url: "adminmonitor.html" },
    { name: "Settings & Security", icon: "settings", url: "settings.html" },
    { name: "Support", icon: "help_center", url: "support.html" },
    { name: "Change Password", icon: "vpn_key", url: "change.html" },
    { name: "Logout", icon: "logout", url: "logout.html" }
];

if (searchInput) {
    searchInput.addEventListener("input", function (e) {
        const searchTerm = e.target.value.toLowerCase().trim();

        if (searchTerm === "") {
            searchDropdown.style.display = "none";
            return;
        }

        const filtered = searchItems.filter(item =>
            item.name.toLowerCase().includes(searchTerm)
        );

        if (filtered.length === 0) {
            searchDropdown.innerHTML = `<div style="padding: 12px; color: #888;">No results found</div>`;
            searchDropdown.style.display = "block";
            return;
        }

        searchDropdown.innerHTML = filtered.map(item => `
            <div class="search-item" data-url="${item.url}" style="
                display: flex;
                align-items: center;
                gap: 12px;
                padding: 12px 16px;
                cursor: pointer;
                transition: background 0.2s;
                border-bottom: 1px solid #eee;
            ">
                <span class="material-icons-sharp" style="color: #24086d;">${item.icon}</span>
                <span>${item.name}</span>
            </div>
        `).join('');

        searchDropdown.style.display = "block";

        document.querySelectorAll(".search-item").forEach(el => {
            el.addEventListener("click", function () {
                window.location.href = this.getAttribute("data-url");
            });

            el.addEventListener("mouseenter", function () {
                this.style.background = "#f0eff5";
            });

            el.addEventListener("mouseleave", function () {
                this.style.background = "";
            });
        });
    });

    document.addEventListener("click", function (e) {
        if (!searchBarContainer.contains(e.target)) {
            searchDropdown.style.display = "none";
        }
    });
}

// ========== card ==========  
async function loadMonthlyStatistics() {

    try {

        const response = await fetch("/api/Banking/transactions", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (!response.ok) return;

        const transactions = await response.json();

        const now = new Date();

        let income = 0;
        let expenses = 0;

        let incomeCount = 0;
        let expenseCount = 0;

        transactions.forEach(t => {

            const date = new Date(t.createdAt);

            if (
                date.getMonth() === now.getMonth() &&
                date.getFullYear() === now.getFullYear()
            ) {

                if (t.isOutgoing) {

                    // فلوس خرجت
                    expenses += t.amount;
                    expenseCount++;

                } else {

                    // فلوس دخلت
                    income += t.amount;
                    incomeCount++;

                }

            }

        });

        document.getElementById("monthlyIncome").textContent =
            "$" + income.toLocaleString();

        document.getElementById("monthlyExpenses").textContent =
            "$" + expenses.toLocaleString();

        document.getElementById("incomeCount").textContent =
            incomeCount;

        document.getElementById("expenseCount").textContent =
            expenseCount;

    }

    catch (err) {

        console.log(err);

    }

}
// ========== AUTH HELPER ==========
function getToken() {
    return localStorage.getItem("token");
}

function getAuthHeaders() {
    return {
        "Authorization": `Bearer ${getToken()}`,
        "Content-Type": "application/json"
    };
}

function handleUnauthorized() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}

// ========== LOAD BALANCE ==========
async function loadBalance() {
    const token = getToken();
    if (!token) {
        handleUnauthorized();
        return;
    }

    try {
        const response = await fetch("/api/Banking/balance", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (response.status === 401) {
            handleUnauthorized();
            return;
        }

        if (!response.ok) return;

        const data = await response.json();

        // Update balance in the first card
        const balanceEl = document.querySelector(".cards .card:first-child .middle h1");
        if (balanceEl) {
            balanceEl.textContent = "$" + data.balance.toLocaleString();
        }

        // Update account number in the first card
        const accountEl = document.querySelector(".cards .card:first-child .bottom .left h5");
        if (accountEl) {
            accountEl.textContent = data.accountNumber;
        }

    } catch (error) {
        console.error("Balance fetch error:", error);
    }
}

// ========== LOAD RECENT TRANSACTIONS ==========
async function loadRecentTransactions() {
    const token = getToken();
    if (!token) return;

    try {
        const response = await fetch("/api/Banking/transactions", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (!response.ok) return;

        const transactions = await response.json();

        const container = document.querySelector(".recent-transactions");
        if (!container) return;

        // Keep the header, replace the rest
        const header = container.querySelector(".header");
        container.innerHTML = "";
        container.appendChild(header);

        if (transactions.length === 0) {
            container.innerHTML += `<p style="padding: 1rem; color: #888;">No transactions yet.</p>`;
            return;
        }

        // Show last 5 transactions
        transactions.slice(0, 5).forEach(tx => {
            const isCredit = !tx.isOutgoing;
            const icon = tx.type === "Transfer" ? "send" :
                tx.type === "BillPayment" ? "payment" : "account_balance";
            const amountColor = isCredit ? "success" : "danger";
            const amountSign = isCredit ? "+" : "-";
            const date = new Date(tx.createdAt).toLocaleDateString("en-GB");

            container.innerHTML += `
                <div class="transaction">
                    <div class="service">
                        <div class="icons bg-purple-light">
                            <span class="material-icons-sharp purple">${icon}</span>
                        </div>
                        <div class="details">
                            <h4>${tx.type}</h4>
                            <p>${date}</p>
                        </div>
                    </div>
                    <div class="card-details">
                        <div class="details">
                            <p>${tx.description ?? ""}</p>
                            <small class="text-muted">${tx.referenceNumber ?? ""}</small>
                        </div>
                    </div>
                    <h4 class="${amountColor}">${amountSign}$${tx.amount.toLocaleString()}</h4>
                </div>
            `;
        });

    } catch (error) {
        console.error("Transactions fetch error:", error);
    }
}

// ========== DASHBOARD DATA (Admin Stats) ==========
async function loadDashboardData() {
    const token = getToken();
    if (!token) {
        handleUnauthorized();
        return;
    }

    try {
        const response = await fetch("/api/Monitoring/dashboard", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (response.status === 401) {
            handleUnauthorized();
            return;
        }

        if (!response.ok) return;

        const data = await response.json();
        updateDashboardUI(data);

    } catch (error) {
        console.error("Dashboard error:", error);
    }
}

function updateDashboardUI(data) {
    const statCards = document.querySelectorAll(".stat-card");

    if (statCards.length >= 4) {
        if (data.totalUsers) statCards[0].querySelector("h3").textContent = data.totalUsers.toLocaleString();
        if (data.totalTransactions) statCards[1].querySelector("h3").textContent = data.totalTransactions.toLocaleString();
        if (data.totalVolume) statCards[2].querySelector("h3").textContent = "$" + data.totalVolume.toLocaleString();
        if (data.pendingReviews) statCards[3].querySelector("h3").textContent = data.pendingReviews;
    }
}

// ========== SIDEBAR ==========
const menuBtn = document.querySelector('#menu-btn');
const closeBtn = document.querySelector('#close-btn');
const sidebar = document.querySelector('aside');

if (menuBtn && closeBtn && sidebar) {
    menuBtn.addEventListener('click', () => {
        sidebar.style.left = '0';
    });

    closeBtn.addEventListener('click', () => {
        sidebar.style.left = '-100%';
    });
}

// ========== THEME TOGGLE ==========


// ========== INIT ==========
document.addEventListener("DOMContentLoaded", () => {

    loadBalance();
    loadRecentTransactions();
    loadDashboardData();
    loadBillsChart();
    loadMonthlyStatistics();
    const dateEl = document.getElementById("currentDate");
    if (dateEl) {
        dateEl.textContent = new Date().toLocaleDateString("en-US", {
            weekday: "long", year: "numeric", month: "long", day: "numeric"
        });
    }

});
