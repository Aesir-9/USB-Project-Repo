using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class CustomLightingGUI : ShaderGUI
{
    private enum Expandable
    {
        SurfaceInputs = 1,
        SpecularInputs = 2,
    }

    private enum SpecularModels
    {
        None = 0, 
        BPhong = 1,
        Ashikhmin = 2,
        GGX = 3,
    }

    private MaterialEditor materialEditor { get; set; }
    private MaterialProperty baseMapProp { get; set; }
    private MaterialProperty matcapProp { get; set; }
    private MaterialProperty lightInfluenceProp { get; set; }
    private MaterialProperty specularModelProp { get; set; }
    private MaterialProperty phongColorProp { get; set; }
    private MaterialProperty phongSpecularFactorProp { get; set; }
    private MaterialProperty phongRoughnessProp { get; set; }
    private MaterialProperty ashikhminColorProp { get; set; }
    private MaterialProperty ashikhminSpecularFactorProp { get; set; }
    private MaterialProperty ashikhminAUProp { get; set; }
    private MaterialProperty ashikhminAVProp { get; set; }
    private MaterialProperty ggxColorProp { get; set; }
    private MaterialProperty ggxSpecularFactorProp { get; set; }
    private MaterialProperty ggxRoughnessProp { get; set; }

    private bool firstTimeApply = true;
    
    // All sections expanded by default
    readonly MaterialHeaderScopeList materialScopeList = new MaterialHeaderScopeList(uint.MaxValue);

    // Here are the strings.
    private class Styles
    {
        public static readonly string[] SpecularModelsNames = Enum.GetNames(typeof(SpecularModels));
        
        public static readonly GUIContent SurfaceInputs = EditorGUIUtility.TrTextContent("Surface Inputs");
        public static readonly GUIContent SpecularInputs = EditorGUIUtility.TrTextContent("Specular Inputs");
        
        public static readonly GUIContent BaseMap = EditorGUIUtility.TrTextContent("Albedo Map");
        public static readonly GUIContent MatcapMap = EditorGUIUtility.TrTextContent("Matcap Map");
        public static readonly GUIContent LightInfluence = EditorGUIUtility.TrTextContent("Light Influence");
        public static readonly GUIContent SpecularModel = EditorGUIUtility.TrTextContent("Specular Model");

        public static readonly GUIContent PhongColor = EditorGUIUtility.TrTextContent("Color");
        public static readonly GUIContent PhongReflectionFactor = EditorGUIUtility.TrTextContent("Specular Factor");
        public static readonly GUIContent PhongRoughness = EditorGUIUtility.TrTextContent("Roughness");
        
        public static readonly GUIContent AshikhminColor = EditorGUIUtility.TrTextContent("Color");
        public static readonly GUIContent AshikhminReflectionFactor = EditorGUIUtility.TrTextContent("Specular Factor");
        public static readonly GUIContent AshikhminAu = EditorGUIUtility.TrTextContent("AU");
        public static readonly GUIContent AshikhminAv = EditorGUIUtility.TrTextContent("AV");
        
        public static readonly GUIContent GgxColor = EditorGUIUtility.TrTextContent("Color");
        public static readonly GUIContent GgxReflectionFactor = EditorGUIUtility.TrTextContent("Specular Factor");
        public static readonly GUIContent GgxRoughness = EditorGUIUtility.TrTextContent("Roughness");
    }

    public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        if (materialEditorIn == null)
        {
            throw new ArgumentNullException(nameof(materialEditorIn));
        }
        
        materialEditor = materialEditorIn;
        Material material = materialEditor.target as Material;
        
        FindProperties(properties); 
        
        if (firstTimeApply)
        {
            OnOpenGUI();
            firstTimeApply = false;
        }
        
        ShaderPropertiesGUI(material);
    }

    private void FindProperties(MaterialProperty[] properties)
    {
        var material = materialEditor?.target as Material;
        
        if (material == null)
        {
            return;
        }
        
        baseMapProp = FindProperty("_Base_Color", properties, false);
        matcapProp = FindProperty("_Matcap", properties, false);
        lightInfluenceProp = FindProperty("_Light_Influence", properties, false);
        specularModelProp = FindProperty("_SPECULAR_MODEL", properties, false);
        
        phongColorProp = FindProperty("_Phong_Color", properties, false);
        phongSpecularFactorProp = FindProperty("_Phong_Specular_Factor", properties, false);
        phongRoughnessProp = FindProperty("_Phong_Roughness", properties, false);
        
        ashikhminColorProp = FindProperty("_Ashikhmin_Color", properties, false);
        ashikhminSpecularFactorProp = FindProperty("_Ashikhmin_Specular_Factor", properties, false);
        ashikhminAUProp = FindProperty("_Ashikhmin_AU", properties, false);
        ashikhminAVProp = FindProperty("_Ashikhmin_AV", properties, false);
        
        ggxColorProp = FindProperty("_GGX_Color", properties, false);
        ggxSpecularFactorProp = FindProperty("_GGX_Specular_Factor", properties, false);
        ggxRoughnessProp = FindProperty("_GGX_Roughness", properties, false);
    }

    private void OnOpenGUI()
    {
        materialScopeList.RegisterHeaderScope(Styles.SurfaceInputs, (uint)Expandable.SurfaceInputs, DrawSurfaceInputs);
        materialScopeList.RegisterHeaderScope(Styles.SpecularInputs, (uint)Expandable.SpecularInputs, DrawSpecularInputs);
    }

    private void DrawSpecularInputs(Material material)
    {
        if (specularModelProp == null) 
            return;
        
        EditorGUI.BeginChangeCheck();
        float oldValue = specularModelProp.floatValue;
            
        DoPopup(Styles.SpecularModel, specularModelProp, Styles.SpecularModelsNames);

        if (EditorGUI.EndChangeCheck())
        {
            float newValue = specularModelProp.floatValue;

            if (!Mathf.Approximately(oldValue, newValue))
            {
                OnSpecularModelChanges(material, (int)newValue);
            }
        }
           
        SpecularModels currentModel = (SpecularModels)(int)specularModelProp.floatValue;

        if (currentModel != SpecularModels.None)
        {
            DrawMatcapProperty();
        }

        if (currentModel == SpecularModels.BPhong)
        {
            DrawPhongProperties();
        }

        if (currentModel == SpecularModels.Ashikhmin)
        {
            DrawAshikhminProperties();
        }

        if (currentModel == SpecularModels.GGX)
        {
            DrawGGXProperties();
        }
    }

    private void DrawSurfaceInputs(Material material)
    {
        if (baseMapProp != null) 
        {
            materialEditor.TexturePropertySingleLine(Styles.BaseMap, baseMapProp);
        }
        
        if (lightInfluenceProp != null)
        {
            materialEditor.ShaderProperty(lightInfluenceProp, Styles.LightInfluence);
        }
    }

    private void DrawMatcapProperty()
    {
        if (matcapProp != null)
        {
            materialEditor.TexturePropertySingleLine(Styles.MatcapMap, matcapProp);
        }
    }

    private void DrawPhongProperties()
    {
        if (phongColorProp != null && phongSpecularFactorProp != null && phongRoughnessProp != null)
        {
            materialEditor.ShaderProperty(phongColorProp, Styles.PhongColor);
            materialEditor.ShaderProperty(phongSpecularFactorProp, Styles.PhongReflectionFactor);
            materialEditor.ShaderProperty(phongRoughnessProp, Styles.PhongRoughness);
        }
    }

    private void DrawAshikhminProperties()
    {
        if (ashikhminAUProp != null && ashikhminAVProp != null && ashikhminColorProp != null && ashikhminSpecularFactorProp != null)
        {
            materialEditor.ShaderProperty(ashikhminColorProp, Styles.AshikhminColor);
            materialEditor.ShaderProperty(ashikhminSpecularFactorProp, Styles.AshikhminReflectionFactor);
            materialEditor.ShaderProperty(ashikhminAUProp, Styles.AshikhminAu);
            materialEditor.ShaderProperty(ashikhminAVProp, Styles.AshikhminAv);
        }
    }

    private void DrawGGXProperties()
    {
        if (ggxColorProp != null && ggxSpecularFactorProp != null && ggxRoughnessProp != null)
        {
            materialEditor.ShaderProperty(ggxColorProp, Styles.GgxColor);
            materialEditor.ShaderProperty(ggxSpecularFactorProp, Styles.GgxReflectionFactor);
            materialEditor.ShaderProperty(ggxRoughnessProp, Styles.GgxRoughness);
        }
    }

    private void OnSpecularModelChanges(Material material, int modelIndex)
    {
        SpecularModels model = (SpecularModels)modelIndex;
        
        material.DisableKeyword("_SPECULAR_MODEL_NONE");
        material.DisableKeyword("_SPECULAR_MODEL_BPHONG");
        material.DisableKeyword("_SPECULAR_MODEL_ASHIKHMIN");
        material.DisableKeyword("_SPECULAR_MODEL_GGX");
        
        switch (model)
        {
            case SpecularModels.None:
                material.EnableKeyword("_SPECULAR_MODEL_NONE");
                break;
            case SpecularModels.BPhong:
                material.EnableKeyword("_SPECULAR_MODEL_BPHONG");
                break;
            case SpecularModels.Ashikhmin:
                material.EnableKeyword("_SPECULAR_MODEL_ASHIKHMIN");
                break;
            case SpecularModels.GGX:
                material.EnableKeyword("_SPECULAR_MODEL_GGX");
                break;
        }
        
        EditorUtility.SetDirty(material);
    }

    private void DoPopup(GUIContent label, MaterialProperty property, string[] options)
    {
        if (property != null)
        {
            materialEditor.PopupShaderProperty(property, label, options);
        }
    }
    
    private void ShaderPropertiesGUI(Material material)
    {
        materialScopeList.DrawHeaders(materialEditor, material);
    }

}
