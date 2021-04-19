using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class planet
    {
        public string Name;
        public planet(string name)
        {
            this.Name = name;
        }
        public override string ToString()
        {
          return this.Name;
        }
    }
}