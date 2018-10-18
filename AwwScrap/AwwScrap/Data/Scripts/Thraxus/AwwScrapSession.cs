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
            if (Constants.DebugMode) DebugLog = new Log(Constants.DebugLogName);
            if (Constants.EnableProfilingLog) ProfilingLog = new Log(Constants.ProfilingLogName);
            if (Constants.EnableGeneralLog) GeneralLog = new Log(Constants.GeneralLogName);
            LogSetupComplete = true;
        }

        private static void CloseLogs()
        {
            if (Constants.DebugMode) DebugLog.Close();
            if (Constants.EnableProfilingLog) ProfilingLog.Close();
            if (Constants.EnableGeneralLog) GeneralLog.Close();
        }


        private void Initialize()
        {
            InitLogs();
            MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(0, DamageHandler);
            //foreach (MyEntityList.MyEntityListInfoItem myEntityListInfoItem in MyEntityList.GetEntityList(MyEntityList.MyEntityTypeEnum.Grids))
            //{
            //    IMyEntity newEntity = GetEntityById(myEntityListInfoItem.EntityId);
            //    //newEntity.d
            //}
            MyAPIGateway.Entities.OnEntityAdd += OnEntityAdd;
        }

        /// <inheritdoc />
        public override MyObjectBuilder_SessionComponent GetObjectBuilder()
        {
            return base.GetObjectBuilder();
        }

        private void OnEntityAdd(IMyEntity obj)
        {
            
        }

        private static void DamageHandler(object damagedObject, ref MyDamageInformation damage)
        {
            IMyEntity attackerEntity = MyAPIGateway.Entities.GetEntityById(damage.AttackerId);
            IMyAngleGrinder grinder = attackerEntity as IMyAngleGrinder;
            if (grinder != null)
            {
                grinder.GameLogic.BeforeRemovedFromContainer += BeforeRemovedFromContainer;
                IMySlimBlock slimBlock = damagedObject as IMySlimBlock;
                if (slimBlock != null)
                {
                    GeneralLog.WriteToLog("DamageHandler", $"damaged Object is:\t{slimBlock.GetType()}");
                }
            }
        }

        private static void BeforeRemovedFromContainer(MyEntityComponentBase obj)
        {
            GeneralLog.WriteToLog("BeforeRemovedFromContainer", $"{obj.GetType()}");
        }

        private static IMyEntity GetEntityById(long entityId)
        {
            return MyAPIGateway.Entities.GetEntityById(entityId);
        }

    }
}

/* Notes
 *  Thraxus Today at 3:54 AM
        so, here is the small of my idea - tell me if you think i can get it to work -- i want to make it so a grinder only ever returns scrap, no more components, making it so that the only way to get components if from an assembler or as loot in a container
        i'm trying to force an economic state in the game where one isn't normally enforcable
        but this must be from all grind sources to keep a level playing field
        i know how to modify the cubeblock for a component to only return scrap, but that isn't good enough here since it has to automatically account for any mod, thus, any cubeblock whether custom or vanilla
        in theory i could go into the OB on init and modify all cubeblocks and their components, but i am not sure that will propagate to existing entities
    Phoera Today at 3:59 AM
        i think you just can alter all definitions in runtime
    Thraxus Today at 4:00 AM
        so hook the grind event and mod the def for the block being ground?
    Phoera Today at 4:00 AM
        no, just do that before
        and once
        adjust all blocks, so they will return scrap
        that will take all bllocks, modded or vanilla, any new, etc
    Thraxus Today at 4:01 AM
        so on init, mod them there, and that should just carry over to the block already on an entity since, in theory, the game logic has to check what something is made of before returning anything?(edited)
    Phoera Today at 4:01 AM
        no, you must not touch blocks, most blocks use definition, without copying data all around(edited)
    Thraxus Today at 4:02 AM
        ahh, ok.  will give it a go, thanks! :smile:
 *
 *
 *
 *
 *
 *
 *
 */
