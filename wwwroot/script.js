// ================= REGISTER =================
const regForm = document.getElementById("registerForm");

if (regForm) {
  regForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    const userData = {
      username: document.getElementById("fullName").value,
      email: document.getElementById("email").value,
      password: document.getElementById("password").value,
      phoneNumber: document.getElementById("phone").value,
      nationalId: document.getElementById("idNumber").value,
      address: document.getElementById("address").value,
      dateOfBirth: document.getElementById("dob").value,
      accountType: document.getElementById("accountType").value
    };

      try {
          const response = await fetch("/api/Auth/register", {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify(userData)
          });
      const data = await response.json();
      console.log("Backend error:", JSON.stringify(data));

      if (response.ok) {
        toast("✅ Account created! Please login.");
        container.classList.remove("right-active");
        regForm.reset();
      } else {
        toast("❌ " + (data.message || data.title || "Registration failed"));
      }
    } catch (error) {
      toast("❌ Network error: " + error.message);
    }
  });
}

// ================= LOGIN =================
const loginForm = document.getElementById("loginForm");

if (loginForm) {
  loginForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    const btn = document.getElementById("loginBtnMain");
    btn.classList.add("loading");

    const email    = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPass").value.trim();

    try {
        const response = await fetch("/api/Auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      const data = await response.json();

        if (response.ok) {
            console.log(data);

            localStorage.setItem("fullName", data.username);

            localStorage.setItem("userEmail", data.email);

            localStorage.setItem("token", data.token);

            localStorage.setItem("loggedIn", "true");

            console.log(data.Username);

            window.location = "index.html";
        
      } else {
        toast("❌ " + (data.message || "Wrong email or password"));
        btn.classList.remove("loading");
      }
    } catch (error) {
      toast("❌ Network error: " + error.message);
      btn.classList.remove("loading");
    }
  });
}

// ================= LOGOUT =================
function logout() {

    localStorage.removeItem("loggedIn");

    localStorage.removeItem("token");

    localStorage.removeItem("fullName");

    localStorage.removeItem("userEmail");

    window.location = "login.html";
}

// ================= PROTECTION FOR PROTECTED PAGES =================
const protectedPages = ["index.html", "profile.html", "support.html", "settings.html", "dashboard.html"];

if (protectedPages.some(page => window.location.pathname.includes(page))) {
  if (!localStorage.getItem("loggedIn")) {
    window.location = "login.html";
  }
}

// ================= SUPPORT FORM =================
const supportForm = document.getElementById("supportForm");
if (supportForm) {
  supportForm.addEventListener("submit", function (e) {
    e.preventDefault();
    const ticket = {
      subject: document.getElementById("subject").value,
      message: document.getElementById("message").value,
      date: new Date().toLocaleString(),
    };
    let tickets = JSON.parse(localStorage.getItem("tickets")) || [];
    tickets.push(ticket);
    localStorage.setItem("tickets", JSON.stringify(tickets));
    toast("Message sent to support ✅");
    supportForm.reset();
  });
}

// ================= TOAST =================
function toast(msg) {
  const t = document.createElement("div");
  t.className = "toast";
  t.innerText = msg;
  document.body.appendChild(t);

  setTimeout(() => t.classList.add("show"), 100);
  setTimeout(() => {
    t.classList.remove("show");
    setTimeout(() => t.remove(), 400);
  }, 3000);
}

// ================= OVERLAY SWITCH =================
const container = document.getElementById("container");
const registerBtn = document.getElementById("registerBtn");
const loginBtn = document.getElementById("loginBtn");

if (registerBtn) {
  registerBtn.addEventListener("click", () => container.classList.add("right-active"));
}
if (loginBtn) {
  loginBtn.addEventListener("click", () => container.classList.remove("right-active"));
}