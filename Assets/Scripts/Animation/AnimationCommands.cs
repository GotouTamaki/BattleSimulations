using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AnimationCommands : MonoBehaviour
{
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int MainColorID = Shader.PropertyToID("_MainColor");

    [SerializeField] Color _targetColor = Color.white;
    [SerializeField] Color _nonTargetColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private AnimationCurve _fasterCharacterMatchMoveCurve;
    [SerializeField] private AnimationCurve _slowerCharacterMatchMoveCurve;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _deadAnimationTime = 1f;
    [SerializeField] private ParticleSystem _deadEffect;
    [SerializeField] private Vector3 _deadEffectOffset;

    private Sequence _sequence;

    public void SetTargetColor(Dictionary<int, GameObject> _characterObjects, GameObject target)
    {
        foreach (var characterObject in _characterObjects)
        {
            if (characterObject.Value == target)
            {
                Material _characterMaterial = characterObject.Value.GetComponentInChildren<SpriteRenderer>().material;
                _characterMaterial.SetColor(MainColorID, _targetColor);
            }
            else
            {
                Material _characterMaterial = characterObject.Value.GetComponentInChildren<SpriteRenderer>().material;
                _characterMaterial.SetColor(MainColorID, _nonTargetColor);
            }
        }
    }

    public void ResetTargetColor(Dictionary<int, GameObject> _characterObjects)
    {
        foreach (var characterObject in _characterObjects)
        {
            Material _characterMaterial = characterObject.Value.GetComponentInChildren<SpriteRenderer>().material;
            _characterMaterial.SetColor(MainColorID, _targetColor);
        }
    }

    public async UniTask CharacterMove(Transform fastCharacterTransform, Transform defender)
    {
        await fastCharacterTransform.DOMove(defender.position, 1f).AsyncWaitForCompletion();
    }

    public async UniTask FasterCharacterMove(Transform fastCharacterTransform, Transform slowerCharacterTransform)
    {
        await fastCharacterTransform.DOMove(slowerCharacterTransform.position, _moveDuration).SetEase(_fasterCharacterMatchMoveCurve).AsyncWaitForCompletion();
    }

    public async UniTask SlowerCharacterMove(Transform slowerCharacterTransform, Transform fastCharacterTransform)
    {
        await slowerCharacterTransform.DOMove(fastCharacterTransform.position, _moveDuration).SetEase(_slowerCharacterMatchMoveCurve).AsyncWaitForCompletion();
    }

    public async UniTask DeadEffects(GameObject character)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_moveDuration));

        Material _characterMaterial = character.GetComponentInChildren<SpriteRenderer>().material;
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(DOTween.To(() => _characterMaterial.GetFloat(DissolveAmountID),
            a => _characterMaterial.SetFloat(DissolveAmountID, a), 1f, _deadAnimationTime));
        _sequence.SetUpdate(true);

        ParticleSystem deadEffect = Instantiate(_deadEffect, character.transform.position + _deadEffectOffset, _deadEffect.transform.rotation);
        //await UniTask.Delay(TimeSpan.FromSeconds(deadEffect.duration * 1.1f));
        await _sequence.Play();

        character.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }
}
