using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BEAT{
    ANGLE,
    HOMING,
    HOMING_Z,
}

public class Beat : MonoBehaviour {

    // 외부 피라미터(Inspector 표시)
    public BEAT fireType = BEAT.HOMING;

    public float BeatDamage = 1;

    public bool penetration = false;

    public float lifeTime = 3.0f;
    public float speedV = 10.0f;
    public float speedA = 0.0f;
    public float angle = 0.0f;
    public float homingTime = 0.0f;
    public float homingangleV = 180.0f;
    public float homingangleA = 20.0f;

    public Vector3 beatScaleV = Vector3.zero;
    public Vector3 beatScaleA = Vector3.zero;

    public Sprite hiteSprite;
    public Vector3 hitEffectScale = Vector3.one;
    public float rotateVt = 360.0f;

    // ====== 외부 파라미터 =========
    [System.NonSerialized] public Transform owner;
    [System.NonSerialized] public GameObject targetObject;
    [System.NonSerialized] public bool attackEnabled;

    // ===== 내부 파라미터 =====
    float fireTime;
    Vector3 posTarget;
    float homingAngle;
    Quaternion homingRotate;
    float speed;

    // ===코드(Monobehaviour 기본 기능 구현) =====
    void Start()
    {
        // 주인 검사 
        if (!owner) {
            return;
        }

        // 초기화 
        targetObject = new GameObject();
        posTarget = targetObject.transform.position +
            new Vector3(0.0f, 1.0f, 0.0f);

        switch (fireType)
        {
            case BEAT.ANGLE:
                speed = (owner.localScale.x < 0.0f) ? -speedV : +speedA;
                break;
            case BEAT.HOMING:
                speed = speedV;
                homingRotate = Quaternion.LookRotation(posTarget - transform.position);
                break;
            case BEAT.HOMING_Z:
                speed = speedV;
                break;
        }
        fireTime = Time.fixedTime;
        homingAngle = angle;
        attackEnabled = true;
        Destroy(this.gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 주인 검사 
        if (!owner)
        {
            return;
        }
        // 자기 자신에게 닿았는지 검사 
        if ((other.isTrigger ||
            (owner.tag == "Player" && other.tag == "PlayerBody") ||
            (owner.tag == "Player" && other.tag == "PlayerArm") ||
            (owner.tag == "Player" && other.tag == "PlayerArmBullet") ||
            (owner.tag == "Enemy" && other.tag == "EnemyBody") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArm") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArmBullet"))) {
            return;
        }

        // 벽에 닿았는지 검사 
        if (!penetration)
        {
            GetComponent<SpriteRenderer>().sprite = hiteSprite;
            GetComponent<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0.5f);
            transform.localScale = hitEffectScale;
            Destroy(this.gameObject, 0.1f);
        }
    }

    void Update()
    {
        // 스프라이트 이미지 회전처리 
        transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotateVt);
    }

    void FixedUpdate()
    {
        // 타깃설정 
        bool homing = ((Time.fixedTime - fireTime) < homingTime);
        if (homing)
        {
            posTarget = targetObject.transform.position +
                new Vector3(0.0f, 1.0f, 0.0f);
        }

        // 호밍 처리 
        switch (fireType) {
            case BEAT.ANGLE:    // 지정한 각도로 발사 
                rigidbody2D.velocity = Quaternion.Euler(0.0f, 0.0f, angle) *
                    new Vector3(speed, 0.0f, 0.0f);
                break;

            case BEAT.HOMING:   // 완전호밍
                {
                    if (homing) {
                        homingRotate = Quaternion.LookRotation(
                            posTarget = transform.position);
                    }
                    Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
                    rigidbody2D.velocity = Quaternion.Euler(0.0f, 0.0f, angle) * vecMove;
                }
                break;
        }
    }
}

