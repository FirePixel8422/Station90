using System.Collections;
using UnityEngine;


public class Door : Interactable
{
    [SerializeField] private Quaternion[] rotationStates;
    [SerializeField] private float rotSpeed = 5;

    private int cRotindex;
    private Coroutine rotAnimCO;



    protected override void OnInteract()
    {
        if (rotAnimCO != null)
        {
            StopCoroutine(rotAnimCO);
        }
        rotAnimCO = StartCoroutine(RotateAnimation(rotationStates[cRotindex], rotSpeed));

        cRotindex.IncrememtSmart(rotationStates.Length);
    }

    private IEnumerator RotateAnimation(Quaternion targetRot, float speed)
    {
        Quaternion startRot = transform.localRotation;
        float startTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            yield return null;

            t  = (Time.time - startTime) * speed;
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
        }
    }
}