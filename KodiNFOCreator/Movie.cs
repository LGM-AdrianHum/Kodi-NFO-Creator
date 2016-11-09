using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LegeDoos.KodiNFOCreator
{
    /// <summary>
    ///     Class representing the NFO file
    /// </summary>
    public class Movie
    {
        private string FileName { get; set; }

        #region.members

        //for def see http://wiki.xbmc.org/index.php?title=NFO_files/movies
        public string Title { get; set; }

        public string Originaltitle
        {
            get { return Title; }
        }

        public string Sorttitle
        {
            get { return Title; }
        }

        public int? Year { get; set; }

        public bool ShouldSerializeyear()
        {
            return Year.HasValue;
        }

        public string Outline { get; set; }
        public string Plot { get; set; }
        public int? Runtime { get; set; }

        public bool ShouldSerializeruntime()
        {
            return Runtime.HasValue;
        }

        public static XmlSerializer Xs;

        #endregion

        #region.constructors

        public Movie()
        {
        }

        public Movie(string file)
        {
            if (!Init(file))
            {
                throw new Exception($"File {file} not found");
            }
            Xs = new XmlSerializer(typeof(Movie));
        }

        #endregion

        #region.methods

        private bool Init(string file)
        {
            if (File.Exists(file))
            {
                FileName = file;
                return true;
            }
            return false;
        }

        public bool SaveNfo(string targetFileName)
        {
            var retVal = false;

            if (File.Exists(targetFileName))
            {
                if (
                    MessageBox.Show(string.Format("File {0} exists, overwrite?", targetFileName), "Confirm",
                        MessageBoxButtons.YesNo) == DialogResult.No)
                    return retVal;
            }

            //fix empty values
            if (Outline != null && Outline.Length == 0)
            {
                Outline = null;
            }
            if (Plot != null && Plot.Length == 0)
            {
                Plot = null;
            }

            if (Directory.Exists(Path.GetDirectoryName(targetFileName)))
            {
                using (var sw = new StreamWriter(targetFileName))
                {
                    Xs.Serialize(sw, this);
                }
                retVal = true;
            }
            return retVal;
        }

        #endregion
    }
}