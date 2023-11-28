namespace SISMA.Infrastructure.Constants
{
    public static class NomenclatureConstants
    {
        public const string AssemblyQualifiedName = "SISMA.Infrastructure.Data.Models.Nomenclatures.{0}, SISMA.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public class Integrations
        {
            public const int EISS = 1;
            public const int EDIS = 2;
            public const int CISSS = 3;
            public const int UIS = 4;
            public const int EISPP = 5;
            public const int NSI = 6;

            public static string EntityLabel(int integrationId)
            {
                switch (integrationId)
                {
                    case EISS:
                    case EDIS:
                        return "съдилища";
                    case UIS:
                        return "прокуратури";
                    default:
                        return string.Empty;
                }
            }
        }

        public class ApealRegionTypes
        {
            public const int Court = 1;
            public const int Prosecutor = 2;
            public const int Inquest = 3;

            public static int GetByIntegration(int integrationId)
            {
                switch (integrationId)
                {
                    case Integrations.EISS:
                    case Integrations.EDIS:
                        return Court;
                    case Integrations.UIS:
                    case Integrations.EISPP:
                    case Integrations.CISSS:
                        return Prosecutor;
                    default: return 0;
                }
            }
        }

        public class SubjectTypes
        {
            public const int Judge = 1;
            public const int Prosecutor = 2;
            public const int Inspector = 3;
        }

        public class CatalogDetailsType
        {
            public const int None = 0;
            public const int CaseCode = 1;
            public const int Court = 2;
            public const int Subject = 3;
        }

        public class StatReportType
        {
            public const int ByIBD = 1;
            public const int ByIBD3Years = 2;
            public const int ByIBD3YearsInverse = 3;
            public const int ByCourt = 4;
            public const int ByCaseCode = 5;
            public const int ByIBD3YearsMixed = 6;
            public const int ByIbdByOSV = 8;
        }

        public class ReportStates
        {
            public const int Saved = 1;
            public const int New = 2;
            public const int Deleted = 3;
            public const int Dublicated = 4;
        }
        public class ReportSources
        {
            public const int API = 1;
            public const int FTP = 2;
        }

        public class CourtTypes
        {
            public const int Raionen = 11;
            public const int Okrajen = 10;
            public const int Apeal = 8;
            public const int Voenen = 7;
            public const int ApealVoenen = 6;

            public const int Konstitucionen = 1;
            public const int VKS = 2;
            public const int VAS = 3;
            public const int SpecNakazatelen = 5;
            public static int[] FilteredByRegion = { Raionen, Okrajen, Apeal, Voenen, ApealVoenen };
            public static int[] FilteredByDistrict = { Raionen, Okrajen, Voenen };
            public static int[] CityDistanseExluded = { Konstitucionen, VKS, VAS, SpecNakazatelen };
        }

        public class ProsecutorTypes
        {
            public const int RegionalnaProkuratura = 1;
            public const int TeritorialnoOtdelenie = 2;
            public const int OkrajnaProkuratura = 3;
            public const int ApealProkuratura = 4;
            public const int VoennaOkrajnaProkuratura = 9;
            public const int ApealVoennaProkuratura = 10;
            public static int[] FilteredByRegion = { RegionalnaProkuratura, OkrajnaProkuratura, ApealProkuratura, VoennaOkrajnaProkuratura, ApealVoennaProkuratura };
            public static int[] FilteredByDistrict = { RegionalnaProkuratura, OkrajnaProkuratura, VoennaOkrajnaProkuratura };
            public static int[] NotSelectable = { TeritorialnoOtdelenie };
        }

        /// <summary>
        /// Стойности за видове операции в журнал на промените
        /// </summary>
        public class AuditOperations
        {
            public const string Add = "Добавяне";
            public const string Edit = "Редакция";
            public const string Patch = "Актуализация";
            public const string Delete = "Изтриване";
            public const string View = "Преглед";
            public const string Correct = "Корекция";
            public const string List = "Списък";
            public const string Login = "Вход в системата";
        }
    }
}
