using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;

public class NewBrain : MonoBehaviour, INetworkObject, INetworkComponent, IGraspable
{
    // networking
    [Header("Ubiq")]
    private NetworkContext netcon;
    public string netId;
    public NetworkId Id { get; set; }
    void Awake() { Id = new NetworkId(netId); }

    // prerequisites
    GameObject thisPlayer;
    Renderer rend;
    Material mat;

    // data processing
    public string dataString;
    float mean;
    float sampleTime = 0f;
    float samplePeriod = 3f;
    float sampleSum = 0f;
    int sampleCount = 0;
    float baseline;
    public float value = 0f;

    // visualisations
    public bool isOod;
    public bool isParticles;

    // 
    void Start()
    {
        netcon = NetworkScene.Register(this);
        rend = GetComponent<Renderer>();
        thisPlayer = GameObject.Find("Player");
    }

    // 
    void FixedUpdate()
    {
        netcon.SendJson(new Message(rend.material.color));

        if (dataString != "")
            ProcessData();
    }

    // hand interactions
    public void Grasp(Hand controller)
    {
        thisPlayer.GetComponent<GetData>().brain = this;
    }
    public void Release(Hand controller) { return; }

    // send network updates
    public struct Message
    {
        public Color color;
        public Message(Color color) { this.color = color; }
    }

    // receive network updates
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        Message msg = message.FromJson<Message>();
        rend.material.color = msg.color;
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
