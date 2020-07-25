using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterSharpOpen.Client.ExtensionMethods;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.VideoModels;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Videos
{
    public partial class AddVideo
    {
        [Inject]
        protected AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }
        private Video Video { get; set; }
        private string title;
        private string videoUrl;
        private bool isSubmitReady;
        private string userMessage;
        private string apiResponse;
        [Parameter]
        public EventCallback<string> TryPlayVideo { get; set; }

        private void SubmitVideo()
        {
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(videoUrl))
            {
                userMessage = "<p class=\"pageError\">Please provide a Title and Url</p>";
                return;
            }
            var videoId = videoUrl.GetVideoId();
            if (videoId == null)
            {
                userMessage = "<p class=\"pageError\">Please provide a full YouTube Url. </p>";
                return;
            }
            Video = new Video { Title = title, VideoId = videoId };
            isSubmitReady = true;
            TryPlayVideo.InvokeAsync(videoId);
            StateHasChanged();
        }

        private async Task AddVideoToDb()
        {
            var apiResult = await PublicClient.PostVideo(Video);
            apiResponse = apiResult ? "Submission Successful!" : "Sorry, something went wrong. Submission failed";
            if (apiResult)
            {
                title = "";
                videoUrl = "";
                Video = null;
                StateHasChanged();
                await Task.Delay(3000);
                isSubmitReady = !isSubmitReady;
                StateHasChanged();
            }
            StateHasChanged();
        }
    }
}
