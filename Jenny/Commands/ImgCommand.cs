using System.Diagnostics;
using System.Numerics;
using ArcaneLibs;
using ArcaneLibs.Extensions;
using LibMatrix.EventTypes.Spec;
using LibMatrix.Utilities.Bot.Interfaces;

namespace Jenny.Commands;

public class ImgCommand : ICommand {
    public string Name { get; } = "img";
    public string[]? Aliases { get; } = [];
    public string Description { get; }
    public bool Unlisted { get; } = true;

    public async Task Invoke(CommandContext ctx) {
        int count = 1;
        if (ctx.Args is { Length: 1 })
            int.TryParse(ctx.Args[0], out count);

        for (var i = 0; i < count; i++) {
            new Thread(async () => {
                var bigNoise = GenerateHeightMap(5000, 2000);
                await ctx.Room.SendMessageEventAsync(new RoomMessageEventContent("m.image", "src_noise.png") {
                    Url = await ctx.Homeserver.UploadFile("data.png", await Float2DArrayToPng(bigNoise), "image/png"),
                    FileInfo = new() {
                        Width = bigNoise.GetWidth(),
                        Height = bigNoise.GetHeight()
                    }
                });
            }).Start();
        }

    }
    
    public async Task<byte[]> Float2DArrayToPng(float[,] data) {
        //dump heightmap as PPM
        Console.WriteLine($"{DateTime.Now} Converting to PNG");
        var width = data.GetLength(1);
        var height = data.GetLength(0);

        //convert ppm to png with ffmpeg
        var process = new Process {
            StartInfo = new ProcessStartInfo {
                // FileName = "/nix/store/4hz763c5w2hnzm55ll5vgfgmrr6i9kgg-imagemagick-7.1.1-28/bin/convert",
                FileName = "convert",
                Arguments = $"ppm:- png:-",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            }
        };
        process.Start();
        await process.StandardInput.WriteLineAsync($"P2\n{width} {height}\n255");
        for (var i = 0; i < height; i++) {
            for (var j = 0; j < width; j++) {
                await process.StandardInput.WriteAsync($"{(int)(data[i, j] * 255)} ");
            }

            // ppm.AppendLine();
        }
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();
        await using var ms = new MemoryStream();
        await process.StandardOutput.BaseStream.CopyToAsync(ms);
        ms.Position = 0;
        Console.WriteLine($"{DateTime.Now} Converted to PNG");
        return ms.ToArray();
    }

    public float[,] GenerateHeightMap(int width, int height) {
        var rnd = new Random();

        var bigNoiseVector3 = new Vector3[height, width];
        for (var y = 0; y < bigNoiseVector3.GetLength(0); y++) {
            for (var x = 0; x < bigNoiseVector3.GetLength(1); x++) {
                bigNoiseVector3[y, x] = new Vector3(0, 0, 0);
            }
        }

        bigNoiseVector3[0, 0] = new Vector3(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        var last = bigNoiseVector3[0, 0];
        for (var y = 0; y < bigNoiseVector3.GetLength(0); y++) {
            if (y > 0) break;
            for (var x = 0; x < bigNoiseVector3.GetLength(1); x++) {
                float currentX = x;
                float currentY = y;
                int steps = 0;
                int maxSteps = 1000;
                while (steps++ < maxSteps) {
                    if (currentX < 0) break;
                    if (currentY < 0) break;
                    if (currentX > bigNoiseVector3.GetWidth() - 1) break;
                    if (currentY > bigNoiseVector3.GetHeight() - 1) break;

                    var current = bigNoiseVector3[(int)currentY, (int)currentX];
                    // if (current is {X: 0f, Y: 0f, Z: 0f}) {
                    // bigNoiseVector3[currentY, currentX] = current = new Vector3(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
                    // }

                    current = new(last.X, last.Y, last.Z);
                    var diff = new Vector3(
                        MathUtil.Map(rnd.NextSingle(), 0f, 1f, -0.2f, 0.2f),
                        MathUtil.Map(rnd.NextSingle(), 0f, 1f, -0.2f, 0.2f),
                        -0.1f
                    );
                    current += diff;
                    bigNoiseVector3[(int)currentY, (int)currentX] = current;

                    // Console.WriteLine("{0}/{1}={2} (+{3})", currentX, currentY, current, diff);
                    currentX += current.X;
                    currentY += current.Y;
                    
                    // if (current.X > 0.666f) currentX++;
                    // else if (current.X < 0.333f) currentX--;
                    // if (current.Y > 0.666f) currentY++;
                    // else if (current.Y < 0.333f) currentY--;

                    last = current;
                }
            }
        }

        var bigNoise = new float[height, width];
        for (var i = 0; i < bigNoise.GetLength(0); i++) {
            for (var j = 0; j < bigNoise.GetLength(1); j++) {
                // bigNoise[i, j] = (float) (bigNoiseVector[i, j].Length() / Math.Sqrt(2));
                bigNoise[i, j] = (float)(bigNoiseVector3[i, j].Z);
            }
        }

        return bigNoise;
    }
}