using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private GameObject currentActiveScreen = null;
    [SerializeField] private GameObject mapScreen;
    [SerializeField] private GameObject retryButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        retryButton.SetActive(false);
    }

    public void Screen_On_Off( GameObject target )
    {
        if (target == null) return;
        bool isOpen = target.activeSelf;
        GameManager.Instance.isUIBlockingInput = !GameManager.Instance.isUIBlockingInput;
        if ( isOpen )
        {
            target.SetActive(false);
            currentActiveScreen = null;
            Time.timeScale = 1f;
        }
        else
        {
            if (currentActiveScreen != null)
                currentActiveScreen.SetActive(false);            

            target.SetActive(true);
            currentActiveScreen = target;
            Time.timeScale = 0f;
        }
    }

    public void CloseCurrentScreen()
    {
        if (currentActiveScreen != null)
            Screen_On_Off(currentActiveScreen);
    }

    public void OpenMapScreen()
    {
        Screen_On_Off(mapScreen);
    }

    public void ShowRetryButton()
    {
        retryButton.SetActive(true);
    }

    public void HideRetryButton()
    {
        retryButton.SetActive(false);
    }
}
