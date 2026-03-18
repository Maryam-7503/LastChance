# Authentication & Security Module

## Overview
ASP.NET Core Web API implementing complete authentication and security module.

## Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | /api/auth/register | Register new user | No |
| POST | /api/auth/login | Login | No |
| POST | /api/auth/logout | Logout | Yes |
| POST | /api/auth/change-password | Change password | Yes |
| POST | /api/auth/forgot-password | Request reset token | No |
| POST | /api/auth/reset-password | Reset password | No |

### Admin
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/admin/users | Get all users | Admin |
| PUT | /api/admin/users/{id}/toggle-active | Toggle user status | Admin |
| POST | /api/admin/2fa/setup | Setup 2FA | Admin |
| POST | /api/admin/2fa/enable | Enable 2FA | Admin |

## Security Features
- JWT Authentication (24h expiry)
- BCrypt Password Hashing
- Password Policy (8+ chars, upper, lower, number, special)
- Password History (last 3 passwords)
- Account Lockout (5 failed attempts = 15 min lock)
- Role-Based Authorization (Admin/User)
- Two-Factor Authentication (TOTP)

## Tech Stack
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- JWT Bearer
- BCrypt.Net
- Otp.NET