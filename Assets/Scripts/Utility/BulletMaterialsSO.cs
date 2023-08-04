using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "BulletMaterials", menuName = "Bullet Materials")]
    public class BulletMaterialsSO : ScriptableObject
    {
        public Material[] Materials;
    }
    
    // this would be a mesh container in a scenario where I have the fbx models for bullets:
    
    // public class BulletMeshesSO : ScriptableObject
    // {
    //     public Mesh[] Meshes;
    // }
}