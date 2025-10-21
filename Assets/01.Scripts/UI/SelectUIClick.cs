using System;
using UnityEngine;

// 스도쿠 숫자 입력 버튼 스크립트
public class SelectUIClick : MonoBehaviour
{
    public void OnClickSelectUI() {
        // 선택된 버튼의 숫자
        String selectedNum = transform.name.Split("_")[1];
        // 선택된 버튼의 숫자 입력하기
        SudokuManager.Instance.OnNumberButtonClicked(selectedNum);
    }
}
