using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour 
{
    public Texture2D black;
    public Texture2D white;

    private Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDirection = -1; // -1 fade in, 1 fade out

    private void Awake()
    {
        if (TitleUIController.darkTheme)
            fadeOutTexture = black;
        else
            fadeOutTexture = white;
    }

    private void OnGUI()
    {
        // fade out/in the alpha value using a direction, speed, time.deltatime to conver the operation to seconds
        alpha += fadeDirection * fadeSpeed * Time.deltaTime;

        //force the number between 0 and 1 because GUI.colour use alpha values between 0 and 1
        alpha = Mathf.Clamp01(alpha);

        //set colour of GUI. all colour values remain the same and the alpha is et to the alpha variable
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),fadeOutTexture);
    }

    //sets dadeDirection to the direction parameter making the scene fade in if -1 and out if 1
    public float BeginFade(int direction)
    {
        fadeDirection = direction;
        return (fadeSpeed);
    }

    private void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }

    public void setTexture(bool isBlack)
    {
        if (isBlack)
            fadeOutTexture = black;
        else
            fadeOutTexture = white;
    }
}
