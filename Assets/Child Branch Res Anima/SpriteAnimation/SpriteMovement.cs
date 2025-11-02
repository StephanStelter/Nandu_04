using System.Collections;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{
    private float moveXRange = 100f; // Bereich für zufällige X-Verschiebung
    private float moveYRange = 80f; // Bereich für zufällige Y-Verschiebung
    private ResourceType resourceType;
    private int amount;

    public AnimationCurve moveCurve; // Animation Curve für die Bewegung


    public void StartMovement(Vector3 targtPosition)
    {
        StartCoroutine(MoveSprite(targtPosition));
    }

    private IEnumerator MoveSprite(Vector3 targetPosition)
    {
        // Schritt 1: Bewege das Sprite zum vom Spawnpunkt weg
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-moveXRange, moveXRange), UnityEngine.Random.Range(-moveYRange, moveYRange), 0);
        yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .2f));
        yield return StartCoroutine(MoveOverTime(transform.position, transform.position + randomOffset, UnityEngine.Random.Range(.1f, 1f)));

        // Schritt 2: Bewege das Sprite zum Zielpunkt
        yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .4f));
        yield return StartCoroutine(MoveOverTime(transform.position, targetPosition, UnityEngine.Random.Range(.2f, 3f)));

        //Schritt 3: Wert rechnen
        yield return StartCoroutine(CalculateValue());

        // Schritt 4: Zerstöre das Sprite nach der Bewegung       
        Destroy(gameObject);
    }
    private IEnumerator MoveOverTime(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Normierte Zeit von 0 bis 1
            float t = Mathf.Clamp01(elapsed / duration);

            // AnimationCurve auswerten (z. B. Beschleunigen → Halten → Bremsen)
            float curveValue = moveCurve.Evaluate(t);

            // Kurvenwert für die Bewegung verwenden
            transform.position = Vector3.Lerp(start, end, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end; // Sicherstellen, dass die Endposition genau erreicht wird
    }

    public void SetObjectStats(ResourceType _resourceType, int _amount)
    {
        resourceType = _resourceType;
        amount = _amount;
    }

    private IEnumerator CalculateValue()
    {
        UIResourceSummary.Instance.UpdateMainPlayerSummaryTextWithAnimationRound(resourceType, amount);
        yield return null;
    }

}
