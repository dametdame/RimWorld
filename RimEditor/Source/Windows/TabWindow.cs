using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DRimEditor
{
   

    public abstract class TabWindow : Window
    {

        public abstract void PreActive();

        // these should be abstract if I want to make this modular
        public virtual void Initialize()
        {

        }

        public virtual void NewDefHandler()
        {

        }

        public virtual void DeleteDefHandler()
        {

        }

    }
}
