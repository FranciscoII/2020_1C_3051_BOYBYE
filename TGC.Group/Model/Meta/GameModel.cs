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
        private Texture glowyObjectsFrameBuffer, bloomHorizontalFrameBuffer, bloomVerticalFrameBuffer, sceneFrameBuffer;

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
            InitializeFrameBuffers();
            effect = TGCShaders.Instance.LoadEffect(ShadersDir + "Bloom.fx");
        }

        public override void Update()
        {
            PreUpdate();
            EntornoActual.Update(ElapsedTime);
            effect.SetValue("scene", true);
            effect.SetValue("bloom", false);
            effect.SetValue("toneMapping", false);
            effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(GameManager.Instance.EyePosition()));
            PostUpdate();
        }


        public override void Render()
        {
            //PreRender();

            var device = D3DDevice.Instance.Device;

            var screenSurface = device.GetRenderTarget(0);
            var originalDepthStencil = device.DepthStencilSurface;

            // --------------
            // Guardamos la escena en un framebuffer

            BeginScene(device, sceneFrameBuffer.GetSurfaceLevel(0), depthStencil);

            EntornoActual.Render();

            device.EndScene();

            // --------------
            // Guardamos lo brillante de la escena en un framebuffer
            /*
            BeginScene(device, glowyObjectsFrameBuffer.GetSurfaceLevel(0), depthStencil);

            EntornoActual.GetNave().Technique = "GlowyObjects";
            trafficLight.Render();

            EndScene(device);
            */
            // --------------
            // Aplicamos una pasada de blur horizontal al framebuffer de los objetos brillantes
            // y una pasada vertical
            var passBuffer = glowyObjectsFrameBuffer;
            for (int index = 0; index < 10; index++)
            {
                BeginScene(device, bloomHorizontalFrameBuffer.GetSurfaceLevel(0), depthStencil);

                effect.Technique = "HorizontalBlur";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, fullScreenQuad, 0);
                effect.SetValue("glowyFrameBuffer", passBuffer);

                effect.Begin(FX.None);
                effect.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                effect.EndPass();
                effect.End();

                device.EndScene();


                BeginScene(device, bloomVerticalFrameBuffer.GetSurfaceLevel(0), depthStencil);

                effect.Technique = "VerticalBlur";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, fullScreenQuad, 0);
                effect.SetValue("glowyFrameBuffer", bloomHorizontalFrameBuffer);

                effect.Begin(FX.None);
                effect.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                effect.EndPass();
                effect.End();

                device.EndScene();

                passBuffer = bloomVerticalFrameBuffer;
            }

            // --------------
            // Este postprocesado integra lo blureado a la escena

            BeginScene(device, screenSurface, originalDepthStencil);

            effect.Technique = "Integrate";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, fullScreenQuad, 0);
            effect.SetValue("verticalBlurFrameBuffer", bloomVerticalFrameBuffer);
            effect.SetValue("sceneFrameBuffer", sceneFrameBuffer);

            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            device.EndScene();
            device.Present();
            
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

        //----------BLOOM--------
        private void BeginScene(Device device, Surface surface, Surface depth)
        {
            device.SetRenderTarget(0, surface);
            device.DepthStencilSurface = depth;

            device.BeginScene();

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        }
        private void InitializeFrameBuffers()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            depthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            glowyObjectsFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            bloomHorizontalFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            bloomVerticalFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            sceneFrameBuffer = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
        }
        private struct Light
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
        private Light[] lights =
        {
            // Red
            new Light
            {
                Enabled = true,
                Position = new TGCVector3(115f, -18f, 455f),
                AmbientColor = new TGCVector3(0.2f, 0.1f, 0.1f),
                DiffuseColor = new TGCVector3(1f, 0.0f, 0.0f),
                SpecularColor = new TGCVector3(0.9f, 0.8f, 0.8f),
            },
            // Yellow
            new Light
            {
                Enabled = true,
                Position = new TGCVector3(115f, -25f, 455f),
                AmbientColor = new TGCVector3(0.2f, 0.2f, 0.05f),
                DiffuseColor = new TGCVector3(1f, 1f, 0f),
                SpecularColor = new TGCVector3(0.85f, 0.85f, 0.65f)
            },
            // Green
            new Light
            {
                Enabled = true,
                Position = new TGCVector3(115f, -22f, 455f),
                AmbientColor = new TGCVector3(0.05f, 0.2f, 0.05f),
                DiffuseColor = new TGCVector3(0f, 1f, 0f),
                SpecularColor = new TGCVector3(0.65f, 0.9f, 0.65f)
            }
    };
    }
}