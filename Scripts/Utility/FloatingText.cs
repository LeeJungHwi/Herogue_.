using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    // 텍스트 이동속도
    [SerializeField] private float moveSpeed;

    // 텍스트 투명속도
    [SerializeField] private float alphaSpeed;

    // TMP
    private TextMeshPro text;

    // TMP Color
    [HideInInspector] public Color alpha;

    // 텍스트 반납 타임
    public float waitTime;

    // 오브젝트 타입
    [SerializeField] private ObjType type;

    // 오브젝트 풀
    private PoolingManager poolingManager;

    private void Start()
    {
        // TMP
        text = GetComponent<TextMeshPro>();

        // TMP Color
        alpha = text.color;

        // 오브젝트 풀
        poolingManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolingManager>();
    }

    private void Update()
    {
        // 텍스트가 위로올라가면서 투명
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;

        // 데미지 텍스트 반납
        if(waitTime <= 0) poolingManager.ReturnObj(gameObject, type);
        else waitTime -= Time.deltaTime;
    }
}