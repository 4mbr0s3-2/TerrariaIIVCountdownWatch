using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using static TerrariaIIVCountdownWatch.TerrariaIIVCountdownWatch;

namespace TerrariaIIVCountdownWatch.Projectiles
{
    class IIVRocketProjectile : ModProjectile
    {
		public override string Texture => "TerrariaIIVCountdownWatch/Projectiles/IIVRocketProjectile";
		public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RocketFireworkRed);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("1.4 Rocket");
        }
        public override void Kill(int timeLeft)
        {
            MemoryStream memoryStream = new MemoryStream();
            Texture2D referencePic = ModContent.GetTexture("TerrariaIIVCountdownWatch/Projectiles/FireworkReference");
            referencePic.SaveAsPng(memoryStream, referencePic.Width, referencePic.Height);
            Bitmap img = new Bitmap(memoryStream);
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if (pixel.R != 0)
                    {
                        float velocityX = i - img.Width / 2;
                        float velocityY = j - img.Height / 2;
                        int dustIndex2 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), 6, 6, 130, 0f, 0f, 100, default(Microsoft.Xna.Framework.Color), 1f);
                        Main.dust[dustIndex2].color = new Microsoft.Xna.Framework.Color(pixel.R, pixel.G, pixel.B, pixel.A);
                        Main.dust[dustIndex2].velocity.X = velocityX;
                        Main.dust[dustIndex2].velocity.Y = velocityY;
                        Main.dust[dustIndex2].scale = 1.3f;
			            Main.dust[dustIndex2].noGravity = true;
                    }
                }
            }
        }
    }
    public class IIVRocketTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
        }
        public override void HitWire(int i, int j)
        {
            this.LaunchRocket(i, j);
            Wiring.SkipWire(i, j);
        }
        public override bool NewRightClick(int i, int j)
        {
            this.LaunchRocket(i, j);
            return true;
        }
        public void LaunchRocket(int x, int y)
        {
            int i = (int)Main.tile[x, y].frameY;
            int num = 0;
            while (i >= 40)
            {
                i -= 40;
                num++;
            }
            if (i == 18)
            {
                y--;
            }
            Vector2 vector = new Vector2((float)(x * 16 + 8), (float)(y * 16 + 4));
            int type = mod.ProjectileType("IIVRocketProjectile");
            int damage = 150;
            int num2 = 7;
            Projectile.NewProjectile(vector.X, vector.Y + 2f, 0f, -8f, type, damage, (float)num2, Main.myPlayer, 0f, 0f);
            Main.tile[x, y].active(false);
            Main.tile[x, y + 1].active(false);
            NetMessage.SendTileSquare(-1, x - 1, y, 3, TileChangeType.None);
        }
    }
    class IIVRocketItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("1.4 Rocket");
        }
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.RedRocket);
            item.createTile = mod.TileType("IIVRocketTile");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RedRocket, 1);
            recipe.AddIngredient(ItemID.GreenRocket, 1);
            recipe.AddIngredient(ItemID.BlueRocket, 1);
            recipe.AddIngredient(ItemID.YellowRocket, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
