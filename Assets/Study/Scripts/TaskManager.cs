using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;

public class TaskManager : MonoBehaviour, INetworkObject, INetworkComponent
{
    // networking
    [Header("Ubiq")]
    public string netID;
    public NetworkId Id { get; set; } = NetworkId.Unique();
    private NetworkContext netcon;

    // method sending
    [Header("Method sending")]
    public int method;
    public string arg;
    public bool send = false;

    // conversation
    [Header("Cyan table")]
    public string[] topics = { "Topic A", "Topic B", "Topic C", "Topic D", "Topic E", "Topic F" };
    public Text leftTopic;
    public Text rightTopic;

    // control activity
    /*[Header("Magenta table")]
    [Space(20)]*/

    // rock paper scissors
    [Header("Yellow table")]
    // text objects
    public Text leftOrder;
    public Text rightOrder;
    public Text leftTimer;
    public Text rightTimer;
    // props and locations
    public GameObject[] leftProps;
    public GameObject[] rightProps;
    public Transform[] leftLocations;
    public Transform[] rightLocations;

    // brains
    [Header("Brains")]
    public GameObject brainPrefab;
    public GameObject leftBrain;
    public GameObject rightBrain;

    // testing
    [Header("DEBUG")]
    public bool editor = false;


    void Start() {
        netcon = NetworkScene.Register(this);
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

        // conversation
        if (Input.GetKeyUp(KeyCode.F1))
            GenConvoTopic();

        // rock paper scissors
        if (Input.GetKeyUp(KeyCode.F9))
            GenRPSOrder();
        if (Input.GetKeyUp(KeyCode.F10))
            GenRPSVisibility("prop");
        if (Input.GetKeyUp(KeyCode.F11))
            GenRPSVisibility("sphere");

        // send and execute the method with its argument
        if (send)
        {
            netcon.SendJson(new Message(method, arg));
            MessageMethod(method, arg);
            send = false;
        }
    }
    
    public struct Message
    {
        public int method;
        public string arg;

        public Message(int method, string arg)
        {
            this.method = method;
            this.arg = arg;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        Message msg = message.FromJson<Message>();
        MessageMethod(msg.method, msg.arg);
    }

    void MessageMethod(int method, string arg)
    {
        switch (method)
        {
            case 1:
                SetConvoTopic(arg);
                break;

            case 9:
                SetRPSOrder(arg);
                break;

            case 10:
                SetRPSVisibility(arg);
                break;

            default:
                break;
        }
    }

    // generate a conversation topic
    void GenConvoTopic()
    {
        method = 1;
        arg = topics[Random.Range(0, topics.Length)];
        send = true;
    }

    // display a conversation topic
    void SetConvoTopic(string arg)
    {
        leftTopic.text = arg;
        rightTopic.text = arg;
    }

    // generate rock paper scissors order
    void GenRPSOrder()
    {
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

        // send message
        method = 9;
        arg = new string(left) + new string(right);
        send = true;
    }

    // set rock paper scissors order
    void SetRPSOrder(string arg)
    {
        // arg as strings for left and right
        char[] left = arg.Substring(0, 3).ToCharArray();
        char[] right = arg.Substring(3).ToCharArray();

        // display prop order
        string spaces = "          ";
        leftOrder.text = $"\n{left[0]}{spaces}{left[1]}{spaces}{left[2]}";
        rightOrder.text = $"\n{right[0]}{spaces}{right[1]}{spaces}{right[2]}";

        // arrange props
        char[] rps = { 'R', 'P', 'S' };
        for (int i = 0; i < 3; i++)
        {
            leftProps[i].transform.position = leftLocations[System.Array.IndexOf(left, rps[i])].position;
            rightProps[i].transform.position = rightLocations[System.Array.IndexOf(right, rps[i])].position;
        }
    }

    // generate rock paper scissors prop visibility
    void GenRPSVisibility(string show)
    {
        method = 10;
        arg = show;
        send = true;
    }

    // set rock paper scissors prop visibility
    void SetRPSVisibility(string arg)
    {
        int show = (arg == "prop") ? 1 : 0;

        for (int i = 0; i < 3; i++)
        {
            leftProps[i].transform.GetChild(show).gameObject.SetActive(true);
            leftProps[i].transform.GetChild(1 - show).gameObject.SetActive(false);
            rightProps[i].transform.GetChild(show).gameObject.SetActive(true);
            rightProps[i].transform.GetChild(1 - show).gameObject.SetActive(false);
        }
    }
}
