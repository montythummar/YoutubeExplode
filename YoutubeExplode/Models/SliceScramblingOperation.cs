// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <SliceScramblingOperation.cs>
//  Created By: Alexey Golub
//  Date: 19/02/2017
// ------------------------------------------------------------------ 

namespace YoutubeExplode.Models
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