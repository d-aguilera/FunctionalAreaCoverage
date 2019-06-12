using System.Collections.Generic;

namespace FunctionalAreaCoverage.Entities
{
    internal class RelationComparer : IEqualityComparer<Relation>
    {
        public bool Equals(Relation first, Relation second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            return first.Key1 == second.Key1
                && first.Key2 == second.Key1;
        }

        public int GetHashCode(Relation obj)
        {
            return (obj.Key1 + " " + obj.Key2).GetHashCode();
        }
    }
}
