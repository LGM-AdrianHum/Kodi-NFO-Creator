using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LegeDoos.KodiNFOCreator
{
    //taken from: http://stackoverflow.com/questions/1437002/winforms-c-sharp-autocomplete-in-the-middle-of-a-textbox
    //taken from: http://stackoverflow.com/questions/8001450/c-sharp-wait-for-user-to-finish-typing-in-a-text-box
    //changed and optmized by LegeDoos

    public class AutoCompleteTextBox : TextBox
    {
        private const int StoppedTypingTimeout = 1000; //1 sec

        //delay
        private Timer _delayedTextChangedTimer;
        private string _formerValue = string.Empty;
        private bool _isAdded;
        //middle search
        private ListBox _listBox;
        private string[] _values;

        public AutoCompleteTextBox()
        {
            InitializeComponent();
            ResetListBox();
        }

        public string[] Values
        {
            get { return _values; }
            set
            {
                _values = value;
                if (_values == null)
                    return;
                UpdateListBox();
                if (Focused)
                    ShowListBox();
            }
        }

        /// <summary>
        ///     Is the textbox in searchmode?
        /// </summary>
        public bool IsSearching { get; set; }

        public event EventHandler StoppedTypingTextChanged;

        #region.middlesearch

        private void InitializeComponent()
        {
            _listBox = new ListBox();

            KeyDown += this_KeyDown;
            KeyUp += this_KeyUp;
        }

        private void ShowListBox()
        {
            if (!_isAdded)
            {
                Parent.Controls.Add(_listBox);
                _listBox.Left = Left;
                _listBox.Top = Top + Height;
                _isAdded = true;
            }
            _listBox.Visible = true;
            _listBox.BringToFront();
        }

        private void ResetListBox()
        {
            _listBox.Visible = false;
        }

        private void this_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateListBox();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //case Keys.Tab:
                case Keys.Enter:
                {
                    if (_listBox.Visible)
                    {
                        InsertWord((string) _listBox.SelectedItem);
                        ResetListBox();
                        _formerValue = Text;
                    }
                    break;
                }
                case Keys.Down:
                {
                    if (_listBox.Visible && (_listBox.SelectedIndex < _listBox.Items.Count - 1))
                        _listBox.SelectedIndex++;

                    break;
                }
                case Keys.Up:
                {
                    if (_listBox.Visible && (_listBox.SelectedIndex > 0))
                        _listBox.SelectedIndex--;

                    break;
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                //case Keys.Tab:
                case Keys.Enter:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        private void UpdateListBox()
        {
            if (Text == _formerValue) return;
            _formerValue = Text;
            var word = Text;

            if (Values != null && word.Length > 0)
            {
                var matches = Array.FindAll(Values,
                    x => x.ToLower().Contains(word.ToLower()) && !SelectedValues.Contains(x));
                if (matches.Length > 0)
                {
                    ShowListBox();
                    _listBox.Items.Clear();
                    Array.ForEach(matches, x => _listBox.Items.Add(x));
                    _listBox.SelectedIndex = 0;
                    _listBox.Height = 0;
                    _listBox.Width = 0;
                    Focus();
                    using (var graphics = _listBox.CreateGraphics())
                    {
                        for (var i = 0; i < _listBox.Items.Count; i++)
                        {
                            _listBox.Height += _listBox.GetItemHeight(i);
                            // it item width is larger than the current one
                            // set it to the new max item width
                            // GetItemRectangle does not work for me
                            // we add a little extra space by using '_'
                            var itemWidth =
                                (int) graphics.MeasureString((string) _listBox.Items[i] + "_", _listBox.Font).Width;
                            _listBox.Width = _listBox.Width < itemWidth ? itemWidth : _listBox.Width;
                        }
                    }
                }
                else
                {
                    ResetListBox();
                }
            }
            else
            {
                ResetListBox();
            }
        }

        private string GetWord()
        {
            var text = Text;
            var pos = SelectionStart;

            var posStart = text.LastIndexOf(' ', pos < 1 ? 0 : pos - 1);
            posStart = posStart == -1 ? 0 : posStart + 1;
            var posEnd = text.IndexOf(' ', pos);
            posEnd = posEnd == -1 ? text.Length : posEnd;

            var length = posEnd - posStart < 0 ? 0 : posEnd - posStart;

            return text.Substring(posStart, length);
        }

        private void InsertWord(string newTag)
        {
            /*String text = Text;
            int pos = SelectionStart;

            int posStart = text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
            posStart = (posStart == -1) ? 0 : posStart + 1;
            int posEnd = text.IndexOf(' ', pos);

            String firstPart = text.Substring(0, posStart) + newTag;
            String updatedText = firstPart + ((posEnd == -1) ? "" : text.Substring(posEnd, text.Length - posEnd));

            
            Text = updatedText;
            SelectionStart = firstPart.Length;
             */
            Text = newTag;
            IsSearching = false;
        }

        public List<string> SelectedValues
        {
            get
            {
                var result = Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                return new List<string>(result);
            }
        }

        #endregion

        #region.delay

        protected override void Dispose(bool disposing)
        {
            if (_delayedTextChangedTimer != null)
            {
                _delayedTextChangedTimer.Stop();
                if (disposing)
                    _delayedTextChangedTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnDelayedTextChanged(EventArgs e)
        {
            if (StoppedTypingTextChanged != null)
                StoppedTypingTextChanged(this, e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            InitializeDelayedTextChangedEvent();
            base.OnTextChanged(e);
        }

        private void InitializeDelayedTextChangedEvent()
        {
            if (_delayedTextChangedTimer != null)
                _delayedTextChangedTimer.Stop();

            if (_delayedTextChangedTimer == null || _delayedTextChangedTimer.Interval != StoppedTypingTimeout)
            {
                _delayedTextChangedTimer = new Timer();
                _delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
                _delayedTextChangedTimer.Interval = StoppedTypingTimeout;
            }
            _delayedTextChangedTimer.Start();
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
        {
            var timer = sender as Timer;
            timer.Stop();

            OnDelayedTextChanged(EventArgs.Empty);
        }

        #endregion
    }
}