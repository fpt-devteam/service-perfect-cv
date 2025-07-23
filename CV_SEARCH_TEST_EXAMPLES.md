# CV Search Feature Test Guide

## Overview

Tính năng search CV by title đã được implement trong endpoint `GET /api/cvs` với các tính năng:

-   Search theo title (case-insensitive partial match)
-   Pagination
-   Sorting theo updatedAt

## API Endpoint

```
GET /api/cvs
```

## Query Parameters

### 1. **Search Parameters**

-   `searchTerm` (optional): Tìm kiếm CV theo title
    -   Case-insensitive
    -   Partial match (contains)
    -   Ví dụ: `searchTerm=software` sẽ tìm tất cả CV có title chứa "software"

### 2. **Pagination Parameters**

-   `limit` (optional, default: 10): Số lượng CV trả về
-   `offset` (optional, default: 0): Số lượng CV bỏ qua

### 3. **Sorting Parameters**

-   `sort.updatedAt` (optional): Sắp xếp theo updatedAt
    -   `0`: Ascending (cũ nhất trước)
    -   `1`: Descending (mới nhất trước)

## Test Examples

### 1. **Get All CVs (No Search)**

```http
GET http://localhost:8080/api/cvs
Authorization: Bearer YOUR_JWT_TOKEN
```

### 2. **Search CVs by Title**

```http
GET http://localhost:8080/api/cvs?searchTerm=software
Authorization: Bearer YOUR_JWT_TOKEN
```

### 3. **Search with Pagination**

```http
GET http://localhost:8080/api/cvs?searchTerm=developer&limit=5&offset=0
Authorization: Bearer YOUR_JWT_TOKEN
```

### 4. **Search with Sorting (Newest First)**

```http
GET http://localhost:8080/api/cvs?searchTerm=engineer&sort.updatedAt=1
Authorization: Bearer YOUR_JWT_TOKEN
```

### 5. **Search with All Parameters**

```http
GET http://localhost:8080/api/cvs?searchTerm=fullstack&limit=10&offset=5&sort.updatedAt=0
Authorization: Bearer YOUR_JWT_TOKEN
```

## Expected Response Format

```json
{
    "total": 25,
    "items": [
        {
            "id": "123e4567-e89b-12d3-a456-426614174000",
            "title": "Software Engineer CV",
            "jobDetail": {
                "jobTitle": "Software Engineer",
                "company": "Tech Corp",
                "location": "San Francisco"
            },
            "analysisId": "456e7890-e89b-12d3-a456-426614174001",
            "userId": "789e0123-e89b-12d3-a456-426614174002",
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-20T14:45:00Z"
        }
    ]
}
```

## Test Scenarios

### 1. **Basic Search**

-   Tạo CV với title "Software Engineer"
-   Tạo CV với title "Data Scientist"
-   Search với `searchTerm=software`
-   Expected: Chỉ trả về CV "Software Engineer"

### 2. **Case Insensitive Search**

-   Search với `searchTerm=SOFTWARE`
-   Expected: Trả về CV có title chứa "software" (không phân biệt hoa thường)

### 3. **Partial Match Search**

-   Search với `searchTerm=eng`
-   Expected: Trả về CV có title chứa "eng" (như "Software Engineer", "Data Engineer")

### 4. **Empty Search Term**

-   Search với `searchTerm=` hoặc không có searchTerm
-   Expected: Trả về tất cả CV của user

### 5. **Pagination Test**

-   Tạo nhiều CV (ví dụ: 15 CV)
-   Search với `limit=5&offset=0`
-   Expected: Trả về 5 CV đầu tiên
-   Search với `limit=5&offset=5`
-   Expected: Trả về 5 CV tiếp theo

### 6. **Sorting Test**

-   Tạo CV với updatedAt khác nhau
-   Search với `sort.updatedAt=1`
-   Expected: CV mới nhất trước
-   Search với `sort.updatedAt=0`
-   Expected: CV cũ nhất trước

## Postman Collection

### 1. **Get All CVs**

```
GET {{baseUrl}}/api/cvs
Authorization: Bearer {{token}}
```

### 2. **Search CVs**

```
GET {{baseUrl}}/api/cvs?searchTerm={{searchTerm}}
Authorization: Bearer {{token}}
```

### 3. **Search with Pagination**

```
GET {{baseUrl}}/api/cvs?searchTerm={{searchTerm}}&limit={{limit}}&offset={{offset}}
Authorization: Bearer {{token}}
```

### 4. **Search with Sorting**

```
GET {{baseUrl}}/api/cvs?searchTerm={{searchTerm}}&sort.updatedAt={{sortOrder}}
Authorization: Bearer {{token}}
```

## Environment Variables for Postman

```json
{
    "baseUrl": "http://localhost:8080",
    "token": "YOUR_JWT_TOKEN",
    "searchTerm": "software",
    "limit": "10",
    "offset": "0",
    "sortOrder": "1"
}
```

## Error Scenarios

### 1. **Invalid Token**

```http
GET http://localhost:8080/api/cvs?searchTerm=test
```

Expected: 401 Unauthorized

### 2. **Invalid Sort Parameter**

```http
GET http://localhost:8080/api/cvs?sort.updatedAt=invalid
```

Expected: 400 Bad Request

### 3. **Negative Pagination**

```http
GET http://localhost:8080/api/cvs?limit=-1&offset=-1
```

Expected: 400 Bad Request

## Performance Considerations

### 1. **Database Index**

Đảm bảo có index trên:

-   `UserId` (for filtering by user)
-   `Title` (for search)
-   `UpdatedAt` (for sorting)
-   `DeletedAt` (for soft delete filtering)

### 2. **Search Optimization**

-   Sử dụng `Contains` thay vì `StartsWith` để tìm kiếm linh hoạt hơn
-   Có thể thêm full-text search trong tương lai
-   Có thể thêm search theo nhiều field khác

### 3. **Caching**

-   Có thể cache kết quả search cho user
-   Cache theo search term và pagination parameters
-   Invalidate cache khi CV được update

## Future Enhancements

### 1. **Advanced Search**

-   Search theo nhiều field (title, jobDetail, etc.)
-   Full-text search
-   Fuzzy search (tìm kiếm gần đúng)

### 2. **Search Filters**

-   Filter theo date range
-   Filter theo analysis status
-   Filter theo job type

### 3. **Search Suggestions**

-   Auto-complete cho search terms
-   Popular search terms
-   Search history

### 4. **Search Analytics**

-   Track search patterns
-   Most searched terms
-   Search performance metrics
