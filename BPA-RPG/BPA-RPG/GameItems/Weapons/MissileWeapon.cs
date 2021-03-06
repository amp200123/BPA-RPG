﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using EtStellas.GameObjects;
using Microsoft.Xna.Framework.Audio;

namespace EtStellas.GameItems.Weapons
{
    public class MissileWeapon : Weapon
    {
        private static SoundEffectInstance explosion;

        private Random rand;
        private int damageBonus;

        public MissileWeapon(string name, int level, Texture2D texture, Texture2D projectileTexture, int damage, int damageBonus, int shots, int maxCooldown, float hitChance, string info = "", bool passShield = true)
            : base(name, level, texture, projectileTexture, damage, shots, maxCooldown, hitChance, 8, info, passShield)
        {
            this.damageBonus = damageBonus;

            rand = new Random();
        }

        protected override void GotHit(BattleShip target)
        {
            explosion.Pause();
            explosion.Play();

            target.hullPoints -= damage + rand.Next(damageBonus);
        }

        public static new void LoadContent(ContentManager content)
        {
            MainGame.eventLogger.Log(typeof(LaserWeapon), "Begin loading missile weapons");

            Texture2D projTex = content.Load<Texture2D>("Images/Missile");

            new MissileWeapon("Basic Missile", 1, content.Load<Texture2D>("Images/Items/Weapons/BasicMissile"), projTex,
                20, 10, 1, 15, .75f, "A basic missile weapon that can pierce shields.");
            new MissileWeapon("Heavy Missile", 3, content.Load<Texture2D>("Images/Items/Weapons/HeavyMissile"), projTex,
                15, 20, 2, 20, .6f, "A two shot missile with high damage but somewhat unrealiable.");
            

            explosion = SoundEffectManager.GetEffectInstance("Explosion1");

            MainGame.eventLogger.Log(typeof(LaserWeapon), "Finished loading missile weapons");
        }
    }
}
