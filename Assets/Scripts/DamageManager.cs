using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set;}
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Canvas worldCanvas;
    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Show(float damage, Vector3 position)
    {
        var go = Instantiate(damageTextPrefab, worldCanvas.transform);

        go.transform.position = position;
        
        //스크립트 세팅
        var dt = go.GetComponent<DamageText>();
        dt.Setup(damage);
    }
}
