using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuOpen : MonoBehaviour
{
    public GameObject sudokuUI;

    void Update()
    {
		//마우스 클릭시
        if (Input.GetMouseButtonDown(0))
        {
        	//마우스 클릭한 좌표값 가져오기
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //해당 좌표에 있는 오브젝트 찾기
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (hit.collider != null)
            {
                GameObject click_obj = hit.transform.gameObject;
                sudokuUI.SetActive(true); // 스도쿠 UI 오픈
            }
        }
    }
}
