using System.Collections;
using UnityEngine;

public class GameMaster : Singelton<GameMaster>
{
    private bool highScoreReached;
    private int currentScore = 0;
    private int highScore = 0;
    private float scorePointX;

    public bool NewHighScore { get { return currentScore > highScore; } }
    public bool HighScoreReached { get { return highScoreReached == false && highScore > 0 && highScore < currentScore; } }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.IsLevelCreated);

        scorePointX = PlayerEngine.Instance.Position.x;
    }

    public void NeedIncreaseScore()
    {
        if (PlayerEngine.Instance.Position.x > scorePointX + 1)
        {
            AddScore();

            scorePointX = PlayerEngine.Instance.Position.x;
        }
    }

    private void AddScore()
    {
        currentScore++;
      
        if (HighScoreReached)
        {
            highScoreReached = true;
            UIManager.Instance.UpdateHighScoreUI(highScore);
            UIManager.Instance.AnimateTexts();
        }
        
        UIManager.Instance.UpdateScoreUI(currentScore);
    }


    public void LoadScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore");

        UIManager.Instance.UpdateScoreUI(currentScore);
        UIManager.Instance.UpdateHighScoreUI(highScore);
    }

    public void SaveScore()
    {
        if (NewHighScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void ClearScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
    }
}
