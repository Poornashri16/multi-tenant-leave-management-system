document.addEventListener("DOMContentLoaded", async () => {

  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  if (!token) {
    alert("Please login");
    window.location.href = "index.html";
    return;
  }

  if (role !== "Admin") {
    document.body.innerHTML = "<h2>Access Denied. Admins Only.</h2>";
    return;
  }

  const tableBody = document.querySelector("#reimTable tbody");

  // -----------------------------
  // Load all reimbursements
  // -----------------------------
  async function loadReimbursements() {

    try {

      const res = await fetch("/api/reimbursements/all", {
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });

      if (!res.ok) throw new Error("Failed to load reimbursements");

      const reimbursements = await res.json();

      tableBody.innerHTML = "";

      if (reimbursements.length === 0) {
        tableBody.innerHTML =
          `<tr><td colspan="6">No reimbursements found</td></tr>`;
        return;
      }

      reimbursements.forEach(r => {

        const tr = document.createElement("tr");

        let actions = "";

        if (r.status && r.status.toLowerCase() === "pending") {
          actions = `
          <button class="btn-approve" onclick="approve('${r.id}')">Approve</button>
          <button class="btn-reject" onclick="reject('${r.id}')">Reject</button>
          `;
        }

        tr.innerHTML = `
          <td>${r.employeeName || r.userName || "N/A"}</td>
          <td>${r.amount}</td>
          <td>${r.description}</td>
          <td>${r.status}</td>
          <td>${new Date(r.createdAt).toLocaleString()}</td>
          <td>${actions}</td>
        `;

        tableBody.appendChild(tr);
      });

    } catch (err) {

      console.error(err);
      alert("Error loading reimbursements");

    }
  }

  // -----------------------------
  // Approve reimbursement
  // -----------------------------
  window.approve = async (id) => {

    try {

      const res = await fetch(`/api/reimbursements/${id}/approve`, {
        method: "PUT",
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });

      if (!res.ok) throw new Error("Approve failed");

      loadReimbursements();

    } catch (err) {

      console.error(err);
      alert("Error approving reimbursement");

    }

  };

  // -----------------------------
  // Reject reimbursement
  // -----------------------------
  window.reject = async (id) => {

    try {

      const res = await fetch(`/api/reimbursements/${id}/reject`, {
        method: "PUT",
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });

      if (!res.ok) throw new Error("Reject failed");

      loadReimbursements();

    } catch (err) {

      console.error(err);
      alert("Error rejecting reimbursement");

    }

  };

  loadReimbursements();

});