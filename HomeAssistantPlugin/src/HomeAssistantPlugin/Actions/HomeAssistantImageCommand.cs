namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.Text.Json.Nodes;

    class HomeAssistantImageCommand : PluginDynamicCommand
    {
        protected class Template
        {
            public String template { get; set; }
        }

        public HomeAssistantImageCommand() 
        {
            this.AddParameter("3zeilig", "Text in drei zeilen", "Multiline");
            this.AddParameter("2zeilig", "Text in zwei Zeilen", "Multiline");
            this.AddParameter("1big", "Eine große Zahl", "Singleline");
            this.AddParameter("1norm", "Text in einer zeilen", "Singleline");
            this.AddParameter("gauge", "Zeige Gauge an der Seite", "Graphic");
            this.AddParameter("circle", "virteilkreis unten rechts", "Graphic");
            this.AddParameter("dreieck", "Dreieck oben rechts", "Graphic");
        }

        protected override void RunCommand(String actionParameter)
        {
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter == null)
            {
                return null;
            }

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var fn = EmbeddedResources.FindFile("ButtonBaseHomeAssistant.png");
                bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage(fn));

                switch (actionParameter)            
                {
                    case "3zeilig":
                        bitmapBuilder.DrawText("Line 2xxx", 3, 30, 75, 15);
                        bitmapBuilder.DrawText("Line 1xx", 3, 13, 75, 15);
                        bitmapBuilder.DrawText("Line 3xxxx", 3, 47, 75, 15);
                        break;
                    case "2zeilig":
                        bitmapBuilder.DrawText("Line 1big", 3, 25, 75, 15, new BitmapColor(230, 230, 230), 20);
                        bitmapBuilder.DrawText("Line 2norm", 3, 47, 75, 15);
                        break;
                    case "1big":
                            bitmapBuilder.DrawText("BIG Line", 3, 31, 75, 15, new BitmapColor(220, 220, 220), 22);
                        break;
                    case "1norm":
                        bitmapBuilder.DrawText("Line 1norm", 3, 30, 75, 15);
                        break;
                    case "gauge":
                        bitmapBuilder.FillRectangle(70, 30, 15, 50, new BitmapColor(20, 240, 20));
                        break;
                    case "circle":
                        bitmapBuilder.FillCircle(75, 75, 15, new BitmapColor(240, 20, 20));
                        break;
                    case "dreieck":
                        bitmapBuilder.DrawLine(55, 0, 90, 25, new BitmapColor(240, 240, 20), 15);
                        break;
                    default:
                        break;
                };
                
                return bitmapBuilder.ToImage();
            }
        }
    }
}
