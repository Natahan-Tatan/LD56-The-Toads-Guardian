using System;

public static class RandomExtensions
{
    public static bool Luck(this Random rand, float num, float denom)
    {
        if(num < 0)
        {
            throw new ArgumentOutOfRangeException("num must be positive");
        }

        if(denom <= 0)
        {
            throw new ArgumentOutOfRangeException("denom must be positive and not null");
        }

        if(num > denom)
        {
            throw new ArgumentOutOfRangeException("num must be lesser than denom");
        }

        return rand.NextDouble() <= (num / denom);
    }
}
