using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Text;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    class Bloque:IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;

        private TGCMatrix ScaleMatrix;
        private TGCMatrix TranslationMatrix;
        private TgcScene Scene;
        private String nombreMapa;
        private List<TGCVector3> posicionesTorretas;
        private Nave nave;
        private List<ObstaculoMapa> listaObstaculos;
        private Effect effect;

        private TGCVector3 g_LightDir; // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCVector3 g_LightLookAt; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map
        private Texture g_pShadowMap; // Texture to which the shadow map is rendered
        // Shadow map
        private readonly int SHADOWMAP_SIZE = 1024;
        private readonly float far_plane = 3000f;
        private readonly float near_plane = 2f;
        //private Texture renderTarget;
        //private Surface depthStencil;

        public Bloque(string mediaDir, TGCVector3 posicionInicial,String nombreMapa,List<TGCVector3> posiciones,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.nombreMapa = nombreMapa;
            this.posicionesTorretas = posiciones;
            this.nave = nave;
            this.listaObstaculos = new List<ObstaculoMapa>();
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Scene = new TgcSceneLoader().loadSceneFromFile(mediaDir + nombreMapa);
            ScaleMatrix = TGCMatrix.Scaling(20f, 20f, 20f);
            TranslationMatrix = TGCMatrix.Translation(posicionInicial);
            instanciarObstaculosMapa();
            instanciarTorretas();
            effect = TGCShaders.Instance.LoadEffect("..\\..\\Shaders\\" + "ShadowMap.fx");
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) {
                mesh.Transform = ScaleMatrix * TranslationMatrix;
                mesh.BoundingBox.transform(ScaleMatrix * TranslationMatrix);
                mesh.Effect = effect; mesh.Technique = "RenderScene";
            });
            //cosas nuevas
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE, 1, Usage.RenderTarget, Format.R32F, Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE, SHADOWMAP_SIZE, DepthFormat.D24S8, MultiSampleType.None, 0, true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);//9000 era originalmente 5000
            
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, near_plane, far_plane).ToMatrix();
            
            var posicionNave = nave.GetPosicion();
            //g_LightPos = new TGCVector3(posicionNave.X, posicionNave.Y+70, posicionNave.Z - 200);
            //g_LightLookAt = new TGCVector3(posicionNave.X, posicionNave.Y, posicionNave.Z + 100);
            g_LightPos = new TGCVector3(posicionInicial.X+90,posicionInicial.Y+20,posicionInicial.Z-100);
            g_LightLookAt = new TGCVector3(posicionInicial.X+90,posicionInicial.Y,posicionInicial.Z);



        }

        private void instanciarTorretas()
        {
            this.posicionesTorretas.
                ForEach(delegate (TGCVector3 posicion) {
                    Torreta torreta = new Torreta(mediaDir,posicion,nave);
                    GameManager.Instance.AgregarRenderizable(torreta); 
                });
        }
        private void instanciarObstaculosMapa()
        {
            for (int i = 3; i < Scene.Meshes.Count; i++)//hardcodeado esto deberia cambiar
            {
                ObstaculoMapa obstaculo = new ObstaculoMapa(nave, Scene.Meshes[i]);
                listaObstaculos.Add(obstaculo);
                GameManager.Instance.AgregarRenderizable(obstaculo);
            }
        }

        public void Update(float elapsedTime)
        {

            if (nave.GetPosicion().Z > posicionInicial.Z + 3000f) //este numero deberia corrseponder idealmente al tamaño del bloque
            {
                for (int i = 0; i < listaObstaculos.Count; i++)
                    GameManager.Instance.QuitarRenderizable(listaObstaculos[i]);

                GameManager.Instance.QuitarRenderizable(this);
                //Logger.Loggear("Se borro el bloque de posicion: " + this.posicionInicial.ToString() + " nombre: " + this.nombreMapa);
            }
            
        }

        public void Render()
        {
            //RenderScene(false);
            //D3DDevice.Instance.Device.EndScene(); // termino el thread anterior
            //TexturesManager.Instance.clearAll();

            //D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            //D3DDevice.Instance.Device.BeginScene();
            //g_LightDir = g_LightLookAt - g_LightPos;

            g_LightPos.X = 105;//mitad del pasillo: 105
            g_LightPos.Y = 510;
            g_LightPos.Z = nave.GetPosicion().Z + 10;

            g_LightDir = new TGCVector3(0, -1f, -0.1f);
            //g_LightDir = new TGCVector3(105,-29,nave.GetPosicion().Z + 30) - g_LightPos;
            //g_LightDir = new TGCVector3(115, -20, nave.GetPosicion().Z+30) - g_LightPos;
            g_LightDir.Normalize();


            // Shadow maps:
            //D3DDevice.Instance.Device.EndScene(); // termino el thread anterior

            //D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);

            //Genero el shadow map
            RenderShadowMap(); //begin y end

            //D3DDevice.Instance.Device.BeginScene();
            // dibujo la escena pp dicha
            //D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            RenderScene(false);

            //D3DDevice.Instance.Device.EndScene();
            //D3DDevice.Instance.Device.Present();


        }
        public void RenderShadowMap()
        {
            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new TGCVector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            effect.SetValue("g_vLightDir", new TGCVector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            effect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            effect.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
           // D3DDevice.Instance.Device.BeginScene();

            // Hago el render de la escena pp dicha
            effect.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);

            // Termino
           // D3DDevice.Instance.Device.EndScene();

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }

        public void RenderScene(bool shadow)
        {
            foreach (var T in Scene.Meshes)
            {
                
                if (shadow)
                {
                    T.Technique = "RenderShadow";
                }
                else
                {
                    T.Technique = "RenderScene";
                }
                if (GameManager.Instance.esVisibleParaLaCamara(T))
                    T.Render();
            }

        }


        public void Dispose()
        {
            Scene.DisposeAll();
        }
    }
}
