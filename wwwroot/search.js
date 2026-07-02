// ========== GLOBAL SEARCH ==========
(function() {
    const searchInput = document.querySelector(".search-bar input");
    
    if (!searchInput) return;
    
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
    if (searchBarContainer) {
        searchBarContainer.style.position = "relative";
        searchBarContainer.appendChild(searchDropdown);
    }
    
    const searchItems = [
        { name: "Dashboard", icon: "dashboard", url: "index.html" },
        { name: "Transfer Money", icon: "send", url: "transfer.html" },
        { name: "Transfer History", icon: "history", url: "transfer_his.html" },
        { name: "Beneficiaries", icon: "group", url: "Beneficiaries.HTML" },
        { name: "Bills Payment", icon: "payment", url: "bills.html" },
        { name: "Settings & Security", icon: "settings", url: "settings.html" },
        { name: "Support", icon: "help_center", url: "support.html" },
        { name: "Change Password", icon: "vpn_key", url: "change.html" },
        { name: "Logout", icon: "logout", url: "logout.html" }
    ];
    
    searchInput.addEventListener("input", function(e) {
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
            el.addEventListener("click", function() {
                window.location.href = this.getAttribute("data-url");
            });
            
            el.addEventListener("mouseenter", function() {
                this.style.background = "#f0eff5";
            });
            
            el.addEventListener("mouseleave", function() {
                this.style.background = "";
            });
        });
    });
    
    document.addEventListener("click", function(e) {
        if (searchBarContainer && !searchBarContainer.contains(e.target)) {
            searchDropdown.style.display = "none";
        }
    });
})();