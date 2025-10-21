using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GhostController : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostShadow; // 귀신 그림자
    public float ghostMoveDistance = 10f; // 이동 거리
    private float ghostMoveTime = 5f; // 이동 시간
    private bool ghostMovingRight = true; // 현재 이동 방향 (true면 →, false면 ←)
    private Rigidbody2D ghostRb; // 귀신 Rigidbody2D

    [Header("Warning Settings")]
    public GameObject warningPanel; // 경고문 패널
    private float minAlpha = 0f;
    private float maxAlpha = 0.7f;
    private float speed = 1.5f; // 깜빡임 속도

    private Coroutine blinkCoroutine;

    private void Start() {
        if (ghostShadow != null) ghostRb = ghostShadow.GetComponent<Rigidbody2D>();
    }

    public void StartGhostRoutine() {
        Debug.Log("귀신 움직인다");
        // Coroutine(코루틴): 시간이 걸리는 동작을 한 프레임에 다 처리하지 않고, 여러 프레임에 걸쳐 나눠서 실행하게 해주는 메소드
        StopCoroutine(nameof(GhostMoveRoutine));
        StartCoroutine(GhostMoveRoutine()); // StartCoroutine(): Coroutine 실행명령
    }

    private IEnumerator GhostMoveRoutine() {
        // 경고문 패널 깜빡임 시작
        StartBlinking();

        Vector2 startPos = ghostRb.position; // 현재 위치를 시작 위치로 지정
        // 목표 위치 계산
        // 현재 방향에 따라 이동 거리만큼 이동
        Vector2 endPos = ghostMovingRight ? 
            startPos + new Vector2(ghostMoveDistance, 0f) : startPos + new Vector2(-ghostMoveDistance, 0f);

        float t = 0f;
        while (t < ghostMoveTime) {

            t += Time.deltaTime; // Time.deltaTime: 한 프레임이 걸린 시간(초).

            // Clamp01(): 값을 0~1 사이로 제한.
            float k = Mathf.Clamp01(t / ghostMoveTime); // 이동 진행률 (0~1) 계산.

            // Vector2.Lerp(a, b, k): 선형 보간.
            // k=0이면 startPos, k=1이면 endPos, 그 사이 값이면 점진적으로 이동
            Vector2 newPos = Vector2.Lerp(startPos, endPos, k); // 부드럽게 start → end로 이동
            // transform.position = newPos 대신 MovePosition을 쓰는 이유 → 물리 충돌 계산과 잘 맞게 하기 위해서
            ghostRb.MovePosition(newPos); // 귀신을 해당 위치로 이동
            
            // 귀신이 움직이는 동안 마우스/키보드 조작 → 게임오버
            if (Input.anyKeyDown) {
                GameManager.Instance.Gameover();
                yield break; // 코루틴 즉시 종료
            }

            yield return null; // 다음 프레임까지 잠깐 멈춤
            // yield return new WaitForSeconds(1f): 1초 동안 기다림
            // yield return StartCoroutine(OtherCoroutine()): 다른 코루틴 다 끝날 때까지 기다림
        }

        ghostRb.MovePosition(endPos); // 귀신을 최종 위치에 둠

        // 방향 전환 (경계 체크)
        if (endPos.x >= 10f || endPos.x <= -10f) {
            ghostMovingRight = !ghostMovingRight;

            // 이동 방향이 바뀌면 귀신 오브젝트 좌우 반전
            Vector3 scale = ghostShadow.transform.localScale;
            scale.x *= -1;
            ghostShadow.transform.localScale = scale;
        }

        // 경고문 패널 깜빡임 종료
        StopBlinking();
    }
    
    // ---------------- Light 깜빡임 ----------------
    private void StartBlinking() {
        warningPanel.SetActive(true); // 경고문 패널 보이기
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkLoop());
    }

    private void StopBlinking() {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
        Image panelImage = warningPanel.GetComponent<Image>();
        // 정지 시 기본값(투명)으로
        Color c = panelImage.color;
        c.a = 0f;
        panelImage.color = c;
        warningPanel.SetActive(false); // 경고문 패널 안보이게 하기
    }

    private IEnumerator BlinkLoop() {
        Image panelImage = warningPanel.GetComponent<Image>();
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * speed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(t, 1f)); 
            Color c = panelImage.color;
            c.a = alpha;
            panelImage.color = c;

            yield return null;
        }
    }
}
