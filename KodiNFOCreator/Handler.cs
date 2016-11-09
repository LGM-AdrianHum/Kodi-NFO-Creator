using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LegeDoos.KodiNFOCreator.Properties;

namespace LegeDoos.KodiNFOCreator
{
    internal class Handler
    {
        public enum ScraperTypes
        {
            Omdb,
            Searchengine
        }

        private const int MinimumStringSize = 3;
        private readonly AutoCompleteTextBox _formAutoCompleteTextBox;
        private readonly ComboBox _formScraperSelectComboBox;
        private readonly Label _sourceFileLabel;
        private readonly BindingSource _theBindingSource;
        private MovieScraper _movieScraper;
        private Movie _nfo;

        //public ScraperTypes SraperType { get; set; }

        private bool _searchingExecuting;

        #region.constructors

        public Handler(AutoCompleteTextBox autoCompleteTextBox, Label sourceFileLabel, BindingSource bindingSourceNfo,
            ComboBox scraperSelectCombo)
        {
            _formAutoCompleteTextBox = autoCompleteTextBox;
            _sourceFileLabel = sourceFileLabel;
            _theBindingSource = bindingSourceNfo;
            _formScraperSelectComboBox = scraperSelectCombo;
            _formScraperSelectComboBox.DataSource = Enum.GetValues(typeof(ScraperTypes)).Cast<ScraperTypes>();
            _formScraperSelectComboBox.SelectedItem = ScraperTypes.Omdb;
            InitMovieScraper();
        }

        #endregion

        public string SourceFull { get; set; }
        public string SourcePath => Path.GetDirectoryName(SourceFull);
        public string SourceFile => Path.GetFileName(SourceFull);
        public string SourceExtension => Path.GetExtension(SourceFull);
        private string TargetFilenameNfo => $"{SourcePath}\\{Path.GetFileNameWithoutExtension(SourceFull)}.nfo";

        #region.ui

        private void Initialize(string fileName)
        {
            _nfo = new Movie(fileName);
            SourceFull = fileName;
            _sourceFileLabel.Text = SourceFile;
            _theBindingSource.DataSource = _nfo;
        }

        public void OpenFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK && dlg.FileName != null)
            {
                Initialize(dlg.FileName);
            }
        }

        internal void CreateNfo()
        {
            if (string.IsNullOrEmpty(SourceFile))
            {
                MessageBox.Show(Resources.Handler_CreateNfo_No_media_file_selected_);
                return;
            }

            var exportNfo = !string.IsNullOrEmpty(_nfo?.Title);

            if (!exportNfo)
            {
                MessageBox.Show(
                    Resources
                        .Handler_CreateNfo_You_have_to_select_a_online_title_or_enter_a_custom_title_to_use_the_create_NFO_function_);
                return;
            }

            //export nfo
            _nfo.SaveNfo(TargetFilenameNfo);
            MessageBox.Show("NFO saved!");
        }

        private bool AddUrlToFile()
        {
            var retVal = false;
            var fileExists = File.Exists(TargetFilenameNfo);

            //append url
            File.AppendAllText(TargetFilenameNfo, fileExists ? Environment.NewLine : "" + "http://www.google.com");
            retVal = true;
            return true;
        }

        internal void MakeFindableForCouchPotato()
        {
            throw new NotImplementedException();
        }

        internal void SraperChanged()
        {
            InitMovieScraper();
        }

        private void InitMovieScraper()
        {
            var type = (ScraperTypes) Enum.Parse(typeof(ScraperTypes), _formScraperSelectComboBox.Text);

            switch (type)
            {
                case ScraperTypes.Omdb:
                    _movieScraper = new MovieScraperOMdb();
                    break;
                case ScraperTypes.Searchengine:
                    _movieScraper = new MovieScraperSearchEngine();
                    break;
                default:
                    //default to omdb
                    _movieScraper = new MovieScraperOMdb();
                    break;
            }
        }

        #endregion

        #region.search

        internal void DoSearch(string p)
        {
            if (!_searchingExecuting && _movieScraper != null && _formAutoCompleteTextBox.IsSearching)
            {
                _searchingExecuting = true;
                if (p.ToLower() != _formAutoCompleteTextBox.Text.ToLower())
                {
                    _movieScraper.SearchForTitlePart(p);
                    _formAutoCompleteTextBox.Values = _movieScraper.SearchResultsArray;
                }
                _searchingExecuting = false;
            }
        }

        internal void ExecuteSearch(bool force)
        {
            var searchString = _formAutoCompleteTextBox.Text.Trim();

            if (searchString.Length > MinimumStringSize)
            {
                DoSearch(searchString);
            }
            else
            {
                _formAutoCompleteTextBox.Values = null;
            }
        }

        internal void StartSearch()
        {
            _formAutoCompleteTextBox.IsSearching = true;
        }

        internal void StopSearch()
        {
            _formAutoCompleteTextBox.IsSearching = false;
        }

        #endregion
    }
}