const API_URL = "/api/users";

// Load users + profile
document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");

    if (!token) {
        window.location.href = "index.html";
        return;
    }

    // Load profile
    try {
        const res = await fetch("/api/users/me", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const user = await res.json();

        document.getElementById("username").innerText = user.email;
        document.getElementById("useremail").innerText = user.email;

        if (user.role !== "Admin") {
            alert("Access denied");
            window.location.href = "dashboard.html";
            return;
        }

    } catch (err) {
        alert("Session expired");
        window.location.href = "index.html";
    }

    loadUsers();
});


// CREATE USER
async function createUser() {
    const token = localStorage.getItem("token");

    const data = {
        name: document.getElementById("name").value,
        email: document.getElementById("email").value,
        password: document.getElementById("password").value,
        department: document.getElementById("department").value,
        phoneNumber: document.getElementById("phone").value,
        role: document.getElementById("role").value
    };

    const res = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(data)
    });

    if (res.ok) {
        alert("✅ User created successfully");
        clearForm();
        loadUsers();
    } else {
        const msg = await res.text();
        alert("❌ Error: " + msg);
    }
}


// LOAD USERS
async function loadUsers() {
    const token = localStorage.getItem("token");

    const res = await fetch(API_URL, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    const users = await res.json();

    const table = document.getElementById("userTable");
    table.innerHTML = "";

    users.forEach(user => {
        table.innerHTML += `
            <tr>
                <td>${user.name}</td>
                <td>${user.email}</td>
                <td>${user.role}</td>
                <td>
                    <button onclick="deleteUser('${user.id}')">Delete</button>
                </td>
            </tr>
        `;
    });
}


// DELETE USER
async function deleteUser(id) {
    const token = localStorage.getItem("token");

    if (!confirm("Are you sure you want to delete this user?")) return;

    const res = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (res.ok) {
        alert("User deleted");
        loadUsers();
    } else {
        alert("Error deleting user");
    }
}


// CLEAR FORM
function clearForm() {
    document.getElementById("name").value = "";
    document.getElementById("email").value = "";
    document.getElementById("password").value = "";
    document.getElementById("department").value = "";
    document.getElementById("phone").value = "";
    document.getElementById("role").value = "Employee";
}