using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace TerrariaIIVCountdownWatch
{
	public class TerrariaIIVCountdownWatchConfig : ModConfig
	{
		[Tooltip("0: D days, H hours, M minutes, S seconds\n1: Dd Hh Mm Ss")]
		[Slider()]
		[DrawTicks()]
		[Increment(1)]
		[Range(0, 1)]
		[Label("Countdown Type")]
		[DefaultValue(0)]
		public int DisplayMode;

		[Slider()]
		[Range(0.01f, 2)]
		[Label("Countdown Size")]
		[DefaultValue(1)]
		public float CountdownSize;

		[Label("Large Countdown")]
		[DefaultValue(true)]
		public bool LargeCountdown;

		[Label("Countdown Color")]
		[SliderColor(255, 255, 255)]
		[DefaultValue(typeof(Color), "255, 255, 255, 255")]
		public Color CountdownColor;

		[DefaultValue(false)]
		[Label("Additive")]
		public bool Additive;

		public override ConfigScope Mode => ConfigScope.ClientSide;

		public override void OnChanged()
		{
			Items.TerrariaIIVCountdownWatch.displayType = DisplayMode;
			Items.TerrariaIIVCountdownWatch.color = CountdownColor;
			Items.TerrariaIIVCountdownWatch.size = CountdownSize;
			Items.TerrariaIIVCountdownWatch.large = LargeCountdown;
			Items.TerrariaIIVCountdownWatch.additive = Additive;
			// Here we use the OnChanged hook to initialize ExampleUI.visible with the new values.
			// We maintain both ExampleUI.visible and ShowCoinUI as separate values so ShowCoinUI can act as a default while ExampleUI.visible can change within a play session.

		}
	}
}
