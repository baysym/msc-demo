using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;

namespace Ubiq.Samples
{

    public class MySpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public bool upArrow;

        void OnTriggerEnter()
        {
            NetworkSpawner.Spawn(this, Prefab);
            Destroy(gameObject);
        }

        void Update()
        {
            upArrow = Input.GetKey(KeyCode.UpArrow);
        }
    }
}