using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public void OnResumePressed()
    {
        Services.PlayerController.Unpause();
        gameObject.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
