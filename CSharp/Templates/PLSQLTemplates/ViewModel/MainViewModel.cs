
using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using PLSQLTemplates.SelectionModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetadataService;
using MetadataService.Model;

namespace PLSQLTemplates.ViewModel
{

    public class MainViewModel : ReactiveObject
    {

        #region Commands

        private RelayCommand _createDatabaseCommand;
        private RelayCommand _updateMetadataCommand;
        private RelayCommand _exitCommand;

        public RelayCommand CreateDatabaseCommand
        {
            get { return _createDatabaseCommand; }
        }

        public RelayCommand UpdateMetadataCommand
        {
            get { return _updateMetadataCommand; }
        }

        public RelayCommand ExitCommand
        {
            get { return _exitCommand; }
        }
        #endregion

        #region Properties

        private bool _isWorking = false;
        private readonly Repository _repository;
        private readonly MethodSelectionModel _methodSelection;
        private ObservableAsPropertyHelper<List<Method>> _Matches;
        private string _SearchTerm;

        public string SearchTerm
        {
            get { return _SearchTerm; }
            set { this.RaiseAndSetIfChanged(p => p.SearchTerm, value); }
        }
        public List<Method> Matches
        {
            get
            {
                if (_Matches != null)
                {
                    var result = _Matches.Value;
                    return result;
                }
                return null;
            }
        }
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                //if (value == _isWorking) return;

                this.RaisePropertyChanging(x => x.IsWorking);
                _isWorking = value;
                this.RaisePropertyChanged(x => x.IsWorking);

                //_updateMetadataCommand.RaiseCanExecuteChanged();
            }
        }
        public PlSqlWrapper PLSQLWrapper
        {
            get
            {
                if (_methodSelection.SelectedMethod == null) return null;
                return _repository.getPLSQLWrapper(_methodSelection.SelectedMethod);
            }
        }
        public Method SelectedMethod
        {
            get
            {


                if (_methodSelection.SelectedMethod == null)
                    return null;

                return _methodSelection.SelectedMethod;
            }
            set
            {
                this.RaisePropertyChanging(x => x.SelectedMethod);
                this.RaisePropertyChanging(x => x.PLSQLWrapper);
                _methodSelection.SelectedMethod = value;
                this.RaisePropertyChanged(x => x.SelectedMethod);
                this.RaisePropertyChanged(x => x.PLSQLWrapper);

            }
        }

        #endregion

        public MainViewModel()
        {

            // Register commands    
            _createDatabaseCommand = new RelayCommand(() => {});
            _updateMetadataCommand = new RelayCommand(() => { _repository.Load();}, () => !IsWorking);
            _exitCommand = new RelayCommand(() => { App.Current.Shutdown(); });

            // Build Servicies    
            _repository = new Repository();
            _methodSelection = new MethodSelectionModel();


var searchTerms = this.ObservableForProperty(x => x.SearchTerm).Value().Throttle(TimeSpan.FromSeconds(0.5));
var searchResults = searchTerms.SelectMany(searchTerm => _repository.findMethods(searchTerm));

var latestMatches = searchTerms.CombineLatest(searchResults, (searchTerm, searchResult) =>
        searchResult.SearchTerm != searchTerm
        ? null : searchResult.Matches)
        .Where(matches => matches != null);

_Matches = latestMatches.ToProperty(this, x => x.Matches);
searchTerms.Subscribe(onNext : x => TaskStarted() ,onCompleted : () => TaskCompleted() );

           // DoUpdateMetadata();

        }

        private void TaskStarted()
        {
            Console.WriteLine("Started");
            IsWorking = true;
        }

        private void TaskCompleted() {
            Console.WriteLine("Completed");
          IsWorking = false;
        }

        private async Task<SearchResult> findMethods(string searchTerm)
        {
            return await _repository.findMethods(searchTerm);
        }

    }
}