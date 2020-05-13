using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;


namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private TgcScene Scene;

        public override void Init()
        {
            Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Xwing\\TRENCH_RUN-TgcScene.xml");


            var posicionInicialDeNave = new TGCVector3(0, 5, -250); 

            Nave naveDelJuego = new Nave(MediaDir, posicionInicialDeNave,Input);
            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            Camara camaraDelJuego = new Camara(posicionInicialDeNave, 30, -150, naveDelJuego);
            Camera = camaraDelJuego;
            GameManager.Instance.Camara = camaraDelJuego;

            Skybox skybox = new Skybox(MediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);

            Torreta torreta = new Torreta(MediaDir, new TGCVector3(10,2, 15), naveDelJuego);
            GameManager.Instance.AgregarRenderizable(torreta);

            Torreta torreta2 = new Torreta(MediaDir, new TGCVector3(-10, 2, 15), naveDelJuego);
            GameManager.Instance.AgregarRenderizable(torreta2);

            TGCVector3 direccionDisparo = posicionInicialDeNave - new TGCVector3(10, 2, 15);
            torreta.Disparar(direccionDisparo);
            torreta2.Disparar(posicionInicialDeNave - new TGCVector3(-10, 2, 15));

        }

        public override void Update()
        {
            PreUpdate();
            GameManager.Instance.Update(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();
            Scene.RenderAll();
            GameManager.Instance.Render();
            PostRender();
        }

        public override void Dispose()
        {
            Scene.DisposeAll();
            GameManager.Instance.Dispose();
        }
    }
}