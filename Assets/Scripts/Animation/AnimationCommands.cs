using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCommands : MonoBehaviour
{
    [SerializeField] Color _targetColor = Color.white;
    [SerializeField] Color _nonTargetColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private AnimationCurve _fasterCharacterMatchMoveCurve;
    [SerializeField] private AnimationCurve _slowerCharacterMatchMoveCurve;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private ParticleSystem _deadEffect;
    [SerializeField] private Vector3 _deadEffectOffset;

    public void SetTargetColor(Dictionary<int, GameObject> _characterObjects, GameObject target)
    {
        foreach (var characterObject in _characterObjects)
        {
            if (characterObject.Value == target)
            {
                SpriteRenderer spriteRenderer = characterObject.Value.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.color = _targetColor;
            }
            else
            {
                SpriteRenderer spriteRenderer = characterObject.Value.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.color = _nonTargetColor;
            }
        }
    }

    public void ResetTargetColor(Dictionary<int, GameObject> _characterObjects)
    {
        foreach (var characterObject in _characterObjects)
        {
            SpriteRenderer spriteRenderer = characterObject.Value.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.color = _targetColor;
        }
    }

    public async UniTaskVoid CharacterMove(Transform fastCharacterTransform, Transform defender)
    {
        await fastCharacterTransform.DOMove(defender.position, 1f).AsyncWaitForCompletion();
    }

    public async UniTaskVoid FasterCharacterMove(Transform fastCharacterTransform, Transform slowerCharacterTransform)
    {
        await fastCharacterTransform.DOMove(slowerCharacterTransform.position, _moveDuration).SetEase(_fasterCharacterMatchMoveCurve).AsyncWaitForCompletion();
    }

    public async UniTaskVoid SlowerCharacterMove(Transform slowerCharacterTransform, Transform fastCharacterTransform)
    {
        await slowerCharacterTransform.DOMove(fastCharacterTransform.position, _moveDuration).SetEase(_slowerCharacterMatchMoveCurve).AsyncWaitForCompletion();
    }

    public async UniTaskVoid DeadEffects(GameObject character)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_moveDuration));
        ParticleSystem deadEffect = Instantiate(_deadEffect, character.transform.position + _deadEffectOffset, _deadEffect.transform.rotation);
        await UniTask.Delay(TimeSpan.FromSeconds(deadEffect.duration * 1.1f));
        character.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }
}
