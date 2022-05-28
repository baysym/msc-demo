using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;

public class TaskControl : MonoBehaviour, INetworkObject, INetworkComponent
{
    // networking
    [Header("Ubiq")]
    public string netID;
    public NetworkId Id { get; set; }
    private NetworkContext netcon;

    // control activity
    /*[Header("Magenta table")]
    [Space(20)]*/

    // brains
    [Header("Brains")]
    public GameObject brainPrefab;
    public GameObject leftBrain;
    public GameObject rightBrain;

    // table controllers
    private CyanControl cyan;
    private MagentaControl magenta;
    private YellowControl yellow;

    // cyan table
    private int animal;

    // yellow table
    private string order;

    //
    void Start()
    {
        Id = new NetworkId(netID);
        netcon = NetworkScene.Register(this);
        cyan = GameObject.Find("Cyan table").GetComponent<CyanControl>();
        magenta = GameObject.Find("Magenta table").GetComponent<MagentaControl>();
        yellow = GameObject.Find("Yellow table").GetComponent<YellowControl>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            int brainCount = GameObject.FindGameObjectsWithTag("Brain").Length;

            if (brainCount < 2)
            {
                NetworkSpawner.SpawnPersistent(this, brainPrefab);
            }
            else
            {
                leftBrain = GameObject.FindGameObjectsWithTag("Brain")[0];
                rightBrain = GameObject.FindGameObjectsWithTag("Brain")[1];
            }
        }

        // change visualisations
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            leftBrain.GetComponent<NewBrain>().isOod = true;
            rightBrain.GetComponent<NewBrain>().isOod = true;
            leftBrain.GetComponent<NewBrain>().isParticles = false;
            rightBrain.GetComponent<NewBrain>().isParticles = false;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            leftBrain.GetComponent<NewBrain>().isOod = false;
            rightBrain.GetComponent<NewBrain>().isOod = false;
            leftBrain.GetComponent<NewBrain>().isParticles = true;
            rightBrain.GetComponent<NewBrain>().isParticles = true;
        }

        // cyan
        // new left text
        if (Input.GetKeyUp(KeyCode.F1))
            SetText(true);
        // new right text
        if (Input.GetKeyUp(KeyCode.F2))
            SetText(false);

        // magenta
        //

        // yellow
        // new round of rock paper scissors
        if (Input.GetKeyUp(KeyCode.F9))
            NewRound();
    }

    //

    // cyan
    void SetText(bool showLeft, string input = "")
    {
        animal = input == "" ? Random.Range(0, 20) : int.Parse(input);
        cyan.SetText(showLeft, animal);

        if (input == "")
        {
            SendMessage(1, showLeft, animal.ToString());
            Debug.Log("SetText sent a message");
        }
    }

    // magenta
    //

    // yellow
    void NewRound(string input = "")
    {
        // generate a random order for the props
        char[] left = { 'R', 'P', 'S' };
        char[] right = { 'R', 'P', 'S' };
        char tempL;
        char tempR;

        for (int i = 0; i < 3; i++)
        {
            int rndL = Random.Range(0, 3);
            tempL = left[i];
            left[i] = left[rndL];
            left[rndL] = tempL;

            int rndR = Random.Range(0, 3);
            tempR = right[i];
            right[i] = right[rndR];
            right[rndR] = tempR;
        }

        order = input == "" ? new string(left) + new string(right) : input;

        yellow.NewRound(order);

        if (input == "")
        {
            SendMessage(3, false, order);
            Debug.Log("NewRound sent a message");
        }
    }

    //

    public struct Message
    {
        public int method;
        public bool argBool;
        public string argString;

        public Message(int method, bool argBool, string argString)
        {
            this.method = method;
            this.argBool = argBool;
            this.argString = argString;
        }
    }

    void SendMessage(int method, bool argBool, string argString)
    {
        netcon.SendJson(new Message(method, argBool, argString));
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        Debug.Log("Message received");
        Message msg = message.FromJson<Message>();
        MessageMethod(msg.method, msg.argBool, msg.argString);
    }

    void MessageMethod(int method, bool argBool, string argString)
    {
        switch (method)
        {
            case 1:
                SetText(argBool, argString);
                break;

            case 2:
                break;

            case 3:
                NewRound(argString);
                break;

            default:
                break;
        }
    }
}
