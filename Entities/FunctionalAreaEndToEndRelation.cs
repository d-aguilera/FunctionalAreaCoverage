using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Entities
{
    public class FunctionalAreaEndToEndRelation : Relation
    {
        public static readonly string CsvHeader = $"FA_Key{CsvHelper.CsvSeparator}E2E_Key";

        public FunctionalAreaEndToEndRelation(string key1, string key2) : base(key1, key2)
        {
        }
    }
}
