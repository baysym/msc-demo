using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YellowControl : MonoBehaviour
{
    [Header("Timers")]
    public float timer = 0f;
    public Text leftTimer;
    public Text rightTimer;

    [Header("Orders")]
    public Text leftOrder;
    public Text rightOrder;

    [Header("Props")]
    public GameObject[] leftProps;
    public GameObject[] rightProps;
    public Transform[] leftLocations;
    public Transform[] rightLocations;

    void Update()
    {
        if (timer >= 2f)
        {
            // show timers
            leftTimer.text =  Mathf.Ceil(timer - 2f).ToString() + "\n";
            rightTimer.text =  Mathf.Ceil(timer - 2f).ToString() + "\n";

            // hide props
            for (int i = 0; i < 3; i++)
            {
                leftProps[i].transform.GetChild(0).gameObject.SetActive(true);
                leftProps[i].transform.GetChild(1).gameObject.SetActive(false);
                rightProps[i].transform.GetChild(0).gameObject.SetActive(true);
                rightProps[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            // show timers
            leftTimer.text = "";
            rightTimer.text = "";

            // show props
            for (int i = 0; i < 3; i++)
            {
                leftProps[i].transform.GetChild(0).gameObject.SetActive(false);
                leftProps[i].transform.GetChild(1).gameObject.SetActive(true);
                rightProps[i].transform.GetChild(0).gameObject.SetActive(false);
                rightProps[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        // decrease timer
        timer -= Time.deltaTime;
    }

    public void NewRound(string order)
    {
        // reset timer
        timer = 5f;

        // show orders
        string left = order.Substring(0, 3);
        string right = order.Substring(3, 3);
        string spaces = "          ";
        leftOrder.text = $"\n{left[0]}{spaces}{left[1]}{spaces}{left[2]}";
        rightOrder.text = $"\n{right[0]}{spaces}{right[1]}{spaces}{right[2]}";
        
        // arrange props
        char[] rps = { 'R', 'P', 'S' };
        for (int i = 0; i < 3; i++)
        {
            leftProps[i].transform.position = leftLocations[System.Array.IndexOf(left.ToCharArray(), rps[i])].position;
            rightProps[i].transform.position = rightLocations[System.Array.IndexOf(right.ToCharArray(), rps[i])].position;
        }
    }
}
