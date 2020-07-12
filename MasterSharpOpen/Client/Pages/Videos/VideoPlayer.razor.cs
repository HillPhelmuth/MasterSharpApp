using System;
using System.Threading.Tasks;
using MasterSharpOpen.Client.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MasterSharpOpen.Client.Pages.Videos
{
    public partial class VideoPlayer : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public EventCallback<bool> VideoEnded { get; set; }
        [Parameter]
        public string VideoId { get; set; }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var refThis = DotNetObjectReference.Create(this);
                await JSRuntime.StartYouTube();
                await Task.Delay(500);
                await JSRuntime.InvokeAsync<object>("getYouTube", refThis, VideoId);
            }
        }
        [JSInvokable]
        // ReSharper disable once UnusedMember.Global -JSInvokable used by javascript code
        public async Task GetNextVideo()
        {
            await Task.Delay(500);
            await JSRuntime.RemoveYouTubePlayer();
            await VideoEnded.InvokeAsync(false);
        }
        public void Dispose()
        {
            Console.WriteLine("VideoPlayer.razor Disposed");
            JSRuntime.RemoveYouTubePlayer();
        }
    }
}
