using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleMask : MonoBehaviour {
    public Texture2D maskTexture;
    
    [SerializeField]
    new ParticleSystem particleSystem = null;
    Material particleMaterial;
    
    Matrix4x4 worldToUVMatrix;

    void OnEnable () {
        var psr = particleSystem.GetComponent<ParticleSystemRenderer>();
        psr.sharedMaterial = particleMaterial = new Material(psr.sharedMaterial);
    }
    void OnDisable () {
        particleMaterial.SetTexture("_MaskTex", null);
    }

    void Update () {
        RefreshMaterialMatrix();
    }
    
    void RefreshMaterialMatrix () {
        // Transform from (-0.5,-0.5)/(0.5,0.5) space to (0,0)/(1,1) space
        var uvOffset = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0), Quaternion.identity, Vector3.one);
        worldToUVMatrix = uvOffset * transform.worldToLocalMatrix;
        
        particleMaterial.SetMatrix("_ObjectMatrix", worldToUVMatrix);
        particleMaterial.SetTexture("_MaskTex", maskTexture);
    }

    void OnDrawGizmosSelected () {
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldMatrix;
    }
}