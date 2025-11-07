using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//contains layers that can be assigned images and videos on the UI
[System.Serializable]
public class GraphicPanel
{
    public string panelName;
    public GameObject rootPanel;
    private List<GraphicLayer> layers = new List<GraphicLayer>();

    public GraphicLayer GetLayer(int layerDepth, bool createIfDoesNotExist = false)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].layerDepth == layerDepth)
            {
                return layers[i];
            }
        }

        if (createIfDoesNotExist)
        {
            return CreateLayer(layerDepth);
        }

        return null;
    }

    private GraphicLayer CreateLayer(int layerDepth)
    {
        GraphicLayer layer = new GraphicLayer();
        GameObject panel = new GameObject(string.Format(GraphicLayer.LAYER_OBJECT_NAME_FORMAT, layerDepth));
        RectTransform rect = panel.AddComponent<RectTransform>();

        panel.AddComponent<CanvasGroup>();
        panel.transform.SetParent(rootPanel.transform, false);

        //set anchors to scale to the size of its parent panel // whole scale of  UI
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        layer.panel = panel.transform;
        layer.layerDepth = layerDepth;

        //check if there any layers that are greater layer depth than current one
        int index = layers.FindIndex(l => l.layerDepth > layerDepth);

        //no match
        if(index == -1)
        {
            layers.Add(layer);
        }
        //matches
        else
        {
            layers.Insert(index, layer);
        }

        //sort objects in hierarchy
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].panel.SetSiblingIndex(layers[i].layerDepth);
        }

        return layer;
    }
}
