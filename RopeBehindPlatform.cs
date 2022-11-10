using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Tile_Entities;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.Utilities;
using LizSoundPack.Core.Effects;
using LizSoundPack.Content.Effects;

namespace LizSoundPack.Content;

public class RopeBehindPlatform : ModSystem
{
	public static bool IsRope(int x, int y)
	{
		if (Main.tile[x, y] == null)
			return false;

		Tile mytile = Main.tile[x, y];

		if (Main.tileRope[mytile.TileType])
			return true;

		if ((Main.tile[x, y].TileType == 314 || TileID.Sets.Platforms[Main.tile[x, y].TileType]) && Main.tile[x, y - 1] != null && Main.tile[x, y + 1] != null && Main.tileRope[Main.tile[x, y - 1].TileType] && Main.tileRope[Main.tile[x, y + 1].TileType])
			return true;

		return false;
	}

	private Texture2D GetTileDrawTexture(Tile tile, int tileX, int tileY)
	{
		Texture2D result = TextureAssets.Tile[tile.TileType].Value;

		return result;
	}

	public override void Load()
	{
        On.Terraria.Player.FindPulley += Player_FindPulley;

		/*IL.Terraria.Main.DrawInventory += context =>
		{
			var cursor = new ILCursor(context);
			while (cursor.TryGotoNext(i => i.MatchLdcI4(56)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 50);
			}
			while (cursor.TryGotoNext(i => i.MatchLdcI4(58)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 50);
			}
			while (cursor.TryGotoNext(i => i.MatchLdcR4(56f)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 50);
			}
			while (cursor.TryGotoNext(i => i.MatchLdcI4(47)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 42);
			}
			while (cursor.TryGotoNext(i => i.MatchLdcI4(-47)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, -42);
			}
		};

		IL.Terraria.UI.ChestUI.DrawSlots += context =>
		{
			var cursor = new ILCursor(context);

			while (cursor.TryGotoNext(i => i.MatchLdcI4(56)))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 50);
			}

		};*/

		IL.Terraria.GameContent.Drawing.TileDrawing.DrawBasicTile += context =>
		{
			var cursor = new ILCursor(context);

			cursor.Emit(OpCodes.Ldarg, 1); //load screenposition
			cursor.Emit(OpCodes.Ldarg, 2); //load screenoffset
			cursor.Emit(OpCodes.Ldarg, 3); //load tilex
			cursor.Emit(OpCodes.Ldarg, 4); //load tiley
			cursor.Emit(OpCodes.Ldarg, 5); //load drawdata
			cursor.Emit(OpCodes.Ldarg, 6); //load normaldrawrect
			cursor.Emit(OpCodes.Ldarg, 7); //load normaltileposition

			cursor.EmitDelegate<Action<Vector2, Vector2, int, int, TileDrawInfo, Rectangle, Vector2>>((screenposition, screenoffset, tilex, tiley, drawData, normalTileRect, normalTilePosition) =>
			{
				if (TileID.Sets.Platforms[drawData.typeCache] && IsRope(tilex, tiley) && Main.tile[tilex, tiley - 1] != null)
				{
					_ = Main.tile[tilex, tiley - 1].TileType;
					int y = (tiley + tilex) % 3 * 18;
					Texture2D tileDrawTexture = GetTileDrawTexture(Main.tile[tilex, tiley - 1], tilex, tiley);
					if (tileDrawTexture != null)
						Main.spriteBatch.Draw(tileDrawTexture, new Vector2(tilex * 16 - (int)screenposition.X, tiley * 16 - (int)screenposition.Y) + screenoffset, new Rectangle(90, y, 16, 16), drawData.tileLight, 0f, default(Vector2), 1f, drawData.tileSpriteEffect, 0f);
					
				}
			});
		};
	}

    private void Player_FindPulley(On.Terraria.Player.orig_FindPulley orig, Player self)
    {
		if (self.portableStoolInfo.IsInUse || (!self.controlUp && !self.controlDown))
			return;

		int num = (int)(self.position.X + (float)(self.width / 2)) / 16;
		int num2 = (int)(self.position.Y - 8f) / 16;
		if (!IsRope(num, num2))
			return;

		float num3 = self.position.Y;
		//i sure hope this isn't important!
		/*if (Main.tile[num, num2 - 1] == null)
			Main.tile[num, num2 - 1] = new Tile();

		if (Main.tile[num, num2 + 1] == null)
			Main.tile[num, num2 + 1] = new Tile();*/

		if ((!Main.tile[num, num2 - 1].HasTile || !Main.tileRope[Main.tile[num, num2 - 1].TileType]) && (!Main.tile[num, num2 + 1].HasTile || !Main.tileRope[Main.tile[num, num2 + 1].TileType]))
			num3 = num2 * 16 + 22;

		float num4 = num * 16 + 8 - self.width / 2 + 6 * self.direction;
		int num5 = num * 16 + 8 - self.width / 2 + 6;
		int num6 = num * 16 + 8 - self.width / 2;
		int num7 = num * 16 + 8 - self.width / 2 + -6;
		int num8 = 1;
		float num9 = Math.Abs(self.position.X - (float)num5);
		if (Math.Abs(self.position.X - (float)num6) < num9)
		{
			num8 = 2;
			num9 = Math.Abs(self.position.X - (float)num6);
		}

		if (Math.Abs(self.position.X - (float)num7) < num9)
		{
			num8 = 3;
			num9 = Math.Abs(self.position.X - (float)num7);
		}

		if (num8 == 1)
		{
			num4 = num5;
			self.pulleyDir = 2;
			self.direction = 1;
		}

		if (num8 == 2)
		{
			num4 = num6;
			self.pulleyDir = 1;
		}

		if (num8 == 3)
		{
			num4 = num7;
			self.pulleyDir = 2;
			self.direction = -1;
		}

		if (!Collision.SolidCollision(new Vector2(num4, self.position.Y), self.width, self.height))
		{
			if (self.whoAmI == Main.myPlayer)
				Main.cameraX = Main.cameraX + self.position.X - num4;

			self.pulley = true;
			self.position.X = num4;
			self.gfxOffY = self.position.Y - num3;
			self.stepSpeed = 2.5f;
			self.position.Y = num3;
			self.velocity.X = 0f;
			return;
		}

		num4 = num5;
		self.pulleyDir = 2;
		self.direction = 1;
		if (!Collision.SolidCollision(new Vector2(num4, self.position.Y), self.width, self.height))
		{
			if (self.whoAmI == Main.myPlayer)
				Main.cameraX = Main.cameraX + self.position.X - num4;

			self.pulley = true;
			self.position.X = num4;
			self.gfxOffY = self.position.Y - num3;
			self.stepSpeed = 2.5f;
			self.position.Y = num3;
			self.velocity.X = 0f;
			return;
		}

		num4 = num7;
		self.pulleyDir = 2;
		self.direction = -1;
		if (!Collision.SolidCollision(new Vector2(num4, self.position.Y), self.width, self.height))
		{
			if (self.whoAmI == Main.myPlayer)
				Main.cameraX = Main.cameraX + self.position.X - num4;

			self.pulley = true;
			self.position.X = num4;
			self.gfxOffY = self.position.Y - num3;
			self.stepSpeed = 2.5f;
			self.position.Y = num3;
			self.velocity.X = 0f;
		}
	}
}