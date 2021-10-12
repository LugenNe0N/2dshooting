using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartButtonPressed()
    {
        Debug.Log("Start Button is pressed");
        SceneManager.LoadScene("Main");
    }

    public void ScoreButtonPressed()
    {
        Debug.Log("Score Button is pressed");
        SceneManager.LoadScene("score");
    }

    public void ReturnButtonPressed()
    {
        Debug.Log("Return Button is pressed");
        SceneManager.LoadScene("Title");
    }

    public void RestartButtonPressed()
    {
        Debug.Log("Retart Button is pressed");
        SceneManager.LoadScene("Main");
    }

    public void QuitButtonPressed()
    {
        Debug.Log("Quit Button is pressed");
        UnityEngine.Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
        //SceneManager.LoadScene ("Quit");
    }
}
