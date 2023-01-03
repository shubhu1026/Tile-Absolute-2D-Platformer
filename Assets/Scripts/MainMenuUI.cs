using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void LoadPlay()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LoadQuit()
    {
        Application.Quit();
    }

    public void LoadControls()
    {
        SceneManager.LoadScene("Controls Menu");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
