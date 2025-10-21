using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 버튼 처리
public class AllButton : MonoBehaviour
{
    public AudioSource gameSound; // 게임 사운드 오브젝트
    public Toggle soundToggle; // 게임 사운드 토글 오브젝트
    public Image soundTargetImage; // 게임 사운드 버튼 이미지
    public Sprite soundOn; // 사운드 on 스프라이트(이미지)
    public Sprite soundOff; // 사운드 off 스프라이트(이미지)
    public GameObject sudokuUI; // 스도쿠 UI 오브젝트
    public GameObject howToSudokuPanel; // 게임 설명 패널 오브젝트

    // 게임 닫기
    public void GameClose() {
        Application.Quit(); // 게임 종료
    }

    // 게임 사운드 On/Off 토글
    public void SoundToggle() {
        if (soundToggle.isOn) {
            soundTargetImage.sprite = soundOn;
            gameSound.volume = 1f;
        }
        else {
            soundTargetImage.sprite = soundOff;
            gameSound.volume = 0f;
        }
    }

    // 스도쿠 창 닫기
    public void SudokuClose() {
        sudokuUI.SetActive(false);
    }

    // 게임 방법 설명
    public void HowToSudoku() {
        if (howToSudokuPanel.activeSelf) {
            howToSudokuPanel.SetActive(false);
        } else {
            howToSudokuPanel.SetActive(true);
        }
    }

    // 삭제 처리
    public void Delete() {
        SudokuManager.Instance.DeleteTile();
    }
    
    // 초기화 처리
    public void Reset() {
        SudokuManager.Instance.ResetTiles();
    }

    // 정답 확인
    public void AnswerCheck() {

        int tileGroupCount = GameObject.FindGameObjectsWithTag("TileGroup").Length;
        int[,] userAnswer = new int[tileGroupCount, tileGroupCount]; // 유저 정답을 저장할 배열

        // 유저 정답 변수에 저장
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles) {
            string[] tileName = tile.name.Split("_");
            string text = tile.GetComponentInChildren<TMP_Text>().text;
            int num = 0;
            if (text != null && text != ""){
                num = int.Parse(text);
            }
            userAnswer[int.Parse(tileName[1]), int.Parse(tileName[2])] = num;
        }

        // 정답 확인
        SudokuManager.Instance.CheckedAnswer(userAnswer);

    }
}
