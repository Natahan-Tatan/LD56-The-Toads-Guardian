using System;
using Godot;

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

    public static Vector2 NextVector2(this Random rand,float amp)
    {
        return new Vector2((float)rand.NextDouble() * amp * (rand.Luck(1,2) ? -1 : 1), (float)rand.NextDouble() * amp * (rand.Luck(1,2) ? -1 : 1));
    }
}
