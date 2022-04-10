using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockPaperScissors : MonoBehaviour
{
    // props
    public GameObject rock1;
    public GameObject paper1;
    public GameObject scissors1;
    public GameObject rock2;
    public GameObject paper2;
    public GameObject scissors2;
    // spheres
    public GameObject rockSphere1;
    public GameObject paperSphere1;
    public GameObject scissorsSphere1;
    public GameObject rockSphere2;
    public GameObject paperSphere2;
    public GameObject scissorsSphere2;
    // models
    public GameObject rockModel1;
    public GameObject paperModel1;
    public GameObject scissorsModel1;
    public GameObject rockModel2;
    public GameObject paperModel2;
    public GameObject scissorsModel2;

    // locations
    public Transform[] propLocs;

    // text
    public Text[] RPSTexts;
    public Text[] timerTexts;
    public string[] letters = { "R", "P", "S", "R", "P", "S" };

    // timer
    float timer = 0f;

    //
    void Start()
    {
        HideModels();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        string rounded = Mathf.Round(3 - timer).ToString();

        if (rounded == "3" || rounded == "2" || rounded == "1")
        {
            timerTexts[0].text = rounded + "\n\n";
            timerTexts[1].text = rounded + "\n\n";
        }

        // reset the timer every 5 seconds
        if (timer > 5f)
        {
            Shuffle();
            timer = -0.49f;
            HideModels();
        }

        // hide the spheres and show the models
        if (timer > 2.5f)
        {
            ShowModels();
            timerTexts[0].text = "GO!\n\n";
            timerTexts[1].text = "GO!\n\n";
        }
    }

    // shuffle props
    void Shuffle()
    {
        int[] order1 = { 0, 1, 2 };

        for (int a = 0; a < 3; a++)
        {
            for (int b = 0; b < 3; b++)
            {
                int r = Random.Range(0, 3);
                int t = order1[a];
                order1[a] = order1[r];
                order1[r] = t;
            }
        }

        rock1.transform.position = propLocs[order1[0]].position;
        paper1.transform.position = propLocs[order1[1]].position;
        scissors1.transform.position = propLocs[order1[2]].position;

        int[] order2 = { 3, 4, 5 };

        for (int a = 0; a < 3; a++)
        {
            for (int b = 0; b < 3; b++)
            {
                int r = Random.Range(0, 3);
                int t = order2[a];
                order2[a] = order2[r];
                order2[r] = t;
            }
        }

        rock2.transform.position = propLocs[order2[0]].position;
        paper2.transform.position = propLocs[order2[1]].position;
        scissors2.transform.position = propLocs[order2[2]].position;

        RPSTexts[0].text = "\n" + letters[order1[0]] + "          " + letters[order1[1]] + "          " + letters[order1[2]];
        RPSTexts[1].text = "\n" + letters[order2[0]] + "          " + letters[order2[1]] + "          " + letters[order2[2]];
    }

    //
    void HideModels()
    {
        // spheres
        rockSphere1.SetActive(true);
        paperSphere1.SetActive(true);
        scissorsSphere1.SetActive(true);
        rockSphere2.SetActive(true);
        paperSphere2.SetActive(true);
        scissorsSphere2.SetActive(true);
        // models
        rockModel1.SetActive(false);
        paperModel1.SetActive(false);
        scissorsModel1.SetActive(false);
        rockModel2.SetActive(false);
        paperModel2.SetActive(false);
        scissorsModel2.SetActive(false);
    }

    //
    void ShowModels()
    {
        // spheres
        rockSphere1.SetActive(false);
        paperSphere1.SetActive(false);
        scissorsSphere1.SetActive(false);
        rockSphere2.SetActive(false);
        paperSphere2.SetActive(false);
        scissorsSphere2.SetActive(false);
        // models
        rockModel1.SetActive(true);
        paperModel1.SetActive(true);
        scissorsModel1.SetActive(true);
        rockModel2.SetActive(true);
        paperModel2.SetActive(true);
        scissorsModel2.SetActive(true);
    }
}
