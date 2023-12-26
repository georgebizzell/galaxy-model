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

    public float centreMass = 1000000; 


// Assign buttons in the Inspector
public Button squareButton;
public Button circleButton;

public Button ringsButton;
public Button spiralButton;

public Button tailButton;

public Slider centreMassSlider;

public Slider numberOfStarsSlider;

public float numberOfStars = 1000;

public float centreOfMass = 1000000;

public void Start()
{
    Debug.Log("Centre Mass Slider is - " + centreMassSlider);
    //Adds a listener to the main slider and invokes a method when the value changes.
    centreMassSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    numberOfStarsSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
}

// Invoked when the value of the slider changes.
public void ValueChangeCheck()
{
    Debug.Log("Centre Mass Slider from ValueChangeCheck - " + centreMassSlider.value);
    Debug.Log(numberOfStarsSlider.value);
    centreOfMass = centreMassSlider.value;
    numberOfStars = numberOfStarsSlider.value;
    PlayerPrefs.SetInt("centreMass", (int)centreMassSlider.value);
    PlayerPrefs.SetInt("numberOfStars", (int)numberOfStarsSlider.value);
}


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
