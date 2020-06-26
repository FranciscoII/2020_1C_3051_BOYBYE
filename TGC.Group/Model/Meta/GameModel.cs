using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;
using TGC.Group.Model.Meta;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        private Surface depthStencil;
        private Texture renderTarget;
        private VertexBuffer fullScreenQuad;

        private Effect effect;


        public Entorno EntornoActual { get; set; }
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        public override void Init()
        {
            GameManager.Instance.Frustum = this.Frustum;
            InputDelJugador input = new InputDelJugador(Input);
            EntornoActual = new EntornoMenu(this,MediaDir,input);
            EntornoActual.Init();
            CreateFullScreenQuad();
            CreateRenderTarget();
            effect = TGCShaders.Instance.LoadEffect(ShadersDir + "Post-Process.fx");
        }

        public override void Update()
        {
            PreUpdate();
            EntornoActual.Update(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            //PreRender();
            //PostRender();

            var device = D3DDevice.Instance.Device;

            // Capturamos las texturas de pantalla
            Surface screenRenderTarget = device.GetRenderTarget(0);
            Surface screenDepthSurface = device.DepthStencilSurface;

            // Especificamos que vamos a dibujar en una textura
            Surface surface = renderTarget.GetSurfaceLevel(0);
            device.SetRenderTarget(0, surface);
            device.DepthStencilSurface = depthStencil;

            // Captura de escena en render target
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            device.BeginScene();
            EntornoActual.Render();
            device.EndScene();
            // Fin de escena


            // Especificamos que vamos a dibujar en pantalla
            device.SetRenderTarget(0, screenRenderTarget);
            device.DepthStencilSurface = screenDepthSurface;

            // Dibujado de textura en full screen quad
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            device.BeginScene();

            effect.Technique = "PostProcess";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, fullScreenQuad, 0);
            effect.SetValue("renderTarget", renderTarget);

            // Dibujamos el full screen quad
            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            RenderFPS();
            RenderAxis();
            device.EndScene();

            device.Present();

            surface.Dispose();
        }

        public override void Dispose()
        {
            EntornoActual.Dispose();
        }

        public void CambiarCamara(TgcCamera nuevaCamara)
        {
            this.Camera = nuevaCamara;
            GameManager.Instance.camaraJuego = nuevaCamara;
        }
        private void CreateFullScreenQuad()
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

        private void CreateRenderTarget()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            depthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            renderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
        }
    }
}