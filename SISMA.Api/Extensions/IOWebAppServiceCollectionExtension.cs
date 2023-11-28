using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using SISMA.Api.Authentication;
using SISMA.Api.Filters;
using SISMA.Core.Contracts;
using SISMA.Core.Services;
using SISMA.Infrastructure.Data;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Identity;

namespace SISMA.Api.Extensions
{
    /// <summary>
    /// Описва услугите и контекстите на приложението
    /// </summary>
    public static class IOWebAppServiceCollectionExtension
    {
        /// <summary>
        /// Регистрира услугите на приложението в  IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            //services.AddAutoMapper(typeof(IOWebFrameworkProfile).Assembly);

            services.AddScoped<INomenclatureService, NomenclatureService>();
            services.AddScoped<IDataService, DataService>();
            //services.AddScoped<ILogOperationService<ApplicationDbContext>, LogOperationService<ApplicationDbContext>>();

            services.AddSwaggerInfo();
        }

        public static void AddSwaggerInfo(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "СПЕЦИАЛИЗИРАНА ИНФОРМАЦИОННА СИСТЕМА, ВКЛЮЧВАЩА ИНСТРУМЕНТИ ЗА АНАЛИЗ И МОНИТОРИНГ НА ФАКТОРИТЕ, СВЪРЗАНИ СЪС СОЦИАЛНО-ИКОНОМИЧЕСКОТО РАЗВИТИЕ НА СЪДЕБНИТЕ РАЙОНИ И НАТОВАРЕНОСТТА НА СЪДИЛИЩАТА И ПРОКУРАТУРИТЕ",
                    Version = "v1",
                    Description = "Услуги за интеграция с **СИСМА**. Тези услуги предоставят възможност на деловодните системи на съдилищата да обменят данни с Електронния публичен регистър на отводите",
                    Contact = new OpenApiContact()
                    {
                        Url = new Uri("https://vss.justice.bg"),
                        Name = "СИСМА"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Лицензирано при условията на EUPL-1.2",
                        Url = new Uri("https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12")
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Автентикационен хедър от тип Bearer Token. Въведете **'Bearer'** [интервал] и Вашия AppKey[точка]HmacSha256 на съдържанието на заявката в полето по-долу. Пример: **'Bearer f60748be-7c48-4793-ace6-88cbd9719521.ea92fb32bd042f0fa76ee9fbd8c637577d4702aacbc61d1f2daba1132dfbc63a'**",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = BearerTokenAuthenticationOptions.DefaultScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = BearerTokenAuthenticationOptions.DefaultScheme,
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                c.IncludeXmlComments("SISMAApi.xml");
                c.DocumentFilter<ApplyTagDescriptions>();

            });
        }

        /// <summary>
        /// Регистрира контекстите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
               //    m => m.MigrationsAssembly("SISMA.Infrastructure"))
               //.UseSnakeCaseNamingConvention()
               //);

               options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                   m => m.MigrationsAssembly("SISMA.Infrastructure"))
               .UseSnakeCaseNamingConvention());
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScoped<IRepository, Repository>();
        }        
    }
}
