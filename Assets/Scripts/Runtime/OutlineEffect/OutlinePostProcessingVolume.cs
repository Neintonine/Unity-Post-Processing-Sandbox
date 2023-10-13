using PostProcessingSandbox.Runtime.Framework;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostProcessingSandbox.Runtime.OutlineEffect
{
    [VolumeComponentMenu("Post-processing/Outlines")]
    public sealed class OutlinePostProcessingVolume : PostProcessingVolume<OutlinePass>
    {
        public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter OutlineColor = new ColorParameter(Color.black, hdr: true, false, true);
        public ClampedIntParameter OutlineWidth = new ClampedIntParameter(2, 1, 10);
        
        [Header("Outline Strength")]
        public FloatParameter SobelPower = new FloatParameter(0.4f);

        public ClampedFloatParameter EdgeFeathering = new ClampedFloatParameter(0f, 0f, 1);

        [Header("Vertical")] 
        public FloatParameter VerticalDiagonalCoefficent = new FloatParameter(2);
        public FloatParameter VerticalAxisCoefficent = new FloatParameter(2);
        [Header("Horizontal")] 
        public FloatParameter HorizontalDiagonalCoefficent = new FloatParameter(2);
        public FloatParameter HorizontalAxisCoefficent = new FloatParameter(2);
        
        [Header("Debug (Only avaliable in Editor)")] 
        public BoolParameter DisplayOnlyOutline = new BoolParameter(false);
        
        public override bool IsActive()
        {
            return this.Intensity.value > 0;
        }
    }
}