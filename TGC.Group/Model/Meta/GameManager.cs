using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Examples.Camara;

namespace TGC.Group.Model
{

    internal class GameManager
    {
        private List<IRenderizable> Renderizables = new List<IRenderizable>();
        public bool estaPausado { get; set; }
        private float cooldownPausa;
        public TgcFrustum Frustum { get; set; }
        public TgcCamera camaraJuego { get; set; }
        public TGCVector3 PosicionSol { get; set; }
        private Microsoft.DirectX.DirectSound.Device device;
        private List<Tgc3dSound> SonidosAReproducir = new List<Tgc3dSound>();

        public void SetearDevice(Microsoft.DirectX.DirectSound.Device device)
        {
            this.device = device;
        }
        public void Update(float elapsedTime)
        {
            List<IRenderizable> RenderizablesAuxiliar = new List<IRenderizable>(Renderizables);
            RenderizablesAuxiliar.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Update(elapsedTime); });
            if (cooldownPausa < 3f)
                cooldownPausa += elapsedTime;
        }

        public void Render()
        {
            ReproducirSonidos();
            List<IRenderizable> RenderizablesAuxiliar = new List<IRenderizable>(Renderizables);
            RenderizablesAuxiliar.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Render(); });
        }

        private void ReproducirSonidos()
        {
            foreach (Tgc3dSound sonido in SonidosAReproducir)
            {
                sonido.play(false);
            }

            SonidosAReproducir = new List<Tgc3dSound>();
        }

        public void Dispose()
        {
            Renderizables.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Dispose(); });
            Renderizables = new List<IRenderizable>();
        }

        public void AgregarRenderizable(IRenderizable unRenderizable)
        {
            Renderizables.Add(unRenderizable);
            unRenderizable.Init();
        }

        public void QuitarRenderizable(IRenderizable unRenderizable)
        {
            Renderizables.Remove(unRenderizable);
            unRenderizable.Dispose();
        }

        public Boolean HayUnLaserEnBoundingBox(TgcBoundingAxisAlignBox unBoundingBox)
        {
            return Renderizables.OfType<Laser>().Any(laser => TgcCollisionUtils.testAABBAABB(laser.GetMainMesh().BoundingBox, unBoundingBox));
        }
        public Laser LaserQueColisiona(TgcBoundingAxisAlignBox unBoundingBox)
        {
            var lista = new List<Laser>(Renderizables.OfType<Laser>().Where(laser => TgcCollisionUtils.testAABBAABB(laser.GetMainMesh().BoundingBox, unBoundingBox)));

            return lista[0];
        }
        public void PausarJuego()
        {
            this.estaPausado = true;
        }
        public void ReanudarOPausarJuego()
        {
            if (cooldownPausa > 0.5f)
            {
                this.estaPausado = !this.estaPausado;
                cooldownPausa = 0f;
            }
        }

        public List<Colisionable> GetColisionables()
        {
            return new List<Colisionable>(Renderizables.OfType<Colisionable>());
        }

        public List<ObstaculoMapa> GetObstaculosMapa()
        {
            return new List<ObstaculoMapa>(Renderizables.OfType<ObstaculoMapa>());
        }
        public List<Destruible> GetDestruibles()
        {
            return new List<Destruible>(Renderizables.OfType<Destruible>());
        }
        public Boolean esVisibleParaLaCamara(TgcMesh mesh)
        {
            TgcCollisionUtils.FrustumResult result = TgcCollisionUtils.classifyFrustumAABB(this.Frustum, mesh.BoundingBox);
            return result != TgcCollisionUtils.FrustumResult.OUTSIDE;
        }
        public TGCVector3 EyePosition()
        {
            return camaraJuego.Position;
        }

        public void ReproducirSonido(string direccionSonido, TGCVector3 posicionSonido) {
            Tgc3dSound nuevoSonido = new Tgc3dSound(direccionSonido, posicionSonido, device);
            SonidosAReproducir.Add(nuevoSonido);
        }
        
        #region Singleton

        private static volatile GameManager instance;
        
        private static readonly object syncRoot = new object();

        /// <summary>
        ///     Permite acceder a una instancia de la clase GameManager desde cualquier parte del codigo.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new GameManager();
                }
                return instance;
            }
        }

        #endregion Singleton
        
    }
}
