using UnityEngine;

public class Services : MonoBehaviour
{
    public static Sfx Sfx { get; set; }

    public static Simulation Simulation { get; set; }
    public static Hud Hud { get; set; }
    public static GameObject PauseScreen { get; set; }
    public static PlayerController PlayerController { get; set; }
}
