using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingSandbox.Runtime.Framework
{
    public sealed class PassRenderer
    {
        private ProfilingSampler _profilingSampler;

        
        public Material Material
        {
            get => this._material;
            set => this._material = value;
        }

        private string _name;
        private Shader _shader;
        private Material _material;
        
        public PassRenderer(string name, string shaderPath)
        {
            _name = name;
            _profilingSampler = new ProfilingSampler(name);
            
            _shader = Shader.Find(shaderPath);
            if (this._shader == null)
            {
                throw new Exception($"Can not create Material for {name}: Shader not found!");
            }
        }


        public void Setup()
        {
            this._material = new Material(this._shader);
        }

        public void Render(
            ScriptableRenderContext context,
            Action<CommandBuffer> renderFunction 
        ) {
            if (this._material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, this._profilingSampler))
            {
                renderFunction(cmd);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public void Destory()
        {
            CoreUtils.Destroy(this._material);
        }
    }
}