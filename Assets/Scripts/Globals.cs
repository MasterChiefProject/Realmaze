using UnityEngine;

public class Globals : MonoBehaviour
{
    public static int points;
    public static bool hasKey;

    public static void InitGlobals()
    {
        points = 0;
        hasKey = false;
    }
}
