using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaIIVCountdownWatch.Items;
using static TerrariaIIVCountdownWatch.Items.TerrariaIIVCountdownWatch;

namespace TerrariaIIVCountdownWatch
{
    public class TerrariaIIVCountdownWatch : Mod
	{
        internal enum TerrariaIIVCountdownMessageType : byte
        {
            CrabRaveOngoing
        }
        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (CountdownInterface?.CurrentState != null)
            {
                CountdownInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "TIVCountdown: Countdown Interface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && CountdownInterface?.CurrentState != null)
                        {
                            CountdownInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));
            }
        }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                CountdownInterface = new UserInterface();

                countdownUI = new CountdownUI();
                countdownUI.Activate(); // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            }
        }
        public TerrariaIIVCountdownWatch()
		{
		}
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            TerrariaIIVCountdownMessageType msgType = (TerrariaIIVCountdownMessageType)reader.ReadByte();
            switch (msgType)
            {
                // This message sent by the server to initialize the Volcano Tremor on clients
                case TerrariaIIVCountdownMessageType.CrabRaveOngoing:
                    CrabRaveWorld.CrabRaveOngoing = reader.ReadBoolean();
                    break;
            }
        }
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if ((Items.TerrariaIIVCountdownWatch.endTime.Subtract(DateTime.Now).TotalSeconds < 7.69
                && Items.TerrariaIIVCountdownWatch.endTime.Subtract(DateTime.Now).TotalSeconds >= 0)
                || CrabRaveWorld.CrabRaveOngoing)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/CrabRave");
                priority = MusicPriority.BossHigh;
            }
        }
    }
}