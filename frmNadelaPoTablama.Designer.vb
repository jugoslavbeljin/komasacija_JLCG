<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNadelaPoTablama
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNadelaPoTablama))
        Me.sf_diag = New System.Windows.Forms.SaveFileDialog()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pb1 = New System.Windows.Forms.ToolStripProgressBar()
        Me.pb2 = New System.Windows.Forms.ToolStripProgressBar()
        Me.tss_label = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.ddl_ttpSpisakTabli = New System.Windows.Forms.ComboBox()
        Me.txt_Help = New System.Windows.Forms.TextBox()
        Me.gridTableNadela = New System.Windows.Forms.DataGridView()
        Me.tt = New System.Windows.Forms.ToolTip(Me.components)
        Me.opf_diag = New System.Windows.Forms.OpenFileDialog()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.NadelaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RucneTableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OdrediVrednostPoligonaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportujUDKPNadeluToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.IzBazeKomtableNadelaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IzFileaDirektnoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DataVrednostToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ProveraPovrsinaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StampaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NadelaIObelezavanjeTableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NadelaToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ObelezavanjeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AsdfToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DfasdfToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.gridTableNadela, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.pb1, Me.pb2, Me.tss_label})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 475)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(743, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'pb1
        '
        Me.pb1.Name = "pb1"
        Me.pb1.Size = New System.Drawing.Size(100, 16)
        '
        'pb2
        '
        Me.pb2.Name = "pb2"
        Me.pb2.Size = New System.Drawing.Size(100, 16)
        '
        'tss_label
        '
        Me.tss_label.Name = "tss_label"
        Me.tss_label.Size = New System.Drawing.Size(0, 17)
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.gridTableNadela)
        Me.SplitContainer1.Size = New System.Drawing.Size(743, 451)
        Me.SplitContainer1.SplitterDistance = 246
        Me.SplitContainer1.TabIndex = 3
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.ddl_ttpSpisakTabli)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.txt_Help)
        Me.SplitContainer2.Size = New System.Drawing.Size(246, 451)
        Me.SplitContainer2.SplitterDistance = 80
        Me.SplitContainer2.TabIndex = 0
        '
        'ddl_ttpSpisakTabli
        '
        Me.ddl_ttpSpisakTabli.FormattingEnabled = True
        Me.ddl_ttpSpisakTabli.Location = New System.Drawing.Point(13, 4)
        Me.ddl_ttpSpisakTabli.Name = "ddl_ttpSpisakTabli"
        Me.ddl_ttpSpisakTabli.Size = New System.Drawing.Size(231, 21)
        Me.ddl_ttpSpisakTabli.TabIndex = 0
        '
        'txt_Help
        '
        Me.txt_Help.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txt_Help.Location = New System.Drawing.Point(0, 0)
        Me.txt_Help.Multiline = True
        Me.txt_Help.Name = "txt_Help"
        Me.txt_Help.Size = New System.Drawing.Size(246, 367)
        Me.txt_Help.TabIndex = 0
        '
        'gridTableNadela
        '
        Me.gridTableNadela.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gridTableNadela.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridTableNadela.Location = New System.Drawing.Point(0, 0)
        Me.gridTableNadela.Name = "gridTableNadela"
        Me.gridTableNadela.Size = New System.Drawing.Size(493, 451)
        Me.gridTableNadela.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NadelaToolStripMenuItem, Me.StampaToolStripMenuItem, Me.AsdfToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(743, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'NadelaToolStripMenuItem
        '
        Me.NadelaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RucneTableToolStripMenuItem, Me.ToolStripSeparator1, Me.IzBazeKomtableNadelaToolStripMenuItem, Me.IzFileaDirektnoToolStripMenuItem, Me.DataVrednostToolStripMenuItem, Me.ToolStripSeparator2, Me.ProveraPovrsinaToolStripMenuItem})
        Me.NadelaToolStripMenuItem.Name = "NadelaToolStripMenuItem"
        Me.NadelaToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.NadelaToolStripMenuItem.Text = "Nadela"
        '
        'RucneTableToolStripMenuItem
        '
        Me.RucneTableToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OdrediVrednostPoligonaToolStripMenuItem, Me.ImportujUDKPNadeluToolStripMenuItem})
        Me.RucneTableToolStripMenuItem.Name = "RucneTableToolStripMenuItem"
        Me.RucneTableToolStripMenuItem.ShowShortcutKeys = False
        Me.RucneTableToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.RucneTableToolStripMenuItem.Text = "Rucne table"
        '
        'OdrediVrednostPoligonaToolStripMenuItem
        '
        Me.OdrediVrednostPoligonaToolStripMenuItem.Name = "OdrediVrednostPoligonaToolStripMenuItem"
        Me.OdrediVrednostPoligonaToolStripMenuItem.Size = New System.Drawing.Size(258, 22)
        Me.OdrediVrednostPoligonaToolStripMenuItem.Text = "Odredi vrednost poligona"
        '
        'ImportujUDKPNadeluToolStripMenuItem
        '
        Me.ImportujUDKPNadeluToolStripMenuItem.Name = "ImportujUDKPNadeluToolStripMenuItem"
        Me.ImportujUDKPNadeluToolStripMenuItem.Size = New System.Drawing.Size(258, 22)
        Me.ImportujUDKPNadeluToolStripMenuItem.Text = "Importuj rucnu tablu u DKP nadelu"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(180, 6)
        '
        'IzBazeKomtableNadelaToolStripMenuItem
        '
        Me.IzBazeKomtableNadelaToolStripMenuItem.Name = "IzBazeKomtableNadelaToolStripMenuItem"
        Me.IzBazeKomtableNadelaToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.IzBazeKomtableNadelaToolStripMenuItem.Text = "Iz baze komasacije"
        '
        'IzFileaDirektnoToolStripMenuItem
        '
        Me.IzFileaDirektnoToolStripMenuItem.Name = "IzFileaDirektnoToolStripMenuItem"
        Me.IzFileaDirektnoToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.IzFileaDirektnoToolStripMenuItem.Text = "Iz CSV datoteke "
        '
        'DataVrednostToolStripMenuItem
        '
        Me.DataVrednostToolStripMenuItem.Name = "DataVrednostToolStripMenuItem"
        Me.DataVrednostToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.DataVrednostToolStripMenuItem.Text = "Konstantna vrednost"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(180, 6)
        '
        'ProveraPovrsinaToolStripMenuItem
        '
        Me.ProveraPovrsinaToolStripMenuItem.Name = "ProveraPovrsinaToolStripMenuItem"
        Me.ProveraPovrsinaToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.ProveraPovrsinaToolStripMenuItem.Text = "Provera povrsina"
        '
        'StampaToolStripMenuItem
        '
        Me.StampaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NadelaIObelezavanjeTableToolStripMenuItem, Me.NadelaToolStripMenuItem1, Me.ObelezavanjeToolStripMenuItem, Me.NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem})
        Me.StampaToolStripMenuItem.Name = "StampaToolStripMenuItem"
        Me.StampaToolStripMenuItem.Size = New System.Drawing.Size(59, 20)
        Me.StampaToolStripMenuItem.Text = "Stampa"
        '
        'NadelaIObelezavanjeTableToolStripMenuItem
        '
        Me.NadelaIObelezavanjeTableToolStripMenuItem.Name = "NadelaIObelezavanjeTableToolStripMenuItem"
        Me.NadelaIObelezavanjeTableToolStripMenuItem.Size = New System.Drawing.Size(329, 22)
        Me.NadelaIObelezavanjeTableToolStripMenuItem.Text = "Nadela i obelezavanje table"
        '
        'NadelaToolStripMenuItem1
        '
        Me.NadelaToolStripMenuItem1.Name = "NadelaToolStripMenuItem1"
        Me.NadelaToolStripMenuItem1.Size = New System.Drawing.Size(329, 22)
        Me.NadelaToolStripMenuItem1.Text = "Nadela"
        '
        'ObelezavanjeToolStripMenuItem
        '
        Me.ObelezavanjeToolStripMenuItem.Name = "ObelezavanjeToolStripMenuItem"
        Me.ObelezavanjeToolStripMenuItem.Size = New System.Drawing.Size(329, 22)
        Me.ObelezavanjeToolStripMenuItem.Text = "Obelezavanje"
        '
        'AsdfToolStripMenuItem
        '
        Me.AsdfToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DfasdfToolStripMenuItem})
        Me.AsdfToolStripMenuItem.Name = "AsdfToolStripMenuItem"
        Me.AsdfToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.AsdfToolStripMenuItem.Text = "asdf"
        '
        'DfasdfToolStripMenuItem
        '
        Me.DfasdfToolStripMenuItem.Name = "DfasdfToolStripMenuItem"
        Me.DfasdfToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.DfasdfToolStripMenuItem.Text = "dfasdf"
        '
        'NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem
        '
        Me.NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem.Name = "NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem"
        Me.NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem.Size = New System.Drawing.Size(329, 22)
        Me.NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem.Text = "Nadela i obelezavanje table u jednom map file-u"
        '
        'frmNadelaPoTablama
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(743, 497)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmNadelaPoTablama"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Komasacija - Nadela"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.gridTableNadela, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents sf_diag As System.Windows.Forms.SaveFileDialog
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents gridTableNadela As System.Windows.Forms.DataGridView
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents ddl_ttpSpisakTabli As System.Windows.Forms.ComboBox
    Friend WithEvents tt As System.Windows.Forms.ToolTip
    Friend WithEvents opf_diag As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pb2 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents pb1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tss_label As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents txt_Help As System.Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents NadelaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StampaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IzBazeKomtableNadelaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DataVrednostToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IzFileaDirektnoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ProveraPovrsinaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NadelaIObelezavanjeTableToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NadelaToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ObelezavanjeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RucneTableToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents OdrediVrednostPoligonaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImportujUDKPNadeluToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AsdfToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DfasdfToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
