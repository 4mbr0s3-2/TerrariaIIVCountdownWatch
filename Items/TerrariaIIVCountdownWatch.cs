using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static TerrariaIIVCountdownWatch.TerrariaIIVCountdownWatch;

namespace TerrariaIIVCountdownWatch.Items
{

	public class TerrariaIIVCountdownWatch : ModItem
	{
		public static Color color = Color.White;
		//public static int opacity = 255;
		public static float size = 1;
		public static bool large = true;
		public static int displayType = 0;
		public static bool additive = true;
		public static DateTime endTime = new DateTime(2020, 05, 16, 0, 0, 0);
		public class CountdownUI : UIState
		{
			UIText text;
			//UIPanel panel;
			public override void OnInitialize()
			{ // 1
				//panel = new UIPanel();
				//panel.Top.Set(0, 0);
				//panel.HAlign = 0.5f;
				//Append(panel);
				text = new UIText("Loading countdown...");
				text.Top.Set(0, 0);
				text.OnMiddleClick += SwitchType;
				text.HAlign = 0.5f;
				Append(text);
			}
			private void SwitchType(UIMouseEvent evt, UIElement listeningElement)
			{
				displayType++;
				if (displayType > 1)
				{
					displayType = 0;
				}
			}
			public override void Draw(SpriteBatch spriteBatch)
			{
				if (additive)
				{
					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
					this.DrawSelf(spriteBatch);
					this.DrawChildren(spriteBatch);
					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
				}
				else
				{
					base.Draw(spriteBatch);
				}
			}
			public override void Update(GameTime gameTime)
			{
				//endTime = new DateTime(2020, 05, 11, 11, 32, 0);
				string countdownText = "";
				TimeSpan ts = endTime.Subtract(DateTime.Now);
				if (displayType == 0)
				{
					countdownText = ts.ToString("d' day" + (ts.Days != 1 ? "s" : "") + " 'h' hour" + (ts.Hours != 1 ? "s" : "") + " 'm' minute" + (ts.Minutes != 1 ? "s" : "") + " 's' second"+ (ts.Seconds != 1 ? "s" : "") + "'");
				}
				if (displayType == 1)
				{
					countdownText = ts.ToString("d'd 'h'h 'm'm 's's'");
				}
				if (DateTime.Now.CompareTo(endTime) > 0)
				{
					countdownText = "1.4 is out!";
				}
				text.Top.Set(20, 0);
				text.SetText(countdownText, size, large);
				text.TextColor = color;
			}
		}
		public static UserInterface CountdownInterface;
		public static CountdownUI countdownUI;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terraria 1.4 Countdown Watch");
		}
		public override void SetDefaults()
		{
			item.accessory = true;
			item.width = 30;
			item.height = 30;
		}
		public override void AddRecipes()
		{
			int[] altWatch = new int[] {ItemID.GoldWatch, ItemID.PlatinumWatch};
			int[] altBar = new int[] { ItemID.CrimtaneBar, ItemID.DemoniteBar};
			foreach (int altWatchID in altWatch)
			{
				foreach (int altBarID in altBar)
				{
					ModRecipe recipe = new ModRecipe(mod);
					recipe.AddIngredient(altWatchID, 1);
					recipe.AddIngredient(altBarID, 1);
					recipe.AddIngredient(ItemID.HallowedBar, 1);
					recipe.AddTile(TileID.Anvils);
					recipe.SetResult(this);
					recipe.AddRecipe();
				}
			}
		}
	}
	public class TerrariaIIVCountdownWatchPlayer : ModPlayer
	{
		public static void ShowMyUI()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				TerrariaIIVCountdownWatch.CountdownInterface.SetState(TerrariaIIVCountdownWatch.countdownUI);
			}
		}
		public static void HideMyUI()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				TerrariaIIVCountdownWatch.CountdownInterface.SetState(null);
			}
		}
		public override void PostUpdate()
		{
			bool watchFlag = false;
			for (int x = 0; x < 10; x++)
			{
				Item item = Main.LocalPlayer.armor[x];
				if (item.type == mod.ItemType("TerrariaIIVCountdownWatch") && item.stack > 0)
				{
					watchFlag = true;
					break;
				}
			}
			if ((Main.LocalPlayer.HasItem(mod.ItemType("TerrariaIIVCountdownWatch")) || watchFlag) && TerrariaIIVCountdownWatch.CountdownInterface.CurrentState == null)
			{
				ShowMyUI();
			}
			else if (!(Main.LocalPlayer.HasItem(mod.ItemType("TerrariaIIVCountdownWatch")) || watchFlag))
			{
				HideMyUI();
			}
		}
	}
	public class CrabRaveWorld : ModWorld
	{
		public static bool CrabRaveOngoing = false;
		public static bool CrabRave = false;
		public static int CrabRaveDuration = 0;
		public override void PostUpdate()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (DateTime.Now.CompareTo(TerrariaIIVCountdownWatch.endTime) > 0 && !CrabRaveOngoing && !CrabRave) // Occurs once
				{
					CrabRave = true;
					CrabRaveOngoing = true;
					foreach (Player player in Main.player)
					{
						Projectile.NewProjectile(player.Center, new Vector2(0, -3), mod.ProjectileType("IIVRocketProjectile"), 140, 2, player.whoAmI);
					}
					if (Main.netMode == NetmodeID.Server)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)TerrariaIIVCountdownMessageType.CrabRaveOngoing);
						netMessage.Write(CrabRaveWorld.CrabRaveOngoing);
						netMessage.Send();
					}
				}
				else if (DateTime.Now.CompareTo(TerrariaIIVCountdownWatch.endTime) < 0)
				{
					CrabRave = false; // Debugging purposes
					CrabRaveOngoing = false;
					CrabRaveDuration = 0;
				}
				if (CrabRave && CrabRaveOngoing)
				{
					CrabRaveDuration++;
					if (CrabRaveDuration > 960)
					{
						CrabRaveOngoing = false;
						if (Main.netMode == NetmodeID.Server)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)TerrariaIIVCountdownMessageType.CrabRaveOngoing);
							netMessage.Write(CrabRaveWorld.CrabRaveOngoing);
							netMessage.Send();
						}
					}
				}
			}
		}
	}
	public class CrabRave : GlobalNPC
	{
		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (CrabRaveWorld.CrabRaveOngoing)
			{
				pool.Clear();
				pool.Add(mod.NPCType("RavingCrab"), 1);
			}
		}
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (CrabRaveWorld.CrabRaveOngoing)
			{
				spawnRate = 1;
				maxSpawns = 999;
			}
		}
	}
}