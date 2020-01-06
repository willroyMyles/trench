using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player
{
    class GunBase
    {
        internal float fireRate = .8f;
        internal float damage = 0f;

        float fireratecap = .1f;



        public GunBase() { }

        public float ModifyFireRate(bool increase = true)
        {
            if (fireRate <= fireratecap) return fireRate;
            if(increase)    fireRate -= .15f;
            else    fireRate += .18f;

            return fireRate;
        }

        public float ModifyDamage(bool increase = true)
        {
            if (increase) damage += .5f;
            else damage -= .7f;
            return damage;
        }

    }
}
