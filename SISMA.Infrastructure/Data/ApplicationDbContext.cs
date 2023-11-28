using Microsoft.EntityFrameworkCore;
using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Common;
using SISMA.Infrastructure.Data.Models.Ekatte;
using SISMA.Infrastructure.Data.Models.Identity;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using SISMA.Infrastructure.Data.Models.Report;

namespace SISMA.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Identity configuration

            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new ApplicationRoleConfiguration());
            builder.ApplyConfiguration(new ApplicationUserRoleConfiguration());
            builder.ApplyConfiguration(new ApplicationUserClaimConfiguration());
            builder.ApplyConfiguration(new ApplicationUserLoginConfiguration());
            builder.ApplyConfiguration(new ApplicationRoleClaimConfiguration());
            builder.ApplyConfiguration(new ApplicationUserTokenConfiguration());

            #endregion


            builder.Entity<CommonCourtEkatte>()
               .HasKey(x => new { x.CourtId, x.EkEkatteId });

            builder.Entity<CommonProsecutorEkatte>()
              .HasKey(x => new { x.ProsecutorId, x.EkEkatteId });
        }

        #region Common
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<CommonCourt> CommonCourt { get; set; }
        public DbSet<CommonCourtDistance> CommonCourtDistance { get; set; }
        public DbSet<CommonInquest> CommonInquest { get; set; }
        public DbSet<CommonProsecutor> CommonProsecutor { get; set; }
        public DbSet<CommonSubject> CommonSubject { get; set; }

        #endregion

        #region Ekatte
        public DbSet<EkEkatte> EkEkatte { get; set; }

        #endregion

        #region Report
        public DbSet<ReportData> ReportData { get; set; }
        public DbSet<ReportEiss> ReportEiss { get; set; }
        public DbSet<ReportEissCode> ReportEissCode { get; set; }
        public DbSet<ReportEissSubject> ReportEissSubject { get; set; }

        public DbSet<ReportCis> ReportCis { get; set; }
        public DbSet<ReportEdis> ReportEdis { get; set; }
        public DbSet<ReportEdisSubject> ReportEdisSubject { get; set; }
        public DbSet<ReportEispp> ReportEispp { get; set; }
        public DbSet<ReportEisppMunicipality> ReportEisppMunicipality { get; set; }

        
        public DbSet<ReportEisppSubject> ReportEisppSubject { get; set; }
        public DbSet<ReportNsi> ReportNsi { get; set; }
        public DbSet<ReportUis> ReportUis { get; set; }

        #endregion

        #region Nomenclatures

        public DbSet<NomCaseCode> NomCaseCode { get; set; }
        public DbSet<NomCaseCodeCatalog> NomCaseCodeCatalog { get; set; }
        public DbSet<NomCatalog> NomCatalog { get; set; }
        public DbSet<NomCatalogCode> NomCatalogCode { get; set; }
        public DbSet<NomCourtType> NomCourtType { get; set; }
        public DbSet<NomInquestType> NomInquestType { get; set; }
        public DbSet<NomIntegration> NomIntegration { get; set; }
        public DbSet<NomProsecutorType> NomProsecutorType { get; set; }
        public DbSet<NomReportSource> NomReportSource { get; set; }
        public DbSet<NomReportState> NomReportState { get; set; }
        public DbSet<NomSubjectType> NomSubjectType { get; set; }
        public DbSet<NomStatReport> NomStatReport { get; set; }
        public DbSet<NomStatReportCode> NomStatReportCode { get; set; }
        public DbSet<NomStatReportCol> NomStatReportCol { get; set; }
        public DbSet<NomStatReportType> NomStatReportType { get; set; }

        #endregion

        public DbSet<LogOperation> LogOperation { get; set; }
    }
}
