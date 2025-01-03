using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Text healthText;
    public Text strengthText;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject nextMapPanel;
    public GameObject pausePanel;
    public GameObject inputButtons;
    public GameObject pauseButton;

    public GameObject player;
    public Image cooldown;
    [HideInInspector] public float tempFill;
    [HideInInspector] public bool hasBeenPaused = false;

    public string exitDestinationScene = "StartScreen";

    private string nextMapName;
    
    //Pulls all the necessary data from the GameMaster

    void Start()
    {
        tempFill = GameMaster.instance.tempFill;
        hasBeenPaused = GameMaster.instance.hasBeenPaused;

        if (GameMaster.instance.hasPotionEffect == true)
        {
            cooldown.fillAmount = GameMaster.instance.tempFill;
        }
    }

    void Update()
    {
        if (player.GetComponent<PlayerController>().hasPotionEffect == true)
        {
            //Make the potion cooldown image display when the player first walks over a potion

            if (player.GetComponent<PlayerController>().potionTime == 0)
            {
                cooldown.fillAmount += 1;
            }

            //Makes the potion cooldown image display and get ticked off by the percentage of its duration it has left

            if (player.GetComponent<PlayerController>().lowerCooldownImage == true)
            {
                cooldown.fillAmount -= 1 / player.GetComponent<PlayerController>().potionDuration;
                player.GetComponent<PlayerController>().lowerCooldownImage = false;
            }

            //Makes the potion cooldown image display when unpaused and not when paused

            if (player.GetComponent<PlayerController>().paused == true && hasBeenPaused == false)
            {
                tempFill = cooldown.fillAmount;
                cooldown.fillAmount -= 1;
                hasBeenPaused = true;
            }

            if (player.GetComponent<PlayerController>().paused == false && hasBeenPaused == true)
            {
                cooldown.fillAmount = tempFill;
                hasBeenPaused = false;
            }

        }

        if (player.GetComponent<PlayerController>().hasPotionEffect == false)
        {
            cooldown.fillAmount -= 1;
        }

        //Makes the strength text do the same as the potion cooldown image and disappear while paused

        if (player.GetComponent<PlayerController>().paused == false)
        {
            strengthText.gameObject.SetActive(true);
            if (player.GetComponent<PlayerController>().hasPotionEffect == true)
            {
                strengthText.GetComponent<Text>().text = "Strength: " + (player.GetComponent<PlayerController>().currentLevel* player.GetComponent<PlayerController>().strengthMultiplier);
            }
            else
            {
                strengthText.GetComponent<Text>().text = "Strength: " + player.GetComponent<PlayerController>().currentLevel;
            }
        }

        if (player.GetComponent<PlayerController>().paused == true)
        {
            strengthText.gameObject.SetActive(false);
            inputButtons.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            hasBeenPaused = true;
        }

        if (player.GetComponent<PlayerController>().paused == false && hasBeenPaused == true)
        {
            strengthText.gameObject.SetActive(true);
            inputButtons.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(true);
        }
    }

	void Awake()
    {
        Services.Hud = this;
        Services.PauseScreen = pausePanel;
    }

    public void ShowNextMapUi(string nextMapName)
    {
        this.nextMapName = nextMapName;

        player.GetComponent<PlayerController>().paused = true;
        hasBeenPaused = false;
        healthText.gameObject.SetActive(false);
        strengthText.gameObject.SetActive(false);
        nextMapPanel.SetActive(true);
    }

    public void ShowVictoryUi()
    {
        player.GetComponent<PlayerController>().paused = true;
        hasBeenPaused = false;
        healthText.gameObject.SetActive(false);
        strengthText.gameObject.SetActive(false);
        victoryPanel.SetActive(true);
    }

    public void ShowDefeatUi()
    {
        player.GetComponent<PlayerController>().paused = true;
        hasBeenPaused = false;
        healthText.gameObject.SetActive(false);
        strengthText.gameObject.SetActive(false);
        defeatPanel.SetActive(true);
    }

    public void OnExitButtonPressed()
    {
        player.GetComponent<PlayerController>().Restart();
        SceneManager.LoadScene(exitDestinationScene);
    }

    public void OnRetryButtonPressed()
    {
        player.GetComponent<PlayerController>().Restart();
        SceneManager.LoadScene("ManyZombiesMap");
    }

    public void OnNextMapButtonPressed()
    {
        SceneManager.LoadScene(nextMapName);
    }
}
