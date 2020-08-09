﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XF_App1.Views;

namespace XF_App1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            //MainPage = new DefaultPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
