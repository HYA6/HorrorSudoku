using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance; // 싱글톤

    [Header("Time Attack")]
    public Image timeBar; // 타임어택 이미지 (줄어드는 바)
    private float maxTime = 600f; // 타임어택 시간 (10분)
    private float timeLeft; // 스크립트 내에서의 채우기 양

    [Header("Ghost")]
    public GhostController ghostController;  // GhostController 객체
    public GameObject ghostShadow; // 귀신 그림자
    public float ghostInterval = 90f; // 등장 주기(단위:초) -> 1: 90s, 2: 75s, 3: 60s, 4: 45)
    private float nextGhostTime; // 귀신 다음 등장 시간
    public GameObject warningPanel; // 경고문 오브젝트

    [Header("SudokuUI")]
    public GameObject sudokuUI; // 스도쿠 UI

    [Header("Game Over")]
    public GameOverEffect gameOverEffect; // 게임오버 효과

    private bool isGameOver = false; // 게임오버 여부 (true: 끝, false: 끝아님)

    private void Start() {
        Instance = this;
        timeLeft = maxTime; // 채우기 양을 타임어택 최대 시간으로 설정
        nextGhostTime = timeLeft - ghostInterval; // 귀신 처음 등장 예약
    }

    private void Update() {
        if (isGameOver) return;

        // 타임바 갱신
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            timeBar.fillAmount = timeLeft / maxTime;
        } else {
            Gameover();
        }

        // 귀신 이동 체크
        if (timeLeft <= nextGhostTime) {
            GhostMove();
            nextGhostTime -= ghostInterval; // 다음 등장 시간 예약
        }
    }

    // 일정 시간마다 귀신 움직임
    private void GhostMove() {
        if (ghostShadow != null) ghostController.StartGhostRoutine(); 
    }

    // 게임 오버
    public void Gameover() {
        if (isGameOver) return; // 중복 호출 방지
        isGameOver = true;
        warningPanel.SetActive(false);

        if (gameOverEffect != null)
            gameOverEffect.Play();
    }

}
