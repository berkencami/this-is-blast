using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class TextRevealAnimation : MonoBehaviour
{
    
    [SerializeField] private float _startingDelay = 0.9f;
    [SerializeField] private float _letterInterval = 0.04f;
    [SerializeField] private float _singleCharDuration = 0.8f;
  
    [SerializeField] private AnimationCurve _scaleCurve = new(
        new Keyframe(0f, 1.2f, 0f, 8f),
        new Keyframe(0.3f, 2.4f, 0f, 0f),
        new Keyframe(0.6f, 0.8f, 0f, 0f),
        new Keyframe(1f, 1.0f, 0f, 0f)
    );
    
    private TextMeshProUGUI textComponent;
    private DOTweenTMPAnimator tmpAnimator;
    private string fullText;
    private Sequence mainSequence;
    
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }
    
    private void OnEnable()
    {
        fullText = textComponent.text;

        textComponent.alpha = 0f;
        
        PlayTextAnimation();
    }
    
    private void OnDisable()
    {
        StopTextAnimation();
    }
    
    private void PlayTextAnimation()
    {
        textComponent.alpha = 1f;

        textComponent.text = fullText;
        textComponent.ForceMeshUpdate(true);
        
        tmpAnimator = new DOTweenTMPAnimator(textComponent);
        
        if (mainSequence != null && mainSequence.IsActive())
        {
            mainSequence.Kill();
        }
        mainSequence = DOTween.Sequence();
        
        var visibleCharCount = 0;
        for (var i = 0; i < tmpAnimator.textInfo.characterCount; i++)
        {
            if (tmpAnimator.textInfo.characterInfo[i].isVisible)
            {
                visibleCharCount++;
                tmpAnimator.DOScaleChar(i, 0f, 0);
            }
        }
        
        if (visibleCharCount == 0)
        {
            return;
        }
        
        for (var i = 0; i < tmpAnimator.textInfo.characterCount; i++)
        {
            if (!tmpAnimator.textInfo.characterInfo[i].isVisible)
                continue;
                
            CreateCharacterAnimation(i);
        }
    }
    
    private void CreateCharacterAnimation(int charIndex)
    {
        var charSequence = DOTween.Sequence();
        
        charSequence.SetDelay(_letterInterval * charIndex + _startingDelay);
        
        charSequence.AppendCallback(() => {
            float initialScale = _scaleCurve.Evaluate(0f);
            tmpAnimator.DOScaleChar(charIndex, initialScale, 0);
        });
        
        charSequence.Append(DOTween.To(
            () => 0f,                        
            value => {                       
                var scale = _scaleCurve.Evaluate(value);
                tmpAnimator.DOScaleChar(charIndex, scale, 0);
            },
            1f,                         
            _singleCharDuration              
        ).SetEase(Ease.Linear));   
        
        mainSequence.Join(charSequence);
    }

    private void StopTextAnimation()
    {
        if (mainSequence != null && mainSequence.IsActive())
        {
            mainSequence.Kill();
            mainSequence = null;
        }

        if (tmpAnimator?.textInfo == null) return;
      
        var finalScale = _scaleCurve.Evaluate(1f);
            
        for (var i = 0; i < tmpAnimator.textInfo.characterCount; i++)
        {
            if (tmpAnimator.textInfo.characterInfo[i].isVisible)
            {
                tmpAnimator.DOScaleChar(i, finalScale, 0);
            }
        }
    }
    
    private void OnValidate()
    {
        // Ensure curve has at least one keyframe
        if (_scaleCurve.length == 0)
        {
            _scaleCurve = new AnimationCurve(
                new Keyframe(0f, 1.2f),
                new Keyframe(0.3f, 2.4f),
                new Keyframe(0.6f, 0.8f),
                new Keyframe(1f, 1.0f)
            );
        }
    }
}