using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    public Rarity socketType = Rarity.None;
    public CardProperties card;
    public GameObject structure = null;
    public bool isBlocked = false;
    private Renderer socketRenderer;
    private Material originalMaterial;    // Das aktuell eingesetzte Basis‑Material
    private Color originalColor;          // Die Original‑Farbe des Basis‑Materials

    // Array für bis zu 6 verschiedene Basis‑Materialien
    [SerializeField]
    private Material[] baseMaterials = new Material[6];

    // Index des aktuell ausgewählten Basis‑Materials
    [SerializeField, Range(0, 5)]
    private int baseMaterialIndex = 0;

    // Wie stark das Highlight aufhellen soll (z.B. 1.2 = 20% heller)
    [SerializeField, Range(1f, 2f)]
    private float highlightMultiplier = 1.2f;

    private void Awake()
    {
        socketRenderer = GetComponent<Renderer>();
        if (socketRenderer == null)
        {
            Debug.LogError("Kein Renderer am GameObject gefunden!");
            return;
        }

        // Start‐Material setzen
        SetBaseMaterial(Rarity.None);
    }

    public void SetBaseMaterial(Rarity rarity)
    {
        if (socketRenderer == null || baseMaterials == null || baseMaterials.Length == 0)
            return;

        // Cast enum auf int und clamp auf [0, baseMaterials.Length-1]
        int idx = Mathf.Clamp((int)rarity, 0, baseMaterials.Length - 1);

        // Material setzen
        socketRenderer.material = baseMaterials[idx];

        // Originalwerte sichern
        originalMaterial = socketRenderer.material;
        originalColor = originalMaterial.color;
        baseMaterialIndex = idx;
    }

    public void HighlightSocket()
    {
        if (socketRenderer == null || originalMaterial == null)
            return;

        // Wir klonen das Material, damit wir den Farbwechsel wieder rückgängig machen können:
        socketRenderer.material = Instantiate(originalMaterial);

        // Farbwerte aufhellen in RGB
        Color c = socketRenderer.material.color;
        c.r = Mathf.Clamp01(c.r * highlightMultiplier);
        c.g = Mathf.Clamp01(c.g * highlightMultiplier);
        c.b = Mathf.Clamp01(c.b * highlightMultiplier);
        socketRenderer.material.color = c;
    }

    public void HighlightSocketRandom()
    {
        HighlightSocket(); // gleiche Logik, Variation könntest du ergänzen
    }

    public void ResetHighlight()
    {
        if (socketRenderer == null || originalMaterial == null)
            return;

        socketRenderer.material = originalMaterial;
        socketRenderer.material.color = originalColor;
    }
}