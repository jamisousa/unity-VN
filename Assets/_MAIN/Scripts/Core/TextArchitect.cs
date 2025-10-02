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
        onComplete();  
    
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
        tmpro.text = preText;
        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else preTextLength = 0;

        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        //fade effect
        TMP_TextInfo textInfo = tmpro.textInfo;
        Color visibleColor = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color hiddenColor = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i]; 

            if(!charInfo.isVisible) { continue; }

            if (i < preTextLength)
            {
                for(int vertex = 0; vertex < 4; vertex++)
                {
                    vertexColors[charInfo.vertexIndex + vertex] = visibleColor;
                }
            }
            else
            {
                for (int vertex = 0; vertex < 4; vertex++)
                {
                    vertexColors[charInfo.vertexIndex + vertex] = hiddenColor;
                }
            }
        }

        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }


    //modes of text building enumerators
    private IEnumerator Build_Typewriter() { 
    
       while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters +=  hurryText ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }

    }

    private IEnumerator Build_Fade() {

        //define a range of characters to fade in and remove from the range to add new ones
        int minRange = preTextLength;
        int maxRange = minRange + 1;

        byte alphaThreshold = 15;
        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while (true)
        {
            float fadeSpeed = ((hurryText ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 4f;

            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) { continue; }


                int vertexIndex = textInfo.characterInfo[i].vertexIndex; 

                //move alpha of character to visible color
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);


                for (int vertex = 0; vertex < 4; vertex++)
                {
                    vertexColors[charInfo.vertexIndex + vertex].a = (byte)alphas[i];
                }

                if (alphas[i] >= 255)
                {
                    minRange++;
                }
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool isLastCharacterInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || isLastCharacterInvisible)
            {
                if(maxRange < textInfo.characterCount)
                {
                    maxRange++;
                }
                else if (alphas[maxRange -1] >= 255 || isLastCharacterInvisible)
                {
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

    }


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
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();
                break;
        }

        StopBuilding();
        onComplete();
    }
}
