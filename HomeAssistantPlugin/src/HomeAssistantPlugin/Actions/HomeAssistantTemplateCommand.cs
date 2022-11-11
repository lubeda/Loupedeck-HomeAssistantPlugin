namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json.Nodes;
    using System.Timers;

    class HomeAssistantTemplateCommand : PluginDynamicCommand
    {
        protected HttpClient httpClient = new HttpClient();
        protected IDictionary<string, TemplateData> templateData = new Dictionary<string, TemplateData>();
        protected Timer timer;

        protected class TemplateData
        {
            public String template;
            public Boolean IsValid = false;
            public Boolean IsLoading = false;
        }

        public HomeAssistantTemplateCommand() : base("Eval a template", "Evaluate a template to show the result.", "")
        {
            this.MakeProfileAction("text;Enter template");

            // Reload the data periodically every 1 minutes
            this.timer = new Timer(60 * 1 * 1000);
            this.timer.Elapsed += (Object, ElapsedEventArgs) =>
            {
                foreach (var actionParameter in new List<String>(this.templateData.Keys))
                {
                    this.LoadData(actionParameter);
                }
            };
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
        }

        protected override void RunCommand(String actionParameter)
        {
            this.LoadData(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter == null)
            {
                return null;
            }

            TemplateData s = this.GetTemplateData(actionParameter);
            if (!s.IsValid)
            {
                return null;
            }

            var img = new BitmapBuilder(imageSize);
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var fn = EmbeddedResources.FindFile("ButtonBaseHomeAssistant.png");
                bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage(fn));
                if (this.templateData[actionParameter].IsValid)
                {
                    bitmapBuilder.DrawText(this.templateData[actionParameter].template);
                }
                else
                {
                    bitmapBuilder.DrawText("Error");
                }


                return bitmapBuilder.ToImage();
            }
        }

        protected TemplateData GetTemplateData(String actionParameter)
        {
            TemplateData d;
            if (this.templateData.TryGetValue(actionParameter, out d))
                return d;

            d = new TemplateData();
            this.templateData[actionParameter] = d;

            this.LoadData(actionParameter);

            return d;
        }

        protected async void LoadData(String actionParameter)
        {
            if (actionParameter == null)
            {
                return;
            }

            if (this.templateData[actionParameter] == null)
            {
                this.templateData[actionParameter] = new TemplateData();
            }

            TemplateData d = this.GetTemplateData(actionParameter);
            if (d.IsLoading)
            {
                return;
            }

            d.IsLoading = true;

            try
            {
                var _client = new HttpClient();

                var url = HomeAssistantPlugin.Config.Url + "template";
                var body = @"{""template"": """ + actionParameter + @"""}";
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.Config.Token);
                var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"); //https://developers.home-assistant.io/docs/api/rest/
                var resp = await _client.PostAsync(url, content);
                if (resp.IsSuccessStatusCode)
                { d.template = await resp.Content.ReadAsStringAsync(); }
                else
                { d.template = "Error"; }

            }
            catch (Exception e)
            {
                d.template = "Error";
            }
            finally
            {
                d.IsLoading = false;
                d.IsValid = true;
                this.ActionImageChanged(actionParameter);
            }
        }
    }

}
