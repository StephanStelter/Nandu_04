using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMoveInHands : MonoBehaviour
{

    // Karten in die Hand
    public Vector3 moveUpInHands;
    public Vector3 moveRightInHands;
    public Vector3 moveDownInHands; // Korrigiert: moveDownInHands

    public bool startMovingInHand = false;

    private float moveSpeed = 100;

    public int upInHand;
    public int rightInHand;
    public int downInHand;

    private bool setPosition = false;
    private bool getPosition = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (setPosition)
        {
            setPosition = false;

            // ZielPositionen ermitteln
            GetPositions();
        }

        // Karten in die Hand
        if (startMovingInHand && getPosition)
        {
            startMovingInHand = false; // Verhindert, dass die Bewegung mehrfach gestartet wird

            StartCoroutine(MoveCardInHand());
        }
    }

    private void GetPositions()
    {
        // ZielPositionen setzen
        moveUpInHands = transform.position + new Vector3(0, upInHand, 0);
        moveRightInHands = transform.position + new Vector3(rightInHand, upInHand, 0);
        moveDownInHands = transform.position + new Vector3(rightInHand, downInHand, 0);

        // ZielPositionen fertig
        getPosition = true;

        // Kartenbewegung starten
        startMovingInHand = true;
    }

    private IEnumerator MoveCardInHand()
    {
        // 1. Bewegung nach oben
        yield return StartCoroutine(MoveToPosition(moveUpInHands));

        // 2. Bewegung nach rechts
        yield return StartCoroutine(MoveToPosition(moveRightInHands));

        // 3. Bewegung nach unten
        yield return StartCoroutine(MoveToPosition(moveDownInHands)); // Korrigiert: moveDownInHands
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

    public void SetHandPosition(Transform handPositionTransform, int cardOffset)
    {
        upInHand = 20;

        rightInHand = (int)handPositionTransform.position.x + cardOffset - (int)transform.position.x;

        downInHand = (int)handPositionTransform.position.y - (int)transform.position.y;

        // PositionsVariablen gesetzt
        setPosition = true;

        getPosition = false;
    }
}
