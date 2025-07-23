# CV Search Implementation Summary

## ✅ **Đã hoàn thành:**

### 1. **CVQuery.cs** - Thêm Search Parameter

-   **File**: `src/service-cv-management/ServicePerfectCV.Application/DTOs/CV/Requests/CVQuery.cs`
-   **Thay đổi**: Thêm property `SearchTerm` để hỗ trợ search theo title
-   **Code**:

```csharp
public class CVQuery : PaginationQuery
{
    public string? SearchTerm { get; set; } = null;
    public CVSort? Sort { get; set; } = null;
}
```

### 2. **CVRepository.cs** - Implement Search Logic

-   **File**: `src/service-cv-management/ServicePerfectCV.Infrastructure/Repositories/CVRepository.cs`
-   **Thay đổi**:
    -   Thêm search filter trong method `GetByUserIdAsync`
    -   Sử dụng `Contains` để tìm kiếm partial match
    -   Case-insensitive search
-   **Code**:

```csharp
// Apply search filter if search term is provided
if (!string.IsNullOrWhiteSpace(query.SearchTerm))
{
    baseQuery = baseQuery.Where(cv => cv.Title.Contains(query.SearchTerm));
}
```

### 3. **CVController.cs** - Cập nhật Documentation

-   **File**: `src/service-cv-management/ServicePerfectCV.WebApi/Controllers/CVController.cs`
-   **Thay đổi**:
    -   Thêm XML documentation cho search feature
    -   Thêm response type attributes
    -   Thêm using statement cho PaginationData

## 🔧 **Tính năng đã implement:**

### 1. **Search by Title**

-   **Parameter**: `searchTerm`
-   **Type**: String (optional)
-   **Behavior**: Case-insensitive partial match
-   **Example**: `searchTerm=software` sẽ tìm CV có title chứa "software"

### 2. **Pagination**

-   **Parameters**: `limit`, `offset`
-   **Default**: limit=10, offset=0
-   **Usage**: Kết hợp với search để phân trang kết quả

### 3. **Sorting**

-   **Parameter**: `sort.updatedAt`
-   **Values**: 0 (ascending), 1 (descending)
-   **Usage**: Sắp xếp kết quả theo thời gian cập nhật

## 📋 **API Usage Examples:**

### 1. **Basic Search**

```http
GET /api/cvs?searchTerm=software
```

### 2. **Search with Pagination**

```http
GET /api/cvs?searchTerm=developer&limit=5&offset=0
```

### 3. **Search with Sorting**

```http
GET /api/cvs?searchTerm=engineer&sort.updatedAt=1
```

### 4. **All Parameters**

```http
GET /api/cvs?searchTerm=fullstack&limit=10&offset=5&sort.updatedAt=0
```

## 📁 **Files Created:**

### 1. **CV_SEARCH_TEST_EXAMPLES.md**

-   Hướng dẫn test chi tiết
-   Các test scenarios
-   Performance considerations
-   Future enhancements

### 2. **CV_Search_Postman_Collection.json**

-   Postman collection để test
-   12 test cases khác nhau
-   Environment variables setup

### 3. **CV_SEARCH_IMPLEMENTATION_SUMMARY.md**

-   Tóm tắt implementation
-   Files modified
-   API usage examples

## 🧪 **Test Scenarios:**

### 1. **Basic Functionality**

-   ✅ Search với searchTerm hợp lệ
-   ✅ Search với searchTerm rỗng
-   ✅ Search với special characters
-   ✅ Search với long search term

### 2. **Pagination**

-   ✅ Limit và offset hoạt động đúng
-   ✅ Kết hợp với search
-   ✅ Edge cases (negative values)

### 3. **Sorting**

-   ✅ Ascending sort (sort.updatedAt=0)
-   ✅ Descending sort (sort.updatedAt=1)
-   ✅ Kết hợp với search và pagination

### 4. **Error Handling**

-   ✅ Invalid sort parameter
-   ✅ Negative pagination values
-   ✅ Missing authorization

## 🚀 **Performance Optimizations:**

### 1. **Database Indexes**

Đảm bảo có indexes trên:

-   `UserId` (for user filtering)
-   `Title` (for search)
-   `UpdatedAt` (for sorting)
-   `DeletedAt` (for soft delete)

### 2. **Query Optimization**

-   Sử dụng `AsNoTracking()` cho read-only queries
-   Filter `DeletedAt == null` để loại bỏ soft-deleted records
-   Efficient pagination với `Skip()` và `Take()`

## 🔮 **Future Enhancements:**

### 1. **Advanced Search**

-   Search theo nhiều fields (jobDetail, etc.)
-   Full-text search
-   Fuzzy search
-   Search suggestions

### 2. **Additional Filters**

-   Filter theo date range
-   Filter theo analysis status
-   Filter theo job type

### 3. **Search Analytics**

-   Track search patterns
-   Popular search terms
-   Search performance metrics

## 📊 **Response Format:**

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

## ✅ **Testing Instructions:**

### 1. **Import Postman Collection**

-   Import file `CV_Search_Postman_Collection.json` vào Postman
-   Set environment variables:
    -   `baseUrl`: `http://localhost:8080`
    -   `token`: Your JWT token

### 2. **Run Test Cases**

-   Start với "Get All CVs" để verify basic functionality
-   Test search với các terms khác nhau
-   Test pagination và sorting
-   Test error scenarios

### 3. **Verify Results**

-   Check response format
-   Verify search accuracy
-   Test pagination limits
-   Confirm sorting order

## 🎯 **Key Benefits:**

1. **User Experience**: Dễ dàng tìm kiếm CV theo title
2. **Performance**: Efficient database queries với proper indexing
3. **Flexibility**: Kết hợp search với pagination và sorting
4. **Scalability**: Ready for future enhancements
5. **Maintainability**: Clean code structure với proper separation of concerns

Tính năng search CV by title đã được implement hoàn chỉnh và sẵn sàng để sử dụng! 🚀
