using System.Collections.Generic;
using System.Linq;
using TrainingProject.DomainLogic.Helpers;
using Xunit;

namespace TrainingProject.Logic.Tests
{
    public class EventControllerTests
    {
        [Fact]
        public void ParseTags()
        { 
            string[] tagLists = { 
                "Tag1, tag2 ",
                "Tag1, taG1, ",
                "," , 
                ", ,"
            };

            IEnumerable<string>[] parsedTagLists = tagLists.Select(tl => tl.ParseSubstrings(",")).ToArray();

            Assert.Equal(parsedTagLists[0], new List<string> { "tag1", "tag2" });
            Assert.Equal(parsedTagLists[1], new List<string> { "tag1" });
            Assert.Empty(parsedTagLists[2]);
            Assert.Empty(parsedTagLists[3]);
        }

    } 
}
