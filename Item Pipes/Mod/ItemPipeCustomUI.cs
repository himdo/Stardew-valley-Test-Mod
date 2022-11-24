﻿using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Minigames;
using System.Reflection;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Xml.Linq;
using SObject = StardewValley.Object;
using ItemPipes.ItemPipeObject;

namespace ItemPipes.ItemPipeUI
{
    public class ItemPipeCustomUI : MenuWithInventory
    {
        private RootElement ui;
        private Table table;
        private ItemPipe itemPipeInstance;
        private int heightOffset = -120;
        // TODO if you close the UI with an item in your hand it will delete the item FIXME

        public ItemPipeCustomUI() : base(null, okButton: false, trashCan: false, 0,0)//12, 132)//: base((Game1.uiViewport.Width - 900) / 2, (Game1.uiViewport.Height - (Game1.uiViewport.Height - 100)) / 2, 900, (Game1.uiViewport.Height - 100))
        {

            //int num2 = 0;
            //int actualCapacity = WhiteListItems.Count + 1;
            //int rows = 3;
            //ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num2 / 2, yPositionOnScreen + 64, playerInventory: false, WhiteListItems, InventoryMenu.highlightAllItems, actualCapacity, rows);
            ReCreateUI();
        }
        private void ReCreateUI()
        {
            ui = new RootElement()
            {
                LocalPosition = new Vector2(this.xPositionOnScreen, this.yPositionOnScreen)
            };

            var title = new Label()
            {
                String = "Pipe Configuration",
                Bold = true,
            };
            title.LocalPosition = new Vector2((width - title.Width) / 2, 10 + heightOffset);
            ui.AddChild(title);

            

            var DirectionText = new Label()
            {
                String = "Direction:",
                Bold = true,
            };
            DirectionText.LocalPosition = new Vector2(15, 100 + heightOffset);
            ui.AddChild(DirectionText);

            int moveOverConstant = 300;
            var SouthToNorth = new Label()
            {
                String = "Up",
                //String = "↑",
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.SouthToNorth : false,
                Callback = (e) => SetPipeDirection(Directions.SouthToNorth),
            };
            SouthToNorth.LocalPosition = new Vector2((width - SouthToNorth.Width) / 2 - moveOverConstant, 150 + heightOffset);
            ui.AddChild(SouthToNorth);

            var EastToWest = new Label()
            {
                String = "Left",
                //String = "→",
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.EastToWest : false,
                Callback = (e) => SetPipeDirection(Directions.EastToWest),
            };
            EastToWest.LocalPosition = new Vector2((width - EastToWest.Width) / 2 - 70 - moveOverConstant, 200 + heightOffset);
            ui.AddChild(EastToWest);
            var WestToEast = new Label()
            {
                String = "Right",
                //String = "←",
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.WestToEast : false,
                Callback = (e) => SetPipeDirection(Directions.WestToEast),
            };
            WestToEast.LocalPosition = new Vector2((width - WestToEast.Width) / 2 + 70 - moveOverConstant, 200 + heightOffset);
            ui.AddChild(WestToEast);

            var NorthToSouth = new Label()
            {

                String = "Down",
                //String = "↓",
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.NorthToSouth : false,
                Callback = (e) => SetPipeDirection(Directions.NorthToSouth),
            };
            NorthToSouth.LocalPosition = new Vector2((width - NorthToSouth.Width) / 2 - moveOverConstant, 250 + heightOffset);
            ui.AddChild(NorthToSouth);
            var accept = new Label()
            {
                String = "Accept",
                Bold = true,
                Callback = (e) => Accept(),
            };

            accept.LocalPosition = new Vector2((width - accept.Width) - 150, height / 2 + 5 + heightOffset);
            ui.AddChild(accept);


            var whitelistText = new Label()
            {
                String = "Whitelist:",
                Bold = true,
            };
            whitelistText.LocalPosition = new Vector2((3*(width - whitelistText.Width)) / 4, 60 + heightOffset);
            ui.AddChild(whitelistText);

            table = new Table()
            {
                RowHeight = (128 - 16) / 8,
                Size = new Vector2(width/2 + 80, height/2 - 165),
                LocalPosition = new Vector2(width/2 - 120, 10),
            };

            if (this.itemPipeInstance != null)
            {
                List<Element> rowSlots = new List<Element>();
                int numberOfSlotsPerRow = 5;
                for (int i = 0; i < itemPipeInstance.WhiteListItems.Count + 1; i++)
                {
                    var itemSlot = new ItemSlot()
                    {
                        LocalPosition = new Vector2(10 + (100*(i% numberOfSlotsPerRow)), 150 * ((int)(Math.Ceiling((float) (i+1)/numberOfSlotsPerRow)))),
                        Callback = (e) => AddItem((ItemSlot)e),
                    };
                    if (i < itemPipeInstance.WhiteListItems.Count)
                    {
                        itemSlot.ItemDisplay = itemPipeInstance.WhiteListItems[i];
                    }
                    rowSlots.Add(itemSlot);
                    if ((i+1)% numberOfSlotsPerRow == 0)
                    {
                        table.AddRow(rowSlots.ToArray());
                        // This appears to only put blank spaces
                        for (int j = 0; j < 2; ++j)
                            table.AddRow(new Element[0]);
                        rowSlots = new List<Element>();
                    }
                }
                if (rowSlots.Count > 0)
                {
                    table.AddRow(rowSlots.ToArray());
                    // This appears to only put blank spaces
                    for (int j = 0; j < 2; ++j)
                        table.AddRow(new Element[0]);
                    rowSlots = new List<Element>();
                }
            }
            ui.AddChild(table);

            

        }

        public ItemPipeCustomUI(ItemPipe instance): this()
        {
            itemPipeInstance = instance;
            ReCreateUI();

        }
        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen + heightOffset, width, height / 2, Color.White);
            IClickableMenu.drawTextureBox(b, xPositionOnScreen + width-310, yPositionOnScreen + height / 2 - 130, 170,75, Color.White);
            base.draw(b, false, false);

            ui.Draw(b);
            if (base.heldItem != null)
            {
                base.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            }

            if (ItemWithBorder.HoveredElement != null)
            {
                if (ItemWithBorder.HoveredElement is ItemSlot slot && slot.Item != null)
                {
                    drawToolTip(b, slot.Item.getDescription(), slot.Item.DisplayName, slot.Item);
                }
                else if (ItemWithBorder.HoveredElement.ItemDisplay != null)
                {
                    drawToolTip(b, ItemWithBorder.HoveredElement.ItemDisplay.getDescription(), ItemWithBorder.HoveredElement.ItemDisplay.DisplayName, ItemWithBorder.HoveredElement.ItemDisplay);
                }
            }
            else
            {
                var hover = base.inventory.hover(Game1.getMouseX(), Game1.getMouseY(), null);
                if (hover != null)
                {
                    drawToolTip(b, base.inventory.hoverText, base.inventory.hoverTitle, hover);
                }
            }

            drawMouse(b);
        }

        public override void update(GameTime time)
        {
            ui.Update();
        }
        protected override void cleanupBeforeExit()
        {
            if (base.heldItem != null)
            {
                Game1.player.addItemToInventory(base.heldItem);
                //base.heldItem = null;
            }
            base.cleanupBeforeExit();
        }
        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                Exit();
            }
        }

        private void Exit()
        {
            this.exitThisMenuNoSound();
        }

        private void SetPipeDirection(Directions direction)
        {
            if (this.itemPipeInstance != null)
            {
                this.itemPipeInstance.ChangeDirection(direction);
                ReCreateUI();
            }
        }
        private void Accept()
        {
            Exit();
        }

        public override void receiveScrollWheelAction(int direction)
        {
            this.table.Scrollbar.ScrollBy(direction / -120);
        }

        private void AddItem(ItemSlot e)
        {
            if (base.heldItem == null)
            {
                itemPipeInstance.WhiteListItems.Remove(e.ItemDisplay);
                e.ItemDisplay = null;
                ReCreateUI();
            }
            else
            {
                bool foundCopy = false;
                for (int i = 0; i < itemPipeInstance.WhiteListItems.Count; i++)
                {
                    if (itemPipeInstance.WhiteListItems[i].ParentSheetIndex == base.heldItem.ParentSheetIndex && (itemPipeInstance.WhiteListItems[i] as SObject).quality.Value == (base.heldItem as SObject).quality.Value)
                    {
                        foundCopy = true;
                        break;
                    }
                }
                if (foundCopy == false)
                {
                    itemPipeInstance.WhiteListItems.Add(base.heldItem.getOne());
                    e.ItemDisplay = base.heldItem.getOne();
                    ReCreateUI();

                }
            }

        }
    }

}