using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using DHotMeals.Comps;

namespace DHotMeals
{
    class Thought_MealTemp : Thought_Memory
    {

        public override string Description
        {
            get
            {
                if (comp == null)
                    return "Consumed something";

                return "That " + comp.parent.Label + " was " + comp.GetState(createdTemp);
            }
        }

        public override string LabelCap 
        {
            get
            {
                if (comp == null)
                    return "Consumed something";

                return comp.GetFoodType() + " was " + comp.GetState(createdTemp);
            }
        } 

        public override float MoodOffset()
        {
            return GetMoodValue();
        }

        public override bool GroupsWith(Thought other)
        {
            if (comp == null)
                return false;
            Thought_MealTemp otherMeal = other as Thought_MealTemp;
            if (otherMeal == null)
                return false;

            if (comp.PropsTemp.mealType == MealTempTypes.None)
                return false;
            else if (comp.PropsTemp.mealType == otherMeal.comp.PropsTemp.mealType)
            {
                return true;
            }
            return false;
        }

        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
            if (thoughts.memories.NumMemoriesInGroup(this) >= 1)
            {
                Thought_MealTemp thought_Memory = thoughts.memories.OldestMemoryInGroup(this) as Thought_MealTemp;
                if (thought_Memory != null)
                {
                    int moodVal = this.GetMoodValue();
                    if (moodVal == 0) // this has 0 value, don't add it
                    {
                        showBubble = false;
                    }
                    else if (thought_Memory.GetMoodValue() == moodVal) // other value is not 0 and equals this value
                    {
                        showBubble = (thought_Memory.age > thought_Memory.def.DurationTicks / 2);
                        thought_Memory.Renew();
                    }
                    else // different values, this one is non-zero
                    {
                        thoughts.memories.RemoveMemory(thought_Memory);
                        thoughts.memories.Memories.Add(this);
                        showBubble = true;
                    }
                    return true;
                }
            }
            showBubble = true;
            return false;
        }

        public int GetMoodValue()
        {
            if (comp == null)
                return 0;
            if (comp.PropsTemp.noHeat)
            {
                if (createdTemp <= 0)
                    return 3 * HotMealsSettings.negativeMoodDebuff;
                return 0;
            }
            int val = comp.GetPositiveMoodEffect(createdTemp);
            if (val > 0)
                return val * HotMealsSettings.positiveMoodBuff;
            val = comp.GetNegativeMoodEffect(createdTemp);
            return Math.Abs(val) * HotMealsSettings.negativeMoodDebuff;
        }

        public double createdTemp = 27;
        public CompDFoodTemperature comp = null;

    }
}
