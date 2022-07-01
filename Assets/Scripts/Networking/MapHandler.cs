using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler
{
    private readonly IReadOnlyCollection<string> maps;
    private readonly int numberOfRounds;

    private int currentRound;
    private List<string> remainingMaps;

    public MapHandler(MapSet mapSet, int numberOfRounds)
    {
        maps = mapSet.Maps;
        this.numberOfRounds = numberOfRounds;

        ResetMaps();
    }

    public string MainMenu
    {
        get
        {
            return remainingMaps[0];
        }
    }

    public string Arena
    {
        get
        {
            return remainingMaps[1];
        }
    }

    private void ResetMaps() => remainingMaps = maps.ToList();
}
