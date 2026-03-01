using UnityEngine;
using UWG.Data;

namespace UWG
{
    /// <summary>
    /// Subscribes to game events and generates contextual narrative text
    /// using string interpolation based on Host archetype, current stats,
    /// and active skill effects. Outputs via GameEvents.FireEventLogEntry.
    /// </summary>
    public class EventLogGenerator : MonoBehaviour
    {
        private GameState State => GameManager.Instance?.State;

        private void OnEnable()
        {
            GameEvents.OnSkillPurchased += OnSkillPurchased;
            GameEvents.OnHostStateChanged += OnHostStateChanged;
            GameEvents.OnGestationChanged += OnGestationMilestone;
        }

        private void OnDisable()
        {
            GameEvents.OnSkillPurchased -= OnSkillPurchased;
            GameEvents.OnHostStateChanged -= OnHostStateChanged;
            GameEvents.OnGestationChanged -= OnGestationMilestone;
        }

        private float _lastMilestone;

        private void OnSkillPurchased(SkillNodeData node)
        {
            if (State == null) return;

            // Generate contextual narrative based on the node's effects
            foreach (var effect in node.effects)
            {
                string narrative = GenerateEffectNarrative(effect, node);
                if (!string.IsNullOrEmpty(narrative))
                    GameEvents.FireEventLogEntry(narrative);
            }
        }

        private void OnHostStateChanged(HostState oldState, HostState newState)
        {
            // Already handled in HostAIController for base transitions.
            // This hook is for visual-layer flavor text.
            if (State == null) return;

            if (newState == HostState.Bedridden && State.ActiveVisualFlags.Contains(SkillEffectType.VisualSkinTranslucent))
            {
                GameEvents.FireEventLogEntry(
                    $"The taut, translucent skin of {State.SelectedHost.hostName}'s abdomen " +
                    "shines under the fluorescent lights as she lies still.");
            }
        }

        private void OnGestationMilestone(float gestation)
        {
            if (State == null) return;

            // Fire narrative at phase boundaries
            if (gestation >= 15f && _lastMilestone < 15f)
            {
                GameEvents.FireEventLogEntry(
                    $"Phase shift: {State.SelectedHost.hostName}'s condition is now visibly apparent.");
                _lastMilestone = 15f;
            }
            else if (gestation >= 41f && _lastMilestone < 41f)
            {
                GameEvents.FireEventLogEntry(
                    $"Phase shift: {State.SelectedHost.hostName} can no longer conceal her state. " +
                    "The burden is undeniable.");
                _lastMilestone = 41f;
            }
            else if (gestation >= 76f && _lastMilestone < 76f)
            {
                GameEvents.FireEventLogEntry(
                    $"TERMINAL PHASE: {State.SelectedHost.hostName} has entered the final stage. " +
                    "Every step is a labored ordeal.");
                _lastMilestone = 76f;
            }
        }

        private string GenerateEffectNarrative(SkillEffect effect, SkillNodeData node)
        {
            if (State == null) return null;
            string host = State.SelectedHost.hostName;

            return effect.effectType switch
            {
                SkillEffectType.VisualSweat =>
                    $"A permanent sheen of perspiration covers {host}'s skin. " +
                    "She dabs at her brow constantly, but the glistening never fades.",

                SkillEffectType.VisualWardrobeFailure =>
                    $"A button gives way on {host}'s blouse with an audible pop. " +
                    "Her wardrobe can no longer contain the expansion.",

                SkillEffectType.VisualAbdominalUndulation =>
                    $"Something shifts violently beneath the taut surface of {host}'s abdomen. " +
                    "She gasps and clutches her midsection as the movement ripples outward.",

                SkillEffectType.VisualGlowingVeins =>
                    $"Faint, luminous veins trace patterns across {host}'s distended belly, " +
                    "pulsing with an otherworldly rhythm.",

                SkillEffectType.VisualSkinTranslucent =>
                    $"The skin of {host}'s abdomen becomes impossibly taut and translucent, " +
                    "stretched like a drum over the massive burden within.",

                SkillEffectType.VisualPetitification =>
                    $"{host}'s frame seems to shrink around the growing burden. " +
                    "Her limbs thin, her shoulders narrow — only the belly remains immense.",

                SkillEffectType.VisualSphericalBelly =>
                    $"{host}'s abdomen rounds into a perfect, taut sphere — " +
                    "geometrically flawless and impossibly large.",

                SkillEffectType.CancelIntervention =>
                    $"A wave of chemically induced calm washes over {host}. " +
                    "The thought of seeking help dissolves into warm compliance.",

                SkillEffectType.DeleteAuthority =>
                    $"NPCs no longer address {host} by name. Their eyes track only " +
                    "the immense curvature of her midsection.",

                _ => null
            };
        }
    }
}
