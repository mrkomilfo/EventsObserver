using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.Data;

namespace TrainingProject.DomainLogic.Services
{
    public class Censor : ICensor
    {
        private readonly ISwearingProvider _swearingProvider;
        private readonly ILogHelper _logger;
        public Censor(ISwearingProvider swearingProvider, ILogHelper logger)
        {
            _swearingProvider = swearingProvider;
            _logger = logger;
        }

        private string MakeRegex(string word)
        {
            _logger.LogMethodCallingWithObject(new { word });

            return Regex.Replace(word, ".{1}", "$0+");
        }

        public async Task<string> HandleMessageAsync(string message)
        {
            _logger.LogMethodCallingWithObject(new { message });

            var swears = _swearingProvider.GetSwearing();
            foreach (string word in swears)
            {
                string pattern = MakeRegex(word);
                Match match = Regex.Match(message, pattern, RegexOptions.IgnoreCase);
                if (match.Length != 0)
                {
                    message = Regex.Replace(message, match.Value, new string('*', match.Length));
                }
            }
            return await Task.FromResult(message);
        }
    }
}
