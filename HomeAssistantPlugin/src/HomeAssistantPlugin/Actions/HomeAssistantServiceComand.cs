namespace Loupedeck.HomeassistantPlugin
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class HomeAssistandCommand : PluginDynamicCommand
    {
        protected class EntityData
        {
            public String entity_id { get; set; }
        }

        public HomeAssistandCommand() : base("Call Service", "Controll your Home", "Entity")
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
                        foreach (var s in e.Entities)
                        {
                            pluginProfileActionTreeNode.SetPropertyValue("Service", e.Service);
                            pluginProfileActionTreeNode.AddItem(e.Service + "|" + s, s, e.Service);
                        }
                    }
                }
            }
            return tree;
        }

        protected async Task CallServiceAsync(String service,
                                                       String entityname)
        {
            using (var _client = new HttpClient())
            {
                var url = HomeAssistantPlugin.HomeAssistantPlugin.Config.Url + "services/" + service.Split(".")[0] + "/" + service.Split(".")[1];

                var entity = new EntityData();
                entity.entity_id = entityname;

                var body = JsonConvert.SerializeObject(entity); // @"{""entity_id"": """ + entity + @"""}";

                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.HomeAssistantPlugin.Config.Token);

                var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"); //https://developers.home-assistant.io/docs/api/rest/
                var resp = await _client.PostAsync(url, content);
            }
        }


        protected override void RunCommand(String actionParameter)
        {
            if (actionParameter.Contains("|"))
            {
                var service = actionParameter.Split("|")[0];
                var entityname = actionParameter.Split("|")[1];
                this.CallServiceAsync(service, entityname);
            }
        }
    }
}
