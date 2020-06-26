using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Shaders;
using TGC.Core.Text;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model.Meta
{
    internal class EntornoJuego : Entorno
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        private Nave naveDelJuego;

        public EntornoJuego(GameModel gameModel, string mediaDir, InputDelJugador input, string shaderDir) : base(gameModel, mediaDir, input, shaderDir)
        {
        }

        public override void Init()
        {
            var posicionInicialDeNave = new TGCVector3(105, -15, 420);
            naveDelJuego = new Nave(mediaDir, posicionInicialDeNave, input);
            //Debe empezar pausado
            //GameManager.Instance.PausarJuego();
            
            GameManager.Instance.AgregarRenderizable(naveDelJuego);
            CamaraDelJuego camaraDelJuego = new CamaraDelJuego(posicionInicialDeNave, 10, -50, naveDelJuego);
            gameModel.CambiarCamara(camaraDelJuego);
            Skybox skybox = new Skybox(mediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);
            escenarioLoader = new EscenarioLoader(mediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(mediaDir, naveDelJuego);
            GameManager.Instance.ReanudarOPausarJuego();
            CreateFullScreenQuad();
            CreateRenderTarget();
            InitializeFrameBuffers();
            effect = TGCShaders.Instance.LoadEffect(shaderDir + "Bloom.fx");
        }

        public override void Update(float elapsedTime)
        {
            //UPDATE del Bloom-----------------------
            effect.SetValue("scene", true);
            effect.SetValue("bloom", false);
            effect.SetValue("toneMapping", false);
            effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(GameManager.Instance.EyePosition()));
            //-------------------

            if (input.HayInputDePausa())
            {
                GameManager.Instance.ReanudarOPausarJuego();
            }

            GameManager.Instance.Update(elapsedTime);
            escenarioLoader.Update(elapsedTime);
            tieFighterSpawner.Update(elapsedTime);
            if (!naveDelJuego.estaVivo)
                CambiarEntorno(new EntornoGameOver(gameModel, mediaDir, input,shaderDir));
        }

        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
        public override void Render()
        {
            var device = D3DDevice.Instance.Device;

            var screenSurface = device.GetRenderTarget(0);
            var originalDepthStencil = device.DepthStencilSurface;

            // --------------
            // Guardamos la escena en un framebuffer

            BeginScene(device, sceneFrameBuffer.GetSurfaceLevel(0), depthStencil);

            //ACA RENDEREAR LA MIERDA XDXDCXDXDXD
            if (GameManager.Instance.estaPausado)
            {
                string textoControles = "Controles:\nWASD: Moverse\nQ: Rollear\nE: Voltearse\nShift: Acelerar\nCtrl: Desacelerar\nEnter: Pausar/Despausar";
                TgcText2D textoDrawer = new TgcText2D();
                textoDrawer.Text = textoControles;
                textoDrawer.changeFont(new System.Drawing.Font("Calibri", 0.009765625f * D3DDevice.Instance.Width));
                textoDrawer.drawText(textoControles, D3DDevice.Instance.Width / 40, D3DDevice.Instance.Height / 20, Color.White);
            }
            GameManager.Instance.Render();


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
