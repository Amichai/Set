using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Timers;
using System.Xml.Linq;


namespace set {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		Deck d;
		string timeFormatString = @"mm\:ss";
        string dateFormatString = "MMMM dd, yyyy";
		public MainWindow() {
			InitializeComponent();
			restart();
            scoresFilepath = @"../../Scores.xml";
            scores = XElement.Load(scoresFilepath);
            loadTopScores();
		}

        private bool _showTopScores;
        public bool ShowTopScores {
            get {
                return _showTopScores;
            }
            set {
                _showTopScores = value;
                if(_showTopScores){
                    this.ScoresHeader.Visibility = System.Windows.Visibility.Visible;
                    this.TopScores.Visibility = System.Windows.Visibility.Visible;
                }else{
                    this.ScoresHeader.Visibility = System.Windows.Visibility.Hidden;
                    this.TopScores.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        void loadTopScores(bool setName = true) {
            this.TopScores.Children.Clear();
            foreach (var a in scores.Elements()) {
                TimeSpan timeToClear = TimeSpan.Parse(a.Attribute("Time").Value);
                TextBlock time = new TextBlock() {
                    Text = string.Format("Time to clear: {0}", timeToClear.ToString(timeFormatString)),
                    Margin = new Thickness(20, 20, 20, 20)
                };
                TextBlock date = new TextBlock() {
                    Text = string.Format("{0}", DateTime.Parse(a.Attribute("Date").Value).ToString(dateFormatString)),
                    Margin = new Thickness(20, 20, 20, 20)
                };
                StackPanel toAdd = new StackPanel() { Orientation = Orientation.Horizontal };
                toAdd.Children.Add(time);
                toAdd.Children.Add(date);
                if (sw.Elapsed != timeToClear || !setName) {
                    toAdd.Children.Add(new TextBlock() { Width = 300, Text = a.Attribute("Name").Value, VerticalAlignment = System.Windows.VerticalAlignment.Center });
                }
                else { 
                    toAdd.Background = Brushes.LightGray;
                    TextBox nameToSave = new TextBox() { Width = 300, Text = "Anonymous", Background = Brushes.LightGray, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    nameToSave.LostFocus += new RoutedEventHandler(nameToSave_LostFocus);
                    nameToSave.GotFocus += new RoutedEventHandler(nameToSave_GotFocus);
                    nameToSave.PreviewKeyDown += new KeyEventHandler(nameToSave_PreviewKeyDown);
                    toAdd.Children.Add(nameToSave);
                }
                this.TopScores.Children.Add(toAdd);
            }
        }

        void updateName(TimeSpan ts, string name) {
            scores.Elements().Where(i => TimeSpan.Parse(i.Attribute("Time").Value) == ts).First().Attribute("Name").Value = name;
            scores.Save(scoresFilepath);
        }

        void nameToSave_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    updateName(sw.Elapsed, ((TextBox)sender).Text);
                    loadTopScores(false);
                    this.Root.KeyDown += Window_KeyDown;
                    break;
            }
        }

        void nameToSave_GotFocus(object sender, RoutedEventArgs e) {
            if (((TextBox)sender).Text == "Anonymous") {
                ((TextBox)sender).Text = "";
            }
            this.Root.KeyDown -= Window_KeyDown;
        }

        void nameToSave_LostFocus(object sender, RoutedEventArgs e) {
            this.Root.KeyDown += Window_KeyDown;
        }

        XElement scores;
        string scoresFilepath;

		void restart() {
            ShowTopScores = false;
			this.Columns.Children.Clear();
			sw = new Stopwatch();
			sw.Start();
			this.elapsed.Text = "Time elapsed: " + sw.Elapsed.ToString(timeFormatString);
			timer = new Timer();
			timer.Interval = 1000;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Start();
			this.ShowAvailableSets = true;
			d = new Deck();
			d.NumberOfSelectedCardsChanged += new EventHandler(d_NumberOfSelectedCardsChanged);
			d.ShuffleDeck();
            SetsAvailable = 0;
			dealToASet();
		}
		
		void timer_Elapsed(object sender, ElapsedEventArgs e) {
			Dispatcher.Invoke((Action)(() => {
				this.elapsed.Text = "Time elapsed: " + sw.Elapsed.ToString(timeFormatString);
			}));
		}

		Stopwatch sw;
		Timer timer;

		bool sameOrDifferent(int a, int b, int c) {
			return ((a == b && b == c) ||
				(a != b && b != c && a != c));
		}

		bool testForSet(Card a, Card b, Card c) {
			bool sameOrDiff1 = sameOrDifferent(a.Color, b.Color, c.Color);
			bool sameOrDiff2 = sameOrDifferent(a.Quantity, b.Quantity, c.Quantity);
			bool sameOrDiff3 = sameOrDifferent(a.Fill, b.Fill, c.Fill);
			bool sameOrDiff4 = sameOrDifferent(a.Shape, b.Shape, c.Shape);
			return (sameOrDiff1 && sameOrDiff2 && sameOrDiff3 && sameOrDiff4);
		}

		private void rearrange() {
			int numberOfColumns = this.Columns.Children.Count;
			for (int i = 0; i < numberOfColumns; i++) {
				int childrenInLastColumn = ((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children.Count;
				if (childrenInLastColumn == 0) {
					this.Columns.Children.RemoveAt(numberOfColumns - 1);
					numberOfColumns--;
					if (numberOfColumns == 0) return;
					childrenInLastColumn = ((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children.Count;
				}
				while (((StackPanel)this.Columns.Children[i]).Children.Count < 3) {
					numberOfColumns = this.Columns.Children.Count; //make sure to take out empty columns
					Card cardToTake = (Card)((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children[childrenInLastColumn - 1];
					((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children.Remove(cardToTake);
					((StackPanel)this.Columns.Children[i]).Children.Add(cardToTake);
					childrenInLastColumn = ((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children.Count;
					if (childrenInLastColumn == 0) {
						this.Columns.Children.RemoveAt(numberOfColumns - 1);
						numberOfColumns--;
						childrenInLastColumn = ((StackPanel)this.Columns.Children[numberOfColumns - 1]).Children.Count;
					}
					//Take a card from the last stack panel
					//put the card in this stack panel
					//check if we can remove the last stack panel (empty)
				}
			}
		}

        void saveScore(TimeSpan ts) {
            foreach (var a in scores.Elements()) {
                if (ts < TimeSpan.Parse(a.Attribute("Time").Value)) {
                    a.AddBeforeSelf(new XElement("Score", new XAttribute("Time", ts.ToString()), new XAttribute("Date", DateTime.Now), new XAttribute("Name", "Anonymous")));
                    break;
                }
            }
            scores.Save(scoresFilepath);
        }

		void dealToASet() {
			while (SetsAvailable == 0 && d.CardsRemaining() > 0) {
				addThreeCards();
				SetsAvailable = numberOfAvailableSets();
			}
			if (SetsAvailable == 0) {
				timer.Stop();
				sw.Stop();
                saveScore(sw.Elapsed);
                loadTopScores();
                ShowTopScores = true;
			}
		}

		void d_NumberOfSelectedCardsChanged(object sender, EventArgs e) {
			if (d.Selected.Count == 3) {
				if (testForSet(d.Selected[0], d.Selected[1], d.Selected[2])) {
					((StackPanel)(d.Selected[0].Parent)).Children.Remove(d.Selected[0]);
					((StackPanel)(d.Selected[1].Parent)).Children.Remove(d.Selected[1]);
					((StackPanel)(d.Selected[2].Parent)).Children.Remove(d.Selected[2]);
					rearrange();
					d.DeselectAll();
					SetsAvailable = numberOfAvailableSets();
					if (ShowAvailableSets) {
						dealToASet();
					}
				}
				else {
					d.DeselectAll();
				}
			}
		}

		List<Card> onTheTable = new List<Card>();

		int numberOfAvailableSets() {
			int totalNumberOfCards = (this.Columns.Children.Count) * 3;
			int setsCounted = 0;
			for (int i = 0; i < totalNumberOfCards; i++) {
				for (int j = i + 1; j < totalNumberOfCards; j++) {
					for (int k = j + 1; k < totalNumberOfCards; k++) {
						Card card1 = ((Card)((StackPanel)this.Columns.Children[i / 3]).Children[i % 3]);
						Card card2 = ((Card)((StackPanel)this.Columns.Children[j / 3]).Children[j % 3]);
						Card card3 = ((Card)((StackPanel)this.Columns.Children[k / 3]).Children[k % 3]);
						if (testForSet(card1, card2, card3)) {
							setsCounted++;
						}
					}
				}
			}
			return setsCounted;
		}

		private int _setsAvailable;
		public int SetsAvailable {
			get {
				return _setsAvailable;
			}
			set {
				_setsAvailable = value;
				this.Info.Text = "Sets available: " + _setsAvailable.ToString();
			}
		}

		private void addThreeCards() {
			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Vertical;
			sp.Children.Add(d.NextCard());
			sp.Children.Add(d.NextCard());
			sp.Children.Add(d.NextCard());
			this.Columns.Children.Add(sp);
			SetsAvailable = numberOfAvailableSets();
			this.remaining.Text = d.CardsRemaining().ToString() + " cards";
		}

		private bool _showAvailableSets;
		public bool ShowAvailableSets {
			get {
				return _showAvailableSets;
			}
			set {
				_showAvailableSets = value;
				if (ShowAvailableSets) {
					this.Info.Visibility = System.Windows.Visibility.Visible;
				}
				else {
					this.Info.Visibility = System.Windows.Visibility.Hidden;
				}
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.N:
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
						restart();
					}
					else if (d.CardsRemaining() > 0) {
						addThreeCards();
					}
					break;
				case Key.S:
					ShowAvailableSets = !ShowAvailableSets;
					break;
                case Key.T:
                    this.ShowTopScores = !this.ShowTopScores;
                    break;

			}
		}
	}
}
