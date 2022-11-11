namespace Loupedeck.HomeAssistantPlugin.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json.Nodes;
    using System.Timers;
    using System.Web;


    class HomeAssistantStateCommand : PluginDynamicCommand
    {
        protected HttpClient httpClient = new HttpClient();
        protected IDictionary<string, StateData> stateData = new Dictionary<string, StateData>();
        protected Timer timer;

        protected class StateData
        {
            public String state;
            public Boolean IsValid = false;
            public Boolean IsLoading = false;
        }

        public HomeAssistantStateCommand() : base("Get State", "Get the state of an entity", "")
        {
            this.MakeProfileAction("text;Enter entity");

            // Reload the data periodically every 1 minutes
            this.timer = new Timer(60 * 1 * 1000);
            this.timer.Elapsed += (Object, ElapsedEventArgs) =>
            {
                foreach (var actionParameter in new List<String>(this.stateData.Keys))
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

            StateData s = this.GetStateData(actionParameter);
            if (!s.IsValid)
            {
                return null;
            }

            var img = new BitmapBuilder(imageSize);
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var fn = EmbeddedResources.FindFile("ButtonBaseHomeAssistant.png");
                bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage(fn));
                if (this.stateData[actionParameter].IsValid)
                {
                    bitmapBuilder.DrawText(this.stateData[actionParameter].state);
                }
                else
                {
                    bitmapBuilder.DrawText(actionParameter);
                }


                return bitmapBuilder.ToImage();
            }
        }

        protected StateData GetStateData(string actionParameter)
        {
            StateData d;
            if (this.stateData.TryGetValue(actionParameter, out d))
                return d;

            d = new StateData();
            stateData[actionParameter] = d;

            LoadData(actionParameter);

            return d;
        }

        protected StateData _GetStateData(String actionParameter)
        {

            this.LoadData(actionParameter);

            return this.stateData[actionParameter];
        }

        protected async void LoadData(String actionParameter)
        {
            if (actionParameter == null)
            {
                return;
            }

            if (this.stateData[actionParameter] == null)
            {
                this.stateData[actionParameter] = new StateData();
            }

            StateData d = this.GetStateData(actionParameter);
            if (d.IsLoading)
            {
                return;
            }

            d.IsLoading = true;

            try
            {
                var _client = new HttpClient();
                var url = HomeAssistantPlugin.Config.Url + "states/" + actionParameter;
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.Config.Token);
                HttpResponseMessage resp = await _client.GetAsync(url);
                var json = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
                d.state = json["state"].GetValue<String>();
            }
            catch (Exception e)
            {
                d.state = "error";
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
