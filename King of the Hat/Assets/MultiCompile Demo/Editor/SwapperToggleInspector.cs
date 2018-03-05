using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SwapperToggleInspector : CustomMaterialEditor {

    protected override void CreateToggleList() {
        Toggles.Add(new FeatureToggle("Color Enabled", "color", "COLOR_ON", "COLOR_OFF"));
        Toggles.Add(new FeatureToggle("Slice Enabled", "slice", "SLICE_ON", "SLICE_OFF"));
        Toggles.Add(new FeatureToggle("Burn Enabled", "burn", "BURN_ON", "BURN_OFF"));
    }

}
