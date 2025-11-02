using System.Collections;
using UnityEngine;

public class CardBasicBahavior : MonoBehaviour
{
    public int cardID;

    public AnimationCurve moveCurve; // Animation Curve für die Bewegung
    public int moveUp;
    public float moveDurationToHand;
    public float moveDurationToEnd;
    public Vector3 startScale;
    public Vector3 endScale = new Vector3(1, 1, 1);

    public bool moveUpToogle = false;

    public GameObject LockedInfo;
    public GameObject InDeckInfo;

    public bool isInDeck;
    public bool isLocked;
    public bool isInHand;

    public int skillpointsToUnlock;

    [Header("Karte drehen")]
    public GameObject front;
    public GameObject back;
    public float flipDuration;
    private bool isFlipping = false;
    public AnimationCurve flipCurve;  // optional, z. B. EaseInOut

    private void Start()
    {
        front.SetActive(true);
        back.SetActive(false);
    }

    //Button Event
    public void ClickedOnCard()
    {
        if (isInHand)
        {
            Debug.Log("Clicked on Card");

            StartCoroutine(MoveCardUpAndDown());
        }

        if (!isInHand)
        {
            Debug.Log("Clicked on Card to add to Hand");
            SetDetailsInWindow();
        }
    }

    public void SetDetailsInWindow()
    {
        CardManager cardManager = Object.FindFirstObjectByType<CardManager>();
        if (cardManager != null)
        {
            cardManager.SetCardDetailsInWindow(gameObject.GetComponent<CardPropertiesTest>(), gameObject.GetComponent<CardBasicBahavior>());
        }
    }

    public void SetStatus()
    {
        if (isLocked)
        {
            LockCard();
            return;
        }
        if (isInDeck)
        {
            CardInDeck();
            return;
        }
        if (!isInDeck && !isLocked)
        {
            CardNotInDeck();
        }

    }

    // Setzt die Karte auf "Gesperrt"
    public void LockCard()
    {
        isLocked = true;
        isInDeck = false;
        LockedInfo.SetActive(true);
        InDeckInfo.SetActive(false);
    }

    // Setzt die Karte auf "Entsperrt"
    public void UnlockCard()
    {
        isLocked = false;

        LockedInfo.SetActive(false);
        InDeckInfo.SetActive(false);
    }

    // Setzt die Karte auf "Im Deck"
    public void CardInDeck()
    {
        isLocked = false;
        isInDeck = true;

        LockedInfo.SetActive(false);
        InDeckInfo.SetActive(true);
    }

    public void CardNotInDeck()
    {
        isLocked = false;
        isInDeck = false;

        LockedInfo.SetActive(false);
        InDeckInfo.SetActive(false);
    }

    // Bewegt die Karte nach oben und unten basierend auf dem aktuellen Zustand (ausgewählte Karte)
    private IEnumerator MoveCardUpAndDown()
    {
        // Karte um 360° drehen
        Flip();

        if (moveUpToogle == false)
        {
            // Alle anderen Karten zurücksetzen
            CardsInHandHandler newCards = Object.FindFirstObjectByType<CardsInHandHandler>();
            newCards.ResetHighlightedCard();

            // Karte nach oben bewegen
            moveUpToogle = true;
            yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, transform.localPosition + new Vector3(0f, moveUp, 0f), moveDurationToHand));
        }
        else
        {
            // Karte nach unten bewegen
            moveUpToogle = false;
            //Flip();
            yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, transform.localPosition - new Vector3(0f, moveUp, 0f), moveDurationToHand));
        }
    }

    // Bewegt die Karte in drei Schritten vom Spawnpunkt zu Handkarten: hoch, seitwärts, runter
    public IEnumerator MoveCardInHandPosition(Vector3 target)
    {
        Vector3 startPos = transform.localPosition;

        // 1) Hochbewegung (relativ zum Startwert)
        yield return StartCoroutine(MoveOverTimeLocal(startPos, startPos + new Vector3(0f, 200f, 0f), moveDurationToHand));

        // Starte die Skalierungs-Animation parallel 
        StartCoroutine(CardScale(startScale, endScale));

        // 2) Seitwärtsbewegung zur Ziel-X (Y bleibt gleich während der Bewegung)
        Vector3 sideTarget = new Vector3(target.x, transform.localPosition.y, target.z);
        yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, sideTarget, moveDurationToHand * 2));

        // 3) Runterbewegung zur Ziel-Y (X ist bereits korrekt)
        Vector3 finalTarget = target;
        yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, finalTarget, moveDurationToHand));
    }

    // Bewegt die Karte seitwärts Schritten, um die neue Reihenfolge in der Hand darzustellen
    public IEnumerator MoveCardNewOrder(Vector3 target)
    {
        // 1) Seitwärtsbewegung zur Ziel-X (Y bleibt gleich während der Bewegung)
        Vector3 sideTarget = new Vector3(target.x, transform.localPosition.y, target.z);
        yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, sideTarget, moveDurationToHand));
    }

    public IEnumerator MoveCardToEndPosition(Vector3 target)
    {
        Vector3 startPos = transform.localPosition;

        // 1) Hochbewegung (relativ zum Startwert)
        yield return StartCoroutine(MoveOverTimeLocal(startPos, startPos + new Vector3(0f, 200f, 0f), moveDurationToEnd));

        // Starte die Skalierungs-Animation parallel 
        StartCoroutine(CardScale(endScale, startScale));

        // 2) Seitwärtsbewegung zur Ziel-X (Y bleibt gleich während der Bewegung)
        Vector3 sideTarget = new Vector3(target.x, transform.localPosition.y, target.z);
        yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, sideTarget, moveDurationToEnd * 2));

        // 3) Runterbewegung zur Ziel-Y (X ist bereits korrekt)
        Vector3 finalTarget = target;
        yield return StartCoroutine(MoveOverTimeLocal(transform.localPosition, finalTarget, moveDurationToEnd));

        // Karte zerstören
        DestroyCard();
    }

    // Allgemeine Methode, die eine Bewegung von Start- zu Endposition über eine bestimmte Dauer mit einer AnimationCurve durchführt
    private IEnumerator MoveOverTimeLocal(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Normierte Zeit von 0 bis 1
            float t = Mathf.Clamp01(elapsed / duration);

            // AnimationCurve auswerten (z. B. Beschleunigen → Halten → Bremsen)
            float curveValue = moveCurve.Evaluate(t);

            // Kurvenwert für die Bewegung verwenden
            transform.localPosition = Vector3.Lerp(start, end, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end; // Sicherstellen, dass die Endposition genau erreicht wird
    }

    private IEnumerator CardScale(Vector3 _startScale, Vector3 _endScale)
    {
        float elapsed = 0f;
        float duration = .8f;
        while (elapsed < duration)
        {
            // Normierte Zeit von 0 bis 1
            float t = Mathf.Clamp01(elapsed / duration);

            // AnimationCurve auswerten (z. B. Beschleunigen → Halten → Bremsen)
            float curveValue = moveCurve.Evaluate(t);

            // Kurvenwert für die Skalierung verwenden
            transform.localScale = Vector3.Lerp(_startScale, _endScale, curveValue);

            elapsed += Time.deltaTime;

            yield return null; // hier wird auf den nächsten Frame gewartet
        }

        transform.localScale = _endScale; // Sicherstellen, dass die Endskalierung genau erreicht wird
    }


    public void InactivateStatus()
    {
        LockedInfo.SetActive(false);
        InDeckInfo.SetActive(false);
    }

    // Zerstört die Karte
    public void DestroyCard()
    {
        Destroy(gameObject);
    }

    // Startet die Flip-Animation
    public void Flip()
    {
        // Verhindert mehrfaches Starten der Animation
        if (isFlipping) return;

        StartCoroutine(FlipRoutine());
    }

    // Die eigentliche Flip-Animation als Coroutine
    private IEnumerator FlipRoutine()
    {
        // Verhindert mehrfaches Starten der Animation
        if (isFlipping) yield break;
        isFlipping = true;

        // Hilfsfunktion: wert aus AnimationCurve holen oder linear fallback
        System.Func<float, float> Eval = (x) =>
        {
            return (flipCurve != null && flipCurve.keys.Length > 0) ? flipCurve.Evaluate(x) : x;
        };

        float quarter = flipDuration / 4f; // jede 90°-Phase

        float time;
        float t;
        float angle;
        float scaleX;

        // ====== 1. FRONT -> BACK (0° -> 90°) ======
        time = 0f;
        while (time < quarter)
        {
            time += Time.deltaTime;
            t = Mathf.Clamp01(time / quarter);
            angle = Mathf.Lerp(0f, 90f, Eval(t));
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            // optionaler Fake-3D Scale-Effekt
            scaleX = 1f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angle)) * 0.3f;
            transform.localScale = new Vector3(scaleX, 1f, 1f);

            yield return null;
        }

        // Mitte: switch auf Back
        front.SetActive(false);
        back.SetActive(true);

        // ====== 2. BACK aufklappen (90° -> 0°) ======
        time = 0f;
        while (time < quarter)
        {
            time += Time.deltaTime;
            t = Mathf.Clamp01(time / quarter);
            angle = Mathf.Lerp(90f, 0f, Eval(t));
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            scaleX = 1f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angle)) * 0.3f;
            transform.localScale = new Vector3(scaleX, 1f, 1f);

            yield return null;
        }

        // ====== 3. BACK -> FRONT (0° -> 90°) ======
        time = 0f;
        while (time < quarter)
        {
            time += Time.deltaTime;
            t = Mathf.Clamp01(time / quarter);
            angle = Mathf.Lerp(0f, 90f, Eval(t));
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            scaleX = 1f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angle)) * 0.3f;
            transform.localScale = new Vector3(scaleX, 1f, 1f);

            yield return null;
        }

        // Mitte: switch zurück auf Front
        front.SetActive(true);
        back.SetActive(false);

        // ====== 4. FRONT aufklappen (90° -> 0°) ======
        time = 0f;
        while (time < quarter)
        {
            time += Time.deltaTime;
            t = Mathf.Clamp01(time / quarter);
            angle = Mathf.Lerp(90f, 0f, Eval(t));
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            scaleX = 1f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angle)) * 0.3f;
            transform.localScale = new Vector3(scaleX, 1f, 1f);

            yield return null;
        }

        // Aufräumen
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        isFlipping = false;
    }


    public void CardScriptTest()
    {
        Debug.Log("CardBasic: InDeck: " + isInDeck);
    }
}





