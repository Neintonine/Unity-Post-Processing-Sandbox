using System;
using System.Runtime.CompilerServices;
using PostProcessingSandbox.Runtime.Framework;
using PostProcessingSandbox.Runtime.OutlineEffect;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostProcessingSandbox.Runtime.DualToneDithering
{
    public sealed class DualToneDithering : PostProcessingVolume<DualToneDitheringPass>
    {
        public enum DebugViewMode
        {
            NORMAL,
            VIEW_SCENE_BRIGHTHNESS,
            VIEW_SCENE_BRIGHTNESS_WITH_DEPTH,
        }
        [Serializable]
        public class DebugViewModeParameter : VolumeParameter<DebugViewMode>
        {
            public DebugViewModeParameter(DebugViewMode value, bool overrideState = false) : base(value, overrideState)
            {
                
            }
        }
        
        [Header("Dithering")]
        public TextureParameter DitherPattern = new(null);
        public MinIntParameter PatternScale = new(1, 1);

        [Header("Coloring")] 
        public ColorParameter Background = new(Color.black, false, false, true);
        public ColorParameter Foreground = new(Color.white, true, false, true);

        [Header("Depth")]
        public BoolParameter AffectedByDepth = new BoolParameter(false);
        
        [Header("Debug")]
        public DebugViewModeParameter ViewMode = new(DebugViewMode.NORMAL);
        
        public override bool IsActive()
        {
            return this.DitherPattern.value != null;
        }
    }
}