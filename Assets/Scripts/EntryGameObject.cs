using Cysharp.Threading.Tasks;
using UniGLTF;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRM;

public sealed class EntryGameObject : MonoBehaviour
{
    [SerializeField] private Canvas _uiCanvas;
    [SerializeField] private ChangeBlendShapesView _changeBlendShapesView;
    [SerializeField] private ChangeAnimationsView _changeAnimationsView;
    [SerializeField] private AnimatorController _animatorController;
    
    // EntryPoint
    private async void Start()
    {
        var path = EditorUtility.OpenFilePanel("Open VRM", "", "vrm");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var model = LoadVrmModel(path);
        var animator = model.GetComponent<Animator>();
        animator.runtimeAnimatorController = _animatorController;
        await UniTask.Yield(); // BlendShapesを読み込むために1フレーム待つ
        _changeBlendShapesView.Initialize(model.GetComponent<VRMBlendShapeProxy>());
        _changeAnimationsView.Initialize(_animatorController);
        Change_MToonToUTS2.ChangeMaterials(model);
    }

    private GameObject LoadVrmModel(string path)
    {
        var parser = new AmbiguousGltfFileParser(path);
        var data = parser.Parse();

        using (var context = new VRMImporterContext(data))
        {
            RuntimeGltfInstance instance = context.Load();
            instance.EnableUpdateWhenOffscreen();
            instance.ShowMeshes();
            return instance.Root;
        }
    }
}