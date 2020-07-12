using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace MasterSharpOpen.Client.Pages.Videos
{
    public partial class VideoHome
    {
        
        [Inject]
        protected PublicClient PublicClient { get; set; }
        public MasterSharpOpen.Shared.VideoModels.Videos Videos { get; set; }
        protected string selectedVideoId { get; set; }
        protected bool IsVideoReady;
        protected bool IsPageVideosReady;
        protected bool IsCodeAlong;
        protected override async Task OnInitializedAsync()
        {
            Videos ??= await PublicClient.GetVideos();
            IsPageVideosReady = true;
        }
        protected void HandleVideoEnd(bool isEnd)
        {
            IsVideoReady = false;
        }
        protected async Task PlayVideos()
        {
            if (IsVideoReady)
            {
                IsVideoReady = false;
                StateHasChanged();
                await Task.Delay(500);
            }
            IsVideoReady = true;
            StateHasChanged();
        }

        protected Task HandleTryPlay(string videoId)
        {
            selectedVideoId = videoId;
            return PlayVideos();
        }
    }
}