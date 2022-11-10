using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LizSoundPack.Items
{
    public class Small_Teleporter : ModItem
    {
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Teleporter");
			Tooltip.SetDefault("Right-click to use");
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = 1;
            Item.consumable = true;
            Item.rare = 1;
            Item.value = Item.buyPrice(0, 2, 50, 0);
			Item.mech = true;
            Item.createTile = ModContent.TileType<Tiles.Small_Teleporter_Tile>();
        }
		/*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(2);
            //recipe.AddTile(0);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
	}
}