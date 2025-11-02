using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIShineController : MonoBehaviour
{
    public Material runtimeMaterial;

    void Start()
    {
        // Erstelle Material-Instanz zur Laufzeit (damit andere nicht überschrieben werden)
        Image img = GetComponent<Image>();
        if (runtimeMaterial != null)
        {
            img.material = Instantiate(runtimeMaterial);
        }
    }

    void Update()
    {
        if (GetComponent<Image>().material != null)
        {
            GetComponent<Image>().material.SetFloat("_CustomTime", Time.time);
        }
    }
}
