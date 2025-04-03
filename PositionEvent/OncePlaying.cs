using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public class OncePlaying : IEnvironmentVisitor
{
    private GameObject instantiated;



    public OncePlaying(MonoBehaviour area, AudioSource audioSource, Vector3 position)
    {
        instantiated = Object.Instantiate(audioSource.gameObject, area.transform);
        instantiated.transform.position = position;
        area.StartCoroutine(PlayAndDestroy(instantiated.GetComponent<AudioSource>()));
    }



    private IEnumerator PlayAndDestroy(AudioSource audioSource)
    {
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        Object.Destroy(audioSource.gameObject);
    }
    public void OnExit(Character character)
    {
        
    }
}