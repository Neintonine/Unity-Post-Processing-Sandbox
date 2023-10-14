using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.Framework
{
    public abstract class PostProcessingVolume : VolumeComponent, IPostProcessComponent
    {
        protected PostProcessingPass pass;

        public virtual bool IsActive()
        {
            return true;
        }

        public virtual bool IsTileCompatible()
        {
            return false;
        }

        public PostProcessingPass GetPass()
        {
            if (this.pass == null)
            {
                this.pass = this.CreatePass();
            }

            return this.pass;
        }

        protected abstract PostProcessingPass CreatePass();
    }

    public abstract class PostProcessingVolume<TPass> : PostProcessingVolume
        where TPass : PostProcessingPass, new()
    {
        protected override PostProcessingPass CreatePass()
        {
            TPass pass = new TPass();
            pass.Setup(this);
            return pass;
        }
    }
}