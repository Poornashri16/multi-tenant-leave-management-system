document.addEventListener("DOMContentLoaded", async () => {

  const token = localStorage.getItem("token");

  if (!token) {
    alert("Please login");
    window.location.href = "index.html";
    return;
  }

  let user = {};
  let role = "";

  try {

    const resProfile = await fetch("/api/users/me", {
      headers: { "Authorization": `Bearer ${token}` }
    });

    if (!resProfile.ok) throw new Error("Failed to load profile");

    user = await resProfile.json();

    console.log("USER DATA:", user);

    role = (user.role || user.Role || "").toLowerCase();

    // Show form only for employees
    if (role !== "admin") {
      document.getElementById("reimFormSection").style.display = "block";
    }

    loadReimbursements();

  } catch (err) {
    console.error(err);
    alert("Error loading profile");
  }

  // ===============================
  // SUBMIT REIMBURSEMENT
  // ===============================

  const form = document.getElementById("reimForm");

  if (form) {

    form.addEventListener("submit", async (e) => {

      e.preventDefault();

      const amount = document.getElementById("reimAmount").value;
      const description = document.getElementById("reimDescription").value;

      try {

        const res = await fetch("/api/reimbursements", {
          method: "POST",
          headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            amount,
            description
          })
        });

        if (!res.ok) throw new Error("Submit failed");

        alert("Reimbursement submitted successfully");

        form.reset();

        loadReimbursements();

      } catch (err) {

        console.error(err);
        alert("Error submitting reimbursement");

      }

    });

  }

  // ===============================
  // LOAD REIMBURSEMENTS
  // ===============================

  async function loadReimbursements() {

    let endpoint = "";

    if (role === "admin") {
      endpoint = "/api/reimbursements/pending";
    } else {
      endpoint = "/api/reimbursements/my";
    }

    try {

      const res = await fetch(endpoint, {
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (!res.ok) throw new Error("Load failed");

      const data = await res.json();

      const tbody = document.querySelector("#reimTable tbody");

      tbody.innerHTML = "";

      data.forEach(r => {

        const tr = document.createElement("tr");

        let actions = "";

        if (role === "admin" && r.status === "Pending") {

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

        tbody.appendChild(tr);

      });

    } catch (err) {

      console.error(err);
      alert("Error loading reimbursements");

    }

  }

  // ===============================
  // APPROVE
  // ===============================

  window.approve = async function (id) {

    try {

      const res = await fetch(`/api/reimbursements/${id}/approve`, {
        method: "PUT",
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (!res.ok) throw new Error("Approve failed");

      loadReimbursements();

    } catch (err) {

      console.error(err);
      alert("Error approving reimbursement");

    }

  };

  // ===============================
  // REJECT
  // ===============================

  window.reject = async function (id) {

    try {

      const res = await fetch(`/api/reimbursements/${id}/reject`, {
        method: "PUT",
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (!res.ok) throw new Error("Reject failed");

      loadReimbursements();

    } catch (err) {

      console.error(err);
      alert("Error rejecting reimbursement");

    }

  };

});