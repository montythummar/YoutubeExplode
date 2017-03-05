using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YoutubeExplode.Tests
{
    [TestClass]
    public class YoutubeClientUnitTests
    {
        [TestMethod]
        public void ValidateVideoId_Test()
        {
            string[] validVideoIds =
            {
                "cpm00Hv1Umg",
                "aI5pUqiVJdw",
                "9onx5sgnkPQ",
                "lg0s242Hg-8",
                "fIDyDVzlqN4",
                "JE1Gvzxfm1E",
                "OpV62-86Fv4",
                "UnUkNfX8v1E",
                "aGTz8o_fey8",
                "10V6xet5ODk"
            };
            string[] invalidVideoIds =
            {
                null,
                "",
                "@pm!!Hv#Lmg",
                "lg0s242Hg#8",
                "f`DyDVzlqN`",
                "JE1Gv[]fm1E",
                "***62-86Fv4",
                "U  kNfX8v1E",
                "aGяк8o_fey8",
                "10Vあxet5ODk"
            };

            foreach (string validId in validVideoIds)
                Assert.IsTrue(YoutubeClient.ValidateVideoId(validId));
            foreach (string invalidId in invalidVideoIds)
                Assert.IsFalse(YoutubeClient.ValidateVideoId(invalidId));
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