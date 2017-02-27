namespace YoutubeExplode.Models
{
    internal interface IScramblingOperation
    {
        /// <summary>
        /// Unscrambles the given string
        /// </summary>
        string Unscramble(string input);
    }
}