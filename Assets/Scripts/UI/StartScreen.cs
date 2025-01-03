using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public Button playButton;
    public string startLevelName;

	void Start ()
    {
        playButton.gameObject.SetActive(false);

        GetComponent<FadeIn>().OnFadeInFinished = OnFadeInFinished;
	}

    public void OnFadeInFinished()
    {
        playButton.gameObject.SetActive(true);
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene(startLevelName);
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
