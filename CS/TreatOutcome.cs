using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class TreatOutcome : IPart
    {
        public static Effect CreateEffectByName(string name)
        {
            return Activator.CreateInstance(ModManager.ResolveType("XRL.World.Effects." + name))
                as Effect;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "OnEat");
            base.Register(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetInventoryActionsAlwaysEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(GetInventoryActionsAlwaysEvent E)
        {
            E.AddAction(
                "Eat",
                "eat",
                "Eat",
                null,
                'e',
                FireOnActor: false,
                Default: 100,
                Priority: 0,
                Override: true
            );
            E.AddAction(
                "Feed To",
                "feed to",
                "Feed To",
                null,
                'f',
                FireOnActor: false,
                Default: -1,
                Priority: 0,
                Override: true
            );
            return base.HandleEvent(E);
        }

        //the meat of the the variable effect is here, this gets a value for the bonus, finds who ate the treat, and rolls from the population tables. then it shuffles random mutations
        public override bool FireEvent(Event E)
        {
            if (E.ID == "OnEat")
            {
                var Bonus = (1);
                var eater = E.GetGameObjectParameter("Eater");
                var result = PopulationManager.RollOneFrom("TreatOutcome");
                eater.PermuteRandomMutationBuys();
                if (result.Hint == "Stat")
                {
                    eater.GetStat(result.Blueprint).BaseValue += Bonus;
                    return true;
                }
                if (result.Hint == "Onset")
                {
                    eater.ApplyEffect(CreateEffectByName(result.Blueprint));
                    return true;
                }
                if (result.Hint == "Disease")
                {
                    eater.ApplyEffect(CreateEffectByName(result.Blueprint));
                    return true;
                }
            }
            return base.FireEvent(E);
        }
    }
}
