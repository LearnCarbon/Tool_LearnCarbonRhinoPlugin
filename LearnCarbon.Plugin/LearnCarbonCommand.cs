using LearnCarbon.Plugin.Properties;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LearnCarbon
{
    public class LearnCarbonCommand : Command
    {
        public LearnCarbonCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
            Panels.RegisterPanel(LearnCarbonPlugin.Instance, typeof(LearnCarbonHost), "LearnCarbon Plugin", Resources.logo);
        }

        ///<summary>The only instance of this command.</summary>
        public static LearnCarbonCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LearnCarbon";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panel_id = typeof(LearnCarbonHost).GUID;
            var panel_visible = Panels.IsPanelVisible(panel_id);

            var prompt = (panel_visible)
              ? "Learn carbon panel is visible. New value"
              : "Learn carbon panel is hidden. New value";

            var go = new GetOption();
            go.SetCommandPrompt(prompt);
            var hide_index = go.AddOption("Hide");
            var show_index = go.AddOption("Show");
            var toggle_index = go.AddOption("Toggle");
            go.Get();

            if (go.CommandResult() != Result.Success)
                return go.CommandResult();

            var option = go.Option();
            if (null == option)
                return Result.Failure;

            var index = option.Index;
            if (index == hide_index)
            {
                if (panel_visible)
                    Panels.ClosePanel(panel_id);
            }
            else if (index == show_index)
            {
                if (!panel_visible)
                    Panels.OpenPanel(panel_id);
            }
            else if (index == toggle_index)
            {
                if (panel_visible)
                    Panels.ClosePanel(panel_id);
                else
                    Panels.OpenPanel(panel_id);
            }
            return Result.Success;
        }
    }
}
