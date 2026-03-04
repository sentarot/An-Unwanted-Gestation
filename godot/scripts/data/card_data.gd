class_name CardData
extends Resource

@export var card_name: String = ""
@export var side: int = Enums.CardSide.PREGNANCY
@export var play_cost: float = 0.0
@export_multiline var description: String = ""
@export var effects: Array[Dictionary] = []
@export_multiline var play_narrative: String = ""


static func create(p_name: String, p_side: int, p_cost: float, p_desc: String,
		p_effects: Array[Dictionary], p_narrative: String = "") -> CardData:
	var card := CardData.new()
	card.card_name = p_name
	card.side = p_side
	card.play_cost = p_cost
	card.description = p_desc
	card.effects = p_effects
	card.play_narrative = p_narrative if p_narrative != "" else ">> %s played." % p_name
	return card
