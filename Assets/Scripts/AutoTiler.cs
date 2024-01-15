using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Вспомогательный класс для настройки тайлинга текстуры у границ карты
/// </summary>
[ExecuteInEditMode]
public class AutoTiler : MonoBehaviour
{
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }
    
    void Update()
    {
        _renderer.material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
}
