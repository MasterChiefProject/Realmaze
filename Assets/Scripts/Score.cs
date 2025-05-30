using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    void Update()
    {
        scoreText.text = "Score: " + Globals.points;
    }
}
