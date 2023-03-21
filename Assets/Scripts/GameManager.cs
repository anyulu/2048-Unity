using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard board;

    public CanvasGroup gameOver;

    public TextMeshProUGUI currentScore;

    public TextMeshProUGUI bestScore;

    private int score;

    private void Start()
    {
        this.NewGame();
    }

    public void NewGame()
    {
        this.gameOver.alpha = 0f;
        this.gameOver.interactable = false;
        this.SetScore(0);
        this.bestScore.text = this.LoadBestScore().ToString();
        this.board.ClearBoard();
        this.board.CreateTile();
        this.board.CreateTile();
        this.board.enabled = true;
    }

    public void Gameover()
    {
        this.board.enabled = false;
        this.gameOver.interactable = true;
        StartCoroutine(this.Fade(this.gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(to, from, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int increment)
    {
        this.SetScore(this.score + increment);
    }

    private void SetScore(int score)
    {
        this.score = score;
        this.currentScore.text = score.ToString();
        int bestScore = this.LoadBestScore();
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("bestScore", score);
        }
        this.bestScore.text = (Mathf.Max(score, bestScore)).ToString();
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("bestScore", 0);
    }
}
