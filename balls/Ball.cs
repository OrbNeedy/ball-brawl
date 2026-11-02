using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum DamageTakenType
{
    None, 
    HBB, 
    Lenen
}

public enum PowerType
{
    None, 
    TimeStop, 
    Cut, 
    Fire, 
    Electricity, 
    Healing, 
    Magic
}

public partial class Ball : RigidBody2D
{
    [Signal]
    public delegate void DamageTakenEventHandler(float newLifePercent, int damageTaken, Ball ball);
    [Signal]
    public delegate void BallDeadEventHandler(Ball ball);
    [Signal]
    public delegate void SpawnProjectileEventHandler(Projectile node);
    [Signal]
    public delegate void SpawnBallEventHandler(Ball node);
    [Signal]
    public delegate void SpawnParticlesEventHandler(SelfDeletingParticles node);
    [Signal]
    public delegate void SpawnNodeEventHandler(Node node);

    // Children
    [Export]
    public AnimatedSprite2D Body;
    [Export]
    public Sprite2D Crown;
    [Export]
    public Reticle Reticle;

    // Physics related
    [Export]
    public float SpeedGain = 0;
    [Export]
    public float MaxSpeedGain = 0;
    [Export]
    public string TeamName = "";

    // Identifiers
    [Export]
    public PowerType Power = PowerType.None;
    [Export]
    public Color Color = Colors.White;
    [Export]
    public string BallName = "Name";
    [Export]
    public bool TeamOutline = false;
    [Export]
    public DamageEffect DamageEffectsType;
    [Export]
    public MusicTheme BackgroundTheme;
    [Export]
    public EntranceData Entrance;
    [Export]
    public bool Boss = false;

    // Stats related
    [Export]
    public DamageTakenType DamagedType = DamageTakenType.None;
    [Export]
    public DamageData ContactDamage;
    [Export]
    public int MaxLife = 100;
    [Export]
    public double StunFrames = 0;
    public int life = 100;
    public int oldLife = 100;
    [Export]
    public double MaxTimeResistance = 0;
    public double TimeResistance = 0;

    // State
    public double iFrames = 0;
    public bool OldFreeze = false;
    public Vector2 OldLinearVelocity = Vector2.Zero;
    /// <summary>
    /// Time is stopped
    /// </summary>
    public bool TimeStopped { get => GlobalTimeState.Instance.TimeStopDuration > 0; }
    /// <summary>
    /// Time is stopped and ball is not resistant
    /// </summary>
    public bool AffectedByTimeStop { get => TimeStopped && TimeResistance <= 0; }
    /// <summary>
    /// Forces queued to act upon the ball after time resumes, does not queue if the ball is resistant
    /// </summary>
    public Vector2 ForcesAppliedDuringTimeStop = Vector2.Zero;
    /// <summary>
    /// Damage queued to be dealt to the ball after time resumes, does not queue if the ball is resistant
    /// </summary>
    public int DamageAppliedDuringTimeStop = 0;
    [Export]
    public StatusEffect[] InitialStatusEffects = new StatusEffect[0];
    public Dictionary<StatusEffectCategory, StatusEffect> StatusEffects = new();
    public Dictionary<StatusEffectCategory, SelfDeletingParticles> StatusParticles = new();

    float acceleration = 0;
    public List<Ball> DetectedBalls = new();

    public override void _Ready()
    {
        if (TeamName == "")
        {
            TeamName = BallName;
        }
        AddToGroup(TeamName);
        Reticle.BallOwner = this;

        life = MaxLife;
        TimeResistance = MaxTimeResistance;
        Body.Modulate = Color;
        BasicCharacterballInit();

        LinearVelocity = new Vector2(300, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
        foreach (StatusEffect effect in InitialStatusEffects)
        {
            AddStatus(effect);
        }

        OnReady();
    }

    /// <summary>
    /// Runs after setup is finished
    /// </summary>
    public virtual void OnReady()
    {
    }

    public override void _EnterTree()
    {
        OnEnterTree();
    }

    public virtual void OnEnterTree()
    {
    }

    public void BasicCharacterballInit()
    {
        Body.Modulate = Colors.White;
    }

    public override void _Draw()
    {
        if (TeamOutline)
        {
            float radius = Body.SpriteFrames.GetFrameTexture(Body.Animation, Body.Frame).GetWidth() / 2f;
            DrawCircle(Body.Offset, (radius * Body.Scale.X) + 4f, Color, antialiased: true);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Rotation = 0;
        if (life <= 0)
        {
            Freeze = true;
            CollisionLayer = 0;
            CollisionMask = 0;

            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)delta);
            if (Modulate.A <= 0)
            {
                QueueFree();
            }
            return;
        }

        if (!Freeze)
        {
            if (OldFreeze)
            {
                LinearVelocity = OldLinearVelocity;
            }

            OldLinearVelocity = LinearVelocity;
        }

        if (!AffectedByTimeStop)
        {   
            if (ForcesAppliedDuringTimeStop != Vector2.Zero)
            {
                LinearVelocity += ForcesAppliedDuringTimeStop;
                ForcesAppliedDuringTimeStop = Vector2.Zero;
            }

            NormalProcess(delta);
        } else
        {
            Freeze = true;
        }

        // Clamp resistance so it doesn't make a mess
        TimeResistance = double.Clamp(TimeResistance, 0, GetMaxTimeResistance());
    }

    public void NormalProcess(double delta)
    {
        if (GetMaxSpeedGain() > 0 && GetSpeedGain() > 0)
        {
            ConstantForce = LinearVelocity.Normalized() * acceleration;
            
            acceleration = float.Clamp(acceleration + GetSpeedGain(), 0, GetMaxSpeedGain());
        } else
        {
            ConstantForce *= 0.9f;
        }
    
        if (float.Abs(LinearVelocity.X) <= 5)
        {
            float dir = 1500;
            if (Main.rng.Randf() < 0.5)
            {
                dir *= -1;
            }
            AddConstantForce(new Vector2(dir, 0));
        }

        OnPhysics(delta);

        if (TimeStopped)
        {
            // Reduce resistance while time is stopped
            TimeResistance -= delta;
        } else
        {
            // Recover resistance while time is not stopped 
            if (TimeResistance < GetMaxTimeResistance())
            {
                TimeResistance += delta * 2f;
            }
        }
    }

    /// <summary>
    /// Runs after all base movement is executed if time is not stopped
    /// </summary>
    /// <param name="delta"></param>
    public virtual void OnPhysics(double delta)
    {
        
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (life <= 0)
        {
            return;
        }

        Transform2D trans = state.Transform;
        
        Transform2D transformation = new Transform2D(trans.Rotation, trans.Scale, 
            trans.Skew, trans.Origin);

        if (!AffectedByTimeStop)
        {
            ModifyForcesTransform(ref transformation, state);
        }

        state.Transform = transformation;
    }

    public virtual void ModifyForcesTransform(ref Transform2D transformation, PhysicsDirectBodyState2D state)
    {
    }

    public override void _Process(double delta)
    {
        if (life <= 0)
        {
            return;
        }

        foreach (StatusEffectCategory category in StatusEffects.Keys)
        {
            if (StatusEffects[category].TimeLeft > 0)
            {
                StatusEffects[category].Update(delta, this);
            }

            if (StatusParticles.ContainsKey(category) && StatusParticles[category] != null)
            {
                StatusParticles[category].TimeLeft = StatusEffects[category].TimeLeft;
            }
        }

        OldFreeze = Freeze;

        PreProcess(delta);
        if (!AffectedByTimeStop)
        {
            if (DamageAppliedDuringTimeStop > 0)
            {
                DamageBall(new DamageData() 
                { 
                    BaseDamage = DamageAppliedDuringTimeStop, 
                    IFrames = 0.5, 
                    Almighty = true, 
                    CanBeDodged = false
                });
                DamageAppliedDuringTimeStop = 0;
            }

            if (StunFrames > 0)
            {
                StunFrames -= delta;

                if (DamagedType == DamageTakenType.HBB)
                {
                    if (StunFrames <= 0)
                    {
                        if (Body != null) Body.Offset = new Vector2(0, 0);
                    } else
                    {
                        if (Body != null) Body.Offset = new Vector2(Main.rng.RandfRange(-10, 10), Main.rng.RandfRange(-10, 10));
                    }
                }
            }

            Freeze = StunFrames > 0 && DamagedType == DamageTakenType.HBB;

            OnProcess(delta);
        } else
        {
        }

        if (iFrames > 0)
        {
            iFrames -= delta;
        }
    }


    /// <summary>
    /// Runs after the base process is executed if time is not stopped
    /// </summary>
    /// <param name="delta"></param>
    public virtual void OnProcess(double delta)
    {
        
    }

    /// <summary>
    /// Runs before any virtual Process is run, regardless of time stop
    /// </summary>
    /// <param name="delta"></param>
    public virtual void PreProcess(double delta)
    {
        
    }

    public void AddStatus(StatusEffect effect)
    {
        if (StatusEffects.ContainsKey(effect.Category) && StatusEffects[effect.Category].TimeLeft > 0)
        {
            if (effect.EffectPriority >= StatusEffects[effect.Category].EffectPriority)
            {
                StatusEffects[effect.Category] = effect;
            }
            StatusEffects[effect.Category].Stacks++;
            StatusEffects[effect.Category].OnReInflict(this);
            
            if (StatusParticles.ContainsKey(effect.Category))
            {
                if (effect.Particles != null)
                {
                    SelfDeletingParticles instance = effect.Particles.Instantiate<SelfDeletingParticles>();
                    instance.TimeLeft = effect.MaxTimeLeft;
                    
                    AddParticles(effect.Category, instance);
                } else
                {
                    if (StatusParticles[effect.Category] != null && IsInstanceValid(StatusParticles[effect.Category]))
                    {
                        StatusParticles[effect.Category].TimeLeft = 0;
                        StatusParticles[effect.Category] = null;
                    }
                }
            }

            return;
        }

        if (effect.Particles != null)
        {
            SelfDeletingParticles instance = effect.Particles.Instantiate<SelfDeletingParticles>();
            instance.TimeLeft = effect.MaxTimeLeft;
            
            AddChild(instance);
            AddParticles(effect.Category, instance);
        }
        
        effect.OnInflict(this);
        effect.Tick = effect.MaxTick;
        StatusEffects.Add(effect.Category, effect);
    }

    public void AddParticles(StatusEffectCategory category, SelfDeletingParticles particle)
    {
        if (StatusParticles.ContainsKey(category))
        {
            if (StatusParticles[category] != null && IsInstanceValid(StatusParticles[category]))
            {
                StatusParticles[category].TimeLeft = 0;
                StatusParticles[category] = null;
            }
            StatusParticles[category] = particle;
        }else
        {
            StatusParticles.Add(category, particle);
        }
    }

    public virtual int ModifyAllDamage(DamageData data)
    {
        return data.BaseDamage;
    }

    public virtual int ModifyContactDamage(DamageData data)
    {
        return data.BaseDamage;
    }

    public virtual int ModifyProjectileDamage(DamageData data)
    {
        return data.BaseDamage;
    }

    public virtual int ModifyWeaponDamage(DamageData data, Weapon weapon)
    {
        return data.BaseDamage;
    }

    public void OnBodyEntered(Node body)
    {
        //GD.Print(BallName + " collided (OnBodyEntered) with " + body.Name);
        if (AffectedByTimeStop)
        {
            if (body is Ball unaffectedEnemy && !unaffectedEnemy.AffectedByTimeStop)
            {
                //GD.Print("Adding ball forces");
                OldLinearVelocity += unaffectedEnemy.LinearVelocity * 0.8f;
            }

            if (body is Weapon unfrozenWeapon && !unfrozenWeapon.Freeze)
            {
                //GD.Print("Adding weapon forces");
                OldLinearVelocity += unfrozenWeapon.LinearVelocity * 0.8f;
            }
        }
        
        if (body.IsInGroup(TeamName)) return;

        if (iFrames > 0) return;

        if (body is Ball enemy)
        {
            BallCollision(enemy);
        }

        if (body is Weapon weapon)
        {
            //GD.Print("Damage (Ball): " + weapon.Damage.BaseDamage);
            if (weapon.Freeze) return;

            bool dodged = !PreWeaponCollision(weapon) && weapon.Damage.CanBeDodged;

            if (!dodged)
            {
                LinearVelocity += weapon.LinearVelocity * 0.5f;

                int baseDamage = weapon.GetWeaponDamage(this);
                int finalDamage = ModifyWeaponDamage(weapon.Damage, weapon);
                if (weapon.Damage.Almighty && finalDamage < baseDamage)
                {
                    finalDamage = baseDamage;
                }
                DamageBall(weapon.Damage);

                if (DamagedType == DamageTakenType.HBB)
                {
                    SetStun(0.5);
                }
            }
        }
    }

    public virtual bool PreWeaponCollision(Weapon weapon)
    {
        return true;
    }

    public void ProjectileCollision(Projectile projectile, DamageData data)
    {
        // No stun or speed change for projectiles, but no iframes
        //GD.Print("Bullet can be dodged: ");
        if (!PreProjectileCollision(projectile, data) && data.CanBeDodged)
        {
            return;
        }
        
        int finalDamage = ModifyProjectileDamage(data);
        if (data.Almighty && finalDamage < data.BaseDamage)
        {
            finalDamage = data.BaseDamage;
        }
        DamageBall(data);
    }

    public virtual bool PreProjectileCollision(Projectile projectile, DamageData data)
    {
        return true;
    }

    /// <summary>
    /// Runs after the sync function is run, only when the ball is calculated to take damage but before the damage is taken
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns>False to cancel the damage entirely</returns>
    public virtual bool PreBallCollision(Ball enemy)
    {
        return true;
    }

    public virtual DamageData GetContactDamage(Ball ball)
    {
        return ContactDamage;
    }

    public void BallCollision(Ball enemy)
    {
        //GD.Print(BallName + " collided (BallCollision) with " + enemy.BallName + " for " + enemy.ContactDamage);
        
        SyncBallCollision(enemy, enemy.LinearVelocity.Length(), LinearVelocity.Length());
        
        // Only really needed when both balls have melee capabilities, to sync them up
        if (GetContactDamage(enemy).BaseDamage > 0 && enemy.GetContactDamage(this).BaseDamage > 0)
        {
            // BallName + " attempting sync (BallCollision) with " + enemy.BallName + " with damages " 
            /*GD.Print($"{BallName} ({GetContactDamage(this)} damage) attempting sync (BallCollision2) " + 
                $"with {enemy.BallName} ({GetContactDamage(enemy)} damage)");*/
            enemy.SyncBallCollision(this, LinearVelocity.Length(), enemy.LinearVelocity.Length());
        }
    }

    public void SyncBallCollision(Ball enemy, float velocity, float myVelocity)
    {
        //GD.Print(BallName + " syncing (SyncBallCollision) damage with " + enemy.BallName + " for " + enemy.ContactDamage);
        bool slower = myVelocity <= velocity;
        bool equal = myVelocity == velocity;
        bool stun = false;
        DamageData finalDamage = enemy.GetContactDamage(this);
        //GD.Print($"{enemy.BallName}'s final damage is {finalDamage.BaseDamage}");
        
        if (finalDamage.BaseDamage > 0 && !enemy.AffectedByTimeStop)
        {
            if (slower || GetContactDamage(enemy).BaseDamage <= 0 || AffectedByTimeStop)
            {
                // GD.Print($"{enemy.BallName} was faster");
                if (!PreBallCollision(enemy) && enemy.ContactDamage.CanBeDodged) return;
                // Slower ball gets damaged, but doesn't lose speed
                int damage = ModifyContactDamage(finalDamage);
                if (enemy.ContactDamage.Almighty && damage < finalDamage.BaseDamage)
                {
                    damage = finalDamage.BaseDamage;
                }

                if (equal)
                {
                    // Clash means damage gets halfed
                    damage /= 2;
                }

                DamageBall(damage, enemy.ContactDamage.IFrames);
                stun = true;
            } else
            {
                //GD.Print($"{enemy.BallName} was slower");
                // Balls with no speed gain should not get affected by this
                if (GetSpeedGain() > 0 && GetMaxSpeedGain() > 0)
                {
                    LinearVelocity *= 0.7f; 
                }
            }
        } else
        {
            // Collision against non-melee are always victory
            // To avoid blitzing the enemy, speed gets halfed
            // Balls with no speed gain should not get affected by this
            if (GetSpeedGain() > 0 && GetMaxSpeedGain() > 0)
            {
                LinearVelocity *= 0.5f; 
            }
        }

        if (stun)
        {
            SetStun(enemy.ContactDamage.IFrames);
        }
        acceleration = 0;
    }

    public void DealUnspecifiedDamage(DamageData data)
    {
        if (!PreDealUnspecifiedDamage(data) && data.CanBeDodged)
        {
            return;
        }
        DamageBall(data);
    }

    public virtual bool PreDealUnspecifiedDamage(DamageData data)
    {
        return true;
    }

    public void DamageBall(DamageData data)
    {
        int finalDamage = ModifyAllDamage(data);
        if (data.Almighty && finalDamage < data.BaseDamage)
        {
            finalDamage = data.BaseDamage;
        }
        CalculateNewLife(finalDamage, data.IFrames);
    }

    public void DamageBall(int finalDamage, double finalIFrames)
    {
        CalculateNewLife(finalDamage, finalIFrames);
    }

    public void CalculateNewLife(int damage, double IFrames)
    {
        if (AffectedByTimeStop)
        {
            DamageAppliedDuringTimeStop += damage;
            // GD.Print("Queueing damage for " + damage);
            return;
        }

        PreDamaged(damage);

        oldLife = life;
        life -= damage;
        iFrames += IFrames;

        DamageEffectsType.TakeDamage(this, damage);

        if (life <= 0)
        {
            bool resurrection = PreDead();
            if (resurrection)
            {
                //GD.Print("Not resurrecting today");
                EmitSignal(SignalName.BallDead, this);
                Body.Animation = "dead";
                ProcessMode = ProcessModeEnum.Always;
            }
        }
        EmitSignal(SignalName.DamageTaken, (float)life / (float)MaxLife, damage, this);

        OnDamaged(damage);
    }

    /// <summary>
    /// Runs after damage is taken and life is calculated to be zero or less, before the death signal is emitted.
    /// </summary>
    /// <returns>False to cancel death, take in mind life will still be reduced regardless.</returns>
    public virtual bool PreDead()
    {
        return true;
    }

    /// <summary>
    /// Happens right before the ball's life gets reduced, after the damage is modified 
    /// </summary>
    /// <param name="damage"></param>
    public virtual void PreDamaged(int damage)
    {
    }

    /// <summary>
    /// Happens after the ball's life is reduced, after the dead signal is emitted
    /// </summary>
    /// <param name="damage"></param>
    public virtual void OnDamaged(int damage)
    {
    }

    public void HealBall(int healing)
    {
        life += healing;
        life = int.Clamp(life, 0, MaxLife);
        EmitSignal(SignalName.DamageTaken, (float)life / (float)MaxLife, -healing, this);
    }

    public virtual double GetMaxTimeResistance()
    {
        return MaxTimeResistance;
    }

    public virtual float GetMaxSpeedGain()
    {
        return MaxSpeedGain;
    }

    public virtual float GetSpeedGain()
    {
        return SpeedGain;
    }

    public void SetStun(double time)
    {
        StunFrames = time;
    }

    public void SetFreeze(bool freeze)
    {
        Freeze = freeze;
    }

    public void Won()
    {
        Body.Animation = "victory";
        if (Crown != null)
        {
            Crown.Visible = true;
        }
    }

    public void AddDetectedBall(Node2D node)
    {
        if (node.IsInGroup(TeamName)) return;

        if (node is Ball ball && !DetectedBalls.Contains(ball))
        {
            DetectedBalls.Add(ball);
        }
    }

    public void RemoveDetectedBall(Node2D node)
    {
        if (node.IsInGroup(TeamName)) return;

        if (node is Ball ball && DetectedBalls.Contains(ball))
        {
            DetectedBalls.Remove(ball);
        }
    }

    public Ball GetClosestBall()
    {
        Ball ClosestBall = null;
        if (DetectedBalls.Count > 0)
        {
            ClosestBall = DetectedBalls[0];
            float distance = ClosestBall.Position.DistanceTo(Position);

            for (int i = 1; i < DetectedBalls.Count; i++)
            {
                float currentDistance = DetectedBalls[i].Position.DistanceTo(Position);

                if (currentDistance < distance)
                {
                    ClosestBall = DetectedBalls[i];
                    distance = currentDistance;
                }
            }
        }

        return ClosestBall;
    }
}
