using UnityEngine;
using UnityEngine.SceneManagement;


public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        SceneManager.LoadScene(1);
    }
}
