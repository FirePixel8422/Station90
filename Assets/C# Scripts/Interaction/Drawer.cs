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
        Vector3 pos;
        while (true)
        {
            pos = transform.localPosition;
            transform.localPosition = Vector3.MoveTowards(pos, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(pos, targetPos) < 0.01f)
            {
                transform.localPosition = targetPos;
                yield break;
            }

            yield return null;
        }
    }
}