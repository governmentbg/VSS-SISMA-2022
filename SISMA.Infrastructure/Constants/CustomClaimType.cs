namespace SISMA.Infrastructure.Constants
{
    public static class CustomClaimType
    {
        public static class IdStampit
        {
            public static string PersonalId = "urn:stampit:pid";

            public static string Organization = "urn:stampit:organization";

            public static string PublicKey = "urn:stampit:public_key";

            public static string CertificateNumber = "urn:stampit:certno";
        }

        public static string FullName = "urn:io:full_name";
        public static string UIC = "urn:io:uic";
    }
}
