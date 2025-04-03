using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using HoJin;

public class FuseBox : MonoBehaviour, IInteractionDown, IUsed, IUsingAnimationPlayer
{
    [SerializeField] private FuseData[] fuseDatas = new FuseData[FuseData.fuseNumber];
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Door electricDoor;
    [SerializeField] private AudioSource fuseInsertingSound;
    private const string key = "FuseBox";



    public IEnumerator DuringAnimationPlaying
    {
        get
        {
            yield return null;
        }
    }
    public void OnBeUsed()
    {
        fuseDatas.First((x) => x.IsTurnOn == false).TurnOn(greenMaterial);
        fuseInsertingSound.Play();
        if (fuseDatas.All((x) => x.IsTurnOn))
        {
            electricDoor.IsLocked = false;
            electricDoor.Open();
        }
    }
    public void OnInteractionDown()
    {
        StageManager.Instance.UIs.SystemMessagePlayer.Stop();
        StageManager.Instance.UIs.SystemMessagePlayer.PlayOneByOne(new SystemMessagePlaying(new TranslatableSystemMessageData(TranslatorKeywordType.Interaction, key)));
    }
    public void OnAfterUsing()
    {
        if (fuseDatas.All((x) => x.IsTurnOn))
        {
            gameObject.SetLayerAs(Keyword.defaultText);
        }
    }
    public void PlayAnimation()
    {
        
    }
    [Serializable]
    public struct FuseData
    {
        [SerializeField] private GameObject fuse;
        [SerializeField] private MeshRenderer fuseLight;
        public const int fuseNumber = 4;



        public bool IsTurnOn { get => fuse.activeSelf; }
        public void TurnOn(Material greenMaterial)
        {
            fuse.SetActive(true);
            fuseLight.material = greenMaterial;
        }
    }
}