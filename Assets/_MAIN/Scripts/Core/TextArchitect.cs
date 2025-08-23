using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_UI;
    private TextMeshPro tmpro_world;

    public TMP_Text tmpro => tmpro_UI != null ? tmpro_UI : tmpro_world;
    
    //current text
    public string currentText => tmpro.text;
    //what we are going to build
    public string targetText { get; private set; } = "";
    //preText is whatever is already on the architect's tmpro when StartBuilding is called
    public string preText { get; private set; } = "";
    private int preTextLength = 0;

    public string fullTargetText => preText + targetText;

    //enum for build method
    public enum BuildMethod
    {
        instant, typewriter, fade
    }

    public BuildMethod buildMethod = BuildMethod.typewriter;
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; }  }
    private float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    //force the text to finish building 
    public bool hurryText = false;


    //constructors for UI and world text 
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_UI = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    //coroutines to build text
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        StopBuilding();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    //apend text to what is already in the text architect
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        StopBuilding();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }


    //control for building process
    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;


    //stop coroutine if it's running
    public void StopBuilding()
    {
        if (!isBuilding) { return; }

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();
        switch (buildMethod)
        {
            case BuildMethod.instant:
                tmpro.text = fullTargetText;
                break;
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }
        yield return null;
    }

    //prepare based on the build method
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    private void Prepare_Instant()
    {
        //reset the color
        tmpro.color = tmpro.color;

        //set the text directly
        tmpro.text = fullTargetText;

        //apply changes made to the text
        tmpro.ForceMeshUpdate();

        //make sure every character is visible on screen
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }


    private void Prepare_Typewriter()
    {
        //reset the color
        tmpro.color = tmpro.color;

        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        //check if there is pretext, force itself to update and make sure all text is visible.
        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }
        tmpro.text += targetText;

        //apply changes made to the text
        tmpro.ForceMeshUpdate();
    }


    private void Prepare_Fade()
    {

    }


    //modes of text building enumerators
    private IEnumerator Build_Typewriter() { 
    
       while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters +=  hurryText ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }

    }

    private IEnumerator Build_Fade() { yield return null; }


    //once the building is done
    public void onComplete()
    {
        buildProcess = null;
        hurryText = false;
    }

    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }

        StopBuilding();
        onComplete();
    }
}
