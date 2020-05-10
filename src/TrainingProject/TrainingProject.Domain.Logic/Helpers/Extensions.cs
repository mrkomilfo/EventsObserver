using System.Collections.Generic;
using System.Linq;

namespace TrainingProject.DomainLogic.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<string> ParseSubstrings(this string tags, string divider) => tags
            .Split(divider)
            .Select(t => t.Trim().ToLower())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct();
    }
}
