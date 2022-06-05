using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 8;
    [SerializeField] private float swayMultiplier = 2;

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        // Calculate target rotation    (Negative because default inverted on the Y)
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // Rotate
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
