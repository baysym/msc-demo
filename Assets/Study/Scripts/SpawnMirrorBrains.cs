using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;
using Ubiq.Samples;
public class SpawnMirrorBrains : MonoBehaviour
{
    public GameObject Prefab;

    public void Update()
    {
        if (Input.GetKeyUp((KeyCode.Alpha1)))
            NetworkSpawner.SpawnPersistent(this, Prefab);
    }
}