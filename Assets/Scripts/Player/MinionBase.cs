using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Mininon type will indicate how the minion is supposed to move
    /// </summary>
    public enum MinionType
    {
        Straighty,
        Crossy,
        Hesitant,
        Random
    }

    public class MinionBase
    {

        internal float health = 2;


       internal float healthPoints = 7;
       internal float speed = 0.5f;
       internal float anugularSpeed = 700;
       internal float acceleration = 80; // instant

        private float speedModifier = .04f;
        private float healthModifier = .4f;

        public MinionBase() {
            health = healthPoints;
        }

        public void SetSpeedbaseOnLevel()
        {
            if (speed >= 8) return;
            speed = GV.Singleton().level * speedModifier + speed;
        }
        
        public void SetHealthBasedOnLevel()
        {
            health = healthPoints = GV.Singleton().level * healthModifier + healthPoints;
        }
    }
}
