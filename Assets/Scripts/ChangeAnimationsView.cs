using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAnimationsView : MonoBehaviour
{
    private readonly string _defaultAnimationPath = "Necocoya/ポーズ詰め合わせ_無料版/Pose010_女性_立ち/";

    [SerializeField] private Dropdown _dropdown;
    private AnimatorController _animatorController;

    public void Initialize(AnimatorController animatorController)
    {
        _animatorController = animatorController;
        SetBinds();
        AddAnimationDropDownOptions();
    }

    private void SetBinds()
    {
        _dropdown.onValueChanged.AddListener(ChangeAnimationState);
    }
    
    private void OnDestroy()
    {
        _dropdown.onValueChanged.RemoveListener(ChangeAnimationState);
    }
    
    private void AddAnimationDropDownOptions()
    {
        var optionData = GetAnimationClips();
        _dropdown.AddOptions(optionData);
    }
    private List<Dropdown.OptionData> GetAnimationClips()
    {
        return Resources.LoadAll<AnimationClip>(_defaultAnimationPath).Select(x => new Dropdown.OptionData(x.name))
            .ToList();
    }
    
    private void ChangeAnimationState(int index)
    {
        var optionData = _dropdown.options[index];
        var animationClip =
            Resources.Load<AnimationClip>(_defaultAnimationPath + optionData.text);
        var animatorStateMachine = _animatorController.layers.First().stateMachine;
        if (animatorStateMachine.states.Any(x => x.state.name == animationClip.name))
        {
            return;
        }
        var state = animatorStateMachine.AddState(animationClip.name);
        state.motion = animationClip;
        animatorStateMachine.defaultState = state;
        AssetDatabase.Refresh(); // 変更を反映する
    }
}