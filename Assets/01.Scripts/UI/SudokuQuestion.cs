using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
/*
랜덤 9X9 스도쿠 구현
조건
    1) 3X3 타일 내에서 중복 X
    2) 각 행 내에서 중복 X
    3) 각 열 내에서 중복 X
=> 정답 구현 후 빈 칸 개수에 맞게 비우기 (정답은 다른 배열로 저장하여 나중에 정답 확인 시 사용)
*/

// 스도쿠 구현 클래스
public class SudokuQuestion : MonoBehaviour
{
    public int gridSize = 9; // 전체 퍼즐 크기 (9X9) -> 6X6이라면 6으로 변경
    public int boxRows = 3; // 각 박스(TileGroup) 크기(가로)
    public int boxCols = 3; // 각 박스(TileGroup) 크기(세로)
    public int blank = 20; // 빈 칸 개수
    public int[,] answer; // 정답

    private int[,] puzzle; // 2차원 배열로 퍼즐 숫자 저장

    public bool[,] boolTile; // 해당 타일이 원래 빈 칸(문제)라면 true, 문제가 아니라면 false

    public static SudokuQuestion Instance; // 싱글톤

    private void Awake() {
        Instance = this; // 싱글톤 초기화
    }

    // 유니티 시작 시 실행
    void Start()
    {
        answer = new int[gridSize, gridSize];
        boolTile = new bool[gridSize, gridSize];
        FillBoard(answer, 0, 0);           // 정답 생성
        puzzle = (int[,])answer.Clone();   // 퍼즐용 복사본 만들기
        RemoveCells(puzzle, blank);          // 빈 칸 지우기
        ApplyPuzzleToTiles(transform);       // UI에 표시
    }

    // 정답 스도쿠 생성 (백트래킹 알고리즘)
    bool FillBoard(int[,] board, int row, int col)
    {  
        // 보드 끝까지 도달했으면 완료
        if (row == gridSize)
            return true;

        // 다음 좌표 계산
        int nextRow = (col == gridSize - 1) ? row + 1 : row; // 마지막 열이면 다음 행 선택
        int nextCol = (col == gridSize - 1) ? 0 : col + 1; // 마지막 열이 아니라면 다음 열 선택

        /*
        Enumerable 메소드
            - static class로 System.Linq namespace에 속해져있음
            - LINQ(Language Integrated Query)를 이용해서 데이터를 검색,정렬,변환등 작업을 수행
            - IEnumberable 형식의 컬렉션에 대해 사용되며 이를 통해 컬렉션의 요소를 쿼리하고 조작
        */
        // 1 ~ gridSize 까지 랜덤 리스트 생성
        List<int> numbers = Enumerable 
                                .Range(1, gridSize) // 범위 지정
                                .OrderBy(x => UnityEngine.Random.value) // 오름차순
                                .ToList(); // List로 변환

        // 숫자 넣기
        foreach (int num in numbers)
        {
            if (IsValid(board, row, col, num)) // 숫자가 유효한지 확인
            {
                board[row, col] = num;

                // 다음 타일로 재귀
                if (FillBoard(board, nextRow, nextCol))
                    return true;

                // 실패했으면 다시 비움 (백트래킹)
                board[row, col] = 0;
            }
        }

        // 모든 수가 실패했다면 이 경로는 실패
        return false;
    }

    // 스도쿠에 숫자를 넣을 수 있는지 판단
    bool IsValid(int[,] board, int row, int col, int num)
    {
        // 스도쿠 사이즈에 따라 행, 열 바꾸기
        switch(gridSize){
            case 6:
                boxRows = 2;
                break;
            case 12:
                boxCols = 4;
                break;
        }

        // 스도쿠 조건에 맞는지 검사
        // 가로줄 검사(조건2)
        for (int i = 0; i < gridSize; i++)
            if (board[row, i] == num) return false;
        // 세로줄 검사(조건3)
        for (int i = 0; i < gridSize; i++)
            if (board[i, col] == num) return false;
        // 박스 검사(조건1)
        int startRow = row / boxRows * boxRows;
        int startCol = col / boxCols * boxCols;
        for (int r = startRow; r < startRow + boxRows; r++)
        {
            for (int c = startCol; c < startCol + boxCols; c++)
            {
                if (board[r, c] == num) return false;
            }
        }

        return true;
    }

    // 빈 칸 지우기 (빈 칸으로 만들 곳 숫자 0으로 만들기)
    // 이후 개수, 레벨에 따라 빈 칸 수 지정하기
    void RemoveCells(int[,] board, int blank)
    {
        int removeCount = 0;
        while (removeCount < blank) // 빈 칸 개수만큼 0으로 전환
        {
            int r = Random.Range(0, gridSize);
            int c = Random.Range(0, gridSize);

            if (board[r, c] != 0) 
            {
                board[r, c] = 0;
                removeCount++;
            }
        }
    }

    // 퍼즐 숫자를 UI 텍스트에 표시
    void ApplyPuzzleToTiles(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // 이름이 Tile_행_열인 오브젝트이라면
            if (child.name.StartsWith("Tile_"))
            { 
                string[] parts = child.name.Split('_');
                if (parts.Length == 3 &&
                    // TryParse: 숫자로 변환하는 메소드, 반환값 bool(성공: true, 실패: false) 
                    // 속성 - out int '변수': 변환된 값을 변수에 저장
                    int.TryParse(parts[1], out int row) && 
                    int.TryParse(parts[2], out int col))
                {
                    TMP_Text text = child.GetComponentInChildren<TMP_Text>(); 
                    if (text != null)
                        // 내부 TMP_Text 텍스트를 퍼즐 숫자로 변경
                        if (puzzle[row, col] == 0){ // 문제라면
                            text.text = "";
                            boolTile[row, col] = true;
                        }
                        else { // 문제가 아니라면
                            text.text = puzzle[row, col].ToString();
                            boolTile[row, col] = false;
                        }
                }
            }
            else
            {
                // 중간에 그룹 오브젝트 있으면 재귀로 내부까지 탐색
                ApplyPuzzleToTiles(child);
            }
        }
    }

    // gridSize 반환
    public int GetGridSize() {
        return gridSize;
    }

    // boolTile 반환
    public bool[, ] GetBoolTile() {
        return boolTile;
    }

    // solution 반환
    public int[, ] GetAnswer() {
        return answer;
    }
}
