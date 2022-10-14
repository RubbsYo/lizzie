using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;


namespace LizSoundPack
{
	public partial class ScreenShakePlayer : ModPlayer
	{
		float ScreenShake;
		Vector2 viewOffset;

		public void SetScreenShake(float shake)
        {
			ScreenShake = shake;
        }

		public void SetViewOffset(Vector2 offset)
		{
			viewOffset = offset;
		}
		public override void ModifyScreenPosition()
		{
			if (ScreenShake > 0)
			{
				Main.screenPosition.X += (float)Math.Round(Main.rand.Next((int)(0f - 1), (int)1) * ScreenShake);
				Main.screenPosition.Y += (float)Math.Round(Main.rand.Next((int)(0f - 1), (int)1) * ScreenShake);
				if (ScreenShake > 10)
				{
					ScreenShake *= 0.8f;
				}
				ScreenShake = Math.Max(ScreenShake - 1, 0);
			}
			if (viewOffset.X != 0 || viewOffset.Y != 0)
            {
				Main.screenPosition.X += (float)Math.Round(viewOffset.X);
				Main.screenPosition.Y += (float)Math.Round(viewOffset.Y);
				viewOffset.X = Utilities.Lerp(viewOffset.X, 0, 0.2f);
				viewOffset.Y = Utilities.Lerp(viewOffset.Y, 0, 0.2f);
				if (Math.Abs(viewOffset.X) < 1)
					viewOffset.X = 0;
				if (Math.Abs(viewOffset.Y) < 1)
					viewOffset.Y = 0;
			}
		}
	}
}