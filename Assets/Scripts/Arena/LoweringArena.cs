using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoweringArena : NetworkBehaviour
{
    [SerializeField] private float occurOverTime = 10f;  // Time in which to lower arena over

    void Start()
    {
        //GameManager.ServerOnArenaAction += ServerHandleArenaAction;
        GameManager.ServerOnLowerArena += ServerHandleLowerArena;
    }

    private void OnDestroy()
    {
        //GameManager.ServerOnArenaAction -= ServerHandleArenaAction;
        GameManager.ServerOnLowerArena -= ServerHandleLowerArena;
    }

    //[Server]
    //private void ServerHandleArenaAction(Constants.GameAction gameAction)
    //{
    //    switch (gameAction)
    //    {
    //        case Constants.GameAction.LowerArena:
    //            StartCoroutine(ScaleOverTime(lowerOverTime));
    //            break;
    //    }
    //}

    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = transform.localScale;

        Vector3 destinationScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);

        float currentTime = 0.0f;

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }

    private void ServerHandleLowerArena(LoweringArena loweringArena)
    {
        if (loweringArena.name != gameObject.name) { return; }
        StartCoroutine(ScaleOverTime(occurOverTime));
    }
}
