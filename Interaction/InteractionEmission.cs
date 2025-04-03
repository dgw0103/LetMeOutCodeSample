using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEmission : InteractionHelping, IRendererLayerSetter
{
    private RendererSetter rendererSetter;
    private Action<Material> turnOnEmission;
    private Action<Material> turnOffEmission;
    private Dictionary<Material, Color> colors = new Dictionary<Material, Color>();



    protected void Awake()
    {
        rendererSetter = new RendererSetter(this);
        turnOnEmission = new Action<Material>((material) =>
        {
            if (material.HasColor(Keyword.emissiveColor))
            {
                colors.Add(material, material.GetColor(Keyword.emissiveColor));
                material.SetColor(Keyword.emissiveColor, new Color(0.1f, 0.1f, 0.1f, 1f));
            }
        });
        turnOffEmission = new Action<Material>((material) =>
        {
            if (colors.TryGetValue(material, out Color color))
            {
                if (material.HasColor(Keyword.emissiveColor))
                {
                    material.SetColor(Keyword.emissiveColor, color);
                }
            }
        });
    }



    public override void OnLookAt()
    {
        base.OnLookAt();
        TurnOnEmission();
    }
    public override void OnNoLookAt()
    {
        base.OnNoLookAt();
        TurnOffEmission();
    }
    public void TurnOnEmission()
    {
        rendererSetter.ActSomethingToAllMaterials(turnOnEmission);
    }
    public void TurnOffEmission()
    {
        rendererSetter.ActSomethingToAllMaterials(turnOffEmission);
        colors.Clear();
    }
    public void SetRendererLayerMask(uint layerMask)
    {
        rendererSetter.SetRendererLayerMask(layerMask);
    }
}
