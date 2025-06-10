using UnityEngine;

public class Exp : MonoBehaviour
{
    //자석의 세기
    public float magnetStrength = 5f;
    //거리에 따른 자석 효과 적용
    public float distanceStrength = 10f;
    //인력 1 척력 -1
    public int magnetDirection = 1;
    public bool looseMagnet = true;

    private Transform trans;
    private Rigidbody2D thisRb;
    private Transform magnetTrans;
    private bool magnetInZone;

    private void Awake()
    {
        trans = transform;
        thisRb = trans.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //자력 범위 내에 있으면
        if(magnetInZone)
        {
            //자석으로 향하는 벡터 설정
            Vector2 directionToMagnet = magnetTrans.position - trans.position;
            //Distance로 거리를 a,b사이의 거리를 구함
            float distance = Vector2.Distance(magnetTrans.position, trans.position);
            //거리에 따른 힘이 달라져야하므로 거리로 나눔
            float magnetDistanceStr = (distanceStrength / distance) * magnetStrength;
            //힘의 크기와 방향이 있으니까 물리적 힘 구현 rigdbodt가 있어야 가능
            thisRb.AddForce(magnetDistanceStr * (directionToMagnet * magnetDirection), ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Magnet")
        {
            magnetTrans = collision.transform;
            magnetInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag =="Magnet" && looseMagnet)
        {
            magnetInZone = false;
        }
    }
}
