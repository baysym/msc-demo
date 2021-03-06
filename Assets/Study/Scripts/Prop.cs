using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;
using Ubiq.Samples;

public class Prop : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable, IGraspable
{
    private Hand follow;
    private NetworkContext ctx;
    bool owner = false;

    // set network ID to editor value
    public string netID;
    public NetworkId Id { get; set; }
    void Awake() { Id = new NetworkId(netID); }

    // receive messages and update location and colour
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        owner = false;  // recieved data therefore this client is not the owner
        var msg = message.FromJson<TransformMessage>();
        transform.localPosition = msg.position;
        transform.localRotation = msg.rotation;
    }

    // follow hand on grasp and stop on release
    public void Grasp(Hand controller)
    {
        follow = controller;
        owner = true;
    }
    public void Release(Hand controller)
    {
        follow = null;
        owner = false;
    }

    // initial set up
    void Start()
    {
        ctx = NetworkScene.Register(this);
    }

    // update visualisations and send messages
    void FixedUpdate()
    {
        if (owner)
        {
            // follow the owner's hand
            if (follow != null)
            {
                transform.position = follow.transform.position;
                transform.rotation = follow.transform.rotation;
            }

            ctx.SendJson(new TransformMessage(transform));
        }
    }

    // if spawned by a client, that client is the owner
    public void OnSpawned(bool local) { owner = local; }
}
