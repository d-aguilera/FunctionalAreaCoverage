using System;
using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Entities
{
    public class Relation : IEquatable<Relation>
    {
        protected Relation(string key1, string key2)
        {
            Key1 = key1;
            Key2 = key2;
        }

        public string Key1 { get; }
        public string Key2 { get; }

        public string ToCsv() => $"{Key1}{CsvHelper.CsvSeparator}{Key2}";

        public bool Equals(Relation other)
        {
            if (other == null) return false;

            if (ReferenceEquals(this, other)) return true;

            return Key1.Equals(other.Key1) && Key2.Equals(other.Key2);
        }

        public override int GetHashCode()
        {
            var hash1 = Key1.GetHashCode();
            var hash2 = Key2.GetHashCode();
            return hash1 ^ hash2;
        }
    }
}
