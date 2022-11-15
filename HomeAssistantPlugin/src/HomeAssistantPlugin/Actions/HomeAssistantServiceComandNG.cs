namespace Loupedeck.HomeassistantPlugin
{
    using System;
    using System.Collections;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public class HomeAssistandCommand1 : PluginDynamicCommand
    {
        protected class StateData
        {
            public String state;
            public String entity_id;
            public Hashtable attributes;
            public Hashtable context;
            public DateTime last_changed;
            public DateTime last_updated;
            public Boolean IsValid = false;
            public Boolean IsLoading = false;
        }
        public HomeAssistandCommand1() : base("Call Service NG", "Controll your Home", "Entity")
        {
            this.MakeProfileAction("tree");
        }
        protected override PluginProfileActionData GetProfileActionData()
        {
            var tree = new PluginProfileActionTree("Select Service and entity");
            tree.AddLevel("Service");
            tree.AddLevel("Entity");
            if (HomeAssistantPlugin.HomeAssistantPlugin.Config != null)
            {
                if (HomeAssistantPlugin.HomeAssistantPlugin.Config.Entries.Length > 0)
                {
                    foreach (HomeAssistantPlugin.HomeAssistantPlugin.HAServiceEntry e in HomeAssistantPlugin.HomeAssistantPlugin.Config.Entries)
                    {
                        PluginProfileActionTreeNode pluginProfileActionTreeNode = tree.Root.AddNode(e.Service);
                        foreach (String s in e.Entities)
                        {
                            pluginProfileActionTreeNode.SetPropertyValue("Service", e.Service);
                            pluginProfileActionTreeNode.AddItem(e.Service + "|" + s, s, e.Service);
                        }
                    }
                }
            }
            return tree;
        }

        protected override void RunCommand(String actionParameter)
        {
            this.CallService(actionParameter);
        }
        protected async void CallService(String actionParameter)
        {
            if (actionParameter.Contains("|"))
            {
                var service = actionParameter.Split("|")[0];
                var entity = actionParameter.Split("|")[1];
                using (var _client = new HttpClient())
                {

                    var url = HomeAssistantPlugin.HomeAssistantPlugin.Config.Url + "services/" + service.Split(".")[0] + "/" + service.Split(".")[1];

                    var body = @"{""entity_id"": """ + entity + @"""}";

                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.HomeAssistantPlugin.Config.Token);

                    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"); //https://developers.home-assistant.io/docs/api/rest/
                    var resp = await _client.PostAsync(url, content);
                    
                    if (resp.IsSuccessStatusCode)
                    {
                        var text = await resp.Content.ReadAsStringAsync();
                        StateData json = JsonConvert.DeserializeObject<StateData>(text);
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
