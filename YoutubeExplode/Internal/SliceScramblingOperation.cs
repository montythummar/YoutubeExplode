namespace YoutubeExplode.Internal
{
    internal class SliceScramblingOperation : IScramblingOperation
    {
        private readonly int _index;

        public SliceScramblingOperation(int index)
        {
            _index = index;
        }

        public string Unscramble(string input)
        {
            return input.Substring(_index);
        }
    }
}