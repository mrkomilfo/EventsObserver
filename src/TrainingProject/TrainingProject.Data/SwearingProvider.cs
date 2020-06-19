using System.Collections.Generic;
using System.Linq;

namespace TrainingProject.Data
{
    public class SwearingProvider : ISwearingProvider
    {
        private readonly IEnumerable<string> Swear;

        public SwearingProvider()
        {
            Swear = new List<string>
            {
                "fuck",
                "bullshit",
                "shit",
                "asshole",
                "bitch",
                "cunt",
                "slut",
                "whore"
            };
        }

        public List<string> GetSwearing()
        {
            return Swear.ToList();
        }
    }
}
