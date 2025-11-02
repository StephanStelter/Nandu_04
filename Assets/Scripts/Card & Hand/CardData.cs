using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string cardID;
    public string cardName;
    public string description;
    public Sprite artwork;
    public int manaCost;
    public CardType type; // Enum z. B. Spell, Creature, etc.
}