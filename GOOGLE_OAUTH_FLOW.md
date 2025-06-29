# 🔐 Google OAuth Flow Documentation

## 📋 Overview

Hệ thống Google OAuth được thiết kế với 2 luồng chính:

1. **Authorization Code Flow** - Luồng chính thức OAuth2
2. **Direct Exchange** - Cho việc test và integration trực tiếp

## 🚀 Luồng hoạt động chính (Authorization Code Flow)

### Bước 1: Lấy Google Login Link

```http
GET /api/auth/google-login-link?returnUrl=http://localhost:3000/callback
```

**Response:**

```
https://accounts.google.com/o/oauth2/v2/auth?client_id=...&redirect_uri=...&response_type=code&scope=openid%20profile%20email&state=?returnUrl=http://localhost:3000/callback
```

### Bước 2: User Authentication

- Frontend redirect user đến link nhận được
- User đăng nhập với Google
- Google redirect về callback URL với authorization code

### Bước 3: Callback Processing

Google sẽ redirect về:

```
GET /api/auth/google-callback?code=4/0AX4XfWh...&state=?returnUrl=http://localhost:3000/callback
```

Backend sẽ:

1. Nhận authorization code
2. Exchange code với Google để lấy access token
3. Lấy thông tin user từ Google API
4. Tạo hoặc đăng nhập user trong hệ thống
5. Tạo JWT token
6. Redirect về frontend với token

## 🧪 Luồng test (Direct Exchange)

Để test trực tiếp without browser redirect:

```http
POST /api/auth/exchange-code
Content-Type: application/json

{
  "code": "4/0AX4XfWh...",
  "state": "optional_state"
}
```

## 📁 File Structure

### DTOs

- `OauthExchangeCodeRequest.cs` - Request DTO cho exchange code
- `GoogleTokenResponse.cs` - Response từ Google token endpoint
- `GoogleUserInfo.cs` - User info từ Google API
- `GoogleLoginRequest.cs` - Request cho Google login
- `LoginResponse.cs` - Response chứa JWT tokens

### Services

- `AuthService.ExchangeCodeAsync()` - Exchange code với Google
- `AuthService.GoogleLoginAsync()` - Xử lý Google login logic

### Controllers

- `GET /api/auth/google-login-link` - Lấy Google OAuth URL
- `GET /api/auth/google-callback` - Callback từ Google
- `POST /api/auth/exchange-code` - Exchange code manually

## ⚙️ Configuration

Cần cấu hình trong `appsettings.json`:

```json
{
  "GoogleSettings": {
    "ClientId": "your_google_client_id",
    "ClientSecret": "your_google_client_secret",
    "RedirectUri": "https://localhost:7001/api/auth/google-callback",
    "Scopes": "openid profile email"
  }
}
```

## 🎯 Google Cloud Console Setup

1. Tạo project tại [Google Cloud Console](https://console.cloud.google.com)
2. Enable Google+ API
3. Tạo OAuth 2.0 credentials
4. Thêm Authorized redirect URIs:
   - `https://localhost:7001/api/auth/google-callback`
   - `http://localhost:7001/api/auth/google-callback` (for development)

## 🔍 Testing

### 1. Manual Test với Browser

1. Call `GET /api/auth/google-login-link`
2. Mở link trong browser
3. Đăng nhập với Google
4. Kiểm tra callback response

### 2. API Test với HTTP Client

1. Lấy authorization code từ browser flow
2. Call `POST /api/auth/exchange-code` với code
3. Kiểm tra response tokens

### 3. Integration Test

```csharp
[Fact]
public async Task ExchangeCode_WithValidCode_ReturnsTokens()
{
    // Test implementation
}
```

## 🚨 Security Notes

1. **HTTPS Required**: Google OAuth yêu cầu HTTPS trong production
2. **State Parameter**: Sử dụng để prevent CSRF attacks
3. **Token Expiry**: Access tokens có thời hạn ngắn
4. **Refresh Tokens**: Sử dụng để renew access tokens

## 🐛 Troubleshooting

### Common Errors:

1. **Invalid redirect URI**: Kiểm tra configuration trong Google Console
2. **Invalid client**: Kiểm tra ClientId/ClientSecret
3. **Code expired**: Authorization code chỉ valid trong vài phút
4. **SSL errors**: Sử dụng valid SSL certificate

### Debug Steps:

1. Check appsettings.json configuration
2. Verify Google Console setup
3. Test with PostMan/HTTP Client
4. Check server logs for detailed errors
