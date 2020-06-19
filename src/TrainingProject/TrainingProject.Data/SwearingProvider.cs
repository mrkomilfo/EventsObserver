using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingProject.Data
{
    public class SwearingProvider : ISwearingProvider
    {
        private IEnumerable<string> Swear;

        public SwearingProvider()
        {
            Swear = new HashSet<string>
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

        public async Task<IList<string>> GetSwearingAsync()
        {
            return await Task.FromResult(Swear.ToList());
        }
    }
}
