// Fetch logged-in user profile
document.addEventListener("DOMContentLoaded", async () => {
    try {
        const token = localStorage.getItem("token"); // assuming JWT stored in localStorage
        const res = await fetch("/api/users/me", {
            headers: { "Authorization": `Bearer ${token}` }
        });
        if (!res.ok) throw new Error("Failed to fetch profile");
        const user = await res.json();
        document.getElementById("username").innerText = user.name;
        document.getElementById("userrole").innerText = user.role;
        document.getElementById("useremail").innerText = user.email;
    } catch (err) {
        console.error(err);
        alert("Error loading profile. Please login again.");
        window.location.href = "index.html";
    }
});

// Navigate to module pages
function navigateTo(page) {
    window.location.href = page;
}