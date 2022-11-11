namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Collections;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Threading;
    
    public class HomeAssistantState : PluginDynamicCommand
    {
        protected Hashtable _States = new Hashtable();
        protected System.Timers.Timer timer;

        public HomeAssistantState() : base("Get State","Get the state of an entity", "" )
        {
            this.MakeProfileAction("text;Enter entity;");

            // Update icons periodically
            this.timer = new System.Timers.Timer(15 * 1000);
            this.timer.Elapsed += (Object, ElapsedEventArgs) => {

                foreach (String key in this._States.Keys )
                {
                    HttpRunAsync(key, this._States);
                }
                this.ActionImageChanged();
            };
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
        }

        protected override void RunCommand(String actionParameter)
        {
            //HttpRunAsync(actionParameter, this._States);
            // ActionImageChanged(actionParameter);
        }

            static async Task HttpRunAsync(String actionParameter, Hashtable States)
        {
            var _client = new HttpClient();
            var url = HomeAssistantPlugin.Config.Url + "states/" + actionParameter;
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.Config.Token);
            HttpResponseMessage resp = await _client.GetAsync(url);
            
            ;
            if (resp.IsSuccessStatusCode)
            {
                var json = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
                if (json["state"].GetValue<String>() != null)
                {
                    if (States.ContainsKey(actionParameter)){
                        States[actionParameter] = json["state"].GetValue<String>();
                    } else    {
                        States.Add(actionParameter, json["state"].GetValue<String>());
                    }
                }               
            } else
            {
                States.Add(actionParameter, "Error");
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var fn = EmbeddedResources.FindFile("ButtonBaseHomeAssistant.png");
                bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage(fn));
                if (this._States.ContainsKey(actionParameter))
                {
                    bitmapBuilder.DrawText(this._States[actionParameter].ToString());
                } else
                { 
                    bitmapBuilder.DrawText(actionParameter); 
                }

                this.ActionImageChanged(actionParameter);
                return bitmapBuilder.ToImage();
            }
        }
    }
}