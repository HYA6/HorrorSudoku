using System;
using UnityEngine;
using UnityEngine.UI;

// Tile 클릭 이벤트 스크립트
public class TileClick : MonoBehaviour
{
    public void OnClick()
    {
        SudokuManager.Instance.SetSelectedTile(transform.gameObject);
    }
}
