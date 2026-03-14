const API = "http://localhost:5221/api";

let token = localStorage.getItem("token");

// ================= LOGIN =================
async function login() {
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const role = document.getElementById("role").value;

    const res = await fetch(`${API}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (res.ok) {
        const data = await res.json();
        localStorage.setItem("token", data.token);
        alert("Login Success");

        if (role === "Admin")
            window.location = "admin.html";
        else
            window.location = "employee.html";
    } else {
        alert("Login Failed");
    }
}

// ================= LOGOUT =================
function logout() {
    localStorage.removeItem("token");
    window.location = "index.html";
}

// ================= ADMIN =================
let currentPage = 1;
const pageSize = 10;

// FETCH ALL LEAVES (with pagination)
async function fetchLeaves(page = 1) {
    const token = localStorage.getItem("token");

    const res = await fetch(`${API}/leaves/all?pageNumber=${page}&pageSize=${pageSize}`, {
        headers: { "Authorization": "Bearer " + token }
    });

    if (!res.ok) {
        console.log("API ERROR:", res.status);
        return;
    }

    const leaves = await res.json();
    renderLeaves(leaves);

    document.getElementById("currentPage").textContent = page;
    currentPage = page;
}

// RENDER LEAVES TABLE
function renderLeaves(leaves) {
    const tbody = document.getElementById("leavesTableBody");
    if (!tbody) return;

    tbody.innerHTML = "";

    leaves.forEach(l => {
        const tr = document.createElement("tr");
        const status = (l.status || "").toLowerCase();

        let actions = "";
        if (status === "pending") {
            actions = `
            <button class="approve-btn" onclick="approveLeave('${l.id}')">Approve</button>
            <button class="reject-btn" onclick="rejectLeave('${l.id}')">Reject</button>
            `;
        }

        tr.innerHTML = `
            <td>${l.userName || ""}</td>
            <td>${l.leaveType || ""}</td>
            <td>${new Date(l.startDate).toLocaleDateString()}</td>
            <td>${new Date(l.endDate).toLocaleDateString()}</td>
            <td>${l.reason || ""}</td>
            <td style="color:${getColor(l.status)}">${l.status}</td>
            <td>${actions}</td>
        `;
        tbody.appendChild(tr);
    });
}

// PAGINATION CONTROLS
function nextPage() {
    fetchLeaves(currentPage + 1);
}

function prevPage() {
    if (currentPage > 1) fetchLeaves(currentPage - 1);
}

// APPROVE LEAVE
async function approveLeave(id) {
    const token = localStorage.getItem("token");

    await fetch(`${API}/leaves/${id}/approve`, {
        method: "PUT",
        headers: { "Authorization": "Bearer " + token }
    });

    fetchLeaves(currentPage);
}

// REJECT LEAVE
async function rejectLeave(id) {
    const token = localStorage.getItem("token");

    await fetch(`${API}/leaves/${id}/reject`, {
        method: "PUT",
        headers: { "Authorization": "Bearer " + token }
    });

    fetchLeaves(currentPage);
}

// REGISTER USER (ADMIN)
async function registerUser() {
    const name = document.getElementById("newName").value;
    const email = document.getElementById("newEmail").value;
    const password = document.getElementById("newPassword").value;
    const role = document.getElementById("newRole").value;

    const token = localStorage.getItem("token");

    const res = await fetch(`${API}/users`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ name, email, password, role })
    });

    if (res.ok)
        alert("User Registered");
    else
        alert("Registration Failed");
}

// ================= EMPLOYEE =================
// APPLY LEAVE
async function applyLeave() {
    const startDate = document.getElementById("leaveStart").value;
    const endDate = document.getElementById("leaveEnd").value;
    const reason = document.getElementById("leaveReason").value;
    const leaveType = document.getElementById("leaveType").value;

    const token = localStorage.getItem("token");

    const res = await fetch(`${API}/leaves/apply`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            startDate: new Date(startDate).toISOString(),
            endDate: new Date(endDate).toISOString(),
            reason,
            leaveType
        })
    });

    if (res.ok) {
        alert("Leave Applied");
        fetchMyLeaves();
        fetchMyPendingLeaves();
    } else {
        alert("Apply Failed");
    }
}

// FETCH EMPLOYEE LEAVES
async function fetchMyLeaves() {
    const token = localStorage.getItem("token");

    const res = await fetch(`${API}/leaves/my`, {
        headers: { "Authorization": "Bearer " + token }
    });

    const leaves = await res.json();
    const tbody = document.getElementById("myLeavesBody");
    if (!tbody) return;

    tbody.innerHTML = "";

    leaves.forEach(l => {
        if (l.status && l.status.toLowerCase() === "pending") return;

        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${l.leaveType || "N/A"}</td>
            <td>${new Date(l.startDate).toLocaleDateString()}</td>
            <td>${new Date(l.endDate).toLocaleDateString()}</td>
            <td>${l.reason || ""}</td>
            <td style="color:${getColor(l.status)}">${l.status}</td>
        `;
        tbody.appendChild(tr);
    });
}

// FETCH PENDING LEAVES
async function fetchMyPendingLeaves() {
    const token = localStorage.getItem("token");

    const res = await fetch(`${API}/leaves/my-pending`, {
        headers: { "Authorization": "Bearer " + token }
    });

    const leaves = await res.json();
    const tbody = document.getElementById("pendingLeavesBody");
    if (!tbody) return;

    tbody.innerHTML = "";

    leaves.forEach(l => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${l.leaveType}</td>
            <td>${new Date(l.startDate).toLocaleDateString()}</td>
            <td>${new Date(l.endDate).toLocaleDateString()}</td>
            <td>${l.reason}</td>
            <td style="color:orange">${l.status}</td>
        `;
        tbody.appendChild(tr);
    });
}

// ================= STATUS COLOR =================
function getColor(status) {
    if (!status) return "black";

    status = status.toLowerCase();
    if (status === "approved") return "green";
    if (status === "rejected") return "red";
    return "orange"; // pending or unknown
}

// ================= PROFILE MODULE =================
async function loadProfile() {
    const token = localStorage.getItem("token");
    const res = await fetch(`${API}/profile/me`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) {
        console.error("Failed to load profile", res.status);
        return;
    }

    const data = await res.json();
    document.getElementById("profile-name").textContent = data.Name;
    document.getElementById("profile-email").textContent = data.Email;
    document.getElementById("profile-phone").textContent = data.PhoneNumber;
    document.getElementById("profile-department").textContent = data.Department;
    document.getElementById("profile-doj").textContent = new Date(data.DateOfJoining).toLocaleDateString();
}

function editProfile() {
    document.getElementById("edit-profile-section").style.display = "block";
    document.getElementById("edit-name").value = document.getElementById("profile-name").textContent;
    document.getElementById("edit-phone").value = document.getElementById("profile-phone").textContent;
    document.getElementById("edit-department").value = document.getElementById("profile-department").textContent;
}

async function saveProfile() {
    const token = localStorage.getItem("token");
    const dto = {
        Name: document.getElementById("edit-name").value,
        PhoneNumber: document.getElementById("edit-phone").value,
        Department: document.getElementById("edit-department").value
    };

    const res = await fetch(`${API}/profile/me`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(dto)
    });

    if (!res.ok) {
        const text = await res.text();
        console.error("Profile update failed:", res.status, text);
        alert("Profile update failed. Check console.");
        return;
    }

    document.getElementById("edit-profile-section").style.display = "none";
    loadProfile(); // reload updated profile
}

// ================= ON LOAD =================
window.onload = function() {
    fetchMyLeaves();
    fetchMyPendingLeaves();
    loadProfile(); // load profile on employee page
    if(document.getElementById("leavesTableBody")) fetchLeaves(); // for admin page
};