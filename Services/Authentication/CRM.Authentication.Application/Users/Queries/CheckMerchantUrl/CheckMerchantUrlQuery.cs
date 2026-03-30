using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Domain.Interfaces;

namespace CRM.Authentication.Application.Users.Queries.CheckMerchantUrl
{
    public class CheckMerchantUrlQuery : IRequest<bool>
    {
        [JsonPropertyName("merchant_url")]
        public string MerchantUrl { get; set; } = string.Empty;
    }

    public class CheckMerchantUrlQueryHandler : IRequestHandler<CheckMerchantUrlQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public CheckMerchantUrlQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(CheckMerchantUrlQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MerchantUrl))
            {
                return false;
            }

            return !await _userRepository.MerchantUrlExistsAsync(request.MerchantUrl);
        }
    }
}
