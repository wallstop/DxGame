using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using MetroFramework.Forms;

namespace MapEditor.UI
{
    
    public class PlatformSelector
    {
        private readonly MenuStrip mainMenu_;

        public PlatformType PlatformType { get; private set; }

        public PlatformSelector(MenuStrip mainMenu)
        {
            Validate.IsNotNull(mainMenu, StringUtils.GetFormattedNullOrDefaultMessage(this, mainMenu));
            mainMenu_ = mainMenu;
            Setup();
        }

        private void Setup()
        {
            var platformType = new ToolStripMenuItem();
            mainMenu_.Items.Add(platformType);
            platformType.Size = new Size(37, 20);
            platformType.Text = "PlatformType";

            var block = new RadioToolStripMenuItem(() => PlatformType = PlatformType.Block)
            {
                Size = new Size(37, 20),
                Text = "Block"
            };
            var platform = new RadioToolStripMenuItem(() => PlatformType = PlatformType.Platform)
            {
                Size = new Size(37, 20),
                Text = "Platform"
            };
            block.CheckState = CheckState.Checked;
            platformType.DropDownItems.AddRange(new ToolStripItem[] { block, platform });
        }
    }
}
