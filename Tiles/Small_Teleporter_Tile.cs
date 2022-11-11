using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ObjectData;
using Terraria.Enums;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent.ObjectInteractions;

namespace LizOverhaul.Tiles
{
	public class Small_Teleporter_Tile : ModTile
	{
		public static readonly SoundStyle TeleportSound = new SoundStyle("LizOverhaul/sounds/teleportStairs") { Volume = 0.7f };
		public override void SetStaticDefaults() 
		{
			Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            //Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
			//Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			//TileObjectData.newTile.CoordinateHeights = new int[] { 8};
			TileObjectData.newTile.CoordinateWidth = 16;
			//TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.AnchorTop = default(AnchorData);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
            TileObjectData.newTile.AnchorLeft = default(AnchorData);
            TileObjectData.newTile.AnchorRight = default(AnchorData);
            TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Small Teleporter");
            AddMapEntry(new Color(228, 200, 37), name);
			DustType = 1;
			ItemDrop = ItemType<Items.Small_Teleporter>();
		}
		public override bool CanPlace(int i, int j)
        {
            return true;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
			return true;
        }
		public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = 1;
		}


		public override bool RightClick(int i, int j) 
		{
			Tile tile = Main.tile[i, j];
			Wiring.TripWire(i, j, 1, 1); //(int left, int top, int width, int height)
			return true;
		}
		public override void MouseOver(int i, int j) 
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ItemType<Items.Small_Teleporter>();
		}
		public override void HitWire(int i, int j) 
		{
			Wiring.SkipWire(i, j);
			for (int p = 0; p < 255; p++)
			{
				if (Main.player[p].active && !Main.player[p].dead)
				{
					Vector2 vector2;
					if(!(Main.tile[i, j + 1].HasTile))
					{
						vector2 = new Vector2(i * 16, (j * 16) - (1 * 16));
					}
					if(!(Main.tile[i, j + 1].HasTile && Main.tile[i, j + 2].HasTile))
					{
						vector2 = new Vector2(i * 16, (j * 16));
					}
					else
					{
						vector2 = new Vector2(i * 16, (j * 16) - (2 * 16));
					}
					
					if (Main.netMode == 2)
					{
						RemoteClient.CheckSection(Main.player[p].whoAmI, vector2);
					}
					var player = Main.player[Main.player[p].whoAmI];
					player.Teleport(vector2, 69);
					Main.BlackFadeIn = 512;
					Lighting.Clear();
					Main.screenLastPosition = Main.screenPosition;
					Main.screenPosition.X = player.position.X + (float)(player.width / 2) - (float)(Main.screenWidth / 2);
					Main.screenPosition.Y = player.position.Y + (float)(player.height / 2) - (float)(Main.screenHeight / 2);
					Main.instantBGTransitionCounter = 10;
					SoundEngine.PlaySound(TeleportSound);
					player.ForceUpdateBiomes();
					if (Main.netMode == 2)
					{
						NetMessage.SendData(65, -1, -1, null, 0, Main.player[p].whoAmI, vector2.X, vector2.Y);
					}
				}
			}
			/*Player player = Main.player[Main.myPlayer];
			if(!(Main.tile[i, j + 1].active()))
			{
				player.Teleport(new Vector2(i * 16, (j * 16) - (1 * 16)), 69);
			}
			if(!(Main.tile[i, j + 1].active() && Main.tile[i, j + 2].active()))
			{
				player.Teleport(new Vector2(i * 16, (j * 16)), 69);
			}
			else
			{
				player.Teleport(new Vector2(i * 16, (j * 16) - (2 * 16)), 69);
			}*/
			
			//for testing
			//Projectile.NewProjectile(i * 16, j * 16, 0, 0, mod.ProjectileType("Small_Amberfire"), 0, 0, Main.myPlayer); //4 = damage
			//int teleportTile = ModLoader.GetMod("ImprovedTeleporters").TileType("Small_Teleporter_Tile");
			//if (Main.tile[(int)(player.Center.X / 16), ((int)(player.Center.Y / 16))].type == teleportTile
			/*|| Main.tile[(int)(player.Center.X / 16), ((int)(player.Center.Y / 16) - 16)].type == teleportTile
			|| Main.tile[((int)(player.Center.X / 16) + 16), (int)(player.Center.Y / 16)].type == teleportTile
			|| Main.tile[((int)(player.Center.X / 16) - 16), (int)(player.Center.Y / 16)].type == teleportTile)*/
			//{
		}
	}
}