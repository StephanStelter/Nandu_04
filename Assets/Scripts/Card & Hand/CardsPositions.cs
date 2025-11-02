using UnityEngine;

public class CardsPositions : MonoBehaviour
{
    public static CardsPositions Instance { get; private set; }



    [SerializeField] private Transform minPos;
    [SerializeField] private Transform maxPos;

    public float distanceBetweenPoints;
    private float cardWidth;



    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardWidth = 90;
    }

    // Update is called once per frame
    void Update()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        distanceBetweenPoints = ((activePlayer.cardsInHand.Count - 1) * cardWidth) / 2;

        minPos.localPosition = new Vector3(transform.localPosition.x - distanceBetweenPoints, transform.localPosition.y, transform.localPosition.z);

        maxPos.localPosition = new Vector3(transform.localPosition.x + distanceBetweenPoints, transform.localPosition.y, transform.localPosition.z);
    }

    public Transform GetMinPos()
    {
        return minPos;
    }

    public Transform GetMaxPos()
    {
        return maxPos;
    }

    public float GetDistance()
    {
        return cardWidth;
    }
}
