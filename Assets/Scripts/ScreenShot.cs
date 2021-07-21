using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEditor.Animations;

public class ScreenShot : MonoBehaviour
{
    private readonly string _defaultAnimationPath = "Necocoya/ポーズ詰め合わせ_無料版/Pose010_女性_立ち/";

    [SerializeField] private Camera _captureCamera;
    [SerializeField] private Button _captureButton;
    [SerializeField] private AnimatorController _animatorController;
    [SerializeField] private Dropdown _dropdown;


    private void Start()
    {
        SetBinds();
        AddAnimationDropDownOptions();
    }

    private void OnDestroy()
    {
        _dropdown.onValueChanged.RemoveListener(ChangeAnimationState);
    }


    private void SetBinds()
    {
        _captureButton.OnClickAsObservable().Subscribe(async unit =>
        {
            CaptureScreenShot(GetPath("ScreenCaptures"), _captureCamera);
        }).AddTo(this);
        _dropdown.onValueChanged.AddListener(ChangeAnimationState);
    }


    private void ChangeAnimationState(int index)
    {
        var optionData = _dropdown.options[index];
        var animationClip =
            Resources.Load<AnimationClip>(_defaultAnimationPath + optionData.text);
        var animatorStateMachine = _animatorController.layers.First().stateMachine;
        var state = animatorStateMachine.AddState(animationClip.name);
        state.motion = animationClip;
        animatorStateMachine.defaultState = state;
        AssetDatabase.Refresh(); // 変更を反映する
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

    private string GetPath(string folderName)
    {
        var environmentalPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        if (!Directory.Exists($"{environmentalPath}/{folderName}"));
        {
            Directory.CreateDirectory($"{environmentalPath}/{folderName}");
        }
        return $"{environmentalPath}/{folderName}/{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png";
    }

    // カメラのスクリーンショットを保存する
    private void CaptureScreenShot(string filePath, Camera camera)
    {
        var screenShotSize = new Vector2Int(1920 * 2, 1080 * 2);
        var rt = new RenderTexture(screenShotSize.x, screenShotSize.y, 24);
        var prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            screenShotSize.x,
           screenShotSize.y,
            TextureFormat.RGBA32,
            false);
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        File.WriteAllBytes(filePath, bytes);
        Debug.Log("キャプチャ完了！");
    }
}