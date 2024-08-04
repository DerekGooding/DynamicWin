﻿using DynamicWin.UI.UIElements;
using DynamicWin.UI.UIElements.Custom;
using DynamicWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWin.UI.Menu.Menus
{
    internal class DropFileMenu : BaseMenu
    {
        private static DropFileMenu instance;

        public override Vec2 IslandSize()
        {
            return new Vec2(450, 200);
        }

        public static void Drop(DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Dropped!");

            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (fileList != null && fileList.Length > 0)
            {
                Tray.AddFiles(fileList);
            }
        }

        public DropFileMenu()
        {
            instance = this;
        }

        public override List<UIObject> InitializeMenu(IslandObject island)
        {
            var objects = base.InitializeMenu(island);
            
            var dropObj = new DropFileElement(island, Vec2.zero, new Vec2(400, 150), UIAlignment.Center);
            objects.Add(dropObj);

            return objects;
        }
    }
}
