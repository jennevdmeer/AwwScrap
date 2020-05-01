using System.Linq;
using AwwScrap.Helpers;
using AwwScrap.Utilities;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using AwwScrap.Support;

namespace AwwScrap
{
	[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
	public class AwwScrapSession : MySessionComponentBase
	{
		public static bool LogSetupComplete;
		public static Log GeneralLog;
		
		public override void BeforeStart()
		{
			base.BeforeStart();
			Initialize();
		}

		protected override void UnloadData()
		{
			CloseLogs();
		}

		private static void InitLogs()
		{
			if (Constants.EnableGeneralLog) GeneralLog = new Log(Constants.GeneralLogName);
			LogSetupComplete = true;
		}

		private static void CloseLogs()
		{
			if (Constants.EnableGeneralLog) GeneralLog.Close();
		}


		private static void Initialize()
		{
			InitLogs();
			MyAPIGateway.Parallel.StartBackground(ScrubCubes);
			MyAPIGateway.Parallel.StartBackground(SetEfficiency);
			MyAPIGateway.Parallel.StartBackground(SetAttributes);
		}



		private static void ScrubCubes()
		{
			MyPhysicalItemDefinition scrapDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Scrap"));
			
			foreach (MyCubeBlockDefinition myCubeBlockDefinition in MyDefinitionManager.Static.GetAllDefinitions().OfType<MyCubeBlockDefinition>().Where(myCubeBlockDefinition => myCubeBlockDefinition?.Components != null))
			{
				foreach (MyCubeBlockDefinition.Component component in myCubeBlockDefinition.Components)
				{
					//GeneralLog.WriteToLog("ScrubCubes", $"{component.Definition.Id.SubtypeId}");

					if (!component.Definition.Public)
						continue;

					string subtypeName;
					if (Statics.ComponentDictionary.TryGetValue(component.Definition.Id.SubtypeId, out subtypeName))
					{
						component.DeconstructItem = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), subtypeName));
						continue;
					}

					if (Statics.SkipTieredTech)
						if (component.Definition.Id.SubtypeId.ToString() == "Tech2x" || component.Definition.Id.SubtypeId.ToString() == "Tech4x" || component.Definition.Id.SubtypeId.ToString() == "Tech8x")
							continue;
					component.DeconstructItem = scrapDef;
				}
			}
		}

		private static void SetAttributes()
		{
			foreach (MyPhysicalItemDefinition item in MyDefinitionManager.Static.GetPhysicalItemDefinitions())
			{
				ScrapAttributes scrap;
				if (!Statics.ScrapAttributesDictionary.TryGetValue(item.Id.SubtypeId, out scrap))
					continue;
				//GeneralLog.WriteToLog("SetAttributes", $"Attributes for {item.Id.SubtypeId} are currently Mass: {item.Mass} - Volume: {item.Volume}");
				item.Mass = scrap.Mass;
				item.Volume = scrap.Volume / 1000;
				//GeneralLog.WriteToLog("SetAttributes", $"Attributes for {item.Id.SubtypeId} are now Mass: {item.Mass} - Volume: {item.Volume}");
			}
		}

		private static void SetEfficiency()
		{
			// This loop accounts for World Settings for the Assembler Efficiency Modifier (x1, x3, x10)
			foreach (MyBlueprintDefinitionBase myBlueprintDefinitionBase in Statics.AwwScrapSubTypeIds.Select(
				subtype => MyDefinitionManager.Static.GetBlueprintDefinition(
					new MyDefinitionId(typeof(MyObjectBuilder_BlueprintDefinition), subtype))))
			{
				for (int index = 0; index < myBlueprintDefinitionBase.Results.Length; index++)
				{   // MyFixedPoint can't do /= operations, so have to do a work around
					float f = (float)myBlueprintDefinitionBase.Results[index].Amount;
					f /= MyAPIGateway.Session.SessionSettings.AssemblerEfficiencyMultiplier;
					myBlueprintDefinitionBase.Results[index].Amount = (MyFixedPoint)f;
				}
			}
		}
	}
}