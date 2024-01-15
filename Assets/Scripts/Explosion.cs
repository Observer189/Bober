using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    protected float timeBeforeDeath;
    [SerializeField]
    protected GameObject shellPrefab;
    void Start()
    {
        Messenger.AddListener("EndGame",OnEndGame);
        Destroy(gameObject, timeBeforeDeath);
        float angle = 0;
        //Спавним осколки во все стороны от центра взрыва
        for (int i = 0; i < 4; i++)
        {
            var shell = Instantiate(shellPrefab, transform.position, Quaternion.Euler(0, 0, angle))
                .GetComponent<Projectile>();
            shell.Direction = Vector2.right.Rotate(angle).Round();
            angle += 90;
        }
    }

    void OnEndGame()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("EndGame", OnEndGame);
    }
}
