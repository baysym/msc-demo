using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;
using UnityEngine.UI;

namespace Ubiq.Samples
{
    public class TaskNet : MonoBehaviour, IGraspable, INetworkObject, INetworkComponent, ISpawnable
    {
        // networking
        private Hand follow;
        private Rigidbody rb;
        private NetworkContext netcon;
        public string netId;
        public NetworkId Id { get; set; }
        public bool owner = false;

        // tasks
        // 0: left text
        // 1: right text
        // 2: rps order
        public string[] taskStatus = { "", "", "" };

        // who am i
        public Text leftText;
        public Text rightText;
        public TextAsset answerFile;
        public List<string> answers;

        // find my brain
        public Transform[] fmbPositions;
        public GameObject leftBrain;
        public GameObject rightBrain;

        // rock paper scissors
        private string leftRPS = "012";
        private string rightRPS = "012";
        public GameObject[] rpsProps;
        public Transform[] rpsLocations;
        private float timer;


        void Start()
        {
            Id = new NetworkId(netId);
            netcon = NetworkScene.Register(this);
            rb = GetComponent<Rigidbody>();

            answers = answerFile.text.Split('\n').ToList();
            for (int i = 0; i < answers.Count; i++)
            {
                int rnd = Random.Range(0, answers.Count - 1);
                string temp = answers[rnd];
                answers[rnd] = answers[i];
                answers[i] = temp;
            }
        }

        public void OnSpawned(bool local) { owner = local; }

        public void Grasp(Hand hand) { follow = hand; }

        public void Release(Hand hand) { follow = null; }

        public struct Message
        {
            public string[] taskStatus;

            public Message(string[] taskStatus)
            {
                this.taskStatus = taskStatus;
            }
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            owner = false;
            var state = message.FromJson<Message>();
            taskStatus = state.taskStatus;
        }

        // -------- function key task control --------

        void Update()
        {
            owner = (follow != null);

            // function key input
            if (owner)
            {
                // who am i
                if (Input.GetKeyUp(KeyCode.F1))
                    LeftDescribes();
                if (Input.GetKeyUp(KeyCode.F2))
                    RightDescribes();

                // find my brain
                if (Input.GetKeyUp(KeyCode.F5))
                    StartFMB();
                if (Input.GetKeyUp(KeyCode.F6))
                    StopFMB();

                // rock paper scissors
                if (Input.GetKeyUp(KeyCode.F9))
                    ShuffleRPS();

                netcon.SendJson(new Message(taskStatus));
            }

            timer += Time.deltaTime;
            if (timer > 5f)
            {
                for (int i = 0; i < 6; i++) {
                    // show spheres for three seconds then show models until reset
                    rpsProps[i].GetChild(0).SetActive(timer < 3f);
                    rpsProps[i].GetChild(1).SetActive(timer > 3f);
                }

                ShuffleRPS();
                timer = 0f;
            }

            // apply to tasks
            leftText.text = taskStatus[0];
            rightText.text = taskStatus[1];
        }

        // -------- who am i --------

        void LeftDescribes()
        {
            taskStatus[0] = "describe " + answers[Random.Range(0, answers.length)];
            taskStatus[1] = "guess";
        }

        void RightDescribes()
        {
            taskStatus[0] = "guess";
            taskStatus[1] = "describe " + answers[Random.Range(0, answers.length)];
        }

        // -------- find my brain --------

        void StartFMB()
        {
            /*int pos = Random.Range(0, 2);
            leftBrain.transform.position = fmbPositions[pos];
            rightBrain.transform.position = fmbPositions[1 - pos];*/

            return;
        }

        void StopFMB()
        {
            return;
        }

        // -------- rock paper scissors --------

        void ShuffleRPS()
        {
            string[] options = { "012", "021", "102", "120", "201", "210" };
            leftRPS = options[Random.Range(0, 6)];
            rightRPS = options[Random.Range(0, 6)];
            taskStatus[2] = leftRPS + rightRPS;
            DisplayRPS();
            ArrangeRPS();
        }

        void DisplayRPS()
        {
            string order = "";
            for (int i = 0; i < 6; i++)
                order += "RPS"[taskStatus[2][i]];
            Debug.Log(order);

            leftText.text = order.Substring(0, 3);
            rightText.text = order.Substring(3);
        }

        void ArrangeRPS()
        {
            for (int i = 0; i < 6; i++)
            {
                if (i < 3)
                    rpsProps[i].transform.position = rpsLocations[leftRPS[i]];
                else
                    rpsProps[i].transform.position = rpsLocations[rightRPS[i - 3]];
            }
        }
    }
}
