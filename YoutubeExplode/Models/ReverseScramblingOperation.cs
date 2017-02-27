namespace YoutubeExplode.Models
{
    internal class ReverseScramblingOperation : IScramblingOperation
    {
        public string Unscramble(string input)
        {
            return input.Reverse();
        }
    }
}