using System.Text;

namespace YoutubeExplode.Models
{
    internal class SwapScramblingOperation : IScramblingOperation
    {
        private readonly int _index;

        public SwapScramblingOperation(int index)
        {
            _index = index;
        }

        public string Unscramble(string input)
        {
            var sb = new StringBuilder(input)
            {
                [0] = input[_index],
                [_index] = input[0]
            };
            return sb.ToString();
        }
    }
}