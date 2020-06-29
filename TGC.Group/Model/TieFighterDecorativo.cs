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
        private TgcMesh mainMesh;
        private Nave nave;
        private TGCVector3 posicionInicial;
        private EnumPosiciones enumPosiciones;
        private TGCVector3 versorDirector;
        private bool esTie;

        public TieFighterDecorativo(string mediaDir, TGCVector3 posicionInicial, bool esTie,Nave nave,EnumPosiciones Salida)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.posicionInicial = posicionInicial;
            this.esTie = esTie;
            if(esTie)
                this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\xwing-TgcScene.xml", posicion);
            else
                this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
            coolDownDisparo = 0f;
            this.nave = nave;
            this.enumPosiciones = Salida;
        }


        public void Init()
        {

            matrizEscala = TGCMatrix.Scaling(.5f, .5f, .5f);
            matrizPosicion = TGCMatrix.Translation(posicion);
            TGCQuaternion rotacionInicial = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f));
            matrizRotacion = TGCMatrix.RotationTGCQuaternion(rotacionInicial);
            mainMesh = modeloNave.GetMesh();
            ConfigurarParaSalida();
        }

        public void Update(float elapsedTime)
        {
            if (GameManager.Instance.estaPausado)
                return;
            var pos = nave.GetPosicion();
            if (EstaFueraDelRango())
            {
                this.posicion = new TGCVector3(this.posicionInicial.X, this.posicionInicial.Y, pos.Z + 185);
                modeloNave.CambiarPosicion(this.posicion);
                return;
            }
                

            var posicionDisparo = new TGCVector3(posicion);
            posicionDisparo.Z += 10;

            //modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(135f), 0f));

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
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            movimientoDelFrame += versorDirector;
            movimientoDelFrame *= 35f * elapsedTime;
            posicion += movimientoDelFrame;
            modeloNave.CambiarPosicion(posicion);
        }
        private void Disparar(TGCVector3 posicionNave, float tiempoTranscurrido)
        {
            coolDownDisparo += tiempoTranscurrido;
            //TGCVector3 direccionDisparo = posicionNave - posicion;
            if (!esTie && coolDownDisparo > 1f)
            {
                var laser = new LaserDecorativo(mediaDir,mediaDir + "Xwing\\laser-TgcScene.xml", posicion, versorDirector);
                laser.SetVelocidad(3f);
                GameManager.Instance.AgregarRenderizable(laser);
                coolDownDisparo = 0f;
            }
        }
        private void ConfigurarParaSalida()
        {
            switch (enumPosiciones)
            {
                case EnumPosiciones.IZQUIERDA:
                    versorDirector = new TGCVector3(1.5f, 0f, 1.5f);
                    modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(-135f), 0f));
                    break;
                case EnumPosiciones.DERECHA:
                    versorDirector = new TGCVector3(-1f, 0f, 1.5f);
                    modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(135f), 0f));
                    break;
            }
        }
        private bool EstaFueraDelRango()
        {
            var ret = false;
            switch (enumPosiciones)
            {
                case EnumPosiciones.IZQUIERDA:
                    ret = this.mainMesh.Position.X > 500;
                    break;
                case EnumPosiciones.DERECHA:
                    ret = this.mainMesh.Position.X < -250;
                    break;
            }
            return ret;
        }


    }
}
