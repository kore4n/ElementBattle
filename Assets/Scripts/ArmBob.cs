using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmBob : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float bobStrength = 1f;
    [SerializeField] private float speed = 2;

    private const float baseStrength = 0.01f;
    private Vector3 basePos;

    private void Awake()
    {
        basePos = transform.localPosition;
    }

    void Update()
    {
        BobUpDown();
    }

    void BobUpDown()
    {
        transform.localPosition = basePos + new Vector3(0f, Mathf.Sin(Time.time * speed) * bobStrength * baseStrength, 0f);
    }
}
