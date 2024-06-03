using LibMatrix.EventTypes.Spec;
using LibMatrix.Utilities.Bot.Services;

namespace Jenny.Handlers;

public static class InviteHandler {
    public static async Task HandleAsync(InviteHandlerHostedService.InviteEventArgs invite) {
        var room = invite.Homeserver.GetRoom(invite.RoomId);
        await room.JoinAsync();
        await room.SendMessageEventAsync(new RoomMessageEventContent("m.notice", "Hello! I'm Jenny!"));
    }
}