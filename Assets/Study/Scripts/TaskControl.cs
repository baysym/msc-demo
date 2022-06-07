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

    // brains
    [Header("Brains")]
    public GameObject brainPrefab;
    public GameObject[] brains;

    // table controllers
    private CyanControl cyan;
    private MagentaControl magenta;
    private YellowControl yellow;

    // cyan table
    private int animal;

    // magenta table
    public Transform[] tablePositions;
    public bool brainsOnTable;

    // yellow table
    private string order;

    //
    void Start()
    {
        netcon = NetworkScene.Register(this);
        cyan = GameObject.Find("Cyan table").GetComponent<CyanControl>();
        magenta = GameObject.Find("Magenta table").GetComponent<MagentaControl>();
        yellow = GameObject.Find("Yellow table").GetComponent<YellowControl>();
        brains = GameObject.FindGameObjectsWithTag("Brain");
    }

    //
    void Update()
    {
        // SPACE - spawn brains
        if (Input.GetKeyUp(KeyCode.Space))
        {
            brains = GameObject.FindGameObjectsWithTag("Brain");
            int brainCount = brains.Length;

            if (brainCount < 2)
                NetworkSpawner.SpawnPersistent(this, brainPrefab);
        }

        // 1 - all brains to ood
        if (Input.GetKeyUp(KeyCode.Alpha1))
            foreach (GameObject brain in brains)
                brain.GetComponent<Brain>().isOod = true;
        // 2 - all brains to particles
        if (Input.GetKeyUp(KeyCode.Alpha2))
            foreach (GameObject brain in brains)
                brain.GetComponent<Brain>().isOod = false;

        // F1 - new left text
        if (Input.GetKeyUp(KeyCode.F1))
            SetText(true);
        // F2 - new right text
        if (Input.GetKeyUp(KeyCode.F2))
            SetText(false);

        // F5 - lock ood brains to table positions
        if (Input.GetKeyUp(KeyCode.F5))
            BrainTable(true);
        // F6 - unlock brain positions
        if (Input.GetKeyUp(KeyCode.F6))
            BrainTable(false);

        // F9 - new round of rock paper scissors
        if (Input.GetKeyUp(KeyCode.F9))
            NewRound();
    }

    // cyan
    void SetText(bool showLeft, string input = "")
    {
        animal = input == "" ? Random.Range(0, 20) : int.Parse(input);
        cyan.SetText(showLeft, animal);

        if (input == "")
            SendMessage(1, showLeft, animal.ToString());
    }

    // magenta
    void BrainTable(bool status, string input = "")
    {
        foreach (GameObject brain in brains)
            brain.GetComponent<Brain>().onTable = status;

        if (input == "")
            SendMessage(2, status, "x");
    }

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
            SendMessage(3, false, order);
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
                BrainTable(argBool);
                break;

            case 3:
                NewRound(argString);
                break;

            default:
                break;
        }
    }
}