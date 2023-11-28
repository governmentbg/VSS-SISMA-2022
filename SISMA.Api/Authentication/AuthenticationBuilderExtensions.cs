using Microsoft.AspNetCore.Authentication;
using System;

namespace SISMA.Api.Authentication
{
    /// <summary>
    /// Методи за добавяне на Bearer Token авторизация в IoC контейнера
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Добавяне към IoC контейнера с възможност за настройка
        /// </summary>
        /// <param name="authenticationBuilder">Разширяван обект</param>
        /// <param name="options">Настройки</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddBearerTokenSupport(this AuthenticationBuilder authenticationBuilder, Action<BearerTokenAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<BearerTokenAuthenticationOptions, BearerTokenAuthenticationHandler>(BearerTokenAuthenticationOptions.DefaultScheme, options);
        }

        /// <summary>
        /// Добавяне към IoC контейнера
        /// </summary>
        /// <param name="authenticationBuilder">Разширяван обект</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddBearerTokenSupport(this AuthenticationBuilder authenticationBuilder)
        {
            return authenticationBuilder.AddScheme<BearerTokenAuthenticationOptions, BearerTokenAuthenticationHandler>(BearerTokenAuthenticationOptions.DefaultScheme, options => { });
        }
    }
}
