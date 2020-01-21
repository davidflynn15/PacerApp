using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Timers;

namespace PacerApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        RunViewModel runVM;
 
        public MainPage()
        {
            InitializeComponent();

            runVM = new RunViewModel();
            BindingContext = runVM;     //to bind viewmodel to view
        }

        private async void BtnGoAsync_Clicked(object sender, EventArgs e)
        {
            if (BtnGo.Text == "Go")
            {
                //if (runVM.IsRestart) runVM.ResetValues();

                runVM.stopwatch.Start();
                BtnGo.Text = "Stop";
                BtnGo.BackgroundColor = Color.FromHex("D91A34");
                                
                await runVM.StartRun();
            }
            else
            {
                runVM.stopwatch.Stop();
                runVM.IsDone = true;
                runVM.IsRestart = true;
                //not now
                //runVM.SerializeRunData();

                BtnGo.Text = "Go";
                BtnGo.BackgroundColor = Color.FromHex("#80BF8A");
            }
        }
    }
}
