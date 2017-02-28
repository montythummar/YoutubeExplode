using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tyrrrz.Extensions;

namespace YoutubeExplode.Tests
{
    [TestClass]
    public class YoutubeClientIntegrationTests
    {
        [TestMethod]
        public void GetVideoInfoFromWatchPageTest()
        {
            var client = new YoutubeClient();

            // This video supports adaptive streams and is parseable from watch page
            // Needs deciphering
            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("9bZkp7q19f0", videoInfo.Id);
            Assert.AreEqual("PSY - GANGNAM STYLE(강남스타일) M/V", videoInfo.Title);
            Assert.AreEqual("officialpsy", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(252), videoInfo.Length);
            Assert.IsTrue(4d <= videoInfo.AverageRating);
            Assert.IsTrue(2774821427ul <= videoInfo.ViewCount);
            Assert.AreEqual(15, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(17, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNotNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsTrue(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoFromWatchPageAsyncTest()
        {
            var client = new YoutubeClient();

            // This video supports adaptive streams and is parseable from watch page
            // Needs deciphering
            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("9bZkp7q19f0", videoInfo.Id);
            Assert.AreEqual("PSY - GANGNAM STYLE(강남스타일) M/V", videoInfo.Title);
            Assert.AreEqual("officialpsy", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(252), videoInfo.Length);
            Assert.IsTrue(4d <= videoInfo.AverageRating);
            Assert.IsTrue(2774821427ul <= videoInfo.ViewCount);
            Assert.AreEqual(15, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(17, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNotNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsTrue(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void GetVideoInfoFromInternalApiTest()
        {
            var client = new YoutubeClient();

            // This video doesn't support adaptive streams and can't be parsed from watch page
            // Doesn't need deciphering
            var videoInfo = client.GetVideoInfo("6MtLqOErEeo", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("6MtLqOErEeo", videoInfo.Id);
            Assert.AreEqual("EPIC (AND I MEAN EPIC) Trade Box From LastPharaoh! Why Did I Eat 30 Year Old Gum?!",
                videoInfo.Title);
            Assert.AreEqual("The Border Geek", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(1053), videoInfo.Length);
            Assert.IsTrue(4d <= videoInfo.AverageRating);
            Assert.IsTrue(200ul <= videoInfo.ViewCount);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(5, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoFromInternalApiAsyncTest()
        {
            var client = new YoutubeClient();

            // This video doesn't support adaptive streams and can't be parsed from watch page
            // Doesn't need deciphering
            var videoInfo = await client.GetVideoInfoAsync("6MtLqOErEeo", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("6MtLqOErEeo", videoInfo.Id);
            Assert.AreEqual("EPIC (AND I MEAN EPIC) Trade Box From LastPharaoh! Why Did I Eat 30 Year Old Gum?!",
                videoInfo.Title);
            Assert.AreEqual("The Border Geek", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(1053), videoInfo.Length);
            Assert.IsTrue(4d <= videoInfo.AverageRating);
            Assert.IsTrue(200ul <= videoInfo.ViewCount);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(5, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void GetVideoInfoLegacyStreamsTest()
        {
            var client = new YoutubeClient();

            // This video doesn't support adaptive streams and is parseable from watch page
            // Doesn't need deciphering
            var videoInfo = client.GetVideoInfo("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(103), videoInfo.Length);
            Assert.IsTrue(0d <= videoInfo.AverageRating);
            Assert.IsTrue(1ul <= videoInfo.ViewCount);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(5, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoLegacyStreamsAsyncTest()
        {
            var client = new YoutubeClient();

            // This video doesn't support adaptive streams and is parseable from watch page
            // Doesn't need deciphering
            var videoInfo = await client.GetVideoInfoAsync("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(103), videoInfo.Length);
            Assert.IsTrue(0d <= videoInfo.AverageRating);
            Assert.IsTrue(1ul <= videoInfo.ViewCount);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(5, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.TypeString);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void DecipherStreamsTest()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));

            // Try open the stream (if decipher failed, should throw error 401)
            var stream = videoInfo.Streams.GetRandom();
            using (WebRequest.CreateHttp(stream.Url).GetResponse())
            {
            }
        }

        [TestMethod]
        public async Task DecipherStreamsAsyncTest()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));

            // Try open the stream (if decipher failed, should throw error 401)
            var stream = videoInfo.Streams.GetRandom();
            using (WebRequest.CreateHttp(stream.Url).GetResponse())
            {
            }
        }

        [TestMethod]
        public void GetFileSizeTest()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            client.GetFileSize(stream);

            Assert.IsTrue(0ul < stream.FileSize);
        }

        [TestMethod]
        public async Task GetFileSizeAsyncTest()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            await client.GetFileSizeAsync(stream);

            Assert.IsTrue(0ul < stream.FileSize);
        }

        [TestMethod]
        public void GetAllFileSizesTest()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            client.GetAllFileSizes(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsTrue(0ul < stream.FileSize);
            }
        }

        [TestMethod]
        public async Task GetAllFileSizesAsyncTest()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            await client.GetAllFileSizesAsync(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsTrue(0ul < stream.FileSize);
            }
        }

        [TestMethod]
        public void DownloadVideoTest()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            client.GetAllFileSizes(videoInfo);

            var stream = videoInfo.Streams.OrderBy(s => s.FileSize).First();
            using (client.DownloadVideo(stream))
            {

            }
        }

        [TestMethod]
        public async Task DownloadVideoAsyncTest()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            await client.GetAllFileSizesAsync(videoInfo);

            var stream = videoInfo.Streams.OrderBy(s => s.FileSize).First();
            using (await client.DownloadVideoAsync(stream))
            {

            }
        }

        [TestMethod]
        public void DownloadVideoToFileTest()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            client.GetAllFileSizes(videoInfo);

            var stream = videoInfo.Streams.OrderBy(s => s.FileSize).First();

            string filePath = "test_video";
            client.DownloadVideo(stream, filePath);

            var fileInfo = new FileInfo(filePath);

            Assert.IsTrue(fileInfo.Exists);
            Assert.IsTrue(0 < fileInfo.Length);
            File.Delete(filePath);
        }

        [TestMethod]
        public async Task DownloadVideoToFileAsyncTest()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            await client.GetAllFileSizesAsync(videoInfo);

            var stream = videoInfo.Streams.OrderBy(s => s.FileSize).First();

            string filePath = "test_video";
            await client.DownloadVideoAsync(stream, filePath);

            var fileInfo = new FileInfo(filePath);

            Assert.IsTrue(fileInfo.Exists);
            Assert.IsTrue(0 < fileInfo.Length);
            File.Delete(filePath);
        }
    }
}