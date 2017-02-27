using System;

namespace YoutubeExplode.Exceptions
{
    /// <summary>
    /// Thrown when Youtube returns an error after a query
    /// </summary>
    public class YoutubeErrorException : Exception
    {
        /// <summary>
        /// Original message, received from Youtube
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Exception messaage
        /// </summary>
        public override string Message { get; }

        internal YoutubeErrorException(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Message = $"Youtube returned an error:{Environment.NewLine}{ErrorMessage}";
        }
    }
}