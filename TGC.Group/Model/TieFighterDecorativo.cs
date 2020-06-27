using TGC.Core.Mathematica;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using System.Drawing.Text;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class TieFighterDecorativo : IRenderizable
    {
        private TGCVector3 posicion;
        private ModeloCompuesto modeloNave;
        private string mediaDir;
        private TGCMatrix matrizEscala;
        private TGCMatrix matrizPosicion;
        private TGCMatrix matrizRotacion;
        private float coolDownDisparo;
        private TieFighterSpawner spawner;
        private TgcMesh mainMesh;


        public TieFighterDecorativo(string mediaDir, TGCVector3 posicionInicial, TieFighterSpawner spawner)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\xwing-TgcScene.xml", posicion);
            coolDownDisparo = 0f;
            this.spawner = spawner;
        }


        public void Init()
        {

            matrizEscala = TGCMatrix.Scaling(.5f, .5f, .5f);
            matrizPosicion = TGCMatrix.Translation(posicion);
            TGCQuaternion rotacionInicial = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f));
            matrizRotacion = TGCMatrix.RotationTGCQuaternion(rotacionInicial);
            mainMesh = modeloNave.GetMesh();
        }

        public void Update(float elapsedTime)
        {
            if (GameManager.Instance.estaPausado)
                return;

            var posicionDisparo = new TGCVector3(posicion);
            posicionDisparo.Z += 10;

            modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(135f), 0f));

            SeguirTrayectoria(elapsedTime);
            Disparar(posicionDisparo, elapsedTime);
            
            modeloNave.UpdateShader(GameManager.Instance.PosicionSol, GameManager.Instance.EyePosition(), 0f);
        }

        public void Render()
        {
            modeloNave.AplicarTransformaciones();
            modeloNave.Render();
        }

        public void Dispose()
        {
            modeloNave.Dispose();
        }


        private void SeguirTrayectoria(float elapsedTime)
        {
            TGCVector3 versorDirector = new TGCVector3(-1, 0, 1);
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 0);
            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 60f * elapsedTime;
            posicion += movimientoDelFrame;
            modeloNave.CambiarPosicion(posicion);
        }
        private void Disparar(TGCVector3 posicionNave, float tiempoTranscurrido)
        {
            coolDownDisparo += tiempoTranscurrido;
            TGCVector3 direccionDisparo = posicionNave - posicion;
            if (coolDownDisparo > 4f)
            {
                //GameManager.Instance.AgregarRenderizable(new LaserEnemigo(mediaDir, posicion, direccionDisparo, naveDelJugador));
                coolDownDisparo = 0f;
            }


        }


    }
}
