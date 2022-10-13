using System;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace LizSoundPack.Core.Time
{
	public sealed class TimeSystem : ModSystem
	{
		public const float FullDayLength = (float)(Main.nightLength + Main.dayLength);
		public const float DayLength = 54000f;
		public const float NightLength = 32400f;
		public const int LogicFramerate = 60;
		public const float LogicDeltaTime = 1f / LogicFramerate;

		public static DateTime Date { get; } = DateTime.Now;
		public static DateTime LastLoadDate { get; private set; }
		public static Stopwatch? GlobalStopwatch { get; private set; }
		public static ulong UpdateCount { get; private set; }
		public static double GlobalTime { get; private set; }

		public static float RealTime => (float)(Main.time + (Main.dayTime ? 0d : Main.dayLength));

		public override void Load()
		{
			LastLoadDate = DateTime.Now;

			GlobalStopwatch = Stopwatch.StartNew();
		}

		public override void Unload()
		{
			GlobalStopwatch?.Stop();

			GlobalStopwatch = null;
		}

		public override void PostUpdateEverything()
		{
			UpdateCount++;
			GlobalTime += LogicDeltaTime;
		}
	}
}
