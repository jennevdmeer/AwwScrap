using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwwScrap.Helpers;
using AwwScrap.Utilities;
using Sandbox.Game.Entities;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace AwwScrap
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AwwScrapSession : MySessionComponentBase
    {
        public static bool LogSetupComplete;
        public static Log ProfilingLog;
        public static Log DebugLog;
        public static Log GeneralLog;

        /// <inheritdoc />
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
            DebugLog = new Log(Constants.DebugLogName);
            ProfilingLog = new Log(Constants.ProfilingLogName);
            GeneralLog = new Log(Constants.GeneralLogName);
            LogSetupComplete = true;
        }

        private static void CloseLogs()
        {
            if (Constants.DebugMode) DebugLog.Close();
            if (Constants.EnableProfilingLog) ProfilingLog.Close();
            if (Constants.EnableGeneralLog) GeneralLog.Close();
        }


        void Initialize()
        {
            InitLogs();
            MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(0, DamageHandler);
            foreach (MyEntityList.MyEntityListInfoItem myEntityListInfoItem in MyEntityList.GetEntityList(MyEntityList.MyEntityTypeEnum.Grids))
            {
                IMyEntity newEntity = GetEntityById(myEntityListInfoItem.EntityId);
                //newEntity.d
            }
            MyAPIGateway.Entities.OnEntityAdd += OnEntityAdd;
        }

        /// <inheritdoc />
        public override MyObjectBuilder_SessionComponent GetObjectBuilder()
        {
            return base.GetObjectBuilder();
        }

        private void OnEntityAdd(IMyEntity obj)
        {
            throw new NotImplementedException();
        }

        static void DamageHandler(object damagedObject, ref MyDamageInformation damage)
        {
            IMyEntity attackerEntity = MyAPIGateway.Entities.GetEntityById(damage.AttackerId);
            IMyEngineerToolBase welder = new MyAngleGrinder();
            //if (attackerEntity == welder)

        }

        static IMyEntity GetEntityById(long entityId)
        {
            return MyAPIGateway.Entities.GetEntityById(entityId);
        }

    }
}
