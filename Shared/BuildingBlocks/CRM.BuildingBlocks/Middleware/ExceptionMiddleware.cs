using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CRM.BuildingBlocks.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> _localizer;

        public ExceptionMiddleware(RequestDelegate next, Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> localizer)
        {
            _next = next;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, _localizer);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> localizer)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var rawMessage = exception.Message;
            
            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exception is CRM.BuildingBlocks.Exceptions.ValidationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (exception.GetType().Name == "TooManyRequestsException" || exception.Message == "TOO_MANY_REQUESTS")
            {
                statusCode = 429;
                rawMessage = "auth_too_many_requests|auth_too_many_requests_detail";
            }

            // Support pipe-delimited message for multi-part localization (Title|Detail)
            string title;
            string detail;

            if (rawMessage.Contains("|"))
            {
                var parts = rawMessage.Split('|');
                var locTitle = localizer[parts[0]];
                var locDetail = localizer[parts[1]];

                // In some .NET versions, even if not found, it returns the key and ResourceNotFound might be false
                string currentLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                title = (locTitle.ResourceNotFound || locTitle.Value == parts[0]) 
                        ? GetHardcodedFallback(parts[0], currentLang) 
                        : locTitle.Value;
                        
                detail = (locDetail.ResourceNotFound || locDetail.Value == parts[1]) 
                         ? GetHardcodedFallback(parts[1], currentLang) 
                         : locDetail.Value;
            }
            else
            {
                var loc = localizer[rawMessage];
                string currentLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                title = (loc.ResourceNotFound || loc.Value == rawMessage) 
                        ? GetHardcodedFallback(rawMessage, currentLang) 
                        : loc.Value;
                detail = title; 
            }

            context.Response.StatusCode = statusCode;

            var errorCode = rawMessage.Contains("|") ? rawMessage.Split('|')[0] : rawMessage;
            string? redirectUrl = null;

            if (errorCode == "AUTH_001" || errorCode == "auth_email_not_verified" || errorCode == "auth_phone_not_verified")
            {
                // Try to get language from Accept-Language header, default to 'vi'
                string lang = "vi";
                var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
                
                if (!string.IsNullOrEmpty(acceptLanguage))
                {
                    // Usually format is "vi,en-US;q=0.9..." or just "vi-VN"
                    var firstLang = acceptLanguage.Split(',').FirstOrDefault()?.Trim();
                    if (!string.IsNullOrEmpty(firstLang))
                    {
                        // Extract just the two letter code if possible, or use the exact string if it's already short
                        lang = firstLang.Length >= 2 ? firstLang.Substring(0, 2).ToLower() : firstLang.ToLower();
                    }
                }
                
                redirectUrl = $"http://localhost:3000/{lang}/active-account";
            }

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = title,
                Detail = detail,
                RedirectUrl = redirectUrl
            }, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = new CRM.BuildingBlocks.Serialization.ExceptionPascalCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            return context.Response.WriteAsync(result);
        }

        private static string GetHardcodedFallback(string key, string lang)
        {
            // Emergency fallback if .resx files are not loaded correctly
            bool isVi = lang.Equals("vi", StringComparison.OrdinalIgnoreCase);

            return key switch
            {
                "auth_invalid_credentials" => isVi ? "Thông tin đăng nhập không chính xác" : "Invalid credentials",
                "auth_invalid_credentials_detail" => isVi ? "Tên đăng nhập hoặc mật khẩu bạn nhập không đúng." : "The username or password you entered is incorrect.",
                "AUTH_002" => isVi ? "Sai email hoặc password" : "Invalid email or password",
                "AUTH_002_detail" => isVi ? "Tên đăng nhập hoặc mật khẩu bạn nhập không đúng." : "The username or password you entered is incorrect.",
                
                "auth_account_inactive" => isVi ? "Tài khoản chưa được kích hoạt." : "Account is not active.",
                "AUTH_003" => isVi ? "Tài khoản bị khóa" : "Account locked",
                "AUTH_003_detail" => isVi ? "Tài khoản của bạn đã bị khóa hoặc không hoạt động. Vui lòng liên hệ quản trị viên." : "Your account is locked or inactive. Please contact administrator.",
                
                "auth_too_many_requests" => isVi ? "Quá nhiều lượt yêu cầu" : "Too many requests",
                "auth_too_many_requests_detail" => isVi ? "Vui lòng thử lại sau vài phút." : "Please try again later.",
                "AUTH_006" => isVi ? "Login quá nhiều lần" : "Too many login attempts",
                "AUTH_006_detail" => isVi ? "Vui lòng thử lại sau vài phút." : "Please try again later.",
                
                "auth_login_success" => isVi ? "Đăng nhập thành công" : "Login successful",
                
                "auth_email_not_verified" => isVi ? "Email chưa xác thực" : "Email not verified",
                "auth_email_not_verified_detail" => isVi ? "Email chưa được xác thực. Vui lòng kiểm tra email để lấy mã OTP." : "Email not verified. Please check your email for the OTP code.",
                "AUTH_001" => isVi ? "Email chưa xác thực" : "Email not verified",
                "AUTH_001_detail" => isVi ? "Tài khoản của bạn chưa được xác thực. Vui lòng kiểm tra email để lấy mã OTP." : "Your account is not verified. Please check your email for the OTP code.",
                
                "auth_phone_not_verified" => isVi ? "Số điện thoại chưa xác thực" : "Phone number not verified",
                "auth_phone_not_verified_detail" => isVi ? "Số điện thoại chưa được xác thực. Vui lòng gửi lại mã OTP để xác nhận." : "Phone number not verified. Please resend OTP to verify your account.",
                
                "AUTH_004" => isVi ? "OTP sai" : "Invalid OTP",
                "AUTH_004_detail" => isVi ? "Mã xác thực bạn nhập không chính xác." : "The authentication code you entered is incorrect.",
                
                "AUTH_005" => isVi ? "OTP hết hạn" : "OTP expired",
                "AUTH_005_detail" => isVi ? "Mã xác thực đã hết hạn. Vui lòng yêu cầu mã mới." : "The authentication code has expired. Please request a new one.",

                "error_duplicate_merchant_name" => isVi ? "Trùng tên Merchant" : "Duplicate Merchant Name",
                "error_duplicate_merchant_name_detail" => isVi ? "Dữ liệu trùng lặp: Merchant Name đã tồn tại." : "Duplicate data: Merchant Name already exists.",
                "error_duplicate_email" => isVi ? "Trùng Email" : "Duplicate Email",
                "error_duplicate_email_detail" => isVi ? "Dữ liệu trùng lặp: Email đã tồn tại." : "Duplicate data: Email already exists.",
                "error_duplicate_phone" => isVi ? "Trùng số điện thoại" : "Duplicate Phone",
                "error_duplicate_phone_detail" => isVi ? "Dữ liệu trùng lặp: Số điện thoại đã tồn tại." : "Duplicate data: Phone number already exists.",
                "error_invalid_phone" => isVi ? "Số điện thoại không hợp lệ" : "Invalid Phone Number",
                "error_invalid_phone_detail" => isVi ? "Số điện thoại phải có định dạng từ 10-11 chữ số." : "Phone number must be between 10 and 11 digits.",
                "error_missing_contact" => isVi ? "Thiếu thông tin liên hệ" : "Missing Contact Information",
                "error_missing_contact_detail" => isVi ? "Bạn phải cung cấp ít nhất Email hoặc Số điện thoại để đăng ký." : "You must provide at least Email or Phone number to register.",
                
                "error_invalid_otp" => isVi ? "Mã OTP không hợp lệ" : "Invalid OTP",
                "error_otp_expired" => isVi ? "Mã OTP hết hạn" : "OTP expired",
                
                _ => key
            };
        }
    }
}
