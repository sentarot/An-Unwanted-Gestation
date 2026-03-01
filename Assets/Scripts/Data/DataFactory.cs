using UnityEngine;

namespace UWG.Data
{
    /// <summary>
    /// Creates all ScriptableObject data at runtime for development/testing.
    /// In production, these would be authored as .asset files in the Editor.
    /// Attach this to a GameObject in the scene and call Generate() or
    /// let it auto-generate on Awake.
    /// </summary>
    public class DataFactory : MonoBehaviour
    {
        public static DataFactory Instance { get; private set; }

        public HostProfile[] Hosts { get; private set; }
        public GestationClassData[] GestationClasses { get; private set; }
        public SkillTreeData GeneralSomatic { get; private set; }
        public SkillTreeData GeneralEndocrine { get; private set; }
        public SkillTreeData GeneralTemporal { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            Generate();
        }

        public void Generate()
        {
            GeneralSomatic = CreateGeneralSomaticTree();
            GeneralEndocrine = CreateGeneralEndocrineTree();
            GeneralTemporal = CreateGeneralTemporalTree();
            Hosts = CreateAllHosts();
            GestationClasses = CreateAllClasses();
        }

        // ====================================================================
        // HOSTS
        // ====================================================================

        private HostProfile[] CreateAllHosts()
        {
            return new[]
            {
                CreateUniversityElite(),
                CreateTrophyWife(),
                CreateRuthlessExecutive(),
                CreateFitnessInfluencer()
            };
        }

        private HostProfile CreateUniversityElite()
        {
            var h = ScriptableObject.CreateInstance<HostProfile>();
            h.hostName = "The University Elite";
            h.archetype = HostArchetype.UniversityElite;
            h.narrativeBackdrop = "A top-tier scholarship athlete and sorority president. Her identity is built on athletic agility and a flawless petite physique.";
            h.physicalResistance = 70;
            h.mentalDefense = 30;
            h.financialResources = 45;
            h.socialStanding = 95;
            h.vulnerability = VulnerabilityType.Humiliation;
            h.vulnerabilityMultiplier = 1.6f;
            h.panicGestationThreshold = 30f;
            h.hideAttemptHumiliationThreshold = 40f;
            h.interventionDrive = 0.8f;
            h.dailySchedule = new[]
            {
                new ScheduleEntry { taskLabel = "Team Training", taskType = TaskType.TeamTraining, category = TaskCategory.Physical, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Physical },
                new ScheduleEntry { taskLabel = "Lecture Hall", taskType = TaskType.LectureHall, category = TaskCategory.Social, baseSuccessChance = 0.80f, primaryStat = StatAffinity.Social },
                new ScheduleEntry { taskLabel = "Sorority Mixer", taskType = TaskType.SororityMixer, category = TaskCategory.Social, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Social },
                new ScheduleEntry { taskLabel = "Campus Plaza", taskType = TaskType.CampusPlaza, category = TaskCategory.Social, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Social }
            };
            return h;
        }

        private HostProfile CreateTrophyWife()
        {
            var h = ScriptableObject.CreateInstance<HostProfile>();
            h.hostName = "The Adulterous Trophy Wife";
            h.archetype = HostArchetype.AdulterousTrophyWife;
            h.narrativeBackdrop = "A woman of extreme vanity who secured a high-status marriage. Her daily supplements have been replaced with aggressive fertility agents.";
            h.physicalResistance = 20;
            h.mentalDefense = 50;
            h.financialResources = 100;
            h.socialStanding = 85;
            h.vulnerability = VulnerabilityType.Discomfort;
            h.vulnerabilityMultiplier = 2.0f;
            h.panicGestationThreshold = 20f;
            h.hideAttemptHumiliationThreshold = 30f;
            h.interventionDrive = 1.5f;
            h.dailySchedule = new[]
            {
                new ScheduleEntry { taskLabel = "Spa Wellness", taskType = TaskType.SpaWellness, category = TaskCategory.Physical, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Physical },
                new ScheduleEntry { taskLabel = "Charity Gala", taskType = TaskType.CharityGala, category = TaskCategory.Social, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Social },
                new ScheduleEntry { taskLabel = "High-End Dining", taskType = TaskType.HighEndDining, category = TaskCategory.Social, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Financial },
                new ScheduleEntry { taskLabel = "Private Clinic", taskType = TaskType.PrivateClinic, category = TaskCategory.Intervention, baseSuccessChance = 0.80f, primaryStat = StatAffinity.Financial }
            };
            return h;
        }

        private HostProfile CreateRuthlessExecutive()
        {
            var h = ScriptableObject.CreateInstance<HostProfile>();
            h.hostName = "The Ruthless Executive";
            h.archetype = HostArchetype.RuthlessExecutive;
            h.narrativeBackdrop = "A cold, hyper-competent CEO who views physical vulnerability as professional failure. Her sharp mind is about to be clouded.";
            h.physicalResistance = 50;
            h.mentalDefense = 90;
            h.financialResources = 85;
            h.socialStanding = 75;
            h.vulnerability = VulnerabilityType.PregnancyBrain;
            h.vulnerabilityMultiplier = 2.0f;
            h.panicGestationThreshold = 40f;
            h.hideAttemptHumiliationThreshold = 50f;
            h.interventionDrive = 1.2f;
            h.dailySchedule = new[]
            {
                new ScheduleEntry { taskLabel = "Boardroom Meeting", taskType = TaskType.BoardroomMeeting, category = TaskCategory.Intellectual, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Mental },
                new ScheduleEntry { taskLabel = "Business Travel", taskType = TaskType.BusinessTravel, category = TaskCategory.Physical, baseSuccessChance = 0.80f, primaryStat = StatAffinity.Physical },
                new ScheduleEntry { taskLabel = "Late Night Work", taskType = TaskType.LateNightWork, category = TaskCategory.Intellectual, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Mental },
                new ScheduleEntry { taskLabel = "High-Stakes Negotiation", taskType = TaskType.HighStakesNegotiation, category = TaskCategory.Social, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Social }
            };
            return h;
        }

        private HostProfile CreateFitnessInfluencer()
        {
            var h = ScriptableObject.CreateInstance<HostProfile>();
            h.hostName = "The Fitness Influencer";
            h.archetype = HostArchetype.FitnessInfluencer;
            h.narrativeBackdrop = "Her body is her business. Millions of followers watch her fitness journey. Her rock-hard abs are about to become a distant memory.";
            h.physicalResistance = 95;
            h.mentalDefense = 40;
            h.financialResources = 60;
            h.socialStanding = 80;
            h.vulnerability = VulnerabilityType.PhysicalNodes;
            h.vulnerabilityMultiplier = 1.5f;
            h.panicGestationThreshold = 35f;
            h.hideAttemptHumiliationThreshold = 30f;
            h.interventionDrive = 0.9f;
            h.dailySchedule = new[]
            {
                new ScheduleEntry { taskLabel = "Gym Session", taskType = TaskType.GymSession, category = TaskCategory.Physical, baseSuccessChance = 0.95f, primaryStat = StatAffinity.Physical },
                new ScheduleEntry { taskLabel = "Sponsored Photo Shoot", taskType = TaskType.SponsoredPhotoShoot, category = TaskCategory.Social, baseSuccessChance = 0.85f, primaryStat = StatAffinity.Social },
                new ScheduleEntry { taskLabel = "Juice Bar", taskType = TaskType.JuiceBar, category = TaskCategory.Social, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Social },
                new ScheduleEntry { taskLabel = "Yoga Studio", taskType = TaskType.YogaStudio, category = TaskCategory.Physical, baseSuccessChance = 0.90f, primaryStat = StatAffinity.Physical }
            };
            return h;
        }

        // ====================================================================
        // GESTATION CLASSES
        // ====================================================================

        private GestationClassData[] CreateAllClasses()
        {
            return new[]
            {
                CreateMacrosomicMultiples(),
                CreateVanityCurse(),
                CreateSymbioticEpidemic()
            };
        }

        private GestationClassData CreateMacrosomicMultiples()
        {
            var c = ScriptableObject.CreateInstance<GestationClassData>();
            c.className = "Macrosomic Multiples";
            c.classType = GestationClassType.MacrosomicMultiples;
            c.concept = "Biological revenge. Genetic hefting of twins or triplets to be massive and far ahead of gestational age.";
            c.physicality = "Extreme density. The belly is exceptionally broad, hard, and heavy.";
            c.gestationSpeedMult = 1.0f;
            c.baseGestationCap = 100f;

            c.classBranchA = CreateMacrosomicBranchA();
            c.classBranchB = CreateMacrosomicBranchB();
            c.classBranchC = CreateMacrosomicBranchC();

            c.earlyPhase = new PhaseVisualDescriptor { phase = GestationPhase.Early, gestationMin = 0, gestationMax = 15, visualDescription = "Subtle density. Barely visible.", bellyScaleMult = 1.0f, postureSwayback = 0f, moveSpeedMult = 1f };
            c.midPhase = new PhaseVisualDescriptor { phase = GestationPhase.Mid, gestationMin = 15, gestationMax = 40, visualDescription = "Hard, dense belly. Grunts during routines.", bellyScaleMult = 1.5f, postureSwayback = 0.15f, moveSpeedMult = 0.85f };
            c.latePhase = new PhaseVisualDescriptor { phase = GestationPhase.Late, gestationMin = 41, gestationMax = 75, visualDescription = "The Chonky Waddle. Immense, broad belly supported with both hands.", bellyScaleMult = 2.5f, postureSwayback = 0.45f, moveSpeedMult = 0.6f };
            c.terminalPhase = new PhaseVisualDescriptor { phase = GestationPhase.Terminal, gestationMin = 76, gestationMax = 100, visualDescription = "Terminal Macrosomia. Massive babies lock her into a slow, heavy, grunting waddle.", bellyScaleMult = 3.5f, postureSwayback = 0.7f, moveSpeedMult = 0.3f };

            return c;
        }

        private GestationClassData CreateVanityCurse()
        {
            var c = ScriptableObject.CreateInstance<GestationClassData>();
            c.className = "The Vanity Curse";
            c.classType = GestationClassType.VanityCurse;
            c.concept = "Karmic punishment for vanity. A payload of quads or quints that consume the host's body.";
            c.physicality = "Contrast between a thinning, skinny-petite frame and an impossible, protruding cluster.";
            c.gestationSpeedMult = 1.1f;
            c.baseGestationCap = 100f;

            c.classBranchA = CreateVanityCurseBranchA();
            c.classBranchB = CreateVanityCurseBranchB();
            c.classBranchC = CreateVanityCurseBranchC();

            c.earlyPhase = new PhaseVisualDescriptor { phase = GestationPhase.Early, gestationMin = 0, gestationMax = 15, visualDescription = "Frame begins to thin. Belly shows a cluster look.", bellyScaleMult = 1.0f, postureSwayback = 0f, moveSpeedMult = 1f };
            c.midPhase = new PhaseVisualDescriptor { phase = GestationPhase.Mid, gestationMin = 15, gestationMax = 40, visualDescription = "Tiny frame thinning. Belly clusters sharply.", bellyScaleMult = 1.6f, postureSwayback = 0.1f, moveSpeedMult = 0.9f };
            c.latePhase = new PhaseVisualDescriptor { phase = GestationPhase.Late, gestationMin = 41, gestationMax = 75, visualDescription = "Dwindling Frame. Limbs skinny-petite; quints protrude sharply.", bellyScaleMult = 2.8f, postureSwayback = 0.35f, moveSpeedMult = 0.55f };
            c.terminalPhase = new PhaseVisualDescriptor { phase = GestationPhase.Terminal, gestationMin = 76, gestationMax = 100, visualDescription = "Fragile Cluster. A tiny vessel carrying five active quints.", bellyScaleMult = 3.8f, postureSwayback = 0.6f, moveSpeedMult = 0.25f };

            return c;
        }

        private GestationClassData CreateSymbioticEpidemic()
        {
            var c = ScriptableObject.CreateInstance<GestationClassData>();
            c.className = "The Symbiotic Epidemic";
            c.classType = GestationClassType.SymbioticEpidemic;
            c.concept = "One singular, massive symbiote mimicking a fetus but growing to a scale that rivals a full set of quints.";
            c.physicality = "Perfectly round, beach-ball-like protrusion. Pheromonal lure.";
            c.gestationSpeedMult = 0.9f;
            c.baseGestationCap = 100f;

            c.classBranchA = CreateSymbioticBranchA();
            c.classBranchB = CreateSymbioticBranchB();
            c.classBranchC = CreateSymbioticBranchC();

            c.earlyPhase = new PhaseVisualDescriptor { phase = GestationPhase.Early, gestationMin = 0, gestationMax = 15, visualDescription = "Perfectly round rounding. Skin glows flawlessly.", bellyScaleMult = 1.0f, postureSwayback = 0f, moveSpeedMult = 1f };
            c.midPhase = new PhaseVisualDescriptor { phase = GestationPhase.Mid, gestationMin = 15, gestationMax = 40, visualDescription = "Languid movements. One singular, pretty rounding.", bellyScaleMult = 1.4f, postureSwayback = 0.1f, moveSpeedMult = 0.9f };
            c.latePhase = new PhaseVisualDescriptor { phase = GestationPhase.Late, gestationMin = 41, gestationMax = 75, visualDescription = "Spherical Burden. One massive sphere. Unconscious flirting while gasping.", bellyScaleMult = 2.6f, postureSwayback = 0.4f, moveSpeedMult = 0.6f };
            c.terminalPhase = new PhaseVisualDescriptor { phase = GestationPhase.Terminal, gestationMin = 76, gestationMax = 100, visualDescription = "Singular Goliath. Beach-ball belly. Full arched-back waddle.", bellyScaleMult = 4.0f, postureSwayback = 0.8f, moveSpeedMult = 0.2f };

            return c;
        }

        // ====================================================================
        // GENERAL SKILL TREES
        // ====================================================================

        private SkillTreeData CreateGeneralSomaticTree()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Somatic Malady";
            tree.branch = SkillBranch.SomaticMalady;
            tree.description = "Physical strain, proportions, and bodily burden.";

            var heartburn = MakeNode("Severe Heartburn", 1, 15, "Payload pressure forces acid into the esophagus.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 1.5f, isPerTick = true });
            var burps = MakeNode("Pregnancy Eructation", 1, 20, "Loud, involuntary eructations shatter her poise.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 2f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.05f, isPerTick = true });
            var gas = MakeNode("Gestational Flatulence", 1, 25, "Frequent, audible gas generates massive passive Humiliation.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 3f, isPerTick = true });
            var sciatic = MakeNode("Sciatic Nerve Compression", 2, 35, "Payload settles on the nerve. Sudden sharp zaps buckle her leg.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 2.5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.1f, isPerTick = true });
            var ligament = MakeNode("Round Ligament Strain", 2, 45, "Agonizingly stretched supports. Must move with extreme deliberation.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.15f, isPerTick = true });
            var pelvic = MakeNode("Pelvic Floor Pressure", 3, 55, "The payload feels as if constantly about to drop.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3.5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.2f, isPerTick = true });
            var gravity = MakeNode("Center of Gravity Shift", 3, 65, "Extreme swayback posture, bracing lower back with hands.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 4f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.25f, isPerTick = true });
            var skin = MakeNode("Integumentary Tension", 4, 80, "Skin impossibly taut, shiny, and translucent.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.VisualSkinTranslucent, magnitude = 1f, isPerTick = true });
            var gasping = MakeNode("Respiratory Gasping", 4, 100, "Payload crushes the diaphragm. Perpetually breathless.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 6f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.35f, isPerTick = true });

            sciatic.prerequisites = new[] { heartburn };
            ligament.prerequisites = new[] { sciatic };
            pelvic.prerequisites = new[] { ligament };
            gravity.prerequisites = new[] { pelvic };
            skin.prerequisites = new[] { gravity };
            gasping.prerequisites = new[] { skin };

            tree.nodes = new[] { heartburn, burps, gas, sciatic, ligament, pelvic, gravity, skin, gasping };
            return tree;
        }

        private SkillTreeData CreateGeneralEndocrineTree()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Endocrine & Aesthetic Sabotage";
            tree.branch = SkillBranch.EndocrineAestheticSabotage;
            tree.description = "Public humiliation, hormonal disruption, and social destruction.";

            var flush = MakeNode("Thermal Flush & Glisten", 1, 15, "Constant perspiration and deep facial flush.", new SkillEffect { effectType = SkillEffectType.VisualSweat, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 1.5f, isPerTick = true });
            var bladder = MakeNode("Bladder Compression", 1, 30, "Frequent restroom detours interrupt long tasks.", new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.08f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 1f, isPerTick = true });
            var wardrobe = MakeNode("Wardrobe Failure", 2, 50, "Buttons pop. Zippers fail. Massive passive Humiliation.", new SkillEffect { effectType = SkillEffectType.VisualWardrobeFailure, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 4f, isPerTick = true });
            var saliva = MakeNode("Gestational Ptyalism", 2, 65, "Excessive saliva. Speech slurred and wet.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.1f, isPerTick = true });
            var vocals = MakeNode("Involuntary Exertion Vocalizations", 3, 80, "Deep grunts and rhythmic pants with every step.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 5f, isPerTick = true });
            var undulation = MakeNode("Public Abdominal Undulation", 3, 95, "Violent shifting ripples the taut skin.", new SkillEffect { effectType = SkillEffectType.VisualAbdominalUndulation, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3f, isPerTick = true });
            var lactation = MakeNode("Chronic Hyper-Lactation", 4, 110, "Massive stains ruin expensive maternity wear.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 6f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.DrainFinancial, magnitude = 2f, isPerTick = true });
            var incontinence = MakeNode("Hormonal Incontinence", 4, 130, "Spontaneous weeping or flustered panic.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 7f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceIntellect, magnitude = 0.3f, isPerTick = false });
            var objectification = MakeNode("Social Objectification", 4, 150, "NPCs focus exclusively on her abdomen. Authority permanently deleted.", new SkillEffect { effectType = SkillEffectType.DeleteAuthority, magnitude = 1f, isPerTick = false }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 8f, isPerTick = true });

            bladder.prerequisites = new[] { flush };
            wardrobe.prerequisites = new[] { bladder };
            saliva.prerequisites = new[] { wardrobe };
            vocals.prerequisites = new[] { saliva };
            undulation.prerequisites = new[] { vocals };
            lactation.prerequisites = new[] { undulation };
            incontinence.prerequisites = new[] { lactation };
            objectification.prerequisites = new[] { incontinence };

            tree.nodes = new[] { flush, bladder, wardrobe, saliva, vocals, undulation, lactation, incontinence, objectification };
            return tree;
        }

        private SkillTreeData CreateGeneralTemporalTree()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Temporal Distortion";
            tree.branch = SkillBranch.TemporalDistortion;
            tree.description = "Clinical labor delay. Extends the gestation cap.";

            var oxytocin = MakeNode("Oxytocin Antagonism", 1, 20, "Mutes internal labor signals. Max Gestation 115%.", new SkillEffect { effectType = SkillEffectType.RaiseGestationCap, magnitude = 115f, isPerTick = false });
            var cervical = MakeNode("Cervical Seal Reinforcement", 2, 50, "Structural lock prevents dilation. Max Gestation 135%.", new SkillEffect { effectType = SkillEffectType.RaiseGestationCap, magnitude = 135f, isPerTick = false });
            var quiescence = MakeNode("Uterine Quiescence", 3, 100, "Smooth muscles chemically paralyzed. Max Gestation 160%.", new SkillEffect { effectType = SkillEffectType.RaiseGestationCap, magnitude = 160f, isPerTick = false });

            cervical.prerequisites = new[] { oxytocin };
            quiescence.prerequisites = new[] { cervical };

            tree.nodes = new[] { oxytocin, cervical, quiescence };
            return tree;
        }

        // ====================================================================
        // CLASS-SPECIFIC SKILL TREES
        // ====================================================================

        // --- Macrosomic Multiples ---
        private SkillTreeData CreateMacrosomicBranchA()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Chonky Cage";
            tree.branch = SkillBranch.ClassBranchA;
            tree.description = "Somatic core: extreme physical density and burden.";

            var t1 = MakeNode("Hefted Multiples", 1, 20, "Large-boned, chonked-up fetuses. +20% Gestation Density.", new SkillEffect { effectType = SkillEffectType.IncreaseGestationDensity, magnitude = 0.2f, isPerTick = true });
            var t2 = MakeNode("Macrosomic Density", 2, 40, "Babies feel like solid stone. Wider, braced waddle.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.15f, isPerTick = true });
            var t3 = MakeNode("Double/Triple Macrosomia", 3, 70, "Each multiple hits max size thresholds. Incredibly broad belly.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.25f, isPerTick = true });
            var t4 = MakeNode("Implacable Weight", 4, 110, "Massive triplets settle into the pelvis. Movement -60%.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 7f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.6f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateMacrosomicBranchB()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Macrosomic Growth";
            tree.branch = SkillBranch.ClassBranchB;
            tree.description = "Fetal demands: accelerated growth and organ compression.";

            var t1 = MakeNode("Genetic Macrosomia", 1, 20, "Accelerates growth speed by +25%.", new SkillEffect { effectType = SkillEffectType.IncreaseGestationSpeed, magnitude = 0.25f, isPerTick = true });
            var t2 = MakeNode("Aggressive Fetal Crowding", 2, 45, "Crushes organs upward. Caps stamina recovery at 50%.", new SkillEffect { effectType = SkillEffectType.ReduceStamina, magnitude = 0.5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3f, isPerTick = true });
            var t3 = MakeNode("Violent Quickening", 3, 65, "Fetal kicks buckle her knees. Random micro-stuns.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 4f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.12f, isPerTick = true });
            var t4 = MakeNode("Structural Displacement", 4, 95, "Massive weight forces hips to separate. Permanent wide waddle.", new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.4f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 6f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateMacrosomicBranchC()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Pregnancy Brain";
            tree.branch = SkillBranch.ClassBranchC;
            tree.description = "Endocrine/psychological warfare against the host's mind.";

            var t1 = MakeNode("Hormonal Fog II", 1, 20, "Reduced Intellect by 40%.", new SkillEffect { effectType = SkillEffectType.ReduceIntellect, magnitude = 0.4f, isPerTick = false });
            var t2 = MakeNode("Insatiable Caloric Drive", 2, 40, "Forced overeating to fuel the chonky payload.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 2f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.DrainFinancial, magnitude = 1.5f, isPerTick = true });
            var t3 = MakeNode("Hyper-Sensitization", 3, 70, "Involuntary arousal from movement friction.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 4f, isPerTick = true });
            var t4 = MakeNode("Nesting Paralysis", 4, 110, "Replaces Seek_Doctor with Rest_and_Stroke_Belly.", new SkillEffect { effectType = SkillEffectType.CancelIntervention, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReplaceTask, magnitude = 1f, isPerTick = false });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        // --- Vanity Curse ---
        private SkillTreeData CreateVanityCurseBranchA()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Multiples";
            tree.branch = SkillBranch.ClassBranchA;
            tree.description = "Somatic core: fecund overload and exponential growth.";

            var t1 = MakeNode("Fecund Overload", 1, 20, "Converts pregnancy to quads/quints. Belly begins to cluster.", new SkillEffect { effectType = SkillEffectType.IncreaseGestationSpeed, magnitude = 0.15f, isPerTick = true });
            var t2 = MakeNode("Glistening Exertion", 2, 35, "Constant sweat. Disables hiding permanently.", new SkillEffect { effectType = SkillEffectType.VisualSweat, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 2f, isPerTick = true });
            var t3 = MakeNode("Exponential Fecundity", 3, 70, "Must support belly with both hands.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.3f, isPerTick = true });
            var t4 = MakeNode("The Impossible Burden", 4, 110, "Logic-defying mass maximizes grunting.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 8f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 6f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateVanityCurseBranchB()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "Precious Vessel";
            tree.branch = SkillBranch.ClassBranchB;
            tree.description = "Morphological dwindling. The host shrinks around the burden.";

            var t1 = MakeNode("Prettification", 1, 15, "Features soften into doll-like perfection.", new SkillEffect { effectType = SkillEffectType.ReduceSocialStanding, magnitude = 5f, isPerTick = false }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 1f, isPerTick = true });
            var t2 = MakeNode("Petitification & Dwindling", 2, 45, "Host's frame shrinks. Limbs become skinny-petite.", new SkillEffect { effectType = SkillEffectType.VisualPetitification, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 3f, isPerTick = true });
            var t3 = MakeNode("Skeletal Contrast", 3, 75, "Fragile frame struggles to contain five active fetuses.", new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 4f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 5f, isPerTick = true });
            var t4 = MakeNode("Total Petitification", 4, 100, "A tiny, skinny-petite vessel carrying a cluster of five normal-sized babies.", new SkillEffect { effectType = SkillEffectType.VisualPetitification, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 6f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 7f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateVanityCurseBranchC()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Thrall";
            tree.branch = SkillBranch.ClassBranchC;
            tree.description = "Endocrine/behavioral. Infantilization and dependency.";

            var t1 = MakeNode("Aura of Infantilization", 1, 20, "Demands sound like pouting. 100% Command failure.", new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.15f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 2f, isPerTick = true });
            var t2 = MakeNode("Intimate Pampering Response", 2, 40, "NPCs force her to sit and have her feet rubbed.", new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 3f, isPerTick = true });
            var t3 = MakeNode("Helpless Dependency", 3, 65, "Must seek NPCs to help her stand up.", new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.25f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.TaskFailureChance, magnitude = 0.2f, isPerTick = true });
            var t4 = MakeNode("Total Maternal Thrall", 4, 100, "Doctors give her lollipops instead of medical treatment.", new SkillEffect { effectType = SkillEffectType.CancelIntervention, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 5f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        // --- Symbiotic Epidemic ---
        private SkillTreeData CreateSymbioticBranchA()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Endless Incubation";
            tree.branch = SkillBranch.ClassBranchA;
            tree.description = "Somatic: singular goliath growth and infinite gestation.";

            var t1 = MakeNode("Singular Goliath", 1, 25, "One massive entity. +50% abdominal projection.", new SkillEffect { effectType = SkillEffectType.VisualSphericalBelly, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 2f, isPerTick = true });
            var t2 = MakeNode("Spherical Perfection", 2, 50, "Belly grows into a perfectly round, taut sphere.", new SkillEffect { effectType = SkillEffectType.VisualSphericalBelly, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 2f, isPerTick = true });
            var t3 = MakeNode("Infinite Trimester I", 3, 75, "Overrides general limits. Gestation can reach 200%.", new SkillEffect { effectType = SkillEffectType.RaiseGestationCap, magnitude = 200f, isPerTick = false });
            var t4 = MakeNode("The Infinite Goliath", 4, 120, "Removes cap. Full arched-back waddle.", new SkillEffect { effectType = SkillEffectType.RaiseGestationCap, magnitude = 300f, isPerTick = false }, new SkillEffect { effectType = SkillEffectType.AddDiscomfort, magnitude = 6f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReduceMobility, magnitude = 0.5f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateSymbioticBranchB()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Hijacked Lure";
            tree.branch = SkillBranch.ClassBranchB;
            tree.description = "Aesthetic/behavioral: pheromonal seduction and mental overwrite.";

            var t1 = MakeNode("Gestation-Induced Attractiveness", 1, 20, "Cellular optimization. Huge Social Standing buff.", new SkillEffect { effectType = SkillEffectType.IncreaseBiomassFromHumiliation, magnitude = 1f, isPerTick = true });
            var t2 = MakeNode("Synthetic Bliss", 2, 45, "Movement triggers dopamine. Converts Discomfort to Euphoria.", new SkillEffect { effectType = SkillEffectType.ConvertDiscomfortToEuphoria, magnitude = 0.5f, isPerTick = true });
            var t3 = MakeNode("Pheromonal Seduction", 3, 75, "Dialogue forced into a seductive register.", new SkillEffect { effectType = SkillEffectType.AddHumiliation, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.IncreaseBiomassFromHumiliation, magnitude = 2f, isPerTick = true });
            var t4 = MakeNode("Manufactured Devotion", 4, 110, "Total mental overwrite. Host actively patrols to find victims.", new SkillEffect { effectType = SkillEffectType.CancelIntervention, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.ReplaceTask, magnitude = 1f, isPerTick = false });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        private SkillTreeData CreateSymbioticBranchC()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeData>();
            tree.treeName = "The Cuckoo Taint";
            tree.branch = SkillBranch.ClassBranchC;
            tree.description = "Transmission: infection spread and epidemic mechanics.";

            var t1 = MakeNode("Tainted Secretions", 1, 30, "Fluids carry spores. Leaves infection zone in rooms.", new SkillEffect { effectType = SkillEffectType.InfectionZone, magnitude = 1f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.PassiveBiomassGeneration, magnitude = 0.5f, isPerTick = true });
            var t2 = MakeNode("Parasitic Resonance", 2, 60, "Converts other fetuses into symbiotes via contact.", new SkillEffect { effectType = SkillEffectType.InfectionZone, magnitude = 2f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.PassiveBiomassGeneration, magnitude = 1f, isPerTick = true });
            var t3 = MakeNode("Airborne Pheromones", 3, 85, "Room-wide infection zone.", new SkillEffect { effectType = SkillEffectType.InfectionZone, magnitude = 3f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.PassiveBiomassGeneration, magnitude = 2f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.VisualGlowingVeins, magnitude = 1f, isPerTick = true });
            var t4 = MakeNode("Epidemic Bloom", 4, 120, "Secondary hosts create exponential spread.", new SkillEffect { effectType = SkillEffectType.PassiveBiomassGeneration, magnitude = 5f, isPerTick = true }, new SkillEffect { effectType = SkillEffectType.IncreaseGestationSpeed, magnitude = 0.4f, isPerTick = true });

            t2.prerequisites = new[] { t1 };
            t3.prerequisites = new[] { t2 };
            t4.prerequisites = new[] { t3 };

            tree.nodes = new[] { t1, t2, t3, t4 };
            return tree;
        }

        // ====================================================================
        // HELPERS
        // ====================================================================

        private SkillNodeData MakeNode(string name, int tier, int cost, string desc, params SkillEffect[] effects)
        {
            var node = ScriptableObject.CreateInstance<SkillNodeData>();
            node.nodeName = name;
            node.tier = tier;
            node.biomassCost = cost;
            node.description = desc;
            node.effects = effects;
            node.activationNarrative = $">> {name} activated.";
            return node;
        }
    }
}
