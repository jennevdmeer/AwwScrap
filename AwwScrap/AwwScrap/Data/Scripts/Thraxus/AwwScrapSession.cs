using System.Collections.Generic;
using AwwScrap.Helpers;
using AwwScrap.Utilities;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace AwwScrap
{
	[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
	// ReSharper disable once ClassNeverInstantiated.Global
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
		}

		private static Dictionary<MyStringHash, string> ComponentDictionary = new Dictionary<MyStringHash, string>
		{
			{ MyStringHash.GetOrCompute("BulletproofGlass"), "BulletproofGlassScrap" },
			{ MyStringHash.GetOrCompute("Computer"), "ComputerScrap" },
			{ MyStringHash.GetOrCompute("Construction"), "ConstructionScrap" },
			{ MyStringHash.GetOrCompute("Detector"), "DetectorScrap" },
			{ MyStringHash.GetOrCompute("Display"), "DisplayScrap" },
			{ MyStringHash.GetOrCompute("Explosives"), "ExplosivesScrap" },
			{ MyStringHash.GetOrCompute("Girder"), "GirderScrap" },
			{ MyStringHash.GetOrCompute("GravityGenerator"), "GravityGeneratorScrap" },
			{ MyStringHash.GetOrCompute("InteriorPlate"), "InteriorPlateScrap" },
			{ MyStringHash.GetOrCompute("LargeTube"), "LargeTubeScrap" },
			{ MyStringHash.GetOrCompute("Medical"), "MedicalScrap" },
			{ MyStringHash.GetOrCompute("MetalGrid"), "MetalGridScrap" },
			{ MyStringHash.GetOrCompute("Motor"), "MotorScrap" },
			{ MyStringHash.GetOrCompute("PowerCell"), "PowerCellScrap" },
			{ MyStringHash.GetOrCompute("RadioCommunication"), "RadioCommunicationScrap" },
			{ MyStringHash.GetOrCompute("Reactor"), "ReactorScrap" },
			{ MyStringHash.GetOrCompute("SmallTube"), "SmallTubeScrap" },
			{ MyStringHash.GetOrCompute("SolarCell"), "SolarCellScrap" },
			{ MyStringHash.GetOrCompute("SteelPlate"), "SteelPlateScrap" },
			{ MyStringHash.GetOrCompute("Superconductor"), "SuperconductorScrap" },
			{ MyStringHash.GetOrCompute("Thrust"), "ThrustScrap" }
		};

		private static void ScrubCubes()
		{
			MyPhysicalItemDefinition scrapDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Scrap"));
			foreach (MyDefinitionBase myDefinitionBase in MyDefinitionManager.Static.GetAllDefinitions())
			{
				MyCubeBlockDefinition myCubeBlockDefinition = myDefinitionBase as MyCubeBlockDefinition;
				if (myCubeBlockDefinition?.Components == null) continue;

				//GeneralLog.WriteToLog("ScrubCubes",$"{myCubeBlockDefinition.Id}"); 

				foreach (MyCubeBlockDefinition.Component component in myCubeBlockDefinition.Components)
				{
					GeneralLog.WriteToLog("ScrubCubes", $"{component.Definition.Id.SubtypeId}");

					if (!component.Definition.Public)
						continue;
					
					string subtypeName;
					if (ComponentDictionary.TryGetValue(component.Definition.Id.SubtypeId, out subtypeName))
					{
						component.DeconstructItem = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), subtypeName));
						continue;
					}

					component.DeconstructItem = scrapDef; 
					//if (component.Definition.Id.SubtypeId == MyStringHash.GetOrCompute("SmallTube"))
					//{
					//	GeneralLog.WriteToLog("ScrubCubes", $"Replaced Small Steel Tube");
					//	component.DeconstructItem = smallSteelTubeScrap;
					//	continue;
					//}
					//if (component.Definition.Id.SubtypeId == MyStringHash.GetOrCompute("Thrust"))
					//{
					//	GeneralLog.WriteToLog("ScrubCubes", $"Replaced Thrusters");
					//	component.DeconstructItem = thrusterScrap;
					//	continue;
					//}
					//component.DeconstructItem = scrapDef;
				}
			}
		}
	}
}