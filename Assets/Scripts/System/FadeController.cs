using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private static readonly int FadeThresholdID = Shader.PropertyToID("_FadeThreshold");

    [SerializeField] private Material _fadeMaterial;
    [SerializeField] private float _fadeTime = 1f;

    public float GetFadeTime => _fadeTime;

    private void OnDisable()
    {
        _fadeMaterial.SetFloat(FadeThresholdID, 1);
    }

    public async UniTaskVoid FadeIn(float fadeTime)
    {
        // 初期化
        _fadeMaterial.SetFloat(FadeThresholdID, 0f);

        // 1秒かけて中心から端に向かって明るくする
        await DOTween.To(() => _fadeMaterial.GetFloat(FadeThresholdID),
            x => _fadeMaterial.SetFloat(FadeThresholdID, x),
            1f,
            _fadeTime).AsyncWaitForCompletion();
    }

    public async UniTaskVoid FadeOut(float fadeTime)
    {
        // 初期化
        _fadeMaterial.SetFloat(FadeThresholdID, 1f);

        // 1秒かけて端から中心に向かって暗くする
        await DOTween.To(() => _fadeMaterial.GetFloat(FadeThresholdID),
            x => _fadeMaterial.SetFloat(FadeThresholdID, x),
            0f,
            _fadeTime).AsyncWaitForCompletion();
    }
}
