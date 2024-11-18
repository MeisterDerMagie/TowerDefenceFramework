using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_TowerDefense.Steinhauer
{
    public class UtilityAction 
    {
        public Consideration Consideration;

        public UtilityAction(Consideration consideration)
        {
            Consideration = consideration;
        }

        public float CalculateUtility(Context context) => Consideration.Evaluate(context);
    }
}
