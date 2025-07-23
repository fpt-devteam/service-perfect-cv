# CV Delete Feature Test Guide

## Overview

Tính năng delete CV đã được implement với soft delete (chỉ update `DeletedAt` thành `DateTime.UtcNow`). CV đã delete sẽ không xuất hiện trong các query khác.

## API Endpoint

```
DELETE /api/cvs/{id}
```

## Authentication

-   **Required**: JWT Bearer Token
-   **Authorization**: User chỉ có thể delete CV của chính mình

## Request Format

### URL Parameters

-   `id` (required): GUID của CV cần delete

### Headers

```
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

## Response Format

### Success Response (200 OK)

```json
{
    "message": "CV deleted successfully"
}
```

### Error Responses

#### 401 Unauthorized

```json
{
    "message": "Unauthorized"
}
```

#### 404 Not Found

```json
{
    "message": "CV not found"
}
```

## Test Examples

### 1. **Delete CV Successfully**

```http
DELETE http://localhost:8080/api/cvs/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer YOUR_JWT_TOKEN
```

### 2. **Delete Non-existent CV**

```http
DELETE http://localhost:8080/api/cvs/00000000-0000-0000-0000-000000000000
Authorization: Bearer YOUR_JWT_TOKEN
```

### 3. **Delete CV Without Authorization**

```http
DELETE http://localhost:8080/api/cvs/123e4567-e89b-12d3-a456-426614174000
```

### 4. **Delete CV of Another User**

```http
DELETE http://localhost:8080/api/cvs/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer ANOTHER_USER_JWT_TOKEN
```

## Test Scenarios

### 1. **Basic Delete Test**

**Steps:**

1. Tạo CV mới
2. Verify CV xuất hiện trong list
3. Delete CV
4. Verify CV không còn xuất hiện trong list
5. Verify response message

**Expected Results:**

-   CV được delete thành công
-   Response: `{"message": "CV deleted successfully"}`
-   CV không còn xuất hiện trong GET /api/cvs

### 2. **Delete Already Deleted CV**

**Steps:**

1. Delete CV lần đầu
2. Try delete cùng CV lần nữa

**Expected Results:**

-   Lần đầu: Success
-   Lần hai: 404 Not Found

### 3. **Delete Non-existent CV**

**Steps:**

1. Try delete với GUID không tồn tại

**Expected Results:**

-   404 Not Found

### 4. **Authorization Test**

**Steps:**

1. Login với user A
2. Tạo CV với user A
3. Login với user B
4. Try delete CV của user A

**Expected Results:**

-   404 Not Found (vì user B không thấy CV của user A)

### 5. **Database Verification**

**Steps:**

1. Delete CV
2. Check database trực tiếp

**Expected Results:**

-   `DeletedAt` field được set thành current timestamp
-   Record vẫn tồn tại trong database
-   Các field khác không thay đổi

## Postman Collection

### 1. **Delete CV**

```
DELETE {{baseUrl}}/api/cvs/{{cvId}}
Authorization: Bearer {{token}}
```

### 2. **Delete Non-existent CV**

```
DELETE {{baseUrl}}/api/cvs/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{token}}
```

### 3. **Delete Without Auth**

```
DELETE {{baseUrl}}/api/cvs/{{cvId}}
```

## Environment Variables for Postman

```json
{
    "baseUrl": "http://localhost:8080",
    "token": "YOUR_JWT_TOKEN",
    "cvId": "123e4567-e89b-12d3-a456-426614174000"
}
```

## Database Verification

### 1. **Check DeletedAt Field**

```sql
SELECT Id, Title, DeletedAt
FROM CVs
WHERE Id = '123e4567-e89b-12d3-a456-426614174000'
```

### 2. **Verify CV Not in Queries**

```sql
SELECT * FROM CVs
WHERE UserId = 'your-user-id'
AND DeletedAt IS NULL
```

### 3. **Check All Deleted CVs**

```sql
SELECT Id, Title, DeletedAt
FROM CVs
WHERE DeletedAt IS NOT NULL
```

## Integration Tests

### 1. **Delete and List Test**

```http
# 1. Get all CVs
GET {{baseUrl}}/api/cvs
Authorization: Bearer {{token}}

# 2. Delete a CV
DELETE {{baseUrl}}/api/cvs/{{cvId}}
Authorization: Bearer {{token}}

# 3. Get all CVs again
GET {{baseUrl}}/api/cvs
Authorization: Bearer {{token}}
```

### 2. **Delete and Search Test**

```http
# 1. Search CVs
GET {{baseUrl}}/api/cvs?searchTerm=test
Authorization: Bearer {{token}}

# 2. Delete a CV
DELETE {{baseUrl}}/api/cvs/{{cvId}}
Authorization: Bearer {{token}}

# 3. Search CVs again
GET {{baseUrl}}/api/cvs?searchTerm=test
Authorization: Bearer {{token}}
```

## Error Scenarios

### 1. **Invalid GUID Format**

```http
DELETE http://localhost:8080/api/cvs/invalid-guid
Authorization: Bearer YOUR_JWT_TOKEN
```

Expected: 400 Bad Request

### 2. **Missing Authorization**

```http
DELETE http://localhost:8080/api/cvs/123e4567-e89b-12d3-a456-426614174000
```

Expected: 401 Unauthorized

### 3. **Invalid Token**

```http
DELETE http://localhost:8080/api/cvs/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer invalid-token
```

Expected: 401 Unauthorized

## Security Considerations

### 1. **User Authorization**

-   User chỉ có thể delete CV của chính mình
-   Không thể delete CV của user khác

### 2. **Soft Delete Benefits**

-   Data không bị mất hoàn toàn
-   Có thể recover nếu cần
-   Audit trail được bảo toàn

### 3. **Cascade Effects**

-   Related data (Education, Experience, etc.) vẫn tồn tại
-   Chỉ CV chính bị soft delete

## Performance Considerations

### 1. **Database Indexes**

Đảm bảo có indexes trên:

-   `Id` (primary key)
-   `UserId` (for filtering)
-   `DeletedAt` (for soft delete filtering)

### 2. **Query Optimization**

-   Soft delete queries filter by `DeletedAt IS NULL`
-   Efficient user authorization check

## Future Enhancements

### 1. **Hard Delete Option**

-   Thêm parameter để hard delete
-   Cascade delete related data

### 2. **Bulk Delete**

-   Delete multiple CVs at once
-   Batch processing

### 3. **Delete Confirmation**

-   Require confirmation for delete
-   Show CV details before delete

### 4. **Recovery Feature**

-   Restore deleted CVs
-   Undo delete operation

### 5. **Delete History**

-   Track delete operations
-   Audit trail

## Testing Checklist

-   [ ] Delete CV successfully
-   [ ] Verify CV disappears from list
-   [ ] Verify CV disappears from search
-   [ ] Try delete non-existent CV
-   [ ] Try delete without authorization
-   [ ] Try delete CV of another user
-   [ ] Try delete already deleted CV
-   [ ] Verify database state after delete
-   [ ] Test with invalid GUID format
-   [ ] Test with invalid token

## Troubleshooting

### 1. **CV Still Appears After Delete**

-   Check if `DeletedAt` was set correctly
-   Verify queries filter by `DeletedAt IS NULL`
-   Check database indexes

### 2. **404 Error for Valid CV**

-   Verify user owns the CV
-   Check if CV was already deleted
-   Verify GUID format

### 3. **401 Unauthorized**

-   Check JWT token validity
-   Verify token format
-   Check token expiration

### 4. **Database Issues**

-   Check database connection
-   Verify table structure
-   Check foreign key constraints
