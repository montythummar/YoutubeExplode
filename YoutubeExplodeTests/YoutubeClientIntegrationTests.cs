using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tyrrrz.Extensions;

namespace YoutubeExplode.Tests
{
    [TestClass]
    public class YoutubeClientIntegrationTests
    {
        [TestMethod]
        public void GetVideoInfo_SignedUnrestrictedAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("TZRvO0S-TLU", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("TZRvO0S-TLU", videoInfo.Id);
            Assert.AreEqual("BABYMETAL - THE ONE (OFFICIAL)", videoInfo.Title);
            Assert.AreEqual("BABYMETALofficial", videoInfo.Author);
            Assert.IsTrue(428 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(6000000 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(30, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.IsNotNull(streamInfo.FileExtension);
            }

            Assert.IsNotNull(videoInfo.CaptionTracks);
            Assert.AreEqual(1, videoInfo.CaptionTracks.Length);
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_SignedUnrestrictedAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("TZRvO0S-TLU", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("TZRvO0S-TLU", videoInfo.Id);
            Assert.AreEqual("BABYMETAL - THE ONE (OFFICIAL)", videoInfo.Title);
            Assert.AreEqual("BABYMETALofficial", videoInfo.Author);
            Assert.IsTrue(428 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(6000000 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(30, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Signature);
                Assert.IsNotNull(streamInfo.FileExtension);
            }

            Assert.IsNotNull(videoInfo.CaptionTracks);
            Assert.AreEqual(1, videoInfo.CaptionTracks.Length);
        }

        [TestMethod]
        public void GetVideoInfo_UnsignedUnrestrictedNonAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.IsTrue(103 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(0 <= videoInfo.AverageRating);
            Assert.IsTrue(1 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(9, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.IsNotNull(streamInfo.FileExtension);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_UnsignedUnrestrictedNonAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("LsNPjFXIPT8", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.IsTrue(103 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(0 <= videoInfo.AverageRating);
            Assert.IsTrue(1 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(9, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.IsNotNull(streamInfo.FileExtension);
            }
        }

        [TestMethod]
        public void GetVideoInfo_SignedRestrictedAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("SkRSXFQerZs", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("SkRSXFQerZs", videoInfo.Id);
            Assert.AreEqual("HELLOVENUS 헬로비너스 - 위글위글(WiggleWiggle) M/V", videoInfo.Title);
            Assert.AreEqual("fantagio 판타지오", videoInfo.Author);
            Assert.IsTrue(203 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(1200000 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(28, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.IsNotNull(streamInfo.FileExtension);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_SignedRestrictedAdaptive_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("SkRSXFQerZs", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("SkRSXFQerZs", videoInfo.Id);
            Assert.AreEqual("HELLOVENUS 헬로비너스 - 위글위글(WiggleWiggle) M/V", videoInfo.Title);
            Assert.AreEqual("fantagio 판타지오", videoInfo.Author);
            Assert.IsTrue(203 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(1200000 <= videoInfo.ViewCount);
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(28, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Length);

            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            Assert.IsNotNull(videoInfo.Streams);
            Assert.AreEqual(22, videoInfo.Streams.Length);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.IsNotNull(streamInfo.FileExtension);
            }
        }

        [TestMethod]
        public void DecipherStreams_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", false, false);
            Assert.IsTrue(videoInfo.NeedsDeciphering);

            client.DecipherStreams(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));
        }

        [TestMethod]
        public async Task DecipherStreamsAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", false, false);
            Assert.IsTrue(videoInfo.NeedsDeciphering);

            await client.DecipherStreamsAsync(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));
        }

        [TestMethod]
        public void GetFileSize_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0", true, false);
            var streamInfo = videoInfo.Streams.GetRandom();
            client.GetFileSize(streamInfo);

            Assert.IsTrue(0 < streamInfo.FileSize);
        }

        [TestMethod]
        public async Task GetFileSizeAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0", true, false);
            var streamInfo = videoInfo.Streams.GetRandom();
            await client.GetFileSizeAsync(streamInfo);

            Assert.IsTrue(0 < streamInfo.FileSize);
        }

        [TestMethod]
        public void DownloadVideo_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = client.GetVideoInfo("9bZkp7q19f0");
            var streamInfo = videoInfo.Streams.OrderBy(s => s.FileSize).First();
            using (var stream = client.DownloadVideo(streamInfo))
            {
                // Read some bytes
                var buffer = new byte[5];
                stream.Read(buffer, 0, 5);
            }
        }

        [TestMethod]
        public async Task DownloadVideoAsync_Normal_Test()
        {
            var client = new YoutubeClient();

            var videoInfo = await client.GetVideoInfoAsync("9bZkp7q19f0");
            var streamInfo = videoInfo.Streams.OrderBy(s => s.FileSize).First();
            using (var stream = await client.DownloadVideoAsync(streamInfo))
            {
                // Read some bytes
                var buffer = new byte[5];
                stream.Read(buffer, 0, 5);
            }
        }
    }
}