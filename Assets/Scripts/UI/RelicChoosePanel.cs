using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicChoosePanel : MonoBehaviour
{
    [SerializeField] private RelicChoose relicChoosePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Button confirmButton;
    private RelicObject selectRelic = null;
    private RelicChoose relicChoosed = null;
    private System.Action onCompleted;

    void Start ()
    {
        GameManager.Instance.RelicChooseEvent += OnOpen;   
    }

    public void Init( List<RelicObject> relicObjects, int relicCount )
    {
        bool hasRelic = false;
        if( relicObjects == null )
            relicObjects = new List<RelicObject>();

        foreach( RelicObject relicObject in relicObjects )
        {
            //if count < list count, didn't fixed
            if( !RelicManager.Instance.IsRelicEuipped( relicObject ) )
            {
                InstantiateRelicChoose( relicObject );
                hasRelic = true;
            }            
        }

        if ( relicObjects.Count < relicCount )
        {
            List<RelicObject> randomRelics = RelicManager.Instance.GetRandomRelic( relicCount - relicObjects.Count );
            if( randomRelics != null )
            {
                foreach( RelicObject relicObject in randomRelics )
                {
                    InstantiateRelicChoose( relicObject );
                }
                hasRelic = true;
            }
        }

        if( !hasRelic )
            OnClose();
        
    }

    public void SelectRelic( RelicObject relicObject, RelicChoose relicChoose )
    {
        selectRelic = relicObject;
        if( relicChoosed != null )
            relicChoosed.GetComponent<RelicChoose>().selectedImage.gameObject.SetActive( false );

        relicChoosed = relicChoose;
        relicChoosed.GetComponent<RelicChoose>().selectedImage.gameObject.SetActive( true );        
    }

    public void OnConfirmClick()
    {
        if( selectRelic != null )
        {
            RelicManager.Instance.AddRelicToPlayer( selectRelic );
            OnClose();
        }
    }

    public void OnOpen(List<RelicObject> relicObjects, int relicCount, System.Action onCompleted)
    {
        this.onCompleted = onCompleted;
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

        onCompleted?.Invoke();
        onCompleted = null;
    }

    public void GetRelicTest()
    {
        OnOpen( null, 3, null );
    }

    private void InstantiateRelicChoose( RelicObject relicObject )
    {
        RelicChoose relicChoose = Instantiate( relicChoosePrefab, contentTransform );
        relicChoose.Init( relicObject );
        RelicObject relic = relicObject;
        relicChoose.GetComponent<Button>().onClick.AddListener( () => SelectRelic( relic, relicChoose ) );
    }

    void OnDestroy()
    {
        GameManager.Instance.RelicChooseEvent -= OnOpen;
    }
}
