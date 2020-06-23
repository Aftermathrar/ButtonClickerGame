﻿using UnityEngine;
using ButtonGame.Inventories;
using ButtonGame.Core.UI.Dragging;
using UnityEngine.EventSystems;

namespace ButtonGame.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>, IPointerClickHandler
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;
        Inventory inventory = null;
        Equipment equipment = null;

        // PUBLIC

        public void Setup(Inventory inventory, Equipment equipment, int index)
        {
            this.inventory = inventory;
            this.equipment = equipment;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetCountInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetCountInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(equipment == null || eventData.pointerId != -2) return;

            InventoryItem item = inventory.GetItemInSlot(index);
            int equipIndex = equipment.TryAddItem(item);
            if(equipIndex >= 0)
            {
                EquipableItem equipableItem = item as EquipableItem;
                EquipLocation equipLocation = equipableItem.GetAllowedEquipLocation();

                EquipableItem takebackItem = equipment.GetItemInSlot(equipLocation, equipIndex);

                equipment.AddItem(equipLocation, equipableItem, equipIndex);
                RemoveItems(1);
                if(takebackItem != null)
                {
                    AddItems(takebackItem, 1);
                }
            }
        }
    }
}