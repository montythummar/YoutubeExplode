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
        public void GetVideoInfo_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("9bZkp7q19f0", videoInfo.Id);
            Assert.AreEqual("PSY - GANGNAM STYLE(강남스타일) M/V", videoInfo.Title);
            Assert.AreEqual("officialpsy", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(252), videoInfo.Length);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(2774821427 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(15, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsTrue(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("9bZkp7q19f0", videoInfo.Id);
            Assert.AreEqual("PSY - GANGNAM STYLE(강남스타일) M/V", videoInfo.Title);
            Assert.AreEqual("officialpsy", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(252), videoInfo.Length);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(2774821427 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(15, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNotNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsTrue(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void GetVideoInfo_NoAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(103), videoInfo.Length);
            Assert.IsTrue(0 <= videoInfo.AverageRating);
            Assert.IsTrue(1 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(10, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_NoAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(103), videoInfo.Length);
            Assert.IsTrue(0 <= videoInfo.AverageRating);
            Assert.IsTrue(1 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(10, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void GetVideoInfo_NoJsonNoAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("6MtLqOErEeo", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("6MtLqOErEeo", videoInfo.Id);
            Assert.AreEqual("EPIC (AND I MEAN EPIC) Trade Box From LastPharaoh! Why Did I Eat 30 Year Old Gum?!",
                videoInfo.Title);
            Assert.AreEqual("The Border Geek", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(1053), videoInfo.Length);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(200 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(8, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_NoJsonNoAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("6MtLqOErEeo", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("6MtLqOErEeo", videoInfo.Id);
            Assert.AreEqual("EPIC (AND I MEAN EPIC) Trade Box From LastPharaoh! Why Did I Eat 30 Year Old Gum?!",
                videoInfo.Title);
            Assert.AreEqual("The Border Geek", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(1053), videoInfo.Length);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(200 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(8, videoInfo.Streams.Length);
            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsNotNull(stream.Url);
                Assert.IsNull(stream.Signature);
                Assert.IsNotNull(stream.FileExtension);
                Assert.IsFalse(stream.NeedsDeciphering);
            }
        }

        [TestMethod]
        public void DecipherStreams_Normal_Test()
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
        public async Task DecipherStreamsAsync_Normal_Test()
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
        public void GetFileSize_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            client.GetFileSize(stream);

            Assert.IsTrue(0 < stream.FileSize);
        }

        [TestMethod]
        public async Task GetFileSizeAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            await client.GetFileSizeAsync(stream);

            Assert.IsTrue(0 < stream.FileSize);
        }

        [TestMethod]
        public void GetAllFileSizes_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            client.DecipherStreams(videoInfo);
            client.GetAllFileSizes(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsTrue(0 < stream.FileSize);
            }
        }

        [TestMethod]
        public async Task GetAllFileSizesAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            await client.GetAllFileSizesAsync(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.IsTrue(0 < stream.FileSize);
            }
        }

        [TestMethod]
        public void DownloadVideo_Normal_Test()
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
        public async Task DownloadVideoAsync_Normal_Test()
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
        public void DownloadVideoToFile_Normal_Test()
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
        public async Task DownloadVideoToFileAsync_Normal_Test()
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