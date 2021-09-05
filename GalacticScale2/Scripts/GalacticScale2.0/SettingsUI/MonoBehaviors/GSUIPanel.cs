using UnityEngine;

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