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
		public static ParticleEntity[] particleEntities = new ParticleEntity[2001];
		private static int particleIndex;

		public override void Load()
		{
			On.Terraria.Main.DrawProjectiles += (orig, self) => {
				orig(self);

				DrawEntities();
			};

			for(int i = 0; i < particleEntities.Length-1; i++)
            {
				particleEntities[i] = new ParticleEntity();
            }
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
			/*
			preinitializer?.Invoke(instance);

			instance.Init();

			if (!entitiesByType.TryGetValue(typeof(T), out var list))
			{
				entitiesByType[typeof(T)] = list = new LinkedList<ParticleEntity>();
			}

			list.AddLast(instance);*/
			while (!particleEntities[particleIndex].Destroyed)
				particleIndex++;
			preinitializer?.Invoke(instance);
			particleEntities[particleIndex] = instance;
			particleEntities[particleIndex].Destroyed = false;
			particleEntities[particleIndex].Init();

			if (particleIndex >= entitiesByType.Count)
				particleIndex = 0;

			return instance;
		}

		private static void UpdateEntities()
		{
			if (!Main.dedServ && Main.gamePaused)
			{
				return;
			}

			for (int i = 0; i < particleEntities.Length - 1; i++)
			{
				if (particleEntities[i].Destroyed)
				{
					particleEntities[i] = new ParticleEntity();
					particleEntities[i].Destroyed = true;
				}
				else
				{
					particleEntities[i].Update();
				}
			}
		}

		private static void DrawEntities()
		{
			var sb = Main.spriteBatch;

			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

			for (int i = 0; i < particleEntities.Length - 1; i++)
			{

				if (!particleEntities[i].Destroyed)
				{
					if (!particleEntities[i].additive)
						particleEntities[i].Draw(sb);
				}
			}

			sb.End();

			sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

			for (int i = 0; i < particleEntities.Length - 1; i++)
			{
				if (!particleEntities[i].Destroyed)
				{

					if (particleEntities[i].additive)
					{
						
						particleEntities[i].Draw(sb);
					}
				}
			}

			sb.End();


		}
	}
}
