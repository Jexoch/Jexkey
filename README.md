# Jexkey — Zero-Knowledge Password Manager API

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)](#project-architecture)
[![Database](https://img.shields.io/badge/Database-PostgreSQL-336791?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Jexkey** is a password manager Web API built around a **Zero-Knowledge** security model.

The server never receives or stores a user's master password, vault plaintext, or decryption keys. Encryption and decryption happen on the client; the server only stores and synchronizes ciphertext and authentication parameters.

> **Status:** early-stage learning project. The cryptographic design has **not** been independently audited. Do not use it to store real credentials.

---

## Features

- **Zero-Knowledge design:** a full database leak exposes only ciphertext, salts and hashed authentication values.
- **Authenticated encryption:** vault items are protected with AES-GCM (nonce + authentication tag).
- **IDOR prevention:** ownership is verified at the service and repository layers, so users cannot read, modify or delete other users' vaults and items.
- **Stateless JWT authentication** with issuer and lifetime validation.
- **Clean Architecture:** Domain, Application, Infrastructure and WebAPI layers with dependency inversion.

---

## Security Model

### What the server stores

| Data | Stored on server | Form |
|------|------------------|------|
| Master password | Never | — |
| Master key (encryption key) | Never | — |
| Master salt | Yes | 16-byte random salt (provided by client during registration) |
| Authentication hash (`MasterHash`) | Yes | Re-hashed server-side with 100k PBKDF2 iterations & per-user `AuthSalt` |
| Vault names | Yes | Ciphertext |
| Item title, username, password, notes | Yes | Ciphertext (AES-GCM) |

### Key derivation

From the master password and a per-user salt, the client is expected to derive two separate values using PBKDF2 (this is a client-side contract; the server receives only the final derived hash and cannot enforce client-side PRF iterations):

1. **MasterKey:** used only for encrypting and decrypting vault data with AES-256-GCM. It never leaves the client.
2. **MasterHash:** used only for authentication against the server. The server verifies it in constant time (`FixedTimeEquals`) after re-hashing with `AuthSalt`.

| Parameter | Value | Specification |
|-----------|-------|---------------|
| Pseudo-Random Function (PRF) | HMAC-SHA256 | Cryptographically secure PRF (client contract) |
| Client Iteration Count | 600,000 | In accordance with OWASP Password Storage Guidance |
| Server Iteration Count | 100,000 | Server-side re-hashing with per-user `AuthSalt` |
| Salt Length | 16 bytes (128 bits) | Generated via CSPRNG |
| Output Derived Length | 32 bytes (256 bits) | Standard 256-bit symmetric key length |

### Registration and login flow

**Registration Flow:**
1. The client generates a random 16-byte `MasterSalt`.
2. The client derives `MasterKey` and `MasterHash` from the master password and `MasterSalt`.
3. The client sends `MasterHash` and `MasterSalt` to `POST /api/auth/register`.
4. The server generates a unique 16-byte `AuthSalt`, re-hashes `MasterHash` with it (PBKDF2-HMAC-SHA256, 100,000 iterations), and stores the result, preventing pass-the-hash attacks if the database is leaked.

**Login Flow:**
1. The client retrieves the user's `MasterSalt` via `GET /api/auth/salt/{identity}`.
2. The client derives `MasterHash` client-side using the retrieved salt and the user's master password.
3. The client sends `MasterHash` to `POST /api/auth/login`.
4. The server re-hashes the incoming hash with the stored `AuthSalt` (or performs a dummy calculation if the user does not exist to mitigate timing side-channels) and verifies it using `FixedTimeEquals`, issuing a JWT on success.
5. Vault names and item fields are encrypted/decrypted exclusively on the client with `MasterKey`.

### Known limitations

- **User enumeration via salt:** The `/api/auth/salt/{identity}` endpoint currently returns a newly generated random salt for non-existent users, leaking user existence across repeated calls (deterministic fake salt via HMAC-SHA256 planned in next step). *Note: Login timing side-channel has been neutralized with dummy PBKDF2 execution.*
- **Payload validation:** DTOs currently lack explicit byte-length boundaries for `MasterHash` and `MasterSalt`.
- **No client yet:** This repository contains only the API; the client-side crypto flow above must be implemented by a client.
- **Server metadata exposure:** The server still sees metadata: account existence, timestamps, number and size of items.
- **Unrecoverable credentials:** A forgotten master password is unrecoverable by design.
- **No independent security audit.**

---

## Project Architecture

```
Jexkey/
├── Jexkey.Domain/          # Pure C# entities, enums, repository abstractions
├── Jexkey.Application/     # Business logic, services, DTOs, token generation contracts
├── Jexkey.Infrastructure/  # EF Core, PostgreSQL DbContext, configurations, repositories
└── Jexkey.WebAPI/          # Controllers, exception middleware, DI configuration
```

Dependencies point inward: `WebAPI → Infrastructure → Application → Domain`.

---

## API Overview

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/auth/salt/{identity}` | Retrieve user salt (or random fake salt if non-existent) | No |
| POST | `/api/auth/register` | Register account with `MasterHash` & `MasterSalt` | No |
| POST | `/api/auth/login` | Authenticate with `MasterHash`, receive JWT | No |
| GET | `/api/vaults` | List all vaults belonging to user | Bearer JWT |
| GET | `/api/vaults/{id}` | Get vault by ID | Bearer JWT |
| POST | `/api/vaults` | Create a new encrypted vault | Bearer JWT |
| DELETE | `/api/vaults/{id}` | Delete a vault and all its contents | Bearer JWT |
| GET | `/api/items/vault/{vaultId}` | List encrypted items in a specific vault | Bearer JWT |
| GET | `/api/items/{id}` | Get encrypted item by ID | Bearer JWT |
| POST | `/api/items` | Add encrypted item to vault | Bearer JWT |
| PUT | `/api/items/{id}` | Update encrypted item | Bearer JWT |
| DELETE | `/api/items/{id}` | Delete encrypted item | Bearer JWT |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/)

### 1. Clone

```bash
git clone https://github.com/Jexoch/Jexkey.git
cd Jexkey
```

### 2. Configure secrets

Do **not** put real credentials in `appsettings.json`. Configure the database connection and JWT authentication settings using .NET user secrets:

```bash
cd Jexkey.WebAPI
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PGConn" "Server=127.0.0.1;Port=5432;Database=JexkeyDB;User Id=postgres;Password=YOUR_PASSWORD;"
dotnet user-secrets set "Jwt:Key" "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG!"
dotnet user-secrets set "Jwt:Issuer" "JexkeyAPI"
dotnet user-secrets set "Jwt:Audience" "JexkeyClient"
```

Alternatively, use environment variables:

```bash
export ConnectionStrings__PGConn="Server=127.0.0.1;Port=5432;Database=JexkeyDB;User Id=postgres;Password=YOUR_PASSWORD;"
export Jwt__Key="YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG!"
export Jwt__Issuer="JexkeyAPI"
export Jwt__Audience="JexkeyClient"
```

### 3. Apply migrations and run

```bash
dotnet restore
dotnet ef database update --project Jexkey.Infrastructure --startup-project Jexkey.WebAPI
dotnet run --project Jexkey.WebAPI
```

The API listens on `http://localhost:5245` by default.

---

## Roadmap

- [ ] Unit tests for the crypto and ownership-check logic
- [ ] GitHub Actions build workflow
- [ ] Reference client implementing the client-side crypto flow
- [ ] Detailed security documentation and threat model

---

## License

Licensed under the [MIT License](LICENSE).
