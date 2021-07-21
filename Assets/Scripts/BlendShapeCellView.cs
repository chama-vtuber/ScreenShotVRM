using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class BlendShapeCellView : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Slider _slider;

    private Subject<(BlendShapeKey key, float value)> _subject = new Subject<(BlendShapeKey key, float value)>();

    public IObservable<(BlendShapeKey key, float value)> OnValueChangedAsObservable => _subject.TakeUntilDestroy(this);

    public void Initialize(BlendShapeKey key, float value)
    {
        _name.text = key.Name;
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.value = value;
        _slider.OnValueChangedAsObservable().Subscribe(x =>
        {
            _subject.OnNext((key, _slider.value));
        }).AddTo(this);
    }
    
}