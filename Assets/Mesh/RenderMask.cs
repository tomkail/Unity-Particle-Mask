using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderMask : MonoBehaviour {
    public Texture2D maskTexture;

    [SerializeField]
    Renderer backingRenderer = null;
    Material backingMaterial;
    
    Matrix4x4 worldToUVMatrix;

    void OnEnable () {
        backingRenderer.sharedMaterial = backingMaterial = new Material(backingRenderer.sharedMaterial);
    }
    void OnDisable () {
        backingMaterial.SetTexture("_MaskTex", null);
    }

    void Update () {
        RefreshMaterialMatrix();
    }
    
    void RefreshMaterialMatrix () {
        // Transform from (-0.5,-0.5)/(0.5,0.5) space to (0,0)/(1,1) space
        var uvOffset = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0), Quaternion.identity, Vector3.one);
        worldToUVMatrix = uvOffset * transform.worldToLocalMatrix;

        backingMaterial.SetMatrix("_ObjectMatrix", worldToUVMatrix);
        backingMaterial.SetTexture("_MaskTex", maskTexture);
    }

    void OnDrawGizmosSelected () {
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldMatrix;
    }
}