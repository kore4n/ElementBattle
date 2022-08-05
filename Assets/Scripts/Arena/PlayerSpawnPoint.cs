using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private Constants.Team team = Constants.Team.Missing;

    public Constants.Team GetTeam()
    {
        return team;
    }
}
