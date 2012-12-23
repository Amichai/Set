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
using System.Windows.Markup;

namespace set {
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class Card : UserControl {
        public Card() {
            InitializeComponent();
        }

        public event EventHandler CardSelected;
        public event EventHandler CardDeselected;

        private void OnCardSelected() {
            var ev = CardSelected;
            if (ev != null) {
                ev(this, new EventArgs());
            }
        }

        private void OnCardDeselected() {
            var ev = CardDeselected;
            if (ev != null) {
                ev(this, new EventArgs());
            }
        }

        private bool _isSelected = false;
        public bool IsSelected {
            get {
                return _isSelected;
            }
            set {
                _isSelected = value;
                if (_isSelected) {
                    this.GridRoot.Background = Brushes.LightBlue;
                    OnCardSelected();
                }
                else {
                    this.GridRoot.Background = Brushes.White;
                    OnCardDeselected();
                }
            }
        }

        public Card(int q, int c, int f, int s) {
            this.Quantity = q;
            this.Color = c;
            this.Fill = f;
            this.Shape = s;
            InitializeComponent();
            //this.Root = new StackPanel();
            Arrange();
        }
        public int Quantity { get; set; }
        public int Color { get; set; }
        public int Fill { get; set; }
        public int Shape { get; set; }

        public override string ToString() {
            return this.Quantity.ToString() + " " + this.Color.ToString() + " " + this.Fill.ToString() + " " + this.Shape.ToString();
        }

        public void Arrange() {
            UIElement c = null;
            double opacityVal = .3;
            switch (Shape) {
                case 0:
                    c = new Border();
                    ((Border)c).Width = 130;
                    ((Border)c).Height = 50;
                    ((Border)c).BorderThickness = new Thickness(3);

                    switch (Color) {
                        case 0:
                            ((Border)c).BorderBrush = Brushes.Red;
                            break;
                        case 1:
                            ((Border)c).BorderBrush = Brushes.Green;
                            break;
                        case 2:
                            ((Border)c).BorderBrush = Brushes.Blue;
                            break;
                    }

                    switch (Fill) {
                        case 0:
                            break;
                        case 1:
                           ((Border)c).Background = ((Border)c).BorderBrush;
                           break;
                        case 2:
                           ((Border)c).Background = ((Border)c).BorderBrush;
                           ((Border)c).Opacity = opacityVal;
                           break;
                    }
                    ((Border)c).CornerRadius = new CornerRadius(30);
                    ((Border)c).Margin = new Thickness(10, 10, 10, 10);
                    break;

                case 1:
                    c = new Rectangle();
                    ((Rectangle)c).Width = 130;
                    ((Rectangle)c).Height = 35;
                    switch (Color) {
                        case 0:
                            ((Rectangle)c).Stroke = Brushes.Red;
                            break;
                        case 1:
                            ((Rectangle)c).Stroke = Brushes.Green;
                            break;
                        case 2:
                            ((Rectangle)c).Stroke = Brushes.Blue;
                            break;
                    }

                    switch (Fill) {
                        case 0:
                            break;
                        case 1:
                            ((Rectangle)c).Fill = ((Rectangle)c).Stroke;
                            break;
                        case 2:
                            ((Rectangle)c).Fill = ((Rectangle)c).Stroke;
                            ((Rectangle)c).Opacity = opacityVal;
                            break;
                    }
                    ((Rectangle)c).StrokeThickness = 3;
                    ((Rectangle)c).Margin = new Thickness(10, 10, 10, 10);
                    break;
                case 2:
                    c = new Polygon();
                    ((Polygon)c).Points.Add(new Point(0, 25));
                    ((Polygon)c).Points.Add(new Point(80, 0));
                    ((Polygon)c).Points.Add(new Point(160, 25));
                    ((Polygon)c).Points.Add(new Point(80, 50));
                    ((Polygon)c).StrokeThickness = 3;
                    switch (Color) {
                        case 0:
                            ((Polygon)c).Stroke = Brushes.Red;
                            break;
                        case 1:
                            ((Polygon)c).Stroke = Brushes.Green;
                            break;
                        case 2:
                            ((Polygon)c).Stroke = Brushes.Blue;
                            break;
                    }
                    switch (Fill) {
                        case 0:
                            break;
                        case 1:
                            ((Polygon)c).Fill = ((Polygon)c).Stroke;
                            break;
                        case 2:
                            ((Polygon)c).Fill = ((Polygon)c).Stroke;
                            ((Polygon)c).Opacity = opacityVal;
                            break;
                    }

                    ((Polygon)c).Margin = new Thickness(10, 10, 10, 10);
                    break;
            }

            for (int i = 0; i < Quantity + 1; i++) {
                UIElement copy = XamlReader.Parse(XamlWriter.Save(c)) as UIElement;
                this.Root.Children.Add(copy);
            }

        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            this.IsSelected = !this.IsSelected;
        }
    }
}
