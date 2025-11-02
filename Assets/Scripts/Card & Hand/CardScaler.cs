using System.Collections;
using UnityEngine;

public class CardScaler : MonoBehaviour
{
    public static CardScaler Instance { get; private set; }


    private Vector3 startscale = new Vector3(2f, 2f, 2f); // Startgröße
    private Vector3 startscaleInUI = new Vector3(.055f, .042f, .1f); // Startgröße in UI
    private Vector3 endscale = new Vector3(4f, 4f, 4f);
    private float duration = .3f; // Dauer der Skalierung in Sekunden
    private float elapsedTime = 0f;

    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Eine Instanz existiert bereits, diese sollte zerstört werden
            Destroy(gameObject);
        }
    }

    private IEnumerator ScaleUpCoroutine(GameObject gameObject)
    {
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Erhöhe die verstrichene Zeit
            elapsedTime += Time.deltaTime;

            // Berechne den normierten Wert (zwischen 0 und 1)
            float t = elapsedTime / duration;

            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, endscale, t);

            // Warte bis zum nächsten Frame
            yield return null;
        }
    }

    private IEnumerator ScaleDownCoroutine(GameObject gameObject)
    {
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Erhöhe die verstrichene Zeit
            elapsedTime += Time.deltaTime;

            // Berechne den normierten Wert (zwischen 0 und 1)
            float t = elapsedTime / duration;

            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, startscale, t);

            // Warte bis zum nächsten Frame
            yield return null;
        }
    }

    private IEnumerator ScaleCoroutine(GameObject gameObject, Vector3 endScale, float _duration)
    {
        elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            // Erhöhe die verstrichene Zeit
            elapsedTime += Time.deltaTime;

            // Berechne den normierten Wert (zwischen 0 und 1)
            float t = elapsedTime / _duration;

            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, endScale, t);

            // Warte bis zum nächsten Frame
            yield return null;
        }
    }

    public void StartScaleDownRoutine(GameObject gameObject)
    {
        StartCoroutine(ScaleDownCoroutine(gameObject));
    }

    public void StartScaleUpRoutine(GameObject gameObject)
    {
        StartCoroutine(ScaleUpCoroutine(gameObject));
    }

    public void StartScaleRoutine(GameObject gameObject, Vector3 endScale, float _duration)
    {
        StartCoroutine(ScaleCoroutine(gameObject, endScale, _duration));
    }
}
