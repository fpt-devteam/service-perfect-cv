# FCM Debug Guide - Lỗi Forbidden

## Các bước debug lỗi "Forbidden" trong FCM

### 1. **Kiểm tra Service Account Key**

**Bước 1:** Đảm bảo file service account key tồn tại

```bash
# Kiểm tra file có tồn tại không
ls -la /Users/thangnhattran/FptDevteam/service-perfect-cv/firebase-service-account.json
```

**Bước 2:** Test endpoint cấu hình

```http
GET http://localhost:8080/api/push-notifications/test-config
```

### 2. **Kiểm tra Service Account Permissions**

**Bước 1:** Vào Firebase Console

-   Project Settings → Service Accounts
-   Đảm bảo service account có quyền "Firebase Admin SDK"

**Bước 2:** Kiểm tra IAM permissions trong Google Cloud Console

-   Vào Google Cloud Console → IAM & Admin → IAM
-   Tìm service account email (thường có dạng: `firebase-adminsdk-xxxxx@project-id.iam.gserviceaccount.com`)
-   Đảm bảo có các roles sau:
    -   `Firebase Admin`
    -   `Cloud Messaging Admin`
    -   `Service Account Token Creator`

### 3. **Kiểm tra Project ID**

**Bước 1:** Lấy Project ID chính xác

-   Vào Firebase Console → Project Settings → General
-   Copy "Project ID" (không phải Project name)

**Bước 2:** Cập nhật appsettings.Development.json

```json
{
    "FcmSettings": {
        "ServiceAccountKeyPath": "/Users/thangnhattran/FptDevteam/service-perfect-cv/firebase-service-account.json",
        "ProjectId": "YOUR_ACTUAL_PROJECT_ID"
    }
}
```

### 4. **Test Access Token Generation**

**Bước 1:** Test endpoint lấy access token

```http
GET http://localhost:8080/api/push-notifications/test-token
```

**Bước 2:** Kiểm tra logs trong console để xem chi tiết lỗi

### 5. **Kiểm tra Service Account Key Content**

**Bước 1:** Mở file service account key

```bash
cat /Users/thangnhattran/FptDevteam/service-perfect-cv/firebase-service-account.json
```

**Bước 2:** Đảm bảo file có cấu trúc đúng:

```json
{
    "type": "service_account",
    "project_id": "your-project-id",
    "private_key_id": "...",
    "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
    "client_email": "firebase-adminsdk-xxxxx@your-project-id.iam.gserviceaccount.com",
    "client_id": "...",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-xxxxx%40your-project-id.iam.gserviceaccount.com"
}
```

### 6. **Các lỗi thường gặp và cách khắc phục**

#### Lỗi 1: "Service account key file not found"

**Nguyên nhân:** Đường dẫn file không đúng
**Giải pháp:** Cập nhật đường dẫn trong appsettings.Development.json

#### Lỗi 2: "Invalid project ID"

**Nguyên nhân:** Project ID không đúng
**Giải pháp:** Kiểm tra và cập nhật Project ID chính xác

#### Lỗi 3: "Permission denied"

**Nguyên nhân:** Service account không có quyền
**Giải pháp:** Thêm roles cần thiết trong Google Cloud Console

#### Lỗi 4: "Invalid device token"

**Nguyên nhân:** Device token không hợp lệ hoặc đã hết hạn
**Giải pháp:** Lấy device token mới từ ứng dụng

### 7. **Test với Device Token thật**

**Bước 1:** Lấy device token từ ứng dụng mobile/web
**Bước 2:** Test với Postman:

```http
POST http://localhost:8080/api/push-notifications/send
Content-Type: application/json

{
  "deviceTokens": ["YOUR_REAL_DEVICE_TOKEN"],
  "title": "Test Notification",
  "message": "This is a test message"
}
```

### 8. **Kiểm tra logs chi tiết**

Khi chạy ứng dụng, theo dõi logs để xem:

-   Service account key có được load thành công không
-   Access token có được tạo thành công không
-   FCM request có được gửi thành công không
-   Response từ FCM có lỗi gì không

### 9. **Troubleshooting Commands**

```bash
# Kiểm tra file permissions
ls -la firebase-service-account.json

# Kiểm tra file content (chỉ xem 10 dòng đầu)
head -10 firebase-service-account.json

# Test JSON syntax
cat firebase-service-account.json | jq .

# Kiểm tra project ID trong file
cat firebase-service-account.json | jq -r '.project_id'
```

### 10. **Fallback Testing**

Nếu vẫn gặp lỗi, thử test trực tiếp với curl:

```bash
# Lấy access token
curl -X POST https://oauth2.googleapis.com/token \
  -d "grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer" \
  -d "assertion=YOUR_JWT_TOKEN" \
  -H "Content-Type: application/x-www-form-urlencoded"

# Gửi FCM message
curl -X POST https://fcm.googleapis.com/v1/projects/YOUR_PROJECT_ID/messages:send \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "message": {
      "token": "DEVICE_TOKEN",
      "notification": {
        "title": "Test",
        "body": "Test message"
      }
    }
  }'
```
