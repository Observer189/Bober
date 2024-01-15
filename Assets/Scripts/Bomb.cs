using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("Время до взрыва бомбы с момента ее появления")]
    [SerializeField]
    protected float detonationTime;
    [Tooltip("Название параметра анимации, отвечающего за скорость анимации бомбы")]
    [SerializeField]
    protected string fireSpeedParameterName = "FireSpeed";
    [Tooltip("Префаб, который будет заспавнен на место бомбы по истечению ее срока жизни")]
    [SerializeField]
    protected GameObject explosionPrefab;

    protected Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.SetFloat(fireSpeedParameterName,1.0f/detonationTime);
        StartCoroutine(Detonate());
        Messenger.AddListener("EndGame",OnEndGame);
    }
    

    protected IEnumerator Detonate()
    {
        yield return new WaitForSeconds(detonationTime);

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    void OnEndGame()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("EndGame",OnEndGame);
        GameMaster.instance.SetBomb(((Vector2)transform.position).Round(),false);
    }
}
