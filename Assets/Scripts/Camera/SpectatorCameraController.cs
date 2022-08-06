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

        if (NetworkServer.active) { return; }   // If running on server as well - stop

        Debug.Log("Adding to spectator cameras!");
        ((FPSNetworkManager)NetworkManager.singleton).spectatorCameras.Add(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }  // Dont let server run anything below

        ((FPSNetworkManager)NetworkManager.singleton).spectatorCameras.Remove(this); // Let all clients remove player list

        if (!hasAuthority) { return; }
    }

    void FixedUpdate()
    {
        float moveSpeedConstant = Time.deltaTime * 30f;

        float keyboardX = Input.GetAxisRaw("Horizontal") * moveSpeedConstant;
        float keyboardY = Input.GetAxisRaw("Vertical") * moveSpeedConstant;

        turn.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity * 600f * Time.deltaTime;
        turn.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 600f * Time.deltaTime;
        turn.y = Mathf.Clamp(turn.y, -90f, 90f);

        Vector3 finalDir = new Vector3(keyboardX, 0f, keyboardY);

        transform.Translate(finalDir, Space.Self);

        if (Input.GetKey(KeyCode.Space)) 
        {
            transform.Translate(Vector3.up * moveSpeedConstant, Space.World); 
        }
        if (Input.GetKey(KeyCode.LeftShift)) 
        { 
            transform.Translate(Vector3.down * moveSpeedConstant, Space.World); 
        }

        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0f);
    }
}
