﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Mathematica;
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model.Meta
{
    internal class EntornoMenu : Entorno
    {
        private TgcCamera camaraDeMenu;
        private MenuPrincipal menuPrincipal;

        public EntornoMenu(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
            menuPrincipal = new MenuPrincipal(mediaDir);
            camaraDeMenu = new TgcCamera();
            camaraDeMenu.SetCamera(new TGCVector3(0, 20, 250), new TGCVector3(0, 20, 0));
        }

        public override void Init()
        {
            gameModel.CambiarCamara(camaraDeMenu);
            NaveDeMenu naveDeMenu = new NaveDeMenu(mediaDir);
            GameManager.Instance.AgregarRenderizable(naveDeMenu);
            Skybox skybox = new Skybox(mediaDir, camaraDeMenu);
            GameManager.Instance.AgregarRenderizable(skybox);
        }

        public override void Render()
        {
            GameManager.Instance.Render();
            menuPrincipal.DibujarMenu();
        }

        public override void Update(float elapsedTime)
        {
            if (input.HayInputDePausa())
            {
                CambiarEntorno(new EntornoJuego(gameModel, mediaDir, input));
            }
            else
            {
                camaraDeMenu.UpdateCamera(elapsedTime);
                GameManager.Instance.Update(elapsedTime);
            }

        }
        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
    }

}