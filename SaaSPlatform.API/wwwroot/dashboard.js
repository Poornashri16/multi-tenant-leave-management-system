document.addEventListener("DOMContentLoaded", async () => {

    console.log("✅ Dashboard JS Loaded");

    // -----------------------------
    // Get token & role
    // -----------------------------
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    console.log("Role from storage:", role);

    // -----------------------------
    // If not logged in
    // -----------------------------
    if (!token) {
        alert("Please login first");
        window.location.href = "index.html";
        return;
    }

    // -----------------------------
    // ⭐ SHOW ADMIN CARD (IMMEDIATE)
    // -----------------------------
    if (role && role.trim().toLowerCase() === "admin") {
        console.log("✅ Admin detected");

        const adminCard = document.getElementById("adminCard");

        if (adminCard) {
            adminCard.style.display = "block";
        } else {
            console.log("❌ adminCard not found");
        }
    }

    // -----------------------------
    // Fetch user details
    // -----------------------------
    try {
        const res = await fetch("/api/users/me", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!res.ok) throw new Error("Failed to fetch user");

        const user = await res.json();

        document.getElementById("username").innerText =
            user.name || user.email || "User";

        document.getElementById("userrole").innerText = role || "N/A";

        document.getElementById("useremail").innerText =
            user.email || "N/A";

    } catch (err) {
        console.error("❌ Error fetching user:", err);

        alert("Session expired. Please login again.");
        localStorage.clear();
        window.location.href = "index.html";
    }

});


// -----------------------------
// Navigation function
// -----------------------------
function navigateTo(page) {
    window.location.href = page;
}