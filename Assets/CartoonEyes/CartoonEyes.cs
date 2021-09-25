using UnityEngine;

namespace CartoonEyes
{
    static class CartoonEyesIDs
    {
        public static int outlineTexture;
        public static int outlineST;
        public static int outlineColor;
        public static int outlineThickness;
        public static int open;
        public static int scleraColor;
        public static int irisColor;
        public static int irisSize;
        public static int pupilSize;
        public static int aspectRatio;
        public static int position;

        static bool _hasInited = false;

        public static void Init()
        {
            if (_hasInited)
                return;
            _hasInited = true;

            outlineTexture = Shader.PropertyToID("_Outline");
            outlineST = Shader.PropertyToID("_Outline_ST");
            outlineColor = Shader.PropertyToID("_OutlineColor");
            outlineThickness = Shader.PropertyToID("_OutlineThickness");
            open = Shader.PropertyToID("_Open");
            scleraColor = Shader.PropertyToID("_ScleraColor");
            irisColor = Shader.PropertyToID("_IrisColor");
            irisSize = Shader.PropertyToID("_IrisSize");
            pupilSize = Shader.PropertyToID("_PupilSize");
            aspectRatio = Shader.PropertyToID("_AspectRatio");
            position = Shader.PropertyToID("_Position");
        }
    }

    [ExecuteInEditMode]
    public class CartoonEyes : MonoBehaviour
    {
        static Material _baseMaterial = null;

        public Renderer leftEye;
        public Renderer rightEye;

        [Header("Outline")]
        public Texture2D outlineTexture;
        public Color outlineColor;
        [Range(0.0f, 0.49f)]
        public float outlineThickness = 0.1f;
        [Range(0, 0.99f)]
        public float open = 0.5f;

        [Header("Sclera")]
        public Color scleraColor = Color.white;

        [Header("Iris")]
        public Color irisColor = Color.white;
        [Range(0.1f, 1.0f)]
        public float irisSize = 0.5f;
        [Range(0.01f, 1.0f)]
        public float pupilSize = 0.5f;
        public Vector2 target2D = Vector2.zero;
        public Transform target3D;

        void SetMaterialProperties(Material material, Vector2 offset, bool flip)
        {
            // outline
            if (outlineTexture != null)
                material.SetTexture(CartoonEyesIDs.outlineTexture, outlineTexture);
            material.SetVector(CartoonEyesIDs.outlineST, new Vector4(flip ? -1 : 1, 1, 0, 0));
            material.SetColor(CartoonEyesIDs.outlineColor, outlineColor);
            material.SetFloat(CartoonEyesIDs.outlineThickness, outlineThickness);
            material.SetFloat(CartoonEyesIDs.open, open);

            // sclera
            material.SetColor(CartoonEyesIDs.scleraColor, scleraColor);

            // iris
            material.SetColor(CartoonEyesIDs.irisColor, irisColor);
            material.SetFloat(CartoonEyesIDs.irisSize, irisSize);
            material.SetFloat(CartoonEyesIDs.pupilSize, pupilSize);
            material.SetFloat(CartoonEyesIDs.aspectRatio, 2);
            material.SetVector(CartoonEyesIDs.position, offset);
        }

        void SetMaterialProperties(Material material, Transform eyetransform, Vector3 target, bool flip)
        {
            Vector3 localpos = eyetransform.InverseTransformPoint(target);

            Vector2 target2d = new Vector2();
            target2d.x = -new Vector3(localpos.x, 0, localpos.z).normalized.x;
            target2d.y = new Vector3(0, localpos.y, localpos.z).normalized.y;

            SetMaterialProperties(material, target2d, flip);
        }

        void UpdateEye(Renderer renderer, bool flip)
        {
            if (renderer.sharedMaterial == null || renderer.sharedMaterial.shader != _baseMaterial.shader)
            {
                Material mat = Instantiate(_baseMaterial);
                mat.hideFlags = HideFlags.HideAndDontSave;
                renderer.sharedMaterial = mat;
            }

            if (target3D)
                SetMaterialProperties(renderer.sharedMaterial, renderer.transform, target3D.position, flip);
            else
                SetMaterialProperties(renderer.sharedMaterial, target2D, flip);
        }

        void Update()
        {
            if (_baseMaterial == null)
            {
                _baseMaterial = Resources.Load<Material>("CartoonEyeBase");
                if (_baseMaterial == null)
                {
                    Debug.LogError("Could not find CartoonEyeBase material in resource folder!");
                    return;
                }
            }

            CartoonEyesIDs.Init();

            if (leftEye != null)
                UpdateEye(leftEye, false);
            if (rightEye != null)
                UpdateEye(rightEye, true);
        }
    }
}