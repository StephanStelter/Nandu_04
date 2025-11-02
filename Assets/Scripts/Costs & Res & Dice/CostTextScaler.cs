using UnityEngine;
using TMPro;

public class CostTextScaler : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    private float minScale = 1f;
    private float maxScale = 6f;

    private float yPositionMin = -350;
    private float yPositionMax = -70;
    private float yPositionNew;
    private float xPositionMin = 0;
    private float xPositionMax = 60;
    private float xPositionNew;

    public Transform parentObject;

    private float calculatedScale;


    void Update()
    {
        ActiveTextMeshPro();
    }

    private void ActiveTextMeshPro()
    {
        if (textMesh.gameObject.activeSelf)
        {
            // Skalierung mit dem Mausrad anpassen
            // Textgröße
            textMesh.gameObject.transform.localScale = new Vector3(NewScrollScaleByZoom(), NewScrollScaleByZoom(), NewScrollScaleByZoom());

            // Textposition
            parentObject.transform.localPosition = new Vector3(xPositionNew, yPositionNew, parentObject.transform.localPosition.z);
        }
    }

    private float NewScrollScaleByZoom()
    {
        float newZoom = CameraMovement.Instance.currentZoom;
        // Textgröße
        calculatedScale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(CameraMovement.Instance.minZoom, CameraMovement.Instance.maxZoom, newZoom));

        // Textposition
        yPositionNew = Mathf.Lerp(yPositionMax, yPositionMin, Mathf.InverseLerp(CameraMovement.Instance.minZoom, CameraMovement.Instance.maxZoom, newZoom));
        xPositionNew = Mathf.Lerp(xPositionMax, xPositionMin, Mathf.InverseLerp(CameraMovement.Instance.minZoom, CameraMovement.Instance.maxZoom, newZoom));

        return calculatedScale;
    }
}