using System;
using ExternPropertyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// ReSharper disable RedundantDefaultMemberInitializer

namespace FlatKit {
[CreateAssetMenu(fileName = "OutlineSettings", menuName = "FlatKit/Outline Settings")]
public class OutlineSettings : ScriptableObject {
#if !UNITY_2022_3_OR_NEWER
    [InfoBox("Since Flat Kit v4 image effects require Unity 2022.3 or newer. " +
             "Please upgrade your Unity version to use this feature.", EInfoBoxType.Warning), Space]
#endif

    #region Outline
    [Tooltip("The color of the lines. Alpha is used for transparency, " +
             "0 means fully transparent and 1 means fully opaque lines.")]
    public Color edgeColor = Color.white;

    [Range(0, 5)]
    [Tooltip("The width of the lines in screen space. If 'Resolution Invariant' is disabled, " +
             "this is the width in pixels. Otherwise, it is a relative width.")]
    public int thickness = 1;

    [Tooltip("If enabled, the line width will stay constant regardless of the rendering resolution. " +
             "However, some of the lines may appear blurry.")]
    public bool resolutionInvariant = false;
    #endregion

    [HorizontalLine]

    #region Depth
    [Tooltip("Whether to use depth information to draw outlines. This adds lines around objects that are in front of " +
             "other objects.")]
    public bool useDepth = true;

    [ShowIf(nameof(useDepth))]
    [Label("    Min Threshold")]
    [Min(0)]
    [Tooltip("Minimum distance between two pixels to be considered an edge. The outline is drawn almost transparent.")]
    public float minDepthThreshold = 0.1f;

    [ShowIf(nameof(useDepth))]
    [Label("    Max Threshold")]
    [Min(0)]
    [Tooltip("Maximum distance between two pixels to be considered an edge. The outline is drawn fully opaque.")]
    public float maxDepthThreshold = 0.25f;
    #endregion

    [HorizontalLine(1, EColor.Translucent)]

    #region Normals
    [Tooltip("Whether to use world-space normals information to draw outlines. This adds lines " +
             "on sharp edges of objects.")]
    public bool useNormals = false;

    [ShowIf(nameof(useNormals))]
    [Label("    Min Threshold")]
    [Min(0)]
    [Tooltip("Minimum angle between two normals to be considered an edge. The outline is drawn almost transparent.")]
    public float minNormalsThreshold = 0.1f;

    [ShowIf(nameof(useNormals))]
    [Label("    Max Threshold")]
    [Min(0)]
    [Tooltip("Maximum angle between two normals to be considered an edge. The outline is drawn fully opaque.")]
    public float maxNormalsThreshold = 0.25f;
    #endregion

    [HorizontalLine(1, EColor.Translucent)]

    #region Color
    [Tooltip("Whether to use color information to draw outlines. This adds lines where the color of the object " +
             "changes.")]
    public bool useColor = false;

    [ShowIf(nameof(useColor))]
    [Label("    Min Threshold")]
    [Min(0)]
    [Tooltip("Minimum difference in color between two pixels to be considered an edge. The outline is drawn almost " +
             "transparent.")]
    public float minColorThreshold = 0.1f;

    [ShowIf(nameof(useColor))]
    [Label("    Max Threshold")]
    [Min(0)]
    [Tooltip("Maximum difference in color between two pixels to be considered an edge. The outline is drawn fully " +
             "opaque.")]
    public float maxColorThreshold = 0.25f;
    #endregion

    [HorizontalLine]

    #region Post Processing
    [Tooltip("The render stage at which the effect is applied. To exclude transparent objects, like water or UI " +
             "elements, set this to \"Before Transparent\".")]
    public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingPostProcessing;

    [Tooltip("Only draw the outline, replacing the original color with a complimentary color.")]
    public bool outlineOnly = false;

    [Tooltip("Whether the effect should be applied in the Scene view as well as in the Game view. Please keep in " +
             "mind that Unity always renders the scene view with the default Renderer settings of the URP config.")]
    public bool applyInSceneView = true;

    internal Action onSettingsChanged;
    internal Action onReset;
    #endregion

    private void OnValidate() {
        if (minDepthThreshold > maxDepthThreshold + float.Epsilon) {
            Debug.LogWarning("<b>[Flat Kit]</b> Outline configuration error: 'Min Depth Threshold' must not " +
                             "be greater than 'Max Depth Threshold'");
        }

        if (minNormalsThreshold > maxNormalsThreshold + float.Epsilon) {
            Debug.LogWarning("<b>[Flat Kit]</b> Outline configuration error: 'Min Normals Threshold' must not " +
                             "be greater than 'Max Normals Threshold'");
        }

        if (minColorThreshold > maxColorThreshold + float.Epsilon) {
            Debug.LogWarning("<b>[Flat Kit]</b> Outline configuration error: 'Min Color Threshold' must not " +
                             "be greater than 'Max Color Threshold'");
        }

        onSettingsChanged?.Invoke();
    }

    private void Reset() {
        onReset?.Invoke();
    }

    private void OnDestroy() {
        onSettingsChanged = null;
        onReset = null;
    }
}
}