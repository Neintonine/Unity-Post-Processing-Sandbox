using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.Framework
{
    public sealed class PostProcessingFeature : ScriptableRendererFeature
    {
        public List<string> Effects
        {
            get => this._effects;
            set => this._effects = value;
        }
        
        [SerializeField] private List<string> _effects = new();

        private List<Type> _typedEffects;
        
        public override void Create()
        {
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            ICollection<PostProcessingVolume> volumes = this.GetRenderVolumes();
            
            foreach (PostProcessingVolume postProcessingVolume in volumes)
            {
                renderer.EnqueuePass(postProcessingVolume.GetPass());
            }
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            ICollection<PostProcessingVolume> volumes = this.GetRenderVolumes();
            foreach (PostProcessingVolume postProcessingVolume in volumes)
            {
                postProcessingVolume.GetPass().SetupForRendering(renderer, renderingData);
            }
        }

        private List<Type> GetEffectTypes()
        {
            if (this._typedEffects == null)
            {
                this._typedEffects = new List<Type>();
                foreach (string effect in this._effects)
                {
                    this._typedEffects.Add(Type.GetType(effect));
                }
            }

            return this._typedEffects;
        }
        
        private List<PostProcessingVolume> GetRenderVolumes()
        {
            VolumeStack stack = VolumeManager.instance.stack;

            List<PostProcessingVolume> volumes = new();
            foreach (Type postProcessingVolume in this.GetEffectTypes())
            {
                PostProcessingVolume component = stack.GetComponent(postProcessingVolume) as PostProcessingVolume;

                if (!component)
                {
                    continue;
                }
                
                if (!component.IsActive())
                {
                    continue;
                }
                
                volumes.Add(component);
            }

            return volumes;
        }
    }
}