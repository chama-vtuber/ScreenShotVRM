using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VRM;

public class ChangeBlendShapesView : MonoBehaviour
{
    [SerializeField] private Transform _contentRoot;

    private List<KeyValuePair<BlendShapeKey, float>> _list = new List<KeyValuePair<BlendShapeKey, float>>();

    public void Initialize(VRMBlendShapeProxy vrmBlendShapeProxy)
    {
        var pairs = vrmBlendShapeProxy.GetValues();
        _list = pairs.ToList();
        var cell = Resources.Load<BlendShapeCellView>("Prefabs/BlendShapeCellView");
        foreach (var pair in _list)
        {
            var instance = (BlendShapeCellView)Instantiate(cell, _contentRoot, false);
            instance.Initialize(pair.Key, pair.Value);
            instance.OnValueChangedAsObservable.Subscribe(nameAndValue =>
            {
                var target = _list.FirstOrDefault(x => x.Key.Name == nameAndValue.key.Name);
                _list.Remove(target);
                target = new KeyValuePair<BlendShapeKey, float>(nameAndValue.key, nameAndValue.value);
                _list.Add(target);
                vrmBlendShapeProxy.SetValues(_list);
            });
        }
    }
}