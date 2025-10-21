using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverEffect : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image screenFlash; // ��üȭ�� Image
    [SerializeField] private Image ghostShadow; // �׸��� ������Ʈ
    [SerializeField] private Image ghostImage; // ���� �ͽ� (AudioSource ����)

    [Header("Settings")]
    [SerializeField] private float flashSpeed = 7f; // ȭ�� ������ �ӵ�
    [SerializeField] private Color flashColor = new Color(15/255f, 0, 0, 0.8f);
    [SerializeField] private float shadowFadeDuration = 1.5f; // �׸��� �� ���� �ð�
    [SerializeField] private int shadowBlinkCount = 5; // �׸��� ������ Ƚ��
    [SerializeField] private float ghostFadeDuration = 1.5f; // �ͽ� ��ü �巯���� �ð�
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private Coroutine flashRoutine;

    /// <summary>
    /// GameManager���� ���ӿ��� �߻� �� ȣ��
    /// </summary>
    public void Play()
    {
        // �ڽ� ������Ʈ Ȱ��ȭ
        if (screenFlash) screenFlash.gameObject.SetActive(true);
        if (ghostShadow) ghostShadow.gameObject.SetActive(true);

        // ���� ����
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // 1. ȭ�� ������ ����
        flashRoutine = StartCoroutine(ScreenFlashLoop());

        // 1. �׸��� �����̸鼭 ���� Ŀ��
        if (ghostShadow)
        {
            float interval = shadowFadeDuration / shadowBlinkCount;
            float alphaStep = 1f / shadowBlinkCount; // �����Ӹ��� ���ݾ� £����

            for (int i = 0; i < shadowBlinkCount; i++)
            {
                // OFF ���� (����)
                SetAlpha(ghostShadow, 0f);
                yield return new WaitForSeconds(interval * 0.5f);

                // ON ���� (���� �� £����)
                float newAlpha = (i + 1) * alphaStep;
                SetAlpha(ghostShadow, newAlpha);
                yield return new WaitForSeconds(interval * 0.5f);
            }
        }

        // ������ ���߱�
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        // ȭ�� ���� ��Ӱ� ����
        if (screenFlash)
        {
            Color finalColor = flashColor;
            finalColor.a = 1f;
            screenFlash.color = finalColor;
        }

        // 2. �ͽ� �巯�� + ȿ����
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

        // 3. �� ��ȯ (�ణ �� �ΰ�)
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
