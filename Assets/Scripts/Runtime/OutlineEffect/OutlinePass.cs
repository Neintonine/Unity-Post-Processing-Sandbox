using PostProcessingSandbox.Runtime.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.OutlineEffect
{
    public sealed class OutlinePass: PostProcessingPass<OutlinePostProcessingVolume>
    {
        private const string SHADER_PATH = "PostProcessing/OutlineShader";
        
        private PassRenderer _renderer;
        private RTHandle _depthTexture;
        
        public OutlinePass()
        {
            this.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            this._renderer = new PassRenderer("OutlinePostProcessing", OutlinePass.SHADER_PATH);
        }
        
        public override void Setup(PostProcessingVolume volume)
        {
            base.Setup(volume);
            
            this._renderer.Setup();
        }

        public override void SetupForRendering(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            base.SetupForRendering(renderer, in renderingData);
            this._depthTexture = renderer.cameraDepthTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            this._renderer.Render(context, this.Render);
        }

        private void Render(CommandBuffer cmd)
        {
#if UNITY_EDITOR
            if (this.Volume.DisplayOnlyOutline.value)
            {
                this._renderer.Material.EnableKeyword("OUTLINE_VIEW");
            }
            else
            {
                this._renderer.Material.DisableKeyword("OUTLINE_VIEW");
            }
#endif
            
            this._renderer.Material.SetFloat("_Intensity", this.Volume.Intensity.value);
            this._renderer.Material.SetTexture("_CameraDepthTexture", this._depthTexture);
            
            this._renderer.Material.SetColor("_OutlineColor", this.Volume.OutlineColor.value);
            this._renderer.Material.SetFloat("_SobelPower", this.Volume.SobelPower.value);
            this._renderer.Material.SetFloat("_HorizDiagCoeff", this.Volume.HorizontalDiagonalCoefficent.value);
            this._renderer.Material.SetFloat("_HorizAxisCoeff", this.Volume.HorizontalAxisCoefficent.value);
            this._renderer.Material.SetFloat("_VertDiagCoeff", this.Volume.VerticalDiagonalCoefficent.value);
            this._renderer.Material.SetFloat("_VertAxisCoeff", this.Volume.VerticalAxisCoefficent.value);
            
            Blitter.BlitCameraTexture(cmd, this.TargetHandle, this.TargetHandle, this._renderer.Material, 0);
        }
    }
}