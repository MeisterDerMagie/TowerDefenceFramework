using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_TowerDefense.Steinhauer
{
    public class Context
    {
        public Dictionary<string, int> ConsiderationValues = new();

        public Context()
        {
            ConsiderationValues.Add("cost", 0);
            ConsiderationValues.Add("gold", 0);
            ConsiderationValues.Add("goldAfterPurchase", 0);
            ConsiderationValues.Add("enemySoldiers", 0);
            ConsiderationValues.Add("ownSoldiers", 0);
            ConsiderationValues.Add("enemyTowers", 0);
            ConsiderationValues.Add("ownTowers", 0);
            ConsiderationValues.Add("enemyHealth", 0);
            ConsiderationValues.Add("ownHealth", 0);
            ConsiderationValues.Add("posX", 0);
            ConsiderationValues.Add("posY", 0);
        }
    }
}
