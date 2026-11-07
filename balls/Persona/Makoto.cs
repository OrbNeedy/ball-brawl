using Godot;
using Godot.Collections;
using System;

public partial class Makoto : Ball
{
    [Export]
    public AudioStream Evoker;
    [Export]
    public Array<PersonaData> PersonaRotation;
    public int CurrentPersona = 0;
    public Stand Persona = null;
    [Export]
    public double MaxPersonaUseCooldown = 12;
    public double PersonaUseCooldown = 0;
    [Export]
    public double MaxPersonaOutTimer = 4;
    public double PersonaOutTimer = 0;

    public override void OnReady()
    {
        BasicCharacterballInit();
        PersonaUseCooldown = 2;
    }

    public override void OnProcess(double delta)
    {
        if (PersonaUseCooldown <= 0)
        {
            int previousPersona = CurrentPersona;
            CurrentPersona = Main.rng.RandiRange(0, PersonaRotation.Count - 1);
            if (Persona != null && IsInstanceValid(Persona))
            {
                if (previousPersona != CurrentPersona)
                {
                    Persona.QueueFree();
                    Persona = null;
                    SpawnPersona(PersonaRotation[CurrentPersona]);
                } else
                {
                    Persona.StandOut = true;
                    PersonaOutTimer = MaxPersonaOutTimer;
                }
            } else
            {
                SpawnPersona(PersonaRotation[CurrentPersona]);
            }
            SoundEffectManager.Instance.PlaySound(Evoker, Position);

            PersonaUseCooldown = MaxPersonaUseCooldown;
        } else
        {
            PersonaUseCooldown -= delta;
        }

        if (PersonaOutTimer > 0)
        {
            PersonaOutTimer -= delta;
        }

        if (Persona != null && IsInstanceValid(Persona))
        {
            Persona.StandOut = PersonaOutTimer > 0;
        }
    }

    public void SpawnPersona(PersonaData data)
    {
        Power = data.Power;
        Stand persona = data.Persona.Instantiate<Stand>();

        persona.SetOwner(this);
        persona.StandOut = true;
        EmitSignal(SignalName.SpawnProjectile, persona);

        Persona = persona;
        PersonaOutTimer = MaxPersonaOutTimer;
    }

    public PersonaData ValidateCurrentPersona()
    {
        if (CurrentPersona >= 0 && CurrentPersona < PersonaRotation.Count)
        {
            return PersonaRotation[CurrentPersona];
        }
        return null;
    }

    public override DamageModification ModifyContactDamage(Ball ball, DamageData data)
    {
        DamageModification modifier = new();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            modifier.CompoundModification(
                new(-persona.ContactDamageReduction, -persona.ContactDamageMultiplicativeReduction)
            );
        }

        return modifier;
    }

    public override DamageModification ModifyProjectileDamage(Projectile projectile, DamageData data)
    {
        DamageModification modifier = new();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            modifier.CompoundModification(
                new(-persona.ProjectileDamageReduction, -persona.ProjectileDamageMultiplicativeReduction)
            );
        }

        return modifier;
    }

    public override DamageModification ModifyWeaponDamage(Weapon weapon, DamageData data)
    {
        DamageModification modifier = new();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            modifier.CompoundModification(
                new(-persona.WeaponDamageReduction, -persona.WeaponDamageMultiplicativeReduction)
            );
        }

        return modifier;
    }

    public override DamageModification ModifyAllDamage(DamageData data)
    {
        DamageModification modifier = new();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            modifier.CompoundModification(
                new(-persona.DamageReduction, -persona.DamageMultiplicativeReduction)
            );
        }

        return modifier;
    }

    public override DamageModification ModifyDealtContactDamage(Ball ball)
    {
        DamageModification modifier = new();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            modifier.CompoundModification(
                new(persona.ContactDamage, -persona.ContactDamageMultiplicative)
            );
        }

        return modifier;
    }

    public override double GetMaxTimeResistance()
    {
        double finalValue = base.GetMaxTimeResistance();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            finalValue += persona.MaxTimeStopResistanceModification;
        }

        return finalValue;
    }

    public override float GetMaxSpeedGain()
    {
        float finalValue = base.GetMaxSpeedGain();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            finalValue += persona.MaxSpeedGainModification;
        }

        return finalValue;
    }

    public override float GetSpeedGain()
    {
        float finalValue = base.GetSpeedGain();
        PersonaData persona = ValidateCurrentPersona();
        if (persona != null)
        {
            finalValue += persona.SpeedGainModification;
        }

        return finalValue;
    }

    public override bool PreDead()
    {
        if (Persona != null && IsInstanceValid(Persona))
        {
            bool resurrect = Persona.PreOwnerDeath();
            //GD.Print("Persona's stance on resurrection: " + resurrect);
            return resurrect;
        }
        //GD.Print("Not resurrecting today");

        return base.PreDead();
    }
}
