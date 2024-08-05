using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using BinaryDocumentClassification.Classes;
using BinaryDocumentClassification.Model;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using System.ComponentModel;

namespace BinaryDocumentClassification.ViewModel
{
    public class VmImage : VmBase
    {
        private ICommand _upCommand;
        private ICommand _rightCommand;
        private ICommand _leftCommand;
        private ICommand _spaceCommand;
        private string _currentImage = "";
        private ModelFileMetadata _modelMetadataList;
        private string _filesPath;
        private string _classOne;
        private string _classTwo;
        private string _currentClassName;
        private string _currentForeGround;

        public VmImage() {
            _filesPath = ConfigurationManager.AppSettings["path"];
            _modelMetadataList = new ModelFileMetadata();
            //Console.WriteLine("path");
            //_currentImage = "235485717.TIF";
            _classOne = ConfigurationManager.AppSettings["classOne"];
            _classTwo = ConfigurationManager.AppSettings["classTwo"];

            //Get metadataList
            if (File.Exists(Path.Combine(_filesPath, "metadata.csv"))) {
                using (FileStream fs = new FileStream(Path.Combine(_filesPath, "metadata.csv"), FileMode.Open)) {
                    using (StreamReader sr = new StreamReader(fs)) {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            //Console.WriteLine(line);
                            string[] splitLine = line.Split(';');
                            FileMetadata fmd = new FileMetadata() {
                                ClassName = splitLine[2].Trim(),
                                Processed = Convert.ToBoolean(splitLine[1].Trim())
                            };
                            _modelMetadataList.MetadataList.Add(splitLine[0], fmd);
                        }
                    }
                }
            } else {
                //Get all Filenames
                DirectoryInfo d = new DirectoryInfo(_filesPath);
                FileInfo[] listFi = d.GetFiles("*.TIF");
                foreach (FileInfo fi in listFi) {
                    _modelMetadataList.MetadataList.Add(fi.Name, new FileMetadata() { ClassName = _classOne, Processed = false });
                }
                listFi = null;
                _modelMetadataList.SaveChanges();
            }
            setFirstUnprocessedImage();
        }

        private string getFirstUnprocessedImageName() {
            int valIndex = _modelMetadataList.MetadataList.IndexOfValue(_modelMetadataList.MetadataList.Values.Where(m => m.Processed == false).FirstOrDefault());
            return _modelMetadataList.MetadataList.Keys[valIndex];
        }

        private string getNextImageName() {
            string lastImageName = _modelMetadataList.MetadataList.Last().Key;
            if (_currentImage == "" || _currentImage == null) {
                return _modelMetadataList.MetadataList.First().Key;
            }
            if (_currentImage == lastImageName) {
                return _modelMetadataList.MetadataList.Last().Key;
            }
            return _modelMetadataList.MetadataList.Keys.Where(m => m.CompareTo(_currentImage) > 0).FirstOrDefault();
        }

        private string getPreviousImageName() {
            string firstImageName = _modelMetadataList.MetadataList.First().Key;

            if (_currentImage == firstImageName || _currentImage == "" || _currentImage == null) {
                return firstImageName;
            }

            return _modelMetadataList.MetadataList.Keys[_modelMetadataList.MetadataList.IndexOfKey(_currentImage)-1];
        }

        private string getOppositeClass(string currentClass) {
            if (currentClass == _classOne) {
                return _classTwo;
            } else {
                return _classOne;
            }
        }

        private void toggleCurrentImageClass() {
            string newClass = getOppositeClass(_modelMetadataList.MetadataList[_currentImage].ClassName);
            _modelMetadataList.MetadataList[_currentImage].ClassName = newClass;
            _modelMetadataList.SaveChanges();
            CurrentClassName = newClass;
        }

        public ICommand RightCommand {
            get {
                return _rightCommand ?? (_rightCommand = new ActionCommand(() => setNextImage()));
            }
        }

        public ICommand LeftCommand {
            get {
                return _leftCommand ?? (_leftCommand = new ActionCommand(() => setPreviousImage()));
            }
        }

        public ICommand UpCommand {
            get {
                return _upCommand ?? (_upCommand = new ActionCommand(() => setFirstUnprocessedImage()));
            }
        }

        public ICommand SpaceCommand {
            get {
                return _spaceCommand ?? (_spaceCommand = new ActionCommand(() => toggleCurrentImageClass()));
            }
        }

        public void setPreviousImage() {
            CurrentImage = getPreviousImageName();
            if (_modelMetadataList.MetadataList[_currentImage].ClassName != _classOne && _modelMetadataList.MetadataList[_currentImage].ClassName != _classTwo) {
                _modelMetadataList.MetadataList[_currentImage].ClassName = _classOne;
                _modelMetadataList.SaveChanges();
            }
            CurrentClassName = _modelMetadataList.MetadataList[_currentImage].ClassName;
            _modelMetadataList.MetadataList[_currentImage].Processed = true;
            GC.Collect();
        }

        public void setNextImage() {
            CurrentImage = getNextImageName();
            if (_modelMetadataList.MetadataList[_currentImage].ClassName != _classOne && _modelMetadataList.MetadataList[_currentImage].ClassName != _classTwo) {
                _modelMetadataList.MetadataList[_currentImage].ClassName = _classOne;
                _modelMetadataList.SaveChanges();
            }
            CurrentClassName = _modelMetadataList.MetadataList[_currentImage].ClassName;
            _modelMetadataList.MetadataList[_currentImage].Processed = true;
            GC.Collect();
        }

        public void setFirstUnprocessedImage() {
            CurrentImage = getFirstUnprocessedImageName();
            if (_modelMetadataList.MetadataList[_currentImage].ClassName != _classOne && _modelMetadataList.MetadataList[_currentImage].ClassName != _classTwo) {
                _modelMetadataList.MetadataList[_currentImage].ClassName = _classOne;
                _modelMetadataList.SaveChanges();
            }
            CurrentClassName = _modelMetadataList.MetadataList[_currentImage].ClassName;
            _modelMetadataList.MetadataList[_currentImage].Processed = true;
            GC.Collect();
        }

        public string CurrentForeground {
            get {
                return _currentForeGround;
            }
            set {
                _currentForeGround = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentForeground"));
            }
        }

        public string CurrentClassName {
            get {
                return _modelMetadataList.MetadataList[_currentImage].ClassName;
            }
            set {
                _currentClassName = value;
                CurrentForeground = (_currentClassName == _classOne) ? "Red" : "Green";
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentClassName"));
            }
        }

        public string CurrentImage {
            get { 
                return Path.Combine(_filesPath, _currentImage); 
            } 
            set {
                _currentImage = value;
                CurrentForeground = (_currentClassName == _classOne) ? "Red" : "Green";
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentImage"));
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentImageName"));
            }
        }

        public string CurrentImageName {
            get {
                return _currentImage;
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e) {
            _modelMetadataList.SaveChanges();
        }
    }
}
