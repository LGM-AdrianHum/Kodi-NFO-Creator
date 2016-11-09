using System;
using System.Windows.Forms;

namespace LegeDoos.KodiNFOCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Handler = new Handler(autoCompleteTextBox, labelSourceFile, kodiNFOBindingSource, comboBoxScraper);
        }

        private Handler Handler { get; }

        private void autoCompleteTextBox_StoppedTypingTextChanged(object sender, EventArgs e)
        {
            //do search with force
            Handler.ExecuteSearch(true);
        }

        private void autoCompleteTextBox_TextChanged_1(object sender, EventArgs e)
        {
            //do search on space
            Handler.StartSearch();
            if (autoCompleteTextBox.Text.EndsWith(" "))
            {
                Handler.ExecuteSearch(false);
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Handler.OpenFile();
        }

        private void createNFOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Handler.CreateNfo();
        }

        private void buttonCreateNFO_Click(object sender, EventArgs e)
        {
            Handler.CreateNfo();
        }

        private void buttonFindableCouchPotato_Click(object sender, EventArgs e)
        {
            Handler.MakeFindableForCouchPotato();
        }

        private void makeFindableForCouchPotatoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Handler.MakeFindableForCouchPotato();
        }

        private void comboBoxScraper_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Handler != null)
                Handler.SraperChanged();
        }

        private void autoCompleteTextBox_Enter(object sender, EventArgs e)
        {
            Handler.StartSearch();
        }

        private void autoCompleteTextBox_Leave(object sender, EventArgs e)
        {
            Handler.StopSearch();
        }
    }
}