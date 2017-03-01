using System;

namespace YoutubeExplode.Internal
{
    internal class PlayerSource
    {
        public IScramblingOperation[] ScramblingOperations { get; internal set; }

        public string Unscramble(string input)
        {
            if (input.IsBlank())
                throw new ArgumentNullException(input);

            foreach (var operation in ScramblingOperations)
                input = operation.Unscramble(input);
            return input;
        }
    }
}