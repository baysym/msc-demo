using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WhoAmI : MonoBehaviour
{
    public Text[] displays;
    public TextAsset file;
    public List<string> answers;
    float timer;
    int thisPlayer = 0;
    int lastPlayer = 0;

    void Start()
    {
        answers = file.text.Split('\n').ToList();

        for (int i = 0; i < answers.Count; i++)
        {
            int rnd = Random.Range(0, answers.Count - 1);
            string temp = answers[rnd];
            answers[rnd] = answers[i];
            answers[i] = temp;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 5f)
        {
            timer = 0f;
            thisPlayer = 1 - thisPlayer;
        }

        if (thisPlayer != lastPlayer)
        {
            lastPlayer = thisPlayer;
            if (thisPlayer == 1)
                aDescribes();
            else
                bDescribes();
        }
    }

    public void aDescribes()
    {
        displays[1].text = PickAnswer();
        displays[0].text = "...";
    }

    public void bDescribes()
    {
        displays[0].text = PickAnswer();
        displays[1].text = "...";
    }

    string PickAnswer()
    {
        string chosen = answers[answers.Count - 1];
        answers.Remove(chosen);
        return chosen;
    }
}
