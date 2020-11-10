using System.Collections;
using System.Collections.Generic;

public class PlayerLeaderboardDataComparer : IComparer<PlayerLeaderboardData>
{
    public int Compare(PlayerLeaderboardData x, PlayerLeaderboardData y)
    {
        PlayerLeaderboardData data1 = x;
        PlayerLeaderboardData data2 = y;

        if (data1.HighestLevel < data2.HighestLevel)
            return 1;
        if (data1.HighestLevel > data2.HighestLevel)
            return -1;
        else
            return 0;
    }
}
