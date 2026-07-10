using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class CreadorMaterialParticulas : Editor
{
    [MenuItem("Tools/Crear Material Particulas URP")]
    static void CrearMaterial()
    {
        // Busca el shader URP de particulas
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");

        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError("No se encontro shader compatible");
            return;
        }

        Material mat = new Material(shader);
        mat.name = "MaterialParticulas";

        // Guarda el material en Assets
        AssetDatabase.CreateAsset(mat, "Assets/MaterialParticulas.mat");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Material creado en Assets/MaterialParticulas.mat");
        Selection.activeObject = mat;
    }
}
#endif