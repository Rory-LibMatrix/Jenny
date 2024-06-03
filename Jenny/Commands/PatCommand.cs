using System.Text;
using LibMatrix.EventTypes.Spec.State;
using LibMatrix.Helpers;
using LibMatrix.Utilities.Bot.Interfaces;

namespace Jenny.Commands;

public class PatCommand : ICommand {
    public string Name { get; } = "pat";
    public string[]? Aliases { get; } = [ "patpat", "patpatpat" ];
    public string Description { get; }
    public bool Unlisted { get; } = true;

    public async Task Invoke(CommandContext ctx) {
        int count = 1;
        if (ctx.Args is { Length: 1 })
            int.TryParse(ctx.Args[0], out count);

        var selfName =
            (await ctx.Room.GetStateAsync<RoomMemberEventContent>(RoomMemberEventContent.EventId, ctx.Homeserver.UserId))?.DisplayName
            ?? (await ctx.Homeserver.GetProfileAsync(ctx.Homeserver.UserId)).DisplayName
            ?? ctx.Homeserver.WhoAmI.UserId;
        
        var remoteName =
            ctx.MessageEvent.Sender == null
                ? null
                : (await ctx.Room.GetStateAsync<RoomMemberEventContent>(RoomMemberEventContent.EventId, ctx.MessageEvent.Sender))?.DisplayName
                  ?? (await ctx.Homeserver.GetProfileAsync(ctx.MessageEvent.Sender)).DisplayName
                  ?? ctx.MessageEvent.Sender;

        var msb = new MessageBuilder("m.emote");
        var pat = new StringBuilder();
        var msg = $"snuggles {remoteName}";

        Console.WriteLine(pat.ToString());
        // msb.WithHtmlTag("code", await GenerateSkyboxAroundString(msg, ctx));
        msb.WithBody(msg);
        
        await ctx.Room.SendMessageEventAsync(msb.Build());
        Console.WriteLine(msb.Build().FormattedBody);

    }
    
#region old stuff
    // TODO: implement:
    // var selfName =
    //     (await ctx.Room.GetStateAsync<RoomMemberEventContent>(RoomMemberEventContent.EventId, ctx.Homeserver.UserId))?.DisplayName
    //     ?? (await ctx.Homeserver.GetProfileAsync(ctx.Homeserver.UserId)).DisplayName
    //     ?? ctx.Homeserver.WhoAmI.UserId;
    //
    // var remoteName =
    //     ctx.MessageEvent.Sender == null
    //         ? null
    //         : (await ctx.Room.GetStateAsync<RoomMemberEventContent>(RoomMemberEventContent.EventId, ctx.MessageEvent.Sender))?.DisplayName
    //           ?? (await ctx.Homeserver.GetProfileAsync(ctx.MessageEvent.Sender)).DisplayName
    //           ?? ctx.MessageEvent.Sender;

    // var msb = new MessageBuilder();
    // var pat = new StringBuilder();
    // var msg = $"{selfName} snuggles {remoteName}";

    // Console.WriteLine(pat.ToString());
    // msb.WithHtmlTag("code", await GenerateSkyboxAroundString(msg, ctx));
    // Console.WriteLine(msb.Build().FormattedBody);
//}

    //
    //
    // private class CharacterWeight {
    //     public float Width { get; set; }
    //     public float Density { get; set; }
    // }
    //
    // private Dictionary<char, CharacterWeight> starrySkyCharacters = new Dictionary<char, CharacterWeight> {
    //     { ' ', new CharacterWeight { Width = 0.5f, Density = 0.1f } }, // Space has the lowest density and width
    //     { '.', new CharacterWeight { Width = 0.2f, Density = 0.2f } }, // Dot has low density but small width
    //     { '+', new CharacterWeight { Width = 0.6f, Density = 0.3f } }, // Plus sign has medium density and width
    //     { '*', new CharacterWeight { Width = 0.7f, Density = 0.7f } }, // Asterisk has high density but medium width
    //
    //     { '¨', new CharacterWeight { Width = 0.3f, Density = 0.2f } },
    //     { '˜', new CharacterWeight { Width = 0.4f, Density = 0.3f } },
    //     { 'ˆ', new CharacterWeight { Width = 0.5f, Density = 0.4f } },
    //     { '”', new CharacterWeight { Width = 0.6f, Density = 0.5f } },
    //     { '⍣', new CharacterWeight { Width = 0.8f, Density = 0.7f } },
    //     { '~', new CharacterWeight { Width = 0.9f, Density = 0.8f } },
    //     { '⊹', new CharacterWeight { Width = 1.2f, Density = 1.1f } },
    //     { '٭', new CharacterWeight { Width = 1.3f, Density = 1.2f } },
    //     { '„', new CharacterWeight { Width = 1.4f, Density = 1.3f } },
    //     { '¸', new CharacterWeight { Width = 1.5f, Density = 1.4f } },
    //     { '¤', new CharacterWeight { Width = 1.9f, Density = 1.8f } },
    //     { '✬', new CharacterWeight { Width = 2.1f, Density = 2.0f } },
    //     { '°', new CharacterWeight { Width = 0.6f, Density = 0.35f } },
    //     { '•', new CharacterWeight { Width = 0.6f, Density = 0.4f } },
    //     { '✡', new CharacterWeight { Width = 2.0f, Density = 4.0f } },
    //     { '#', new CharacterWeight { Width = 1.0f, Density = 1.0f } },
    // };
    //
    // private Dictionary<char, CharacterWeight> characterWeights = new Dictionary<char, CharacterWeight> {
    //     { 'a', new() { Density = 1, Width = 1 } }
    // };
    //
    // private async Task<string> GenerateSkyboxAroundString(string str, CommandContext ctx) {
    //     var sb = new StringBuilder();
    //     int scale = 32;
    //     var outerTopBottomBorder = 4;
    //     var outerLeftRightBorder = 2;
    //     var innerTopBottomBorder = 1;
    //     var innerLeftRightBorder = 2;
    //
    //     var innerBorder = 2;
    //
    //     var width = str.Length + outerLeftRightBorder * 2 + innerLeftRightBorder * 2;
    //     var height = outerTopBottomBorder * 2 + innerTopBottomBorder * 2 + 1;
    //     var skybox = new char[height, width];
    //     var bigNoise = GenerateHeightMap(2000, 1000);
    //
    //     // await ctx.Room.SendMessageEventAsync(new RoomMessageEventContent("m.image", "heightmap.png") {
    //     //     Url = await Float2DArrayToMxc(noise, ctx),
    //     //     FileInfo = new() {
    //     //         Width = noise.GetLength(1),
    //     //         Height = noise.GetLength(0)
    //     //     }
    //     // });
    //     //
    //     // //fill skybox with characters according to gradient noise
    //     // for (var i = 0; i < height; i++) {
    //     //     for (var j = 0; j < width; j++) {
    //     //         var c = ' ';
    //     //         var shuffled = Random.Shared.GetItems(starrySkyCharacters.ToArray(), starrySkyCharacters.Count);
    //     //         var cellWeight = noise[i, j];
    //     //         var item = starrySkyCharacters.OrderByDescending(x => x.Value.Density).FirstOrDefault(x => x.Value.Density <= cellWeight);
    //     //         if (item.Value != null) {
    //     //             c = item.Key;
    //     //         }
    //     //
    //     //         // foreach (var (key, value) in starrySkyCharacters) {
    //     //         //     var diff = Math.Abs(noise[i, j] - value.Density);
    //     //         //     if (diff < min) {
    //     //         //         min = diff;
    //     //         //         c = key;
    //     //         //     }
    //     //         // }
    //     //
    //     //         skybox[i, j] = c;
    //     //     }
    //     // }
    //     //
    //     // for (var i = 0; i < str.Length; i++) {
    //     //     skybox[outerTopBottomBorder + innerTopBottomBorder, i + outerLeftRightBorder + innerLeftRightBorder] = str[i];
    //     // }
    //     //
    //     // for (var i = 0; i < height; i++) {
    //     //     for (var j = 0; j < width; j++) {
    //     //         sb.Append(skybox[i, j]);
    //     //     }
    //     //
    //     //     sb.AppendLine();
    //     // }
    //
    //     return sb.ToString();
    // }

#endregion
}