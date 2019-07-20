using System;
using Scai.RaceTrack.Data;
using Scai.RaceTrack.Player;

public class Ai : IPlayer
{
    private Random rnd;

    public void Start()
    {
        rnd = new Random();
    }

    public int UpdateSpeed(Car car)
    {
        return rnd.Next(0, 100);
    }
}