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

    public override DamageData GetContactDamage(Ball ball)
    {
        DamageData finalDamage = (DamageData)base.GetContactDamage(ball).Duplicate();
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalDamage.BaseDamage += persona.ContactDamage;
        }

        return finalDamage;
    }

    public override int ModifyContactDamage(DamageData data)
    {
        int finalDamage = base.ModifyContactDamage(data);
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalDamage -= persona.ContactDamageReduction;
        }

        return int.Clamp(finalDamage, 0, 9999);
    }

    public override int ModifyWeaponDamage(DamageData data, Weapon weapon)
    {
        int finalDamage = base.ModifyWeaponDamage(data, weapon);
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalDamage -= persona.WeaponDamageReduction;
        }

        return int.Clamp(finalDamage, 0, 9999);
    }

    public override int ModifyProjectileDamage(DamageData data)
    {
        int finalDamage = base.ModifyProjectileDamage(data);
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalDamage -= persona.ProjectileDamageReduction;
        }

        return int.Clamp(finalDamage, 0, 9999);
    }

    public override int ModifyAllDamage(DamageData data)
    {
        int finalDamage = base.ModifyAllDamage(data);
        PersonaData persona = CurrentPersonaData;
        if (persona != null)
        {
            finalDamage -= persona.DamageReduction;
        }

        return int.Clamp(finalDamage, 0, 9999);
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
