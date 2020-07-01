using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Meta
{
    public abstract class Entorno
    {
        internal GameModel gameModel;
        internal string mediaDir;
        internal InputDelJugador input;
        internal string shaderDir;

        //agregados para el bloom
        internal Surface depthStencil;
        internal Texture renderTarget;
        internal VertexBuffer fullScreenQuad;
        internal Texture glowyObjectsFrameBuffer, bloomHorizontalFrameBuffer, bloomVerticalFrameBuffer, sceneFrameBuffer;

        internal Effect effect;
        internal struct Light
        {
            public bool Enabled;
            public TGCVector3 Position;
            public TGCVector3 SpecularColor;
            public TGCVector3 DiffuseColor;
            public TGCVector3 AmbientColor;

            public void SetLight(int index, Effect effect)
            {
                effect.SetValue("lights[" + index + "].Position", TGCVector3.TGCVector3ToFloat3Array(Position));
                effect.SetValue("lights[" + index + "].SpecularColor", TGCVector3.TGCVector3ToFloat3Array(SpecularColor));
                effect.SetValue("lights[" + index + "].DiffuseColor", TGCVector3.TGCVector3ToFloat3Array(DiffuseColor));
                effect.SetValue("lights[" + index + "].AmbientColor", TGCVector3.TGCVector3ToFloat3Array(AmbientColor));
            }
        }


        public Entorno(GameModel gameModel,string mediaDir, InputDelJugador input,string shaderDir)
        {
            this.gameModel = gameModel;
            this.mediaDir = mediaDir;
            this.input = input;
            this.shaderDir = shaderDir;
        }

        abstract public void Init();
        abstract public void Update(float elapsedTime);
        abstract public void  Render();
        abstract public void Dispose();

        public virtual void CambiarEntorno(Entorno nuevoEntorno)
        {
            this.Dispose();
            nuevoEntorno.Init();
            gameModel.EntornoActual = nuevoEntorno;
        }

        internal void BeginScene(Device device, Surface surface, Surface depth)
        {
            device.SetRenderTarget(0, surface);
            device.DepthStencilSurface = depth;

            device.BeginScene();

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        }
        internal void InitializeFrameBuffers()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            depthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            glowyObjectsFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            bloomHorizontalFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            bloomVerticalFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            sceneFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
        }
        internal void CreateRenderTarget() //lo usaba el tgcExam
        {
            var d3dDevice = D3DDevice.Instance.Device;

            depthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            renderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
        }
        internal void CreateFullScreenQuad()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            // Creamos un FullScreen Quad
            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };

            // Vertex buffer de los triangulos
            fullScreenQuad = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            fullScreenQuad.SetData(vertices, 0, LockFlags.None);
        }
    }
}
