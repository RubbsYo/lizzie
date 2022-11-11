﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace LizOverhaul.Core.Effects
{
	// There is nothing that exists in vanilla that does what I want to do. I am sorry to Mirsario and shamelessly stealing his work
	public class ParticleEntity : ILoadable
	{
		public bool Destroyed = true;

		public void Destroy(bool allowEffects = false)
		{
			if (!Destroyed)
			{
				OnDestroyed(allowEffects);

				Destroyed = true;
			}
		}

		public bool additive = true;

		public bool back = false;

		public virtual void Load(Mod mod) { 
		}

		public virtual void Unload() { }

		public virtual void Init() { }

		public virtual void Update() { }

		public virtual void Draw(SpriteBatch sb) { }

		public virtual void OnDestroyed(bool allowEffects) { }

		public static T Instantiate<T>(Action<T>? preinitializer = null) where T : ParticleEntity
			=> ParticleEntitySystem.InstantiateEntity(preinitializer);
	}
}
