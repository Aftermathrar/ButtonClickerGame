﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ButtonGame.UI.EffectIcon
{
    public class EffectIconSpawner : MonoBehaviour
    {
        [SerializeField] protected EffectIconImage fxIcon = null;
        protected Dictionary<string, EffectIconImage> iconInstances = new Dictionary<string, EffectIconImage>();
        protected Stack<EffectIconImage> fxIconStack = new Stack<EffectIconImage>();

        public virtual void Spawn(string id, int count, Sprite sprite)
        {
            // float spawnPosX = transform.position.x + (50f * count);
            // Vector3 spawnPos = new Vector3(spawnPosX, transform.position.y, transform.position.z);
            // Icons are 75 units square
            float spawnPosX = (80f * count);
            Vector3 spawnPos = new Vector3(spawnPosX, 0, 0);
            EffectIconImage instance = null;
            if(fxIconStack.Count == 0)
            {
                // instance = Instantiate(fxIcon, spawnPos, Quaternion.identity, transform);
                instance = Instantiate(fxIcon, transform);
                instance.SetPosition(spawnPos);
            }
            else
            {
                instance = fxIconStack.Pop();
                instance.SetPosition(spawnPos);
                instance.gameObject.SetActive(true);
                instance.transform.SetAsLastSibling();
            }
            instance.SetIcon(sprite);
            iconInstances[id] = instance;
        }

        public virtual void ReturnToPool(string id)
        {
            if(iconInstances.ContainsKey(id))
            {
                EffectIconImage instance = iconInstances[id];
                int childCount = transform.childCount;
                if (childCount > 1)
                {
                    int fxIndex = instance.transform.GetSiblingIndex();
                    for (int i = fxIndex + 1; i < childCount; i++)
                    {
                        EffectIconImage icon = transform.GetChild(i).GetComponent<EffectIconImage>();
                        icon.StartCoroutine(icon.MoveHorizontal());
                    }
                }
                instance.gameObject.SetActive(false);
                instance.transform.SetAsFirstSibling();
                fxIconStack.Push(instance);
            }
        }

        public void UpdateIconFill(string id, float fillPercent, float timeRemaining)
        {
            if (iconInstances.ContainsKey(id))
            {
                EffectIconImage instance = iconInstances[id];
                instance.UpdateIconFill(fillPercent, timeRemaining);
            }
        }

        public void UpdateStacks(string id, float stacks)
        {
            if(iconInstances.ContainsKey(id))
            {
                EffectIconImage instance = iconInstances[id];
                instance.UpdateStacks(stacks.ToString());
            }
        }
    }
}
