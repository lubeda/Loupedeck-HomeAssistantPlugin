namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.Net.Http.Headers;
    using System.Net.Http;

    public class HomeAssistantState : PluginDynamicCommand
    {
        public HomeAssistantState() : base()
        {
            this.MakeProfileAction("text;Select entity;");
        }

        protected override void RunCommand(String actionParameter)
        {
            var _client = new HttpClient();
            var url = HomeAssistantPlugin.Config.Url + "states/" + actionParameter;
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.Config.Token);
            var resp = _client.GetStringAsync(url); 
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {       
             return actionParameter;
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var fn = EmbeddedResources.FindFile("ButtonBaseHomeAssistant.png");
                bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage(fn));
                bitmapBuilder.DrawText(actionParameter);

                return bitmapBuilder.ToImage();
            }
        }
    }
}