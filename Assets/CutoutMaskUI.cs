using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutoutMaskUI : Image {
    Material customMaskMaterial = null;
    public override Material materialForRendering {
        get {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            customMaskMaterial = material; //Init For Destroy
            return material;
        }
    }

    protected override void OnDestroy() {
        if (customMaskMaterial != null) {
            DestroyImmediate(customMaskMaterial);
        }
    }
}
