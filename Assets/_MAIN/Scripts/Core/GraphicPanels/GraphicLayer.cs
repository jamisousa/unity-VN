using UnityEngine;

//a single layer on a graphic panel that can be assigned an image or video and stacked on other layers
public class GraphicLayer
{
    public const string LAYER_OBJECT_NAME_FORMAT = "Layer: {0}";
    public int layerDepth = 0;
    public Transform panel;

    public GraphicObject currentGraphic { get; private set;} = null;

    //create texture to apply image to layer
    //accept both loading directly assigning and file paths
    public void SetTexture(string filePath, float transitionSpeed = 1f, Texture blendingTexture = null)
    {
        Texture tex = Resources.Load<Texture2D>(filePath);

        if(tex == null)
        {
            Debug.LogError("Unable to load graphic textures from path");
            return;
        }

        SetTexture(tex, transitionSpeed, blendingTexture, filePath);
    }
    public void SetTexture(Texture tex, float transitionSpeed = 1f, Texture blendingTexture = null, string filePath = "")
    {
        CreateGraphic(tex, transitionSpeed, filePath, blendingTexture: blendingTexture);
    }

    //create both texture and video
    private void CreateGraphic<T>(T graphicData, float transitionSpeed, string filePath, bool useAudioForVideo = true, Texture blendingTexture = null)
    {
        GraphicObject newGraphic = null;

        if(graphicData is Texture)
        {
            Debug.Log("is texture");
            newGraphic = new GraphicObject(this, filePath, graphicData as Texture);
        }

        currentGraphic = newGraphic;

        currentGraphic.FadeIn(transitionSpeed, blendingTexture);
    }
}
