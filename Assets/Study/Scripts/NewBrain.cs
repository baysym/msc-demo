using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;

public class NewBrain : MonoBehaviour, INetworkObject, INetworkComponent, IGraspable
{
    // networking
    private NetworkContext netcon;
    public NetworkId Id { get; set; } = NetworkId.Unique();
    /*public string netID;
    public NetworkId Id { get; set; }
    void Awake() { Id = new NetworkId(netID); }*/

    // movement
    private Hand follow;
    bool owner = false;

    // prerequisites
    GameObject thisPlayer;
    Renderer rend;
    Material mat;

    // data processing
    [Header("Data")]
    public string dataString;
    float mean;
    float sampleTime = 0f;
    float samplePeriod = 3f;
    float sampleSum = 0f;
    int sampleCount = 0;
    float baseline;
    public float value = 0f;

    // visualisations
    [Header("Visualisations")]
    public bool isOod;
    public bool isParticles;

    // 
    void Start()
    {
        netcon = NetworkScene.Register(this);
        rend = GetComponent<Renderer>();
        thisPlayer = GameObject.Find("Player");

        int brainCount = GameObject.FindGameObjectsWithTag("Brain").Length;
        if (brainCount == 1)
            transform.position = new Vector3(-0.25f, 0.6f, 1f);
        else
            transform.position = new Vector3(0.25f, 0.6f, 1f);
    }

    // 
    void FixedUpdate()
    {
        if (owner)
        {
            if (dataString != "")
                ProcessData();

            if (follow != null)
            {
                transform.position = follow.transform.position;
                transform.rotation = follow.transform.rotation;
            }

            netcon.SendJson(new Message(rend.material.color, transform));
        }
    }

    // hand interactions
    public void Grasp(Hand controller)
    {
        owner = true;
        follow = controller;
        thisPlayer.GetComponent<GetData>().brain = this;
    }
    public void Release(Hand controller)
    {
        follow = null;
    }

    // network updates
    public struct Message
    {
        public Color c;
        public Transform t;
        
        public Message(Color c, Transform t) {
            this.c = c;
            this.t = t;
        }
    }

    // receive network updates
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        Message msg = message.FromJson<Message>();
        rend.material.color = msg.c;
        transform.position = msg.t.position;
        transform.rotation = msg.t.rotation;
        Debug.Log(msg.t.position);
    }

    // 
    public void ProcessData()
    {
        string[] stringValues = dataString.Split(' ');
        float[] floatValues = new float[16];

        mean = 0f;
        for (int i = 1; i < floatValues.Length; i++)
            mean += float.Parse(stringValues[i].Replace(",", "")) / 16f;

        if (sampleTime < samplePeriod)
        {
            sampleTime += Time.deltaTime;
            sampleSum += mean;
            baseline = sampleSum / ++sampleCount;
        }
        else
        {
            value = (mean / baseline) * 0.5f;
        }

        if (isOod)
            Ood();
        else if (isParticles)
            Particles();
    }

    //
    void Ood()
    {
        rend.material.color = new Color(value, value, value);
        rend.material.SetColor("_EmissionColor", new Color(value, value, value));
    }

    // 
    void Particles()
    {

    }
}
