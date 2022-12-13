using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCanvasMB : MonoBehaviour
{
    [SerializeField]
    private Canvas _gameOverCanvas;

    [SerializeField]
    private Text _finalScoreText;

    public void Disable()
    {
        _gameOverCanvas.enabled = false;
    }

    public void Enable(int finalScore)
    {
        _gameOverCanvas.enabled = true;

        _finalScoreText.text += finalScore.ToString();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
