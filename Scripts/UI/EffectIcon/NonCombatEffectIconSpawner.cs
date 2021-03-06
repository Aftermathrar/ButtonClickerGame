﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ButtonGame.UI.EffectIcon
{
    public class NonCombatEffectIconSpawner : EffectIconSpawner
    {
        EffectDisplay fxDisplay;

        public override void Spawn(string id, int count, Sprite sprite)
        {
            float spawnPosX = (50f * count);
            Vector3 spawnPos = new Vector3(spawnPosX, 0, 0);
            EffectIconImage instance = null;
            if (fxIconStack.Count == 0)
            {
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

            // Set tooltip info
            fxDisplay = instance.GetComponent<EffectDisplay>();
            fxDisplay.SetEffectName(id);
        }

        public override void DeactivateIcon(string id)
        {
            if (iconInstances.ContainsKey(id))
            {
                EffectIconImage instance = iconInstances[id];
                instance.gameObject.SetActive(false);
                instance.transform.SetAsFirstSibling();
                fxIconStack.Push(instance);
            }
        }
    }
}
