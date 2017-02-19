// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <IScramblingOperation.cs>
//  Created By: Alexey Golub
//  Date: 19/02/2017
// ------------------------------------------------------------------ 

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