using UnityEngine;

/// <summary>
/// Component for controlling what happens when the player steps on an exit tile.
/// If the exit tile is the last one of the game ("the victory exit"), the game ends.
/// Otherwise, the exit tile teleports the player to the next map, which causes a scene
/// load to happen in Unity.
/// </summary>
public class MapExit : MonoBehaviour
{
    public bool isVictoryExit = false;
    public string nextMapSceneName = "";
    public AudioClip victorySound;

    public void OnPlayerTrigger()
    {
        if (isVictoryExit)
        {
            Services.Sfx.Play(victorySound);
            Services.Hud.ShowVictoryUi();
        }
        else
        {
            Services.Hud.ShowNextMapUi(nextMapSceneName);
        }
    } 
}
