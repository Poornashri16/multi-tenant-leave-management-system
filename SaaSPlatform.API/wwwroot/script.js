const API = "http://localhost:5221/api";

let token = localStorage.getItem("token");


// ================= LOGIN =================

async function login(){

const email = document.getElementById("email").value;
const password = document.getElementById("password").value;
const role = document.getElementById("role").value;

const res = await fetch(`${API}/auth/login`,{
method:"POST",
headers:{
"Content-Type":"application/json"
},
body:JSON.stringify({email,password})
});

if(res.ok){

const data = await res.json();

localStorage.setItem("token",data.token);

alert("Login Success");

if(role === "Admin")
window.location="admin.html";
else
window.location="employee.html";

}
else{
alert("Login Failed");
}

}



// ================= LOGOUT =================

function logout(){

localStorage.removeItem("token");

window.location="index.html";

}



// ================= ADMIN =================


// FETCH ALL LEAVES

async function fetchLeaves(){

const token = localStorage.getItem("token");

const res = await fetch(`${API}/leaves/all`,{
headers:{
"Authorization":"Bearer "+token
}
});

if(!res.ok){
console.log("API ERROR:",res.status);
return;
}

const leaves = await res.json();

console.log("Leaves from API:",leaves);

renderLeaves(leaves);

}



// RENDER LEAVES TABLE

function renderLeaves(leaves){

const tbody = document.getElementById("leavesTableBody");

if(!tbody) return;

tbody.innerHTML="";

leaves.forEach(l=>{

const tr=document.createElement("tr");

const status=(l.status || "").toLowerCase();

let actions="";

if(status==="pending"){

actions=`
<button onclick="approveLeave('${l.id}')">Approve</button>
<button onclick="rejectLeave('${l.id}')">Reject</button>
`;

}

tr.innerHTML=`
<td>${l.userName || ""}</td>
<td>${new Date(l.startDate).toLocaleDateString()}</td>
<td>${new Date(l.endDate).toLocaleDateString()}</td>
<td>${l.reason || ""}</td>
<td style="color:${getColor(l.status)}">${l.status}</td>
<td>${actions}</td>
`;

tbody.appendChild(tr);

});

}



// APPROVE LEAVE

async function approveLeave(id){

const token = localStorage.getItem("token");

await fetch(`${API}/leaves/${id}/approve`,{
method:"PUT",
headers:{
"Authorization":"Bearer "+token
}
});

fetchLeaves();

}



// REJECT LEAVE

async function rejectLeave(id){

const token = localStorage.getItem("token");

await fetch(`${API}/leaves/${id}/reject`,{
method:"PUT",
headers:{
"Authorization":"Bearer "+token
}
});

fetchLeaves();

}



// REGISTER USER (ADMIN)

async function registerUser(){

const name=document.getElementById("newName").value;
const email=document.getElementById("newEmail").value;
const password=document.getElementById("newPassword").value;
const role=document.getElementById("newRole").value;

const token = localStorage.getItem("token");

const res = await fetch(`${API}/users`,{
method:"POST",
headers:{
"Authorization":"Bearer "+token,
"Content-Type":"application/json"
},
body:JSON.stringify({name,email,password,role})
});

if(res.ok)
alert("User Registered");
else
alert("Registration Failed");

}



// ================= EMPLOYEE =================


// APPLY LEAVE

async function applyLeave(){

const startDate=document.getElementById("leaveStart").value;
const endDate=document.getElementById("leaveEnd").value;
const reason=document.getElementById("leaveReason").value;

const token = localStorage.getItem("token");

const res = await fetch(`${API}/leaves/apply`,{
method:"POST",
headers:{
"Authorization":"Bearer "+token,
"Content-Type":"application/json"
},
body:JSON.stringify({
startDate:new Date(startDate).toISOString(),
endDate:new Date(endDate).toISOString(),
reason
})
});

if(res.ok){

alert("Leave Applied");

fetchMyLeaves();

}
else{

alert("Apply Failed");

}

}



// FETCH EMPLOYEE LEAVES

async function fetchMyLeaves(){

const token = localStorage.getItem("token");

const res = await fetch(`${API}/leaves/my`,{
headers:{
"Authorization":"Bearer "+token
}
});

const leaves = await res.json();

const tbody=document.getElementById("myLeavesBody");

if(!tbody) return;

tbody.innerHTML="";

leaves.forEach(l=>{

const tr=document.createElement("tr");

tr.innerHTML=`
<td>${new Date(l.startDate).toLocaleDateString()}</td>
<td>${new Date(l.endDate).toLocaleDateString()}</td>
<td>${l.reason}</td>
<td style="color:${getColor(l.status)}">${l.status}</td>
`;

tbody.appendChild(tr);

});

}



// STATUS COLOR

function getColor(status){

if(!status) return "black";

status=status.toLowerCase();

if(status==="approved") return "green";
if(status==="rejected") return "red";

return "orange";

}