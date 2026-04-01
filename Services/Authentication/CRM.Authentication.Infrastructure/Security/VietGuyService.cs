using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Common.Models;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;

namespace CRM.Authentication.Infrastructure.Security
{
    public class VietGuyService : IVietGuyService
    {
        private readonly ILogger<VietGuyService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ISystemConfigRepository _configRepository;

        public VietGuyService(
            ILogger<VietGuyService> logger,
            HttpClient httpClient,
            ISystemConfigRepository configRepository)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configRepository = configRepository;
        }

        private string GetAccessTokenKey(string accountName) => $"vietguy_access_token_{accountName}";
        private string GetRefreshTokenKey(string accountName) => $"vietguy_refresh_token_{accountName}";
        private string GetExpiredAtKey(string accountName) => $"vietguy_expired_at_{accountName}";

        public async Task<string> GetAccessTokenAsync(EmailSetting setting)
        {
            var accountName = setting.account_name ?? "default";
            var expiredAtConfig = await _configRepository.GetByKeyAsync(GetExpiredAtKey(accountName));
            long expiredAt = 0;
            if (expiredAtConfig != null && long.TryParse(expiredAtConfig.value, out long val))
            {
                expiredAt = val;
            }

            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            // If expired or about to expire in 1 minute
            if (currentTime >= (expiredAt - 60000))
            {
                await RefreshAccessTokenAsync(setting);
            }

            var accessTokenConfig = await _configRepository.GetByKeyAsync(GetAccessTokenKey(accountName));
            return accessTokenConfig?.value ?? string.Empty;
        }

        public async Task RefreshAccessTokenAsync(EmailSetting setting)
        {
            var accountName = setting.account_name ?? string.Empty;
            _logger.LogInformation($"[VIETGUY] Refreshing access token for {accountName}...");

            var refreshTokenConfig = await _configRepository.GetByKeyAsync(GetRefreshTokenKey(accountName));
            // In a real multi-account scenario, initial refresh token would be somewhere. 
            // In the DB image, it wasn't shown. We'll use the one from config as fallback if we had it, but user wants ONLY DB.
            var refreshToken = refreshTokenConfig?.value ?? string.Empty;

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError($"[VIETGUY] No refresh token found for {accountName}. Cannot refresh.");
                return;
            }

            // Using the refresh URL from standard Vietguy V2 or custom if needed. 
            // Here we'll use a standard one or assume it's in a specific env config.
            var refreshUrl = "https://api-v2.vietguys.biz:4438/token/v1/refresh";

            var payload = new
            {
                type = "refresh_token",
                username = accountName
            };

            var request = new HttpRequestMessage(HttpMethod.Post, refreshUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshToken);
            request.Content = JsonContent.Create(payload);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<VietGuyAuthResponse>(content);
                    if (result != null && result.error == 0 && result.data != null)
                    {
                        await _configRepository.UpsertAsync(new SystemConfig { key = GetAccessTokenKey(accountName), value = result.data.access_token });
                        await _configRepository.UpsertAsync(new SystemConfig { key = GetRefreshTokenKey(accountName), value = result.data.refresh_token });
                        await _configRepository.UpsertAsync(new SystemConfig { key = GetExpiredAtKey(accountName), value = result.data.expired_at.ToString() });
                        
                        _logger.LogInformation($"[VIETGUY] Token refreshed for {accountName} successfully.");
                    }
                }
                else
                {
                    _logger.LogError($"[VIETGUY] Token refresh error for {accountName}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[VIETGUY] Exception during token refresh for {accountName}");
            }
        }

        public async Task SendSmsAsync(string phone, string message, EmailSetting? setting)
        {
            if (setting == null)
            {
                _logger.LogError("[VIETGUY] Settings missing. Cannot send SMS.");
                return;
            }

            var accessToken = await GetAccessTokenAsync(setting);
            if (string.IsNullOrEmpty(accessToken))
            {
                 accessToken = setting.passcode ?? string.Empty;
            }

            // Normalize phone
            string normalizedPhone = phone.Trim();
            if (normalizedPhone.StartsWith("0")) normalizedPhone = "84" + normalizedPhone.Substring(1);
            else if (!normalizedPhone.StartsWith("84") && !normalizedPhone.StartsWith("+84")) normalizedPhone = "84" + normalizedPhone;
            normalizedPhone = normalizedPhone.Replace("+", "");

            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["u"] = setting.account_name;
                query["pwd"] = accessToken;
                query["from"] = setting.send_id;
                query["phone"] = normalizedPhone;
                query["sms"] = message;
                query["bid"] = Guid.NewGuid().ToString("N");

                var url = $"{setting.base_url_sms ?? "https://cloudsms.vietguys.biz:4438"}{setting.api_endpoint ?? "/api/index.php"}?{query}";
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"[VIETGUY] SMS sent to {normalizedPhone}. Account: {setting.account_name}. Response: {content}");
                }
                else
                {
                    _logger.LogError($"[VIETGUY] SMS send failed for {normalizedPhone}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[VIETGUY] Exception sending SMS to {phone}");
            }
        }

        private class VietGuyAuthResponse
        {
            public int error { get; set; }
            public string message { get; set; } = string.Empty;
            public VietGuyAuthData? data { get; set; }
        }

        private class VietGuyAuthData
        {
            public string access_token { get; set; } = string.Empty;
            public string refresh_token { get; set; } = string.Empty;
            public long expired_at { get; set; }
        }
    }
}
