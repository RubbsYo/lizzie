using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;

namespace LizSoundPack.Common.TextureColors;

/// <summary> This system provides information about average colors of texturse. </summary>
[Autoload(Side = ModSide.Client)]
public class TextureColorSystem : ModSystem
{
	private readonly Dictionary<Asset<Texture2D>, Color> cache = new(); // Non-static to not trigger anything on servers.

	public override void Unload()
	{
		cache?.Clear();
	}

	/// <summary> Returns an average color from a texture's pixel data. </summary>
	public static Color GetAverageColor(Asset<Texture2D> texture)
	{
		var instance = ModContent.GetInstance<TextureColorSystem>() ?? throw new InvalidOperationException($"{nameof(TextureColorSystem)} has not been loaded.");

		if (!instance.cache.TryGetValue(texture, out var color))
		{
			instance.cache[texture] = color = CalculateAverageColor(texture.Value);
		}

		return color;
	}

	public static Color GetBrightestColor(Asset<Texture2D> texture)
	{
		var instance = ModContent.GetInstance<TextureColorSystem>() ?? throw new InvalidOperationException($"{nameof(TextureColorSystem)} has not been loaded.");

		if (!instance.cache.TryGetValue(texture, out var color))
		{
			instance.cache[texture] = color = CalculateBrightestColor(texture.Value);
		}

		return color;
	}

	/// <summary> Calculates an average color from a texture's pixel data. </summary>
	private static Color CalculateAverageColor(Texture2D tex, byte alphaTest = 64, Rectangle rect = default, HashSet<Color>? excludedColors = null)
	{
		bool hasRect = rect != default;
		bool hasExcludedColors = excludedColors != null;
		int texWidth = tex.Width;

		var data = new Color[tex.Width * tex.Height];

		tex.GetData(data);

		long[] values = new long[3];
		long numFittingPixels = 0;

		for (int i = 0; i < data.Length; i++)
		{
			if (hasRect)
			{
				int y = i / texWidth;
				int x = i - (y * texWidth);

				if (x < rect.X || y < rect.Y || x >= rect.X + rect.Width || y >= rect.Y + rect.Height)
				{
					continue;
				}
			}

			var col = data[i];

			if (col.A >= 0)
			{
				if (hasExcludedColors && excludedColors!.Contains(col))
				{
					continue;
				}

				values[0] += col.R;
				values[1] += col.G;
				values[2] += col.B;

				numFittingPixels++;
			}
		}

		Color result;

		if (numFittingPixels == 0)
		{
			result = Color.Transparent;
		}
		else
		{
			result = new Color(
				(byte)(values[0] / numFittingPixels),
				(byte)(values[1] / numFittingPixels),
				(byte)(values[2] / numFittingPixels),
				255
			);
		}

		return result;
	}

	private static Color CalculateBrightestColor(Texture2D tex, byte alphaTest = 64, Rectangle rect = default, HashSet<Color>? excludedColors = null)
	{
		bool hasRect = rect != default;
		bool hasExcludedColors = excludedColors != null;
		int texWidth = tex.Width;

		var data = new Color[tex.Width * tex.Height];

		tex.GetData(data);

		long[] values = new long[3];
		long numFittingPixels = 0;

		for (int i = 0; i < data.Length; i++)
		{
			if (hasRect)
			{
				int y = i / texWidth;
				int x = i - (y * texWidth);

				if (x < rect.X || y < rect.Y || x >= rect.X + rect.Width || y >= rect.Y + rect.Height)
				{
					continue;
				}
			}

			var col = data[i];

			if (col.A >= 0)
			{
				if (hasExcludedColors && excludedColors!.Contains(col))
				{
					continue;
				}
				var lastvalue = Math.Max(values[0], Math.Max(values[1], values[2]));
				var colvalue = Math.Max(col.R , Math.Max(col.G, col.B));
				if ((colvalue > lastvalue) && (col != Color.White) && (col.R+col.B+col.G <= 505))
				{
					values[0] = col.R;
					values[1] = col.G;
					values[2] = col.B;
				}

				numFittingPixels++;
			}
		}

		Color result;

		if (numFittingPixels == 0)
		{
			result = Color.Transparent;
		}
		else
		{
			result = new Color(
				(byte)(values[0]),
				(byte)(values[1]),
				(byte)(values[2]),
				255
			);
		}

		return result;
	}
}
