using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace set {
    class Deck {
        public Deck() {
            this.cards = new Stack<Card>();
            this.Selected = new List<Card>();
            allCards = new List<Card>();
            
            constructSortedDeck();
        }

        private void constructSortedDeck() {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        for (int l = 0; l < 3; l++) {
                            var card = new Card(i, j, k, l);
                            card.CardSelected += new EventHandler(card_CardSelected);
                            card.CardDeselected += new EventHandler(card_CardDeselected);
                            allCards.Add(card);
                        }
                    }
                }
            }
        }

        public List<Card> Selected;

        public event EventHandler NumberOfSelectedCardsChanged;

        private void OnNumberOfSelectedCardsChanged() {
            var ev = NumberOfSelectedCardsChanged;
            if (ev != null) {
                ev(this, new EventArgs());
            }
        }

        public void DeselectAll() {
            while (Selected.Count() > 0) {
                Selected.First().IsSelected = false;
            }
        }

        void card_CardDeselected(object sender, EventArgs e) {
            totalNumberOfSelectedCards--;
            Selected.Remove((Card)sender);
            OnNumberOfSelectedCardsChanged();
        }

        int totalNumberOfSelectedCards = 0;

        void card_CardSelected(object sender, EventArgs e) {
            totalNumberOfSelectedCards++;
            Selected.Add((Card)sender);
            OnNumberOfSelectedCardsChanged();
        }

        public int CardsRemaining() {
            return this.cards.Count;
        }

        Stack<Card> cards;

        static List<Card> allCards;
        static Random rng = new Random();

        public void ShuffleDeck() {
            if (this.cards.Count() != 0) throw new Exception();
            HashSet<int> indicesTaken = new HashSet<int>();
            while (this.cards.Count() < allCards.Count) {
                int idxToTake = rng.Next(0, allCards.Count);
                if (!indicesTaken.Contains(idxToTake)) {
                    this.cards.Push(allCards[idxToTake]);
                    indicesTaken.Add(idxToTake);
                }
            }
            if (indicesTaken.Count() != allCards.Count) throw new Exception();
        }

        public Card NextCard() {
            return this.cards.Pop();
        }
    }
}
