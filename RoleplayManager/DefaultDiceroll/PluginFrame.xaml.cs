using System;
using System.Collections.Generic;
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

namespace DefaultDiceroll {
    /// <summary>
    /// Interaction logic for PluginFrame.xaml
    /// </summary>
    public partial class PluginFrame:UserControl {
        public PluginFrame() {
            InitializeComponent();
        }

        public void ReceiveData(string senderUsername, string data) {
            this.Dispatcher.Invoke(() => {
                TB_RollLog.Text += senderUsername + " has rolled: " + data + "\n";
            });
        }
        private void Button_Click(object sender,RoutedEventArgs e) {
            int diceAmount;
            bool isNumericAmount = Int32.TryParse(TB_DiceAmount.Text, out diceAmount);
            int diceSize;
            bool isNumericSize = Int32.TryParse(TB_DiceSize.Text, out diceSize);

            if (isNumericAmount && isNumericSize) {

                if (diceAmount > 0 && diceAmount < 101 && diceSize > 0 && diceSize < 1001) {

                    int[] results = new int[diceAmount];

                    Random random = new Random();

                    for(int i = 0; i < results.Length; i++) {
                        results[i] = random.Next(1,Int32.Parse(TB_DiceSize.Text));
                    }

                    string msg = "[" + TB_DiceAmount.Text + "d" + TB_DiceSize.Text + "] ";

                    int sum = 0;
                    foreach(int result in results) {
                        sum += result;
                        msg = msg + result + " ";
                    }

                    msg = msg + "Sum: " + sum;

                    DefaultDiceroll.entity.SendPluginPacket(msg);

                } else {
                    MessageBox.Show("You may not roll more than 100 or less than 1 dice. Dice may not be larger than 1000 or smaller than 0 sides, either.");
                }


            } else {
                MessageBox.Show("Please enter an amount and a size of dice before attempting to roll!");
            }

        }

        private void TB_DiceAmount_TextChanged(object sender,TextChangedEventArgs e) {
            var TextBox = (sender as TextBox);
            if (System.Text.RegularExpressions.Regex.IsMatch(TB_DiceAmount.Text, "[^0-9]")) {
                foreach (TextChange change in e.Changes) {
                    TB_DiceAmount.Text = TB_DiceAmount.Text.Remove(change.Offset,change.AddedLength);

                    TB_DiceAmount.CaretIndex = change.Offset;
                }
            }
        }

        private void TB_DiceSize_TextChanged(object sender,TextChangedEventArgs e) {
            if(System.Text.RegularExpressions.Regex.IsMatch(TB_DiceSize.Text,"[^0-9]")) {
                foreach (TextChange change in e.Changes) {
                    TB_DiceSize.Text = TB_DiceSize.Text.Remove(change.Offset,change.AddedLength);

                    TB_DiceSize.CaretIndex = change.Offset;
                }
            }
        }
    }
}
