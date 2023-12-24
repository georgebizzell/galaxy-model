using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Options : MonoBehaviour
{

   public enum StartingShape
    {
        Square,
        Circle,
        Rings,
        Tail,
        Spiral
    }

    public StartingShape startingShape = StartingShape.Square;
    public StartingShape GetChosenStartingShape()
    {
        return startingShape;
    }


// Assign buttons in the Inspector
public Button squareButton;
public Button circleButton;

public Button ringsButton;
public Button spiralButton;

public Button tailButton;


public void OptionOnAnyButton ()
{
    SceneManager.LoadScene(0);
}

public void OnSquareButton()
{
    startingShape = StartingShape.Square;
    PlayerPrefs.SetInt("StartingShape", (int)startingShape);
}

public void OnCircleButton()
{
    startingShape = StartingShape.Circle;
    PlayerPrefs.SetInt("StartingShape", (int)startingShape);
}

public void OnRingsButton()
{
    startingShape = StartingShape.Rings;
    PlayerPrefs.SetInt("StartingShape", (int)startingShape);
}

public void OnSpiralButton()
{
    startingShape = StartingShape.Spiral;
    PlayerPrefs.SetInt("StartingShape", (int)startingShape);
}

public void OnTailButton()
{
    startingShape = StartingShape.Tail;
    PlayerPrefs.SetInt("StartingShape", (int)startingShape);
}

}
