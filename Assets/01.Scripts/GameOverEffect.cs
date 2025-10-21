using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverEffect : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image screenFlash; // 전체화면 Image
    [SerializeField] private Image ghostShadow; // 그림자 오브젝트
    [SerializeField] private Image ghostImage; // 실제 귀신 (AudioSource 포함)

    [Header("Settings")]
    [SerializeField] private float flashSpeed = 7f; // 화면 깜빡임 속도
    [SerializeField] private Color flashColor = new Color(15/255f, 0, 0, 0.8f);
    [SerializeField] private float shadowFadeDuration = 1.5f; // 그림자 총 연출 시간
    [SerializeField] private int shadowBlinkCount = 5; // 그림자 깜빡임 횟수
    [SerializeField] private float ghostFadeDuration = 1.5f; // 귀신 본체 드러나는 시간
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private Coroutine flashRoutine;

    /// <summary>
    /// GameManager에서 게임오버 발생 시 호출
    /// </summary>
    public void Play()
    {
        // 자식 오브젝트 활성화
        if (screenFlash) screenFlash.gameObject.SetActive(true);
        if (ghostShadow) ghostShadow.gameObject.SetActive(true);

        // 연출 시작
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // 1. 화면 깜빡임 시작
        flashRoutine = StartCoroutine(ScreenFlashLoop());

        // 1. 그림자 깜빡이면서 점점 커짐
        if (ghostShadow)
        {
            float interval = shadowFadeDuration / shadowBlinkCount;
            float alphaStep = 1f / shadowBlinkCount; // 깜빡임마다 조금씩 짙어짐

            for (int i = 0; i < shadowBlinkCount; i++)
            {
                // OFF 상태 (투명)
                SetAlpha(ghostShadow, 0f);
                yield return new WaitForSeconds(interval * 0.5f);

                // ON 상태 (조금 더 짙어짐)
                float newAlpha = (i + 1) * alphaStep;
                SetAlpha(ghostShadow, newAlpha);
                yield return new WaitForSeconds(interval * 0.5f);
            }
        }

        // 깜빡임 멈추기
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        // 화면 완전 어둡게 고정
        if (screenFlash)
        {
            Color finalColor = flashColor;
            finalColor.a = 1f;
            screenFlash.color = finalColor;
        }

        // 2. 귀신 드러남 + 효과음
        if (ghostImage)
        {
            ghostImage.gameObject.SetActive(true);

            Color c = ghostImage.color;
            c.a = 0f;
            ghostImage.color = c;

            float t = 0f;
            while (t < ghostFadeDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / ghostFadeDuration);
                c.a = k;
                ghostImage.color = c;
                yield return null;
            }
        }

        // 3. 씬 전환 (약간 텀 두고)
        yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene(gameOverSceneName);
    }

    private void SetAlpha(Image img, float alpha)
    {
        if (img)
        {
            Color c = img.color;
            c.a = Mathf.Clamp01(alpha);
            img.color = c;
        }
    }

    private IEnumerator ScreenFlashLoop()
    {
        while (true)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            if (screenFlash)
            {
                Color c = flashColor;
                c.a = alpha * flashColor.a;
                screenFlash.color = c;
            }
            yield return null;
        }
    }
}
