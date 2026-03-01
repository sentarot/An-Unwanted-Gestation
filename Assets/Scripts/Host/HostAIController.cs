using UnityEngine;
using UWG.Data;

namespace UWG
{
    /// <summary>
    /// The Host's autonomous agent. Each tick it evaluates its condition,
    /// picks a state, and attempts a task from its schedule or interventions.
    /// </summary>
    public class HostAIController : MonoBehaviour
    {
        private GameState _state;
        private ScheduleEntry[] _schedule;

        public void Initialize(GameState state)
        {
            _state = state;
            _schedule = state.SelectedHost.dailySchedule;
        }

        public void ProcessTick(GameState state)
        {
            _state = state;
            EvaluateState();
            ExecuteAction();
        }

        private void EvaluateState()
        {
            HostState oldState = _state.CurrentHostState;

            // Mind-control check
            if (_state.CancelIntervention)
                _state.IsMindControlled = true;

            // State transitions based on condition meters
            if (_state.Discomfort > GameConstants.BEDRIDDEN_DISCOMFORT_THRESHOLD)
            {
                _state.CurrentHostState = HostState.Bedridden;
                _state.Mobility = 0f;
            }
            else if (_state.Humiliation > GameConstants.ISOLATED_HUMILIATION_THRESHOLD)
            {
                _state.CurrentHostState = HostState.Isolated;
            }
            else
            {
                _state.CurrentHostState = HostState.Active;
            }

            if (_state.CurrentHostState != oldState)
            {
                GameEvents.FireHostStateChanged(oldState, _state.CurrentHostState);
                string msg = _state.CurrentHostState switch
                {
                    HostState.Bedridden =>
                        $"{_state.SelectedHost.hostName} collapses under the weight. She is bedridden.",
                    HostState.Isolated =>
                        $"{_state.SelectedHost.hostName} refuses to leave the house. The humiliation is too great.",
                    HostState.Active =>
                        $"{_state.SelectedHost.hostName} forces herself back into her routine.",
                    _ => ""
                };
                if (!string.IsNullOrEmpty(msg))
                    GameEvents.FireEventLogEntry(msg);
            }
        }

        private void ExecuteAction()
        {
            switch (_state.CurrentHostState)
            {
                case HostState.Active:
                    if (ShouldSeekIntervention())
                        AttemptTask(CreateInterventionEntry());
                    else
                        AttemptScheduledTask();
                    break;

                case HostState.Isolated:
                    // Can still try intervention from home if not mind-controlled
                    if (ShouldSeekIntervention())
                        AttemptTask(CreateInterventionEntry());
                    else
                        AttemptRest();
                    break;

                case HostState.Bedridden:
                    AttemptRest();
                    break;
            }
        }

        private bool ShouldSeekIntervention()
        {
            if (_state.IsMindControlled) return false;
            if (_state.Gestation < _state.SelectedHost.panicGestationThreshold) return false;
            return _state.FinancialResources > 5f;
        }

        private void AttemptScheduledTask()
        {
            if (_schedule == null || _schedule.Length == 0) return;

            _state.ScheduleIndex = _state.TickWithinDay % _schedule.Length;
            var entry = _schedule[_state.ScheduleIndex];
            AttemptTask(entry);
        }

        private void AttemptTask(ScheduleEntry entry)
        {
            float successChance = CalculateSuccessChance(entry);
            bool succeeded = Random.value <= successChance;

            if (entry.taskType == TaskType.SeekDoctor || entry.taskType == TaskType.PrivateClinic)
            {
                if (succeeded)
                {
                    float interventionGain = GameConstants.INTERVENTION_PER_DOCTOR_VISIT
                                           * _state.SelectedHost.interventionDrive
                                           * (_state.FinancialResources / 100f);
                    _state.InterventionMeter = Mathf.Min(100f,
                        _state.InterventionMeter + interventionGain);
                    _state.FinancialResources = Mathf.Max(0f,
                        _state.FinancialResources - 5f);
                    GameEvents.FireInterventionChanged(_state.InterventionMeter);
                    GameEvents.FireEventLogEntry(
                        $"{_state.SelectedHost.hostName} visits a specialist. " +
                        $"Intervention progress: {_state.InterventionMeter:F1}%.");
                }
                else
                {
                    GameEvents.FireEventLogEntry(
                        $"{_state.SelectedHost.hostName} attempts to see a doctor but fails. " +
                        $"The condition interferes.");
                }
            }
            else
            {
                if (succeeded)
                {
                    ApplyTaskSuccess(entry);
                }
                else
                {
                    ApplyTaskFailure(entry);
                }
            }

            GameEvents.FireHostTaskResolved(entry.taskType, succeeded);
        }

        private float CalculateSuccessChance(ScheduleEntry entry)
        {
            float chance = entry.baseSuccessChance;

            // Penalties from host condition
            chance -= _state.Discomfort * GameConstants.DISCOMFORT_TASK_PENALTY_MULT;
            chance -= _state.Humiliation * GameConstants.HUMILIATION_TASK_PENALTY_MULT;

            // Penalty from player skills
            chance -= _state.TaskFailureChanceBonus;

            // Physical tasks penalized by mobility
            if (entry.category == TaskCategory.Physical)
                chance *= _state.Mobility;

            // Social tasks penalized by humiliation
            if (entry.category == TaskCategory.Social && _state.Humiliation > 50f)
                chance *= 0.6f;

            return Mathf.Clamp01(chance);
        }

        private void ApplyTaskSuccess(ScheduleEntry entry)
        {
            // Successful tasks slightly reduce humiliation (maintaining routine)
            _state.Humiliation = Mathf.Max(0f, _state.Humiliation - 1f);

            // Social tasks maintain social standing
            if (entry.category == TaskCategory.Social)
                _state.SocialStanding = Mathf.Min(100f, _state.SocialStanding + 0.5f);

            GameEvents.FireEventLogEntry(
                $"{_state.SelectedHost.hostName} completes: {FormatTaskName(entry.taskLabel)}.");
        }

        private void ApplyTaskFailure(ScheduleEntry entry)
        {
            float humGain = 3f;
            float disGain = 2f;

            // Vulnerability amplification
            if (_state.SelectedHost.vulnerability == VulnerabilityType.Humiliation)
                humGain *= _state.SelectedHost.vulnerabilityMultiplier;
            if (_state.SelectedHost.vulnerability == VulnerabilityType.Discomfort)
                disGain *= _state.SelectedHost.vulnerabilityMultiplier;

            _state.Humiliation = Mathf.Clamp(_state.Humiliation + humGain, 0f, 100f);
            _state.Discomfort = Mathf.Clamp(_state.Discomfort + disGain, 0f, 100f);
            _state.SocialStanding = Mathf.Max(0f, _state.SocialStanding - 1f);

            GameEvents.FireHumiliationChanged(_state.Humiliation);
            GameEvents.FireDiscomfortChanged(_state.Discomfort);
            GameEvents.FireSocialStandingChanged(_state.SocialStanding);

            GameEvents.FireEventLogEntry(
                $"{_state.SelectedHost.hostName} fails: {FormatTaskName(entry.taskLabel)}. " +
                $"Humiliation +{humGain:F0}, Discomfort +{disGain:F0}.");
        }

        private void AttemptRest()
        {
            // Resting recovers some discomfort but generates no intervention progress
            _state.Discomfort = Mathf.Max(0f, _state.Discomfort - 1.5f);
            GameEvents.FireDiscomfortChanged(_state.Discomfort);
        }

        private ScheduleEntry CreateInterventionEntry()
        {
            return new ScheduleEntry
            {
                taskLabel = "Seek Medical Intervention",
                taskType = TaskType.SeekDoctor,
                category = TaskCategory.Intervention,
                baseSuccessChance = 0.7f,
                primaryStat = StatAffinity.Financial
            };
        }

        private string FormatTaskName(string raw)
        {
            return raw.Replace("_", " ");
        }
    }
}
