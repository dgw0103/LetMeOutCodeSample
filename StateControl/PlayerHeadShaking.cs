using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadShaking : MonoBehaviour
{
    [SerializeField] private float intensity = 5f;
    private Transform head;
    private Vector3 originalHeadPosition;
    private Vector3 previousRandomPosition;



    public static PlayerHeadShaking AddComponent(GameObject head)
    {
        PlayerHeadShaking playerHeadShaking = head.AddComponent<PlayerHeadShaking>();

        playerHeadShaking.head = head.transform;
        playerHeadShaking.originalHeadPosition = head.transform.localPosition;

        return playerHeadShaking;
    }



    private void LateUpdate()
    {
        Vector3 randomPosition = Time.deltaTime * intensity * new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);

        if (head.transform.localPosition - previousRandomPosition != originalHeadPosition)
        {
            originalHeadPosition = head.transform.localPosition;
        }

        head.transform.localPosition = originalHeadPosition + randomPosition;
        previousRandomPosition = randomPosition;
    }
    private void OnDisable()
    {
        head.transform.localPosition = originalHeadPosition;
    }
}