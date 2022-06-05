using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;

public class Brain : MonoBehaviour, INetworkObject, INetworkComponent, IGraspable
{
    // networking
    private NetworkContext netcon;
    public string netID;
    public NetworkId Id { get; set; }
    void Awake() { Id = new NetworkId(netID); }

    // movement
    private Hand follow;

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
    public float activity = 0f;

    // visualisations
    [Header("Visualisations")]
    public bool isOod;
    private ParticleSystem realParticles;
    private ParticleSystem fakeParticles;
    private Transform head;
    bool owner = false;

    // 
    void Start()
    {
        netcon = NetworkScene.Register(this);
        rend = GetComponent<Renderer>();
        thisPlayer = GameObject.Find("Player");

        realParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        fakeParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    // 
    void FixedUpdate()
    {
        var rpe = realParticles.emission;
        rpe.rateOverTime = 0f;

        var fpe = fakeParticles.emission;
        float perlin = Mathf.PerlinNoise(0f, Time.time / 2f) * 10f;
        fpe.rateOverTime = isOod ? 0f : perlin;

        if (follow != null)
        {
            ProcessData();
            netcon.SendJson(new Message(activity, transform));
        }
    }

    // hand interactions
    public void Grasp(Hand controller)
    {
        follow = controller;
        thisPlayer.GetComponent<GetData>().brain = this;
        owner = true;
    }
    public void Release(Hand controller)
    {
        if (isOod)
        {
            follow = null;
            owner = false;
        }
    }

    // network updates
    public struct Message
    {
        public float a;
        public Transform t;
        
        public Message(float a, Transform t)
        {
            this.a = a;
            this.t = t;
        }
    }

    // receive network updates
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        owner = false;
        Message msg = message.FromJson<Message>();
        activity = msg.a;
        rend.material.color = new Color(msg.a, msg.a, msg.a);
        transform.position = msg.t.position;
        transform.rotation = msg.t.rotation;
    }

    // take in raw data, convert to relative value, pass to visualisations
    public void ProcessData()
    {
        if (dataString != "")
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
                activity = (mean / baseline) * 0.5f;
            }
        }
        else
        {
            activity = 0f;
        }        

        if (isOod) Ood();
        else Particles();
    }

    // change colour and disable particles
    void Ood()
    {
        rend.enabled = true;

        rend.material.color = new Color(activity, activity, activity);
        rend.material.SetColor("_EmissionColor", new Color(activity, activity, activity));

        transform.position = follow.transform.position;
        transform.rotation = follow.transform.rotation;
    }

    // change particle emission rate
    void Particles()
    {
        rend.enabled = false;

        var rpe = realParticles.emission;
        rpe.rateOverTime = activity * 10f;

        head = follow.transform.parent.GetChild(0).GetChild(0).transform.Find("Brain follow");
        transform.position = head.position;
        transform.rotation = head.rotation;
    }
}

