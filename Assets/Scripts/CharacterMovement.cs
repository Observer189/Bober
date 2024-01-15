using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool IsMoving => isMoving;

    [SerializeField]
    protected float moveSpeed;

    protected bool isMoving;

    protected float startMovingTime = float.MinValue;
    //Откуда мы двигаемся в данный момент
    protected Vector2 moveFrom;
    //Куда мы двигаемся в данный момент
    protected Vector2 moveTo;
   
    void Update()
    {
        if (isMoving)
        {
            //Во время движения линейно интерполируем позицию персонажа от начального вектора до целевого
            var t = (Time.time - startMovingTime) * moveSpeed;
            if (t > 1)
            {
                //После того как мы дошли в точку назначения переходим в состояние покоя
                isMoving = false;
            }
            //После движения мы можем оказать немного не в центре клетки, но в этом нет ничего страшного
            //Так как следующее движение мы будем интерполировать от фактической позиции в которой мы оказались
            //Если же ставить персонажа прям в центр клетки, то будет видно едва заметное подрагивание при непрерывном движении
            transform.position = moveFrom + (moveTo - moveFrom) * t;
        }
    }

    public void Move(Vector2Int direction)
    {
        if(direction is { x: 0, y: 0 }) return;
        //Проверяем что целевая клетка свободна
        var targetPos = ((Vector2Int)transform.position.Round()) + direction;
        if (GameMaster.instance.GetCellType(targetPos) != CellState.Obstacle)
        {
            if (!isMoving)
            {
                //При предыдущем движении мы немного зашли за центр клетки и нам надо сделать на это упреждение
                //Лучше было бы конечно прям точно его рассчитать, но если вычесть время одного кадра
                //То разница с точным значение будет незаметна глазу
                startMovingTime = Time.time - Time.deltaTime;
                moveFrom = transform.position;
                moveTo = targetPos;
                //Поворачиваемся в сторону движения
                transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y,Vector2.SignedAngle(Vector2.up,direction));
                isMoving = true;
            }
        }
    }

    protected IEnumerator Movement(Vector2Int startPosition, Vector2Int targetPosition)
    {
        isMoving = true;
        var direction = targetPosition - startPosition;
        transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y,Vector2.SignedAngle(Vector2.up,direction));
        startMovingTime = Time.time;

        while (Time.time - startMovingTime < 1/moveSpeed)
        {
            float t = (Time.time - startMovingTime)*moveSpeed;
            transform.position = (startPosition.ToVector2() + (direction).ToVector2() * t);
            yield return null;
        }

        transform.position = targetPosition.ToVector2();
        isMoving = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
