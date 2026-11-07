using Godot;
using System;

public partial class MakotoEx : Ball
{
    [Export]
    public FunctioningAnimatedSprite BackAura;
    [Export]
    public FunctioningAnimatedSprite FrontAura;

    [Export]
    public AudioStream Evoker;

    [Export]
    public PersonaData[] Phase1Personas;
    public int Persona = 0;
    [Export]
    public PersonaData MessiahData;

    public PersonaData CurrentPersonaData = null;
    public Stand CurrentPersona = null;

    [Export]
    public double MaxPersonaSummonTimer = 8;
    public double PersonaSummonTimer = 2;

    [Export]
    public double MaxPersonaOnTime = 6;
    public double PersonaOnTime = 0;

    [Export]
    public bool PhaseTwoActive = false;

    public override void PreProcess(double delta)
    {
        BackAura.ResistsTime = !AffectedByTimeStop;
        FrontAura.ResistsTime = !AffectedByTimeStop;
    }

    public override void OnProcess(double delta)
    {
        if (PhaseTwoActive)
        {
            PhaseTwo(delta);
        } else
        {
            PhaseOne(delta);
        }
    }

    public void PhaseOne(double delta)
    {
        if ((float)life / (float)MaxLife <= 0.25)
        {
            PhaseTwoActive = true;
            SummonPersona();
            return;
        }

        if (PersonaSummonTimer <= 0)
        {
            SummonPersona();
            PersonaSummonTimer = MaxPersonaSummonTimer;
            PersonaOnTime = 0;
        } else
        {
            PersonaSummonTimer -= delta;
        }

        if (CurrentPersona != null && IsInstanceValid(CurrentPersona))
        {
            CurrentPersona.StandOut = PersonaOnTime <= MaxPersonaOnTime;
        }

        PersonaOnTime += delta;
    }

    public void PhaseTwo(double delta)
    {
        CurrentPersona.StandOut = true;
    }

    public void SummonPersona()
    {
        Persona++; 
        if (Persona >= Phase1Personas.Length) Persona = 0;

        PersonaData targetData = Phase1Personas[Persona];
        if (PhaseTwoActive)
        {
            targetData = MessiahData;
        }

        Power = targetData.Power;
        Stand persona = targetData.Persona.Instantiate<Stand>();

        persona.SetOwner(this);
        persona.StandOut = true;
        EmitSignal(SignalName.SpawnProjectile, persona);

        SoundEffectManager.Instance.PlaySound(Evoker, Position);

        if (CurrentPersona != null && IsInstanceValid(CurrentPersona))
        {
            CurrentPersona.QueueFree();
            CurrentPersona = null;
        }
        CurrentPersonaData = targetData;
        CurrentPersona = persona;
    }

    public override DamageModification ModifyContactDamage(Ball ball, DamageData data)
    {
        DamageModification modifier = new();
        PersonaData persona = CurrentPersonaData;
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
        PersonaData persona = CurrentPersonaData;
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
        PersonaData persona = CurrentPersonaData;
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
        PersonaData persona = CurrentPersonaData;
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
        PersonaData persona = CurrentPersonaData;
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
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalValue += persona.MaxTimeStopResistanceModification;
        }

        return finalValue;
    }

    public override float GetMaxSpeedGain()
    {
        float finalValue = base.GetMaxSpeedGain();
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalValue += persona.MaxSpeedGainModification;
        }

        return finalValue;
    }

    public override float GetSpeedGain()
    {
        float finalValue = base.GetSpeedGain();
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalValue += persona.SpeedGainModification;
        }

        return finalValue;
    }

    public override bool PreDead()
    {
        if (CurrentPersona != null && IsInstanceValid(CurrentPersona))
        {
            bool resurrect = CurrentPersona.PreOwnerDeath();
            //GD.Print("Persona's stance on resurrection: " + resurrect);
            return resurrect;
        }
        //GD.Print("Not resurrecting today");

        return base.PreDead();
    }
}
