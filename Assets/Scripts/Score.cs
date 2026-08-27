using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;

    private int displayedScore = int.MinValue;

    private void OnEnable()
    {
        RefreshScore(force: true);
    }

    private void Update()
    {
        RefreshScore(force: false);
    }

    private void RefreshScore(bool force)
    {
        if (!scoreText)
        {
            return;
        }

        int currentScore = Globals.points;

        if (!force && currentScore == displayedScore)
        {
            return;
        }

        displayedScore = currentScore;
        scoreText.text = $"Score: {displayedScore}";
    }
}
