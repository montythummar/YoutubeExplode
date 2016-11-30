// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <ScramblingOperation.cs>
//  Created By: Alexey Golub
//  Date: 30/11/2016
// ------------------------------------------------------------------ 

namespace YoutubeExplode.Models
{
    internal class ScramblingOperation
    {
        public ScramblingOperationType Type { get; }
        public int Parameter { get; }

        public ScramblingOperation(ScramblingOperationType type, int parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public ScramblingOperation(ScramblingOperationType type)
            : this(type, -1)
        { }
    }
}
