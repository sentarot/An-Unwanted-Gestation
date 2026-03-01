using UnityEngine;

namespace UWG
{
    /// <summary>
    /// Manages the player's Biomass economy. Biomass is generated
    /// each tick from base rate + host suffering multipliers.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private GameState _state;

        public void Initialize(GameState state)
        {
            _state = state;
        }

        public void GenerateBiomass(GameState state)
        {
            _state = state;

            float socialMult = state.SocialStanding / 100f;

            float fromHumiliation = state.Humiliation
                * GameConstants.HUMILIATION_BIOMASS_MULT
                * socialMult;

            float fromDiscomfort = state.Discomfort
                * GameConstants.DISCOMFORT_BIOMASS_MULT;

            float total = GameConstants.BASE_BIOMASS_PER_TICK
                        + fromHumiliation
                        + fromDiscomfort
                        + state.TickBiomassBonus;

            state.Biomass += total;
            GameEvents.FireBiomassChanged(state.Biomass);
        }
    }
}
