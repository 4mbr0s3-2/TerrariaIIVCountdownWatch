using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaIIVCountdownWatch.NPCs
{
    class RavingCrab : ModNPC
    {
        double acceleration = 0;
        float frame
        {
            get
            {
                return npc.localAI[1];
            }
            set
            {
                npc.localAI[1] = value;
            }
        }
        float speed = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;
            DisplayName.SetDefault("Raving Crab");
        }
        public override void SetDefaults()
        {
            npc.width = 44;
            npc.height = 38;
            npc.friendly = true;
            npc.aiStyle = 3;
            npc.lifeMax = 140;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                acceleration = Main.rand.Next(10, 30) / 100f;
            }
            npc.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(acceleration);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            acceleration = reader.ReadDouble();
        }
        public override void AI()
        {
            npc.TargetClosest();
            if (Math.Abs(npc.velocity.Y) <= 0.001f)
            {
                npc.velocity.Y = -7;
            }
            //if (Math.Abs(npc.velocity.X) <= 0.001f)
            //{
            //    speed = -npc.direction * 3;
            //    //return;
            //} 
            if (npc.position.X > Main.player[npc.target].position.X && npc.HasValidTarget)
            {
                speed -= (float)acceleration;
            }
            if (npc.position.X < Main.player[npc.target].position.X && npc.HasValidTarget)
            {
                speed += (float)acceleration;
            }
            if (speed > 4.5f)
            {
                speed = 4.5f;
            }
            if (speed < -4.5f)
            {
                speed = -4.5f;
            }
            npc.velocity.X = speed;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frameCounterThreshold = 5;
            if (npc.frameCounter >= frameCounterThreshold)
            {
                npc.frameCounter = 0;
                frame += 1;
                if (frame >= Main.npcFrameCount[npc.type])
                {
                    frame = 0;
                }
            }
            npc.spriteDirection = npc.direction;
            npc.frame.Y = (int)frame * frameHeight;
        }
    }
}
