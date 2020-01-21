using UnityEngine;

public class Corridor_Music : MonoBehaviour
{
    // Play Global
    private static Corridor_Music instance = null;
    public static Corridor_Music Instance
    {
        get { return instance;  }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
