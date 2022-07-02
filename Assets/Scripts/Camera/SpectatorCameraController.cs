using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCameraController : NetworkBehaviour
{
    private float mouseSensitivity = 1f;
    private Vector2 turn;

    public override void OnStartClient()
    {
        if (!hasAuthority)
        {
            gameObject.GetComponent<Camera>().enabled = false;
            gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }

    void FixedUpdate()
    {
        float keyboardX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * 30f;
        float keyboardY = Input.GetAxisRaw("Vertical") * Time.deltaTime * 30f;

        turn.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity * 300f * Time.deltaTime;
        turn.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 300f * Time.deltaTime;
        turn.y = Mathf.Clamp(turn.y, -90f, 90f);

        Vector3 finalDir = new Vector3(keyboardX, 0f, keyboardY);

        transform.Translate(finalDir, Space.Self);

        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0f);
    }
}
