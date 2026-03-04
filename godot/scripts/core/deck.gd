class_name Deck
extends RefCounted

var draw_pile: Array[CardData] = []
var hand: Array[CardData] = []
var discard_pile: Array[CardData] = []


func add_card(card: CardData) -> void:
	draw_pile.append(card)


func shuffle() -> void:
	draw_pile.shuffle()


func draw(count: int) -> Array[CardData]:
	var drawn: Array[CardData] = []
	for i in count:
		if hand.size() >= GameConstants.MAX_HAND_SIZE:
			break
		if draw_pile.is_empty():
			_reshuffle_discard()
		if draw_pile.is_empty():
			break
		var card: CardData = draw_pile.pop_back()
		hand.append(card)
		drawn.append(card)
	return drawn


func play_card(card: CardData) -> bool:
	var idx: int = hand.find(card)
	if idx < 0:
		return false
	hand.remove_at(idx)
	discard_pile.append(card)
	return true


func _reshuffle_discard() -> void:
	if not GameConstants.DECK_SHUFFLE_ON_EMPTY:
		return
	draw_pile = discard_pile.duplicate()
	discard_pile.clear()
	shuffle()


func get_total_cards() -> int:
	return draw_pile.size() + hand.size() + discard_pile.size()
