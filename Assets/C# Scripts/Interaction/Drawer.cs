using System.Collections;
using UnityEngine;


public class Drawer : Interactable
{
    [SerializeField] private Vector3[] positionStates;
    [SerializeField] private float rotSpeed = 5;

    private int cPosIndex;
    private Coroutine rotAnimCO;


    protected override void OnInteract()
    {
        if (rotAnimCO != null)
        {
            StopCoroutine(rotAnimCO);
        }
        rotAnimCO = StartCoroutine(RotateAnimation(positionStates[cPosIndex], rotSpeed));

        cPosIndex.IncrememtSmart(positionStates.Length);
    }

    private IEnumerator RotateAnimation(Vector3 targetPos, float speed)
    {
        Vector3 pos;
        while (true)
        {
            pos = transform.position;
            transform.position = Vector3.MoveTowards(pos, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(pos, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                yield break;
            }

            yield return null;
        }
    }
}