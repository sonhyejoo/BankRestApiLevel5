# C# REST API Banking Application

This project implements a simple RESTful API for a banking system using C# and ASP.NET Core. The API allows you to create and manage bank accounts, make deposits and withdrawals, and transfer funds between accounts.

---

## Features

- Create accounts with unique, auto-generated account numbers
- View account details
- Deposit and withdraw funds
- Transfer funds between accounts

---

## Requirements

- [.NET 8 SDK ](https://dotnet.microsoft.com/download)

---

## Running the Project

1. **Clone the repository:**
    ```sh
    git clone https://github.com/sonhyejoo/BankRestApi.git
    cd BankRestApi
    ```
2. **Build and run:**
    ```sh
    dotnet restore
    dotnet build
    dotnet run
    ```
3. The API will be available at `https://localhost:5001` or `http://localhost:5000` by default.

---

## Account Model

The `Account` class has the following attributes:
- **ID**: auto-generated GUID
- **Name**: string
- **Balance**: decimal

---

## API Endpoints

### 1. Create a New Account

- **POST** `/api/accounts`
- **Request Body**
    ```json
    {
      "name": "Alice Smith"
    }
    ```
- **Response**
   - `201 Created`
    ```json
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Alice Smith",
      "balance": 0
    }
    ```
  - `400 Bad Request` if name is empty or whitespace

---

### 2. Get Account Details

- **GET** `/api/accounts/{id}`
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

- **POST** `/api/accounts/{id}/deposits`
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

- **POST** `/api/accounts/{id}/withdrawals`
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

- **POST** `/api/accounts/transfer`
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
