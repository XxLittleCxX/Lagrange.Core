using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event.Protocol;
using Lagrange.Core.Internal.Event.Protocol.Action;
using Lagrange.Core.Internal.Packets;
using Lagrange.Core.Internal.Packets.Service.Oidb;
using Lagrange.Core.Internal.Packets.Service.Oidb.Request;
using Lagrange.Core.Internal.Packets.Service.Oidb.Response;
using Lagrange.Core.Internal.Service.Abstraction;
using Lagrange.Core.Utility.Binary;
using ProtoBuf;

namespace Lagrange.Core.Internal.Service.Action;

[EventSubscribe(typeof(RenameMemberEvent))]
[Service("OidbSvcTrpcTcp.0x8fc_3")]
internal class RenameMemberService : BaseService<RenameMemberEvent>
{
    protected override bool Build(RenameMemberEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new OidbSvcTrpcTcpBase<OidbSvcTrpcTcp0x8FC_3>(new OidbSvcTrpcTcp0x8FC_3
        {
            GroupUin = input.GroupUin,
            Body = new OidbSvcTrpcTcp0x8FC_3Body
            {
                TargetUid = input.TargetUid,
                TargetName = input.TargetName
            }
        });

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, packet);
        output = new BinaryPacket(stream);

        extraPackets = null;
        return true;
    }

    protected override bool Parse(SsoPacket input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, 
        out RenameMemberEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var payload = input.Payload.ReadBytes(BinaryPacket.Prefix.Uint32 | BinaryPacket.Prefix.WithPrefix);
        var packet = Serializer.Deserialize<OidbSvcTrpcTcpResponse<OidbSvcTrpcTcp0x8FC_3Response>>(payload.AsSpan());
        
        output = RenameMemberEvent.Result((int)packet.ErrorCode);
        extraEvents = null;
        return true;
    }
}