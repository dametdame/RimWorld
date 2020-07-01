using System;
using Verse;

namespace DRimEditor.DetailView
{
	public interface DefView
	{
	
        // <summary>
        /// Handles jumping the GUI to a specific helpDef
        /// from a ResearchProjectDef
        /// </summary>
        /// <param name="def"></param>
        void JumpTo(Verse.Def def);

        /// <summary>
        /// Handles jumping the GUI to a specific helpDef
        /// </summary>
        /// <param name="def"></param>
        void JumpTo(ItemDetail def);

        /// <summary>
        /// Does this view accept links to this helpDef
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        bool Accept(ItemDetail def);

        /// <summary>
        /// What other view should links be redirected to. Only called when Accept() returns false.
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
    }
}
