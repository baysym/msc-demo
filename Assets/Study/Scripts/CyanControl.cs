using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CyanControl : MonoBehaviour
{
    public Text leftText;
    public Text rightText;

    public string[] animals =
    {
        "Dog",
        "Cat",
        "Goat",
        "Pig",
        "Cow",
        "Sheep",
        "Chicken",
        "Pigeon",
        "Elephant",
        "Giraffe",
        "Camel",
        "Whale",
        "Shark",
        "Bee",
        "Fly",
        "Rabbit",
        "Hedgehog",
        "Horse",
        "Tiger",
        "Lion"
    };

    public void SetText(bool showLeft, int random)
    {
        if (showLeft)
        {
            leftText.text = animals[random];
            rightText.text = "";
        }
        else
        {
            leftText.text = "";
            rightText.text = animals[random];
        }
    }
}
