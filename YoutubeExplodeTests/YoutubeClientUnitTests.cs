using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tyrrrz.Extensions;
using YoutubeExplode.Tests.Mocks;

namespace YoutubeExplode.Tests
{
    [TestClass]
    public class YoutubeClientUnitTests
    {
        [TestMethod]
        public void GetVideoInfo_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = client.GetVideoInfo("test", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("JE1Gvzxfm1E", videoInfo.Id);
            Assert.AreEqual("BURNT RICE", videoInfo.Title);
            Assert.AreEqual("Shawn Wasabi", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(122), videoInfo.Length);
            Assert.AreEqual(4.96507930756d, videoInfo.AverageRating);
            Assert.AreEqual(8481110ul, videoInfo.ViewCount);
            Assert.AreEqual(36, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/default.jpg", videoInfo.Thumbnail);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/hqdefault.jpg", videoInfo.ImageHighQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/mqdefault.jpg", videoInfo.ImageMediumQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/sddefault.jpg", videoInfo.ImageLowQuality);

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
        public async Task GetVideoInfoAsync_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = await client.GetVideoInfoAsync("test", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("JE1Gvzxfm1E", videoInfo.Id);
            Assert.AreEqual("BURNT RICE", videoInfo.Title);
            Assert.AreEqual("Shawn Wasabi", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(122), videoInfo.Length);
            Assert.AreEqual(4.96507930756d, videoInfo.AverageRating);
            Assert.AreEqual(8481110ul, videoInfo.ViewCount);
            Assert.AreEqual(36, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/default.jpg", videoInfo.Thumbnail);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/hqdefault.jpg", videoInfo.ImageHighQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/mqdefault.jpg", videoInfo.ImageMediumQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/sddefault.jpg", videoInfo.ImageLowQuality);

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
        public void GetVideoInfo_NoJson_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = false;
            requestService.IsInternalApiWorking = true;
            var videoInfo = client.GetVideoInfo("test", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("JE1Gvzxfm1E", videoInfo.Id);
            Assert.AreEqual("BURNT RICE", videoInfo.Title);
            Assert.AreEqual("Shawn Wasabi", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(122), videoInfo.Length);
            Assert.AreEqual(4.96507930756d, videoInfo.AverageRating);
            Assert.AreEqual(8481110ul, videoInfo.ViewCount);
            Assert.AreEqual(36, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/default.jpg", videoInfo.Thumbnail);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/hqdefault.jpg", videoInfo.ImageHighQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/mqdefault.jpg", videoInfo.ImageMediumQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/sddefault.jpg", videoInfo.ImageLowQuality);

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
        public async Task GetVideoInfoAsync_NoJson_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = false;
            requestService.IsInternalApiWorking = true;
            var videoInfo = await client.GetVideoInfoAsync("test", false, false);

            Assert.IsNotNull(videoInfo);
            Assert.AreEqual("JE1Gvzxfm1E", videoInfo.Id);
            Assert.AreEqual("BURNT RICE", videoInfo.Title);
            Assert.AreEqual("Shawn Wasabi", videoInfo.Author);
            Assert.AreEqual(TimeSpan.FromSeconds(122), videoInfo.Length);
            Assert.AreEqual(4.96507930756d, videoInfo.AverageRating);
            Assert.AreEqual(8481110ul, videoInfo.ViewCount);
            Assert.AreEqual(36, videoInfo.Keywords.Length);
            Assert.IsTrue(videoInfo.Keywords.All(k => !string.IsNullOrWhiteSpace(k)));
            Assert.AreEqual(2, videoInfo.Watermarks.Length);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/default.jpg", videoInfo.Thumbnail);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/hqdefault.jpg", videoInfo.ImageHighQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/mqdefault.jpg", videoInfo.ImageMediumQuality);
            Assert.AreEqual("https://i.ytimg.com/vi/JE1Gvzxfm1E/sddefault.jpg", videoInfo.ImageLowQuality);

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
        public void DecipherStreams_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = client.GetVideoInfo("test", false, false);
            client.DecipherStreams(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));
        }

        [TestMethod]
        public async Task DecipherStreamsAsync_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = await client.GetVideoInfoAsync("test", false, false);
            await client.DecipherStreamsAsync(videoInfo);

            Assert.IsFalse(videoInfo.NeedsDeciphering);
            Assert.IsFalse(videoInfo.Streams.Any(s => s.NeedsDeciphering));
        }

        [TestMethod]
        public void GetFileSize_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = client.GetVideoInfo("test", false, false);
            client.DecipherStreams(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            client.GetFileSize(stream);

            Assert.AreEqual(1337ul, stream.FileSize);
        }

        [TestMethod]
        public async Task GetFileSizeAsync_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = await client.GetVideoInfoAsync("test", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            var stream = videoInfo.Streams.GetRandom();
            await client.GetFileSizeAsync(stream);

            Assert.AreEqual(1337ul, stream.FileSize);
        }

        [TestMethod]
        public void GetAllFileSizes_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = client.GetVideoInfo("test", false, false);
            client.DecipherStreams(videoInfo);
            client.GetAllFileSizes(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.AreEqual(1337ul, stream.FileSize);
            }
        }

        [TestMethod]
        public async Task GetAllFileSizesAsync_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            requestService.IsWatchPageWorking = true;
            requestService.IsInternalApiWorking = false;
            var videoInfo = await client.GetVideoInfoAsync("test", false, false);
            await client.DecipherStreamsAsync(videoInfo);
            await client.GetAllFileSizesAsync(videoInfo);

            foreach (var stream in videoInfo.Streams)
            {
                Assert.AreEqual(1337ul, stream.FileSize);
            }
        }

        [TestMethod]
        public void DownloadVideo_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            var videoInfo = client.GetVideoInfo("test", false, false);
            client.DecipherStreams(videoInfo);

            var stream = videoInfo.Streams.GetRandom();
            using (client.DownloadVideo(stream))
            {

            }
        }

        [TestMethod]
        public async Task DownloadVideoAsync_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            var videoInfo = await client.GetVideoInfoAsync("test", false, false);
            await client.DecipherStreamsAsync(videoInfo);

            var stream = videoInfo.Streams.GetRandom();
            using (await client.DownloadVideoAsync(stream))
            {

            }
        }

        [TestMethod]
        public void DownloadVideoToFile_Normal_Test()
        {
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            var videoInfo = client.GetVideoInfo("test", false, false);
            client.DecipherStreams(videoInfo);

            var stream = videoInfo.Streams.GetRandom();

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
            var requestService = new FakeRequestService();
            var client = new YoutubeClient(requestService);

            var videoInfo = await client.GetVideoInfoAsync("test", false, false);
            await client.DecipherStreamsAsync(videoInfo);

            var stream = videoInfo.Streams.Last();

            string filePath = "test_video";
            await client.DownloadVideoAsync(stream, filePath);

            var fileInfo = new FileInfo(filePath);

            Assert.IsTrue(fileInfo.Exists);
            Assert.IsTrue(0 < fileInfo.Length);
            File.Delete(filePath);
        }
    }
}