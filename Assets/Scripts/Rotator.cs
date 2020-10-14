using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 speed = Vector3.zero;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, Space.World);
    }
}
