using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public static Dice Instance { get; set; }

    public SpritesEnty spritesEnty;

    [Header("SpriteRenderer für sichtbare Seite")]
    public SpriteRenderer visibleSideRenderer;
    public Image visibleSideImage;

    [Header("Würfelseiten-Sprites (1-6)")]
    public int spriteNumberRound; // Würfelseite speichern
    public string spriteNameRound; // Würfelseite speichern

    [Header("Animationseinstellungen")]
    public float rollDuration;  // Gesamtdauer
    public float rollInterval = 0.1f;  // Zeit zwischen Spritewechseln

    private bool isRolling = false;
    private int finalRoll;

    private int lastRoll = -1; // -1 = noch kein vorheriger Wurf

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (spritesEnty == null)
        {
            spritesEnty = FindObjectOfType<SpritesEnty>();
            if (spritesEnty == null) { Debug.LogWarning("SpritesEnty nicht gefunden!"); }
        }
    }

    public IEnumerator RollDicePerRound()
    {
        isRolling = true;

        rollDuration = Random.Range(.3f, 2f);

        // Würfeln nur für visuelle Animation
        float timer = 0f;
        while (timer < rollDuration)
        {
            int rollingSprit = Random.Range(0, spritesEnty.signSprites.Count);
            visibleSideImage.sprite = spritesEnty.GetSignSprite(spritesEnty.signSprites[rollingSprit].signName);
            timer += rollInterval;
            yield return new WaitForSeconds(rollInterval);
        }

        spriteNumberRound = finalRoll;
        spriteNameRound = spritesEnty.GetSignSprite(spritesEnty.signSprites[spriteNumberRound].signName).ToString();
        visibleSideImage.sprite = spritesEnty.GetSignSprite(spritesEnty.signSprites[spriteNumberRound].signName);

        isRolling = false;

        // Am Ende der Animation die Buttons wieder frei geben
        HandController.Instance.EndRoundButton.interactable = true;
        HandController.Instance.RecycleButton.interactable = true;
    }

    public int DiceRoll()
    {
        //int finalRoll;
        do
        {
            finalRoll = Random.Range(0, spritesEnty.signSprites.Count);
        } while (finalRoll == lastRoll && Random.value < 0.1f); //  // Wenn gleiche Zahl wie vorher, dann mit z.B. 10 % erlauben

        lastRoll = finalRoll;
        return finalRoll;
    }

    public IEnumerator StartDiceRoll()
    {
        // 🔒 Finales Ergebnis bestimmen – NICHT das letzte Animationsergebnis
        finalRoll = DiceRoll();
        //Debug.Log("Würfelergebnis: " + finalRoll);
        // Animation
        yield return StartCoroutine(RollDicePerRound());
    }

    public string GetDiceName(int _finalRoll)
    {
        return spritesEnty.GetSignSprite(spritesEnty.signSprites[_finalRoll].signName).ToString();
    }

    public Sprite GetSpriteOfDice(int _finalRoll)
    {
        if (_finalRoll < 0)
            return null;
        else
            return spritesEnty.GetSignSprite(spritesEnty.signSprites[spriteNumberRound].signName);
    }

    public Sign GetSignOfDice()
    {
        if (finalRoll < 0)
            return Sign.Bear; // Default-Wert
        else
            return (Sign)spritesEnty.signSprites[finalRoll].signName;
    }

    public int GetSpriteNumberRound()
    {
        return spriteNumberRound;
    }

    public float GetRollDuration()
    {
        return rollDuration;
    }
}
