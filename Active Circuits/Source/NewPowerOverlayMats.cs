using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace DActiveCircuits
{
    [StaticConstructorOnStartup]
    class NewPowerOverlayMats
    {
        static NewPowerOverlayMats()
        {
            Graphic graphic = GraphicDatabase.Get<Graphic_Single>(NewPowerOverlayMats.TransmitterAtlasPath, NewPowerOverlayMats.TransmitterShader);
            NewPowerOverlayMats.LinkedOverlayGraphic = (Graphic_LinkedTransmitterOverlay)GraphicUtility.WrapLinked(graphic, LinkDrawerType.TransmitterOverlay);
            graphic.MatSingle.renderQueue = 3600;
        }

        private const string TransmitterAtlasPath = "Things/Special/Power/WhiteTransmitterAtlas";
        public static Shader TransmitterShader = ShaderDatabase.MetaOverlay;
        public static Graphic_LinkedTransmitterOverlay LinkedOverlayGraphic;
    }
}
