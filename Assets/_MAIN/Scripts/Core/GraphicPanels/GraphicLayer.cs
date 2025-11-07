using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

//a single layer on a graphic panel that can be assigned an image or video and stacked on other layers
public class GraphicLayer
{
    public const string LAYER_OBJECT_NAME_FORMAT = "Layer: {0}";
    public int layerDepth = 0;
    public Transform panel;
    private List<GraphicObject> oldGraphics = new List<GraphicObject>();

    public GraphicObject currentGraphic = null;

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

        if (graphicData is Texture)
        {
            Debug.Log("is texture");
            newGraphic = new GraphicObject(this, filePath, graphicData as Texture);
        }
        else if (graphicData is VideoClip) {
            newGraphic = new GraphicObject(this, filePath, graphicData as VideoClip, useAudioForVideo);
        }

        if (currentGraphic != null && !oldGraphics.Contains(currentGraphic)) {
            oldGraphics.Add(currentGraphic);
        }

        currentGraphic = newGraphic;

        currentGraphic.FadeIn(transitionSpeed, blendingTexture);
    }

    public void SetVideo(string filePath, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null)
    {
        VideoClip clip = Resources.Load<VideoClip>(filePath);

        if (clip == null)
        {
            Debug.LogError("Unable to load graphic video from path");
            return;
        }

        SetVideo(clip, transitionSpeed, useAudio, blendingTexture, filePath);
    }

    public void SetVideo(VideoClip video, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, string filePath = "")
    {
        CreateGraphic(video, transitionSpeed, filePath, useAudio, blendingTexture: blendingTexture);
    }

    public void DestroyOldGraphics()
    {
        foreach(var g in oldGraphics)
        {
            Object.Destroy(g.renderer.gameObject);
        }

        oldGraphics.Clear();
    }

    public void Clear()
    {
        if(currentGraphic != null)
        {
            currentGraphic.FadeOut();
        }

        foreach(var g in oldGraphics)
        {
            g.FadeOut();
        }
    }

}
