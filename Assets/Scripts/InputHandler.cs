using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Tooltip("Время в течении которого ввод сделанный во время движения считается валидным. " +
             "Скажем, игрок во время движения вверх нажал направо и потом отпустил кнопки. Если игрок" +
             "в течение bufferTime доберется до центра верхней клетки, то он сразу начнет движение направо")]
    [SerializeField]
    protected float moveInputBufferTime;
    
    protected CharacterMovement characterMovement;

    protected BombPlacer bombPlacer;

    protected Vector2Int currentMovement;

    protected Vector2Int? bufferMovement;

    protected bool fire;

    protected bool fireBuffer;

    protected float lastMoveBufferTime;
    // Start is called before the first frame update
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        bombPlacer = GetComponent<BombPlacer>();
    }

    private void Update()
    {
        //Проверяем является валидным наш буферный ввод и если нет, то очищаем
        if (Time.time - lastMoveBufferTime > moveInputBufferTime)
        {
            bufferMovement = null;
        }
        //Двигать персонажа можно только когда он стоит
        if (!characterMovement.IsMoving)
        {
            //Если в момент остановки у нас есть неочищенный буферный ввод и при этом нет текущего ввода, то применяем его
            if (bufferMovement != null && currentMovement == Vector2Int.zero)
            {
                characterMovement.Move(bufferMovement.Value);
                bufferMovement = null;
            }
            //В противном случае применяем текущий ввод
            else
            {
                characterMovement.Move(currentMovement);
            }
            //Если мы нажимаем огонь или нажали его во время движения к текущей клетке то ставим бомбу
            if (fireBuffer || fire)
            {
                bombPlacer.PlaceBomb();
                fireBuffer = false;
            }
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            var val = context.ReadValue<Vector2>().Round();
            //Записываем ввод только если он не диагональный
            if (val.x == 0 || val.y == 0)
            {
                currentMovement = val;
                if (characterMovement.IsMoving && val != Vector2Int.zero)
                {
                  
                    bufferMovement = val;
                    lastMoveBufferTime = Time.time;
                }
            }
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            fire = true;
            fireBuffer = true;
        }
        else if(context.canceled)
        {
            fire = false;
        }
                  
    }
}
