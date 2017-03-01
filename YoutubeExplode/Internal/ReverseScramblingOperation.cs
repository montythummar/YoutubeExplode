namespace YoutubeExplode.Internal
{
    internal class ReverseScramblingOperation : IScramblingOperation
    {
        public string Unscramble(string input)
        {
            return input.Reverse();
        }
    }
}