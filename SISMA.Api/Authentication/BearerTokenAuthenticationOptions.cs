using Microsoft.AspNetCore.Authentication;

namespace SISMA.Api.Authentication
{
    /// <summary>
    /// Настройки на Bearer Token Авторизацията
    /// </summary>
    public class BearerTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Схема по подразбиране
        /// </summary>
        public const string DefaultScheme = "BearerToken";

        /// <summary>
        /// Схема
        /// </summary>
        public string Scheme => DefaultScheme;

        /// <summary>
        /// Тип на автентикацията
        /// </summary>
        public string AuthenticationType = DefaultScheme;
    }
}
