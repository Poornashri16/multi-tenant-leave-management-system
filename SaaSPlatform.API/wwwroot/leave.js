document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role"); // "Employee" or "Admin"
    const employeeSection = document.getElementById("employeeSection");
    const tableTitle = document.getElementById("tableTitle");
    const leaveTableBody = document.querySelector("#leaveTable tbody");

    if (!token || !role) {
        alert("Please login first.");
        window.location.href = "index.html";
        return;
    }

    // Show Employee form only for Employee role
    if (role === "Employee") {
        employeeSection.style.display = "block";
        tableTitle.innerText = "My Leaves";
        loadLeavesForEmployee();
    } else {
        tableTitle.innerText = "All Employee Leaves";
        loadLeavesForAdmin();
    }

    // ================= Employee Apply Leave =================
    if (role === "Employee") {
        document.getElementById("leaveForm").addEventListener("submit", async (e) => {
            e.preventDefault();

            const leaveData = {
                startDate: new Date(document.getElementById("startDate").value).toISOString(),
                endDate: new Date(document.getElementById("endDate").value).toISOString(),
                reason: document.getElementById("reason").value,
                leaveType: document.getElementById("leaveType").value
            };

            try {
                const res = await fetch("/api/leaves/apply", {
                    method: "POST",
                    headers: {
                        "Authorization": `Bearer ${token}`,
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(leaveData)
                });

                if (!res.ok) {
                    const err = await res.json().catch(() => null);
                    throw new Error(err?.message || "Failed to apply leave");
                }

                alert("Leave applied successfully!");
                document.getElementById("leaveForm").reset();
                loadLeavesForEmployee();
            } catch (err) {
                console.error(err);
                alert("Error applying leave: " + err.message);
            }
        });
    }

    // ================= Load Leaves Functions =================
    async function loadLeavesForEmployee() {
        leaveTableBody.innerHTML = "";
        try {
            const res = await fetch("/api/leaves/my", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!res.ok) throw new Error("Failed to fetch leaves");

            const leaves = await res.json();

            leaves.forEach(l => {
                const statusClass = l.status.toLowerCase() === "pending" ? "status-pending"
                    : l.status.toLowerCase() === "approved" ? "status-approved"
                    : "status-rejected";

                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${l.userName || "Me"}</td>
                    <td>${l.startDate.split("T")[0]}</td>
                    <td>${l.endDate.split("T")[0]}</td>
                    <td>${l.reason}</td>
                    <td class="${statusClass}">${l.status}</td>
                    <td>${l.leaveType}</td>
                    <td>-</td>
                `;
                leaveTableBody.appendChild(tr);
            });
        } catch (err) {
            console.error(err);
            alert("Error loading leaves: " + err.message);
        }
    }

    async function loadLeavesForAdmin() {
        leaveTableBody.innerHTML = "";
        try {
            const res = await fetch("/api/leaves/all", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!res.ok) throw new Error("Failed to fetch leaves");

            const leaves = await res.json();

            leaves.forEach(l => {
                const statusClass = l.status.toLowerCase() === "pending" ? "status-pending"
                    : l.status.toLowerCase() === "approved" ? "status-approved"
                    : "status-rejected";

                const tr = document.createElement("tr");

                // Ensure leave ID is valid
                const leaveId = l.id || "";

                tr.innerHTML = `
                    <td>${l.userName}</td>
                    <td>${l.startDate.split("T")[0]}</td>
                    <td>${l.endDate.split("T")[0]}</td>
                    <td>${l.reason}</td>
                    <td class="${statusClass}">${l.status}</td>
                    <td>${l.leaveType}</td>
                    <td>
                        ${l.status.toLowerCase() === "pending" ? `
                            <button class="action-btn btn-approve" onclick="updateLeave('${leaveId}','Approved')">Approve</button>
                            <button class="action-btn btn-reject" onclick="updateLeave('${leaveId}','Rejected')">Reject</button>
                        ` : "-"}
                    </td>
                `;
                leaveTableBody.appendChild(tr);
            });
        } catch (err) {
            console.error(err);
            alert("Error loading leaves: " + err.message);
        }
    }

    // ================= Admin Approve/Reject =================
    window.updateLeave = async (id, status) => {
        if (!id) {
            alert("Leave ID is missing!");
            return;
        }

        console.log("Updating leave:", id, "Status:", status);

        try {
            // Correct backend endpoints
            const url = status.toLowerCase() === "approved"
                ? `/api/leaves/${id}/approve`
                : `/api/leaves/${id}/reject`;

            const res = await fetch(url, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!res.ok) {
                console.log("Response status:", res.status);
                const errData = await res.json().catch(() => null);
                console.log("Response body:", errData);
                let errMsg = errData?.message || "Failed to update leave";
                throw new Error(errMsg);
            }

            alert(`Leave ${status.toLowerCase()} successfully`);
            loadLeavesForAdmin();
        } catch (err) {
            console.error(err);
            alert("Error updating leave: " + err.message);
        }
    };

    // ================= Go Back =================
    window.goBack = () => window.location.href = "dashboard.html";
});