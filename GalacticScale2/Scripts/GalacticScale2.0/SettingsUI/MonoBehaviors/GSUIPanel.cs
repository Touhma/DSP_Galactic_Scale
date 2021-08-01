using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIPanel : MonoBehaviour
    {
        public GSUIList contents;
        public GSUITemplates templates;
        public GameObject Add(GSUITemplate template)
        {
            return contents.AddItem(template);
        }
    }

}