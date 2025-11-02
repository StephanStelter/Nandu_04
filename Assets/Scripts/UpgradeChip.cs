using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class UpgradeChip : MonoBehaviour
{
    public Coroutine moveRoutine;
    public Settlement referenceSettlement;
    public Transform referenceHex;
    public TMP_Text tmpText;
    private Vector3 initialPosition;

    private void Update()
    {
        if (moveRoutine == null) Vibrate();
    }

    public void SetChip(Transform hex, Settlement settlement)
    {
        referenceHex = hex;
        referenceSettlement = settlement;
        tmpText.text = settlement.name;

    }
    public void SpawnChip(List<Vector3> targetPos)
    {
        MoveAlongPath(
            targetPos,
            3000f,
            180,
            "local"
        );
    }

    public void Vibrate()
    {
        float frequency = 1f;       // Schwingungen pro Sekunde (niedrig = langsamer)
        float amplitude = 5f;    // Ausschlag der Vibration (niedrig = seicht)

        // Leichte Sinusbewegung auf der Y-Achse
        float offsetY = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;

        transform.localPosition = initialPosition + new Vector3(0, offsetY, 0);
    }

    public void MoveAlongPath(List<Vector3> pathPoints, float speed = 800f, float rotation = 0f, string transformMode = "local")
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPoints(pathPoints, speed, rotation, transformMode));
    }
    private IEnumerator MoveToPoints(List<Vector3> targets, float speed, float rotation, string transformMode)
    {
        Transform tf = this.transform;

        int rotationSpeed = 700;  // Geschwindigkeit der Rotation

        foreach (Vector3 target in targets)
        {
            bool moving = true;
            bool rotating = true;
            float rotated = 0f;

            while (moving || rotating)
            {
                // Bewegung
                if (moving)
                {
                    float distance = Vector3.Distance(tf.localPosition, target);
                    if (distance > 0.01f)
                    {
                        tf.localPosition = Vector3.MoveTowards(tf.localPosition, target, speed * Time.deltaTime);
                    }
                    else
                    {
                        tf.localPosition = target;
                        moving = false;
                    }
                }

                // Rotation
                if (rotating)
                {
                    float step = Mathf.Min(rotationSpeed * Time.deltaTime, rotation - rotated);
                    if (rotated < rotation)
                    {
                        tf.Rotate(Vector3.up, step);
                        rotated += step;
                    }
                    if (rotated >= rotation)
                    {
                        tf.localEulerAngles = new Vector3(0, 0, 0);
                        rotating = false;
                    }
                }

                yield return null;
            }
        }

        moveRoutine = null;
        initialPosition = this.transform.localPosition;
    }
}
