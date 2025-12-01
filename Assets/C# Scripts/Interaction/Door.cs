using System.Collections;
using UnityEngine;


public class Door : Interactable
{
    [SerializeField] private Quaternion[] rotationStates;
    [SerializeField] private float rotSpeed = 5;

    private int cRotState;
    private Coroutine rotAnimCO;


    protected override void OnInteract()
    {
        if (rotAnimCO != null)
        {
            StopCoroutine(rotAnimCO);
        }
        rotAnimCO = StartCoroutine(RotateAnimation(rotationStates[cRotState], rotSpeed));

        cRotState.IncrememtSmart(rotationStates.Length);
    }

    private IEnumerator RotateAnimation(Quaternion targetRot, float speed)
    {
        Quaternion rot;
        while (true)
        {
            rot = transform.rotation;
            transform.rotation = Quaternion.RotateTowards(rot, targetRot, speed * Time.deltaTime);

            if (Quaternion.Angle(rot, targetRot) < 0.01f)
            {
                transform.rotation = targetRot;
                yield break;
            }

            yield return null;
        }
    }
}