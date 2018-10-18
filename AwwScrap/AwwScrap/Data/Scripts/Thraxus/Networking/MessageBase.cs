using ProtoBuf;
using Sandbox.ModAPI;

// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable ExplicitCallerInfoArgument

namespace AwwScrap.Networking
{
	//[ProtoInclude(10, typeof(FactionsChangeMessage))]  Leaving this for an example os usage for custom handlers
	[ProtoContract]
	public abstract class MessageBase
	{
		[ProtoMember(1)] protected readonly ulong SenderId;

		protected MessageBase()
		{
			SenderId = MyAPIGateway.Multiplayer.MyId;
		}

		public abstract void HandleServer();

		public abstract void HandleClient();
	}
}
