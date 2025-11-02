using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHolder : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        GameObject mainCamObj = GameObject.Find("Main Camera");
        if (mainCamObj != null)
            mainCameraTransform = mainCamObj.transform;
    }



    [SerializeField] private float rotationSmoothSpeed = 5f;

    void Update()
    {
        if (mainCameraTransform != null)
        {
            // hier als refrenz für die Richtung in die gedreht wird die Position der Kamera nehmen , nicht die kameraeigene Rotation
            float targetYRotation = mainCameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation - 90f, 0f);

            // Weiche Interpolation zur Zielrotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }

}
