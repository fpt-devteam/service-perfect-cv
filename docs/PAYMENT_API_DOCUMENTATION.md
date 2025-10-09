# Payment, Billing & Package API Documentation

This document provides comprehensive API documentation for the Payment, Billing History, and Package management endpoints for frontend integration.

---

## Table of Contents

1. [Payment API](#payment-api)
2. [Billing History API](#billing-history-api)
3. [Package API](#package-api)
4. [Common Error Responses](#common-error-responses)
5. [Authentication](#authentication)

---

## Payment API

Base URL: `/api/payment`

### 1. Create Payment Link

Creates a new payment link using PayOS API.

**Endpoint:** `POST /api/payment/create-payment`

**Authentication:** Required (Bearer Token)

**Request Body:**
```json
{
  "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561"
}
```

**Success Response (200 OK):**
```json
{
  "checkoutUrl": "https://payos.vn/checkout/...",
  "qrCode": "https://payos.vn/qr/...",
  "orderCode": 123456789,
  "paymentLinkId": "abc123def456",
  "amount": 99000,
  "currency": "VND",
  "description": "Payment for Premium Package"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid payment request, amount mismatch, or package inactive
- `404 Not Found` - Package not found
- `500 Internal Server Error` - Payment creation failed or configuration error
- `503 Service Unavailable` - Payment service unavailable

**Example cURL:**
```bash
curl -X POST https://api.example.com/api/payment/create-payment \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561"}'
```

---

### 2. Get Payment Information

Retrieves payment information by order code.

**Endpoint:** `GET /api/payment/payment-info/{orderCode}`

**Authentication:** Required (Bearer Token)

**Path Parameters:**
- `orderCode` (integer, required) - The order code to lookup

**Success Response (200 OK):**
```json
{
  "orderCode": 123456789,
  "amount": 99000,
  "amountPaid": 99000,
  "amountRemaining": 0,
  "status": "PAID",
  "createdAt": "2024-01-15T10:30:00Z",
  "transactionDateTime": "2024-01-15T10:35:00Z",
  "description": "Payment for Premium Package",
  "accountNumber": "1234567890",
  "reference": "REF123456",
  "counterAccountBankId": "",
  "counterAccountBankName": "",
  "counterAccountName": "",
  "counterAccountNumber": "",
  "virtualAccountName": "",
  "virtualAccountNumber": ""
}
```

**Error Responses:**
- `400 Bad Request` - Invalid order code
- `404 Not Found` - Payment info not found
- `500 Internal Server Error` - Payment service unavailable

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/payment/payment-info/123456789 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### 3. Cancel Payment

Cancels a payment by order code.

**Endpoint:** `POST /api/payment/payment/{orderCode}/cancel`

**Authentication:** Required (Bearer Token)

**Path Parameters:**
- `orderCode` (integer, required) - The order code to cancel

**Request Body:**
```json
{
  "cancellationReason": "User requested cancellation"
}
```

**Success Response (200 OK):**
```json
{
  "orderCode": 123456789,
  "status": "CANCELLED",
  "cancelledAt": "2024-01-15T11:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid order code or cancellation failed
- `404 Not Found` - Payment info not found
- `409 Conflict` - Payment already cancelled
- `500 Internal Server Error` - Payment service unavailable

**Example cURL:**
```bash
curl -X POST https://api.example.com/api/payment/payment/123456789/cancel \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"cancellationReason": "User requested cancellation"}'
```

---

### 4. Get Payment Status

Retrieves payment status summary by order code.

**Endpoint:** `GET /api/payment/status/{orderCode}`

**Authentication:** Required (Bearer Token)

**Path Parameters:**
- `orderCode` (integer, required) - The order code to check status

**Success Response (200 OK):**
```json
{
  "orderCode": 123456789,
  "status": "PAID",
  "amount": 99000,
  "createdAt": "2024-01-15T10:30:00Z",
  "transactionDateTime": "2024-01-15T10:35:00Z"
}
```

**Payment Status Values:**
- `PENDING` - Payment is pending
- `PAID` - Payment completed successfully
- `CANCELLED` - Payment was cancelled
- `EXPIRED` - Payment link expired

**Error Responses:**
- `400 Bad Request` - Invalid order code
- `404 Not Found` - Payment info not found
- `500 Internal Server Error` - Payment service unavailable

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/payment/status/123456789 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### 5. Process Payment Webhook

Receives and processes payment webhook from PayOS (used by PayOS, not frontend).

**Endpoint:** `POST /api/payment/receive-hook`

**Authentication:** Not required (webhook endpoint)

**Request Body:**
```json
{
  "code": "00",
  "desc": "success",
  "data": {
    "orderCode": 123456789,
    "amount": 99000,
    "description": "Payment for Premium Package",
    "accountNumber": "1234567890",
    "reference": "REF123456",
    "transactionDateTime": "2024-01-15T10:35:00Z",
    "currency": "VND",
    "paymentLinkId": "abc123def456",
    "code": "00",
    "desc": "success",
    "counterAccountBankId": "",
    "counterAccountBankName": "",
    "counterAccountName": "",
    "counterAccountNumber": "",
    "virtualAccountName": "",
    "virtualAccountNumber": ""
  },
  "signature": "webhook_signature_here"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Webhook processed successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Webhook processing failed
- `500 Internal Server Error` - Payment service unavailable

---

## Billing History API

Base URL: `/api/billing-histories`

### 1. Get Billing History by User ID

Retrieves all billing history records for a specific user.

**Endpoint:** `GET /api/billing-histories/user/{userId}`

**Authentication:** Not specified (recommend adding authorization)

**Path Parameters:**
- `userId` (guid, required) - The user ID to retrieve billing history

**Success Response (200 OK):**
```json
[
  {
    "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
    "packageName": "Premium Package",
    "amount": 99000,
    "currency": "VND",
    "orderCode": 123456789,
    "status": "PAID",
    "paymentMethod": "BANK_TRANSFER",
    "transactionDate": "2024-01-15T10:35:00Z",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:35:00Z"
  },
  {
    "id": "e37bc20a-47bb-3261-a456-0d01a1b2c3d8",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "packageId": "68fbf31b-f3ac-5483-b827-557766551111",
    "packageName": "Basic Package",
    "amount": 49000,
    "currency": "VND",
    "orderCode": 123456788,
    "status": "PAID",
    "paymentMethod": "BANK_TRANSFER",
    "transactionDate": "2024-01-10T14:20:00Z",
    "createdAt": "2024-01-10T14:15:00Z",
    "updatedAt": "2024-01-10T14:20:00Z"
  }
]
```

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/billing-histories/user/550e8400-e29b-41d4-a716-446655440000
```

---

### 2. Get Billing History by ID

Retrieves a specific billing history record by ID.

**Endpoint:** `GET /api/billing-histories/{id}`

**Authentication:** Not specified (recommend adding authorization)

**Path Parameters:**
- `id` (guid, required) - The billing history ID

**Success Response (200 OK):**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
  "packageName": "Premium Package",
  "amount": 99000,
  "currency": "VND",
  "orderCode": 123456789,
  "status": "PAID",
  "paymentMethod": "BANK_TRANSFER",
  "transactionDate": "2024-01-15T10:35:00Z",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:35:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Billing history not found

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/billing-histories/f47ac10b-58cc-4372-a567-0e02b2c3d479
```

---

### 3. Create Billing History

Creates a new billing history record (typically used by webhook, not frontend).

**Endpoint:** `POST /api/billing-histories`

**Authentication:** Not specified (recommend adding authorization)

**Request Body:**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
  "amount": 99000,
  "currency": "VND",
  "orderCode": 123456789,
  "status": "PAID",
  "paymentMethod": "BANK_TRANSFER",
  "transactionDate": "2024-01-15T10:35:00Z"
}
```

**Success Response (201 Created):**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
  "packageName": "Premium Package",
  "amount": 99000,
  "currency": "VND",
  "orderCode": 123456789,
  "status": "PAID",
  "paymentMethod": "BANK_TRANSFER",
  "transactionDate": "2024-01-15T10:35:00Z",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

**Example cURL:**
```bash
curl -X POST https://api.example.com/api/billing-histories \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
    "amount": 99000,
    "currency": "VND",
    "orderCode": 123456789,
    "status": "PAID",
    "paymentMethod": "BANK_TRANSFER",
    "transactionDate": "2024-01-15T10:35:00Z"
  }'
```

---

## Package API

Base URL: `/api/packages`

### 1. Get Active Packages

Retrieves all active packages available for purchase.

**Endpoint:** `GET /api/packages/active`

**Authentication:** Not required

**Success Response (200 OK):**
```json
[
  {
    "id": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
    "name": "Premium",
    "description": "Full access to all features including unlimited CV reviews and advanced AI insights",
    "price": 99000,
    "currency": "VND",
    "durationDays": 30,
    "features": [
      "Unlimited CV reviews",
      "Advanced AI insights",
      "Priority support",
      "Custom templates",
      "Export in multiple formats"
    ],
    "isActive": true,
    "displayOrder": 2,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  {
    "id": "68fbf31b-f3ac-5483-b827-557766551111",
    "name": "Basic",
    "description": "Essential features for job seekers including 5 CV reviews per month",
    "price": 49000,
    "currency": "VND",
    "durationDays": 30,
    "features": [
      "5 CV reviews per month",
      "Basic AI insights",
      "Standard templates",
      "PDF export"
    ],
    "isActive": true,
    "displayOrder": 1,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/packages/active
```

---

### 2. Get Package by ID

Retrieves detailed information about a specific package by ID.

**Endpoint:** `GET /api/packages/{id}`

**Authentication:** Not required

**Path Parameters:**
- `id` (guid, required) - The package ID

**Success Response (200 OK):**
```json
{
  "id": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
  "name": "Premium",
  "description": "Full access to all features including unlimited CV reviews and advanced AI insights",
  "price": 99000,
  "currency": "VND",
  "durationDays": 30,
  "features": [
    "Unlimited CV reviews",
    "Advanced AI insights",
    "Priority support",
    "Custom templates",
    "Export in multiple formats"
  ],
  "isActive": true,
  "displayOrder": 2,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Package not found

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/packages/57e2340a-c5fe-45bc-9ba5-558bfacd7561
```

---

### 3. Get Package by Name

Retrieves detailed information about a specific package by name.

**Endpoint:** `GET /api/packages/by-name/{name}`

**Authentication:** Not required

**Path Parameters:**
- `name` (string, required) - The package name (e.g., "Premium", "Basic")

**Success Response (200 OK):**
```json
{
  "id": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
  "name": "Premium",
  "description": "Full access to all features including unlimited CV reviews and advanced AI insights",
  "price": 99000,
  "currency": "VND",
  "durationDays": 30,
  "features": [
    "Unlimited CV reviews",
    "Advanced AI insights",
    "Priority support",
    "Custom templates",
    "Export in multiple formats"
  ],
  "isActive": true,
  "displayOrder": 2,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Package not found

**Example cURL:**
```bash
curl -X GET https://api.example.com/api/packages/by-name/Premium
```

---

## Common Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "packageId": ["The packageId field is required."]
  }
}
```

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization has been denied for this request."
}
```

### 404 Not Found
```json
{
  "errorCode": "PACKAGE_NOT_FOUND",
  "message": "Package not found",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 500 Internal Server Error
```json
{
  "errorCode": "PAYMENT_SERVICE_ERROR",
  "message": "An error occurred while processing the payment",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## Authentication

Most payment-related endpoints require authentication using Bearer tokens.

### How to Authenticate

Include the JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Getting a Token

Tokens are obtained through the authentication endpoints (not covered in this document). Typically:
1. Login via `/api/auth/login`
2. Receive JWT token in response
3. Use token for subsequent API calls

### Token Expiration

- Access tokens typically expire after 1 hour
- Refresh tokens can be used to obtain new access tokens
- When a token expires, the API returns a 401 Unauthorized response

---

## Frontend Integration Guide

### Payment Flow

1. **Display Packages**
   - Call `GET /api/packages/active` to get available packages
   - Display packages with pricing and features

2. **Initiate Payment**
   - User selects a package
   - Call `POST /api/payment/create-payment` with packageId
   - Receive checkout URL and QR code

3. **Redirect to Payment**
   - Open checkoutUrl in new window or redirect user
   - User completes payment on PayOS platform

4. **Handle Payment Return**
   - PayOS redirects back to your return URL
   - Extract orderCode from URL parameters
   - Call `GET /api/payment/status/{orderCode}` to verify payment

5. **Display Confirmation**
   - Show payment success/failure message
   - Update user's subscription status
   - Display billing history via `GET /api/billing-histories/user/{userId}`

### Example JavaScript Integration

```javascript
// 1. Fetch available packages
async function getPackages() {
  const response = await fetch('https://api.example.com/api/packages/active');
  return await response.json();
}

// 2. Create payment link
async function createPayment(packageId, token) {
  const response = await fetch('https://api.example.com/api/payment/create-payment', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({ packageId })
  });
  return await response.json();
}

// 3. Check payment status
async function checkPaymentStatus(orderCode, token) {
  const response = await fetch(`https://api.example.com/api/payment/status/${orderCode}`, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  return await response.json();
}

// 4. Get user billing history
async function getBillingHistory(userId) {
  const response = await fetch(`https://api.example.com/api/billing-histories/user/${userId}`);
  return await response.json();
}

// Usage example
async function handlePackagePurchase(packageId, userToken) {
  try {
    // Create payment
    const payment = await createPayment(packageId, userToken);

    // Open payment page
    window.open(payment.checkoutUrl, '_blank');

    // Poll for payment status (or use webhook callback)
    const pollInterval = setInterval(async () => {
      const status = await checkPaymentStatus(payment.orderCode, userToken);

      if (status.status === 'PAID') {
        clearInterval(pollInterval);
        alert('Payment successful!');
        // Refresh billing history
        const history = await getBillingHistory(userId);
        console.log('Billing history:', history);
      } else if (status.status === 'CANCELLED' || status.status === 'EXPIRED') {
        clearInterval(pollInterval);
        alert('Payment was not completed.');
      }
    }, 5000); // Check every 5 seconds
  } catch (error) {
    console.error('Payment error:', error);
    alert('An error occurred during payment processing.');
  }
}
```

---

## Notes

1. **Currency**: All amounts are in VND (Vietnamese Dong)
2. **Date Format**: All dates follow ISO 8601 format (e.g., `2024-01-15T10:30:00Z`)
3. **GUID Format**: All IDs use standard GUID format (e.g., `57e2340a-c5fe-45bc-9ba5-558bfacd7561`)
4. **Rate Limiting**: Consider implementing rate limiting on frequent status checks
5. **Webhook Security**: Verify webhook signatures in production
6. **Error Handling**: Always implement proper error handling for network failures and API errors
7. **Testing**: Use sandbox/test environment for development before production deployment

---

## Support

For technical support or questions about API integration, please contact:
- Email: support@perfectcv.com
- Documentation: https://docs.perfectcv.com

---

*Last Updated: January 2025*
