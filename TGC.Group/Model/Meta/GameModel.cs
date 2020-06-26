using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;
using TGC.Group.Model.Meta;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
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
            //EntornoActual = new EntornoMenu(this,MediaDir,input);
            EntornoActual = new EntornoJuego(this, MediaDir, input);
            EntornoActual.Init();
        }

        public override void Update()
        {
            PreUpdate();
            EntornoActual.Update(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();
            EntornoActual.Render();
            PostRender();
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
    }
}