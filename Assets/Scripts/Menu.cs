using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum PlayerPrefsValues
{
    Score
}

enum Scenes
{
    Game,
    Menu
}

public class Menu : MonoBehaviour
{
    public Text titleText;
    public Text scoreText;
    public Button playButton;

    private void Awake()
    {
        SetupBG();
        SetupTitleText();
        SetupScoreText();
        SetupPlayButton();
        ConfigureHighscore();
    }

    private void SetupBG()
    {
        GetComponent<Camera>().backgroundColor = new Color(34f/255, 44f/255, 55f/255, 0.9f);
    }

    private void SetupTitleText()
    {
        titleText.fontStyle = FontStyle.BoldAndItalic;
        titleText.fontSize = 150;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.UpperCenter;
        titleText.text = "The Stack";
    }

    private void SetupScoreText()
    {
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.fontSize = 100;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.LowerCenter;
    }

    private void SetupPlayButton()
    {
        Text buttonText = playButton.GetComponentInChildren<Text>();
        buttonText.color = new Color(34f/255, 44f/255, 55f/255, 0.9f);
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.text = "Play";
    }

    private void ConfigureHighscore()
    {
        scoreText.text = string.Format("Highscore: {0}!", PlayerPrefs.GetInt(PlayerPrefsValues.Score.ToString()));
    }

    public void StartGame()
    {
        SceneManager.LoadScene(Scenes.Game.ToString());
    }
}