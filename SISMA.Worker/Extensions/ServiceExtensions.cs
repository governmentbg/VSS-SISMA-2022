// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SISMA.Core.Contracts;
using SISMA.Core.Services;
using SISMA.Infrastructure.Data;
using SISMA.Infrastructure.Data.Common;
using SISMA.Worker.Contracts;
using SISMA.Worker.Services;

namespace SISMA.Worker.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Регистрира услугите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHostedService<FtpWorkerTimedHostedService>();
            services.AddScoped(typeof(IFtpWorker), typeof(FtpWorker));
            services.AddScoped(typeof(INomenclatureService), typeof(NomenclatureService));
            services.AddScoped(typeof(IDataService), typeof(DataService));
            services.AddScoped(typeof(IExcelFileProccess), typeof(ExcelFileProccess));
        }

        /// <summary>
        /// Регистрира контекстите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                   m => m.MigrationsAssembly("SISMA.Infrastructure"))
               .UseSnakeCaseNamingConvention());
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScoped<IRepository, Repository>();
        }

        public static string UriCombine(string basePath, string path)
        {
            return new Uri(new Uri(basePath), path).AbsoluteUri;
        }
    }
}
