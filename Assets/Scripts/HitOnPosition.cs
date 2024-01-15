using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Компонент, который наносит урон по клетке в которой находится объект
/// </summary>
public class HitOnPosition : MonoBehaviour
{
    [SerializeField]
    protected bool affectEnemies;
    void Update()
    {
        GameMaster.instance.HitPoint(((Vector2)transform.position).Round(),affectEnemies);
    }
}
