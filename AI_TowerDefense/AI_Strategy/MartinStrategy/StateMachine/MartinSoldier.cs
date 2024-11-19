using GameFramework;

namespace AI_Strategy {
public class MartinSoldier : Soldier
{
    public bool ShouldWalk;
    
    public override void Move()
    {
        if (ShouldWalk)
        {
            PerformDefaultMovement();
        }
        else
        {
            if(PosY < 0)
                WalkForward();
        }
    }

    private void PerformDefaultMovement()
    {
        if (speed <= 0 || posY >= PlayerLane.HEIGHT) return;
            
        int x = posX;
        int y = posY;
        for (int i = speed; i > 0; i--)
        {
            if (MoveTo(x, y + i)) return;
            if (MoveTo(x + i, y + i)) return;
            if (MoveTo(x - i, y + i)) return;
            if (MoveTo(x + i, y)) return;
            if (MoveTo(x - i, y)) return;
            if (MoveTo(x, y - i)) return;
            if (MoveTo(x - i, y - i)) return;
            if (MoveTo(x + i, y - i)) return;
        }
    }

    private void WalkForward()
    {
        if (speed <= 0 || posY >= PlayerLane.HEIGHT) return;
            
        int x = posX;
        int y = posY;
        for (int i = speed; i > 0; i--)
        {
            MoveTo(x, y + i);
        }
    }
}
}