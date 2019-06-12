using System.Collections.Generic;

namespace FunctionalAreaCoverage.Entities
{
    internal class JiraIssueComparer : IEqualityComparer<EndToEnd>
    {
        public bool Equals(EndToEnd left, EndToEnd right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.Key == right.Key;
        }

        public int GetHashCode(EndToEnd obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
