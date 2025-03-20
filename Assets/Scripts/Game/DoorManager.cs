using UnityEngine;

public enum DoorKeyType
{
    Red,
    Green,
    Blue,
    Yellow
}

public class DoorManager : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] Vector3 closePosition;
    [SerializeField] Vector3 openPosition;
    public bool usesLock; // If the door uses a lock system
    public DoorKeyType doorKeyType;
    public PlayerStatsHandler playerStatsHandler;

    private bool isOpening = false;
    private bool isClosing = false;
    private float doorSpeed = 2f; // Speed of the door movement

    void Start()
    {
        playerStatsHandler = FindAnyObjectByType<PlayerStatsHandler>();
        soundManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SoundManager>();
        closePosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (usesLock)
            {
                if (CheckKey(playerStatsHandler))
                {
                    StopClosing();
                    StartOpening();
                    soundManager.Play("doorOpen");
                }
            }
            else
            {
                StopClosing();
                StartOpening();
                soundManager.Play("doorOpen");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            StopOpening();
            StartClosing();
            if (Vector3.Distance(transform.position, closePosition) >= 0.25f)
            {
                soundManager.Play("doorClose");
            }
        }
    }

    bool CheckKey(PlayerStatsHandler playerStatsHandler)
    {
        foreach (DoorKeyScript keyScript in playerStatsHandler.doorKeyScripts)
        {
            if (keyScript.doorKeyType == doorKeyType)
            {
                return true;
            }
        }

        return false;
    }

    void StartOpening()
    {
        if (!isOpening)
        {
            isOpening = true;
            isClosing = false;
            
            StopAllCoroutines();
            StartCoroutine(MoveDoor(new Vector3(closePosition.x, openPosition.y, closePosition.z)));
        }
    }

    void StopOpening()
    {
        isOpening = false;
    }

    void StartClosing()
    {
        if (!isClosing)
        {
            isClosing = true;
            isOpening = false;
            
            StopAllCoroutines();
            StartCoroutine(MoveDoor(closePosition));
        }
    }

    void StopClosing()
    {
        isClosing = false;
    }

    System.Collections.IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // Snap to position to prevent small discrepancies
    }
}
