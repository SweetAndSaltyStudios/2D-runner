using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singelton<UIManager>
{
    private Text currentScoreText;
    private Text highScoreText;

    private void Awake()
    {
        var canvas = transform.GetChild(0);
        currentScoreText = canvas.transform.Find("CurrentScoreText").GetComponent<Text>();
        highScoreText = canvas.transform.Find("HighScoreText").GetComponent<Text>();
    }

    public void UpdateScoreUI(int currentScore)
    {
        currentScoreText.text = "SCORE: " + currentScore.ToString("0");
    }

    public void UpdateHighScoreUI(int highScore)
    {
        highScoreText.text = "BEST: " + highScore.ToString("0");           
    }

    public void AnimateTexts()
    {
        currentScoreText.transform.localScale = new Vector2(1.1f, 1.1f);
        currentScoreText.color = Color.green;

        highScoreText.transform.localScale = new Vector2(0.9f, 0.9f);
        highScoreText.color = Color.grey;
    }
}
