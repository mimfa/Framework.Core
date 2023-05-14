using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MiMFa.Service;

namespace MiMFa.General
{
    public class ButtonAction
    {
        public static void Exit(Form mainForm = null)
        {
            if (mainForm == null) Application.Exit();
            else mainForm.Close();
        }

        private static Size FirstSize = new Size(0, 0);
        private static Point FirstLocation = new Point(0, 0);
        /// <summary>
        /// Change Windows State
        /// </summary>
        /// <param name="state"> 0: Minimize, 1: Normal, 2: Normal Maximize, 3: Maximize</param>
        public static void WindowsState(Form mainForm,int state)//windows state
        {
            if (FirstSize == new Size(0, 0))
                if (mainForm.Size == Screen.PrimaryScreen.Bounds.Size)
                {
                    FirstSize = new Size(800,600);
                    FirstLocation = mainForm.Location;
                }
                else
                {
                    FirstSize = mainForm.Size;
                    FirstLocation = mainForm.Location;
                }
            switch (state)
            {
                case 0:
                    mainForm.WindowState = FormWindowState.Minimized;
                    break;
                case 1:
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.Location = FirstLocation;
                    mainForm.Size = FirstSize;
                    break;
                case 3:
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.Location = new Point(0, 0);
                    mainForm.Size = Screen.PrimaryScreen.Bounds.Size;
                    break;
                default:
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.Location = new Point(0, 0);
                    mainForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height-30);
                    break;
            }
        }
        public static void WindowsState(Form mainForm)//windows state
        {
            if (mainForm.Size == Screen.PrimaryScreen.Bounds.Size)
                WindowsState(mainForm,1);
            else if (mainForm.Size.Width <= FirstSize.Width && mainForm.Size.Height <= FirstSize.Height)
                WindowsState(mainForm, 2);
            else WindowsState(mainForm, 3);
        }
        public static void Minimize(Form mainForm)
        {
            mainForm.WindowState = FormWindowState.Minimized;
        }
        public static void SelectAll(Form mainForm)
        {
            try
            {
                ((TextBox)ControlService.FindFocusedControl(mainForm)).SelectAll();
            }
            catch { }
        }
        public static void Copy(Form mainForm)
        {
            try
            {
                Clipboard.SetText(((TextBox)ControlService.FindFocusedControl(mainForm)).SelectedText);
            }
            catch { }
        }
        public static void Paste(Form mainForm)
        {
            try
            {
                ((TextBox)ControlService.FindFocusedControl(mainForm)).SelectedText = Clipboard.GetText();
            }
            catch { }
        }
        public static void Cut(Form mainForm)
        {
            try
            {
                TextBox tb = (TextBox)ControlService.FindFocusedControl(mainForm);
                Clipboard.SetText(tb.SelectedText);
                tb.SelectedText = "";
            }
            catch { }
        }
        public static void SelectAllIn(Control control)
        {
            try
            {
                ((TextBox)control).SelectAll();
            }
            catch { }
        }
        public static void CopyIn(Control control)
        {
            try
            {
                Clipboard.SetText(((TextBox)control).SelectedText);
            }
            catch { }
        }
        public static void PasteIn(Control control)
        {
            try
            {
                ((TextBox)control).SelectedText = Clipboard.GetText();
            }
            catch { }
        }
        public static void CutIn(Control control)
        {
            try
            {
                TextBox tb = (TextBox)control;
                Clipboard.SetText(tb.SelectedText);
                tb.SelectedText = "";
            }
            catch { }
        }
    }
}
