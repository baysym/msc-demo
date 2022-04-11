using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // 0: left wai text
        // 1: right wai text
        // 2: rps order
        // 3: rps timer
        // 4: 
        public string[] taskStatus = { "", "", "", "", "" };

        // who am i
        public Text leftWAIText;
        public Text rightWAIText;
        public Text leftTimer;
        public Text rightTimer;
        public TextAsset answerFile;
        public List<string> answers;

        // find my brain
        public Transform[] fmbPositions;
        public GameObject leftBrain;
        public GameObject rightBrain;

        // rock paper scissors
        public Text leftRPSText;
        public Text rightRPSText;
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

        public void Release(Hand hand) { return; }

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

            //owner = true;  // ######## demo only ########

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

                timer += Time.deltaTime;
                taskStatus[3] = timer.ToString();

                if (timer > 5f)
                {
                    ShuffleRPS();
                    timer = 0f;
                }

                netcon.SendJson(new Message(taskStatus));
            }
            else
            {
                if (taskStatus[3] != "")
                    timer = float.Parse(taskStatus[3]);
            }

            for (int i = 0; i < 6; i++)
            {
                // show spheres for three seconds then show models until reset
                rpsProps[i].transform.GetChild(0).gameObject.SetActive(timer < 3f);
                rpsProps[i].transform.GetChild(1).gameObject.SetActive(timer > 3f);
            }

            leftWAIText.text = taskStatus[0];
            rightWAIText.text = taskStatus[1];

            if (taskStatus[2] != "")
            {
                DisplayRPS();
                ArrangeRPS();
            }
        }

        // -------- who am i --------

        void LeftDescribes()
        {
            taskStatus[0] = "describe " + answers[Random.Range(0, answers.Count)];
            taskStatus[1] = "guess";
        }

        void RightDescribes()
        {
            taskStatus[0] = "guess";
            taskStatus[1] = "describe " + answers[Random.Range(0, answers.Count)];
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
        }

        void DisplayRPS()
        {
            /*string order = "";
            for (int i = 0; i < 6; i++)
                order += "RPS"[int.Parse(taskStatus[2][i].ToString())];

            leftRPSText.text = order.Substring(0, 3);
            rightRPSText.text = order.Substring(3);*/

            leftRPSText.text = taskStatus[2].Substring(3);
            rightRPSText.text = taskStatus[2].Substring(0, 3);
        }

        void ArrangeRPS()
        {
            /*char[] rps = { 'R', 'P', 'S' };
            string order = "";
            for (int i = 0; i < 6; i++)
                order += System.Array.IndexOf(rps, taskStatus[2][i]);

            leftRPS = order.Substring(0, 3);
            rightRPS = order.Substring(3);

            for (int i = 0; i < 6; i++)
            {
                if (i < 3)
                    rpsProps[i].transform.position = rpsLocations[int.Parse(rightRPS[i].ToString())].position;
                else
                    rpsProps[i].transform.position = rpsLocations[int.Parse(leftRPS[i - 3].ToString()) + 3].position;
            }*/

            for (int i = 0; i < 3; i++)
            {
                string order = taskStatus[2];
                rpsProps[i].transform.position = rpsLocations[int.Parse(order[i].ToString())].position;
                rpsProps[i + 3].transform.position = rpsLocations[int.Parse(order[i + 3].ToString()) + 3].position;
            }
        }
    }
}
