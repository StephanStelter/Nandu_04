using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [Header("Eigenschaften")]
    public string skillName;



    public bool skillacivated = false;
    public bool isunlocked = false;

    private Button skillButton;
    [SerializeField] private List<GameObject> unlockableObjects;
    [SerializeField] private TextMeshProUGUI btnText;
    [SerializeField] private Sprite inactivSprite;
    [SerializeField] private Sprite activSprite;
    [SerializeField] private Sprite usedSprite;

    private void Start()
    {
        skillButton = GetComponent<Button>();
        if (!isunlocked)
            skillButton.interactable = false;

        transform.name = skillName;

        btnText.text = skillName;

        Image image = GetComponent<Image>();
        image.sprite =inactivSprite;
    }

    public void ActivateSkill()
    {
        if (!skillacivated && skillButton.interactable)
        {
            // 1. Skill wir aktiviert, markieren, widerholtes klicken unterbinden
            skillacivated = true;
            skillButton.interactable = false;
            ColoredBtnActivatedSkill();

            // 2. Skill ausführen
            UnlockedSkill();

            // 3. Weg zu nächsten Skills freischalten
            foreach (GameObject gameObject in unlockableObjects)
            {
                Button unlockSkillButton = gameObject.GetComponent<Button>();

                unlockSkillButton.interactable = true;

                Skill skill = gameObject.GetComponent<Skill>();
                if (skill == null) { Debug.Log("skill == nul"); }

                skill.ColoredBtnUnlockSkill();
                Debug.Log("Unlocked!");
            }

        }
    }

    private void ColoredBtnActivatedSkill()
    {
        //skillButton.GetComponent<Image>().color = Color.green;

        Image image = GetComponent<Image>();
        image.sprite = usedSprite;
    }

    public void ColoredBtnUnlockSkill()
    {
        //skillButton.GetComponent<Image>().color = Color.yellow;

        Image image = GetComponent<Image>();
        image.sprite = activSprite;
    }

    private void UnlockedSkill()
    {

    }
}
