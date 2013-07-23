﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePassLib.Utility;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private KeeAgentExt mExt;

    public ManageDialog(KeeAgentExt aExt)
    {
      InitializeComponent();

      if (Type.GetType("Mono.Runtime") == null)
        Icon = Properties.Resources.KeeAgent_icon;
      else
        Icon = Properties.Resources.KeeAgent_icon_mono;

      // update title depending on Agent Mode
      mExt = aExt;
      if (mExt.mAgent is Agent) {
        Text += Translatable.TitleSuffixAgentMode;
      } else {
        Text += Translatable.TitleSuffixClientMode;
      }
      keyInfoView.SetAgent(mExt.mAgent);
    }

    protected override void OnShown (EventArgs e)
    {
      base.OnShown (e);
      keyInfoView.dataGridView.ClearSelection();
    }

    private void addButtonFromFileMenuItem_Click(object sender, EventArgs e)
    {
      keyInfoView.ShowFileOpenDialog();
    }

    private void addButtonFromKeePassMenuItem_Click(object sender, EventArgs e)
    {
      var openDatabaseCount =
        mExt.mPluginHost.MainWindow.DocumentManager.GetOpenDatabases().Count;
      if (openDatabaseCount == 0) {
        MessageService.ShowWarning("No open databases found.",
          "Please open or unlock a database and then try again.");
        return;
      }

      var showConstraintControls = !(mExt.mAgent is PageantClient);
      var entryPicker =
        new EntryPickerDialog(mExt.mPluginHost, showConstraintControls);
      var result = entryPicker.ShowDialog();
      if (result == DialogResult.OK) {
        try {
          mExt.AddEntry(entryPicker.SelectedEntry, entryPicker.Constraints);
        } catch (Exception) {
          // error message already shown
        }
      }
      if (mExt.mAgent is AgentClient) {
        keyInfoView.ReloadKeyListView();
      }
    }

    private void keyInfoView_AddFromFileHelpRequested(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpAddFromFile);
    }

    private void ManageDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      OnHelpRequested();
    }

    private void ManageDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      OnHelpRequested();
      e.Cancel = true;
    }

    private void OnHelpRequested()
    {
      Process.Start(Properties.Resources.WebHelpKeeAgentManager);
    }
  }
}
