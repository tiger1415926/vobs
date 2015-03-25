using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace BackGroundWorkerTemplate
{
    class MainWindowViewModel : NotificationObject
    {
        private BackgroundWorker _bgWorker;
        private bool m_task_running;


        public MainWindowViewModel()
        {
            m_task_running = false;

            // command handle.
            this.BtnStart = new DelegateCommand(btnStart, canBtnStart);
            this.BtnStop = new DelegateCommand(btnStop, canBtnStop);

            // Backgournd worker initialization.
            this._bgWorker = new BackgroundWorker();
            this._bgWorker.WorkerReportsProgress = true;
            this._bgWorker.WorkerSupportsCancellation = true;
            this._bgWorker.DoWork += _bgWorker_DoWork;
            this._bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            this._bgWorker.ProgressChanged += _bgWorker_ProgressChanged;


            this.updateUIState();
        }

        #region Static Property for Binding Usage.

        public string Title
        {
            get { return "Background Worker template program"; }
        }

        public int TaskPercentage { private set; get; }
        public int TestNumber { private set; get; }

        #endregion

        #region Command Delegate handle.

        public DelegateCommand BtnStart { get; private set; }
        public DelegateCommand BtnStop { get; private set; }


        private void btnStart()
        {
            m_task_running = true;
            if (!this._bgWorker.IsBusy)
                this._bgWorker.RunWorkerAsync();
            this.updateUIState();
        }

        private bool canBtnStart()
        {
            return !m_task_running;
        }

        private void btnStop()
        {
            m_task_running = false;
            if (this._bgWorker.IsBusy)
            {
                this._bgWorker.CancelAsync();
            }
        }

        private bool canBtnStop()
        {
            return m_task_running;
        }

        #endregion

        #region Background Worker Deleage method.

        private void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // test program for report number.
            for (int i = 0; i < 200; i++)
            {
                this.TestNumber = i;
                this.RaisePropertyChanged(() => this.TestNumber);
                System.Threading.Thread.Sleep(100);
                this._bgWorker.ReportProgress((i + 1) * 100 / 200);

                // check if cancelled by the user.
                if (_bgWorker.CancellationPending)
                {
                    this.TestNumber = 0;
                    this.RaisePropertyChanged(() => this.TestNumber);
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.TaskPercentage = 0;
                this.RaisePropertyChanged(() => this.TaskPercentage);
            }
            else
            {
                this.TaskPercentage = 100;
                this.RaisePropertyChanged(() => this.TaskPercentage);
            }
            this.m_task_running = false;
            this.updateUIState();
        }

        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.TaskPercentage = e.ProgressPercentage;
            this.RaisePropertyChanged(() => this.TaskPercentage);
        }

        #endregion

        #region Private method

        private void updateUIState()
        {
            this.BtnStart.RaiseCanExecuteChanged();
            this.BtnStop.RaiseCanExecuteChanged();
        }

        #endregion

    }
}
