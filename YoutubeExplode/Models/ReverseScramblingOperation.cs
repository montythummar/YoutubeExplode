// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <ReverseScramblingOperation.cs>
//  Created By: Alexey Golub
//  Date: 19/02/2017
// ------------------------------------------------------------------ 

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