using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace LizSoundPack.Core.Effects
{
	public class ParticleEntitySystem : ModSystem
	{
		private static Dictionary<Type, LinkedList<ParticleEntity>> entitiesByType = new();

		public override void Load()
		{
			On.Terraria.Main.DrawProjectiles += (orig, self) => {
				orig(self);

				DrawEntities();
			};
		}

		public override void Unload()
		{
			entitiesByType?.Clear();
		}

        public override void PreUpdateEntities() => UpdateEntities();

        public static IEnumerable<ParticleEntity> EnumerateEntities()
		{
			foreach (var entities in entitiesByType.Values)
			{
				var node = entities.First;

				while (node != null)
				{
					var entity = node.Value;

					if (entity.Destroyed)
					{
						var next = node.Next;

						entities.Remove(node);

						node = next;
					}
					else
					{
						yield return entity;

						node = node.Next;
					}
				}
			}
		}

		internal static T InstantiateEntity<T>(Action<T>? preinitializer = null) where T : ParticleEntity
		{
			T instance = Activator.CreateInstance<T>();

			preinitializer?.Invoke(instance);

			instance.Init();

			if (!entitiesByType.TryGetValue(typeof(T), out var list))
			{
				entitiesByType[typeof(T)] = list = new LinkedList<ParticleEntity>();
			}

			list.AddLast(instance);

			return instance;
		}

		private static void UpdateEntities()
		{
			if (!Main.dedServ && Main.gamePaused)
			{
				return;
			}
			try
			{
				foreach (var entity in EnumerateEntities())
				{
					entity.Update();
				}
			} catch(System.InvalidOperationException)
            {
				//just shit myself i guess idk
            }
		}

		private static void DrawEntities()
		{
			var sb = Main.spriteBatch;

			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

			foreach (var entity in EnumerateEntities())
			{
				if (!entity.additive)
					entity.Draw(sb);
			}

			sb.End();

			sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

			foreach (var entity in EnumerateEntities())
			{
				if (entity.additive)
				entity.Draw(sb);
			}

			sb.End();

			
		}
	}
}
