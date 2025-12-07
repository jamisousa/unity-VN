using System.Collections;
using TMPro;
using UnityEngine;

public class TextArchitect
{

    private TextMeshProUGUI tmpro_UI;
    private TextMeshPro tmpro_world;

    public TMP_Text tmpro => tmpro_UI != null ? tmpro_UI : tmpro_world;
    
    public string currentText => tmpro.text;
    public string targetText { get; private set; } = "";
    public string preText { get; private set; } = "";
    private int preTextLength = 0;

    public string fullTargetText => preText + targetText;

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

    public bool hurryText = false;
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_UI = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        StopBuilding();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        StopBuilding();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

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
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }


    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;

        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }
        tmpro.text += targetText;

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

    private IEnumerator Build_Typewriter()
    {
        float soundInterval = 0.05f; 
        float soundTimer = 0f;

        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            int previous = tmpro.maxVisibleCharacters;
            tmpro.maxVisibleCharacters += hurryText ? charactersPerCycle * 5 : charactersPerCycle;
            int lettersAdded = tmpro.maxVisibleCharacters - previous;

            soundTimer += 0.015f / speed;

            if (soundTimer >= soundInterval && AudioManager.instance != null && lettersAdded > 0)
            {
                soundTimer = 0f;

                if (AudioManager.instance.typingClip1 != null)
                {
                    AudioManager.instance.PlaySoundEffect(AudioManager.instance.typingClip1,
                                                          AudioManager.instance.sfxMixer, 0.5f, 1f, false);
                }
            }

            yield return new WaitForSeconds(0.015f / speed);
        }
    }



    private IEnumerator Build_Fade() {

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
