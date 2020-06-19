using Serilog;
using System.Collections.Generic;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.DomainLogic.Services;
using Xunit;

namespace TrainingProject.Logic.Tests
{
    public class CensoringTests
    {
        private readonly ICensor _censor;
        public CensoringTests()
        {
            _censor = new Censor(new SwearingProvider(), new LogHelper(Log.Logger));
        }

        [Fact]
        public async void HandleMessage()
        {
            string[] messages = {
                "fucking bullshit", "FUUUCKK!!!", "Finally! Some good fucking food", "Only sluts and bitches"
            };

            List<string> censored = new List<string>();
            foreach (var message in messages)
            {
                censored.Add(await _censor.HandleMessageAsync(message));
            }

            Assert.Equal("****ing ********", censored[0]);
            Assert.Equal("*******!!!", censored[1]);
            Assert.Equal("Finally! Some good ****ing food", censored[2]);
            Assert.Equal("Only ****s and *****es", censored[3]);
        }
    }
}
