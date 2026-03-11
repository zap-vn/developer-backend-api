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

            var result = JsonSerializer.Serialize(new
            {
                statusCode = statusCode,
                message = title,
                detail = detail
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

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
                "auth_account_inactive" => isVi ? "Tài khoản chưa được kích hoạt." : "Account is not active.",
                "auth_too_many_requests" => isVi ? "Quá nhiều lượt yêu cầu" : "Too many requests",
                "auth_too_many_requests_detail" => isVi ? "Vui lòng thử lại sau vài phút." : "Please try again later.",
                "auth_login_success" => isVi ? "Đăng nhập thành công" : "Login successful",
                "auth_email_not_verified" => isVi ? "Email chưa xác thực" : "Email not verified",
                "auth_email_not_verified_detail" => isVi ? "Email chưa được xác thực. Vui lòng kiểm tra email để lấy mã OTP." : "Email not verified. Please check your email for the OTP code.",
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
                _ => key
            };
        }
    }
}
