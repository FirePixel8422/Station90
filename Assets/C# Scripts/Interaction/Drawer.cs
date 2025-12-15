using System.Collections;
using UnityEngine;


public class Drawer : Interactable
{
    [SerializeField] private Vector3[] positionStates;
    [SerializeField] private float moveSpeed = 5;

    private int cPosIndex;
    private Coroutine posAnimCO;


    protected override void OnInteract()
    {
        if (posAnimCO != null)
        {
            StopCoroutine(posAnimCO);
        }
        posAnimCO = StartCoroutine(MoveAnimation(positionStates[cPosIndex], moveSpeed));

        cPosIndex.IncrememtSmart(positionStates.Length);
    }

    private IEnumerator MoveAnimation(Vector3 targetPos, float speed)
    {
        Vector3 startPos = transform.localPosition;
        float startTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            yield return null;

            t = (Time.time - startTime) * speed;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
        }
    }
}