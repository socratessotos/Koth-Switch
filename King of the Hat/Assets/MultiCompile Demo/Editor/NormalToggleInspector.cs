using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class NormalToggleInspector : CustomMaterialEditor {
    protected override void CreateToggleList() {
        Toggles.Add(new FeatureToggle("Normal Enabled", "normal", "NORMALMAP_ON", "NORMALMAP_OFF"));
    }
}