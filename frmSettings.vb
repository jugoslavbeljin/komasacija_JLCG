Public Class frmSettings
    Public doc_ As Manifold.Interop.Document


    Private Sub frmSettings_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        txtCentriMoci.Text = My.Settings.layerName_centar
        txtParcele.Text = My.Settings.layerName_parcele
        txtProcRazredi.Text = My.Settings.layerName_ProcembeniRazredi
        txtTable.Text = My.Settings.layerName_table
        txtTacke.Text = My.Settings.layerName_nadelaSmer
        txtUlice.Text = My.Settings.layerName_Ulice
        txtNadela.Text = My.Settings.layerName_ParceleNadela
        txtTackeObelezavanje.Text = My.Settings.layerName_pointTableObelezavanje

        txt_nadela_brinteracija.Text = My.Settings.nadela_brInteracija
        txt_nadela_duzina.Text = My.Settings.nadela_duzina
        txt_podelanalistove.Text = My.Settings.layerName_podelaNaListove

        txt_poligonskeTacke.Text = My.Settings.layerName_poligonskeTacke
        txt_poligonskeBrOdmeranja.Text = My.Settings.poligonske_brojOdmeranjaOpis
        txt_poligonske_sirinazonetrazenja.Text = My.Settings.poligonske_sirinaBaferZone

        txt_tahimetrija_sirinazoneTrazenja.Text = My.Settings.tahimetrija_sirinaBaferZone
        txt_tahimetrija_zapisnik_broj_razmaka.Text = My.Settings.tahimetrija_razmakIzmeduRedova

        'GPS merenje - pakovanje

        'aj sad da ovo napakujemo?!
        Dim pp_ = My.Settings.GPSMerenje_datumPocetka.Split("/")
        Dim dt_ As DateTime = New Date(pp_(2), pp_(1), pp_(0))
        dt_datumPocetkaSnimanja.Value = dt_
        'sada ovo jos da napravimo!

        txt_gps_brzinaHodanjaCovek.Text = My.Settings.GPSMerenje_brzinaHoda
        txt_gps_DuzinaSmeneSati.Text = My.Settings.GPSMerenje_duzinaRada
        txt_gps_zadrzavanjeNaTacki.Text = My.Settings.GPSMerenje_zadrzavanjeNaTacki
        txt_gps_pocetakSnimanja.Text = My.Settings.GPSMerenje_vremePocetka
        txt_gps_vremePreseljenjeBaze.Text = My.Settings.GPSMerenje_vremePreseljenjeBaze

        If My.Settings.GPSMerenje_preskacemPraznik = 1 Then
            chk_praznik.Checked = True
        Else
            chk_praznik.Checked = False
        End If

        If My.Settings.GPSMerenje_preskacemVikend = 1 Then
            chk_Vikend.Checked = True
        Else
            chk_Vikend.Checked = False
        End If

        If My.Settings.resenja_stampaProcembeniRazredi = True Then
            chk_porazredima.Checked = True
        Else
            chk_porazredima.Checked = False
        End If

        'sada idemo na server!
        Dim a_ = Split(My.Settings.mysqlConnString, ";")
        If a_.Length > 0 Then
            Dim b_ = Split(a_(0), "=")
            txt_username.Text = b_(1)
            b_ = Split(a_(1), "=")
            txt_password.Text = b_(1)
            b_ = Split(a_(2), "=")
            txt_server.Text = b_(1)
            b_ = Split(a_(3), "=")
            txt_port.Text = b_(1)
            b_ = Split(a_(4), "=")
            txt_tabela.Text = b_(1)
        End If

        txt_zaokruzivanje.Text = My.Settings.zaokruzivanjeBrojDecMesta


        'sada ide lista drawinga  e kakko to?
        For i = 0 To doc_.ComponentSet.Count - 1
            If doc_.ComponentSet.Item(i).TypeName = "Drawing" Then
                cb_parcele.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_procRazredi.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_Table.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_tacke.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_ulice.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_naselja.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_nadeladrw.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_tackeObelezavanje.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_podelaNaListove.Items.Add(doc_.ComponentSet.Item(i).Name)
                cb_poligonske.Items.Add(doc_.ComponentSet.Item(i).Name)
                'cmb_layers.Items.Add(doc_.ComponentSet.Item(i).Name)
            End If
        Next

        'komponent
        If My.Settings.komponentShowTable = 1 Then
            chkBox_prikazujemTabeluZaSelekciju.Checked = True
        Else
            chkBox_prikazujemTabeluZaSelekciju.Checked = False
        End If

        'sada idemo na parametrre vezane za pozivanje

        pp_ = My.Settings.pozivanje_pocetakDatum.Split("/")
        Try
            dt_ = New Date(pp_(2), pp_(1), pp_(0))
            dt_pozivanje.Value = dt_
        Catch ex As Exception

        End Try

        pp_ = My.Settings.GPSMerenje_datumPocetka.Split("/")

        Try
            dt_datumPocetkaSnimanja.Value = New Date(pp_(2), pp_(1), pp_(0))
        Catch ex As Exception

        End Try


        txt_pozivanje_nultovreme.Text = My.Settings.pozivanje_nultoVreme
        txt_pozivanje_minutiPoLN.Text = My.Settings.pozivanje_vremePosedovni
        txt_pozivanje_minutiPoParceli.Text = My.Settings.pozivanje_vremeBrojParcela
        txt_pozivanje_minutiPoVlasniku.Text = My.Settings.pozivanje_vremeVlasnik

        txt_pozivanje_smena1_pocetak.Text = My.Settings.pozivanje_smena1Pocetak
        txt_pozivanje_smena1_kraj.Text = My.Settings.pozivanje_smena1Kraj
        txt_pozivanje_smena2pocetak.Text = My.Settings.pozivanje_smena2Pocetak
        txt_pozivanje_smena2kraj.Text = My.Settings.pozivanje_smena2Kraj

        txt_pozivanje_MaticnoNaselje.Text = My.Settings.pozivanje_MaticnoNaselje
        txt_pozivanje_putanja_doPozivTemplate.Text = My.Settings.pozivanje_wordFileTemplatePath
        txt_ogranicenjeStranka.Text = My.Settings.ogranicenje_poStranci

        txt_resenjeTemplatePath.Text = My.Settings.resenja_wordFileTemplatePath
        cmb_resenjaPismo.SelectedItem = My.Settings.resenja_pismo
        txt_resenje_vjeddin.Text = My.Settings.resenje_vjedinice_din

        If My.Settings.resenje_koeficijent0 = 1 Then
            chk_resenjeKoeficijent.Checked = True
        Else
            chk_resenjeKoeficijent.Checked = False
        End If

        txt_idko.Text = My.Settings.pozivanje_idko

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then cb_izbaciGradevinski.Checked = True Else cb_izbaciGradevinski.Checked = False
        If My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Then cb_zeljeUcesnika.Checked = True Else cb_zeljeUcesnika.Checked = False
        If My.Settings.pozivanje_kriterijumIzbaciIndustrijsku = 1 Then cb_izbaciIndustrijsku.Checked = True Else cb_izbaciIndustrijsku.Checked = False
        If My.Settings.pozivanje_stampamOdmah = 1 Then cb_pozivanje_stampanje_poziva.Checked = True Else cb_pozivanje_stampanje_poziva.Checked = False
        If My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Then cb_pozivanje_stampaParcela.Checked = True Else cb_pozivanje_stampaParcela.Checked = False
        If My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Then cb_samoimenaBezDatuma.Checked = True Else cb_samoimenaBezDatuma.Checked = False

    End Sub

    Private Sub btnUpdateLayers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateLayers.Click
        My.Settings.layerName_centar = txtCentriMoci.Text
        My.Settings.layerName_parcele = txtParcele.Text
        My.Settings.layerName_ProcembeniRazredi = txtProcRazredi.Text
        My.Settings.layerName_table = txtTable.Text
        My.Settings.layerName_nadelaSmer = txtTacke.Text
        My.Settings.layerName_Ulice = txtUlice.Text
        My.Settings.layerName_ParceleNadela = txtNadela.Text
        My.Settings.layerName_pointTableObelezavanje = txtTackeObelezavanje.Text
        My.Settings.layerName_podelaNaListove = txt_podelanalistove.Text
        My.Settings.layerName_poligonskeTacke = txt_poligonskeTacke.Text
        My.Settings.Save()
    End Sub

    Private Sub cb_parcele_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_parcele.SelectedIndexChanged
        txtParcele.Text = cb_parcele.SelectedItem
    End Sub

    Private Sub cb_procRazredi_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_procRazredi.SelectedIndexChanged
        txtProcRazredi.Text = cb_procRazredi.SelectedItem
    End Sub

    Private Sub cb_Table_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_Table.SelectedIndexChanged
        txtTable.Text = cb_Table.SelectedItem
    End Sub

    Private Sub cb_tacke_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_tacke.SelectedIndexChanged
        txtTacke.Text = cb_tacke.SelectedItem
    End Sub

    Private Sub cb_ulice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_ulice.SelectedIndexChanged
        txtUlice.Text = cb_ulice.SelectedItem
    End Sub

    Private Sub cb_naselja_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_naselja.SelectedIndexChanged
        txtCentriMoci.Text = cb_naselja.SelectedItem
    End Sub

    Private Sub cb_nadeladrw_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_nadeladrw.SelectedIndexChanged
        txtNadela.Text = cb_nadeladrw.SelectedItem
    End Sub

    Private Sub cb_tackeObelezavanje_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_tackeObelezavanje.SelectedIndexChanged
        txtTackeObelezavanje.Text = cb_tackeObelezavanje.SelectedItem
    End Sub

    Private Sub cb_podelaNaListove_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_podelaNaListove.SelectedIndexChanged
        txt_podelanalistove.Text = cb_podelaNaListove.SelectedItem
    End Sub

    Private Sub cb_poligonske_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cb_poligonske.SelectedIndexChanged
        txt_poligonskeTacke.Text = cb_poligonske.SelectedItem
    End Sub
    Private Sub btn_serverSacuvaj_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_serverSacuvaj.Click
        If txt_server.Text <> "" And txt_port.Text <> "" And txt_password.Text <> "" And txt_username.Text <> "" And txt_tabela.Text <> "" Then
            My.Settings.mysqlConnString = "User ID=" & txt_username.Text & ";Password=" & txt_password.Text & ";Host=" & txt_server.Text & ";Port=" & txt_port.Text & ";Database=" & txt_tabela.Text & ";"
            My.Settings.Save()
        End If
    End Sub

    Private Sub btn_promeniOstalo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_promeniOstalo.Click
        'brojOdmeranjaOpis
        If txt_poligonskeBrOdmeranja.Text <> "" Then
            My.Settings.poligonske_brojOdmeranjaOpis = txt_poligonskeBrOdmeranja.Text
            My.Settings.Save()
        End If
        If txt_poligonske_sirinazonetrazenja.Text <> "" Then
            My.Settings.poligonske_sirinaBaferZone = txt_poligonske_sirinazonetrazenja.Text
            My.Settings.Save()
        End If
        If txt_tahimetrija_sirinazoneTrazenja.Text <> "" Then
            My.Settings.tahimetrija_sirinaBaferZone = txt_tahimetrija_sirinazoneTrazenja.Text
            My.Settings.Save()
        End If
        If txt_tahimetrija_zapisnik_broj_razmaka.Text <> "" Then
            My.Settings.tahimetrija_razmakIzmeduRedova = txt_tahimetrija_zapisnik_broj_razmaka.Text
            My.Settings.Save()
        End If
        If txt_zaokruzivanje.Text <> "" Then
            My.Settings.zaokruzivanjeBrojDecMesta = Val(txt_zaokruzivanje.Text)
            My.Settings.Save()
        End If

        If chk_porazredima.Checked = True Then
            My.Settings.resenja_stampaProcembeniRazredi = "True"
            My.Settings.Save()
        Else
            My.Settings.resenja_stampaProcembeniRazredi = "False"
            My.Settings.Save()
        End If
        If chkBox_prikazujemTabeluZaSelekciju.Checked = True Then
            'da prikazujem to je 1
            My.Settings.komponentShowTable = 1
        Else
            'neprikazujem to je 0
            My.Settings.komponentShowTable = 0
        End If

        If txt_nadela_brinteracija.Text <> "" Then
            My.Settings.nadela_brInteracija = txt_nadela_brinteracija.Text
            My.Settings.Save()
        End If
        If txt_nadela_duzina.Text <> "" Then
            My.Settings.nadela_duzina = txt_nadela_duzina.Text
            My.Settings.Save()
        End If

    End Sub
    Private Sub btnPromeniGPSZapisnik_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPromeniGPSZapisnik.Click

        'aj sad da ovo napakujemo?!
        Dim dt_ As DateTime = dt_datumPocetkaSnimanja.Value 'sada ostaje format?!

        'sada ovo jos da napravimo!
        My.Settings.GPSMerenje_datumPocetka = dt_.Day & "/" & dt_.Month & "/" & dt_.Year

        My.Settings.GPSMerenje_brzinaHoda = txt_gps_brzinaHodanjaCovek.Text
        My.Settings.GPSMerenje_duzinaRada = txt_gps_DuzinaSmeneSati.Text
        My.Settings.GPSMerenje_zadrzavanjeNaTacki = txt_gps_zadrzavanjeNaTacki.Text
        My.Settings.GPSMerenje_vremePocetka = txt_gps_pocetakSnimanja.Text
        My.Settings.GPSMerenje_vremePreseljenjeBaze = txt_gps_vremePreseljenjeBaze.Text

        If chk_praznik.Checked = True Then
            My.Settings.GPSMerenje_preskacemPraznik = 1
        Else
            My.Settings.GPSMerenje_preskacemPraznik = 0
        End If

        If chk_Vikend.Checked = True Then
            My.Settings.GPSMerenje_preskacemVikend = 1
        Else
            My.Settings.GPSMerenje_preskacemVikend = 0
        End If

    End Sub

    Private Sub btn_pozivanjePromeni_Click(sender As System.Object, e As System.EventArgs) Handles btn_pozivanjePromeni.Click

        Dim dt_ As DateTime = dt_pozivanje.Value 'sada ostaje format?!

        'sada ovo jos da napravimo!
        My.Settings.pozivanje_pocetakDatum = dt_.Day & "/" & dt_.Month & "/" & dt_.Year

        My.Settings.pozivanje_nultoVreme = txt_pozivanje_nultovreme.Text
        My.Settings.pozivanje_vremePosedovni = txt_pozivanje_minutiPoLN.Text
        My.Settings.pozivanje_vremeBrojParcela = txt_pozivanje_minutiPoParceli.Text
        My.Settings.pozivanje_vremeVlasnik = txt_pozivanje_minutiPoVlasniku.Text

        My.Settings.pozivanje_smena1Pocetak = txt_pozivanje_smena1_pocetak.Text
        My.Settings.pozivanje_smena1Kraj = txt_pozivanje_smena1_kraj.Text
        My.Settings.pozivanje_smena2Pocetak = txt_pozivanje_smena2pocetak.Text
        My.Settings.pozivanje_smena2Kraj = txt_pozivanje_smena2kraj.Text

        My.Settings.pozivanje_MaticnoNaselje = txt_pozivanje_MaticnoNaselje.Text

        If cb_izbaciGradevinski.Checked = True Then My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Else My.Settings.pozivanje_kriterijum_izbaciGradevinski = 0

        '1 -izbaci one koji su bili!
        If cb_zeljeUcesnika.Checked = True Then My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Else My.Settings.pozivanje_kriterijum_zeljeUcesnika = 0

        If cb_izbaciIndustrijsku.Checked = True Then My.Settings.pozivanje_kriterijumIzbaciIndustrijsku = 1 Else My.Settings.pozivanje_kriterijumIzbaciIndustrijsku = 0

        My.Settings.pozivanje_wordFileTemplatePath = txt_pozivanje_putanja_doPozivTemplate.Text

        If cb_pozivanje_stampanje_poziva.Checked = True Then My.Settings.pozivanje_stampamOdmah = 1 Else My.Settings.pozivanje_stampamOdmah = 0
        If cb_pozivanje_stampaParcela.Checked = True Then My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Else My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 0
        If cb_samoimenaBezDatuma.Checked = True Then My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Else My.Settings.pozivanje_pisemSamoImenaBezVremena = 0

        My.Settings.pozivanje_idko = txt_idko.Text

        My.Settings.ogranicenje_poStranci = txt_ogranicenjeStranka.Text

        My.Settings.Save()

    End Sub

    Private Sub btn_templateWordPoziv_Click(sender As System.Object, e As System.EventArgs) Handles btn_templateWordPoziv.Click
        opf_diag.FileName = ""
        opf_diag.Filter = "Word file (*.doc)|*.doc"
        opf_diag.ShowDialog()
        txt_pozivanje_putanja_doPozivTemplate.Text = opf_diag.FileName
    End Sub

    Private Sub btn_proveriKonekciju_Click(sender As Object, e As System.EventArgs) Handles btn_proveriKonekciju.Click

        Try
            Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection("User ID=" & txt_username.Text & ";Password=" & txt_password.Text & ";Host=" & txt_server.Text & ";Port=" & txt_port.Text & ";Database=" & txt_tabela.Text & ";")
            conn_.Open()
            MsgBox("Veza uspostavljena - parametri dobri")
            conn_.Close()
            conn_ = Nothing
        Catch ex As Exception
            MsgBox("Proverite - nema veze sa bazom")
        End Try
        
    End Sub

    Private Sub btn_resenjeTemplate_Click(sender As Object, e As System.EventArgs) Handles btn_resenjeTemplate.Click
        opf_diag.FileName = ""
        opf_diag.Filter = "Word File (*.doc)|*.doc"
        opf_diag.ShowDialog()

        If opf_diag.FileName = "" Then
            MsgBox("pronadite tempalte resenja")
        Else
            txt_resenjeTemplatePath.Text = opf_diag.FileName
        End If

    End Sub

    Private Sub btn_promeniResenja_Click(sender As Object, e As System.EventArgs) Handles btn_promeniResenja.Click
        If txt_resenjeTemplatePath.Text <> "" Then
            My.Settings.resenja_wordFileTemplatePath = txt_resenjeTemplatePath.Text
        End If
        If chk_porazredima.Checked = True Then
            My.Settings.resenja_stampaProcembeniRazredi = True
        Else
            My.Settings.resenja_stampaProcembeniRazredi = False
        End If
        If chk_resenjeKoeficijent.Checked = True Then
            My.Settings.resenje_koeficijent0 = 1
        Else
            My.Settings.resenje_koeficijent0 = 0
        End If

        If txt_resenje_vjeddin.Text <> "" Then
            My.Settings.resenje_vjedinice_din = txt_resenje_vjeddin.Text
        End If

        My.Settings.resenja_pismo = cmb_resenjaPismo.SelectedItem
    End Sub


End Class