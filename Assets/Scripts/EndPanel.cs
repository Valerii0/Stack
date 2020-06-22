using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndPanel : MonoBehaviour
{
    public Button retryButton;
    public Button menuButton;

    private void Awake()
    {
        SetupButton("Retry", retryButton);
        SetupButton("Menu", menuButton);
    }

    private void SetupButton(string title, Button button)
    {
        var colors = button.colors;
        colors.normalColor = new Color(34f/255, 44f/255, 55f/255, 0.9f);
        button.colors = colors;
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.text = title;
    }

    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene(Scenes.Game.ToString());
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene(Scenes.Menu.ToString());
    }
}