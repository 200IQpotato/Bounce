using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EnemyDescriptionBox : MonoBehaviour
{
    public static EnemyDescriptionBox Instance { get; private set; }
    private Stats targetStats; //目標詳情
    [SerializeField] private GameObject descriptionBox; //詳情UI物件
    [SerializeField] private EnemyDescriptionEffect effectPrefab; //效果UI物件
    [SerializeField] private RectTransform effectContainer; //效果UI容器
    private Dictionary<EffectObject, EnemyDescriptionEffect> activeEffects = new Dictionary<EffectObject, EnemyDescriptionEffect>();
    [SerializeField] private Image enemyImage; //敵人圖片
    [SerializeField] private TextMeshProUGUI enemyNameText; //敵人名稱文字
    [SerializeField] private TextMeshProUGUI healthText; //敵人血量文字
    [SerializeField] private Slider healthSlider; //敵人血量滑桿

    private void Awake()
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

    public void RegisterStats(Stats stats)
    {
        targetStats = stats;
        EnemyBasicInfoUpdate();
        HealthBarUpdate();
        UpdateEffectUI();
        targetStats.OnHealthChanged += HealthBarUpdate;
        targetStats.OnMaxHealthChanged += HealthBarUpdate;
        targetStats.OnEffectChange += UpdateEffectUI;
    }

    public void UnregisterStats()
    {
        if (targetStats != null)
        {
            targetStats.OnHealthChanged -= HealthBarUpdate;
            targetStats.OnMaxHealthChanged -= HealthBarUpdate;
            targetStats.OnEffectChange -= UpdateEffectUI;
            targetStats = null;
        }
    }

    private void UpdateEffectUI()
    {
        if (targetStats == null) return;

        var distinctEffectObjects = new HashSet<EffectObject>();
        foreach (var effect in targetStats.effects)
            distinctEffectObjects.Add(effect.effectObject);

        // 更新效果UI
        foreach (var effectObject in distinctEffectObjects)
        {
            if (!activeEffects.ContainsKey(effectObject))
            {
                EnemyDescriptionEffect newEffectUI = Instantiate(effectPrefab, effectContainer);
                newEffectUI.SetEffect(effectObject, targetStats);
                activeEffects.Add(effectObject, newEffectUI);
            }
            else
            {
                activeEffects[effectObject].SetEffect(effectObject, targetStats);
            }
        }

        // 移除已經不存在的效果UI
        List<EffectObject> effectsToRemove = new List<EffectObject>();
        foreach (var kvp in activeEffects)
        {
            if (!targetStats.effects.Exists(e => e.effectObject == kvp.Key))
            {
                Destroy(kvp.Value.gameObject);
                effectsToRemove.Add(kvp.Key);
            }
        }
        foreach (var effectObj in effectsToRemove)
        {
            activeEffects.Remove(effectObj);
        }
    }

    private void EnemyBasicInfoUpdate() //設定圖片跟名字
    {
        enemyImage.sprite = targetStats.GetComponent<Enemy>().enemyInfo.enemySprite;
        enemyNameText.text = targetStats.GetComponent<Enemy>().enemyInfo.enemyID;
    }

    private void HealthBarUpdate()
    {
        healthSlider.value = (float)targetStats.health / targetStats.maxHealth;
        healthText.text = $"{targetStats.health}/{targetStats.maxHealth}";
    }

    public void Show()
    {
        descriptionBox.SetActive(true);
    }

    public void Hide()
    {
        descriptionBox.SetActive(false);
    }
}
