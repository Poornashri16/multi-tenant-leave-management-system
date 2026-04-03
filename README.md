# Multi-Tenant SaaS HR Management System

## Overview

This project is a full-stack Multi-Tenant SaaS HR Management System developed using C#, ASP.NET Core Web API, PostgreSQL, HTML, CSS, and JavaScript. It enables multiple companies (tenants) to securely manage their employees, tasks, leave requests, and reimbursements on a single platform with complete data isolation.

---

## Objective

The main objective of this project is to design and develop a real-world SaaS-based HR platform where:

* Multiple companies can register and use a shared application
* Each company has its own secure and isolated data
* Admins can manage employees and operations efficiently
* Employees can interact with HR processes digitally

---

## Key Features

### Multi-Tenant Architecture

* Each company is assigned a unique TenantId
* All data is filtered based on TenantId
* Ensures complete data isolation between companies

### Authentication and Authorization

* Secure login using JWT (JSON Web Token)
* Token contains user role and TenantId
* Role-based access control implemented

### Role-Based Access

Admin:

* Add and manage employees
* Assign tasks
* Approve or reject leave and reimbursement requests

Employee:

* View assigned tasks
* Apply for leave
* Submit reimbursement requests
* Track request status

### HR Management Modules

* Employee Management
* Task Management
* Leave Management
* Reimbursement Management

---

## Technologies Used

### Backend

* C#
* ASP.NET Core Web API
* Entity Framework Core (ORM)
* PostgreSQL

### Frontend

* HTML
* CSS
* JavaScript (Fetch API)

### Security

* JWT Authentication
* BCrypt Password Hashing
* Role-Based Authorization

---

## System Architecture

Frontend (HTML, CSS, JavaScript)
↓
ASP.NET Core Web API (Controllers)
↓
Service Layer (Business Logic)
↓
Entity Framework Core (ORM)
↓
PostgreSQL Database

---

## Security Implementation

* Passwords are securely stored using BCrypt hashing
* JWT tokens are used for authentication and session management
* Role-based access ensures only authorized users can perform actions
* TenantId-based filtering prevents cross-company data access

---

## Multi-Tenancy Explained

* Each company is assigned a unique GUID-based TenantId
* All entities (Users, Tasks, Leave Requests, Reimbursements) include TenantId
* TenantId is extracted from the JWT token in every API request
* Queries are filtered using TenantId to ensure data isolation

---

## Getting Started

### 1. Clone the Repository

git clone [https://github.com/Poornashri16/multi-tenant-leave-management-system](https://github.com/Poornashri16/multi-tenant-leave-management-system)

### 2. Navigate to the Project Directory

cd multi-tenant-leave-management-system

### 3. Configure Database

* Update the PostgreSQL connection string in appsettings.json

### 4. Apply Migrations

dotnet ef database update

### 5. Run the Backend

dotnet run

### 6. Run the Frontend

* Open the HTML files in your browser

---

## Future Enhancements

* Email notifications
* File upload for reimbursement bills
* Dashboard analytics and reporting
* Mobile application support

---

## Key Learning Outcomes

* Implementation of multi-tenant architecture
* Secure authentication using JWT
* Role-based authorization in real-world applications
* Backend development using .NET Core
* Database design and integration using PostgreSQL
* Full-stack development using JavaScript and Web APIs

---


