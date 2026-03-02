class_name DataFactory
extends Node

var hosts: Array[HostProfile] = []
var gestation_classes: Array[GestationClassData] = []
var general_somatic: SkillTreeData
var general_endocrine: SkillTreeData
var general_temporal: SkillTreeData


func generate() -> void:
	general_somatic = _create_general_somatic_tree()
	general_endocrine = _create_general_endocrine_tree()
	general_temporal = _create_general_temporal_tree()
	hosts = _create_all_hosts()
	gestation_classes = _create_all_classes()


# =====================================================================
# HELPERS
# =====================================================================

func _e(type: int, mag: float, per_tick: bool = true) -> Dictionary:
	return { "type": type, "magnitude": mag, "is_per_tick": per_tick }


func _make_node(p_name: String, p_tier: int, p_cost: int, p_desc: String, p_effects: Array[Dictionary]) -> SkillNodeData:
	return SkillNodeData.create(p_name, p_tier, p_cost, p_desc, p_effects)


# =====================================================================
# HOSTS
# =====================================================================

func _create_all_hosts() -> Array[HostProfile]:
	return [
		_create_university_elite(),
		_create_trophy_wife(),
		_create_ruthless_executive(),
		_create_fitness_influencer()
	]


func _create_university_elite() -> HostProfile:
	var h := HostProfile.new()
	h.host_name = "The University Elite"
	h.archetype = Enums.HostArchetype.UNIVERSITY_ELITE
	h.narrative_backdrop = "A top-tier scholarship athlete and sorority president. Her identity is built on athletic agility and a flawless petite physique."
	h.physical_resistance = 70
	h.mental_defense = 30
	h.financial_resources = 45
	h.social_standing = 95
	h.vulnerability = Enums.VulnerabilityType.HUMILIATION
	h.vulnerability_multiplier = 1.6
	h.panic_gestation_threshold = 30.0
	h.hide_attempt_humiliation_threshold = 40.0
	h.intervention_drive = 0.8
	h.daily_schedule = [
		{ "task_label": "Team Training", "task_type": Enums.TaskType.TEAM_TRAINING, "category": Enums.TaskCategory.PHYSICAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.PHYSICAL },
		{ "task_label": "Lecture Hall", "task_type": Enums.TaskType.LECTURE_HALL, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.80, "primary_stat": Enums.StatAffinity.SOCIAL },
		{ "task_label": "Sorority Mixer", "task_type": Enums.TaskType.SORORITY_MIXER, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.SOCIAL },
		{ "task_label": "Campus Plaza", "task_type": Enums.TaskType.CAMPUS_PLAZA, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.SOCIAL },
	]
	return h


func _create_trophy_wife() -> HostProfile:
	var h := HostProfile.new()
	h.host_name = "The Adulterous Trophy Wife"
	h.archetype = Enums.HostArchetype.ADULTEROUS_TROPHY_WIFE
	h.narrative_backdrop = "A woman of extreme vanity who secured a high-status marriage. Her daily supplements have been replaced with aggressive fertility agents."
	h.physical_resistance = 20
	h.mental_defense = 50
	h.financial_resources = 100
	h.social_standing = 85
	h.vulnerability = Enums.VulnerabilityType.DISCOMFORT
	h.vulnerability_multiplier = 2.0
	h.panic_gestation_threshold = 20.0
	h.hide_attempt_humiliation_threshold = 30.0
	h.intervention_drive = 1.5
	h.daily_schedule = [
		{ "task_label": "Spa Wellness", "task_type": Enums.TaskType.SPA_WELLNESS, "category": Enums.TaskCategory.PHYSICAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.PHYSICAL },
		{ "task_label": "Charity Gala", "task_type": Enums.TaskType.CHARITY_GALA, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.SOCIAL },
		{ "task_label": "High-End Dining", "task_type": Enums.TaskType.HIGH_END_DINING, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.FINANCIAL },
		{ "task_label": "Private Clinic", "task_type": Enums.TaskType.PRIVATE_CLINIC, "category": Enums.TaskCategory.INTERVENTION, "base_success_chance": 0.80, "primary_stat": Enums.StatAffinity.FINANCIAL },
	]
	return h


func _create_ruthless_executive() -> HostProfile:
	var h := HostProfile.new()
	h.host_name = "The Ruthless Executive"
	h.archetype = Enums.HostArchetype.RUTHLESS_EXECUTIVE
	h.narrative_backdrop = "A cold, hyper-competent CEO who views physical vulnerability as professional failure. Her sharp mind is about to be clouded."
	h.physical_resistance = 50
	h.mental_defense = 90
	h.financial_resources = 85
	h.social_standing = 75
	h.vulnerability = Enums.VulnerabilityType.PREGNANCY_BRAIN
	h.vulnerability_multiplier = 2.0
	h.panic_gestation_threshold = 40.0
	h.hide_attempt_humiliation_threshold = 50.0
	h.intervention_drive = 1.2
	h.daily_schedule = [
		{ "task_label": "Boardroom Meeting", "task_type": Enums.TaskType.BOARDROOM_MEETING, "category": Enums.TaskCategory.INTELLECTUAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.MENTAL },
		{ "task_label": "Business Travel", "task_type": Enums.TaskType.BUSINESS_TRAVEL, "category": Enums.TaskCategory.PHYSICAL, "base_success_chance": 0.80, "primary_stat": Enums.StatAffinity.PHYSICAL },
		{ "task_label": "Late Night Work", "task_type": Enums.TaskType.LATE_NIGHT_WORK, "category": Enums.TaskCategory.INTELLECTUAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.MENTAL },
		{ "task_label": "High-Stakes Negotiation", "task_type": Enums.TaskType.HIGH_STAKES_NEGOTIATION, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.SOCIAL },
	]
	return h


func _create_fitness_influencer() -> HostProfile:
	var h := HostProfile.new()
	h.host_name = "The Fitness Influencer"
	h.archetype = Enums.HostArchetype.FITNESS_INFLUENCER
	h.narrative_backdrop = "Her body is her business. Millions of followers watch her fitness journey. Her rock-hard abs are about to become a distant memory."
	h.physical_resistance = 95
	h.mental_defense = 40
	h.financial_resources = 60
	h.social_standing = 80
	h.vulnerability = Enums.VulnerabilityType.PHYSICAL_NODES
	h.vulnerability_multiplier = 1.5
	h.panic_gestation_threshold = 35.0
	h.hide_attempt_humiliation_threshold = 30.0
	h.intervention_drive = 0.9
	h.daily_schedule = [
		{ "task_label": "Gym Session", "task_type": Enums.TaskType.GYM_SESSION, "category": Enums.TaskCategory.PHYSICAL, "base_success_chance": 0.95, "primary_stat": Enums.StatAffinity.PHYSICAL },
		{ "task_label": "Sponsored Photo Shoot", "task_type": Enums.TaskType.SPONSORED_PHOTO_SHOOT, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.85, "primary_stat": Enums.StatAffinity.SOCIAL },
		{ "task_label": "Juice Bar", "task_type": Enums.TaskType.JUICE_BAR, "category": Enums.TaskCategory.SOCIAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.SOCIAL },
		{ "task_label": "Yoga Studio", "task_type": Enums.TaskType.YOGA_STUDIO, "category": Enums.TaskCategory.PHYSICAL, "base_success_chance": 0.90, "primary_stat": Enums.StatAffinity.PHYSICAL },
	]
	return h


# =====================================================================
# GESTATION CLASSES
# =====================================================================

func _create_all_classes() -> Array[GestationClassData]:
	return [
		_create_macrosomic_multiples(),
		_create_vanity_curse(),
		_create_symbiotic_epidemic()
	]


func _create_macrosomic_multiples() -> GestationClassData:
	var c := GestationClassData.new()
	c.class_name_text = "Macrosomic Multiples"
	c.class_type = Enums.GestationClassType.MACROSOMIC_MULTIPLES
	c.concept = "Biological revenge. Genetic hefting of twins or triplets to be massive and far ahead of gestational age."
	c.physicality = "Extreme density. The belly is exceptionally broad, hard, and heavy."
	c.gestation_speed_mult = 1.0
	c.base_gestation_cap = 100.0
	c.class_branch_a = _create_macrosomic_branch_a()
	c.class_branch_b = _create_macrosomic_branch_b()
	c.class_branch_c = _create_macrosomic_branch_c()
	c.early_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.EARLY, 0, 15, "Subtle density. Barely visible.", 1.0, 0.0, 1.0)
	c.mid_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.MID, 15, 40, "Hard, dense belly. Grunts during routines.", 1.5, 0.15, 0.85)
	c.late_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.LATE, 41, 75, "The Chonky Waddle. Immense, broad belly supported with both hands.", 2.5, 0.45, 0.6)
	c.terminal_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.TERMINAL, 76, 100, "Terminal Macrosomia. Massive babies lock her into a slow, heavy, grunting waddle.", 3.5, 0.7, 0.3)
	return c


func _create_vanity_curse() -> GestationClassData:
	var c := GestationClassData.new()
	c.class_name_text = "The Vanity Curse"
	c.class_type = Enums.GestationClassType.VANITY_CURSE
	c.concept = "Karmic punishment for vanity. A payload of quads or quints that consume the host's body."
	c.physicality = "Contrast between a thinning, skinny-petite frame and an impossible, protruding cluster."
	c.gestation_speed_mult = 1.1
	c.base_gestation_cap = 100.0
	c.class_branch_a = _create_vanity_curse_branch_a()
	c.class_branch_b = _create_vanity_curse_branch_b()
	c.class_branch_c = _create_vanity_curse_branch_c()
	c.early_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.EARLY, 0, 15, "Frame begins to thin. Belly shows a cluster look.", 1.0, 0.0, 1.0)
	c.mid_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.MID, 15, 40, "Tiny frame thinning. Belly clusters sharply.", 1.6, 0.1, 0.9)
	c.late_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.LATE, 41, 75, "Dwindling Frame. Limbs skinny-petite; quints protrude sharply.", 2.8, 0.35, 0.55)
	c.terminal_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.TERMINAL, 76, 100, "Fragile Cluster. A tiny vessel carrying five active quints.", 3.8, 0.6, 0.25)
	return c


func _create_symbiotic_epidemic() -> GestationClassData:
	var c := GestationClassData.new()
	c.class_name_text = "The Symbiotic Epidemic"
	c.class_type = Enums.GestationClassType.SYMBIOTIC_EPIDEMIC
	c.concept = "One singular, massive symbiote mimicking a fetus but growing to a scale that rivals a full set of quints."
	c.physicality = "Perfectly round, beach-ball-like protrusion. Pheromonal lure."
	c.gestation_speed_mult = 0.9
	c.base_gestation_cap = 100.0
	c.class_branch_a = _create_symbiotic_branch_a()
	c.class_branch_b = _create_symbiotic_branch_b()
	c.class_branch_c = _create_symbiotic_branch_c()
	c.early_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.EARLY, 0, 15, "Perfectly round rounding. Skin glows flawlessly.", 1.0, 0.0, 1.0)
	c.mid_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.MID, 15, 40, "Languid movements. One singular, pretty rounding.", 1.4, 0.1, 0.9)
	c.late_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.LATE, 41, 75, "Spherical Burden. One massive sphere. Unconscious flirting while gasping.", 2.6, 0.4, 0.6)
	c.terminal_phase = PhaseVisualDescriptor.create(Enums.GestationPhase.TERMINAL, 76, 100, "Singular Goliath. Beach-ball belly. Full arched-back waddle.", 4.0, 0.8, 0.2)
	return c


# =====================================================================
# GENERAL SKILL TREES
# =====================================================================

func _create_general_somatic_tree() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Somatic Malady"
	tree.branch = Enums.SkillBranch.SOMATIC_MALADY
	tree.description = "Physical strain, proportions, and bodily burden."

	var E := Enums.SkillEffectType
	var heartburn := _make_node("Severe Heartburn", 1, 15, "Payload pressure forces acid into the esophagus.",
		[_e(E.ADD_DISCOMFORT, 1.5)] as Array[Dictionary])
	var burps := _make_node("Pregnancy Eructation", 1, 20, "Loud, involuntary eructations shatter her poise.",
		[_e(E.ADD_HUMILIATION, 2.0), _e(E.TASK_FAILURE_CHANCE, 0.05)] as Array[Dictionary])
	var gas := _make_node("Gestational Flatulence", 1, 25, "Frequent, audible gas generates massive passive Humiliation.",
		[_e(E.ADD_HUMILIATION, 3.0)] as Array[Dictionary])
	var sciatic := _make_node("Sciatic Nerve Compression", 2, 35, "Payload settles on the nerve. Sudden sharp zaps buckle her leg.",
		[_e(E.ADD_DISCOMFORT, 2.5), _e(E.REDUCE_MOBILITY, 0.1)] as Array[Dictionary])
	var ligament := _make_node("Round Ligament Strain", 2, 45, "Agonizingly stretched supports. Must move with extreme deliberation.",
		[_e(E.ADD_DISCOMFORT, 3.0), _e(E.REDUCE_MOBILITY, 0.15)] as Array[Dictionary])
	var pelvic := _make_node("Pelvic Floor Pressure", 3, 55, "The payload feels as if constantly about to drop.",
		[_e(E.ADD_DISCOMFORT, 3.5), _e(E.REDUCE_MOBILITY, 0.2)] as Array[Dictionary])
	var gravity := _make_node("Center of Gravity Shift", 3, 65, "Extreme swayback posture, bracing lower back with hands.",
		[_e(E.ADD_DISCOMFORT, 4.0), _e(E.REDUCE_MOBILITY, 0.25)] as Array[Dictionary])
	var skin := _make_node("Integumentary Tension", 4, 80, "Skin impossibly taut, shiny, and translucent.",
		[_e(E.ADD_DISCOMFORT, 5.0), _e(E.VISUAL_SKIN_TRANSLUCENT, 1.0)] as Array[Dictionary])
	var gasping := _make_node("Respiratory Gasping", 4, 100, "Payload crushes the diaphragm. Perpetually breathless.",
		[_e(E.ADD_DISCOMFORT, 6.0), _e(E.REDUCE_MOBILITY, 0.35)] as Array[Dictionary])

	sciatic.prerequisites = [heartburn]
	ligament.prerequisites = [sciatic]
	pelvic.prerequisites = [ligament]
	gravity.prerequisites = [pelvic]
	skin.prerequisites = [gravity]
	gasping.prerequisites = [skin]

	tree.nodes = [heartburn, burps, gas, sciatic, ligament, pelvic, gravity, skin, gasping]
	return tree


func _create_general_endocrine_tree() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Endocrine & Aesthetic Sabotage"
	tree.branch = Enums.SkillBranch.ENDOCRINE_AESTHETIC_SABOTAGE
	tree.description = "Public humiliation, hormonal disruption, and social destruction."

	var E := Enums.SkillEffectType
	var flush := _make_node("Thermal Flush & Glisten", 1, 15, "Constant perspiration and deep facial flush.",
		[_e(E.VISUAL_SWEAT, 1.0), _e(E.ADD_HUMILIATION, 1.5)] as Array[Dictionary])
	var bladder := _make_node("Bladder Compression", 1, 30, "Frequent restroom detours interrupt long tasks.",
		[_e(E.TASK_FAILURE_CHANCE, 0.08), _e(E.ADD_DISCOMFORT, 1.0)] as Array[Dictionary])
	var wardrobe := _make_node("Wardrobe Failure", 2, 50, "Buttons pop. Zippers fail. Massive passive Humiliation.",
		[_e(E.VISUAL_WARDROBE_FAILURE, 1.0), _e(E.ADD_HUMILIATION, 4.0)] as Array[Dictionary])
	var saliva := _make_node("Gestational Ptyalism", 2, 65, "Excessive saliva. Speech slurred and wet.",
		[_e(E.ADD_HUMILIATION, 3.0), _e(E.TASK_FAILURE_CHANCE, 0.1)] as Array[Dictionary])
	var vocals := _make_node("Involuntary Exertion Vocalizations", 3, 80, "Deep grunts and rhythmic pants with every step.",
		[_e(E.ADD_HUMILIATION, 5.0)] as Array[Dictionary])
	var undulation := _make_node("Public Abdominal Undulation", 3, 95, "Violent shifting ripples the taut skin.",
		[_e(E.VISUAL_ABDOMINAL_UNDULATION, 1.0), _e(E.ADD_HUMILIATION, 5.0), _e(E.ADD_DISCOMFORT, 3.0)] as Array[Dictionary])
	var lactation := _make_node("Chronic Hyper-Lactation", 4, 110, "Massive stains ruin expensive maternity wear.",
		[_e(E.ADD_HUMILIATION, 6.0), _e(E.DRAIN_FINANCIAL, 2.0)] as Array[Dictionary])
	var incontinence := _make_node("Hormonal Incontinence", 4, 130, "Spontaneous weeping or flustered panic.",
		[_e(E.ADD_HUMILIATION, 7.0), _e(E.REDUCE_INTELLECT, 0.3, false)] as Array[Dictionary])
	var objectification := _make_node("Social Objectification", 4, 150, "NPCs focus exclusively on her abdomen. Authority permanently deleted.",
		[_e(E.DELETE_AUTHORITY, 1.0, false), _e(E.ADD_HUMILIATION, 8.0)] as Array[Dictionary])

	bladder.prerequisites = [flush]
	wardrobe.prerequisites = [bladder]
	saliva.prerequisites = [wardrobe]
	vocals.prerequisites = [saliva]
	undulation.prerequisites = [vocals]
	lactation.prerequisites = [undulation]
	incontinence.prerequisites = [lactation]
	objectification.prerequisites = [incontinence]

	tree.nodes = [flush, bladder, wardrobe, saliva, vocals, undulation, lactation, incontinence, objectification]
	return tree


func _create_general_temporal_tree() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Temporal Distortion"
	tree.branch = Enums.SkillBranch.TEMPORAL_DISTORTION
	tree.description = "Clinical labor delay. Extends the gestation cap."

	var E := Enums.SkillEffectType
	var oxytocin := _make_node("Oxytocin Antagonism", 1, 20, "Mutes internal labor signals. Max Gestation 115%.",
		[_e(E.RAISE_GESTATION_CAP, 115.0, false)] as Array[Dictionary])
	var cervical := _make_node("Cervical Seal Reinforcement", 2, 50, "Structural lock prevents dilation. Max Gestation 135%.",
		[_e(E.RAISE_GESTATION_CAP, 135.0, false)] as Array[Dictionary])
	var quiescence := _make_node("Uterine Quiescence", 3, 100, "Smooth muscles chemically paralyzed. Max Gestation 160%.",
		[_e(E.RAISE_GESTATION_CAP, 160.0, false)] as Array[Dictionary])

	cervical.prerequisites = [oxytocin]
	quiescence.prerequisites = [cervical]

	tree.nodes = [oxytocin, cervical, quiescence]
	return tree


# =====================================================================
# CLASS-SPECIFIC SKILL TREES
# =====================================================================

# --- Macrosomic Multiples ---

func _create_macrosomic_branch_a() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Chonky Cage"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_A
	tree.description = "Somatic core: extreme physical density and burden."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Hefted Multiples", 1, 20, "Large-boned, chonked-up fetuses. +20% Gestation Density.",
		[_e(E.INCREASE_GESTATION_DENSITY, 0.2)] as Array[Dictionary])
	var t2 := _make_node("Macrosomic Density", 2, 40, "Babies feel like solid stone. Wider, braced waddle.",
		[_e(E.ADD_DISCOMFORT, 3.0), _e(E.REDUCE_MOBILITY, 0.15)] as Array[Dictionary])
	var t3 := _make_node("Double/Triple Macrosomia", 3, 70, "Each multiple hits max size thresholds. Incredibly broad belly.",
		[_e(E.ADD_DISCOMFORT, 5.0), _e(E.REDUCE_MOBILITY, 0.25)] as Array[Dictionary])
	var t4 := _make_node("Implacable Weight", 4, 110, "Massive triplets settle into the pelvis. Movement -60%.",
		[_e(E.ADD_DISCOMFORT, 7.0), _e(E.REDUCE_MOBILITY, 0.6)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_macrosomic_branch_b() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Macrosomic Growth"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_B
	tree.description = "Fetal demands: accelerated growth and organ compression."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Genetic Macrosomia", 1, 20, "Accelerates growth speed by +25%.",
		[_e(E.INCREASE_GESTATION_SPEED, 0.25)] as Array[Dictionary])
	var t2 := _make_node("Aggressive Fetal Crowding", 2, 45, "Crushes organs upward. Caps stamina recovery at 50%.",
		[_e(E.REDUCE_STAMINA, 0.5), _e(E.ADD_DISCOMFORT, 3.0)] as Array[Dictionary])
	var t3 := _make_node("Violent Quickening", 3, 65, "Fetal kicks buckle her knees. Random micro-stuns.",
		[_e(E.ADD_DISCOMFORT, 4.0), _e(E.TASK_FAILURE_CHANCE, 0.12)] as Array[Dictionary])
	var t4 := _make_node("Structural Displacement", 4, 95, "Massive weight forces hips to separate. Permanent wide waddle.",
		[_e(E.REDUCE_MOBILITY, 0.4), _e(E.ADD_DISCOMFORT, 6.0)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_macrosomic_branch_c() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Pregnancy Brain"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_C
	tree.description = "Endocrine/psychological warfare against the host's mind."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Hormonal Fog II", 1, 20, "Reduced Intellect by 40%.",
		[_e(E.REDUCE_INTELLECT, 0.4, false)] as Array[Dictionary])
	var t2 := _make_node("Insatiable Caloric Drive", 2, 40, "Forced overeating to fuel the chonky payload.",
		[_e(E.ADD_HUMILIATION, 2.0), _e(E.DRAIN_FINANCIAL, 1.5)] as Array[Dictionary])
	var t3 := _make_node("Hyper-Sensitization", 3, 70, "Involuntary arousal from movement friction.",
		[_e(E.ADD_HUMILIATION, 4.0)] as Array[Dictionary])
	var t4 := _make_node("Nesting Paralysis", 4, 110, "Replaces Seek_Doctor with Rest_and_Stroke_Belly.",
		[_e(E.CANCEL_INTERVENTION, 1.0), _e(E.REPLACE_TASK, 1.0, false)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


# --- Vanity Curse ---

func _create_vanity_curse_branch_a() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Multiples"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_A
	tree.description = "Somatic core: fecund overload and exponential growth."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Fecund Overload", 1, 20, "Converts pregnancy to quads/quints. Belly begins to cluster.",
		[_e(E.INCREASE_GESTATION_SPEED, 0.15)] as Array[Dictionary])
	var t2 := _make_node("Glistening Exertion", 2, 35, "Constant sweat. Disables hiding permanently.",
		[_e(E.VISUAL_SWEAT, 1.0), _e(E.ADD_HUMILIATION, 2.0)] as Array[Dictionary])
	var t3 := _make_node("Exponential Fecundity", 3, 70, "Must support belly with both hands.",
		[_e(E.ADD_DISCOMFORT, 5.0), _e(E.REDUCE_MOBILITY, 0.3)] as Array[Dictionary])
	var t4 := _make_node("The Impossible Burden", 4, 110, "Logic-defying mass maximizes grunting.",
		[_e(E.ADD_DISCOMFORT, 8.0), _e(E.ADD_HUMILIATION, 6.0)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_vanity_curse_branch_b() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "Precious Vessel"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_B
	tree.description = "Morphological dwindling. The host shrinks around the burden."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Prettification", 1, 15, "Features soften into doll-like perfection.",
		[_e(E.REDUCE_SOCIAL_STANDING, 5.0, false), _e(E.ADD_HUMILIATION, 1.0)] as Array[Dictionary])
	var t2 := _make_node("Petitification & Dwindling", 2, 45, "Host's frame shrinks. Limbs become skinny-petite.",
		[_e(E.VISUAL_PETITIFICATION, 1.0), _e(E.ADD_HUMILIATION, 3.0)] as Array[Dictionary])
	var t3 := _make_node("Skeletal Contrast", 3, 75, "Fragile frame struggles to contain five active fetuses.",
		[_e(E.ADD_DISCOMFORT, 4.0), _e(E.ADD_HUMILIATION, 5.0)] as Array[Dictionary])
	var t4 := _make_node("Total Petitification", 4, 100, "A tiny, skinny-petite vessel carrying a cluster of five normal-sized babies.",
		[_e(E.VISUAL_PETITIFICATION, 1.0), _e(E.ADD_DISCOMFORT, 6.0), _e(E.ADD_HUMILIATION, 7.0)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_vanity_curse_branch_c() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Thrall"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_C
	tree.description = "Endocrine/behavioral. Infantilization and dependency."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Aura of Infantilization", 1, 20, "Demands sound like pouting. 100% Command failure.",
		[_e(E.TASK_FAILURE_CHANCE, 0.15), _e(E.ADD_HUMILIATION, 2.0)] as Array[Dictionary])
	var t2 := _make_node("Intimate Pampering Response", 2, 40, "NPCs force her to sit and have her feet rubbed.",
		[_e(E.REDUCE_MOBILITY, 0.1), _e(E.ADD_HUMILIATION, 3.0)] as Array[Dictionary])
	var t3 := _make_node("Helpless Dependency", 3, 65, "Must seek NPCs to help her stand up.",
		[_e(E.REDUCE_MOBILITY, 0.25), _e(E.TASK_FAILURE_CHANCE, 0.2)] as Array[Dictionary])
	var t4 := _make_node("Total Maternal Thrall", 4, 100, "Doctors give her lollipops instead of medical treatment.",
		[_e(E.CANCEL_INTERVENTION, 1.0), _e(E.ADD_HUMILIATION, 5.0)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


# --- Symbiotic Epidemic ---

func _create_symbiotic_branch_a() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Endless Incubation"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_A
	tree.description = "Somatic: singular goliath growth and infinite gestation."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Singular Goliath", 1, 25, "One massive entity. +50% abdominal projection.",
		[_e(E.VISUAL_SPHERICAL_BELLY, 1.0), _e(E.ADD_DISCOMFORT, 2.0)] as Array[Dictionary])
	var t2 := _make_node("Spherical Perfection", 2, 50, "Belly grows into a perfectly round, taut sphere.",
		[_e(E.VISUAL_SPHERICAL_BELLY, 1.0), _e(E.ADD_DISCOMFORT, 3.0), _e(E.ADD_HUMILIATION, 2.0)] as Array[Dictionary])
	var t3 := _make_node("Infinite Trimester I", 3, 75, "Overrides general limits. Gestation can reach 200%.",
		[_e(E.RAISE_GESTATION_CAP, 200.0, false)] as Array[Dictionary])
	var t4 := _make_node("The Infinite Goliath", 4, 120, "Removes cap. Full arched-back waddle.",
		[_e(E.RAISE_GESTATION_CAP, 300.0, false), _e(E.ADD_DISCOMFORT, 6.0), _e(E.REDUCE_MOBILITY, 0.5)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_symbiotic_branch_b() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Hijacked Lure"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_B
	tree.description = "Aesthetic/behavioral: pheromonal seduction and mental overwrite."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Gestation-Induced Attractiveness", 1, 20, "Cellular optimization. Huge Social Standing buff.",
		[_e(E.INCREASE_BIOMASS_FROM_HUMILIATION, 1.0)] as Array[Dictionary])
	var t2 := _make_node("Synthetic Bliss", 2, 45, "Movement triggers dopamine. Converts Discomfort to Euphoria.",
		[_e(E.CONVERT_DISCOMFORT_TO_EUPHORIA, 0.5)] as Array[Dictionary])
	var t3 := _make_node("Pheromonal Seduction", 3, 75, "Dialogue forced into a seductive register.",
		[_e(E.ADD_HUMILIATION, 3.0), _e(E.INCREASE_BIOMASS_FROM_HUMILIATION, 2.0)] as Array[Dictionary])
	var t4 := _make_node("Manufactured Devotion", 4, 110, "Total mental overwrite. Host actively patrols to find victims.",
		[_e(E.CANCEL_INTERVENTION, 1.0), _e(E.REPLACE_TASK, 1.0, false)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree


func _create_symbiotic_branch_c() -> SkillTreeData:
	var tree := SkillTreeData.new()
	tree.tree_name = "The Cuckoo Taint"
	tree.branch = Enums.SkillBranch.CLASS_BRANCH_C
	tree.description = "Transmission: infection spread and epidemic mechanics."

	var E := Enums.SkillEffectType
	var t1 := _make_node("Tainted Secretions", 1, 30, "Fluids carry spores. Leaves infection zone in rooms.",
		[_e(E.INFECTION_ZONE, 1.0), _e(E.PASSIVE_BIOMASS_GENERATION, 0.5)] as Array[Dictionary])
	var t2 := _make_node("Parasitic Resonance", 2, 60, "Converts other fetuses into symbiotes via contact.",
		[_e(E.INFECTION_ZONE, 2.0), _e(E.PASSIVE_BIOMASS_GENERATION, 1.0)] as Array[Dictionary])
	var t3 := _make_node("Airborne Pheromones", 3, 85, "Room-wide infection zone.",
		[_e(E.INFECTION_ZONE, 3.0), _e(E.PASSIVE_BIOMASS_GENERATION, 2.0), _e(E.VISUAL_GLOWING_VEINS, 1.0)] as Array[Dictionary])
	var t4 := _make_node("Epidemic Bloom", 4, 120, "Secondary hosts create exponential spread.",
		[_e(E.PASSIVE_BIOMASS_GENERATION, 5.0), _e(E.INCREASE_GESTATION_SPEED, 0.4)] as Array[Dictionary])

	t2.prerequisites = [t1]
	t3.prerequisites = [t2]
	t4.prerequisites = [t3]
	tree.nodes = [t1, t2, t3, t4]
	return tree
