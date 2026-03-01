using UWG.Data;

namespace UWG
{
    /// <summary>
    /// Each tick, recalculates aggregate effects from all purchased skill nodes.
    /// This avoids stacking math errors and keeps the pipeline stateless.
    /// </summary>
    public class SkillEffectProcessor
    {
        public void RecalculateEffects(GameState state)
        {
            // Reset all per-tick aggregates
            state.TickDiscomfort = 0f;
            state.TickHumiliation = 0f;
            state.TickMobilityReduction = 0f;
            state.TickIntellectReduction = 0f;
            state.TickStaminaReduction = 0f;
            state.TickFinancialDrain = 0f;
            state.TickGestationSpeedBonus = 0f;
            state.TickBiomassBonus = 0f;
            state.TaskFailureChanceBonus = 0f;
            state.CancelIntervention = false;
            state.ActiveVisualFlags.Clear();

            foreach (var node in state.PurchasedSkills)
            {
                foreach (var effect in node.effects)
                {
                    ApplyEffect(state, effect);
                }
            }
        }

        private void ApplyEffect(GameState state, SkillEffect effect)
        {
            if (!effect.isPerTick) return; // One-time effects applied at purchase

            switch (effect.effectType)
            {
                case SkillEffectType.AddDiscomfort:
                    state.TickDiscomfort += effect.magnitude;
                    break;
                case SkillEffectType.AddHumiliation:
                    state.TickHumiliation += effect.magnitude;
                    break;
                case SkillEffectType.ReduceMobility:
                    state.TickMobilityReduction += effect.magnitude;
                    break;
                case SkillEffectType.ReduceIntellect:
                    state.TickIntellectReduction += effect.magnitude;
                    break;
                case SkillEffectType.ReduceStamina:
                    state.TickStaminaReduction += effect.magnitude;
                    break;
                case SkillEffectType.DrainFinancial:
                    state.TickFinancialDrain += effect.magnitude;
                    break;
                case SkillEffectType.IncreaseGestationSpeed:
                    state.TickGestationSpeedBonus += effect.magnitude;
                    break;
                case SkillEffectType.IncreaseGestationDensity:
                    state.TickDiscomfort += effect.magnitude * 0.5f;
                    break;
                case SkillEffectType.RaiseGestationCap:
                    // Cap is set once at purchase, not per-tick
                    break;
                case SkillEffectType.IncreaseBiomassFromHumiliation:
                case SkillEffectType.IncreaseBiomassFromDiscomfort:
                case SkillEffectType.PassiveBiomassGeneration:
                    state.TickBiomassBonus += effect.magnitude;
                    break;
                case SkillEffectType.TaskFailureChance:
                    state.TaskFailureChanceBonus += effect.magnitude;
                    break;
                case SkillEffectType.CancelIntervention:
                    state.CancelIntervention = true;
                    break;
                case SkillEffectType.ConvertDiscomfortToEuphoria:
                    // Neutralize discomfort damage (symbiotic class mechanic)
                    state.TickDiscomfort *= (1f - effect.magnitude);
                    break;

                // Visual flags — just mark them active
                case SkillEffectType.VisualSweat:
                case SkillEffectType.VisualWardrobeFailure:
                case SkillEffectType.VisualAbdominalUndulation:
                case SkillEffectType.VisualGlowingVeins:
                case SkillEffectType.VisualSkinTranslucent:
                case SkillEffectType.VisualPetitification:
                case SkillEffectType.VisualSphericalBelly:
                    state.ActiveVisualFlags.Add(effect.effectType);
                    break;
            }
        }
    }
}
