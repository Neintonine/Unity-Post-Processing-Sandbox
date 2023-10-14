using PostProcessingSandbox.Runtime.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.DualToneDithering
{
    public sealed class DualToneDitheringPass: PostProcessingPass<DualToneDithering>
    {
        private const string SHADER = "PostProcessing/DualToneDithering";
        
        private PassRenderer _renderer = new("Dual Tone Dithering", DualToneDitheringPass.SHADER);

        private readonly int _temporaryRTId = Shader.PropertyToID("_SceneColor");
        private RTHandle _temporaryRtHandle;

        private RTHandle _depthTexture;
        
        protected override RenderPassEvent GetEvent()
        {
            return RenderPassEvent.BeforeRenderingPostProcessing;
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

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            
            cmd.GetTemporaryRT(this._temporaryRTId, renderingData.cameraData.cameraTargetDescriptor);
        }
        
        
        private void Render(CommandBuffer cmd)
        {
            this._renderer.Material.SetTexture("_DitherPattern", this.Volume.DitherPattern.value);
            this._renderer.Material.SetFloat("_DitherPatternScale", Mathf.Pow(this.Volume.PatternScale.value, -2));
            
            this._renderer.Material.SetColor("_BackgroundColor", this.Volume.Background.value);
            this._renderer.Material.SetColor("_ForegroundColor", this.Volume.Foreground.value);

            if (this.Volume.AffectedByDepth.value)
            {
                this._renderer.Material.EnableKeyword("DEPTH_AFFECTED");
                this._renderer.Material.SetTexture("_CameraDepthTexture", this._depthTexture);
            }
            else
            {
                this._renderer.Material.DisableKeyword("DEPTH_AFFECTED");
            }
            
#if UNITY_EDITOR
            this.SetDebugViewMode(DualToneDithering.DebugViewMode.VIEW_SCENE_BRIGHTHNESS, "VIEW_SCENE_BRIGHTHNESS");
            this.SetDebugViewMode(DualToneDithering.DebugViewMode.VIEW_SCENE_BRIGHTNESS_WITH_DEPTH, "VIEW_SCENE_BRIGHTNESS_WITH_DEPTH");
#endif
            
            cmd.CopyTexture(this.TargetHandle, this._temporaryRTId);
            Blitter.BlitCameraTexture(cmd, this.TargetHandle, this.TargetHandle, this._renderer.Material, 0);
        }

        private void SetDebugViewMode(DualToneDithering.DebugViewMode viewMode, string keyword)
        {
            if (this.Volume.ViewMode.value == viewMode)
            {
                this._renderer.Material.EnableKeyword(keyword);
            }
            else
            {
                this._renderer.Material.DisableKeyword(keyword);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            this._renderer.Render(context, this.Render);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
            cmd.ReleaseTemporaryRT(this._temporaryRTId);
        }
    }
}