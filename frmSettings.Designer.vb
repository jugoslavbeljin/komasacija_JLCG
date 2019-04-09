<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            Try
                MyBase.Dispose(disposing)
            Catch ex As Exception

            End Try

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSettings))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txt_server = New System.Windows.Forms.TextBox()
        Me.txt_port = New System.Windows.Forms.TextBox()
        Me.txt_username = New System.Windows.Forms.TextBox()
        Me.txt_password = New System.Windows.Forms.TextBox()
        Me.txt_tabela = New System.Windows.Forms.TextBox()
        Me.tb4 = New System.Windows.Forms.TabPage()
        Me.btn_proveriKonekciju = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btn_serverSacuvaj = New System.Windows.Forms.Button()
        Me.tb1 = New System.Windows.Forms.TabPage()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.cb_poligonske = New System.Windows.Forms.ComboBox()
        Me.txt_poligonskeTacke = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cb_podelaNaListove = New System.Windows.Forms.ComboBox()
        Me.txt_podelanalistove = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.cb_tackeObelezavanje = New System.Windows.Forms.ComboBox()
        Me.txtTackeObelezavanje = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cb_nadeladrw = New System.Windows.Forms.ComboBox()
        Me.txtNadela = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnUpdateLayers = New System.Windows.Forms.Button()
        Me.cb_ulice = New System.Windows.Forms.ComboBox()
        Me.cb_tacke = New System.Windows.Forms.ComboBox()
        Me.cb_naselja = New System.Windows.Forms.ComboBox()
        Me.cb_Table = New System.Windows.Forms.ComboBox()
        Me.cb_procRazredi = New System.Windows.Forms.ComboBox()
        Me.cb_parcele = New System.Windows.Forms.ComboBox()
        Me.txtUlice = New System.Windows.Forms.TextBox()
        Me.txtTacke = New System.Windows.Forms.TextBox()
        Me.txtCentriMoci = New System.Windows.Forms.TextBox()
        Me.txtTable = New System.Windows.Forms.TextBox()
        Me.txtProcRazredi = New System.Windows.Forms.TextBox()
        Me.txtParcele = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tb8 = New System.Windows.Forms.TabPage()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.txt_gps_vremePreseljenjeBaze = New System.Windows.Forms.TextBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.chk_Vikend = New System.Windows.Forms.CheckBox()
        Me.chk_praznik = New System.Windows.Forms.CheckBox()
        Me.txt_gps_DuzinaSmeneSati = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.txt_gps_brzinaHodanjaCovek = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txt_gps_zadrzavanjeNaTacki = New System.Windows.Forms.TextBox()
        Me.txt_gps_pocetakSnimanja = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.dt_datumPocetkaSnimanja = New System.Windows.Forms.DateTimePicker()
        Me.btnPromeniGPSZapisnik = New System.Windows.Forms.Button()
        Me.tb2 = New System.Windows.Forms.TabPage()
        Me.cb_izbaciIndustrijsku = New System.Windows.Forms.CheckBox()
        Me.txt_ogranicenjeStranka = New System.Windows.Forms.TextBox()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.txt_idko = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.cb_samoimenaBezDatuma = New System.Windows.Forms.CheckBox()
        Me.txt_pozivanje_minutiPoVlasniku = New System.Windows.Forms.TextBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.btn_templateWordPoziv = New System.Windows.Forms.Button()
        Me.cb_pozivanje_stampaParcela = New System.Windows.Forms.CheckBox()
        Me.cb_pozivanje_stampanje_poziva = New System.Windows.Forms.CheckBox()
        Me.txt_pozivanje_putanja_doPozivTemplate = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.txt_pozivanje_MaticnoNaselje = New System.Windows.Forms.TextBox()
        Me.cb_izbaciGradevinski = New System.Windows.Forms.CheckBox()
        Me.cb_zeljeUcesnika = New System.Windows.Forms.CheckBox()
        Me.txt_pozivanje_smena2kraj = New System.Windows.Forms.TextBox()
        Me.txt_pozivanje_smena2pocetak = New System.Windows.Forms.TextBox()
        Me.txt_pozivanje_smena1_kraj = New System.Windows.Forms.TextBox()
        Me.txt_pozivanje_smena1_pocetak = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.txt_pozivanje_minutiPoParceli = New System.Windows.Forms.TextBox()
        Me.txt_pozivanje_minutiPoLN = New System.Windows.Forms.TextBox()
        Me.txt_pozivanje_nultovreme = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.dt_pozivanje = New System.Windows.Forms.DateTimePicker()
        Me.btn_pozivanjePromeni = New System.Windows.Forms.Button()
        Me.tb5 = New System.Windows.Forms.TabPage()
        Me.txt_resenje_vjeddin = New System.Windows.Forms.TextBox()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.chk_resenjeKoeficijent = New System.Windows.Forms.CheckBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.cmb_resenjaPismo = New System.Windows.Forms.ComboBox()
        Me.btn_resenjeTemplate = New System.Windows.Forms.Button()
        Me.txt_resenjeTemplatePath = New System.Windows.Forms.TextBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.btn_promeniResenja = New System.Windows.Forms.Button()
        Me.chk_porazredima = New System.Windows.Forms.CheckBox()
        Me.tb6 = New System.Windows.Forms.TabPage()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txt_nadela_duzina = New System.Windows.Forms.TextBox()
        Me.txt_nadela_brinteracija = New System.Windows.Forms.TextBox()
        Me.chkBox_prikazujemTabeluZaSelekciju = New System.Windows.Forms.CheckBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.txt_zaokruzivanje = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txt_tahimetrija_zapisnik_broj_razmaka = New System.Windows.Forms.TextBox()
        Me.txt_tahimetrija_sirinazoneTrazenja = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.txt_poligonske_sirinazonetrazenja = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.btn_promeniOstalo = New System.Windows.Forms.Button()
        Me.txt_poligonskeBrOdmeranja = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.LineShape3 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.LineShape2 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.opf_diag = New System.Windows.Forms.OpenFileDialog()
        Me.sf_diag = New System.Windows.Forms.SaveFileDialog()
        Me.tb4.SuspendLayout()
        Me.tb1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tb8.SuspendLayout()
        Me.tb2.SuspendLayout()
        Me.tb5.SuspendLayout()
        Me.tb6.SuspendLayout()
        Me.SuspendLayout()
        '
        'txt_server
        '
        Me.txt_server.Location = New System.Drawing.Point(145, 16)
        Me.txt_server.Name = "txt_server"
        Me.txt_server.Size = New System.Drawing.Size(131, 20)
        Me.txt_server.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txt_server, "Server name")
        '
        'txt_port
        '
        Me.txt_port.Location = New System.Drawing.Point(145, 42)
        Me.txt_port.Name = "txt_port"
        Me.txt_port.Size = New System.Drawing.Size(131, 20)
        Me.txt_port.TabIndex = 1
        Me.txt_port.Text = "3306"
        Me.ToolTip1.SetToolTip(Me.txt_port, "Port")
        '
        'txt_username
        '
        Me.txt_username.Location = New System.Drawing.Point(145, 68)
        Me.txt_username.Name = "txt_username"
        Me.txt_username.Size = New System.Drawing.Size(131, 20)
        Me.txt_username.TabIndex = 2
        Me.txt_username.Text = "root"
        Me.ToolTip1.SetToolTip(Me.txt_username, "Korisnicko ime")
        '
        'txt_password
        '
        Me.txt_password.Location = New System.Drawing.Point(145, 94)
        Me.txt_password.Name = "txt_password"
        Me.txt_password.Size = New System.Drawing.Size(131, 20)
        Me.txt_password.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txt_password, "Lozinka")
        '
        'txt_tabela
        '
        Me.txt_tabela.Location = New System.Drawing.Point(145, 121)
        Me.txt_tabela.Name = "txt_tabela"
        Me.txt_tabela.Size = New System.Drawing.Size(131, 20)
        Me.txt_tabela.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txt_tabela, "Baza")
        '
        'tb4
        '
        Me.tb4.Controls.Add(Me.btn_proveriKonekciju)
        Me.tb4.Controls.Add(Me.Label11)
        Me.tb4.Controls.Add(Me.Label10)
        Me.tb4.Controls.Add(Me.Label9)
        Me.tb4.Controls.Add(Me.Label8)
        Me.tb4.Controls.Add(Me.Label7)
        Me.tb4.Controls.Add(Me.txt_tabela)
        Me.tb4.Controls.Add(Me.txt_password)
        Me.tb4.Controls.Add(Me.txt_username)
        Me.tb4.Controls.Add(Me.txt_port)
        Me.tb4.Controls.Add(Me.txt_server)
        Me.tb4.Controls.Add(Me.btn_serverSacuvaj)
        Me.tb4.Location = New System.Drawing.Point(4, 22)
        Me.tb4.Name = "tb4"
        Me.tb4.Size = New System.Drawing.Size(500, 318)
        Me.tb4.TabIndex = 3
        Me.tb4.Text = "Server"
        Me.tb4.UseVisualStyleBackColor = True
        '
        'btn_proveriKonekciju
        '
        Me.btn_proveriKonekciju.Location = New System.Drawing.Point(293, 121)
        Me.btn_proveriKonekciju.Name = "btn_proveriKonekciju"
        Me.btn_proveriKonekciju.Size = New System.Drawing.Size(75, 23)
        Me.btn_proveriKonekciju.TabIndex = 11
        Me.btn_proveriKonekciju.Text = "Proveri"
        Me.btn_proveriKonekciju.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(49, 121)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(84, 13)
        Me.Label11.TabIndex = 10
        Me.Label11.Text = "Database Name"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(49, 94)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 13)
        Me.Label10.TabIndex = 9
        Me.Label10.Text = "Password"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(49, 68)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(60, 13)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "User Name"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(49, 42)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(60, 13)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Server Port"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(49, 16)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(69, 13)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "Server Name"
        '
        'btn_serverSacuvaj
        '
        Me.btn_serverSacuvaj.Location = New System.Drawing.Point(417, 286)
        Me.btn_serverSacuvaj.Name = "btn_serverSacuvaj"
        Me.btn_serverSacuvaj.Size = New System.Drawing.Size(75, 23)
        Me.btn_serverSacuvaj.TabIndex = 5
        Me.btn_serverSacuvaj.Text = "Promeni"
        Me.btn_serverSacuvaj.UseVisualStyleBackColor = True
        '
        'tb1
        '
        Me.tb1.Controls.Add(Me.Label17)
        Me.tb1.Controls.Add(Me.cb_poligonske)
        Me.tb1.Controls.Add(Me.txt_poligonskeTacke)
        Me.tb1.Controls.Add(Me.Label16)
        Me.tb1.Controls.Add(Me.cb_podelaNaListove)
        Me.tb1.Controls.Add(Me.txt_podelanalistove)
        Me.tb1.Controls.Add(Me.Label15)
        Me.tb1.Controls.Add(Me.cb_tackeObelezavanje)
        Me.tb1.Controls.Add(Me.txtTackeObelezavanje)
        Me.tb1.Controls.Add(Me.Label14)
        Me.tb1.Controls.Add(Me.cb_nadeladrw)
        Me.tb1.Controls.Add(Me.txtNadela)
        Me.tb1.Controls.Add(Me.Label6)
        Me.tb1.Controls.Add(Me.Label5)
        Me.tb1.Controls.Add(Me.Label4)
        Me.tb1.Controls.Add(Me.Label3)
        Me.tb1.Controls.Add(Me.Label2)
        Me.tb1.Controls.Add(Me.Label1)
        Me.tb1.Controls.Add(Me.btnUpdateLayers)
        Me.tb1.Controls.Add(Me.cb_ulice)
        Me.tb1.Controls.Add(Me.cb_tacke)
        Me.tb1.Controls.Add(Me.cb_naselja)
        Me.tb1.Controls.Add(Me.cb_Table)
        Me.tb1.Controls.Add(Me.cb_procRazredi)
        Me.tb1.Controls.Add(Me.cb_parcele)
        Me.tb1.Controls.Add(Me.txtUlice)
        Me.tb1.Controls.Add(Me.txtTacke)
        Me.tb1.Controls.Add(Me.txtCentriMoci)
        Me.tb1.Controls.Add(Me.txtTable)
        Me.tb1.Controls.Add(Me.txtProcRazredi)
        Me.tb1.Controls.Add(Me.txtParcele)
        Me.tb1.Location = New System.Drawing.Point(4, 22)
        Me.tb1.Name = "tb1"
        Me.tb1.Padding = New System.Windows.Forms.Padding(3)
        Me.tb1.Size = New System.Drawing.Size(500, 318)
        Me.tb1.TabIndex = 0
        Me.tb1.Text = "Layers"
        Me.tb1.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(62, 194)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(92, 13)
        Me.Label17.TabIndex = 21
        Me.Label17.Text = "Poligonske tacke:"
        '
        'cb_poligonske
        '
        Me.cb_poligonske.FormattingEnabled = True
        Me.cb_poligonske.Location = New System.Drawing.Point(362, 190)
        Me.cb_poligonske.Name = "cb_poligonske"
        Me.cb_poligonske.Size = New System.Drawing.Size(121, 21)
        Me.cb_poligonske.TabIndex = 18
        '
        'txt_poligonskeTacke
        '
        Me.txt_poligonskeTacke.Location = New System.Drawing.Point(155, 191)
        Me.txt_poligonskeTacke.Name = "txt_poligonskeTacke"
        Me.txt_poligonskeTacke.Size = New System.Drawing.Size(200, 20)
        Me.txt_poligonskeTacke.TabIndex = 8
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(61, 167)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(91, 13)
        Me.Label16.TabIndex = 21
        Me.Label16.Text = "Podela na listove:"
        '
        'cb_podelaNaListove
        '
        Me.cb_podelaNaListove.FormattingEnabled = True
        Me.cb_podelaNaListove.Location = New System.Drawing.Point(362, 163)
        Me.cb_podelaNaListove.Name = "cb_podelaNaListove"
        Me.cb_podelaNaListove.Size = New System.Drawing.Size(121, 21)
        Me.cb_podelaNaListove.TabIndex = 17
        '
        'txt_podelanalistove
        '
        Me.txt_podelanalistove.Location = New System.Drawing.Point(155, 164)
        Me.txt_podelanalistove.Name = "txt_podelanalistove"
        Me.txt_podelanalistove.Size = New System.Drawing.Size(200, 20)
        Me.txt_podelanalistove.TabIndex = 7
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(34, 140)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(117, 13)
        Me.Label15.TabIndex = 21
        Me.Label15.Text = "Detaljne tacke parcela:"
        '
        'cb_tackeObelezavanje
        '
        Me.cb_tackeObelezavanje.FormattingEnabled = True
        Me.cb_tackeObelezavanje.Location = New System.Drawing.Point(362, 136)
        Me.cb_tackeObelezavanje.Name = "cb_tackeObelezavanje"
        Me.cb_tackeObelezavanje.Size = New System.Drawing.Size(121, 21)
        Me.cb_tackeObelezavanje.TabIndex = 16
        '
        'txtTackeObelezavanje
        '
        Me.txtTackeObelezavanje.Location = New System.Drawing.Point(155, 137)
        Me.txtTackeObelezavanje.Name = "txtTackeObelezavanje"
        Me.txtTackeObelezavanje.Size = New System.Drawing.Size(200, 20)
        Me.txtTackeObelezavanje.TabIndex = 6
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(108, 114)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(44, 13)
        Me.Label14.TabIndex = 21
        Me.Label14.Text = "Nadela:"
        '
        'cb_nadeladrw
        '
        Me.cb_nadeladrw.FormattingEnabled = True
        Me.cb_nadeladrw.Location = New System.Drawing.Point(362, 110)
        Me.cb_nadeladrw.Name = "cb_nadeladrw"
        Me.cb_nadeladrw.Size = New System.Drawing.Size(121, 21)
        Me.cb_nadeladrw.TabIndex = 15
        '
        'txtNadela
        '
        Me.txtNadela.Location = New System.Drawing.Point(155, 111)
        Me.txtNadela.Name = "txtNadela"
        Me.txtNadela.Size = New System.Drawing.Size(200, 20)
        Me.txtNadela.TabIndex = 5
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(118, 247)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(34, 13)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "Ulice:"
        Me.Label6.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(35, 85)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(115, 13)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Tacke - azimut nadele:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(89, 220)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 13)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Centri Moci:"
        Me.Label4.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(53, 59)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(99, 13)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Projektovane table:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(50, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 13)
        Me.Label2.TabIndex = 21
        Me.Label2.Text = "Procembeni razredi:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(47, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(103, 13)
        Me.Label1.TabIndex = 21
        Me.Label1.Text = "Parcele staro stanje:"
        '
        'btnUpdateLayers
        '
        Me.btnUpdateLayers.Location = New System.Drawing.Point(417, 286)
        Me.btnUpdateLayers.Name = "btnUpdateLayers"
        Me.btnUpdateLayers.Size = New System.Drawing.Size(75, 23)
        Me.btnUpdateLayers.TabIndex = 21
        Me.btnUpdateLayers.Text = "Promeni"
        Me.btnUpdateLayers.UseVisualStyleBackColor = True
        '
        'cb_ulice
        '
        Me.cb_ulice.FormattingEnabled = True
        Me.cb_ulice.Location = New System.Drawing.Point(362, 244)
        Me.cb_ulice.Name = "cb_ulice"
        Me.cb_ulice.Size = New System.Drawing.Size(121, 21)
        Me.cb_ulice.TabIndex = 20
        Me.cb_ulice.Visible = False
        '
        'cb_tacke
        '
        Me.cb_tacke.FormattingEnabled = True
        Me.cb_tacke.Location = New System.Drawing.Point(362, 83)
        Me.cb_tacke.Name = "cb_tacke"
        Me.cb_tacke.Size = New System.Drawing.Size(121, 21)
        Me.cb_tacke.TabIndex = 14
        '
        'cb_naselja
        '
        Me.cb_naselja.FormattingEnabled = True
        Me.cb_naselja.Location = New System.Drawing.Point(362, 217)
        Me.cb_naselja.Name = "cb_naselja"
        Me.cb_naselja.Size = New System.Drawing.Size(121, 21)
        Me.cb_naselja.TabIndex = 19
        Me.cb_naselja.Visible = False
        '
        'cb_Table
        '
        Me.cb_Table.FormattingEnabled = True
        Me.cb_Table.Location = New System.Drawing.Point(362, 56)
        Me.cb_Table.Name = "cb_Table"
        Me.cb_Table.Size = New System.Drawing.Size(121, 21)
        Me.cb_Table.TabIndex = 13
        '
        'cb_procRazredi
        '
        Me.cb_procRazredi.FormattingEnabled = True
        Me.cb_procRazredi.Location = New System.Drawing.Point(362, 32)
        Me.cb_procRazredi.Name = "cb_procRazredi"
        Me.cb_procRazredi.Size = New System.Drawing.Size(121, 21)
        Me.cb_procRazredi.TabIndex = 12
        '
        'cb_parcele
        '
        Me.cb_parcele.FormattingEnabled = True
        Me.cb_parcele.Location = New System.Drawing.Point(362, 6)
        Me.cb_parcele.Name = "cb_parcele"
        Me.cb_parcele.Size = New System.Drawing.Size(121, 21)
        Me.cb_parcele.TabIndex = 11
        '
        'txtUlice
        '
        Me.txtUlice.Location = New System.Drawing.Point(155, 245)
        Me.txtUlice.Name = "txtUlice"
        Me.txtUlice.Size = New System.Drawing.Size(200, 20)
        Me.txtUlice.TabIndex = 10
        Me.txtUlice.Visible = False
        '
        'txtTacke
        '
        Me.txtTacke.Location = New System.Drawing.Point(155, 83)
        Me.txtTacke.Name = "txtTacke"
        Me.txtTacke.Size = New System.Drawing.Size(200, 20)
        Me.txtTacke.TabIndex = 4
        '
        'txtCentriMoci
        '
        Me.txtCentriMoci.Location = New System.Drawing.Point(155, 217)
        Me.txtCentriMoci.Name = "txtCentriMoci"
        Me.txtCentriMoci.Size = New System.Drawing.Size(200, 20)
        Me.txtCentriMoci.TabIndex = 9
        Me.txtCentriMoci.Visible = False
        '
        'txtTable
        '
        Me.txtTable.Location = New System.Drawing.Point(155, 57)
        Me.txtTable.Name = "txtTable"
        Me.txtTable.Size = New System.Drawing.Size(200, 20)
        Me.txtTable.TabIndex = 2
        '
        'txtProcRazredi
        '
        Me.txtProcRazredi.Location = New System.Drawing.Point(155, 32)
        Me.txtProcRazredi.Name = "txtProcRazredi"
        Me.txtProcRazredi.Size = New System.Drawing.Size(200, 20)
        Me.txtProcRazredi.TabIndex = 1
        '
        'txtParcele
        '
        Me.txtParcele.Location = New System.Drawing.Point(155, 8)
        Me.txtParcele.Name = "txtParcele"
        Me.txtParcele.Size = New System.Drawing.Size(200, 20)
        Me.txtParcele.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tb1)
        Me.TabControl1.Controls.Add(Me.tb4)
        Me.TabControl1.Controls.Add(Me.tb8)
        Me.TabControl1.Controls.Add(Me.tb2)
        Me.TabControl1.Controls.Add(Me.tb5)
        Me.TabControl1.Controls.Add(Me.tb6)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(508, 344)
        Me.TabControl1.TabIndex = 0
        '
        'tb8
        '
        Me.tb8.Controls.Add(Me.Label48)
        Me.tb8.Controls.Add(Me.txt_gps_vremePreseljenjeBaze)
        Me.tb8.Controls.Add(Me.Label27)
        Me.tb8.Controls.Add(Me.chk_Vikend)
        Me.tb8.Controls.Add(Me.chk_praznik)
        Me.tb8.Controls.Add(Me.txt_gps_DuzinaSmeneSati)
        Me.tb8.Controls.Add(Me.Label26)
        Me.tb8.Controls.Add(Me.txt_gps_brzinaHodanjaCovek)
        Me.tb8.Controls.Add(Me.Label25)
        Me.tb8.Controls.Add(Me.Label24)
        Me.tb8.Controls.Add(Me.txt_gps_zadrzavanjeNaTacki)
        Me.tb8.Controls.Add(Me.txt_gps_pocetakSnimanja)
        Me.tb8.Controls.Add(Me.Label23)
        Me.tb8.Controls.Add(Me.Label22)
        Me.tb8.Controls.Add(Me.dt_datumPocetkaSnimanja)
        Me.tb8.Controls.Add(Me.btnPromeniGPSZapisnik)
        Me.tb8.Location = New System.Drawing.Point(4, 22)
        Me.tb8.Name = "tb8"
        Me.tb8.Size = New System.Drawing.Size(500, 318)
        Me.tb8.TabIndex = 7
        Me.tb8.Text = "GPS merenja zapisnik"
        Me.tb8.UseVisualStyleBackColor = True
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Location = New System.Drawing.Point(10, 85)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(195, 13)
        Me.Label48.TabIndex = 8
        Me.Label48.Text = "covek hoda od 3.2km/h sto je 0.89 m/s"
        '
        'txt_gps_vremePreseljenjeBaze
        '
        Me.txt_gps_vremePreseljenjeBaze.Location = New System.Drawing.Point(213, 143)
        Me.txt_gps_vremePreseljenjeBaze.Name = "txt_gps_vremePreseljenjeBaze"
        Me.txt_gps_vremePreseljenjeBaze.Size = New System.Drawing.Size(100, 20)
        Me.txt_gps_vremePreseljenjeBaze.TabIndex = 5
        Me.txt_gps_vremePreseljenjeBaze.Text = "60"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(30, 146)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(172, 13)
        Me.Label27.TabIndex = 8
        Me.Label27.Text = "Vreme za preseljenje baze (minuti): "
        '
        'chk_Vikend
        '
        Me.chk_Vikend.AutoSize = True
        Me.chk_Vikend.Location = New System.Drawing.Point(213, 206)
        Me.chk_Vikend.Name = "chk_Vikend"
        Me.chk_Vikend.Size = New System.Drawing.Size(120, 17)
        Me.chk_Vikend.TabIndex = 7
        Me.chk_Vikend.Text = "Preskacem vikend?"
        Me.chk_Vikend.UseVisualStyleBackColor = True
        '
        'chk_praznik
        '
        Me.chk_praznik.AutoSize = True
        Me.chk_praznik.Location = New System.Drawing.Point(213, 182)
        Me.chk_praznik.Name = "chk_praznik"
        Me.chk_praznik.Size = New System.Drawing.Size(128, 17)
        Me.chk_praznik.TabIndex = 6
        Me.chk_praznik.Text = "Preskacem praznike?"
        Me.chk_praznik.UseVisualStyleBackColor = True
        '
        'txt_gps_DuzinaSmeneSati
        '
        Me.txt_gps_DuzinaSmeneSati.Location = New System.Drawing.Point(213, 118)
        Me.txt_gps_DuzinaSmeneSati.Name = "txt_gps_DuzinaSmeneSati"
        Me.txt_gps_DuzinaSmeneSati.Size = New System.Drawing.Size(100, 20)
        Me.txt_gps_DuzinaSmeneSati.TabIndex = 4
        Me.txt_gps_DuzinaSmeneSati.Text = "8"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(55, 121)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(147, 13)
        Me.Label26.TabIndex = 8
        Me.Label26.Text = "Duzina rada / snimanja (sati): "
        '
        'txt_gps_brzinaHodanjaCovek
        '
        Me.txt_gps_brzinaHodanjaCovek.Location = New System.Drawing.Point(213, 67)
        Me.txt_gps_brzinaHodanjaCovek.Name = "txt_gps_brzinaHodanjaCovek"
        Me.txt_gps_brzinaHodanjaCovek.Size = New System.Drawing.Size(100, 20)
        Me.txt_gps_brzinaHodanjaCovek.TabIndex = 2
        Me.txt_gps_brzinaHodanjaCovek.Text = "0.85"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(61, 70)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(141, 13)
        Me.Label25.TabIndex = 8
        Me.Label25.Text = "Brzina hoda pri snimanju (s): "
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(75, 98)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(127, 13)
        Me.Label24.TabIndex = 8
        Me.Label24.Text = "Zadrzavanje na tacki (s): "
        '
        'txt_gps_zadrzavanjeNaTacki
        '
        Me.txt_gps_zadrzavanjeNaTacki.Location = New System.Drawing.Point(213, 92)
        Me.txt_gps_zadrzavanjeNaTacki.Name = "txt_gps_zadrzavanjeNaTacki"
        Me.txt_gps_zadrzavanjeNaTacki.Size = New System.Drawing.Size(100, 20)
        Me.txt_gps_zadrzavanjeNaTacki.TabIndex = 3
        Me.txt_gps_zadrzavanjeNaTacki.Text = "20"
        '
        'txt_gps_pocetakSnimanja
        '
        Me.txt_gps_pocetakSnimanja.Location = New System.Drawing.Point(213, 43)
        Me.txt_gps_pocetakSnimanja.Name = "txt_gps_pocetakSnimanja"
        Me.txt_gps_pocetakSnimanja.Size = New System.Drawing.Size(100, 20)
        Me.txt_gps_pocetakSnimanja.TabIndex = 1
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(73, 46)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(129, 13)
        Me.Label23.TabIndex = 8
        Me.Label23.Text = "Vreme pocetka snimanja: "
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(72, 21)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(130, 13)
        Me.Label22.TabIndex = 8
        Me.Label22.Text = "Datum pocetka snimanja: "
        '
        'dt_datumPocetkaSnimanja
        '
        Me.dt_datumPocetkaSnimanja.Location = New System.Drawing.Point(213, 19)
        Me.dt_datumPocetkaSnimanja.Name = "dt_datumPocetkaSnimanja"
        Me.dt_datumPocetkaSnimanja.Size = New System.Drawing.Size(200, 20)
        Me.dt_datumPocetkaSnimanja.TabIndex = 0
        '
        'btnPromeniGPSZapisnik
        '
        Me.btnPromeniGPSZapisnik.Location = New System.Drawing.Point(417, 286)
        Me.btnPromeniGPSZapisnik.Name = "btnPromeniGPSZapisnik"
        Me.btnPromeniGPSZapisnik.Size = New System.Drawing.Size(75, 23)
        Me.btnPromeniGPSZapisnik.TabIndex = 8
        Me.btnPromeniGPSZapisnik.Text = "Promeni"
        Me.btnPromeniGPSZapisnik.UseVisualStyleBackColor = True
        '
        'tb2
        '
        Me.tb2.Controls.Add(Me.cb_izbaciIndustrijsku)
        Me.tb2.Controls.Add(Me.txt_ogranicenjeStranka)
        Me.tb2.Controls.Add(Me.Label44)
        Me.tb2.Controls.Add(Me.txt_idko)
        Me.tb2.Controls.Add(Me.Label43)
        Me.tb2.Controls.Add(Me.cb_samoimenaBezDatuma)
        Me.tb2.Controls.Add(Me.txt_pozivanje_minutiPoVlasniku)
        Me.tb2.Controls.Add(Me.Label42)
        Me.tb2.Controls.Add(Me.btn_templateWordPoziv)
        Me.tb2.Controls.Add(Me.cb_pozivanje_stampaParcela)
        Me.tb2.Controls.Add(Me.cb_pozivanje_stampanje_poziva)
        Me.tb2.Controls.Add(Me.txt_pozivanje_putanja_doPozivTemplate)
        Me.tb2.Controls.Add(Me.Label41)
        Me.tb2.Controls.Add(Me.Label40)
        Me.tb2.Controls.Add(Me.txt_pozivanje_MaticnoNaselje)
        Me.tb2.Controls.Add(Me.cb_izbaciGradevinski)
        Me.tb2.Controls.Add(Me.cb_zeljeUcesnika)
        Me.tb2.Controls.Add(Me.txt_pozivanje_smena2kraj)
        Me.tb2.Controls.Add(Me.txt_pozivanje_smena2pocetak)
        Me.tb2.Controls.Add(Me.txt_pozivanje_smena1_kraj)
        Me.tb2.Controls.Add(Me.txt_pozivanje_smena1_pocetak)
        Me.tb2.Controls.Add(Me.Label39)
        Me.tb2.Controls.Add(Me.Label38)
        Me.tb2.Controls.Add(Me.Label37)
        Me.tb2.Controls.Add(Me.Label36)
        Me.tb2.Controls.Add(Me.Label35)
        Me.tb2.Controls.Add(Me.Label34)
        Me.tb2.Controls.Add(Me.txt_pozivanje_minutiPoParceli)
        Me.tb2.Controls.Add(Me.txt_pozivanje_minutiPoLN)
        Me.tb2.Controls.Add(Me.txt_pozivanje_nultovreme)
        Me.tb2.Controls.Add(Me.Label33)
        Me.tb2.Controls.Add(Me.Label32)
        Me.tb2.Controls.Add(Me.Label31)
        Me.tb2.Controls.Add(Me.Label30)
        Me.tb2.Controls.Add(Me.Label29)
        Me.tb2.Controls.Add(Me.dt_pozivanje)
        Me.tb2.Controls.Add(Me.btn_pozivanjePromeni)
        Me.tb2.Location = New System.Drawing.Point(4, 22)
        Me.tb2.Name = "tb2"
        Me.tb2.Padding = New System.Windows.Forms.Padding(3)
        Me.tb2.Size = New System.Drawing.Size(500, 318)
        Me.tb2.TabIndex = 8
        Me.tb2.Text = "Pozivanje"
        Me.tb2.UseVisualStyleBackColor = True
        '
        'cb_izbaciIndustrijsku
        '
        Me.cb_izbaciIndustrijsku.AutoSize = True
        Me.cb_izbaciIndustrijsku.Location = New System.Drawing.Point(194, 150)
        Me.cb_izbaciIndustrijsku.Margin = New System.Windows.Forms.Padding(2)
        Me.cb_izbaciIndustrijsku.Name = "cb_izbaciIndustrijsku"
        Me.cb_izbaciIndustrijsku.Size = New System.Drawing.Size(218, 17)
        Me.cb_izbaciIndustrijsku.TabIndex = 39
        Me.cb_izbaciIndustrijsku.Text = "Kriterijum: Izbaci INDUSTRIJSKU ZONU"
        Me.cb_izbaciIndustrijsku.UseVisualStyleBackColor = True
        '
        'txt_ogranicenjeStranka
        '
        Me.txt_ogranicenjeStranka.Location = New System.Drawing.Point(396, 237)
        Me.txt_ogranicenjeStranka.Margin = New System.Windows.Forms.Padding(2)
        Me.txt_ogranicenjeStranka.Name = "txt_ogranicenjeStranka"
        Me.txt_ogranicenjeStranka.Size = New System.Drawing.Size(53, 20)
        Me.txt_ogranicenjeStranka.TabIndex = 38
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Location = New System.Drawing.Point(280, 240)
        Me.Label44.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(110, 13)
        Me.Label44.TabIndex = 37
        Me.Label44.Text = "Ogranicenje po osobi:"
        '
        'txt_idko
        '
        Me.txt_idko.Location = New System.Drawing.Point(334, 99)
        Me.txt_idko.Name = "txt_idko"
        Me.txt_idko.Size = New System.Drawing.Size(115, 20)
        Me.txt_idko.TabIndex = 36
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Location = New System.Drawing.Point(249, 102)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(79, 13)
        Me.Label43.TabIndex = 35
        Me.Label43.Text = "ID kat. opstine:"
        '
        'cb_samoimenaBezDatuma
        '
        Me.cb_samoimenaBezDatuma.AutoSize = True
        Me.cb_samoimenaBezDatuma.Location = New System.Drawing.Point(350, 202)
        Me.cb_samoimenaBezDatuma.Name = "cb_samoimenaBezDatuma"
        Me.cb_samoimenaBezDatuma.Size = New System.Drawing.Size(148, 17)
        Me.cb_samoimenaBezDatuma.TabIndex = 34
        Me.cb_samoimenaBezDatuma.Text = "Samo imena bez datuma?"
        Me.cb_samoimenaBezDatuma.UseVisualStyleBackColor = True
        '
        'txt_pozivanje_minutiPoVlasniku
        '
        Me.txt_pozivanje_minutiPoVlasniku.Location = New System.Drawing.Point(194, 73)
        Me.txt_pozivanje_minutiPoVlasniku.Name = "txt_pozivanje_minutiPoVlasniku"
        Me.txt_pozivanje_minutiPoVlasniku.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_minutiPoVlasniku.TabIndex = 33
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Location = New System.Drawing.Point(64, 76)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(127, 13)
        Me.Label42.TabIndex = 32
        Me.Label42.Text = "Trajanjepo vlasniku (min):"
        '
        'btn_templateWordPoziv
        '
        Me.btn_templateWordPoziv.Location = New System.Drawing.Point(307, 170)
        Me.btn_templateWordPoziv.Name = "btn_templateWordPoziv"
        Me.btn_templateWordPoziv.Size = New System.Drawing.Size(26, 23)
        Me.btn_templateWordPoziv.TabIndex = 31
        Me.btn_templateWordPoziv.Text = "..."
        Me.btn_templateWordPoziv.UseVisualStyleBackColor = True
        '
        'cb_pozivanje_stampaParcela
        '
        Me.cb_pozivanje_stampaParcela.AutoSize = True
        Me.cb_pozivanje_stampaParcela.Location = New System.Drawing.Point(154, 202)
        Me.cb_pozivanje_stampaParcela.Name = "cb_pozivanje_stampaParcela"
        Me.cb_pozivanje_stampaParcela.Size = New System.Drawing.Size(190, 17)
        Me.cb_pozivanje_stampaParcela.TabIndex = 30
        Me.cb_pozivanje_stampaParcela.Text = "Stampam spisak parcela u pozivu?"
        Me.cb_pozivanje_stampaParcela.UseVisualStyleBackColor = True
        '
        'cb_pozivanje_stampanje_poziva
        '
        Me.cb_pozivanje_stampanje_poziva.AutoSize = True
        Me.cb_pozivanje_stampanje_poziva.Location = New System.Drawing.Point(33, 202)
        Me.cb_pozivanje_stampanje_poziva.Name = "cb_pozivanje_stampanje_poziva"
        Me.cb_pozivanje_stampanje_poziva.Size = New System.Drawing.Size(104, 17)
        Me.cb_pozivanje_stampanje_poziva.TabIndex = 29
        Me.cb_pozivanje_stampanje_poziva.Text = "Stampam poziv?"
        Me.cb_pozivanje_stampanje_poziva.UseVisualStyleBackColor = True
        '
        'txt_pozivanje_putanja_doPozivTemplate
        '
        Me.txt_pozivanje_putanja_doPozivTemplate.Location = New System.Drawing.Point(165, 171)
        Me.txt_pozivanje_putanja_doPozivTemplate.Name = "txt_pozivanje_putanja_doPozivTemplate"
        Me.txt_pozivanje_putanja_doPozivTemplate.Size = New System.Drawing.Size(136, 20)
        Me.txt_pozivanje_putanja_doPozivTemplate.TabIndex = 28
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Location = New System.Drawing.Point(8, 175)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(151, 13)
        Me.Label41.TabIndex = 27
        Me.Label41.Text = "Word template poziv (putanja):"
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.Location = New System.Drawing.Point(13, 101)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(119, 13)
        Me.Label40.TabIndex = 26
        Me.Label40.Text = "Naziv maticnog naselja:"
        '
        'txt_pozivanje_MaticnoNaselje
        '
        Me.txt_pozivanje_MaticnoNaselje.Location = New System.Drawing.Point(133, 99)
        Me.txt_pozivanje_MaticnoNaselje.Name = "txt_pozivanje_MaticnoNaselje"
        Me.txt_pozivanje_MaticnoNaselje.Size = New System.Drawing.Size(115, 20)
        Me.txt_pozivanje_MaticnoNaselje.TabIndex = 25
        '
        'cb_izbaciGradevinski
        '
        Me.cb_izbaciGradevinski.AutoSize = True
        Me.cb_izbaciGradevinski.Location = New System.Drawing.Point(194, 127)
        Me.cb_izbaciGradevinski.Name = "cb_izbaciGradevinski"
        Me.cb_izbaciGradevinski.Size = New System.Drawing.Size(278, 17)
        Me.cb_izbaciGradevinski.TabIndex = 24
        Me.cb_izbaciGradevinski.Text = "Kriterijum Izbaciti LN koji imaju samo gradevinski rejon"
        Me.cb_izbaciGradevinski.UseVisualStyleBackColor = True
        '
        'cb_zeljeUcesnika
        '
        Me.cb_zeljeUcesnika.AutoSize = True
        Me.cb_zeljeUcesnika.Location = New System.Drawing.Point(12, 127)
        Me.cb_zeljeUcesnika.Name = "cb_zeljeUcesnika"
        Me.cb_zeljeUcesnika.Size = New System.Drawing.Size(176, 17)
        Me.cb_zeljeUcesnika.TabIndex = 23
        Me.cb_zeljeUcesnika.Text = "Kriterijum Prosao Zelje ucesnika"
        Me.cb_zeljeUcesnika.UseVisualStyleBackColor = True
        '
        'txt_pozivanje_smena2kraj
        '
        Me.txt_pozivanje_smena2kraj.Location = New System.Drawing.Point(198, 285)
        Me.txt_pozivanje_smena2kraj.Name = "txt_pozivanje_smena2kraj"
        Me.txt_pozivanje_smena2kraj.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_smena2kraj.TabIndex = 22
        Me.txt_pozivanje_smena2kraj.Text = "0"
        '
        'txt_pozivanje_smena2pocetak
        '
        Me.txt_pozivanje_smena2pocetak.Location = New System.Drawing.Point(198, 258)
        Me.txt_pozivanje_smena2pocetak.Name = "txt_pozivanje_smena2pocetak"
        Me.txt_pozivanje_smena2pocetak.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_smena2pocetak.TabIndex = 21
        Me.txt_pozivanje_smena2pocetak.Text = "0"
        '
        'txt_pozivanje_smena1_kraj
        '
        Me.txt_pozivanje_smena1_kraj.Location = New System.Drawing.Point(76, 288)
        Me.txt_pozivanje_smena1_kraj.Name = "txt_pozivanje_smena1_kraj"
        Me.txt_pozivanje_smena1_kraj.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_smena1_kraj.TabIndex = 20
        '
        'txt_pozivanje_smena1_pocetak
        '
        Me.txt_pozivanje_smena1_pocetak.Location = New System.Drawing.Point(76, 261)
        Me.txt_pozivanje_smena1_pocetak.Name = "txt_pozivanje_smena1_pocetak"
        Me.txt_pozivanje_smena1_pocetak.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_smena1_pocetak.TabIndex = 19
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Location = New System.Drawing.Point(142, 289)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(28, 13)
        Me.Label39.TabIndex = 18
        Me.Label39.Text = "Kraj:"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Location = New System.Drawing.Point(142, 262)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(50, 13)
        Me.Label38.TabIndex = 17
        Me.Label38.Text = "Pocetak:"
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Location = New System.Drawing.Point(42, 292)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(28, 13)
        Me.Label37.TabIndex = 16
        Me.Label37.Text = "Kraj:"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Location = New System.Drawing.Point(20, 265)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(50, 13)
        Me.Label36.TabIndex = 15
        Me.Label36.Text = "Pocetak:"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Location = New System.Drawing.Point(174, 237)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(57, 13)
        Me.Label35.TabIndex = 14
        Me.Label35.Text = "SMENA 2:"
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Location = New System.Drawing.Point(8, 240)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(57, 13)
        Me.Label34.TabIndex = 13
        Me.Label34.Text = "SMENA 1:"
        '
        'txt_pozivanje_minutiPoParceli
        '
        Me.txt_pozivanje_minutiPoParceli.Location = New System.Drawing.Point(431, 73)
        Me.txt_pozivanje_minutiPoParceli.Name = "txt_pozivanje_minutiPoParceli"
        Me.txt_pozivanje_minutiPoParceli.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_minutiPoParceli.TabIndex = 12
        '
        'txt_pozivanje_minutiPoLN
        '
        Me.txt_pozivanje_minutiPoLN.Location = New System.Drawing.Point(431, 48)
        Me.txt_pozivanje_minutiPoLN.Name = "txt_pozivanje_minutiPoLN"
        Me.txt_pozivanje_minutiPoLN.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_minutiPoLN.TabIndex = 11
        '
        'txt_pozivanje_nultovreme
        '
        Me.txt_pozivanje_nultovreme.Location = New System.Drawing.Point(194, 48)
        Me.txt_pozivanje_nultovreme.Name = "txt_pozivanje_nultovreme"
        Me.txt_pozivanje_nultovreme.Size = New System.Drawing.Size(47, 20)
        Me.txt_pozivanje_nultovreme.TabIndex = 10
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Location = New System.Drawing.Point(61, 52)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(131, 13)
        Me.Label33.TabIndex = 9
        Me.Label33.Text = "Trajanje nulto vreme (min):"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Location = New System.Drawing.Point(249, 51)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(176, 13)
        Me.Label32.TabIndex = 8
        Me.Label32.Text = "Trajanje po LN (Posedovnom) (min):"
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Location = New System.Drawing.Point(303, 76)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(122, 13)
        Me.Label31.TabIndex = 7
        Me.Label31.Text = "Trajanje po parceli (min):"
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Location = New System.Drawing.Point(22, 39)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(57, 13)
        Me.Label30.TabIndex = 6
        Me.Label30.Text = "FORMILA:"
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Location = New System.Drawing.Point(22, 12)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(83, 13)
        Me.Label29.TabIndex = 5
        Me.Label29.Text = "Datum pocetka:"
        '
        'dt_pozivanje
        '
        Me.dt_pozivanje.Location = New System.Drawing.Point(126, 6)
        Me.dt_pozivanje.Name = "dt_pozivanje"
        Me.dt_pozivanje.Size = New System.Drawing.Size(200, 20)
        Me.dt_pozivanje.TabIndex = 4
        '
        'btn_pozivanjePromeni
        '
        Me.btn_pozivanjePromeni.Location = New System.Drawing.Point(417, 286)
        Me.btn_pozivanjePromeni.Name = "btn_pozivanjePromeni"
        Me.btn_pozivanjePromeni.Size = New System.Drawing.Size(75, 23)
        Me.btn_pozivanjePromeni.TabIndex = 0
        Me.btn_pozivanjePromeni.Text = "Promeni"
        Me.btn_pozivanjePromeni.UseVisualStyleBackColor = True
        '
        'tb5
        '
        Me.tb5.Controls.Add(Me.txt_resenje_vjeddin)
        Me.tb5.Controls.Add(Me.Label47)
        Me.tb5.Controls.Add(Me.chk_resenjeKoeficijent)
        Me.tb5.Controls.Add(Me.Label46)
        Me.tb5.Controls.Add(Me.cmb_resenjaPismo)
        Me.tb5.Controls.Add(Me.btn_resenjeTemplate)
        Me.tb5.Controls.Add(Me.txt_resenjeTemplatePath)
        Me.tb5.Controls.Add(Me.Label45)
        Me.tb5.Controls.Add(Me.btn_promeniResenja)
        Me.tb5.Controls.Add(Me.chk_porazredima)
        Me.tb5.Location = New System.Drawing.Point(4, 22)
        Me.tb5.Margin = New System.Windows.Forms.Padding(2)
        Me.tb5.Name = "tb5"
        Me.tb5.Size = New System.Drawing.Size(500, 318)
        Me.tb5.TabIndex = 9
        Me.tb5.Text = "Resenja"
        Me.tb5.UseVisualStyleBackColor = True
        '
        'txt_resenje_vjeddin
        '
        Me.txt_resenje_vjeddin.Location = New System.Drawing.Point(152, 120)
        Me.txt_resenje_vjeddin.Margin = New System.Windows.Forms.Padding(2)
        Me.txt_resenje_vjeddin.Name = "txt_resenje_vjeddin"
        Me.txt_resenje_vjeddin.Size = New System.Drawing.Size(43, 20)
        Me.txt_resenje_vjeddin.TabIndex = 22
        '
        'Label47
        '
        Me.Label47.AutoSize = True
        Me.Label47.Location = New System.Drawing.Point(22, 120)
        Me.Label47.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(123, 13)
        Me.Label47.TabIndex = 21
        Me.Label47.Text = "Vrednosna jedinica (din):"
        '
        'chk_resenjeKoeficijent
        '
        Me.chk_resenjeKoeficijent.AutoSize = True
        Me.chk_resenjeKoeficijent.Location = New System.Drawing.Point(254, 56)
        Me.chk_resenjeKoeficijent.Margin = New System.Windows.Forms.Padding(2)
        Me.chk_resenjeKoeficijent.Name = "chk_resenjeKoeficijent"
        Me.chk_resenjeKoeficijent.Size = New System.Drawing.Size(84, 17)
        Me.chk_resenjeKoeficijent.TabIndex = 20
        Me.chk_resenjeKoeficijent.Text = "Koeficijent 0"
        Me.chk_resenjeKoeficijent.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.chk_resenjeKoeficijent.UseVisualStyleBackColor = True
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.Location = New System.Drawing.Point(20, 89)
        Me.Label46.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(38, 13)
        Me.Label46.TabIndex = 19
        Me.Label46.Text = "Pismo:"
        '
        'cmb_resenjaPismo
        '
        Me.cmb_resenjaPismo.FormattingEnabled = True
        Me.cmb_resenjaPismo.Items.AddRange(New Object() {"Latinica", "Cirilica"})
        Me.cmb_resenjaPismo.Location = New System.Drawing.Point(62, 87)
        Me.cmb_resenjaPismo.Margin = New System.Windows.Forms.Padding(2)
        Me.cmb_resenjaPismo.Name = "cmb_resenjaPismo"
        Me.cmb_resenjaPismo.Size = New System.Drawing.Size(133, 21)
        Me.cmb_resenjaPismo.TabIndex = 18
        '
        'btn_resenjeTemplate
        '
        Me.btn_resenjeTemplate.Location = New System.Drawing.Point(381, 24)
        Me.btn_resenjeTemplate.Margin = New System.Windows.Forms.Padding(2)
        Me.btn_resenjeTemplate.Name = "btn_resenjeTemplate"
        Me.btn_resenjeTemplate.Size = New System.Drawing.Size(27, 19)
        Me.btn_resenjeTemplate.TabIndex = 17
        Me.btn_resenjeTemplate.Text = "..."
        Me.btn_resenjeTemplate.UseVisualStyleBackColor = True
        '
        'txt_resenjeTemplatePath
        '
        Me.txt_resenjeTemplatePath.Location = New System.Drawing.Point(106, 25)
        Me.txt_resenjeTemplatePath.Margin = New System.Windows.Forms.Padding(2)
        Me.txt_resenjeTemplatePath.Name = "txt_resenjeTemplatePath"
        Me.txt_resenjeTemplatePath.Size = New System.Drawing.Size(271, 20)
        Me.txt_resenjeTemplatePath.TabIndex = 16
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Location = New System.Drawing.Point(18, 28)
        Me.Label45.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(91, 13)
        Me.Label45.TabIndex = 15
        Me.Label45.Text = "Template resenja:"
        '
        'btn_promeniResenja
        '
        Me.btn_promeniResenja.Location = New System.Drawing.Point(417, 286)
        Me.btn_promeniResenja.Name = "btn_promeniResenja"
        Me.btn_promeniResenja.Size = New System.Drawing.Size(75, 23)
        Me.btn_promeniResenja.TabIndex = 14
        Me.btn_promeniResenja.Text = "Promeni"
        Me.btn_promeniResenja.UseVisualStyleBackColor = True
        '
        'chk_porazredima
        '
        Me.chk_porazredima.AutoSize = True
        Me.chk_porazredima.Location = New System.Drawing.Point(20, 56)
        Me.chk_porazredima.Name = "chk_porazredima"
        Me.chk_porazredima.Size = New System.Drawing.Size(221, 17)
        Me.chk_porazredima.TabIndex = 13
        Me.chk_porazredima.Text = "Ispisujem P po Proc. razredima u Resenju"
        Me.chk_porazredima.UseVisualStyleBackColor = True
        '
        'tb6
        '
        Me.tb6.Controls.Add(Me.Label13)
        Me.tb6.Controls.Add(Me.Label12)
        Me.tb6.Controls.Add(Me.txt_nadela_duzina)
        Me.tb6.Controls.Add(Me.txt_nadela_brinteracija)
        Me.tb6.Controls.Add(Me.chkBox_prikazujemTabeluZaSelekciju)
        Me.tb6.Controls.Add(Me.Label28)
        Me.tb6.Controls.Add(Me.txt_zaokruzivanje)
        Me.tb6.Controls.Add(Me.Label21)
        Me.tb6.Controls.Add(Me.txt_tahimetrija_zapisnik_broj_razmaka)
        Me.tb6.Controls.Add(Me.txt_tahimetrija_sirinazoneTrazenja)
        Me.tb6.Controls.Add(Me.Label20)
        Me.tb6.Controls.Add(Me.txt_poligonske_sirinazonetrazenja)
        Me.tb6.Controls.Add(Me.Label19)
        Me.tb6.Controls.Add(Me.btn_promeniOstalo)
        Me.tb6.Controls.Add(Me.txt_poligonskeBrOdmeranja)
        Me.tb6.Controls.Add(Me.Label18)
        Me.tb6.Controls.Add(Me.ShapeContainer1)
        Me.tb6.Location = New System.Drawing.Point(4, 22)
        Me.tb6.Name = "tb6"
        Me.tb6.Size = New System.Drawing.Size(500, 318)
        Me.tb6.TabIndex = 5
        Me.tb6.Text = "Ostalo"
        Me.tb6.UseVisualStyleBackColor = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(241, 207)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(78, 13)
        Me.Label13.TabIndex = 17
        Me.Label13.Text = "Offset distance"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(19, 207)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(70, 13)
        Me.Label12.TabIndex = 16
        Me.Label12.Text = "Broj interacija"
        '
        'txt_nadela_duzina
        '
        Me.txt_nadela_duzina.Location = New System.Drawing.Point(330, 206)
        Me.txt_nadela_duzina.Name = "txt_nadela_duzina"
        Me.txt_nadela_duzina.Size = New System.Drawing.Size(113, 20)
        Me.txt_nadela_duzina.TabIndex = 15
        '
        'txt_nadela_brinteracija
        '
        Me.txt_nadela_brinteracija.Location = New System.Drawing.Point(108, 207)
        Me.txt_nadela_brinteracija.Name = "txt_nadela_brinteracija"
        Me.txt_nadela_brinteracija.Size = New System.Drawing.Size(113, 20)
        Me.txt_nadela_brinteracija.TabIndex = 14
        '
        'chkBox_prikazujemTabeluZaSelekciju
        '
        Me.chkBox_prikazujemTabeluZaSelekciju.AutoSize = True
        Me.chkBox_prikazujemTabeluZaSelekciju.Location = New System.Drawing.Point(11, 176)
        Me.chkBox_prikazujemTabeluZaSelekciju.Name = "chkBox_prikazujemTabeluZaSelekciju"
        Me.chkBox_prikazujemTabeluZaSelekciju.Size = New System.Drawing.Size(173, 17)
        Me.chkBox_prikazujemTabeluZaSelekciju.TabIndex = 13
        Me.chkBox_prikazujemTabeluZaSelekciju.Text = "Prikazujem tabelu za selekciju?"
        Me.chkBox_prikazujemTabeluZaSelekciju.UseVisualStyleBackColor = True
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(8, 83)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(204, 13)
        Me.Label28.TabIndex = 11
        Me.Label28.Text = "Broj decimalnih mesta kod zaokruzivanja: "
        '
        'txt_zaokruzivanje
        '
        Me.txt_zaokruzivanje.Location = New System.Drawing.Point(218, 80)
        Me.txt_zaokruzivanje.Name = "txt_zaokruzivanje"
        Me.txt_zaokruzivanje.Size = New System.Drawing.Size(35, 20)
        Me.txt_zaokruzivanje.TabIndex = 10
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(260, 43)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(154, 13)
        Me.Label21.TabIndex = 8
        Me.Label21.Text = "Tahimetrijski zapisnik / razmak:"
        '
        'txt_tahimetrija_zapisnik_broj_razmaka
        '
        Me.txt_tahimetrija_zapisnik_broj_razmaka.Location = New System.Drawing.Point(420, 40)
        Me.txt_tahimetrija_zapisnik_broj_razmaka.Name = "txt_tahimetrija_zapisnik_broj_razmaka"
        Me.txt_tahimetrija_zapisnik_broj_razmaka.Size = New System.Drawing.Size(35, 20)
        Me.txt_tahimetrija_zapisnik_broj_razmaka.TabIndex = 7
        '
        'txt_tahimetrija_sirinazoneTrazenja
        '
        Me.txt_tahimetrija_sirinazoneTrazenja.Location = New System.Drawing.Point(218, 40)
        Me.txt_tahimetrija_sirinazoneTrazenja.Name = "txt_tahimetrija_sirinazoneTrazenja"
        Me.txt_tahimetrija_sirinazoneTrazenja.Size = New System.Drawing.Size(35, 20)
        Me.txt_tahimetrija_sirinazoneTrazenja.TabIndex = 6
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(14, 43)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(198, 13)
        Me.Label20.TabIndex = 5
        Me.Label20.Text = "Detaljne sirina zone trazenja (tahimetrija):"
        '
        'txt_poligonske_sirinazonetrazenja
        '
        Me.txt_poligonske_sirinazonetrazenja.Location = New System.Drawing.Point(368, 10)
        Me.txt_poligonske_sirinazonetrazenja.Name = "txt_poligonske_sirinazonetrazenja"
        Me.txt_poligonske_sirinazonetrazenja.Size = New System.Drawing.Size(49, 20)
        Me.txt_poligonske_sirinazonetrazenja.TabIndex = 4
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(260, 13)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(102, 13)
        Me.Label19.TabIndex = 3
        Me.Label19.Text = "Sirina zone trazenja:"
        '
        'btn_promeniOstalo
        '
        Me.btn_promeniOstalo.Location = New System.Drawing.Point(417, 286)
        Me.btn_promeniOstalo.Name = "btn_promeniOstalo"
        Me.btn_promeniOstalo.Size = New System.Drawing.Size(75, 23)
        Me.btn_promeniOstalo.TabIndex = 2
        Me.btn_promeniOstalo.Text = "Promeni"
        Me.btn_promeniOstalo.UseVisualStyleBackColor = True
        '
        'txt_poligonskeBrOdmeranja
        '
        Me.txt_poligonskeBrOdmeranja.Location = New System.Drawing.Point(201, 10)
        Me.txt_poligonskeBrOdmeranja.Name = "txt_poligonskeBrOdmeranja"
        Me.txt_poligonskeBrOdmeranja.Size = New System.Drawing.Size(52, 20)
        Me.txt_poligonskeBrOdmeranja.TabIndex = 1
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(8, 13)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(187, 13)
        Me.Label18.TabIndex = 0
        Me.Label18.Text = "Opis poligonske tacke (br Odmeranja):"
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape3, Me.LineShape2, Me.LineShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(500, 318)
        Me.ShapeContainer1.TabIndex = 9
        Me.ShapeContainer1.TabStop = False
        '
        'LineShape3
        '
        Me.LineShape3.Name = "LineShape3"
        Me.LineShape3.X1 = 17
        Me.LineShape3.X2 = 464
        Me.LineShape3.Y1 = 163
        Me.LineShape3.Y2 = 163
        '
        'LineShape2
        '
        Me.LineShape2.Name = "LineShape2"
        Me.LineShape2.X1 = 18
        Me.LineShape2.X2 = 465
        Me.LineShape2.Y1 = 129
        Me.LineShape2.Y2 = 129
        '
        'LineShape1
        '
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.X1 = 22
        Me.LineShape1.X2 = 469
        Me.LineShape1.Y1 = 68
        Me.LineShape1.Y2 = 68
        '
        'opf_diag
        '
        Me.opf_diag.FileName = "OpenFileDialog1"
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(508, 344)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Podesavanja"
        Me.tb4.ResumeLayout(False)
        Me.tb4.PerformLayout()
        Me.tb1.ResumeLayout(False)
        Me.tb1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.tb8.ResumeLayout(False)
        Me.tb8.PerformLayout()
        Me.tb2.ResumeLayout(False)
        Me.tb2.PerformLayout()
        Me.tb5.ResumeLayout(False)
        Me.tb5.PerformLayout()
        Me.tb6.ResumeLayout(False)
        Me.tb6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents tb4 As System.Windows.Forms.TabPage
    Friend WithEvents txt_tabela As System.Windows.Forms.TextBox
    Friend WithEvents txt_password As System.Windows.Forms.TextBox
    Friend WithEvents txt_username As System.Windows.Forms.TextBox
    Friend WithEvents txt_port As System.Windows.Forms.TextBox
    Friend WithEvents txt_server As System.Windows.Forms.TextBox
    Friend WithEvents btn_serverSacuvaj As System.Windows.Forms.Button
    Friend WithEvents tb1 As System.Windows.Forms.TabPage
    Friend WithEvents btnUpdateLayers As System.Windows.Forms.Button
    Friend WithEvents cb_ulice As System.Windows.Forms.ComboBox
    Friend WithEvents cb_tacke As System.Windows.Forms.ComboBox
    Friend WithEvents cb_naselja As System.Windows.Forms.ComboBox
    Friend WithEvents cb_Table As System.Windows.Forms.ComboBox
    Friend WithEvents cb_procRazredi As System.Windows.Forms.ComboBox
    Friend WithEvents cb_parcele As System.Windows.Forms.ComboBox
    Friend WithEvents txtUlice As System.Windows.Forms.TextBox
    Friend WithEvents txtTacke As System.Windows.Forms.TextBox
    Friend WithEvents txtCentriMoci As System.Windows.Forms.TextBox
    Friend WithEvents txtTable As System.Windows.Forms.TextBox
    Friend WithEvents txtProcRazredi As System.Windows.Forms.TextBox
    Friend WithEvents txtParcele As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents cb_nadeladrw As System.Windows.Forms.ComboBox
    Friend WithEvents txtNadela As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents cb_tackeObelezavanje As System.Windows.Forms.ComboBox
    Friend WithEvents txtTackeObelezavanje As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents cb_podelaNaListove As System.Windows.Forms.ComboBox
    Friend WithEvents txt_podelanalistove As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents cb_poligonske As System.Windows.Forms.ComboBox
    Friend WithEvents txt_poligonskeTacke As System.Windows.Forms.TextBox
    Friend WithEvents tb6 As System.Windows.Forms.TabPage
    Friend WithEvents btn_promeniOstalo As System.Windows.Forms.Button
    Friend WithEvents txt_poligonskeBrOdmeranja As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents txt_poligonske_sirinazonetrazenja As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txt_tahimetrija_sirinazoneTrazenja As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents txt_tahimetrija_zapisnik_broj_razmaka As System.Windows.Forms.TextBox
    Friend WithEvents tb8 As System.Windows.Forms.TabPage
    Friend WithEvents btnPromeniGPSZapisnik As System.Windows.Forms.Button
    Friend WithEvents dt_datumPocetkaSnimanja As System.Windows.Forms.DateTimePicker
    Friend WithEvents txt_gps_brzinaHodanjaCovek As System.Windows.Forms.TextBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txt_gps_zadrzavanjeNaTacki As System.Windows.Forms.TextBox
    Friend WithEvents txt_gps_pocetakSnimanja As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents txt_gps_DuzinaSmeneSati As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents chk_Vikend As System.Windows.Forms.CheckBox
    Friend WithEvents chk_praznik As System.Windows.Forms.CheckBox
    Friend WithEvents txt_gps_vremePreseljenjeBaze As System.Windows.Forms.TextBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents txt_zaokruzivanje As System.Windows.Forms.TextBox
    Friend WithEvents tb2 As System.Windows.Forms.TabPage
    Friend WithEvents btn_pozivanjePromeni As System.Windows.Forms.Button
    Friend WithEvents txt_pozivanje_minutiPoParceli As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_minutiPoLN As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_nultovreme As System.Windows.Forms.TextBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents dt_pozivanje As System.Windows.Forms.DateTimePicker
    Friend WithEvents txt_pozivanje_smena2kraj As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_smena2pocetak As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_smena1_kraj As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_smena1_pocetak As System.Windows.Forms.TextBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents cb_zeljeUcesnika As System.Windows.Forms.CheckBox
    Friend WithEvents cb_izbaciGradevinski As System.Windows.Forms.CheckBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents txt_pozivanje_MaticnoNaselje As System.Windows.Forms.TextBox
    Friend WithEvents txt_pozivanje_putanja_doPozivTemplate As System.Windows.Forms.TextBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents cb_pozivanje_stampaParcela As System.Windows.Forms.CheckBox
    Friend WithEvents cb_pozivanje_stampanje_poziva As System.Windows.Forms.CheckBox
    Friend WithEvents btn_templateWordPoziv As System.Windows.Forms.Button
    Friend WithEvents opf_diag As System.Windows.Forms.OpenFileDialog
    Friend WithEvents txt_pozivanje_minutiPoVlasniku As System.Windows.Forms.TextBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents cb_samoimenaBezDatuma As System.Windows.Forms.CheckBox
    Friend WithEvents chkBox_prikazujemTabeluZaSelekciju As System.Windows.Forms.CheckBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txt_nadela_duzina As System.Windows.Forms.TextBox
    Friend WithEvents txt_nadela_brinteracija As System.Windows.Forms.TextBox
    Friend WithEvents btn_proveriKonekciju As System.Windows.Forms.Button
    Friend WithEvents txt_idko As System.Windows.Forms.TextBox
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Private WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Private WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
    Private WithEvents LineShape2 As Microsoft.VisualBasic.PowerPacks.LineShape
    Private WithEvents LineShape3 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents txt_ogranicenjeStranka As System.Windows.Forms.TextBox
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents tb5 As System.Windows.Forms.TabPage
    Friend WithEvents btn_promeniResenja As System.Windows.Forms.Button
    Friend WithEvents chk_porazredima As System.Windows.Forms.CheckBox
    Friend WithEvents btn_resenjeTemplate As System.Windows.Forms.Button
    Friend WithEvents txt_resenjeTemplatePath As System.Windows.Forms.TextBox
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents sf_diag As System.Windows.Forms.SaveFileDialog
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents cmb_resenjaPismo As System.Windows.Forms.ComboBox
    Friend WithEvents chk_resenjeKoeficijent As System.Windows.Forms.CheckBox
    Friend WithEvents txt_resenje_vjeddin As TextBox
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents cb_izbaciIndustrijsku As CheckBox
    Friend WithEvents Label48 As System.Windows.Forms.Label
End Class
