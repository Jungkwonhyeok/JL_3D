using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class BossSkill : MonoBehaviour
{
    public enum Type { SkeletonMagic, HandSpell, DarkSpell, Necromancer}; // 어떤 타입의 Skill인지 구분 해주는 열거형 변수
    public Type SkillType;
    public int damage;

    public GameObject HandSpell;
    public GameObject Explosion; //FireBall 폭발 효과
    public GameObject[] Skeleton;
    public Transform[] NecromancerPos;

    float PlayerSpeed = 10f;
    float hitCool = 1f;
    float hitDelay = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Wall" || other.gameObject.tag == "Shield") 
            && SkillType != Type.HandSpell) //HandSpell이 아니면 벽에 충돌 시 obj삭제
        {
            if (SkillType == Type.SkeletonMagic) // SkeletonMagic이면 폭발 효과를 남김
            {
                SpawnSkeletonMagicEffect();
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer == 7 && SkillType == Type.SkeletonMagic)
        {
            SpawnSkeletonMagicEffect();
            Destroy(gameObject); //Skeleton obj 삭제
        }
        else if(other.gameObject.layer == 7 && SkillType == Type.DarkSpell)
        {
            Transform[] necro = GameObject.FindWithTag("Necromancer").GetComponent<BossSkill>().NecromancerPos;
            foreach(Transform Pos in necro)
            {
                GameObject skeleton = Skeleton[Random.Range(0, Skeleton.Length)];
                GameObject enemy = Instantiate(skeleton, Pos.position, Pos.rotation, Pos.parent);
                enemy.GetComponent<Enemy>().ChaseStart();
            }
        }
    }

    void SpawnSkeletonMagicEffect()
    {
        if (Explosion != null)
        {
            GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(explosion, 2f);
        }
        if (HandSpell != null)
        {
            GameObject handspell = Instantiate(HandSpell,
                new Vector3(transform.position.x, 1f, transform.position.z), transform.rotation);
            Destroy(handspell, 2f);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (SkillType == Type.HandSpell && other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.speed = 0f;

            hitDelay += Time.deltaTime;   // 트리거에 닿아 있는 동안 시간 누적

                if (hitCool <= hitDelay)      // 쿨타임 1초 넘으면
                {
                    player.health -= damage;   // 데미지 1회 적용
                    hitDelay = 0;        // 타이머 초기화
                    player.StartCoroutine(player.OnDamage());
                }
        }
    }
    private void OnDestroy()
    {
        if (SkillType != Type.HandSpell) 
            return;

        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.speed = PlayerSpeed;
    }
}
