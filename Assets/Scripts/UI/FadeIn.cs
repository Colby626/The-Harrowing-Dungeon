using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeIn : MonoBehaviour
{
    public delegate void FadeInFinishedHandler();
    
    enum FadeState
    {
        Delay,
        FadingIn,
        FadeInComplete,
        Finished
    }

    /// <summary>
    /// How long to wait before fading in. Measured in seconds.
    /// </summary>
    [Tooltip("How long to wait before fading in. Measured in seconds.")]
    public float fadeStartDelay = 3.0f;

    /// <summary>
    /// How long to fade from zero alpha to full alpha. Measured in seconds.
    /// </summary>
    [Tooltip("How long to fade from zero alpha to full alpha. Measured in seconds.")]
    public float fadeInDuration = 4.0f;

    public FadeInFinishedHandler OnFadeInFinished;

    private CanvasGroup canvasGroup;
    private FadeState fadeState = FadeState.Delay;
    private float timer = 0.0f;
    
	void Start ()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
	}
	
	void Update ()
    {
        if (fadeState != FadeState.Finished && Input.GetKeyDown(KeyCode.Escape))
        {
            canvasGroup.alpha = 1.0f;

            if (OnFadeInFinished != null)
            {
                OnFadeInFinished();
            }

            fadeState = FadeState.Finished;
            return;
        }

        switch (fadeState)
        {
            case FadeState.Delay:
                timer += Time.deltaTime;

                if (timer >= fadeStartDelay)
                {
                    timer -= fadeStartDelay;
                    fadeState = FadeState.FadingIn;
                }

                break;

            case FadeState.FadingIn:
                timer += Time.deltaTime;

                canvasGroup.alpha = timer / fadeInDuration;

                if (timer >= fadeInDuration)
                {
                    fadeState = FadeState.FadeInComplete;
                }

                break;

            case FadeState.FadeInComplete:
                if (OnFadeInFinished != null)
                {
                    OnFadeInFinished();
                }

                fadeState = FadeState.Finished;
                break;

            case FadeState.Finished:
                // do nothing
                break;
        }
	}
}
