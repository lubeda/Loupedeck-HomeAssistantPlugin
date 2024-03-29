﻿namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text.Json.Nodes;
    using System.Timers;

    class HomeAssistantStateCommand : PluginDynamicCommand
    {
        protected HttpClient httpClient = new HttpClient();
        protected IDictionary<String, StateData> stateData = new Dictionary<String, StateData>();
        protected Timer timer;

        protected class StateData
        {
            public String state;
            public Boolean IsValid = false;
            public Boolean IsLoading = false;
        }

        public HomeAssistantStateCommand() : base("Get a state", "Get the state value of an entity.", "")
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

        protected StateData GetStateData(String actionParameter)
        {
            StateData d;
            
            if (this.stateData.TryGetValue(actionParameter, out d))
            {
                return d;
            }

            d = new StateData();
            this.stateData[actionParameter] = d;

            this.LoadData(actionParameter);

            return d;
        }

        protected async void LoadData(String actionParameter)
        {
            if (actionParameter == null)
            {
                return;
            }

            StateData d = this.GetStateData(actionParameter);
            
            if (d.IsLoading)
            {
                d.IsValid = false;
                return;
            }

            d.IsLoading = true;

            try
            {
                var _client = new HttpClient();

                var url = HomeAssistantPlugin.Config.Url + "states/" + actionParameter;
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.Config.Token);
                var resp = await _client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    try
                    {
                        var body = await resp.Content.ReadAsStringAsync();
                        var json = JsonNode.Parse(body);
                        d.state = json["state"].GetValue<String>();
                        d.IsValid = true;
                    } 
                    catch (HttpRequestException e)
                    {
                        d.state = e.Message;
                        d.IsValid = true;
                    }
                }
                else
                { 
                    d.state = "Error1"; 
                }

            }
            catch (Exception e)
            {
                d.state = "Error2";
            }
            finally
            {
                d.IsLoading = false;
                this.ActionImageChanged(actionParameter);
            }
        }
    }
}
