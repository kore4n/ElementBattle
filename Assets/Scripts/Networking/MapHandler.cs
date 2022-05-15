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


    public string NextMap
    {
        get
        {
            string map = remainingMaps[0];


            return map;
        }
    }

    private void ResetMaps() => remainingMaps = maps.ToList();
}
