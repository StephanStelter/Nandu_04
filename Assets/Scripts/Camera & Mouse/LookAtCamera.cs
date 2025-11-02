using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Die Hauptkamera finden
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // In Richtung Kamera drehen
            transform.LookAt(mainCamera.transform);

            // Optional: Falls es nur in eine Achse schauen soll (z. B. nur in der Y-Rotation)
            // Vector3 direction = mainCamera.transform.position - transform.position;
            // direction.y = 0; // Y-Achse fixieren
            // transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
