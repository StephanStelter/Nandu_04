using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSkills : MonoBehaviour
{
    public Button RightBtn;
    public Button LeftBtn;
    public GameObject gameobjectToMove;
    public TextMeshProUGUI currentPageText;

    public float moveDistance = 600f;
    public float moveDuration = 0.5f; // Dauer in Sekunden

    public int counter = 1;
    public int minCounter = 1;
    public int maxCounter = 2;
    private Vector3 startPosition;

    public bool moveRight = false;
    public bool moveLeft = false;

    public bool moveInSkilltree = false;
    public SkillTreeManager skillTreeManager;

    private bool isMoving = false;

    private void Update()
    {
        if (moveLeft)
        {
            MoveLeft();
        }

        if (moveRight)
        {
            MoveRight();
        }
    }

    public void Initialize()
    {
        startPosition = gameobjectToMove.transform.localPosition;
        CheckButtons();
        SetPageText();
    }

    public void MoveLeft()
    {
        if (counter <= minCounter || isMoving) return;

        counter--;
        Vector3 targetPosition = gameobjectToMove.transform.localPosition + new Vector3(moveDistance, 0f, 0f);
        StartCoroutine(MoveToPosition(targetPosition));
        CheckButtons();
        SetPageText();
        UpdateSkillDisplay();
    }

    public void MoveRight()
    {
        if (counter >= maxCounter || isMoving) return;

        counter++;
        Vector3 targetPosition = gameobjectToMove.transform.localPosition + new Vector3(-moveDistance, 0f, 0f);
        StartCoroutine(MoveToPosition(targetPosition));
        CheckButtons();
        SetPageText();
        UpdateSkillDisplay();
    }

    public void SetNewPage()
    {
        maxCounter++;
        CheckButtons();
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        RightBtn.interactable = false;
        LeftBtn.interactable = false;

        Vector3 startPos = gameobjectToMove.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            gameobjectToMove.transform.localPosition = Vector3.Lerp(startPos, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;

              

            yield return null;
        }

        gameobjectToMove.transform.localPosition = targetPosition;
        isMoving = false;
    }

    private void CheckButtons()
    {
        LeftBtn.interactable = counter > minCounter;
        RightBtn.interactable = counter < maxCounter;
    }

    private void SetPageText()
    {
        currentPageText.text = counter.ToString();
    }

    private void UpdateSkillDisplay()
    {
        if (moveInSkilltree)
        {
            if (skillTreeManager != null)
            {
                skillTreeManager.SetSkillpointDisplay();
            }
        }
    }
}
