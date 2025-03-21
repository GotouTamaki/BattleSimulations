using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    [SerializeField] private Material _fadeMaterial;
    [SerializeField] private float _fadeTime = 1f;

    public float GetFadeTime => _fadeTime;

    private void OnDisable()
    {
        _fadeMaterial.SetFloat(DissolveAmountID, 1);
    }

    public async UniTask FadeIn(float fadeTime)
    {
        // 初期化
        _fadeMaterial.SetFloat(DissolveAmountID, 0f);

        // 1秒かけて中心から端に向かって明るくする
        await DOTween.To(() => _fadeMaterial.GetFloat(DissolveAmountID),
            x => _fadeMaterial.SetFloat(DissolveAmountID, x),
            1f,
            _fadeTime).AsyncWaitForCompletion();
    }

    public async UniTask FadeOut(float fadeTime)
    {
        // 初期化
        _fadeMaterial.SetFloat(DissolveAmountID, 1f);

        // 1秒かけて端から中心に向かって暗くする
        await DOTween.To(() => _fadeMaterial.GetFloat(DissolveAmountID),
            x => _fadeMaterial.SetFloat(DissolveAmountID, x),
            0f,
            _fadeTime).AsyncWaitForCompletion();
    }
}
