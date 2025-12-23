using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fellow : MonoBehaviour
{
    public Transform target; // 따라다닐 대상(플레이어 등)
    public Vector3 offset;   // 대상과의 상대 위치(거리/방향)

    void Update()
    {
        // 매 프레임마다 대상의 위치를 기준으로 일정 거리(offset)를 유지하며 이동
        transform.position = target.position + offset;
    }
}