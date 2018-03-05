using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PaletteMaterialEditor : MaterialEditor {

    public override void OnInspectorGUI() {
        if (!isVisible)
            return;

        Material material = target as Material;

        MaterialProperty[] properties = GetMaterialProperties(targets);

        string[] keys = material.shaderKeywords;

        bool colorEffectsLayerEnabled = keys.Contains("EFFECTS_LAYER_1_ON");
        bool sliceEffectsLayerEnabled = keys.Contains("SLICE_EFFECTS_LAYER_ON");
        bool burnEffectsLayerEnabled = keys.Contains("BURN_EFFECTS_LAYER_ON");

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < 2; i++)
            TexturePropertySingleLine(new GUIContent(properties[i].displayName), properties[i]);

        EditorGUILayout.Separator();
        
        //new
        colorEffectsLayerEnabled = EditorGUILayout.Toggle("Effects Layer 1", colorEffectsLayerEnabled);
        if (colorEffectsLayerEnabled)
            DrawEffectsLayer(properties, 1);

        sliceEffectsLayerEnabled = EditorGUILayout.Toggle("Effects Layer 2", sliceEffectsLayerEnabled);
        if (sliceEffectsLayerEnabled)
            DrawEffectsLayer(properties, 2);

        burnEffectsLayerEnabled = EditorGUILayout.Toggle("Burn Effects Layer", burnEffectsLayerEnabled);
        if (burnEffectsLayerEnabled)
            DrawEffectsLayer(properties, 3);

        if (EditorGUI.EndChangeCheck()) {
            string[] newKeys = new string[] {
             
                colorEffectsLayerEnabled ? "EFFECTS_LAYER_1_ON" : "EFFECTS_LAYER_1_OFF",
                sliceEffectsLayerEnabled ? "EFFECTS_LAYER_2_ON" : "EFFECTS_LAYER_1_OFF",
                burnEffectsLayerEnabled ? "BURN_EFFECTS_LAYER_ON" : "BURN_EFFECTS_LAYER_OFF",

            };

            material.shaderKeywords = newKeys;
            EditorUtility.SetDirty(material);
        }
    }

    void DrawEffectsLayer(MaterialProperty[] properties, int layer) {
        GUIStyle style = EditorStyles.helpBox;
        style.margin = new RectOffset(20, 20, 0, 0);

        EditorGUILayout.BeginVertical(style);
        {

            ColorProperty(properties.GetByName(EffectName(layer, "Color")), "Tint Color");
            ColorProperty(properties.GetByName(EffectName(layer, "OverrideColor")), "Override Color");

            TexturePropertySingleLine(new GUIContent("Slice Texture"), properties.GetByName(EffectName(layer, "SliceGuide")));
            RangeProperty(properties.GetByName(EffectName(layer, "SliceAmount")), "Slice Amount");
            

            /*
                TexturePropertySingleLine(new GUIContent("Effect Texture"), properties.GetByName(EffectName(layer, "Tex")));
                TexturePropertySingleLine(new GUIContent("Motion Texture"), properties.GetByName(EffectName(layer, "Motion")));

                ColorProperty(properties.GetByName(EffectName(layer, "Color")), "Tint Color");

                FloatProperty(properties.GetByName(EffectName(layer, "MotionSpeed")), "Motion Speed");
                FloatProperty(properties.GetByName(EffectName(layer, "Rotation")), "Rotation Speed");

                Vector4 translation = properties.GetByName(EffectName(layer, "Translation")).vectorValue;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Positon");
                    translation.x = EditorGUILayout.FloatField(translation.x);
                    translation.y = EditorGUILayout.FloatField(translation.y);
                }
                EditorGUILayout.EndHorizontal();
                properties.GetByName(EffectName(layer, "Translation")).vectorValue = translation;

                Vector4 pivotScale = properties.GetByName(EffectName(layer, "PivotScale")).vectorValue;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Pivot");
                    pivotScale.x = EditorGUILayout.FloatField(pivotScale.x);
                    pivotScale.y = EditorGUILayout.FloatField(pivotScale.y);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Scale");
                    pivotScale.z = EditorGUILayout.FloatField(pivotScale.z);
                    pivotScale.w = EditorGUILayout.FloatField(pivotScale.w);
                }
                EditorGUILayout.EndHorizontal();
                properties.GetByName(EffectName(layer, "PivotScale")).vectorValue = pivotScale;

                BoolProperty(properties.GetByName(EffectName(layer, "Foreground")), "Foreground");
            */
        }
        EditorGUILayout.EndVertical();
    }

    bool BoolProperty(MaterialProperty property, string name) {
        bool toggle = property.floatValue == 0 ? false : true;
        toggle = EditorGUILayout.Toggle(name, toggle);
        property.floatValue = toggle ? 1 : 0;

        return toggle;
    }

    string EffectName(int layer, string property) {
        return string.Format("_EffectsLayer{0}{1}", layer.ToString(), property);
    }

}
