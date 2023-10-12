using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.Framework
{
    public abstract class PostProcessingPass : ScriptableRenderPass
    {
        protected PostProcessingVolume Volume { get; private set; }
        
        protected RTHandle TargetHandle { get; private set; }
        
        public virtual void Setup(PostProcessingVolume volume)
        {
            this.Volume = volume;
        }

        public virtual void SetupForRendering(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            this.TargetHandle = renderer.cameraColorTargetHandle;
        }
    }
    
    public abstract class PostProcessingPass<TVolume> : PostProcessingPass
        where TVolume: PostProcessingVolume
    {
        protected new TVolume Volume
        {
            get => (TVolume) base.Volume; 
        }
    }
}