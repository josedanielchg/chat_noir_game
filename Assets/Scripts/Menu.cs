using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Out()
    {
        Debug.Log("Out...");
        Application.Quit();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0);
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex * 0) + 1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void PlayGame()
    {
        Time.timeScale = 1;
    }

    public void GoBackMenuInGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0);
    }

    public void MuteGame()
    {
        AudioListener.pause = true;
    }

    public void MusiceGame()
    {
        AudioListener.pause = false;
    }


}
