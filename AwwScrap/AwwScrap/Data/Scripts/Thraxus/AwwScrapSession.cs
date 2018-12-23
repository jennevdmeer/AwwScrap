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

		private static void ScrubCubes()
		{
			MyPhysicalItemDefinition scrapDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Scrap"));
			MyPhysicalItemDefinition smallSteelTubeScrap = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "SmallSteelTubeScrap"));
			MyPhysicalItemDefinition thrusterScrap = MyDefinitionManager.Static.GetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "ThrustScrap"));
			foreach (MyDefinitionBase myDefinitionBase in MyDefinitionManager.Static.GetAllDefinitions())
			{
				MyCubeBlockDefinition myCubeBlockDefinition = myDefinitionBase as MyCubeBlockDefinition;
				if (myCubeBlockDefinition?.Components == null) continue;
				GeneralLog.WriteToLog("ScrubCubes",$"{myCubeBlockDefinition.Id}"); 
				foreach (MyCubeBlockDefinition.Component component in myCubeBlockDefinition.Components)
				{
					GeneralLog.WriteToLog("ScrubCubes", $"{component.Definition.Id.SubtypeId}");
					if (!component.Definition.Public)
						continue;
					if (component.Definition.Context == MyModContext.BaseGame)
					{
						component.DeconstructItem = scrapDef;
						continue;
					}
					if (component.Definition.Id.SubtypeId == MyStringHash.GetOrCompute("SmallTube"))
					{
						GeneralLog.WriteToLog("ScrubCubes", $"Replaced Small Steel Tube");
						component.DeconstructItem = smallSteelTubeScrap;
						continue;
					}
					if (component.Definition.Id.SubtypeId == MyStringHash.GetOrCompute("Thrust"))
					{
						GeneralLog.WriteToLog("ScrubCubes", $"Replaced Thrusters");
						component.DeconstructItem = thrusterScrap;
						continue;
					}
					component.DeconstructItem = scrapDef;
				}
			}
		}
	}
}