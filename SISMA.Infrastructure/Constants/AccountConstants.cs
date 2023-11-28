namespace SISMA.Infrastructure.Constants
{
    /// <summary>
    /// Константи за управление на потребители
    /// </summary>
    public class AccountConstants
    {
        public const string EmailRegexPatern = @"^(?:[a-zA-Z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+\/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[A-Za-z0-9-]*[A-Za-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";

        /// <summary>
        /// Роли в системата
        /// </summary>
        public class Roles
        {
            /// <summary>
            /// Статистик
            /// </summary>
            public const string REPORT = "REPORT";

            /// <summary>
            /// Администратор на данни
            /// </summary>
            public const string DATA = "DATA";

            /// <summary>
            /// Локален администратор
            /// </summary>
            public const string ADMIN = "ADMIN";

            ///// <summary>
            ///// Глобален администратор
            ///// </summary>
            //public const string GLOBAL_ADMIN = "GLOBAL_ADMIN";
        }
    }
}
