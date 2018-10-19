using AwwScrap.Helpers;
using AwwScrap.Utilities;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;

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
            foreach (MyDefinitionBase myDefinitionBase in MyDefinitionManager.Static.GetAllDefinitions())
            {
                MyCubeBlockDefinition myCubeBlockDefinition = myDefinitionBase as MyCubeBlockDefinition;
                if (myCubeBlockDefinition?.Components == null) continue;
                GeneralLog.WriteToLog("ScrubCubes",$"{myCubeBlockDefinition.Id}"); 
                foreach (MyCubeBlockDefinition.Component component in myCubeBlockDefinition.Components)
                {
                    component.DeconstructItem = scrapDef;
                }
            }
        }
    }
}