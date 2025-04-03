using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoJin;

public class RendererSetter : IRendererLayerSetter
{
    private MonoBehaviour monoBehaviour;
    private IEnumerable<Renderer> renderers;



    public RendererSetter(MonoBehaviour monoBehaviour)
    {
        this.monoBehaviour = monoBehaviour;
        InitRenderers();
    }



    public void SetRendererLayerMask(uint layerMask)
    {
        foreach (var item in renderers)
        {
            item.renderingLayerMask = layerMask;
        }
    }
    private void InitRenderers()
    {
        renderers = monoBehaviour.GetComponentsInChildren<Renderer>(true);
    }
    public void ActSomethingToAllMaterials(Action<Material> action)
    {
        bool hasNull = false;



        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                hasNull = true;
            }
            else
            {
                foreach (var material in renderer.materials)
                {
                    action.Invoke(material);
                }
            }
        }

        if (hasNull)
        {
            InitRenderers();
        }
    }
}