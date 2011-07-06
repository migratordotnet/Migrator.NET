namespace Migrator.Providers
{
    public class ProviderGlobalSettings
    {
        public static string TableSchemaInfo { get; set; }
        public static string ColumnScope { get; set; }
        public static string ColumnVersion { get; set; }
        public static string GlobalScopeId { get; set; }

        static ProviderGlobalSettings()
        {
            TableSchemaInfo = "DbSchemaInfo";
            ColumnScope = "Scope";
            ColumnVersion = "Version";
            GlobalScopeId = "00000000000000000000000000000000";
        }
    }
}
