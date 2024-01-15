using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2Int Direction { get; set; }
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float flyDistance;
    [Tooltip("Время после того как пуля закончила лететь на заданное расстояние перед ее разрушением" +
             "Сама пуля уже будет отключена, но нужно дать следу от нее исчезнуть естественным образом")]
    [SerializeField]
    protected float timeBeforeDestruction;
    [SerializeField]
    protected Transform model;

    protected HitOnPosition hitOnPosition;

    private void Awake()
    {
        hitOnPosition = GetComponent<HitOnPosition>();
    }

    void Start()
    {
        Invoke(nameof(Kill),flyDistance/speed);
        Messenger.AddListener("EndGame",OnEndGame);
    }

    // Update is called once per frame
    void Update()
    {
        //Перемещаемся в заданном направлении с заданной скоростью
        transform.position += (Vector3)(Direction.ToVector2() * (speed * Time.deltaTime));
        //Если встречаем препятствие то разрушаемся
        if (GameMaster.instance.GetCellType(((Vector2)transform.position).Round()) == CellState.Obstacle)
        {
           Kill();
        }
    }
    /// <summary>
    /// Не уничтожаем объект сразу, чтобы след мог исчезнуть естественным образом
    /// </summary>
    protected void Kill()
    {
        hitOnPosition.enabled = false;
        model.gameObject.SetActive(false);
        Direction = Vector2Int.zero;
        
        Destroy(gameObject,timeBeforeDestruction);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("EndGame",OnEndGame);
    }

    void OnEndGame()
    {
        Destroy(gameObject);
    }
}
