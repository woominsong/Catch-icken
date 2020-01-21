using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void press_start()
    {
        SceneManager.LoadScene("Login");
    }
}
