# C# REST API Banking Application

This project implements a simple RESTful API for a banking system using C# and ASP.NET Core. The API allows you to create and manage bank accounts, make deposits and withdrawals, and transfer funds between accounts.

---

## Features

- Bank accounts:
  - Create accounts with unique, auto-generated account numbers
  - View account details
  - Deposit and withdraw funds
  - Transfer funds between accounts
- Authentication:
  - Create user for authentication to bank API
  - Login using created password to get access token
  - Refresh access token using valid refresh token
  - Revoke refresh token from active status

---

## Requirements

- [.NET 8 SDK ](https://dotnet.microsoft.com/download)
- Free Currency API Account: [freecurrencyapi.com](https://freecurrencyapi.com/)

---

## Running the Project

1. **Clone the repository:**
    ```sh
    git clone https://github.com/sonhyejoo/BankRestApi.git
    cd BankRestApi
    ```
2. Securely store API key in user secrets:
    ```
    dotnet user-secrets init
    dotnet set "apikey" "YOUR API KEY HERE"
    ```
3. **Build and run:**
    ```sh
    dotnet restore
    dotnet build
    dotnet run
    ```

4. **Create user for authentication and login**
    - Create new user at POST /api/users
    - Get access token and refresh token by logging in using created credentials at POST /api/authentication/login
    - Use access token in "Bearer" header for access to Bank API
    - Get new access token without logging in using valid refresh token /api/authentication/refresh-token

---

## User API Endpoints

### 1. Create a New User

**POST** /api/users
- **Request Body**
  ```json
  {
    "name": "Alice Smith",
    "password": "strongPasswordHere"
  }
  ```
- Response
  - 200 OK
  ```json
  {
    "name": "Alice Smith",
    "password": "strongPasswordHere"
  }
  ```
  - 400 Bad Request on invalid name

---

## Authentication Endpoints

### 1. Login
**POST** /api/authentication/login
- **Request Body**
  ```json
  {
    "name": "Alice Smith",
    "password": "strongPasswordHere"
  }
  ```
- Response
  - 200 OK
  ```json
  {
    "accessToken": "Alice Smith",
    "refreshToken": "strongPasswordHere"
  }
  ```
  - 404 Not Found on wrong name or password

### 2. Refresh access token
**POST** /api/authentication/refresh-token
- **Request Body**
  ```json
  {
    "name": "Alice Smith",
    "refreshToken": "ZkTaVVG0zTXfNszHC3zB3MRibqCzxokOnufekK88M6U="
  }
  ```
- Response
  - 200 OK
  ```json
  {
    "accessToken": "Alice Smith",
    "refreshToken": "strongPasswordHere"
  }
  ```
  - 404 not found on invalid name or refresh token

### 3. Revoke refresh token
**POST** /api/authentication/revoke
- **Request Body**
  ```json
  {
    "name": "Alice Smith",
    "refreshToken": "ZkTaVVG0zTXfNszHC3zB3MRibqCzxokOnufekK88M6U="
  }
  ```
- Response
  - 204 No Content
  - 404 Not Found on invalid name or refresh token


---

## Bank API Endpoints

### 1. Get List of Accounts

**GET** `/api/accounts`
- **Request Query Parameters**
  - string? name
  - string sortBy = ""
  - bool desc = false
  - int pageNumber = 1
  - int pageSize = 5
- **Response**
   - `200 OK`
    ```json
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Alice Smith",
      "balance": 0
    }
    ```
  - `400 Bad Request` if name is empty or whitespace

---

### 1. Create a New Account

**POST** `/api/accounts`
- **Request Body**
    ```json
    {
      "name": "Alice Smith"
    }
    ```
- **Response**
  - `201 Created`
  ```json
  [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Alice Smith",
      "balance": 0
    },
    {
      "id": "2ee2ea74-76a3-4666-b667-63fbe2435cd1",
      "name": "John Smith",
      "balance": 0
    }  
  ]
  ```
    - `400 Bad Request` if name is empty or whitespace

---

### 2. Get Account Details

**GET** `/api/accounts/{id}`
- **Response**
   - `200 OK`
    ```json
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Alice Smith",
      "balance": 0
    }
    ```
   - `400 Bad Request` if invalid id


---

### 3. Deposit Funds

**POST** `/api/accounts/{id}/deposits`
- **Request Body**
    ```json
    {
      "amount": 50
    }
    ```
- **Response**
   - `200 OK`
   ```json
  {
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Alice Smith",
  "balance": 50
  }
    ```
   - `400 Bad Request` if invalid id, invalid amount, or insufficient funds



---

### 4. Withdraw Funds

**POST** `/api/accounts/{id}/withdrawals`
- **Request Body**
    ```json
    {
      "amount": 50
    }
    ```
- **Response**
  - `200 OK`    
  ```json
    {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Alice Smith",
    "balance": 0
    }
  ```
  - `400 Bad Request` if invalid id, invalid amount, or insufficient funds

---

### 5. Transfer Funds

**POST** `/api/accounts/transfer`
- **Request Body**
    ```json
    {
      "amount": 25,
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "recipientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
    ```
- **Response**
  - `200 OK`
  ```json
    {
      "sender": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Alice Smith",
        "balance": 50
      },
      "recipient": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "John Smith",
        "balance": 75
      }
    }
  ```
  - `400 Bad Request` if invalid amount, insufficient funds, or invalid account(s)

---

### 6. Get Account Balance in Other Currencies

**GET** `/api/accounts/{id}/conversion`
- **Request Body**
  ```json
  {
    "currencies": ""
  }
  ```
  or 
  ```json
  {
    "currencies": "AUD,BGN,BRL"
  }
  ```

- **Response**
  - `200 OK`
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Alice Smith",
    "balance": 0,
    "balances": {
      "AUD": 30.59260479,
      "BGN": 33.37160448,
      "BRL": 111.574017814,
      "CAD": 27.359803406,
      "CHF": 15.855402378,
      "CNY": 143.495618862,
      "CZK": 419.432272406,
      "DKK": 127.174015902,
      "EUR": 17.045602262,
      "GBP": 14.712402764,
      "HKD": 156.953022156,
      "HRK": 124.54741407,
      "HUF": 6802.619868668,
      "IDR": 324555.00410498,
      "ILS": 66.358611172,
      "INR": 1712.255598214,
      "ISK": 2447.18308889,
      "JPY": 2922.245691926,
      "KRW": 27441.962864382,
      "MXN": 372.454061836,
      "MYR": 84.941012182,
      "NOK": 201.736027024,
      "NZD": 33.343605536,
      "PHP": 1130.275373334,
      "PLN": 72.222407832,
      "RON": 86.545411504,
      "RUB": 1562.733816422,
      "SEK": 189.958836314,
      "SGD": 25.585003174,
      "THB": 652.461530398,
      "TRY": 800.974124622,
      "USD": 20,
      "ZAR": 356.015851088    
    }
  }
  ````
  - `400 Bad Request` if invalid id
  - `422 Unprocessable Entity` if invalid currencies
    - see [freecurrencyapi.com/docs/status-codes#validation-errors](https://freecurrencyapi.com/docs/status-codes#validation-errors) for details

