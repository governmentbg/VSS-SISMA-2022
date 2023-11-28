using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SISMA.Api.Authentication
{
    /// <summary>
    /// Автентикация с Bearer Token
    /// </summary>
    public class BearerTokenAuthenticationHandler : AuthenticationHandler<BearerTokenAuthenticationOptions>
    {
        private readonly IGetBearerTokenQuery _getBearerTokenQuery;
        private const string AuthorizationHeaderName = "Authorization";
        private readonly ILogger log;
        /// <summary>
        /// Инжектиране на зависимости
        /// </summary>
        /// <param name="options">Настройки</param>
        /// <param name="logger">Системен Логър</param>
        /// <param name="encoder">форматоира текст в URL съвместим</param>
        /// <param name="clock">Системен часовник</param>
        /// <param name="getApiKeyQuery">Метод, позволяващ извличане на токена</param>
        public BearerTokenAuthenticationHandler(
            IOptionsMonitor<BearerTokenAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IGetBearerTokenQuery getApiKeyQuery) : base(options, logger, encoder, clock)
        {
            _getBearerTokenQuery = getApiKeyQuery ?? throw new ArgumentNullException(nameof(getApiKeyQuery));
            log = logger.CreateLogger("BearerToken");
        }

        /// <summary>
        /// Метод, осъществяващ автентикацията
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Path.Value == "/health")
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.TryGetValue(AuthorizationHeaderName, out var tokenHeaderValues))
            {
                log.LogError("No Auth header");
                return AuthenticateResult.Fail("Invalid Token provided.");
            }

            var providedToken = tokenHeaderValues.FirstOrDefault();

            if (tokenHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedToken))
            {
                log.LogError("No Auth value");
                return AuthenticateResult.Fail("Invalid Token provided.");
            }

            string token = providedToken
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault()
                ?.Trim();

            string[] tokenParts = token.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (tokenParts.Length != 2)
            {
                log.LogError("Token parts not 2");
                return AuthenticateResult.Fail("Invalid Token provided.");
            }

            string appKey = tokenParts[0];
            string hash = tokenParts[1];

            var clientInfo = await _getBearerTokenQuery.GetDataByToken(appKey);

            if (clientInfo != null)
            {
                byte[] data = await GetData();

                if (CheckData(data, hash, clientInfo.AppSecret) == false)
                {
                    log.LogError("Token check error");
                    return AuthenticateResult.Fail("Invalid Token provided.");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.UserData, clientInfo.UserData)
                };

                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }

            log.LogError("Token error");
            return AuthenticateResult.Fail("Invalid Token provided.");
        }

        private bool CheckData(byte[] data, string hash, string secret)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] computedHash = hmac.ComputeHash(data);

                return hash == ToHexString(computedHash);
            }
        }

        private async Task<byte[]> GetData()
        {
            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            {
                var body = Request.Body;
                body.Seek(0, SeekOrigin.Begin);
                await body.CopyToAsync(ms);
                data = ms.ToArray();
                body.Seek(0, SeekOrigin.Begin);
            }

            return data;
        }

        /// <summary>
        /// Кодира текст в шестнайсетичен код
        /// </summary>
        /// <param name="bytes">Текста за кодиране, 
        /// като масив от байтове</param>
        /// <returns>текст в шестнайсетичен код</returns>
        private string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
