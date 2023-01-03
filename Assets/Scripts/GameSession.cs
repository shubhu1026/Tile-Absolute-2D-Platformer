using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Header("Lives & Score")]
    [SerializeField] int playerLives = 3;
    [SerializeField] int numOfHearts;

    [SerializeField] Image[] hearts;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int score = 0;

    [Header("SFXs")]
    [SerializeField] AudioClip gameOverSFX;
    [SerializeField] float gameOverSFXVolume = 0.3f;
    [SerializeField] AudioClip dieSFX;
    [SerializeField] float dieSFXVolume = 0.3f;

    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if(numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() 
    {
        scoreText.text = score.ToString();
    }

    void Update()
    {
        numOfHearts = playerLives;
        for(int i=0; i < hearts.Length; i++)
        {
            if(i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            AudioSource.PlayClipAtPoint(dieSFX, Camera.main.transform.position, dieSFXVolume);
            StartCoroutine(TakeLife());
        }
        else
        {
            AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position, gameOverSFXVolume);
            ResetGameSession();
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    void ResetGameSession()
    {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        StartCoroutine(WaitAndLoadGameOver());
    }

    IEnumerator WaitAndLoadGameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene("Game Over");
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        Destroy(gameObject);
    }

    IEnumerator TakeLife()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        playerLives--;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
