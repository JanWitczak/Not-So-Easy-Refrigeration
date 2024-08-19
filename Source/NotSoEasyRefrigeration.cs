using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;

namespace NotSoEasyRefrigeration
{
	class NotSoEasyRefrigerationMod : Mod
	{
		public static NotSoEasyRefrigerationSettings Settings;
		public NotSoEasyRefrigerationMod(ModContentPack content) : base(content)
		{
			Settings = GetSettings<NotSoEasyRefrigerationSettings>();
		}

		public override string SettingsCategory()
		{
			return "Not So Easy Refrigeration";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);
			Settings.RefrigerationCurveBase = (float)Math.Round(listing.SliderLabeled($"Refrigeration Curve Base\t{Settings.RefrigerationCurveBase}", Settings.RefrigerationCurveBase, 1.01f, 2.0f, tooltip: "The base of the exponential function dictating spoilage rate, low value will make the relationship more linear, high value will make changes between low temparatures have lower impact, while changes between higher temepratures have greater impact."), 2);
			Settings.RefrigerationMaxTemperature = (float)Math.Round(listing.SliderLabeled($"Refrigeration temperature \t{Settings.RefrigerationMaxTemperature.ToStringTemperature()}", Settings.RefrigerationMaxTemperature, 0.0f, 20.0f, tooltip: "Maximal temperature of refrigeration."), 0);
			Settings.RefrigerationMinRotRate = (float)Math.Round(listing.SliderLabeled($"Minimal spoilage factor\t{Settings.RefrigerationMinRotRate}", Settings.RefrigerationMinRotRate, 0.001f, 0.5f), 3);
			Settings.FreezingTemperature = (float)Math.Round(listing.SliderLabeled($"Freezing Temperature\t{Settings.FreezingTemperature.ToStringTemperature()}", Settings.FreezingTemperature, -270.0f, 0.0f, tooltip: "Temperature below which spoilage will not progress."), 0);
			listing.End();
		}
	}
	class NotSoEasyRefrigerationSettings : ModSettings
	{
		private float _RefrigerationMaxTemperature = 10.0f;
		public float RefrigerationMaxTemperature
		{
			get { return _RefrigerationMaxTemperature; }
			set
			{
				if (value != _RefrigerationMaxTemperature)
				{
					_RefrigerationMaxTemperature = value;
					RotRatePatch.CasheRotRate();
				}
			}
		}
		private float _RefrigerationMinRotRate = 0.1f;
		public float RefrigerationMinRotRate
		{
			get { return _RefrigerationMinRotRate; }
			set
			{
				if (value != _RefrigerationMinRotRate)
				{
					_RefrigerationMinRotRate = value;
					RotRatePatch.CasheRotRate();
				}
			}
		}
		private float _RefrigerationCurveBase = 1.16f;
		public float RefrigerationCurveBase
		{
			get { return _RefrigerationCurveBase; }
			set
			{
				if (value != _RefrigerationCurveBase)
				{
					_RefrigerationCurveBase = value;
					RotRatePatch.CasheRotRate();
				}
			}
		}
		private float _FreezingTemperature = -100.0f;
		public float FreezingTemperature
		{
			get { return _FreezingTemperature; }
			set
			{
				if (value != _FreezingTemperature)
				{
					_FreezingTemperature = value;
					RotRatePatch.CasheRotRate();
				}
			}
		}
		public override void ExposeData()
		{
			Scribe_Values.Look(ref _RefrigerationMaxTemperature, "RefrigerationMaxTemperature", defaultValue: 10.0f);
			Scribe_Values.Look(ref _RefrigerationMinRotRate, "RefrigerationMinRotRate", defaultValue: 0.1f);
			Scribe_Values.Look(ref _RefrigerationCurveBase, "RefrigerationCurveBase", defaultValue: 1.16f);
			Scribe_Values.Look(ref _FreezingTemperature, "FreezingTemperature", defaultValue: -100.0f);
		}
	}
}