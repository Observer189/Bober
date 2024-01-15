using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlacer : MonoBehaviour
{
   [SerializeField]
   protected GameObject bombPrefab;
   
   public void PlaceBomb()
   {
      var pos = ((Vector2)transform.position).Round();
      if (GameMaster.instance.GetCellType(pos) == CellState.Empty)
      {
         var bomb = Instantiate(bombPrefab, pos.ToVector2(), Quaternion.identity);
         GameMaster.instance.SetBomb(pos);
      }
   }
}
