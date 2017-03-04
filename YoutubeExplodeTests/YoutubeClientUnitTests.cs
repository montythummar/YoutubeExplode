using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YoutubeExplode.Tests
{
    [TestClass]
    public class YoutubeClientUnitTests
    {
        [TestMethod]
        public void ValidateVideoId_Test()
        {
            string validVideoId = "cpm00Hv1Umg";
            string invalidVideoId = "sdpj#$e]";

            Assert.IsTrue(YoutubeClient.ValidateVideoId(validVideoId));
            Assert.IsFalse(YoutubeClient.ValidateVideoId(invalidVideoId));
        }

        [TestMethod]
        public void ParseVideoId_Test()
        {
            string videoUrl = "https://www.youtube.com/watch?v=cpm00Hv1Umg";

            string parsed = YoutubeClient.ParseVideoId(videoUrl);

            Assert.AreEqual("cpm00Hv1Umg", parsed);
        }

        [TestMethod]
        public void TryParseVideoId_Test()
        {
            string validVideoUrl = "https://www.youtube.com/watch?v=cpm00Hv1Umg";
            string invalidVideoUrl = "https://www.youtube.com/";

            string parsedValid;
            string parsedInvalid;

            bool valid = YoutubeClient.TryParseVideoId(validVideoUrl, out parsedValid);
            bool invalid = YoutubeClient.TryParseVideoId(invalidVideoUrl, out parsedInvalid);

            Assert.IsTrue(valid);
            Assert.IsFalse(invalid);
            Assert.AreEqual("cpm00Hv1Umg", parsedValid);
        }
    }
}