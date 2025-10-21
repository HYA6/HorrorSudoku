using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Blinking  : MonoBehaviour

{
    public Light2D theLight; // 사용할 조명
    public float flickerSpeed = 1.2f; // 깜빡이는 속도
    public float minIntensity = 0f; // 최소 강도
    public float maxIntensity = 1f; // 최대 강도

    void Update()
    {
        // 시간에 따라 0에서 1 사이의 값을 반복적으로 생성하고, 이를 최소/최대 강도에 적용
        float timeBasedValue = Mathf.PingPong(Time.time * flickerSpeed, 1f);
        theLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, timeBasedValue);
    }
}