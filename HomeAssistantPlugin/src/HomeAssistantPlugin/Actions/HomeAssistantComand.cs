namespace Loupedeck.HomeassistantPlugin
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class HomeAssistandCommand : PluginDynamicCommand
    {
 
        public HomeAssistandCommand() : base("Home assistant", "Controll your Home", "Entity")
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
                            pluginProfileActionTreeNode.AddItem(e.Service + "|" + s,s,e.Service);
                        }
                    }
                }
            }
            return tree;
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => String.IsNullOrEmpty(actionParameter) ? "HomeAssistant" : actionParameter;

        
        protected override void RunCommand(String actionParameter)
        {
            if (actionParameter.Contains("|"))
            {
                var service = actionParameter.Split("|")[0];
                var entity = actionParameter.Split("|")[1];
                var _client = new HttpClient();
                var url = HomeAssistantPlugin.HomeAssistantPlugin.Config.Url + "services/" + service.Split(".")[0] + "/" + service.Split(".")[1];
                // var body = "{\"entity_id\": \"" + entity + "\"}";
                var body = @"{""entity_id"": """ + entity + @"""}";
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HomeAssistantPlugin.HomeAssistantPlugin.Config.Token);
                var content = new StringContent(body,System.Text.Encoding.UTF8, "application/json"); //https://developers.home-assistant.io/docs/api/rest/
                _client.PostAsync(url, content);
            }
        }

        /*
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);
                if (actionParameter.Contains("|"))
                {
                    bitmapBuilder.DrawText(actionParameter.Split("|")[1].Replace(".",".\n"), BitmapColor.White, 15, 13, 3);
                } 
                return bitmapBuilder.ToImage();
            }
        } */

    }
}
