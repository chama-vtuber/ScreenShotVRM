using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UniRx;

public class ScreenShot : MonoBehaviour
{

    [SerializeField] private Camera _captureCamera;
    [SerializeField] private Button _captureButton;


    private void Start()
    {
        SetBinds();
    }
    
    private void SetBinds()
    {
        _captureButton.OnClickAsObservable().Subscribe(async unit =>
        {
            CaptureScreenShot(GetPath("ScreenCaptures"), _captureCamera);
        }).AddTo(this);
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