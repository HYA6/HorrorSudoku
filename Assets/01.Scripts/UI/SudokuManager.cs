using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// 전체적인 스도쿠 로직 스크립트
public class SudokuManager : MonoBehaviour
{
    public static SudokuManager Instance; // 싱글톤

    [Header("Tile References")]
    public GameObject selectedTile; // 현재 선택된 타일
    public List<GameObject> allTiles; // 모든 타일

    [Header("Colors")]
    private readonly Color defaultColor = new(191/255f,177/255f,177/255f);
    private readonly Color highlightColor = new(207/255f,203/255f,131/255f);
    private readonly Color inputColor = new(169/255f,55/255f,41/255f);

    // 싱글톤 초기화
    private void Awake() {
        Instance = this;
    }

    // "_" 기준으로 타일 이름 분리
    public string[] SplitTile(string editTile) => editTile.Split("_");

    // 원래 문제인지 확인
    public bool CheckedTiles(int row, int col) {
        return SudokuQuestion.Instance.GetBoolTile()[row, col]; // 문제라면 true 반환, 아니라면 false 반환
    }

    // 선택된 타일 갱신 & 조건에 맞게 색상 변경
    public void SetSelectedTile(GameObject tile) {
        selectedTile = tile;
        SetNormalColor();
        HighlightRowCol();
    }

    // 타일 배경을 기본 타일로 초기화
    public void SetNormalColor() {
        allTiles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
        foreach (GameObject tile in allTiles) {
            // var: 암시적 형식(local variable type inference)
            // 변수 타입을 명시하지 않고도 컴파일러가 대입되는 값으로 타입을 추론하게 해주는 문법
            var img = tile.GetComponent<Image>();
            if (img != null && img.color != defaultColor) img.color = defaultColor;
        }
    }

    // 같은 행 또는 열에 있는 타일을 하이라이트 타일로 변경
    public void HighlightRowCol() {
        string[] selectedName = SplitTile(selectedTile.name);
        allTiles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
        foreach (GameObject tile in allTiles) {
            string[] tileName = SplitTile(tile.name);
            if (tileName[1] == selectedName[1] || tileName[2] == selectedName[2]) {
                var img = tile.GetComponent<Image>();
                if (img != null) img.color = highlightColor;
            }
        }
    }

    // 숫자 입력 처리
    public void OnNumberButtonClicked(string number) {
        if (selectedTile == null) return;
        var text = selectedTile.GetComponentInChildren<TMP_Text>(); // 선택된 타일의 TMP
        string[] selectedName = SplitTile(selectedTile.name); // 선택된 타일 이름 분리
        // 선택된 타일이 문제라면 숫자 입력/변경 & 색처리
        if (CheckedTiles(int.Parse(selectedName[1]), int.Parse(selectedName[2]))) {
            text.text = number;
            text.color = inputColor;
        }
    }

    // 선택한 타일 숫자 삭제
    public void DeleteTile() {
        if (selectedTile == null) return;
        var text = selectedTile.GetComponentInChildren<TMP_Text>(); // 선택된 타일의 TMP
        string[] selectedName = SplitTile(selectedTile.name); // 선택된 타일 이름 분리
        // 선택된 타일의 숫자가 있고 문제라면 숫자 삭제
        if (!string.IsNullOrEmpty(text.text) && 
        CheckedTiles(int.Parse(selectedName[1]), int.Parse(selectedName[2]))) text.text = "";
    }

    // 초기화
    public void ResetTiles() {
        allTiles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
        // 모든 타일을 순회
        foreach(GameObject tile in allTiles) {
            var text = tile.GetComponentInChildren<TMP_Text>(); // 타일의 TMP
            string[] tileName = SplitTile(tile.name); // 타일 이름 분리
            // 타일의 숫자가 있고 문제라면 빈 칸 처리
            if (!string.IsNullOrEmpty(text.text) && 
            CheckedTiles(int.Parse(tileName[1]), int.Parse(tileName[2]))) text.text = "";
        }
    }

    // 정답 확인
    public void CheckedAnswer(int[, ] userAnswer) {
        // 정답
        int[, ] answer = SudokuQuestion.Instance.GetAnswer();
        // 스도쿠 사이즈
        int gridSize = SudokuQuestion.Instance.GetGridSize();

        // Life 오브젝트 배열
        GameObject[] lifeOb = GameObject.FindGameObjectsWithTag("LifeCount");
        // LifeCount 개수
        int lifeCount = lifeOb.Length;
        // 정답인지 아닌지
        bool checkAnswer = true; // true면 정답, false면 오답

        // 유저 답과 정답 비교
        for(int i=0; i<gridSize; i++) {
            for (int j=0; j<gridSize; j++) {
                // 둘이 같지 않으면 실패처리
                if (userAnswer[i, j] != answer[i, j]) {
                    lifeOb[--lifeCount].SetActive(false);
                    checkAnswer = false;
                    break;
                }
            }
            if (checkAnswer == false) break;
        }
        if(lifeCount <= 0) { // 남은 생명이 없으면 게임 오버
            GameManager.Instance.Gameover();
        }
        Debug.Log("남은 기회: " + lifeCount);
        // 둘이 모두 같으면 성공처리
        return;
    }
}
