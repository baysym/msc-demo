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

        // find my brain
        //

        // rock paper scissors


        void Start()
        {
            Id = new NetworkId(netId);
            netcon = NetworkScene.Register(this);
            rb = GetComponent<Rigidbody>();
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

            // apply to tasks
            leftText.text = taskStatus[0];
            rightText.text = taskStatus[1];
        }

        // -------- who am i logic --------

        void LeftDescribes()
        {
            taskStatus[0] = "describe " + Random.Range(0, 100).ToString();
            taskStatus[1] = "guess";
        }

        void RightDescribes()
        {
            taskStatus[0] = "guess";
            taskStatus[1] = "describe " + Random.Range(0, 100).ToString();
        }

        // -------- find my brain logic --------

        void StartFMB()
        {
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
            taskStatus[2] = options[Random.Range(0, 6)] + options[Random.Range(0, 6)];
            ArrangeRPS();
        }

        void ArrangeRPS()
        {

        }
    }
}
