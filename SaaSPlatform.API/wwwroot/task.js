const token = localStorage.getItem("token"); // JWT token
const userRole = localStorage.getItem("role"); // Admin or Employee
const apiBase = "http://localhost:5221/api/tasks";

const adminSection = document.getElementById("adminSection");
const tasksTableBody = document.querySelector("#tasksTable tbody");

// Show admin section only if Admin
if (userRole === "Admin") {
    adminSection.style.display = "block";
    loadEmployees();
}

// -------------------------
// Load Employees for Dropdown
// -------------------------
async function loadEmployees() {
    try {
        const res = await fetch(`${apiBase}/employees`, {
            headers: { "Authorization": `Bearer ${token}` }
        });
        const employees = await res.json();
        const select = document.getElementById("assignedTo");
        select.innerHTML = "";
        employees.forEach(emp => {
            const option = document.createElement("option");
            option.value = emp.id;
            option.text = emp.name;
            select.add(option);
        });
    } catch (err) {
        console.error("Failed to load employees", err);
    }
}

// -------------------------
// Load Tasks
// -------------------------
async function loadTasks() {
    let url = userRole === "Admin" ? `${apiBase}/all` : `${apiBase}/my`;
    try {
        const res = await fetch(url, {
            headers: { "Authorization": `Bearer ${token}` }
        });
        const tasks = await res.json();
        displayTasks(tasks);
    } catch (err) {
        console.error("Failed to load tasks", err);
    }
}

// -------------------------
// Display Tasks in Table
// -------------------------
function displayTasks(tasks) {
    tasksTableBody.innerHTML = "";
    tasks.forEach(task => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${task.title}</td>
            <td>${task.description}</td>
            <td>${task.assignedToUserName || "-"}</td>
            <td>${new Date(task.dueDate).toLocaleDateString()}</td>
            <td class="status-${task.status.toLowerCase().replace(' ', '')}">${task.status}</td>
            <td>${generateActions(task)}</td>
        `;
        tasksTableBody.appendChild(tr);
    });
}

// -------------------------
// Generate Action Buttons
// -------------------------
function generateActions(task) {
    if (userRole === "Employee" && task.status !== "Completed") {
        return `<button onclick="updateStatus('${task.id}','Completed')">Mark Completed</button>`;
    }
    return "-";
}

// -------------------------
// Update Task Status (Employee)
// -------------------------
async function updateStatus(taskId, status) {
    try {
        const res = await fetch(`${apiBase}/${taskId}/status`, {
            method: "PUT",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ status })
        });
        if (res.ok) {
            loadTasks();
        } else {
            const err = await res.text();
            alert("Error: " + err);
        }
    } catch (err) {
        console.error(err);
    }
}

// -------------------------
// Admin Create Task Form
// -------------------------
if (userRole === "Admin") {
    document.getElementById("createTaskForm").addEventListener("submit", async e => {
        e.preventDefault();
        const dto = {
            title: document.getElementById("title").value,
            description: document.getElementById("description").value,
            assignedToUserId: document.getElementById("assignedTo").value,
            dueDate: document.getElementById("dueDate").value
        };
        try {
            const res = await fetch(apiBase, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(dto)
            });
            if (res.ok) {
                alert("Task Created!");
                loadTasks();
            } else {
                const err = await res.text();
                alert("Error: " + err);
            }
        } catch (err) {
            console.error(err);
        }
    });
}

// Initial load
loadTasks();