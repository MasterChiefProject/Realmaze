using Unity.VisualScripting;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static int points = 0;
    public static bool hasKey = false;

    public static void InitGlobals()
    {
        points = 0;
        hasKey = false;
    }
}
