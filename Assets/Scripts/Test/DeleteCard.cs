using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCard : MonoBehaviour
{
    // Karten in die Hand
    public Vector3 moveUpDelete;
    public Vector3 moveRightDelete;
    public Transform deletePointTransform;

    public bool startDeleteCards = false;

    private float moveSpeed = 100;

    public int deleteUp;
    public int deleteRight;

    private bool setPosition = false;

    // Update is called einmal pro Frame
    private void Update()
    {
        // Karten in die Hand
        if (startDeleteCards)
        {
            startDeleteCards = false; // Verhindert, dass die Bewegung mehrfach gestartet wird

            if (!setPosition)
            {
                SetDeletePosition();
            }

            StartCoroutine(DeleteCardsInHand());
        }
    }

    private void SetDeletePosition()
    {
        deleteUp = (int)(deletePointTransform.position.y - transform.position.y);
        deleteRight = (int)(deletePointTransform.position.x - transform.position.x);

        setPosition = true;

        GetPositions();
    }

    private void GetPositions()
    {
        moveUpDelete = transform.position + new Vector3(0, deleteUp, 0);
        moveRightDelete = transform.position + new Vector3(deleteRight, deleteUp, 0);
    }

    private IEnumerator DeleteCardsInHand()
    {
        // 1. Bewegung nach oben
        yield return StartCoroutine(MoveToPosition(moveUpDelete));

        // 2. Bewegung nach rechts
        yield return StartCoroutine(MoveToPosition(moveRightDelete));

        // 3. Löschen des GameObjects
        Destroy(gameObject);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.000001f)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            yield return null;
        }

        transform.position = targetPosition; // Stellt sicher, dass die Endposition exakt erreicht wird
    }

    public void SetDeletePoint(Transform newDeletePointTransform)
    {
        deletePointTransform = newDeletePointTransform;
        setPosition = false; // SetPosition muss zurückgesetzt werden, wenn der DeletePoint geändert wird
    }
}
