using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicChoosePanel : MonoBehaviour
{
    [SerializeField] private RelicChoose relicChoosePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Button confirmButton;
    private RelicObject selectRelic = null;

    void Start ()
    {
        BattleManager.Instance.OnBattleEnd += OnOpen;   
    }

    public void Init( List<RelicObject> relicObjects, int relicCount )
    {
        bool hasRelic = false;
        if( relicObjects == null )
            relicObjects = new List<RelicObject>();

        foreach( RelicObject relicObject in relicObjects )
        {
            RelicChoose relicChoose = Instantiate( relicChoosePrefab, contentTransform );
            relicChoose.Init( relicObject );
            RelicObject relic = relicObject;
            relicChoose.GetComponent<Button>().onClick.AddListener( () => SelectRelic( relic ) );
            hasRelic = true;
        }

        if ( relicObjects.Count < relicCount )
        {
            List<RelicObject> randomRelics = RelicManager.Instance.GetRandomRelic( relicCount - relicObjects.Count );
            if( randomRelics != null )
            {
                foreach( RelicObject relicObject in randomRelics )
                {
                    RelicChoose relicChoose = Instantiate( relicChoosePrefab, contentTransform );
                    relicChoose.Init( relicObject );
                    RelicObject relic = relicObject;
                    relicChoose.GetComponent<Button>().onClick.AddListener( () => SelectRelic( relic ) );
                }
                hasRelic = true;
            }
        }

        if( !hasRelic )
            OnClose();
        
    }

    public void SelectRelic( RelicObject relicObject )
    {
        selectRelic = relicObject;
    }

    public void OnConfirmClick()
    {
        if( selectRelic != null )
        {
            RelicManager.Instance.AddRelicToPlayer( selectRelic );
            OnClose();
        }
    }

    public void OnOpen(List<RelicObject> relicObjects, int relicCount)
    {
        foreach( Transform child in contentTransform )
        {
            Destroy( child.gameObject );
        }  
        contentTransform.gameObject.SetActive( true );
        confirmButton.gameObject.SetActive( true );
        Init( relicObjects, relicCount );
    }

    public void OnClose()
    {
        contentTransform.gameObject.SetActive( false );
        confirmButton.gameObject.SetActive( false );
        foreach( Transform child in contentTransform )
        {
            Destroy( child.gameObject );
        }        
    }

    public void GetRelicTest()
    {
        OnOpen( null, 3 );
    }
}
