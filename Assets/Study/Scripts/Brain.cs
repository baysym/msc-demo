using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;

namespace Ubiq.Samples
{
    public class Brain : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable, IGraspable
    {
        private Hand follow;
        private NetworkContext ctx;
        Material material;
        bool owner = false;
        private Rigidbody body;
        Renderer rend;

        GameObject thisPlayer;  // this client's player
        public string dataString;  // raw data receieved from OxySoft
        float[] dataValues;  // current brain data values
        float baseline;  // measured baseline value
        public float value;  // comparative value to visualise
        float sampleTimeElapsed = 0f;  // how long baseline data has been sampled for
        float sampleTimePeriod = 5f;  // seconds to sample data for baseline
        float sampleSum = 0f;  // sum of all baseline values
        int sampleCount = 0;  // how many baseline values were sampled

        // visualisations
        public bool ood;
        public bool waves;
        GameObject psObj;
        ParticleSystem ps;



        // set network ID to editor value
        public string netID;
        public NetworkId Id { get; set; }
        //public NetworkId Id { get; set; } = NetworkId.Unique();
        void Awake() { Id = new NetworkId(netID); }



        // message structure
        public struct Message
        {
            public TransformMessage transform;
            public Color colour;

            public Message(Transform transform, Color colour)
            {
                this.transform = new TransformMessage(transform);
                this.colour = colour;
            }
        }



        // receive messages and update location and colour
        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            owner = false;  // recieved data therefore this client is not the owner
            var msg = message.FromJson<Message>();            
            transform.localPosition = msg.transform.position;
            transform.localRotation = msg.transform.rotation;
            material.color = msg.colour;
        }



        // follow hand on grasp and stop on release
        public void Grasp(Hand controller) {
            follow = controller;
            //thisPlayer.GetComponent<GetData>().brain = this;
            owner = true;
            sampleTimeElapsed = 0f;
        }
        public void Release(Hand controller) {
            if (!waves)
            {
                follow = null;
                owner = false;
            }
        }



        // initial set up
        void Start()
        {
            ctx = NetworkScene.Register(this);
            body = GetComponent<Rigidbody>();
            rend = GetComponent<Renderer>();
            material = GetComponent<Renderer>().material;
            thisPlayer = GameObject.Find("Player");

            psObj = transform.GetChild(0).gameObject;
            ps = psObj.GetComponent<ParticleSystem>();
        }



        // update visualisations and send messages
        void FixedUpdate()
        {
            if (owner)
            {
                Visualise();
                Move();

                ctx.SendJson(new Message(transform, material.color));
            }

            if (waves)
                psObj.SetActive(true);
            else
                psObj.SetActive(false);
        }



        // move the visualisation
        void Move()
        {
            if (follow != null)
            {
                // follow the player's hand
                if (ood)
                {
                    transform.position = follow.transform.position;
                    transform.rotation = follow.transform.rotation;
                }
                // follow inside player's head
                else if (waves)
                {
                    this.transform.parent = thisPlayer.transform;
                    transform.localPosition = new Vector3(0f, 1.4f, 0.05f);
                }
                // follow above player's head
                else
                {
                    this.transform.parent = thisPlayer.transform;
                    transform.localPosition = new Vector3(0f, 1.7f, 0.1f);
                }
            }
        }



        // get baseline and trigger visualisations
        void Visualise()
        {
            dataValues = ParseData();  // get data points as floats

            // get the mean data point value
            float mean = 0f;
            for (int i = 0; i < dataValues.Length; i++)
                mean += dataValues[i] / 16f;

            // sample input data for baseline value
            if (sampleTimeElapsed < sampleTimePeriod)
            {
                sampleTimeElapsed += Time.deltaTime;
                sampleSum += mean;
                baseline = sampleSum / ++sampleCount;
            }
            // compare brain data to baseline
            else
            {
                value = mean / baseline;  // compare input to baseline
                value *= 0.5f;

                if (ood)
                    VisualiseOod(value);

                if (waves)
                    VisualiseWaves(value);
            }
        }



        // ood: value mapped to a white-to-black gradient
        void VisualiseOod(float value)
        {
            rend.material.color = new Color(value, value, value);
            rend.material.SetColor("_EmissionColor", new Color(value, value, value));
        }



        // waves: value mapped to particle emission rate
        void VisualiseWaves(float value)
        {
            var emission = ps.emission;
            emission.rateOverTime = value * 3f;
        }



        // convert the input data from a string to usable numbers
        float[] ParseData()
        {
            try
            {
                string[] stringValues = dataString.Split(' ');
                float[] floatValues = new float[stringValues.Length - 1];

                // skip the id then parse data points to floats
                for (int i = 1; i < floatValues.Length; i++)
                    floatValues[i] = float.Parse(stringValues[i].Replace(",", ""));

                return floatValues;
            }
            catch
            {
                Debug.Log("Parsing failed");
                return new float[] { 0f };
            }
        }



        // if spawned by a client, that client is the owner
        public void OnSpawned(bool local) { owner = local; }
    }
}
