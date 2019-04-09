Imports System.Globalization
Imports Microsoft.Office.Interop
Imports System.Text.RegularExpressions
'Imports OSGeo.OGR
Imports System.Xml
'Imports NetTopologySuite
'Imports MicroStationDGN
'Imports Microsoft.Office.Interop.Word
'Imports Microsoft.Office.Interop.Excel
Imports System.IO
Imports System.Text

<Runtime.InteropServices.Guid("6B17B9A3-5559-42C6-81EE-B16560620174")>
Public Class frmMain

    Public P_, x_, y_ As Double
    'Public provider_ As SharpMap.Data.Providers.GeometryProvider

    Private Sub mnu_file_Ucitaj_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_file_Ucitaj.Click

        opf_diag.FileName = ""
        opf_diag.Filter = "Map File|*.map|All Files|*.*"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        ManifoldCtrl.Visible = True
        '        axSharpMap.Visible = False
        Me.Cursor = Cursors.WaitCursor
        ManifoldCtrl.DocumentPath = opf_diag.FileName
        napuniLayersByMap()
        Me.Text = " Komasacija - dokument: " & ManifoldCtrl.DocumentPath

        dodajFileUListuOtvorenihFileova(ManifoldCtrl.DocumentPath)

        Try
            Dim node_ = layersTV.Nodes(0).Nodes(0)
            Dim a_ = node_.FullPath.Split("\")
            ManifoldCtrl.set_Component(a_(1))
        Catch ex As Exception

        End Try

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub dodajFileUListuOtvorenihFileova(ByVal filePath_ As String)

        Dim p_ = My.Settings.openFiles.Split(";")
        Dim ima_ As Boolean = False

        For i = 0 To p_.Length - 1
            If p_(i) = filePath_ Then
                ima_ = True
                Exit For
            End If
        Next

        'sada bi u stvari trebalo da se napravi listo do nekog file-a recimo da cuva 10 file-a! to mi deluje ok

        If ima_ = False Then
            'dodajes

            My.Settings.openFiles = My.Settings.openFiles & ";" & filePath_
            My.Settings.Save()

        End If

        'sada idemo da skratimo
        If My.Settings.openFiles.Split(";").Length > 10 Then
            'skracujes
            Dim k_ = My.Settings.openFiles.Split(";")
            Dim pera_ As String = ""
            For i = 0 To 10
                pera_ = pera_ & ";" & k_(k_.Length - 1 - i)
            Next
            My.Settings.openFiles = pera_
            My.Settings.Save()
        End If

    End Sub

    Private Sub napuniLayersByMap()
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        layersTV.Nodes.Clear()
        Dim nodeMap_ As New TreeNode

        nodeMap_.Text = "Definisane karte"
        layersTV.Nodes.Add(nodeMap_)

        Dim comps_ As Manifold.Interop.ComponentSet = doc.ComponentSet
        For i = 0 To comps_.Count - 1
            If comps_.Item(i).TypeName = "Map" Then
                Dim mNode = nodeMap_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                'sada za ovu mapu treba da dodas foldere i u njih smestis layer-e
                Dim map_ As Manifold.Interop.Map = comps_.Item(i)
                For k = 0 To map_.LayerSet.Count - 1
                    'treba mi folder!
                    Dim componen_ As Manifold.Interop.Component = map_.LayerSet.Item(k).Component
                    Dim folderName2_ As Manifold.Interop.Folder = componen_.Folder
                    Dim folderName_ = "bez Foldera"
                    If Not IsNothing(folderName2_) Then
                        folderName_ = folderName2_.Name
                    Else
                        Dim comp2_ As Manifold.Interop.Component = componen_.Owner
                        If Not IsNothing(comp2_) Then
                            Dim fd_ As Manifold.Interop.Folder = comp2_.Folder
                            If Not IsNothing(fd_) Then folderName_ = fd_.Name
                            fd_ = Nothing
                        End If
                        comp2_ = Nothing
                    End If

                    'sada dali ovaj folder vec postoji!
                    Dim prosao_ As Boolean = False
                    For l = 0 To mNode.Nodes.Count - 1
                        If mNode.Nodes.Item(l).Text = folderName_ Then
                            Dim pnode2 = mNode.Nodes.Item(l).Nodes.Add(map_.LayerSet.Item(k).Component.ID, map_.LayerSet.Item(k).Component.Name)
                            If map_.LayerSet.Item(k).Component.TypeName = "Drawing" Then pnode2.ImageIndex = 1 Else pnode2.ImageIndex = 2
                            If map_.LayerSet.Item(k).Visible = True Then pnode2.Checked = True Else pnode2.Checked = False
                            pnode2 = Nothing : prosao_ = True
                            Exit For
                        End If
                    Next
                    If prosao_ = False Then
                        'kreiras folder i u njega ubacujes
                        Dim pnode2 = mNode.Nodes.Add(folderName_)
                        Dim pnode3 = pnode2.Nodes.Add(map_.LayerSet.Item(k).Component.ID, map_.LayerSet.Item(k).Component.Name)
                        If map_.LayerSet.Item(k).Component.TypeName = "Drawing" Then pnode2.ImageIndex = 1 Else pnode2.ImageIndex = 2
                        If map_.LayerSet.Item(k).Visible = True Then pnode2.Checked = True : pnode3.Checked = True Else pnode2.Checked = False : pnode3.Checked = False
                        pnode2 = Nothing : pnode3 = Nothing
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub napuniLayers()
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        layersTV.Nodes.Clear()

        Dim nodeDrw_ As New TreeNode : nodeDrw_.Text = "Drawings" : layersTV.Nodes.Add(nodeDrw_)
        Dim nodeImg_ As New TreeNode : nodeImg_.Text = "Images" : layersTV.Nodes.Add(nodeImg_)
        Dim nodeMap_ As New TreeNode : nodeMap_.Text = "Maps" : layersTV.Nodes.Add(nodeMap_)
        Dim nodeQ_ As New TreeNode : nodeQ_.Text = "Query" : layersTV.Nodes.Add(nodeQ_)
        Dim nodeTab_ As New TreeNode : nodeTab_.Text = "Table" : layersTV.Nodes.Add(nodeTab_)

        Try
            Dim comps_ As Manifold.Interop.ComponentSet = doc.ComponentSet
            For i = 0 To comps_.Count - 1
                Dim p_ = comps_.Item(i).TypeName
                Select Case comps_.Item(i).TypeName
                    Case "Drawing"
                        nodeDrw_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                    Case "Labels"
                        nodeDrw_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                    Case "Image"
                        nodeImg_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                    Case "Map"
                        Dim dnode_ = nodeMap_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                        'ako je mapa trebalo bi da doda i sve ostale layere!
                        Dim map_ As Manifold.Interop.Map = comps_.Item(i)
                        For k = 0 To map_.LayerSet.Count - 1
                            Dim pnode = dnode_.Nodes.Add(map_.LayerSet.Item(k).Component.ID, map_.LayerSet.Item(k).Component.Name)
                            If map_.LayerSet.Item(k).Visible = True Then
                                pnode.Checked = True
                            Else
                                pnode.Checked = False
                            End If
                            pnode = Nothing
                        Next
                        map_ = Nothing
                    Case "Query"
                        nodeQ_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                    Case "Table"
                        If comps_.Item(i).Owner Is Nothing Then
                            nodeTab_.Nodes.Add(comps_.Item(i).ID, comps_.Item(i).Name)
                        End If
                End Select

            Next
            comps_ = Nothing
        Catch ex As Exception

        End Try

        doc = Nothing

    End Sub

    Private Sub mnu_komasacija_staroStanje_kfmss_kreirajINapuni_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_komasacija_staroStanje_kfmss_kreirajINapuni.Click

        'ulaz u funkciju treba da budu:
        '1. drawing procembenih razreda (nazivi polja su: procembeni i faktor)
        '2. drawing sa parcelama

        'izlaz je tabela kfmss koja je napunjena sa zaokruzenim povrsinama i izravnata na katastarsko stanje
        'kada su u pitanju procembeni razredi generalno na nivou komasacije je napravljeno da ima 8 procembenih razreda ne vise

        '//ucitavnje map file-a
        If ManifoldCtrl.get_Document.Name = "" Then
            MsgBox("Ucitaj map file")
            Dock = Nothing
            Exit Sub
        End If
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document


        Try
            sf_diag.FileName = ""
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            sf_diag.FileName = "KFSS.map"
            sf_diag.Title = "Upisite naziv za izlazni Map File"
            sf_diag.ShowDialog()
            If sf_diag.FileName = "" Then
                MsgBox("Kraj operacije")
                doc = Nothing
                Exit Sub
            Else
                doc = ManifoldCtrl.get_Document
                doc.SaveAs(sf_diag.FileName)
                ManifoldCtrl.DocumentPath = sf_diag.FileName
            End If
            ManifoldCtrl.Refresh()
        Catch ex As Exception
            MsgBox("Dokument je read onlyu Zatvorite ga u Manifoldu i ponovo pokrenite ovu funkciju.")
            FileClose()
            doc = Nothing
            Exit Sub
        End Try

        doc = ManifoldCtrl.get_Document
        'proveris dali postoji tabela : par_pr_raz_tab

        Dim tbl_ As Manifold.Interop.Table

        Try
            doc.ComponentSet.Remove("parc_pr_razred")
        Catch ex As Exception

        End Try

        '//proveri da li postoje drawing sa parcelama i procembenim razredima
        Dim drw_parcele As Manifold.Interop.Drawing
        Try
            drw_parcele = doc.ComponentSet.Item(My.Settings.layerName_parcele)
        Catch ex As Exception
            MsgBox("Podesavanje za parcele nije dobro - sredite to pa pokrenite ponovo")
            Exit Sub
        End Try

        Dim drw_procRazredi As Manifold.Interop.Drawing
        Try
            drw_procRazredi = doc.ComponentSet.Item(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Podesavanje za procembene razrede nije dobro - sredite to pa pokrenite ponovo")
            Exit Sub
        End Try

        'podesavanje polja na copy-copy
        Dim i As Integer
        pb1.Value = 1
        Try
            tbl_ = drw_procRazredi.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                End If
            Next
        Catch ex2 As Exception
            'MsgBox(ex2.Message)
        End Try

        Try
            tbl_ = drw_parcele.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
        Catch ex1 As Exception
            'MsgBox(ex1.Message)
        End Try

        'kreiranje topologije za procembene razrede i parcele
        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology
        topPRazredi.Bind(drw_procRazredi)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drw_parcele)
        topParcele.Build()

        'pravi presek izmedu procembenih razreda i parcela u stvari formira knjigu fonda mase starog stanja - grafiku
        topParcele.DoIntersect(topPRazredi, "parc_pr_razred")

        Dim topParcPrRaz As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcPrRaz.Bind(doc.ComponentSet.Item(doc.ComponentSet.ItemByName("parc_pr_razred"))) : topParcPrRaz.Build()

        doc.Save()

        '//kreiras tabelu ako je nema a ako je ima onda brises
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "drop table IF EXISTS kom_kfmss"
        comm_.ExecuteNonQuery()
        comm_.CommandText = "CREATE TABLE kom_kfmss (`VSuma` double not null, PRIMARY KEY (`idParc`), INDEX (`idParc`)) select SKATOPST as idko, idParc, kom_parcele.brParceleF as brParcele, (ifnull(hektari,0)*10000+ifnull(ari,0)*100+ifnull(metri,0)) as Povrsina, kom_parcele.RASPRAVNIZAPISNIK, kom_parcele.ukomasaciji as SR,  B.Oznaka_ as OZNAKA, 0 Prazred_1, 0 Prazred_2,0 Prazred_3,0 Prazred_4,0 Prazred_5,0 Prazred_6,0 Prazred_7,0 Prazred_8,0 Prazred_Neplodno, 0 grafP, 0.00 VSuma, 0 obrisan  FROM kom_parcele LEFT OUTER JOIN (SELECT GROUP_CONCAT(OZNAKA) as Oznaka_,brParceleF FROM (select distinct OZNAKA,brParceleF FROM kom_parcele LEFT OUTER JOIN kat_kultura on kat_kultura.idKulture=kom_parcele.SKULTURE WHERE DEOPARCELE=1 order by brParceleF,OZNAKA) as A GROUP BY brParceleF) as B on B.brParceleF=kom_parcele.brParceleF WHERE DEOPARCELE=0 and obrisan=0"
        comm_.ExecuteNonQuery()


        Dim popravljam As Boolean = False


        Dim qrKFMSS2 As Manifold.Interop.Query = doc.NewQuery("kfmss_povrsine")

        qrKFMSS2.Text = "TRANSFORM sum([Area (I)]) as suma_ SELECT [brParcele], sum([Area (I)]) as Povrsina FROM [Parc_pr_razred] group by [brParcele] PIVOT [Parc_pr_razred].[procembeni]"
        doc.Save()
        qrKFMSS2.RunEx(True)

        'sada treba peglanje!
        tbl_ = qrKFMSS2.Table

        pb1.Maximum = tbl_.RecordSet.Count + 1
        For i = 0 To tbl_.RecordSet.Count - 1
            pb1.Value = i
            Dim stinsert_ As String = "update kom_kfmss set grafP=" & tbl_.RecordSet(i).DataText(2) & ","
            For j = 2 To tbl_.ColumnSet.Count - 1
                If tbl_.ColumnSet(j).Name = "NP" Then
                    stinsert_ = stinsert_ & "prazred_neplodno" & "=" & Val(tbl_.RecordSet(i).DataText(j + 1)) & ","
                Else
                    stinsert_ = stinsert_ & "prazred_" & tbl_.ColumnSet(j).Name & "=" & Val(tbl_.RecordSet(i).DataText(j + 1)) & ","
                End If
            Next
            stinsert_ = stinsert_.Substring(0, stinsert_.Length - 1)
            stinsert_ = stinsert_ & " where " & My.Settings.parcele_fieldName_brParcele & "='" & tbl_.RecordSet(i).DataText(1) & "'"
            comm_.CommandText = stinsert_
            comm_.ExecuteNonQuery()
            stinsert_ = ""
        Next

        pb1.Value = 5

        'sada ostaje da izravnas
        If MsgBox("Da li popravljam povrsinu za neslaganje sa grafikom?", MsgBoxStyle.OkCancel, "Upit") = 1 Then
            Dim stupdate As String
            'If popravljam = True Then
            stupdate = "update kom_kfmss set kom_kfmss.prazred_1=kom_kfmss.prazred_1*(povrsina/grafP) where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_2=kom_kfmss.prazred_2*(povrsina/grafP)  where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_3=kom_kfmss.prazred_3*(povrsina/grafP)  where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_4=kom_kfmss.prazred_4*(povrsina/grafP)  where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_5=kom_kfmss.prazred_5*(povrsina/grafP)  where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_6=kom_kfmss.prazred_6*(povrsina/grafP) where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_7=kom_kfmss.prazred_7*(povrsina/grafP) where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_8=kom_kfmss.prazred_8*(povrsina/grafP) where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "update kom_kfmss set kom_kfmss.prazred_neplodno=kom_kfmss.prazred_neplodno*(povrsina/grafP) where grafP<>0"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            stupdate = "UPDATE kom_kfmss set vsuma=(Prazred_1+Prazred_2*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=2) +Prazred_3*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=3)+Prazred_4*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=4)+Prazred_5*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=5)+Prazred_6*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=6)+Prazred_7*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=7)+Prazred_8*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=8))"
            comm_.CommandText = stupdate
            comm_.ExecuteNonQuery()
            conn_.Close()
        End If

        If MsgBox("Da li postavljam objekte na nulu?", MsgBoxStyle.OkCancel, "Upit") = 1 Then
            comm_.CommandText = "update kom_kfmss set prazred_neplodno=povrsina, prazred_1=0, prazred_2=0, prazred_3=0, prazred_4=0, prazred_5=0, prazred_6=0, prazred_7=0,prazred_8=0, vsuma=0 where SR=3"
            Try
                conn_.Open()
            Catch ex As Exception

            End Try

            comm_.ExecuteNonQuery()
            conn_.Close()
        End If

        comm_ = Nothing : conn_ = Nothing

        qrKFMSS2 = Nothing
        doc.Save()

        MsgBox("Kraj. Proverite kontrolu da li su sve parcele u nuli.")

    End Sub
    Private Sub mnu_komasacija_nadela_pokreniNadelu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_komasacija_nadela_pokreniNadelu.Click
        Dim frm_ As New frmNadelaPoTablama
        frm_.ShowDialog()
    End Sub

    Private Sub mnu_podesavanja_komasacije_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_podesavanja_komasacije.Click
        Dim frm As New frmSettings
        frm.doc_ = ManifoldCtrl.get_Document
        frm.Show()
    End Sub

    Private Sub mnu_komasacija_novoStanje_RacunanjePovrsinaTabliIObjekata_NultoStanje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_komasacija_novoStanje_RacunanjePovrsinaTabliIObjekata_NultoStanje.Click
        'trebaju ti dve stvari : layeru kome se nalaze definisane table 
        'iz baze ti treba broj lista-ovo bi trebalo da se doda
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If doc.FullName = "" Then
            MsgBox("Greska : Ucitajte map file prvo")
            Exit Sub
        End If

        sf_diag.FileName = "Spisak_tacaka_tabli_i_procembenih_razreda.txt"
        sf_diag.Title = "Izlazni file za spisak Tacaka"
        sf_diag.DefaultExt = "Text files (*.txt)|*.txt"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then
            Exit Sub
        End If


        'priprema izlazne file-ove za stampu koji se korsite za upisivanje koordinata tabli i objekata
        Dim freeFile_ As Integer = FreeFile()
        Try
            FileOpen(freeFile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Catch ex As Exception
            FileClose()
            freeFile_ = FreeFile()
            FileOpen(freeFile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        End Try

        sf_diag.FileName = "tableSpisakKontrola.txt"
        sf_diag.Title = "Kontrolni file"
        sf_diag.DefaultExt = "Text files (*.txt)|*.txt"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then
            Exit Sub
        End If

        Dim freefile2_ As Integer = FreeFile()
        Try
            FileOpen(freefile2_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Catch ex As Exception
            FileClose()
            freefile2_ = FreeFile()
            FileOpen(freefile2_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        End Try

        'proverava podesavanja da li postoji drawing sa tablama
        Dim drwTable As Manifold.Interop.Drawing
        Try
            drwTable = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Proverite podesavanje ulaznih parametara")
            Exit Sub
        End Try

        'sada zaokruzi koordinate tacaka tabli na 2 decimale i onda nema problem posle!
        zaokruziGeomPovrsine2Dec(My.Settings.layerName_table) : drwTable = doc.ComponentSet(My.Settings.layerName_table)
        'kreira drawing za detaljne tacke nadele 
        My.Settings.layerName_pointTableObelezavanje = "pntTableObelezavanje" : My.Settings.Save()

        Dim pntTableObelezavanje As Manifold.Interop.Drawing
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        Dim tbl_ As Manifold.Interop.Table

        'treba formirati themu sa tackama ne ovako!!!!!

        Try
            pntTableObelezavanje = doc.NewDrawing(My.Settings.layerName_pointTableObelezavanje, drwTable.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove(My.Settings.layerName_pointTableObelezavanje)
            pntTableObelezavanje = doc.NewDrawing(My.Settings.layerName_pointTableObelezavanje, drwTable.CoordinateSystem, True)
        End Try

        Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
        Dim objSet_ As Manifold.Interop.ObjectSet
        objSet_ = analizer_.Points(drwTable, drwTable, drwTable.ObjectSet)

        col_.Name = "idTable"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_ = pntTableObelezavanje.OwnedTable
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_.Name = "tipTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_) 'ovde mozes da imas tri vrste 1 - katastarske i gup, 2 table, 3 parcele!
        col_ = Nothing


        doc.Save() 'sacuvas i imas dva drawinga

        lbl_infoMain.Text = "Kreiranje novog drawinga i kopiranja svih tacka tabli u njega"
        My.Application.DoEvents()

        'kreira i kopira sve tacke tabli u pntobelezavanje
        Dim qvrKopirajOK As Manifold.Interop.Query = doc.NewQuery("updateObelezavanje")
        qvrKopirajOK.Text = "INSERT INTO [" & My.Settings.layerName_pointTableObelezavanje & "] ([Geom (I)],[idTable]) SELECT pnt_, case  when br_ = 1 THEN (SELECT [idTable] from [" & My.Settings.layerName_table & "] where [Geom (I)]=pnt_) ELSE (SELECT top 1 [idTable] FROM [" & My.Settings.layerName_table & "] where [Geom (I)]=pnt_  ORDER by [tipTable]) end as idtable_  FROM  (SELECT [Geom (I)] as pnt_,count(*) as br_  FROM [" & My.Settings.layerName_table & "]  where Ispoint([ID]) GROUP BY [Geom (I)]) as A  ORDER by idtable_,CentroidX(pnt_),CentroidY(pnt_) "
        qvrKopirajOK.RunEx(True)

        lbl_infoMain.Text = "Zavrseno kopiranje tacaka u novi Drawing" : My.Application.DoEvents()

        objSet_ = pntTableObelezavanje.ObjectSet
        analizer_ = Nothing

        doc.Save()


        'kreranje rednog broja tacke
        Dim qvrOperat_ As Manifold.Interop.Query = doc.NewQuery("temp")

        tbl_ = pntTableObelezavanje.OwnedTable : lbl_infoMain.Text = "Kreiranje rednog broja"

        My.Application.DoEvents() : pb1.Maximum = tbl_.RecordSet.Count

        For i = 0 To tbl_.RecordSet.Count - 1
            pb1.Value = i : qvrOperat_.Text = "update [" & pntTableObelezavanje.Name & "] set [idTacke]=" & Chr(34) & i + 1 & Chr(34) & ", [tipTacke]=2 where [ID]=" & tbl_.RecordSet.Item(i).ID : qvrOperat_.RunEx(True)
        Next

        tbl_ = Nothing : pb1.Value = 0
        qvrOperat_.Text = "delete from [" & My.Settings.layerName_table & "] where isPoint([ID])" : qvrOperat_.RunEx(True) : doc.Save()

        'ovde moras da ubacis i tacke gradevinskog rejona i tacke ko 
        Try

            Dim drwPP As Manifold.Interop.Drawing = doc.ComponentSet("Tacke_Granice_KO")
            tbl_ = pntTableObelezavanje.OwnedTable
            lbl_infoMain.Text = "Zamena tacaka tabli sa datim tackama KO i GR"
            qvrOperat_.Text = "UPDATE (SELECT [" & pntTableObelezavanje.Name & "].[idTacke] as out_,[" & pntTableObelezavanje.Name & "].[tipTacke] as tip_,[Tacke_Granice_KO].[idTacke] as in_ FROM [" & pntTableObelezavanje.Name & "],[Tacke_Granice_KO] WHERE round([" & pntTableObelezavanje.Name & "].[X (I)],2)=round([Tacke_Granice_KO].[X (I)],2) and round([" & pntTableObelezavanje.Name & "].[Y (I)],2)=round([Tacke_Granice_KO].[Y (I)],2)) set out_=in_, tip_=1"
            qvrOperat_.RunEx(True)

        Catch ex As Exception
            'ako ne postoji javi gresku
            MsgBox("Nemate Drawing sa spiskom tacaka granice KO i Gradevinkog rejona")

        End Try

        doc.ComponentSet.Remove("temp") : qvrOperat_ = Nothing

        'sada ovo ispisivanje mora drugacije!!!!!!!!!! 
        'znaci za svaki tablu moras da trazis overlap sa tackama a onda da ih exportujes e sad bi bilo dobro da ih 
        'exportujes u nekom redu

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("tableInfo")
        qvr_.Text = "select [idTable],[ID] from [" & My.Settings.layerName_table & "] order by [idTable]" : qvr_.RunEx(True)
        pb1.Value = 0 : pb1.Maximum = qvr_.Table.RecordSet.Count

        lbl_infoMain.Text = "Export koordinata tabli u file" : My.Application.DoEvents()

        lbl_infoMain.Text = "Ispisivanje listinga tacaka po tablama u file" : My.Application.DoEvents()
        PrintLine(freeFile_, "br table, br plana,br tacaka u tabli,povrsina, brtacke,y,x")
        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            'sada ti treba iz atributa broj table i broj lista
            'sada bi iz baze trebalo da da oznaku i da broj lista
            conn_.Open() : comm_.CommandText = "select oznakaTable,BrojPlana, psuma from kom_kfmns where idTable=" & qvr_.Table.RecordSet.Item(i).DataText(1)
            Dim myReader As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection) : myReader.Read()
            If myReader.HasRows Then
                'PrintLine(freeFile_, "")
                PrintLine(freeFile_, myReader.GetValue(0) & "," & myReader.GetValue(1) & "," & drwTable.ObjectSet.Item(drwTable.ObjectSet.ItemByID(qvr_.Table.RecordSet.Item(i).DataText(2))).Geom.BranchSet.Item(0).PointSet.Count & "," & myReader.GetValue(2) & ",,")
            Else
                PrintLine(freeFile_, "nema podataka u bazi. tabla " & qvr_.Table.RecordSet.Item(i).DataText(1))
            End If

            myReader.Close() : myReader = Nothing : conn_.Close()

            'kriras sam pntTable - kao tacke iz svih tabli!           
            'sada idu tacke odnosno print u file!
            Dim qvrTacke As Manifold.Interop.Query = doc.NewQuery("pronadiTacke")
            qvrTacke.Text = "select B.* FROM (SELECT round(CentroidX(pnt_),2) as x1, round(CentroidY(pnt_),2) as y1 FROM [" & My.Settings.layerName_table & "] WHERE [idTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & " SPLIT by Coords([Geom (I)]) as pnt_) as A LEFT OUTER JOIN (SELECT [idTacke],round(CentroidX([Geom (I)]),2) as x2,round(CentroidY([Geom (I)]),2) as y2 FROM [" & pntTableObelezavanje.Name & "]) as B on  A.x1=B.x2 or A.y1=B.y2"
            qvrTacke.RunEx(True)

            For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                PrintLine(freeFile_, ",,,," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3))
                PrintLine(freefile2_, j & "," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvr_.Table.RecordSet.Item(i).DataText(1))
            Next
            'printas poslednju liniju
            PrintLine(freefile2_, qvrTacke.Table.RecordSet.Count & "," & qvrTacke.Table.RecordSet.Item(0).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(0).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(0).DataText(3) & "," & qvr_.Table.RecordSet.Item(i).DataText(1))

            doc.ComponentSet.Remove("PronadiTacke")

            qvrTacke = Nothing
        Next
        pb1.Value = 0
        'End If
        'sada mi treba za svaku tablu da imam presek sa procembenim razredima i da da izlaz - spisak koordinata tabla - procembeni razred!!!!
        lbl_infoMain.Text = "Export koordinata procembenih razreda po tablama u file"
        My.Application.DoEvents()


        'sada radis presek tabli i procembenih razreda da bi mogao to da uradis!
        Try
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception

        End Try

        'treba ti layer u kome su table
        drwTable = doc.ComponentSet(My.Settings.layerName_table)
        Dim drwProcRazredi As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology : topPRazredi.Bind(drwProcRazredi) : topPRazredi.Build()
        Dim topTable As Manifold.Interop.Topology = doc.Application.NewTopology : topTable.Bind(drwTable) : topTable.Build()
        topTable.DoIntersect(topPRazredi, "table_pr_razred")

        'sada idemo na upisivanje
        doc.Save()
        pb1.Value = 0

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            Dim qvrTacke As Manifold.Interop.Query = doc.NewQuery("pronadiTacke")

            PrintLine(freeFile_, "Broj table: " & qvr_.Table.RecordSet.Item(i).DataText(1) & "  spisak povrsina po procembenim razredima")
            'sada idemo prvo povrsine
            qvrTacke.Text = "SELECT [ID], round([Area (I)]),[procembeni] FROM [Table_pr_razred] WHERE [idtable]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
            qvrTacke.RunEx(True)

            For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                PrintLine(freeFile_, qvrTacke.Table.RecordSet.Item(j).DataText(1), qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2))
            Next

            'pa koordinate!
            PrintLine(freeFile_, "Broj table: " & qvr_.Table.RecordSet.Item(i).DataText(1) & "  spisak koordinata tacaka po procembenim razredima")

            qvrTacke.Text = "SELECT round(CentroidX(pnt_),2) as x1, round(CentroidY(pnt_),2) as y1,[procembeni],[ID] FROM [Table_pr_razred] WHERE [idTable]=" &
            qvr_.Table.RecordSet.Item(i).DataText(1) & " SPLIT by Coords([Geom (I)]) as pnt_"
            qvrTacke.RunEx(True)

            For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                PrintLine(freeFile_, qvrTacke.Table.RecordSet.Item(j).DataText(4), qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2))
            Next
            'printas poslednju liniju

            doc.ComponentSet.Remove("PronadiTacke")
            pb1.Value = i
        Next

        'End If
        pb1.Value = 0
        lbl_infoMain.Text = ""

        conn_ = Nothing
        comm_ = Nothing
        doc.ComponentSet.Remove("tableInfo")
        qvr_ = Nothing
        qvrKopirajOK = Nothing
        qvrOperat_ = Nothing
        FileClose()
        doc.Save()
        MsgBox("Kraj")

    End Sub

    Private Sub mnu_komasacija_novoStanje_RacunanjePovrsinaTabliIObjekata_TrenutnoStanje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_komasacija_novoStanje_RacunanjePovrsinaTabliIObjekata_TrenutnoStanje.Click
        'trebaju ti dve stvari : layeru kome se nalaze definisane table 
        'iz baze ti treba broj lista-ovo bi trebalo da se doda
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If doc.FullName = "" Then
            MsgBox("Greska : Ucitajte map file prvo")
            Exit Sub
        End If

        Try
            doc.Save()
        Catch ex As Exception
            MsgBox("File je otvoren u Manifold-u. Zatvoriter i probajte ponovo")
            doc = Nothing
            Exit Sub
        End Try

        'prveoris postojanje pntobelezavanje

        Dim pntTableObelezavanje As Manifold.Interop.Drawing
        Try
            pntTableObelezavanje = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Catch ex As Exception
            MsgBox("Nemate pntObelezavanje - proverite ulazni map file a startujte ponovo rutinu")
            doc = Nothing
            Exit Sub
        End Try

        'definisanje putanja do izlaznog file-a

        sf_diag.FileName = "Spisak_tacaka_tabli_i_procembenih_razreda.txt"
        sf_diag.Title = "Izlazni file za spisak Tacaka"
        sf_diag.DefaultExt = "txt"
        sf_diag.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then
            Exit Sub
        End If

        Dim freeFile_ As Integer = FreeFile()
        Try
            FileOpen(freeFile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Catch ex As Exception
            FileClose()
            FileOpen(freeFile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        End Try


        sf_diag.FileName = "tableSpisakKontrola.txt"
        sf_diag.Title = "Kontrolni file"
        sf_diag.DefaultExt = "Text files (*.txt)|*.txt"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then
            Exit Sub
        End If

        Dim freefile2_ As Integer = FreeFile()
        Try
            FileOpen(freefile2_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Catch ex As Exception
            FileClose()
            FileOpen(freefile2_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        End Try


        'sada ti trebaju dva drawinga 
        Dim drwTable As Manifold.Interop.Drawing
        Try
            drwTable = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Proverite podesavanje ulaznih parametara")
            Exit Sub
        End Try

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        Me.Cursor = Cursors.WaitCursor


        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("tableInfo")
        qvr_.Text = "select [idTable],[ID] from [" & My.Settings.layerName_table & "] order by [idTable]" : qvr_.RunEx(True)
        pb1.Value = 0 : pb1.Maximum = qvr_.Table.RecordSet.Count

        lbl_infoMain.Text = "Ispisivanje listinga tacaka po tablama u file"
        My.Application.DoEvents()

        Dim x(-1), y(-1) As Double
        Dim brMesta As Integer = My.Settings.zaokruzivanjeBrojDecMesta

        'stampas naslov 
        PrintLine(freeFile_, "brtable, brplana,brtacaka,povrsinaBaza,povrsinaPnt,brtacke,y,x")

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i

            Try
                lbl_infoMain.Text = "Ispisivanje listinga tacaka po tablama u file. Obrada table id= " & qvr_.Table.RecordSet.Item(i).DataText(1)
                My.Application.DoEvents()

                'selektujes tacke iz pntObelezavanje na osnovu geometrije table
                Dim qvrTacke As Manifold.Interop.Query = doc.NewQuery("pronadiTacke")
                qvrTacke.Text = "select * FROM (select B.* FROM (SELECT CentroidX(pnt_) as x1, CentroidY(pnt_) as y1 FROM [" & My.Settings.layerName_table & "] WHERE [idTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & " SPLIT by Coords([Geom (I)]) as pnt_) as A LEFT OUTER JOIN (SELECT [idTacke],CentroidX([Geom (I)]) as x2,CentroidY([Geom (I)]) as y2,[tipTacke] FROM [" & pntTableObelezavanje.Name & "]) as B on  round(A.x1,2)=round(B.x2,2) and round(A.y1,2)=round(B.y2,2) and (B.[tiptacke]=1 or B.[tipTacke]=2) ) as C where  [idTacke] IS NOT NULL"
                qvrTacke.RunEx(True)

                'smestas njihove koordinate u posebne matrice radi racunanja povrsine iz koordinata
                ReDim x(qvrTacke.Table.RecordSet.Count), y(qvrTacke.Table.RecordSet.Count) 'da imas mesta i za poslednju tacku!

                For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                    x(j) = qvrTacke.Table.RecordSet.Item(j).DataText(2) : y(j) = qvrTacke.Table.RecordSet.Item(j).DataText(3)
                Next
                x(qvrTacke.Table.RecordSet.Count) = qvrTacke.Table.RecordSet.Item(0).DataText(2) : y(qvrTacke.Table.RecordSet.Count) = qvrTacke.Table.RecordSet.Item(0).DataText(3)

                'sada mozes povrsinu!
                Dim p_ As Double = 0 : Dim p2_ As Double = 0 : Dim dp12_ As Double = 0

                For j = 0 To x.Length - 2
                    p_ += ((x(j + 1) - x(j)) * (y(j + 1) + y(j))) / 2
                    p2_ += ((Math.Round(x(j + 1), brMesta) - Math.Round(x(j), brMesta)) * (Math.Round(y(j + 1), brMesta) + Math.Round(y(j), brMesta))) / 2
                Next

                'sada vadis podatke iz baze komasacije i stampas prvi red pre listinga
                conn_.Open()
                comm_.CommandText = "select oznakaTable, ifnull(BrojPlana,''), psuma from kom_kfmns where idTable=" & qvr_.Table.RecordSet.Item(i).DataText(1)
                Dim myReader As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
                Dim brojtable_ As Integer = qvr_.Table.RecordSet.Item(i).DataText(1)
                myReader.Read()
                If myReader.HasRows Then
                    PrintLine(freeFile_, myReader.GetValue(0) & "," & myReader.GetValue(1) & "," & drwTable.ObjectSet.Item(drwTable.ObjectSet.ItemByID(qvr_.Table.RecordSet.Item(i).DataText(2))).Geom.BranchSet.Item(0).PointSet.Count & "," & myReader.GetValue(2) & "," & Math.Abs(p_) & ",,")
                Else
                    PrintLine(freeFile_, "nema podataka u bazi. tabla " & qvr_.Table.RecordSet.Item(i).DataText(1))
                End If

                myReader.Close() : myReader = Nothing : conn_.Close()

                For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                    PrintLine(freeFile_, ",,,,," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3))
                    PrintLine(freefile2_, j & "," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvr_.Table.RecordSet.Item(i).DataText(1))
                Next

                doc.ComponentSet.Remove("PronadiTacke") : qvrTacke = Nothing

            Catch ex As Exception
                'sta ako nema onda puca jos na upitu
                If MsgBox(ex.Message, MsgBoxStyle.OkCancel, "Pitanje: izlazim?") = MsgBoxResult.Ok Then
                    Exit Sub
                End If
            End Try

        Next

        If MsgBox("Da li ispisujem i procembene razrede?", MsgBoxStyle.OkCancel, "Pitanje") = MsgBoxResult.Ok Then

            lbl_infoMain.Text = "Export koordinata procembenih razreda po tablama u file"
            My.Application.DoEvents()

            Try
                doc.ComponentSet.Remove("table_pr_razred")
            Catch ex As Exception

            End Try

            'treba ti layer u kome su table
            drwTable = doc.ComponentSet(My.Settings.layerName_table)
            Dim drwProcRazredi As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
            Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology : topPRazredi.Bind(drwProcRazredi) : topPRazredi.Build()
            Dim topTable As Manifold.Interop.Topology = doc.Application.NewTopology : topTable.Bind(drwTable) : topTable.Build()
            topTable.DoIntersect(topPRazredi, "table_pr_razred")

            'sada idemo na upisivanje
            doc.Save()
            pb1.Value = 0
            For i = 0 To qvr_.Table.RecordSet.Count - 1

                Dim qvrTacke As Manifold.Interop.Query = doc.NewQuery("pronadiTacke")

                PrintLine(freeFile_, "Broj table: " & qvr_.Table.RecordSet.Item(i).DataText(1) & "  spisak povrsina po procembenim razredima")
                'sada idemo prvo povrsine
                qvrTacke.Text = "SELECT [ID], round([Area (I)]),[procembeni] FROM [Table_pr_razred] WHERE [idtable]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
                qvrTacke.RunEx(True)

                For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                    PrintLine(freeFile_, qvrTacke.Table.RecordSet.Item(j).DataText(1), qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2))
                Next

                'pa koordinate!
                PrintLine(freeFile_, "Broj table: " & qvr_.Table.RecordSet.Item(i).DataText(1) & "  spisak koordinata tacaka po procembenim razredima")

                qvrTacke.Text = "SELECT round(CentroidX(pnt_),2) as x1, round(CentroidY(pnt_),2) as y1,[procembeni],[ID] FROM [Table_pr_razred] WHERE [idTable]=" &
                qvr_.Table.RecordSet.Item(i).DataText(1) & " SPLIT by Coords([Geom (I)]) as pnt_"
                qvrTacke.RunEx(True)

                For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                    PrintLine(freeFile_, qvrTacke.Table.RecordSet.Item(j).DataText(4), qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2))
                Next
                'printas poslednju liniju

                doc.ComponentSet.Remove("PronadiTacke")
                pb1.Value = i
            Next

        End If

        pb1.Value = 0
        conn_ = Nothing
        comm_ = Nothing
        doc.ComponentSet.Remove("tableInfo")
        qvr_ = Nothing
        'qvrKopirajOK = Nothing
        'qvrOperat_ = Nothing
        FileClose()
        doc.Save()
        Me.Cursor = Cursors.Default
        MsgBox("Kraj")
    End Sub

    Private Sub mnu_legendaPrikazi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_legendaPrikazi.Click
        Me.Cursor = Cursors.WaitCursor
        Try
            Dim pera_ = layersTV.SelectedNode.FullPath
            Dim a_ = pera_.Split("\")
            'If a_(0) = "Maps" And a_.Length >= 2 Then
            'ne ucitas samo mapu broj 2
            Try
                If a_(0) <> "Query" Then
                    ManifoldCtrl.set_Component(a_(1))
                Else
                    Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
                    Dim q_ As Manifold.Interop.Query = doc_.ComponentSet(a_(1))
                    txt_Query.DocumentText = q_.Text
                    q_ = Nothing
                    doc_ = Nothing
                End If

            Catch ex As Exception
                MsgBox("Nije moguce ucitati tabelu ili query")
            End Try
        Catch ex As Exception

        End Try
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub mnu_legendaProzirnost_Click(sender As System.Object, e As System.EventArgs) Handles mnu_legendaProzirnost.Click
        'treba to koji je layer!
        Dim pera_ = layersTV.SelectedNode.FullPath
        Dim a_ = pera_.Split("\")

        'sada u zavisnosti da li je celamapa ili lazer
        Select Case a_.Length

            Case 4
                'novi layer(a)- prozirnost je na nivou layer!!!
                Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
                Dim map_ As Manifold.Interop.Map = doc_.ComponentSet(a_(1))
                map_.LayerSet(map_.LayerSet.ItemByName(a_(3))).Opacity = InputBox("Unesite novu prozirnost za themu " & a_(3), "Unos podataka", map_.LayerSet(map_.LayerSet.ItemByName(a_(3))).Opacity)
                ManifoldCtrl.Refresh()
        End Select

    End Sub

    Private Sub layersTV_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles layersTV.AfterSelect
        If e.Node.Level = 3 Then
            'selectujes level 2
            e.Node.Parent.Parent.Checked = True
        ElseIf e.Node.Level = 2 Then
            e.Node.Parent.Checked = True
        End If
    End Sub

    Private Sub layersTV_AfterCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles layersTV.AfterCheck
        'sada ga treba selektovati!

        If e.Action = TreeViewAction.ByMouse Then
            layersTV.SelectedNode = e.Node
            Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
            Try
                Dim pera_ = layersTV.SelectedNode.FullPath
                Dim a_ = pera_.Split("\")
                Dim map_ As Manifold.Interop.Map = doc_.ComponentSet(a_(1))
                Dim layerset_ As Manifold.Interop.LayerSet = map_.LayerSet
                If e.Node.Level = 2 Then
                    'sada ides na vidljivost odnosno nevidljivost!
                    'prolazis kroz komponente i podesis da je vidljivos
                    Dim node_ As TreeNode = layersTV.SelectedNode

                    For i = 0 To node_.Nodes.Count - 1
                        'sada imas layer-e!
                        If node_.Checked = True Then
                            layerset_.Item(layerset_.ItemByID(node_.Nodes.Item(i).Name)).Visible = True
                            node_.Nodes.Item(i).Checked = True
                        Else
                            layerset_.Item(layerset_.ItemByID(node_.Nodes.Item(i).Name)).Visible = False
                            node_.Nodes.Item(i).Checked = False
                        End If
                    Next
                ElseIf e.Node.Level = 3 Then
                    If e.Node.Checked = True Then
                        layerset_.Item(layerset_.ItemByID(e.Node.Name)).Visible = True
                    Else
                        layerset_.Item(layerset_.ItemByID(e.Node.Name)).Visible = False
                    End If
                End If
                map_ = Nothing
                layerset_ = Nothing
            Catch ex As Exception

            End Try
            ManifoldCtrl.Refresh()
        End If

    End Sub

    Private Sub mnu_funkcije_KreiranjeLabelaLinijskiElement_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_funkcije_KreiranjeLabelaLinijskiElement.Click
        'u istom drawingu kreira label 
        'ulaz je naziv Drawinga
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        doc = ManifoldCtrl.get_Document

        'Frontovi

        Dim drwParceleLinije As Manifold.Interop.Drawing
        Dim draw_ = InputBox("Upisite naziv Drawinga za koji radite label", "Unos podataka", " ")
        If draw_ <> " " Then

            Try
                drwParceleLinije = doc.ComponentSet(draw_)
            Catch ex As Exception
                MsgBox("Drawing ne postoji")
                Exit Sub
            End Try

        Else

            Exit Sub

        End If

        Dim dodajem_ As Integer = InputBox("Da li dodajem za geodetsku liniju? 0 - duzina kao sto je linija, u suprotnom upisite koliko se dodaje u zavisnoti od razmere.", "Unos podataka", "0")

        Dim namestam_ As Integer = InputBox("Da li namestam duzinu (0)? (ili stampam njenu pravu vrednost (1)", "Pitanje")

        Dim tbl_ As Manifold.Interop.Table
        tbl_ = drwParceleLinije.OwnedTable
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)


        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("analiza")

        If namestam_ = 0 Then

            If dodajem_ <> 0 Then
                qvr_.Text = "update [" & draw_ & "] set [Duzina]=round([Length (I)]+rnd*0.07,2)+" & dodajem_ & ", [Ugao]=round([Bearing (I)])"
            Else
                qvr_.Text = "update [" & draw_ & "] set [Duzina]=round([Length (I)]+rnd*0.07,2), [Ugao]=round([Bearing (I)])"
            End If

        Else

            If dodajem_ <> 0 Then
                qvr_.Text = "update [" & draw_ & "] set [Duzina]=round([Length (I)],2)+" & dodajem_ & ", [Ugao]=round([Bearing (I)])"
            Else
                qvr_.Text = "update [" & draw_ & "] set [Duzina]=round([Length (I)],2), [Ugao]=round([Bearing (I)])"
            End If

        End If




        qvr_.RunEx(True)

        doc.Save()

        'sada formiras label

        'kreiras tacke i sa ova dva polja treba da napravis label - odnosno duzina ti je label a ugao ti je rotacija! - samo se postavlja pitanje kako pojedinacnu da napravis
        qvr_.Text = "select centroidx(LinePoint([Geom (I)], [Length (I)]/2)),centroidy(LinePoint([Geom (I)], [Length (I)]/2)),[Ugao],[Duzina] from [" & draw_ & "]"
        qvr_.RunEx(True)

        Dim drwLabelFront As Manifold.Interop.Labels = doc.NewLabels("label_front", drwParceleLinije.CoordinateSystem, True)
        drwLabelFront.LabelAlignX = LabelAlignX.LabelAlignXLeft : drwLabelFront.LabelAlignY = LabelAlignY.LabelAlignYTop
        drwLabelFront.OptimizeLabelAlignX = False : drwLabelFront.OptimizeLabelAlignY = False
        drwLabelFront.ResolveOverlaps = False : drwLabelFront.PerLabelFormat = True

        Dim lblSets_ As Manifold.Interop.LabelSet = drwLabelFront.LabelSet
        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        lbl_infoMain.Text = "Kreiranje Label-a za frontove"
        My.Application.DoEvents()

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            'e sada da vidimo kreiras tacku i onda ide dalje
            Dim pnt_ As Manifold.Interop.Point = doc.Application.NewPoint(qvr_.Table.RecordSet.Item(i).DataText(1), qvr_.Table.RecordSet.Item(i).DataText(2))
            lblSets_.Add(qvr_.Table.RecordSet.Item(i).DataText(4), pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets_.LastAdded
            labb_.Rotation = qvr_.Table.RecordSet.Item(i).DataText(3) - 90
            labb_.Size = 3.75
            pnt_ = Nothing
        Next
        doc.Save()
        pb1.Value = 0

        tbl_ = Nothing
        col_ = Nothing
        qvr_ = Nothing
        doc = Nothing

        MsgBox("Kraj")
    End Sub

    Private Sub ManifoldCtrl_EndTrack(ByVal sender As Object, ByVal e As AxManifold.Interop.IComponentControlEvents_EndTrackEvent) Handles ManifoldCtrl.EndTrack
        Me.Cursor = Cursors.WaitCursor
        Dim trackArgs As Manifold.Interop.ControlTrackEventArgs
        trackArgs = e.pArgs
        Dim obj_ As Manifold.Interop.Geom = trackArgs.GeomScreen
        Dim pointScreen As Manifold.Interop.Point = trackArgs.GeomScreen.BranchSet(0).PointSet(0)
        Dim myObject = ManifoldCtrl.GetObjectAt(pointScreen)
        ManifoldCtrl.RenderSelection = True
        mnuGridMenu.Items.Clear()

        Dim freefile_ As Integer = FreeFile()

        Select Case ManifoldCtrl.MouseMode
            Case ControlMouseMode.ControlMouseModeGenericPoint

                tvSelektion.Nodes.Clear()

                Dim plg_ = trackArgs.GeomNative.ToTextWKT
                P_ = trackArgs.GeomNative.Area
                y_ = trackArgs.GeomNative.Center.Y
                x_ = trackArgs.GeomNative.Center.X
                'sada ide za svaki!
                Dim qvr_ As Manifold.Interop.Query = ManifoldCtrl.get_Document.NewQuery("temp_")
                Dim geom_ = "POLYGON((" & x_ - 5 & " " & y_ + 5 & "," & x_ + 5 & " " & y_ + 5 & "," & x_ + 5 & " " & y_ - 5 & "," & x_ - 5 & " " & y_ - 5 & "," & x_ - 5 & " " & y_ + 5 & "))"
                'posto je otvorena map-a ide u okviru nje i ide za svaki dwg layer posebno@
                'aj sad sa mapom!
                Dim gde_ As Integer = -1
                For i = 0 To ManifoldCtrl.get_Document.ComponentSet.Count - 1
                    If ManifoldCtrl.get_Document.ComponentSet.Item(i).TypeName = "Map" Then
                        gde_ = i
                        Exit For
                    End If
                Next

                If gde_ <> -1 Then
                    Dim map_ As Manifold.Interop.Map = ManifoldCtrl.get_Document.ComponentSet(gde_)
                    Dim lset_ As Manifold.Interop.LayerSet = map_.LayerSet

                    For i = 0 To lset_.Count - 1
                        If lset_.Item(i).Component.TypeName = "Drawing" And lset_.Item(i).Visible = True Then

                            Dim nodeMap_ As New TreeNode
                            nodeMap_.Text = lset_.Item(i).Component.Name
                            tvSelektion.Nodes.Add(nodeMap_)

                            'sada mozes ovde da sprovedes ispitivanje
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = False", lset_.Item(i).Component.Name)
                            qvr_.RunEx(True)
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = True WHERE Touches(ConvertToArea(AssignCoordSys(CGeom(CGeomWKB(""{1}"")), CoordSys(""{0}"" AS COMPONENT))), [ID])", lset_.Item(i).Component.Name, geom_)
                            qvr_.RunEx(True)
                            'sada ovo mozes da printas!
                            'showSelectionInWeb(lset_.Item(i).Component.Name, freefile_)
                            'showSelectionInTreeView(lset_.Item(i).Component.Name, nodeMap_)
                        End If
                    Next

                Else
                    'nema map
                End If

                ManifoldCtrl.get_Document.ComponentSet.Remove("temp_")

            Case ControlMouseMode.ControlMouseModeGenericBox
                FileOpen(freefile_, Path.GetTempPath() & "\izlaz.html", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
                PrintLine(freefile_, "<html><body>")
                Dim plg_ = trackArgs.GeomNative.ToTextWKT
                P_ = trackArgs.GeomNative.Area
                x_ = trackArgs.GeomNative.Center.Y
                y_ = trackArgs.GeomNative.Center.X
                'sada ide za svaki!
                Dim qvr_ As Manifold.Interop.Query = ManifoldCtrl.get_Document.NewQuery("temp_")

                'posto je otvorena map-a ide u okviru nje i ide za svaki dwg layer posebno@
                'aj sad sa mapom!
                Dim gde_ As Integer = -1
                For i = 0 To ManifoldCtrl.get_Document.ComponentSet.Count - 1
                    If ManifoldCtrl.get_Document.ComponentSet.Item(i).TypeName = "Map" Then
                        gde_ = i
                        Exit For
                    End If
                Next

                If gde_ <> -1 Then
                    Dim map_ As Manifold.Interop.Map = ManifoldCtrl.get_Document.ComponentSet(gde_)
                    Dim lset_ As Manifold.Interop.LayerSet = map_.LayerSet

                    For i = 0 To lset_.Count - 1
                        If lset_.Item(i).Component.TypeName = "Drawing" And lset_.Item(i).Visible = True Then
                            'sada mozes ovde da sprovedes ispitivanje
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = False", lset_.Item(i).Component.Name)
                            qvr_.RunEx(True)
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = True WHERE Contains(ConvertToArea(AssignCoordSys(CGeom(CGeomWKB(""{1}"")), CoordSys(""{0}"" AS COMPONENT))), [ID])", lset_.Item(i).Component.Name, trackArgs.GeomNative.ToTextWKT)
                            qvr_.RunEx(True)
                            'sada ovo mozes da printas!
                            ' showSelectionInWeb(lset_.Item(i).Component.Name, freefile_)
                        End If
                    Next

                Else
                    'nema map
                End If

                ManifoldCtrl.get_Document.ComponentSet.Remove("temp_")

                PrintLine(freefile_, "</body></html>")
                Process.Start(Path.GetTempPath() & "\izlaz.html")
                FileClose()

            Case ControlMouseMode.ControlMouseModeGenericCircle
                FileOpen(freefile_, Path.GetTempPath() & "\izlaz.html", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
                PrintLine(freefile_, "<html><body>")

                Dim qvr_ As Manifold.Interop.Query = ManifoldCtrl.get_Document.NewQuery("temp_")
                P_ = trackArgs.GeomNative.Area
                x_ = trackArgs.GeomNative.Center.Y : y_ = trackArgs.GeomNative.Center.X
                lbl_infoMain.Text = "Poluprecnik kruga poslednje selekcije : " & Math.Round(trackArgs.GeomNative.Box.Width)

                'posto je otvorena map-a ide u okviru nje i ide za svaki dwg layer posebno@
                'aj sad sa mapom!
                Dim gde_ As Integer = -1
                For i = 0 To ManifoldCtrl.get_Document.ComponentSet.Count - 1
                    If ManifoldCtrl.get_Document.ComponentSet.Item(i).TypeName = "Map" Then
                        gde_ = i
                        Exit For
                    End If
                Next

                If gde_ <> -1 Then
                    Dim map_ As Manifold.Interop.Map = ManifoldCtrl.get_Document.ComponentSet(gde_)
                    Dim lset_ As Manifold.Interop.LayerSet = map_.LayerSet

                    For i = 0 To lset_.Count - 1
                        If lset_.Item(i).Component.TypeName = "Drawing" And lset_.Item(i).Visible = True Then
                            'sada mozes ovde da sprovedes ispitivanje
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = False", lset_.Item(i).Component.Name)
                            qvr_.RunEx(True)
                            qvr_.Text = String.Format("UPDATE [{0}] SET [Selection (I)] = True WHERE Contains(ConvertToArea(AssignCoordSys(CGeom(CGeomWKB(""{1}"")), CoordSys(""{0}"" AS COMPONENT))), [ID])", lset_.Item(i).Component.Name, trackArgs.GeomNative.ToTextWKT)
                            qvr_.RunEx(True)
                            'sada ovo mozes da printas!
                            ' showSelectionInWeb(lset_.Item(i).Component.Name, freefile_)

                        End If
                    Next

                Else
                    'nema map
                End If

                ManifoldCtrl.get_Document.ComponentSet.Remove("temp_")

                PrintLine(freefile_, "</body></html>")
                Process.Start(Path.GetTempPath() & "\izlaz.html")
                FileClose()

        End Select

        ManifoldCtrl.Refresh()
        'analizaNesaB()
        'sada mozes analizu koja ti treba za Nesu Bosanca
        Me.Cursor = Cursors.Default

        pb1.Value = 0
    End Sub
    Private Sub showSelectionInGrid(ByVal componentName_ As String)
        Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
        'aj sad da nademo tabelu!
        Dim table_ As New DataTable

        Dim tbl_ As Manifold.Interop.Table = doc_.ComponentSet(componentName_).OwnedTable
        For i = 0 To tbl_.ColumnSet.Count - 1
            table_.Columns.Add(tbl_.ColumnSet(i).Name)
        Next

        For i = 0 To tbl_.Selection.Count - 1

            Dim newR_ As DataRow = table_.NewRow
            For j = 0 To tbl_.ColumnSet.Count - 1
                Try
                    newR_(j) = tbl_.Selection.Item(i).DataText(j + 1)
                Catch ex As Exception

                End Try
            Next
            table_.Rows.Add(newR_)
            newR_ = Nothing
        Next

        MsgBox("selektovanih " & tbl_.Selection.Count)

        'sada aj da dodamo ovo kao 
        dgv_ManifoldData.DataSource = table_
        table_ = Nothing
        doc_ = Nothing
    End Sub

    Private Sub mnu_Help_OProgramu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_Help_OProgramu.Click
        Dim frm_ As New oNama
        frm_.Show()
    End Sub
    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'aj sad da dodamo ono sto nam treba!

        Dim mnuGde As New ToolStripMenuItem
        mnuGde.Text = "Ranije otvoreni file-ovi"
        'sada mozes da mu dodas!

        Dim p_ = My.Settings.openFiles.Split(";")
        For i = 0 To p_.Length - 1
            'dodajes menu!
            Dim mnu_ As New ToolStripMenuItem
            mnu_.Name = "mnu_file_" & i
            mnu_.Text = p_(i)
            mnuGde.DropDownItems.Add(mnu_)
            AddHandler mnu_.Click, AddressOf MenuItemClicked
        Next

        mnu_file.DropDownItems.Add(mnuGde)

        ManifoldCtrl.Visible = False

    End Sub

    Private Sub MenuItemClicked(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnu_ = DirectCast(sender, ToolStripMenuItem)
        Try
            Me.Cursor = Cursors.WaitCursor
            ManifoldCtrl.DocumentPath = mnu_.Text
            napuniLayersByMap()
            Me.Text = " Komasacija - dokument: " & ManifoldCtrl.DocumentPath
            Me.Cursor = Cursors.Default
            ManifoldCtrl.Visible = True
            '            axSharpMap.Visible = False
        Catch ex As Exception
            'iz nekog razloga ne moze da otvori
        End Try
    End Sub


    Private Sub mnuGridMenu_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnuGridMenu.ItemClicked
        'sada mozes!

        Try
            Me.Cursor = Cursors.WaitCursor
            showSelectionInGrid(e.ClickedItem.Text)
            Me.Cursor = Cursors.Default
        Catch ex As Exception
            'iz nekog razloga ne moze da otvori
        End Try
    End Sub

    Private Sub btn_KSistemIsti_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_KSistemIsti.Click
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        opf_diag.FileName = ""
        opf_diag.Filter = "XML files (*.xml)|*.xml|PRJ files (*.prj)|*.prj|All files (*.*)|*.*"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub

        Dim oCS As Manifold.Interop.CoordinateSystem

        If InStr(UCase(opf_diag.FileName), ".XML") <> 0 Then
            oCS = doc.Application.NewCoordinateSystemFromFile(opf_diag.FileName)
        End If

        If InStr(UCase(opf_diag.FileName), ".PRJ") <> 0 Then
            oCS = doc.Application.NewCoordinateSystemFromTextPRJ(opf_diag.FileName)
        End If

        Dim oComps As Manifold.Interop.ComponentSet = doc.ComponentSet
        Dim cDrw As Manifold.Interop.Drawing : Dim cLbl As Manifold.Interop.Labels : Dim cImg As Manifold.Interop.Image : Dim cMap As Manifold.Interop.Map
        Dim csParams As Manifold.Interop.CoordinateSystemParameterSet : Dim localScaleX As Double : Dim localScaleY As Double : Dim localOffsetX As Double : Dim localOffsetY As Double

        pb1.Value = 0
        pb1.Maximum = oComps.Count

        Try

            For iLoop As Integer = 0 To oComps.Count - 1
                Select Case True                                                         ' Themes handled implicitly
                    Case (TypeOf (oComps(iLoop)) Is Manifold.Interop.Drawing)
                        cDrw = oComps(iLoop)
                        'If Not cDrw.CoordinateSystemVerified Then
                        cDrw.CoordinateSystem = oCS
                        'End If
                    Case (TypeOf (oComps(iLoop)) Is Manifold.Interop.Labels)
                        cLbl = oComps(iLoop)
                        'If Not cLbl.CoordinateSystemVerified Then
                        cLbl.CoordinateSystem = oCS
                        'End If
                    Case (TypeOf (oComps(iLoop)) Is Manifold.Interop.Image)
                        ' If image was imported from TIFF+TFW then preserve scale and offset from the import! 
                        cImg = oComps(iLoop)
                        csParams = cImg.CoordinateSystem.ParameterSet
                        localScaleX = csParams.Item("localScaleX").Value
                        localScaleY = csParams.Item("localScaleY").Value
                        localOffsetX = csParams.Item("localOffsetX").Value
                        localOffsetY = csParams.Item("localOffsetY").Value
                        'If Not cImg.CoordinateSystemVerified Then
                        cImg.CoordinateSystem = oCS
                        With cImg.CoordinateSystem.ParameterSet
                            .Item("localScaleX").Value = localScaleX
                            .Item("localScaleY").Value = localScaleY
                            .Item("localOffsetX").Value = localOffsetX
                            .Item("localOffsetY").Value = localOffsetY
                        End With
                        'End If
                    Case TypeOf (oComps(iLoop)) Is Manifold.Interop.Map ' Doesn't support CoordinateSystemVerified property
                        cMap = oComps(iLoop)
                        If (cMap.CoordinateSystem.Preset <> oCS.Preset) Then
                            cMap.CoordinateSystem = oCS
                        End If
                    Case Else
                        ' leave it alone
                End Select
                pb1.Value = iLoop
            Next
        Catch ex As Exception
            doc.Application.MessageBox(ex.ToString(), "Script error")
        End Try

        pb1.Value = 0

        doc.Save()

        oCS = Nothing

        oComps = Nothing : cDrw = Nothing : cLbl = Nothing : cImg = Nothing : cMap = Nothing : csParams = Nothing : localScaleX = Nothing : localScaleY = Nothing : localOffsetX = Nothing : localOffsetY = Nothing


        MsgBox("Kraj ")

    End Sub

    Private Sub mnu_selectionPoint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_selectionPoint.Click
        ManifoldCtrl.MouseMode = ControlMouseMode.ControlMouseModeGenericPoint
    End Sub

    Private Sub mnu_selectionCircle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_selectionCircle.Click
        ManifoldCtrl.MouseMode = ControlMouseMode.ControlMouseModeGenericCircle
    End Sub

    Private Sub mnu_selectionRec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_selectionRec.Click
        ManifoldCtrl.MouseMode = ControlMouseMode.ControlMouseModeGenericBox
    End Sub

    Private Sub mnu_selectionNothing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnu_selectionNothing.Click
        ManifoldCtrl.MouseMode = ControlMouseMode.ControlMouseModeNone
    End Sub


    Private Sub mnu_RacunanjePovrsinaIzKoordinataToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles mnu_RacunanjePovrsinaIzKoordinataToolStripMenuItem.Click
        opf_diag.FileName = ""
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub

        Dim free_ As Integer = FreeFile()
        Dim x(-1), y(-1) As Double
        FileOpen(free_, opf_diag.FileName, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)
        Dim brojac_ As Integer = 0
        Dim c_ As String = ""
        Do While Not EOF(free_)
            Dim a_ = LineInput(free_)
            If brojac_ = 0 Then
                'znaci da je u pitanju prvi pa ga treba sacuvati
                c_ = a_
            End If

            Dim b_ = a_.Split(",")
            ReDim Preserve x(brojac_) : ReDim Preserve y(brojac_)
            x(brojac_) = b_(0) : y(brojac_) = b_(1)
            brojac_ += 1
        Loop
        'sada ostaje da dodas ovaj kao poslednju koordinateu

        ReDim Preserve x(brojac_) : ReDim Preserve y(brojac_)
        Dim d_ = c_.Split(",")
        x(brojac_) = d_(0) : y(brojac_) = d_(1)


        Dim p_ As Double = 0 : Dim p2_ As Double = 0
        For j = 0 To x.Length - 2
            p_ += ((x(j + 1) - x(j)) * (y(j + 1) + y(j))) / 2
            p2_ += ((Math.Round(x(j + 1), 2) - Math.Round(x(j), 2)) * (Math.Round(y(j + 1), 2) + Math.Round(y(j), 2))) / 2
        Next

        MsgBox(Math.Abs(p_) & "," & Math.Abs(p2_))

    End Sub

    Private Sub zaokruziKFMSS()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "SELECT brParcele, (Povrsina - sum( round(prazred_1) + round(Prazred_2) + round(Prazred_3) + round(Prazred_4) + round(Prazred_5) + round(Prazred_6) + round(Prazred_7) + round(prazred_neplodno))) AS dP FROM kom_kfmss WHERE obrisan = 0 GROUP BY brparcele HAVING dP <> 0"


        Dim freefile_ As Integer = FreeFile() : Dim myadapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : Dim ds_ As New DataSet

        myadapter_.SelectCommand = comm_
        myadapter_.Fill(ds_) ' ovde imas broj parcele i broj koji ti treba za limit!

        Dim mysql_(-1) As String : Dim brojac_ As Integer = 0

        pb1.Value = 0 : pb1.Maximum = ds_.Tables(0).Rows.Count

        lbl_infoMain.Text = "Ukupan broj za zaokruziavanje je " & ds_.Tables(0).Rows.Count

        For i = 0 To ds_.Tables(0).Rows.Count - 1

            pb1.Value = i

            comm_.CommandText = "SELECT GG1.brParcele, GG1.A, GG1.B, GG2.dP, GG3.br_, GG3.postat_ FROM (( SELECT brParcele, A, B FROM (( SELECT brParcele, abs(Prazred_1 - round(Prazred_1)) AS A, 1 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_2 - round(Prazred_2)) AS A, 2 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_3 - round(Prazred_3)) AS A, 3 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_4 - round(Prazred_4)) AS A, 4 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_5 - round(Prazred_5)) AS A, 5 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_6 - round(Prazred_6)) AS A, 6 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_7 - round(Prazred_7)) AS A, 7 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs( prazred_neplodno - round(prazred_neplodno)) AS A, 8 AS B FROM kom_kfmss WHERE obrisan = 0 )) AS GG WHERE A <> 0 AND brParcele IN ( SELECT brParcele FROM ( SELECT brParcele, ( Povrsina - sum( round(prazred_1) + round(Prazred_2) + round(Prazred_3) + round(Prazred_4) + round(Prazred_5) + round(Prazred_6) + round(Prazred_7) + round(prazred_neplodno))) AS dP FROM kom_kfmss WHERE obrisan = 0 GROUP BY brparcele HAVING dP <> 0 ) AS CC ) ORDER BY brparcele, A DESC ) AS GG1 LEFT OUTER JOIN ( SELECT brParcele, dP FROM ( SELECT brParcele, ( Povrsina - sum( round(prazred_1) + round(Prazred_2) + round(Prazred_3) + round(Prazred_4) + round(Prazred_5) + round(Prazred_6) + round(Prazred_7) + round(prazred_neplodno))) AS dP FROM kom_kfmss WHERE obrisan = 0 GROUP BY brparcele HAVING dP <> 0 ) AS CC ) AS GG2 ON GG1.brParcele = GG2.brParcele LEFT OUTER JOIN ( SELECT brParcele, count(*) AS br_, sum(A) AS postat_ FROM (( SELECT brParcele, abs(Prazred_1 - round(Prazred_1)) AS A, 1 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_2 - round(Prazred_2)) AS A, 2 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_3 - round(Prazred_3)) AS A, 3 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_4 - round(Prazred_4)) AS A, 4 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_5 - round(Prazred_5)) AS A, 5 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_6 - round(Prazred_6)) AS A, 6 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs(Prazred_7 - round(Prazred_7)) AS A, 7 AS B FROM kom_kfmss WHERE obrisan = 0 ) UNION ( SELECT brParcele, abs( prazred_neplodno - round(prazred_neplodno)) AS A, 8 AS B FROM kom_kfmss WHERE obrisan = 0 )) AS GG WHERE A <> 0 AND brParcele IN ( SELECT brParcele FROM ( SELECT brParcele, ( Povrsina - sum( round(prazred_1) + round(Prazred_2) + round(Prazred_3) + round(Prazred_4) + round(Prazred_5) + round(Prazred_6) + round(Prazred_7) + round(prazred_neplodno))) AS dP FROM kom_kfmss WHERE obrisan = 0 GROUP BY brparcele HAVING dP <> 0 ) AS CC ) GROUP BY brparcele ) AS GG3 ON GG1.brParcele = GG3.brParcele ) WHERE GG1.brparcele = '" & ds_.Tables(0).Rows(i).Item(0).ToString & "' ORDER BY A DESC LIMIT " & Math.Abs(Val(ds_.Tables(0).Rows(i).Item(1)))
            'sada imas ono sto ti valjda treba daj da vidimo sta sad ide!
            'koje slucajeve imas?

            'pretpostavka je da nemas manje od jedan!
            'proveris da li je broj selectovanih jednak broju u limit-u ako nije onda je valjda problem!
            Dim ds2_ As New DataTable
            Dim myadapter22_ As New MySql.Data.MySqlClient.MySqlDataAdapter(comm_)
            Try

                myadapter22_.Fill(ds2_)

                'sada ovo imas u tabeli! pa mozes da ispitujes koliko i sta hoces!

                If ds2_.Rows.Count <> Math.Abs(Val(ds_.Tables(0).Rows(i).Item(1))) Then
                    'ovde ima neki problem treba videti koji!
                    MsgBox("Ovo je novi problem")
                Else
                    'sada sve ide po planu aj sad da vidimo sta dalje! i sta sad kako da smislis najbolji nacin kod ovog zaokruzivanja u stvari teoretskiovo moze da fula maksimalno 7 odnosno 8 ako cemo realno! ali kako da dei s
                    'aj prvo da resimo za 1 pa cemo dalje, sta cemo ako je ona veca od 1?

                    Dim ostatak_ As Integer = ds2_.Rows(0).Item(3)
                    Dim poslednji_ As Integer = ds2_.Rows.Count

                    For j = 0 To ds2_.Rows.Count - 2
                        Dim g_ As Integer = (ds2_.Rows(j).Item(1) / ds2_.Rows(j).Item(5)) * ds2_.Rows(j).Item(2)

                        If ds2_.Rows(j).Item(5) = 0.5 Then
                            'e ovo je zanimljivo ovo je da je tacno 0.5! tada mu dodaje dva puta!
                            g_ = 0
                        End If
                        'znaci ovo je popravka!
                        If ds2_.Rows(j).Item(2) = 8 Then
                            comm_.CommandText = "update kom_kfmss set prazred_neplodno=prazred_neplodno + " & g_ & " where brparcele='" & ds2_.Rows(j).Item(0) & "'"
                        Else
                            comm_.CommandText = "update kom_kfmss set prazred_" & ds2_.Rows(j).Item(2) & "=" & " prazred_" & ds2_.Rows(j).Item(2) & " + " & g_ & " where brparcele='" & ds2_.Rows(j).Item(0) & "'"
                        End If

                        comm_.ExecuteNonQuery()
                        ostatak_ -= g_
                    Next
                    'ovde se ispituje poslednji u nizu ! da bi mu se dodalo sve sto je ostalo!

                    If ds2_.Rows(poslednji_ - 1).Item(5) = 0.5 Then
                        If ds2_.Rows(poslednji_ - 1).Item(2) = 8 Then
                            comm_.CommandText = "update kom_kfmss set prazred_neplodno = Povrsina where brparcele='" & ds2_.Rows(poslednji_ - 1).Item(0) & "'"
                        Else
                            comm_.CommandText = "update kom_kfmss set prazred_" & ds2_.Rows(poslednji_ - 1).Item(2) & "=Povrsina where brparcele='" & ds2_.Rows(poslednji_ - 1).Item(0) & "'"
                        End If
                    Else
                        If ds2_.Rows(poslednji_ - 1).Item(2) = 8 Then
                            comm_.CommandText = "update kom_kfmss set prazred_neplodno = prazred_neplodno + " & ostatak_ & " where brparcele='" & ds2_.Rows(poslednji_ - 1).Item(0) & "'"
                        Else
                            comm_.CommandText = "update kom_kfmss set prazred_" & ds2_.Rows(poslednji_ - 1).Item(2) & "=" & " prazred_" & ds2_.Rows(poslednji_ - 1).Item(2) & " + " & ostatak_ & " where brparcele='" & ds2_.Rows(poslednji_ - 1).Item(0) & "'"
                        End If
                    End If

                    comm_.ExecuteNonQuery()

                End If

                'sada realno je sledeca prica ako podignes za jedan pa kada ga zaokruzis onda mislim da nema problem - u stvari to mozes i na kraju!
                ds2_ = Nothing
                myadapter22_ = Nothing
            Catch ex As Exception
                MsgBox("Ovde je problem sto vam povrsina u tabeli kom_kfmss nije zaokruzena proverite to! " & ex.Message)
            End Try
        Next

        comm_.CommandText = " update kom_kfmss set prazred_1=round(prazred_1),prazred_2=round(prazred_2),prazred_3=round(prazred_3),prazred_4=round(prazred_4),prazred_5=round(prazred_5),prazred_6=round(prazred_6),prazred_7=round(prazred_7),prazred_neplodno=round(prazred_neplodno)"
        comm_.ExecuteNonQuery()

        ds_ = Nothing : comm_ = Nothing : conn_.Close()
        pb1.Value = 0 : FileClose()
        MsgBox("Kraj")
    End Sub

    Private Sub ZaokruziGeometrijuNa2DecimaleToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ZaokruziGeometrijuNa2DecimaleToolStripMenuItem.Click
        'aj da probamo sledece!: da zaokruzimo sve poligone koordinata na 2 decimale!
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If MsgBox("Radim zaokruzivanje na layer-u: " & My.Settings.layerName_ParceleNadela & " ako zelite neki drugi layer promenite parametar: layerName_ParceleNadela", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
            pb1.Maximum = drw_.ObjectSet.Count
            pb1.Value = 0
            For i = 0 To drw_.ObjectSet.Count - 1
                'aj da probamo!
                Dim geom_ As Manifold.Interop.Geom = drw_.ObjectSet.Item(i).Geom
                For j = 0 To geom_.BranchSet.Count - 1
                    For k = 0 To geom_.BranchSet.Item(j).PointSet.Count - 1
                        For p = 0 To geom_.BranchSet.Item(j).PointSet.Count - 1
                            geom_.BranchSet.Item(j).PointSet(p).X = Math.Round(geom_.BranchSet.Item(j).PointSet(p).X, 2) : geom_.BranchSet.Item(j).PointSet(p).Y = Math.Round(geom_.BranchSet.Item(j).PointSet(p).Y, 2)
                        Next
                    Next
                Next
                pb1.Value = i
            Next
            pb1.Value = 0
            doc.Save()
            MsgBox("kraj ")
        End If
        doc = Nothing

    End Sub
    Private Sub StampajPozivFaktickoStanjePojedancno(brojIskaza As Integer)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Try
            conn_.Open()
        Catch ex As Exception
            MsgBox("Nemoguce uspostaviti vezu sa bazom. Proverite.")
            Exit Sub
        End Try


        Dim dt_ As New DataTable

        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter ': adapter_.SelectCommand = comm_ : adapter_.Fill(dt_) : adapter_ = Nothing

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application : docApp_.Visible = True

        pb1.Value = 0

        Dim dg_ As New DataTable
        comm_.CommandText = "SELECT DISTINCT fs_vezaparcelavlasnik.idvlasnika, concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') <> '', concat('(', IMEOCA, ')'), '' ), ' ', ifnull(IME, '')) AS imeprezime, ULICA, concat( ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS kucnibr, udeo, mesto FROM fs_vlasnik LEFT OUTER JOIN fs_vezaparcelavlasnik ON fs_vlasnik.idVlasnika = fs_vezaparcelavlasnik.idvlasnika WHERE idpl = " & brojIskaza
        Dim adapter3 As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter3.SelectCommand = comm_ : adapter3.Fill(dg_) : adapter3 = Nothing

        For k = 0 To dg_.Rows.Count - 1
            'sada tek mozes da napravis pricu za word!!!!!
            Dim wDoc_ As Microsoft.Office.Interop.Word.Document = docApp_.Documents.Open(My.Settings.pozivanje_wordFileTemplatePath)
            'otvoris word kao template

            wDoc_.SaveAs("D:\" & brojIskaza & "_" & Replace(dg_.Rows(k).Item(1).ToString, "\", "Đ") & ".doc")
            'sada mozes dalje!
            'idemo prvo na identifikaciju
            Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

            bokMarks_.Item("brojListaNepokretnosti").Range.Text = brojIskaza 'dg_.Rows(k).Item(5).ToString
            ' bokMarks_.Item("brojListaNepokretnosti1").Range.Text = brojIskaza 'dg_.Rows(k).Item(5).ToString
            bokMarks_.Item("broj").Range.Text = "" 'dg_.Rows(k).Item(3).ToString
            'If My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Then
            '    'ako je 1 - chekiran znaci nema datuma!

            'Else
            '    bokMarks_.Item("datum_gore").Range.Text = dg_.Rows(k).Item(6).ToString
            '    bokMarks_.Item("datum_gore1").Range.Text = dg_.Rows(k).Item(6).ToString
            '    bokMarks_.Item("datum_gore2").Range.Text = dg_.Rows(k).Item(6).ToString
            '    bokMarks_.Item("datumPoziva_dole").Range.Text = dg_.Rows(k).Item(6).ToString
            '    bokMarks_.Item("datumPoziva_gore").Range.Text = dg_.Rows(k).Item(6).ToString
            '    bokMarks_.Item("vremePoziva_dole").Range.Text = dg_.Rows(k).Item(7).ToString
            '    bokMarks_.Item("vremePoziva_gore").Range.Text = dg_.Rows(k).Item(7).ToString
            'End If

            bokMarks_.Item("indikacije_dole").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Đ")
            bokMarks_.Item("indikacije_gore").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Đ")
            bokMarks_.Item("indikacije_gore3").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Đ")
            bokMarks_.Item("mesto").Range.Text = dg_.Rows(k).Item(5).ToString
            bokMarks_.Item("ulica").Range.Text = dg_.Rows(k).Item(2).ToString & " " & dg_.Rows(k).Item(3).ToString

            'bokMarks_.Item("zavodnibroj1").Range.Text = dg_.Rows(k).Item(7).ToString
            'bokMarks_.Item("zavodnibroj2").Range.Text = dg_.Rows(k).Item(7).ToString
            'bokMarks_.Item("zavodnibroj3").Range.Text = dg_.Rows(k).Item(7).ToString
            wDoc_.Save()

            'proveris da li stampa tabelu sa spiskom parcela?

            If My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Then
                Dim wTable As Word.Table
                'sada u stvari imam novu selekciju a ovo moze da ide i kao rekord set!

                Dim parcele_ As New DataTable : Dim adapParcele As New MySql.Data.MySqlClient.MySqlDataAdapter
                comm_.CommandText = "SELECT idpl, brParceleF, udeo, NAZIV, ( hektari * 10000 + ari * 100 + metri ) AS P FROM ( SELECT DISTINCT idparcele, idpl, udeo FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & dg_.Rows.Item(k).Item(0).ToString & " AND obrisan = 0 ) AS A LEFT OUTER JOIN fs_parcele ON A.idparcele = fs_parcele.idParc LEFT OUTER JOIN kat_potesi ON fs_parcele.POTES = kat_potesi.SIFRA ORDER BY idpl, naziv, brParceleF"
                adapParcele.SelectCommand = comm_ : adapParcele.Fill(parcele_)
                Dim wordRange As Object ' Word.Range

                wordRange = wDoc_.Range(wDoc_.Range.Characters.Count - 1)
                wTable = wDoc_.Tables.Add(wordRange, parcele_.Rows.Count + 2, 5)

                With wTable
                    .Borders.Enable = True
                    .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                    .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                    .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                    .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                    .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                End With

                'sada header

                For j1 = 1 To 5
                    For k1 = 1 To parcele_.Rows.Count
                        wTable.Cell(k1 + 1, j1).Range.Text = parcele_.Rows(k1 - 1).Item(j1 - 1).ToString : wTable.Cell(k1 + 1, j1).Range.Font.Bold = False
                    Next
                Next

                wTable.Rows(1).Range.Font.Bold = True
                wTable.Rows(wTable.Rows.Count).Range.Font.Bold = True
                wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

                wTable.Cell(1, 1).Range.Text = "Бр. листа непокретности" : wTable.Cell(1, 2).Range.Text = "Број парцеле" : wTable.Cell(1, 3).Range.Text = "Удео" : wTable.Cell(1, 4).Range.Text = "Потес" : wTable.Cell(1, 5).Range.Text = "Површина (m2)"

                'sada idemo da uradimo po procembenim razredima 
                parcele_.Rows.Clear() : parcele_.Columns.Clear()

                wTable = Nothing
                wDoc_.Save()
                wDoc_.Close()
                parcele_ = Nothing
            End If

            'sad je ostalo jos da vidimo za parcele!!!!


        Next

        'dg_ = Nothing

        '    Next

        'dv_ = Nothing
        'pb1.Value = i
        'Next

        dt_ = Nothing
        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        MsgBox("Kraj")
    End Sub

    Private Sub StampajPoziveFaktickoStanjeSvi()

        'uf idemo prvo citamo sta kaze 
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Try
            conn_.Open()
        Catch ex As Exception
            MsgBox("Nemoguce uspostaviti vezu sa bazom. Proverite.")
            Exit Sub
        End Try

        Dim putanja As String
        'ubaci sta se da biras forlder
        fbd_diag.ShowDialog()
        If fbd_diag.SelectedPath = "" Then putanja = "d:\" Else putanja = fbd_diag.SelectedPath


        Dim dt_ As New DataTable

        comm_.CommandText = "SELECT DISTINCT datum_ FROM zapozivanje_fakticko ORDER BY cast(mid(datum_, 4, 2) AS UNSIGNED), cast(LEFT(datum_, 2) AS UNSIGNED)"
        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(dt_) : adapter_ = Nothing

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application
        docApp_.Visible = True
        pb1.Value = 0

        For i = 0 To dt_.Rows.Count - 1
            'sada idemo sa stampom
            'sada ide redni broj 
            Dim dv_ As New DataTable

            comm_.CommandText = "SELECT DISTINCT rednibroj FROM zapozivanje_fakticko WHERE datum_ = '" & dt_.Rows.Item(i).Item(0).ToString & "'"
            Dim adapter2_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter2_.SelectCommand = comm_ : adapter2_.Fill(dv_)
            adapter2_ = Nothing

            For j = 0 To dv_.Rows.Count - 1
                'sada idemo za svaki od rednih brojeva!?
                Dim dg_ As New DataTable
                'comm_.CommandText = "SELECT distinct zapozivanje_fakticko.idvlasnika, concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') <> '', concat('(', IMEOCA, ')'), '' ), ' ', ifnull(IME, '')) AS imeprezime, ifnull(ULICA,'') ULICA, concat( ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS kucnibr, udeo, group_concat(DISTINCT idpl SEPARATOR ';') AS idpls, datum_, vreme_, fs_vlasnik.mesto FROM zapozivanje_fakticko LEFT OUTER JOIN fs_vlasnik ON fs_vlasnik.idVlasnika = zapozivanje_fakticko.idvlasnika WHERE rednibroj = " & dv_.Rows(j).Item(0).ToString & " GROUP BY indikacije"
                comm_.CommandText = "SELECT idvlasnika, left(indikacije,instr(indikacije,',')-1) imeprezime, right(indikacije, instr(REVERSE(indikacije),',')-1) ULICA, '' kucnibr, udeo,  group_concat(DISTINCT idpl SEPARATOR ';') AS idpls,datum_, vreme_, mesto FROM zapozivanje_fakticko WHERE rednibroj = " & dv_.Rows(j).Item(0).ToString & " GROUP BY indikacije"

                Dim adapter3 As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter3.SelectCommand = comm_ : adapter3.Fill(dg_) : adapter3 = Nothing

                For k = 0 To dg_.Rows.Count - 1
                    'sada tek mozes da napravis pricu za word!!!!!
                    Dim wDoc_ As Microsoft.Office.Interop.Word.Document = docApp_.Documents.Open(My.Settings.pozivanje_wordFileTemplatePath)
                    'otvoris word kao template

                    wDoc_.SaveAs(putanja & "\" & Replace(dg_.Rows(k).Item(6).ToString, "/", "") & "_" & dv_.Rows(j).Item(0).ToString & "_" & dg_.Rows(k).Item(0).ToString & ".doc")
                    'sada mozes dalje!
                    'idemo prvo na identifikaciju
                    Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

                    Try
                        bokMarks_.Item("brojListaNepokretnosti").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("brojListaNepokretnosti1").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("broj").Range.Text = dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try

                    If My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Then
                        'ako je 1 - chekiran znaci nema datuma!

                    Else
                        Try
                            bokMarks_.Item("datum_gore").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datum_gore1").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datum_gore2").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datumPoziva_dole").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datumPoziva_gore").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("vremePoziva_dole").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("vremePoziva_gore").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                    End If

                    Try
                        bokMarks_.Item("indikacije_dole").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("indikacije_gore").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("mesto").Range.Text = Replace(dg_.Rows(k).Item(8).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("ulica").Range.Text = Replace(dg_.Rows(k).Item(2).ToString, "\", "Ђ") & " " & dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try

                    wDoc_.Save()

                    'proveris da li stampa tabelu sa spiskom parcela?

                    If My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Then

                        'sada treba nova strana!
                        wDoc_.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak)

                        Dim wTable As Word.Table
                        'sada u stvari imam novu selekciju a ovo moze da ide i kao rekord set!

                        Dim parcele_ As New DataTable : Dim adapParcele As New MySql.Data.MySqlClient.MySqlDataAdapter
                        comm_.CommandText = "SELECT idpl, brParceleF, udeo, NAZIV, ( hektari * 10000 + ari * 100 + metri ) AS P FROM ( SELECT DISTINCT idparcele, idpl, udeo FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & dg_.Rows.Item(k).Item(0).ToString & " AND obrisan = 0 ) AS A inner JOIN fs_parcele ON A.idparcele = fs_parcele.idParc and UKOMASACIJI=1 LEFT OUTER JOIN kat_potesi ON fs_parcele.POTES = kat_potesi.SIFRA ORDER BY idpl, naziv, brParceleF"
                        adapParcele.SelectCommand = comm_ : adapParcele.Fill(parcele_)
                        Dim wordRange As Object ' Word.Range

                        wordRange = wDoc_.Range(wDoc_.Range.Characters.Count - 1)
                        wTable = wDoc_.Tables.Add(wordRange, parcele_.Rows.Count + 2, 5)

                        With wTable
                            .Borders.Enable = True
                            .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                            .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                        End With

                        'sada header

                        For j1 = 1 To 5
                            For k1 = 1 To parcele_.Rows.Count
                                wTable.Cell(k1 + 1, j1).Range.Text = parcele_.Rows(k1 - 1).Item(j1 - 1).ToString : wTable.Cell(k1 + 1, j1).Range.Font.Bold = False
                            Next
                        Next

                        wTable.Rows(1).Range.Font.Bold = True
                        wTable.Rows(wTable.Rows.Count).Range.Font.Bold = True
                        wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

                        wTable.Cell(1, 1).Range.Text = "Бр. листа непокретности" : wTable.Cell(1, 2).Range.Text = "Број парцеле" : wTable.Cell(1, 3).Range.Text = "Удео" : wTable.Cell(1, 4).Range.Text = "Потес" : wTable.Cell(1, 5).Range.Text = "Површина (m2)"

                        'sada idemo da uradimo po procembenim razredima 
                        parcele_.Rows.Clear() : parcele_.Columns.Clear()

                        wTable = Nothing
                        wDoc_.Save()
                        wDoc_.Close()
                        parcele_ = Nothing
                    End If

                    'sad je ostalo jos da vidimo za parcele!!!!


                Next

                dg_ = Nothing

            Next

            dv_ = Nothing
            pb1.Value = i

        Next

        dt_ = Nothing
        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        MsgBox("Kraj")

    End Sub

    Private Sub StampajPoziveFaktickoStanjeSvi_old()

        'uf idemo prvo citamo sta kaze 
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Try
            conn_.Open()
        Catch ex As Exception
            MsgBox("Nemoguce uspostaviti vezu sa bazom. Proverite.")
            Exit Sub
        End Try


        Dim dt_ As New DataTable

        comm_.CommandText = "SELECT DISTINCT datum_ FROM zapozivanje_fakticko ORDER BY cast(mid(datum_, 4, 2) AS UNSIGNED), cast(LEFT(datum_, 2) AS UNSIGNED)"
        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(dt_) : adapter_ = Nothing

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application
        docApp_.Visible = True
        pb1.Value = 0

        For i = 0 To dt_.Rows.Count - 1
            'sada idemo sa stampom
            'sada ide redni broj 
            Dim dv_ As New DataTable

            comm_.CommandText = "SELECT DISTINCT rednibroj FROM zapozivanje_fakticko WHERE datum_ = '" & dt_.Rows.Item(i).Item(0).ToString & "'"
            Dim adapter2_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter2_.SelectCommand = comm_ : adapter2_.Fill(dv_)
            adapter2_ = Nothing

            For j = 0 To dv_.Rows.Count - 1
                'sada idemo za svaki od rednih brojeva!?
                Dim dg_ As New DataTable
                comm_.CommandText = "SELECT zapozivanje_fakticko.idvlasnika, concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') <> '', concat('(', IMEOCA, ')'), '' ), ' ', ifnull(IME, '')) AS imeprezime, ULICA, concat( ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS kucnibr, udeo, group_concat(DISTINCT idpl SEPARATOR ';') AS idpls, datum_, vreme_, fs_vlasnik.mesto FROM zapozivanje_fakticko LEFT OUTER JOIN fs_vlasnik ON fs_vlasnik.idVlasnika = zapozivanje_fakticko.idvlasnika WHERE rednibroj = " & dv_.Rows(j).Item(0).ToString & " GROUP BY indikacije"
                Dim adapter3 As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter3.SelectCommand = comm_ : adapter3.Fill(dg_) : adapter3 = Nothing

                For k = 0 To dg_.Rows.Count - 1
                    'sada tek mozes da napravis pricu za word!!!!!
                    Dim wDoc_ As Microsoft.Office.Interop.Word.Document = docApp_.Documents.Open(My.Settings.pozivanje_wordFileTemplatePath)
                    'otvoris word kao template

                    wDoc_.SaveAs("D:\LOCIKA_POZIVI\" & dv_.Rows(j).Item(0).ToString & "_" & dg_.Rows(k).Item(0).ToString & ".doc")
                    'sada mozes dalje!
                    'idemo prvo na identifikaciju
                    Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

                    Try
                        bokMarks_.Item("brojListaNepokretnosti").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("brojListaNepokretnosti1").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("broj").Range.Text = dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try

                    If My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Then
                        'ako je 1 - chekiran znaci nema datuma!

                    Else
                        Try
                            bokMarks_.Item("datum_gore").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datum_gore1").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datum_gore2").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datumPoziva_dole").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("datumPoziva_gore").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("vremePoziva_dole").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("vremePoziva_gore").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                    End If

                    Try
                        bokMarks_.Item("indikacije_dole").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("indikacije_gore").Range.Text = Replace(dg_.Rows(k).Item(1).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("mesto").Range.Text = Replace(dg_.Rows(k).Item(8).ToString, "\", "Ђ")
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("ulica").Range.Text = Replace(dg_.Rows(k).Item(2).ToString, "\", "Ђ") & " " & dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try


                    'bokMarks_.Item("zavodnibroj1").Range.Text = dg_.Rows(k).Item(7).ToString
                    'bokMarks_.Item("zavodnibroj2").Range.Text = dg_.Rows(k).Item(7).ToString
                    'bokMarks_.Item("zavodnibroj3").Range.Text = dg_.Rows(k).Item(7).ToString
                    wDoc_.Save()

                    'proveris da li stampa tabelu sa spiskom parcela?

                    If My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Then

                        'sada treba nova strana!
                        wDoc_.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak)

                        Dim wTable As Word.Table
                        'sada u stvari imam novu selekciju a ovo moze da ide i kao rekord set!

                        Dim parcele_ As New DataTable : Dim adapParcele As New MySql.Data.MySqlClient.MySqlDataAdapter
                        comm_.CommandText = "SELECT idpl, brParceleF, udeo, NAZIV, ( hektari * 10000 + ari * 100 + metri ) AS P FROM ( SELECT DISTINCT idparcele, idpl, udeo FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & dg_.Rows.Item(k).Item(0).ToString & " AND obrisan = 0 ) AS A inner JOIN fs_parcele ON A.idparcele = fs_parcele.idParc and UKOMASACIJI=1 LEFT OUTER JOIN kat_potesi ON fs_parcele.POTES = kat_potesi.SIFRA ORDER BY idpl, naziv, brParceleF"
                        adapParcele.SelectCommand = comm_ : adapParcele.Fill(parcele_)
                        Dim wordRange As Object ' Word.Range

                        wordRange = wDoc_.Range(wDoc_.Range.Characters.Count - 1)
                        wTable = wDoc_.Tables.Add(wordRange, parcele_.Rows.Count + 2, 5)

                        With wTable
                            .Borders.Enable = True
                            .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                            .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                            .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                        End With

                        'sada header

                        For j1 = 1 To 5
                            For k1 = 1 To parcele_.Rows.Count
                                wTable.Cell(k1 + 1, j1).Range.Text = parcele_.Rows(k1 - 1).Item(j1 - 1).ToString : wTable.Cell(k1 + 1, j1).Range.Font.Bold = False
                            Next
                        Next

                        wTable.Rows(1).Range.Font.Bold = True
                        wTable.Rows(wTable.Rows.Count).Range.Font.Bold = True
                        wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

                        wTable.Cell(1, 1).Range.Text = "Бр. листа непокретности" : wTable.Cell(1, 2).Range.Text = "Број парцеле" : wTable.Cell(1, 3).Range.Text = "Удео" : wTable.Cell(1, 4).Range.Text = "Потес" : wTable.Cell(1, 5).Range.Text = "Површина (m2)"

                        'sada idemo da uradimo po procembenim razredima 
                        parcele_.Rows.Clear() : parcele_.Columns.Clear()

                        wTable = Nothing
                        wDoc_.Save()
                        wDoc_.Close()
                        parcele_ = Nothing
                    End If

                    'sad je ostalo jos da vidimo za parcele!!!!


                Next

                dg_ = Nothing

            Next

            dv_ = Nothing
            pb1.Value = i
        Next

        dt_ = Nothing
        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        MsgBox("Kraj")

    End Sub

    Private Sub ZaokruziGeometrijuNa2DecimaleIzQueryaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ZaokruziGeometrijuNa2DecimaleIzQueryaToolStripMenuItem.Click
        'sada idemo ovo deluje kao mnogo elegantnije resenje nego sto je bilo ranije

        'gleda layer koji je definisan kao         My.Settings.layerName_ParceleNadela
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If MsgBox("Radim zaokruzivanje na layer-u: " & My.Settings.layerName_ParceleNadela & " ako zelite neki drugi layer promenite parametar: layerName_ParceleNadela", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then

            Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)

            'bilo bi dobro da napravi kopiju ovoga! ali u koji document
            Dim drwNew_ As Manifold.Interop.Drawing = doc.NewDrawing(My.Settings.layerName_ParceleNadela & "_2dec", drw_.CoordinateSystem)
            drw_.Copy()
            drwNew_.Paste()

            'sada mozes dalje

            Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("udff")
            qvr_.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[Geom (I)] as geom_,newArea_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToArea( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID],1 as broj_  FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[ID]=AA.id ) set geom_=newArea_"
            qvr_.RunEx(True)
            doc.ComponentSet.Remove("udff")
            doc.Save()

            MsgBox("Kraj")
        End If

        doc = Nothing
    End Sub

    Private Sub zaokruziGeomPovrsine2Dec(imeDrawinga As String)

        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(imeDrawinga)

        'bilo bi dobro da napravi kopiju ovoga! ali u koji document
        Dim drwNew_ As Manifold.Interop.Drawing = doc.NewDrawing(imeDrawinga & "_2dec", drw_.CoordinateSystem) : drw_.Copy() : drwNew_.Paste()

        'sada mozes dalje

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("udff")
        qvr_.Text = "OPTIONS COORDSYS(" & Chr(34) & imeDrawinga & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & (imeDrawinga & "_2dec") & "].[Geom (I)] as geom_,newArea_ FROM [" & (imeDrawinga & "_2dec") & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToArea( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & imeDrawinga & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & (imeDrawinga & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID], 1 as broj_  FROM [" & (imeDrawinga & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & (imeDrawinga & "_2dec") & "].[ID]=AA.id ) set geom_=newArea_"
        qvr_.RunEx(True)
        doc.ComponentSet.Remove("udff")
        'promenis ime da bi dalje nastavio sa istim file-om
        doc.ComponentSet.Remove(imeDrawinga)
        drwNew_.Name = imeDrawinga
        doc.Save()

        doc = Nothing

    End Sub

    Private Sub StampajPoziveNadela()

        'uf idemo prvo citamo sta kaze 
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Try
            conn_.Open()
        Catch ex As Exception
            MsgBox("Nemoguce uspostaviti vezu sa bazom. Proverite.")
            Exit Sub
        End Try


        Dim dt_ As New DataTable

        comm_.CommandText = "SELECT DISTINCT datum_ FROM zapozivanje_nadela ORDER BY cast(mid(datum_, 4, 2) AS UNSIGNED), cast(LEFT(datum_, 2) AS UNSIGNED)"
        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(dt_) : adapter_ = Nothing

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application
        docApp_.Visible = True
        pb1.Value = 0

        Dim folderPath_ As String = ""
        fbd_diag.ShowDialog()
        folderPath_ = fbd_diag.SelectedPath.ToString

        For i = 0 To dt_.Rows.Count - 1
            'sada idemo sa stampom
            'sada ide redni broj 
            Dim dv_ As New DataTable

            comm_.CommandText = "SELECT DISTINCT rednibroj FROM zapozivanje_nadela WHERE datum_ = '" & dt_.Rows.Item(i).Item(0).ToString & "'"
            Dim adapter2_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter2_.SelectCommand = comm_ : adapter2_.Fill(dv_)
            adapter2_ = Nothing

            For j = 0 To dv_.Rows.Count - 1
                'sada idemo za svaki od rednih brojeva!?
                Dim dg_ As New DataTable

                comm_.CommandText = "SELECT zapozivanje_nadela.idvlasnika, concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') <> '', concat('(', IMEOCA, ')'), '' ), ' ', ifnull(IME, '')) AS imeprezime, ULICA, concat( ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS kucnibr, udeo, group_concat(DISTINCT idiskazzemljista SEPARATOR ';') AS idpls, datum_, vreme_, kom_vlasnik.mesto FROM zapozivanje_nadela LEFT OUTER JOIN kom_vlasnik ON kom_vlasnik.idVlasnika = zapozivanje_nadela.idvlasnika WHERE rednibroj = " & dv_.Rows(j).Item(0).ToString & " GROUP BY indikacije"

                'comm_.CommandText = "SELECT B.idvlasnika, concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') <> '', concat('(', IMEOCA, ')'), '' ), ' ', ifnull(IME, '')) AS imeprezime, ULICA, concat( ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS kucnibr, udeo, group_concat( DISTINCT idiskazzemljista SEPARATOR ';' ) AS idpls, datum_, vreme_, kom_vlasnik.mesto, idopstina FROM ( SELECT zapozivanje_nadela.*, idopstina FROM zapozivanje_nadela LEFT OUTER JOIN ( SELECT idOpstina, zapozivanje_komisija.idPL, idVlasnika, autoid FROM zapozivanje_komisija LEFT OUTER JOIN kom_izt ON zapozivanje_komisija.idPL = kom_izt.idpl ORDER BY idOpstina ) AS A ON zapozivanje_nadela.idiskazzemljista = A.autoid AND zapozivanje_nadela.idvlasnika = A.idVlasnika ) AS B LEFT OUTER JOIN kom_vlasnik ON kom_vlasnik.idVlasnika = B.idvlasnika WHERE rednibroj = " & dv_.Rows(j).Item(0).ToString & " GROUP BY indikacije"

                Dim adapter3 As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter3.SelectCommand = comm_ : adapter3.Fill(dg_) : adapter3 = Nothing

                For k = 0 To dg_.Rows.Count - 1
                    'sada tek mozes da napravis pricu za word!!!!!
                    Dim wDoc_ As Microsoft.Office.Interop.Word.Document = docApp_.Documents.Open(My.Settings.pozivanje_wordFileTemplatePath)
                    'otvoris word kao template

                    wDoc_.SaveAs(folderPath_ & "\" & Replace(dg_.Rows(k).Item(6).ToString, "/", "_") & "DD_" & dv_.Rows(j).Item(0).ToString & "_" & dg_.Rows(k).Item(0).ToString & ".doc")
                    'sada mozes dalje!
                    'idemo prvo na identifikaciju
                    Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

                    'bokMarks_.Item("brojListaNepokretnosti").Range.Text = dg_.Rows(k).Item(5).ToString
                    'bokMarks_.Item("brojListaNepokretnosti1").Range.Text = dg_.Rows(k).Item(5).ToString
                    'bokMarks_.Item("broj").Range.Text = dg_.Rows(k).Item(3).ToString
                    If My.Settings.pozivanje_pisemSamoImenaBezVremena = 1 Then
                        '    'ako je 1 - chekiran znaci nema datuma!

                    Else
                        'bokMarks_.Item("datum_gore").Range.Text = dg_.Rows(k).Item(6).ToString
                        'bokMarks_.Item("datum_gore1").Range.Text = dg_.Rows(k).Item(6).ToString
                        'bokMarks_.Item("datum_gore2").Range.Text = dg_.Rows(k).Item(6).ToString
                        Try
                            bokMarks_.Item("dole_datum").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("gore_datum").Range.Text = dg_.Rows(k).Item(6).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("dole_vreme").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                        Try
                            bokMarks_.Item("gore_vreme").Range.Text = dg_.Rows(k).Item(7).ToString
                        Catch ex As Exception

                        End Try

                    End If

                    Try
                        bokMarks_.Item("dole_indikacije").Range.Text = dg_.Rows(k).Item(1).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("gore_indikacije").Range.Text = dg_.Rows(k).Item(1).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("gore_mesto").Range.Text = dg_.Rows(k).Item(8).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("dole_mesto").Range.Text = dg_.Rows(k).Item(8).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("gore_ulica").Range.Text = dg_.Rows(k).Item(2).ToString & " " & dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("dole_ulica").Range.Text = dg_.Rows(k).Item(2).ToString & " " & dg_.Rows(k).Item(3).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("gore_brojZapisnika").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try

                    Try
                        bokMarks_.Item("dole_brojZapisnika").Range.Text = dg_.Rows(k).Item(5).ToString
                    Catch ex As Exception

                    End Try
                    'bokMarks_.Item("zavodnibroj3").Range.Text = dg_.Rows(k).Item(7).ToString
                    wDoc_.Save()

                    'proveris da li stampa tabelu sa spiskom parcela?

                    'If My.Settings.pozivanje_stampamSpisakParcelaUPozivu = 1 Then
                    '    Dim wTable As Word.Table
                    '    'sada u stvari imam novu selekciju a ovo moze da ide i kao rekord set!

                    '    Dim parcele_ As New DataTable : Dim adapParcele As New MySql.Data.MySqlClient.MySqlDataAdapter
                    '    comm_.CommandText = "SELECT idiskazzemljista, brParceleF, udeo, NAZIV, ( hektari * 10000 + ari * 100 + metri ) AS P FROM ( SELECT DISTINCT idparcele, idiskazzemljista, udeo FROM kom_vezaparcelavlasnik WHERE idVlasnika = " & dg_.Rows.Item(k).Item(0).ToString & " AND obrisan = 0 ) AS A LEFT OUTER JOIN kom_parcele ON A.idparcele = kom_parcele.idParc LEFT OUTER JOIN kat_potesi ON kom_parcele.POTES = kat_potesi.SIFRA ORDER BY idiskazzemljista, naziv, brParceleF"
                    '    adapParcele.SelectCommand = comm_ : adapParcele.Fill(parcele_)
                    '    Dim wordRange As Object ' Word.Range

                    '    wordRange = wDoc_.Range(wDoc_.Range.Characters.Count - 1)
                    '    wTable = wDoc_.Tables.Add(wordRange, parcele_.Rows.Count + 2, 5)

                    '    With wTable
                    '        .Borders.Enable = True
                    '        .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                    '        .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                    '        .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                    '        .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                    '        .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                    '    End With

                    '    'sada header

                    '    For j1 = 1 To 5
                    '        For k1 = 1 To parcele_.Rows.Count
                    '            wTable.Cell(k1 + 1, j1).Range.Text = parcele_.Rows(k1 - 1).Item(j1 - 1).ToString : wTable.Cell(k1 + 1, j1).Range.Font.Bold = False
                    '        Next
                    '    Next

                    '    wTable.Rows(1).Range.Font.Bold = True
                    '    wTable.Rows(wTable.Rows.Count).Range.Font.Bold = True
                    '    wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

                    '    wTable.Cell(1, 1).Range.Text = "Бр. листа непокретности" : wTable.Cell(1, 2).Range.Text = "Број парцеле" : wTable.Cell(1, 3).Range.Text = "Удео" : wTable.Cell(1, 4).Range.Text = "Потес" : wTable.Cell(1, 5).Range.Text = "Површина (m2)"

                    '    'sada idemo da uradimo po procembenim razredima 
                    '    parcele_.Rows.Clear() : parcele_.Columns.Clear()

                    ' wTable = Nothing
                    'wDoc_.Save()
                    wDoc_.Close()
                    'parcele_ = Nothing
                    'End If

                    'sad je ostalo jos da vidimo za parcele!!!!


                Next

                dg_ = Nothing

            Next

            dv_ = Nothing
            pb1.Value = i
        Next

        dt_ = Nothing
        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing
        MsgBox("Kraj")

    End Sub

    Private Sub CSVaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles CSVaToolStripMenuItem.Click

        'ides u petlji po fileovima

        Dim brOpstine As Integer = InputBox("Unesite idko, za feketic je 801461, a za lovcenac ", "Unos podataka", 801500)

        If brOpstine = 0 Then Exit Sub

        fbd_diag.SelectedPath = ""
        fbd_diag.ShowDialog()

        Me.Cursor = Cursors.WaitCursor

        lbl_infoMain.Text = "Ucitavam podatke iz baze"
        My.Application.DoEvents()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "SELECT kom_iskazzemljista.idIskaza, obrisan, IF(isnull(A.VSumaNEZ) ,- 1, A.VSumaNEZ) FROM kom_iskazzemljista LEFT OUTER JOIN ( SELECT idiskazzemljista, round( sum( koefUdeo * ( prazred_1 + Prazred_2 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 2 AND idko = " & brOpstine & " ) + Prazred_3 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 3 AND idko = " & brOpstine & " ) + Prazred_4 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 4 AND idko = " & brOpstine & " ) + Prazred_5 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 5 AND idko = " & brOpstine & " ) + Prazred_6 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 6 AND idko = " & brOpstine & " ) + Prazred_7 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 7 AND idko = " & brOpstine & " ))) * ( SELECT opisText FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' AND idko = " & brOpstine & " ), 2 ) AS VSumaNEZ FROM kom_vezaparcelavlasnik, kom_kfmss WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_kfmss.obrisan = 0 AND kom_kfmss.idko = " & brOpstine & " AND kom_vezaparcelavlasnik.idParcele = kom_kfmss.idParc AND (sr = 1 OR sr = 2) GROUP BY idiskazzemljista ORDER BY idiskazzemljista ) AS A ON kom_iskazzemljista.idIskaza = A.idiskazzemljista WHERE kom_iskazzemljista.idko = " & brOpstine
        Dim myreader_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        Dim matcsvStrukture(-1) As csvKontrola
        Dim brojac_ As Integer = 1


        Do While myreader_.Read
            ReDim Preserve matcsvStrukture(brojac_)
            matcsvStrukture(brojac_).idIskaza = myreader_.GetValue(0)
            matcsvStrukture(brojac_).iskazStatus = myreader_.GetValue(1)
            Try
                matcsvStrukture(brojac_).iskazVrednost = myreader_.GetValue(2)
            Catch ex As Exception
                matcsvStrukture(brojac_).iskazVrednost = -1
            End Try
            brojac_ += 1
        Loop

        lbl_infoMain.Text = "Ucitavam podatke iz CSV-ova"
        My.Application.DoEvents()

        For Each f_ In System.IO.Directory.GetFiles(fbd_diag.SelectedPath, "*.csv")
            'sada za svaki file idemo redom ucitavanje
            Dim fileReader_ As New StreamReader(f_)
            Do While fileReader_.Peek <> -1
                Dim a_ = fileReader_.ReadLine.Split(",")
                If Val(a_(0)) <> 0 Then
                    For i = 0 To matcsvStrukture.Length - 1
                        If matcsvStrukture(i).idIskaza = Val(a_(0)) Then
                            matcsvStrukture(i).iskazNadeljenCSV = matcsvStrukture(i).iskazNadeljenCSV + Val(a_(1))
                            matcsvStrukture(i).listingFileova = matcsvStrukture(i).listingFileova & "," & f_
                            Exit For
                        End If
                    Next
                End If

            Loop
        Next

        'sada nesto uraditi sa ovim? ali sta 
        sf_diag.FileName = ""
        sf_diag.Filter = "Text Files (*.txt)|*.txt"
        sf_diag.ShowDialog()

        If sf_diag.FileName = "" Then sf_diag.FileName = Path.GetTempPath() & "\csvKontrola.txt"

        lbl_infoMain.Text = "Pisem podatke u file"
        My.Application.DoEvents()

        Dim fileWrite_ As New StreamWriter(sf_diag.FileName)
        fileWrite_.WriteLine("idIskaza;statusIskaza;zaNadeluVrednost;iskazNadeljenCSV;listingFileova")
        For i = 0 To matcsvStrukture.Length - 1
            fileWrite_.WriteLine(matcsvStrukture(i).idIskaza & ";" & matcsvStrukture(i).iskazStatus & ";" & matcsvStrukture(i).iskazVrednost & ";" & matcsvStrukture(i).iskazNadeljenCSV & ";" & matcsvStrukture(i).listingFileova)
        Next
        fileWrite_.Close()
        lbl_infoMain.Text = ""
        Me.Cursor = Cursors.Default
        MsgBox("Kraj")
    End Sub


    Public Sub podelaNaListoveUTM(razmera_ As Integer)

        'granica ti je data u layer-u centri moci jer dok radis postavi da je podela na listove u setting-u
        'radis podelu na listove za layer podela na listove i to za selektovano! a moramo da vidimo gde cemo to da smestimo :)

        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Me.Cursor = Cursors.WaitCursor

        lbl_infoMain.Text = "Brisem prethodne podele ako postoje"
        My.Application.DoEvents()

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela50000")
            doc.ComponentSet.Remove("podela50000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela5000")
            doc.ComponentSet.Remove("podela5000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2500")
            doc.ComponentSet.Remove("podela2500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela1000")
            doc.ComponentSet.Remove("podela1000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela500")
            doc.ComponentSet.Remove("podela500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Dim yts1_, xts1_, yts2_, xts2_ As Integer
        Dim y50001_, x50001_, y50002_, x50002_ As Integer
        Dim y25001_, x25001_, y25002_, x25002_ As Integer
        Dim y10001_, x10001_, y10002_, x10002_ As Integer
        Dim y5001_, x5001_, y5002_, x5002_ As Integer

        Dim NOMENKLATU_ As String = ""

        yts1_ = 0 : xts1_ = 0 : yts2_ = 0 : xts2_ = 0
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("cetiriTacke")
        qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]  WHERE [Selection (I)]=true ) SPLIT by Coords(geom_) as pnt_"
        qvr_.RunEx(True)

        If qvr_.Table.RecordSet.Count = 0 Then
            'ovo znaci da nema nista selektovano - uzimas sve sto je u drawingu i nastavljas dalje
            qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]) SPLIT by Coords(geom_) as pnt_"
            qvr_.RunEx(True)

        End If

        lbl_infoMain.Text = "Radim podelu za 50000"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = 320000 To 680000 Step 30000
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 30000)) Then
                    For j = 4620000 To 5120000 Step 20000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 20000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                yts1_ = i : xts1_ = j
                                yts2_ = i + 30000 : xts2_ = j + 20000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > yts2_ Then yts2_ = i + 30000
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > xts2_ Then xts2_ = j + 20000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'ovo je za razmeru 50000 iz koje ide dalje podela
        Dim drwUTM_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_podelaNaListove)

        Dim drw_ As Manifold.Interop.Drawing = doc.NewDrawing("podela50000", drwUTM_.CoordinateSystem, True)
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "NOMENKLATU"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText
        drw_.OwnedTable.ColumnSet.Add(col_)
        'ovde treba napraviti update to je najbolje

        lbl_infoMain.Text = "Radim podelu za 5000"
        My.Application.DoEvents()

        Dim qvrInsert As Manifold.Interop.Query = doc.NewQuery("insertQ")

        For i = yts1_ To yts2_ - 30000 Step 30000
            For j = xts1_ To xts2_ - 20000 Step 20000
                qvrInsert.Text = "insert into [podela50000] ([NOMENKLATU],[Geom (I)]) values (" & Chr(34) & "E" & ((i - 200000) / 30000) + 1 & "-N" & ((j - 4600000) / 20000) + 1 & Chr(34) & ", AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 30000 & " " & j & "," & i + 30000 & " " & j + 20000 & "," & i & " " & j + 20000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)
            Next
        Next

        If razmera_ = 50000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        'sada moze na 5000

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts1_ + 30000 Step 3000
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 3000)) Then
                    For j = xts1_ To xts2_ Step 2000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 2000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y50001_ = i : x50001_ = j
                                y50002_ = i + 3000 : x50002_ = j + 2000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y50002_ Then y50002_ = i + 3000
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x50002_ Then x50002_ = j + 2000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela5000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        'ovde je veci problem za nomenklaturu jer moze da bude vise od jedne trig sekcije!!!
        For i = y50001_ To y50002_ - 3000 Step 3000
            For j = x50001_ To x50002_ - 2000 Step 2000

                qvrInsert.Text = "insert into [podela5000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 3000 & " " & j & "," & i + 3000 & " " & j + 2000 & "," & i & " " & j + 2000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela50000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1500 & "," & j + 1000 & "), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 3000) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 2000
                    If n > 1 Then n = (n - 1) * 10 Else n = 0
                    qvrInsert.Text = "update [podela5000] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela5000])"
                    qvrInsert.RunEx(True)
                End If


            Next
        Next

        If razmera_ = 5000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        'sada moze na 2500
        lbl_infoMain.Text = "Radim podelu za 2500"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y50001_ To y50002_ + 1500 Step 1500
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 1500)) Then
                    For j = x50001_ To x50002_ Step 1000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y25001_ = i : x25001_ = j
                                y25002_ = i + 1500 : x25002_ = j + 1000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 1500
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela2500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        NOMENKLATU_ = NOMENKLATU_ & " " & ((y50002_ - y50001_) / 3000) * ((x50002_ - x50001_) / 2000)
        For i = y25001_ To y25002_ - 1500 Step 1500
            For j = x25001_ To x25002_ - 1000 Step 1000
                qvrInsert.Text = "insert into [podela2500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 1500 & " " & j & "," & i + 1500 & " " & j + 1000 & "," & i & " " & j + 1000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 750 & "," & j + 500 & "), COORDSYS(" & Chr(34) & "Podela5000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 1500) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1000
                    'MsgBox("m=" & m & ", n=" & n & "nom=" & qvrInsert.Table.RecordSet(0).DataText(3))
                    If n > 1 Then n = (n - 1) * 2 Else n = 0
                    'If m > 1 Then m = (m - 1) * 2 Else m = 0

                    'Dim ubaci_ As Integer = -1
                    'Select Case (m + n)
                    '    Case 1
                    '        ubaci_ = 1
                    '    Case 2
                    '        ubaci_ = 3
                    '    Case 3
                    '        ubaci_ = 2
                    '    Case 4
                    '        ubaci_ = 4
                    'End Select

                    ' MsgBox("m=" & m & ", n=" & n)
                    qvrInsert.Text = "update [podela2500] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela2500])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 2500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 1000"
        My.Application.DoEvents()

        'sada moze na 1000 ide iz 5000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y50001_ To y50002_ + 600 Step 600
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 600)) Then
                    For j = x50001_ To x50002_ Step 400
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 400)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y10001_ = i : x10001_ = j
                                y10002_ = i + 600 : x10002_ = j + 400
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y10002_ Then y10002_ = i + 600
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x10002_ Then x10002_ = j + 400
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela1000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y10001_ To y10002_ - 600 Step 600
            For j = x10001_ To x10002_ - 400 Step 400
                qvrInsert.Text = "insert into [podela1000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 600 & " " & j & "," & i + 600 & " " & j + 400 & "," & i & " " & j + 400 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 200 & "), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 600) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 400
                    If n > 1 Then n = (n - 1) * 5 Else n = 0
                    qvrInsert.Text = "update [podela1000] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela1000])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 1000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 500"
        My.Application.DoEvents()
        'sada moze na 500 koja ide iz 1000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y10001_ To y10002_ + 300 Step 300
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 300)) Then
                    For j = x10001_ To x10002_ Step 200
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 200)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y5001_ = i : x5001_ = j
                                y5002_ = i + 300 : x5002_ = j + 200
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y5002_ Then y5002_ = i + 300
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x5002_ Then x5002_ = j + 200
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y5001_ To y5002_ - 300 Step 300
            For j = x5001_ To x5002_ - 200 Step 200
                qvrInsert.Text = "insert into [podela500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 300 & " " & j & "," & i + 300 & " " & j + 200 & "," & i & " " & j + 200 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela500" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela1000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 200 & "), COORDSYS(" & Chr(34) & "Podela1000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 300) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 200
                    If n > 1 Then n = (n - 1) * 2 Else n = 0
                    qvrInsert.Text = "update [podela500] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela500])"
                    qvrInsert.RunEx(True)
                End If


            Next
        Next

        If razmera_ = 500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        doc.ComponentSet.Remove("cetiriTacke")
        doc.ComponentSet.Remove("insertQ")
        doc.Save()

        lbl_infoMain.Text = ""
        Cursor = Cursors.Default

        MsgBox("Kraj")

    End Sub

    Private Sub podelaNaListoveUTM50000_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveUTM50000.Click
        podelaNaListoveUTM(50000)
    End Sub

    Private Sub podelaNaListoveUTM5000_Click_1(sender As Object, e As System.EventArgs) Handles podelaNaListoveUTM5000.Click
        podelaNaListoveUTM(5000)
    End Sub

    Private Sub podelaNaListoveUTM1000_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveUTM1000.Click
        podelaNaListoveUTM(1000)
    End Sub

    Private Sub podelaNaListoveUTM2500_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveUTM2500.Click
        podelaNaListoveUTM(2500)
    End Sub

    Private Sub podelaNaListoveUTM500_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveUTM500.Click
        podelaNaListoveUTM(500)
    End Sub

    'Public Sub podelaNaListoveGK(razmera_ As Integer)

    '    Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

    '    Me.Cursor = Cursors.WaitCursor

    '    lbl_infoMain.Text = "Brisem prethodne podele ako postoje"
    '    My.Application.DoEvents()

    '    Try
    '        Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela50000")
    '        doc.ComponentSet.Remove("podela50000")
    '        pp_ = Nothing
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela5000")
    '        doc.ComponentSet.Remove("podela5000")
    '        pp_ = Nothing
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2500")
    '        doc.ComponentSet.Remove("podela2500")
    '        pp_ = Nothing
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela1000")
    '        doc.ComponentSet.Remove("podela1000")
    '        pp_ = Nothing
    '    Catch ex As Exception

    '    End Try

    '    Try
    '        Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela500")
    '        doc.ComponentSet.Remove("podela500")
    '        pp_ = Nothing
    '    Catch ex As Exception

    '    End Try

    '    Dim yts1_, xts1_, yts2_, xts2_ As Integer
    '    Dim y50001_, x50001_, y50002_, x50002_ As Integer
    '    Dim y25001_, x25001_, y25002_, x25002_ As Integer
    '    Dim y10001_, x10001_, y10002_, x10002_ As Integer
    '    Dim y5001_, x5001_, y5002_, x5002_ As Integer

    '    Dim NOMENKLATU_ As String = ""

    '    yts1_ = 0 : xts1_ = 0 : yts2_ = 0 : xts2_ = 0
    '    Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("cetiriTacke")
    '    qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]  WHERE [Selection (I)]=true ) SPLIT by Coords(geom_) as pnt_"
    '    qvr_.RunEx(True)

    '    If qvr_.Table.RecordSet.Count = 0 Then
    '        'ovo znaci da nema nista selektovano - uzimas sve sto je u drawingu i nastavljas dalje
    '        qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]) SPLIT by Coords(geom_) as pnt_"
    '        qvr_.RunEx(True)

    '    End If

    '    lbl_infoMain.Text = "Radim podelu za 50000"
    '    My.Application.DoEvents()

    '    For k = 0 To qvr_.Table.RecordSet.Count - 1
    '        For i = 320000 To 680000 Step 30000
    '            If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 30000)) Then
    '                For j = 4620000 To 5120000 Step 20000
    '                    'sada mogu unutra po drugoj osi
    '                    If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 20000)) Then
    '                        'sada si ga nasao!!!!!
    '                        If k = 0 Then
    '                            yts1_ = i : xts1_ = j
    '                            yts2_ = i + 30000 : xts2_ = j + 20000
    '                            Exit For
    '                        Else
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > yts2_ Then yts2_ = i + 30000
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > xts2_ Then xts2_ = j + 20000
    '                            Exit For
    '                        End If

    '                    End If

    '                Next
    '            End If
    '        Next
    '    Next
    '    'ovo je za razmeru 50000 iz koje ide dalje podela
    '    Dim drwUTM_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_podelaNaListove)

    '    Dim drw_ As Manifold.Interop.Drawing = doc.NewDrawing("podela50000", drwUTM_.CoordinateSystem, True)
    '    Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
    '    col_.Name = "NOMENKLATU"
    '    col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText
    '    drw_.OwnedTable.ColumnSet.Add(col_)
    '    'ovde treba napraviti update to je najbolje

    '    lbl_infoMain.Text = "Radim podelu za 5000"
    '    My.Application.DoEvents()

    '    Dim qvrInsert As Manifold.Interop.Query = doc.NewQuery("insertQ")

    '    For i = yts1_ To yts2_ - 30000 Step 30000
    '        For j = xts1_ To xts2_ - 20000 Step 20000
    '            qvrInsert.Text = "insert into [podela50000] ([NOMENKLATU],[Geom (I)]) values (" & Chr(34) & "E" & ((i - 200000) / 30000) + 1 & "-N" & ((j - 4600000) / 20000) + 1 & Chr(34) & ", AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 30000 & " " & j & "," & i + 30000 & " " & j + 20000 & "," & i & " " & j + 20000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
    '            qvrInsert.RunEx(True)
    '        Next
    '    Next

    '    If razmera_ = 50000 Then
    '        doc.ComponentSet.Remove("cetiriTacke")
    '        doc.ComponentSet.Remove("insertQ")
    '        doc.Save()
    '        MsgBox("Kraj ")
    '        Exit Sub
    '    End If

    '    'sada moze na 5000

    '    For k = 0 To qvr_.Table.RecordSet.Count - 1
    '        For i = yts1_ To yts1_ + 30000 Step 3000
    '            If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 3000)) Then
    '                For j = xts1_ To xts2_ Step 2000
    '                    'sada mogu unutra po drugoj osi
    '                    If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 2000)) Then
    '                        'sada si ga nasao!!!!!
    '                        If k = 0 Then
    '                            y50001_ = i : x50001_ = j
    '                            y50002_ = i + 3000 : x50002_ = j + 2000
    '                            Exit For
    '                        Else
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y50002_ Then y50002_ = i + 3000
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x50002_ Then x50002_ = j + 2000
    '                            Exit For
    '                        End If

    '                    End If

    '                Next
    '            End If
    '        Next
    '    Next

    '    'sada ides u krug od do pa iscrtavas poligone za 5000
    '    drw_ = doc.NewDrawing("podela5000", drwUTM_.CoordinateSystem, True)
    '    drw_.OwnedTable.ColumnSet.Add(col_)

    '    'ovde je veci problem za nomenklaturu jer moze da bude vise od jedne trig sekcije!!!
    '    For i = y50001_ To y50002_ - 3000 Step 3000
    '        For j = x50001_ To x50002_ - 2000 Step 2000

    '            qvrInsert.Text = "insert into [podela5000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 3000 & " " & j & "," & i + 3000 & " " & j + 2000 & "," & i & " " & j + 2000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
    '            qvrInsert.RunEx(True)

    '            'sada za ovaj list treba da nades trigsekciju
    '            qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela50000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1500 & "," & j + 1000 & "), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
    '            'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
    '            qvrInsert.RunEx(True)

    '            If qvrInsert.Table.RecordSet.Count <> 1 Then
    '                'nesto nije ok
    '            Else
    '                'sada ga imas - ide ti x,y, NOMENKLATU

    '                Dim m, n As Integer
    '                m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 3000) + 1
    '                n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 2000
    '                If n > 1 Then n = (n - 1) * 10 Else n = 0
    '                qvrInsert.Text = "update [podela5000] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela5000])"
    '                qvrInsert.RunEx(True)
    '            End If


    '        Next
    '    Next

    '    If razmera_ = 5000 Then
    '        doc.ComponentSet.Remove("cetiriTacke")
    '        doc.ComponentSet.Remove("insertQ")
    '        doc.Save()
    '        MsgBox("Kraj ")
    '        Exit Sub
    '    End If

    '    'sada moze na 2500
    '    lbl_infoMain.Text = "Radim podelu za 2500"
    '    My.Application.DoEvents()

    '    For k = 0 To qvr_.Table.RecordSet.Count - 1
    '        For i = y50001_ To y50002_ + 1500 Step 1500
    '            If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 1500)) Then
    '                For j = x50001_ To x50002_ Step 1000
    '                    'sada mogu unutra po drugoj osi
    '                    If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1000)) Then
    '                        'sada si ga nasao!!!!!
    '                        If k = 0 Then
    '                            y25001_ = i : x25001_ = j
    '                            y25002_ = i + 1500 : x25002_ = j + 1000
    '                            Exit For
    '                        Else
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 1500
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1000
    '                            Exit For
    '                        End If

    '                    End If

    '                Next
    '            End If
    '        Next
    '    Next

    '    'sada ides u krug od do pa iscrtavas poligone za 5000
    '    drw_ = doc.NewDrawing("podela2500", drwUTM_.CoordinateSystem, True)
    '    drw_.OwnedTable.ColumnSet.Add(col_)

    '    NOMENKLATU_ = NOMENKLATU_ & " " & ((y50002_ - y50001_) / 3000) * ((x50002_ - x50001_) / 2000)
    '    For i = y25001_ To y25002_ - 1500 Step 1500
    '        For j = x25001_ To x25002_ - 1000 Step 1000
    '            qvrInsert.Text = "insert into [podela2500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 1500 & " " & j & "," & i + 1500 & " " & j + 1000 & "," & i & " " & j + 1000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
    '            qvrInsert.RunEx(True)

    '            'sada za ovaj list treba da nades trigsekciju
    '            qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 750 & "," & j + 500 & "), COORDSYS(" & Chr(34) & "Podela5000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
    '            'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
    '            qvrInsert.RunEx(True)

    '            If qvrInsert.Table.RecordSet.Count <> 1 Then
    '                'nesto nije ok
    '            Else
    '                'sada ga imas - ide ti x,y, NOMENKLATU

    '                Dim m, n As Integer
    '                m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 1500) + 1
    '                n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1000
    '                'MsgBox("m=" & m & ", n=" & n & "nom=" & qvrInsert.Table.RecordSet(0).DataText(3))
    '                If n > 1 Then n = (n - 1) * 2 Else n = 0
    '                'If m > 1 Then m = (m - 1) * 2 Else m = 0

    '                'Dim ubaci_ As Integer = -1
    '                'Select Case (m + n)
    '                '    Case 1
    '                '        ubaci_ = 1
    '                '    Case 2
    '                '        ubaci_ = 3
    '                '    Case 3
    '                '        ubaci_ = 2
    '                '    Case 4
    '                '        ubaci_ = 4
    '                'End Select

    '                ' MsgBox("m=" & m & ", n=" & n)
    '                qvrInsert.Text = "update [podela2500] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela2500])"
    '                qvrInsert.RunEx(True)
    '            End If

    '        Next
    '    Next

    '    If razmera_ = 2500 Then
    '        doc.ComponentSet.Remove("cetiriTacke")
    '        doc.ComponentSet.Remove("insertQ")
    '        doc.Save()
    '        MsgBox("Kraj ")
    '        Exit Sub
    '    End If

    '    lbl_infoMain.Text = "Radim podelu za 1000"
    '    My.Application.DoEvents()

    '    'sada moze na 1000 ide iz 5000
    '    For k = 0 To qvr_.Table.RecordSet.Count - 1
    '        For i = y50001_ To y50002_ + 600 Step 600
    '            If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 600)) Then
    '                For j = x50001_ To x50002_ Step 400
    '                    'sada mogu unutra po drugoj osi
    '                    If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 400)) Then
    '                        'sada si ga nasao!!!!!
    '                        If k = 0 Then
    '                            y10001_ = i : x10001_ = j
    '                            y10002_ = i + 600 : x10002_ = j + 400
    '                            Exit For
    '                        Else
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y10002_ Then y10002_ = i + 600
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x10002_ Then x10002_ = j + 400
    '                            Exit For
    '                        End If

    '                    End If

    '                Next
    '            End If
    '        Next
    '    Next
    '    'sada ides u krug od do pa iscrtavas poligone za 5000
    '    drw_ = doc.NewDrawing("podela1000", drwUTM_.CoordinateSystem, True)
    '    drw_.OwnedTable.ColumnSet.Add(col_)

    '    For i = y10001_ To y10002_ - 600 Step 600
    '        For j = x10001_ To x10002_ - 400 Step 400
    '            qvrInsert.Text = "insert into [podela1000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 600 & " " & j & "," & i + 600 & " " & j + 400 & "," & i & " " & j + 400 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT)))"
    '            qvrInsert.RunEx(True)

    '            qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 200 & "), COORDSYS(" & Chr(34) & "Podela50000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
    '            'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
    '            qvrInsert.RunEx(True)

    '            If qvrInsert.Table.RecordSet.Count <> 1 Then
    '                'nesto nije ok
    '            Else
    '                'sada ga imas - ide ti x,y, NOMENKLATU

    '                Dim m, n As Integer
    '                m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 600) + 1
    '                n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 400
    '                If n > 1 Then n = (n - 1) * 5 Else n = 0
    '                qvrInsert.Text = "update [podela1000] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela1000])"
    '                qvrInsert.RunEx(True)
    '            End If

    '        Next
    '    Next

    '    If razmera_ = 1000 Then
    '        doc.ComponentSet.Remove("cetiriTacke")
    '        doc.ComponentSet.Remove("insertQ")
    '        doc.Save()
    '        MsgBox("Kraj ")
    '        Exit Sub
    '    End If

    '    lbl_infoMain.Text = "Radim podelu za 500"
    '    My.Application.DoEvents()
    '    'sada moze na 500 koja ide iz 1000
    '    For k = 0 To qvr_.Table.RecordSet.Count - 1
    '        For i = y10001_ To y10002_ + 300 Step 300
    '            If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 300)) Then
    '                For j = x10001_ To x10002_ Step 200
    '                    'sada mogu unutra po drugoj osi
    '                    If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 200)) Then
    '                        'sada si ga nasao!!!!!
    '                        If k = 0 Then
    '                            y5001_ = i : x5001_ = j
    '                            y5002_ = i + 300 : x5002_ = j + 200
    '                            Exit For
    '                        Else
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y5002_ Then y5002_ = i + 300
    '                            If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x5002_ Then x5002_ = j + 200
    '                            Exit For
    '                        End If
    '                    End If
    '                Next
    '            End If
    '        Next
    '    Next
    '    'sada ides u krug od do pa iscrtavas poligone za 5000
    '    drw_ = doc.NewDrawing("podela500", drwUTM_.CoordinateSystem, True)
    '    drw_.OwnedTable.ColumnSet.Add(col_)

    '    For i = y5001_ To y5002_ - 300 Step 300
    '        For j = x5001_ To x5002_ - 200 Step 200
    '            qvrInsert.Text = "insert into [podela500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 300 & " " & j & "," & i + 300 & " " & j + 200 & "," & i & " " & j + 200 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela500" & Chr(34) & " as COMPONENT)))"
    '            qvrInsert.RunEx(True)

    '            'sada za ovaj list treba da nades trigsekciju
    '            qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela1000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 200 & "), COORDSYS(" & Chr(34) & "Podela1000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
    '            'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
    '            qvrInsert.RunEx(True)

    '            If qvrInsert.Table.RecordSet.Count <> 1 Then
    '                'nesto nije ok
    '            Else
    '                'sada ga imas - ide ti x,y, NOMENKLATU

    '                Dim m, n As Integer
    '                m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 300) + 1
    '                n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 200
    '                If n > 1 Then n = (n - 1) * 2 Else n = 0
    '                qvrInsert.Text = "update [podela500] set [NOMENKLATU]=" & Chr(34) & (m + n) & "-" & qvrInsert.Table.RecordSet(0).DataText(3) & Chr(34) & " where [id]=(select max([id]) from [podela500])"
    '                qvrInsert.RunEx(True)
    '            End If


    '        Next
    '    Next

    '    If razmera_ = 500 Then
    '        doc.ComponentSet.Remove("cetiriTacke")
    '        doc.ComponentSet.Remove("insertQ")
    '        doc.Save()
    '        MsgBox("Kraj ")
    '        Exit Sub
    '    End If

    '    doc.ComponentSet.Remove("cetiriTacke")
    '    doc.ComponentSet.Remove("insertQ")
    '    doc.Save()

    '    lbl_infoMain.Text = ""
    '    Cursor = Cursors.Default

    '    MsgBox("Kraj")

    'End Sub

    Private Sub mnu_pozivi_fs_generisi_Click(sender As Object, e As System.EventArgs) Handles mnu_pozivi_fs_generisi.Click


        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        comm_.CommandText = "CREATE TABLE `zapozivanje_fakticko` ( `idpl` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
        conn_.Open()
        Try
            comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table zapozivanje_fakticko"
            comm_.ExecuteNonQuery()
            comm_.CommandText = "CREATE TABLE `zapozivanje_fakticko` ( `idpl` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
            comm_.ExecuteNonQuery()
        End Try

        Dim stsql_ As String = "" '"INSERT INTO zapozivanje_fakticko ( idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )( ) AS B "

        'sada u zavisnosti od podesavanja definises query! imas trenutno dva podesavanja a to je za zelje i cestitke i za 
        'da li je LN samo u gradevinskom!

        'If My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Then
        '    stsql_ = stsql_ & " WHERE zeljeucesnika IS NULL AND brojpredmeta = '' " & " ) AS C1 LEFT OUTER JOIN ( SELECT B2.idPL, br_vlas, br_parc FROM ( SELECT idPL, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idPL, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idPL = B3.idPL ) AS C2 ON C1.idpl = C2.idPL ORDER BY slucaj, udeo, idpl )"
        'Else
        '    stsql_ = stsql_ & " ) AS C1 LEFT OUTER JOIN ( SELECT B2.idPL, br_vlas, br_parc FROM ( SELECT idPL, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idPL, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idPL = B3.idPL ) AS C2 ON C1.idpl = C2.idPL ORDER BY slucaj, udeo, idpl )"
        'End If
        If MsgBox("Da li ima prioriteta definisanih po Iskazu?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            'ima prioriteta
            'stsql_ = "INSERT INTO zapozivanje_fakticko ( idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT GG.*, 1 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 1 ) GG UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 5 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idpl, br_vlas, br_parc FROM ( SELECT idpl, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idpl, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idpl = B3.idpl ) AS C2 ON C1.idpl = C2.idpl ORDER BY slucaj, udeo, idpl)"

            stsql_ = "INSERT INTO zapozivanje_fakticko ( idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT GG.*, 1 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 1 ) GG UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 5 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 AND fs_raspravnizapisnik.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idpl, br_vlas, br_parc FROM ( SELECT idpl, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idpl, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idpl = B3.idpl ) AS C2 ON C1.idpl = C2.idpl ORDER BY slucaj, udeo, idpl)"

        Else
            'stsql_ = "INSERT INTO zapozivanje_fakticko ( idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 5 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idpl, br_vlas, br_parc FROM ( SELECT idpl, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idpl, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idpl = B3.idpl ) AS C2 ON C1.idpl = C2.idpl ORDER BY slucaj, udeo, idpl)"

            stsql_ = "INSERT INTO zapozivanje_fakticko ( idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idpl, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 6 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idpl, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idpl = fs_raspravnizapisnik.idpl WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idpl, br_vlas, br_parc FROM ( SELECT idpl, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik where idParcele in (select idparc from fs_parcele where deoparcele=0 and ukomasaciji=1)) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idpl, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik where idParcele in (select idparc from fs_parcele where deoparcele=0 and ukomasaciji=1)) AS B1 GROUP BY idpl ) AS B3 ON B2.idpl = B3.idpl ) AS C2 ON C1.idpl = C2.idpl ORDER BY slucaj, udeo, idpl)"
            'nema prioriteta
        End If


        Try
            'comm_.CommandText = "CREATE TABLE zaPozivanje SELECT C1.*, br_vlas, br_parc, (" & My.Settings.pozivanje_nultoVreme & " + br_vlas * " & My.Settings.pozivanje_vremePosedovni & " + br_parc * " & My.Settings.pozivanje_vremeBrojParcela & ") AS min_,  0 as prosao_, 0 as rednibroj  FROM ( SELECT * FROM ( SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = 'RADUJEVAC' UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> 'RADUJEVAC' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = 'RADUJEVAC' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> 'RADUJEVAC' ) AS B WHERE zeljeucesnika IS NULL AND brojpredmeta = '' ) AS C1 LEFT OUTER JOIN ( SELECT B2.idPL, br_vlas, br_parc FROM ( SELECT idPL, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idPL, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idPL = B3.idPL ) AS C2 ON C1.idpl = C2.idPL ORDER BY idpl"
            comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table zapozivanje_fakticko" : comm_.ExecuteNonQuery() : comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        End Try

        'sada treba izdvojiti one koji su samo u gradevinkom i dajes im slucaj 5 a ovaj slucaj ne tretiras!

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            comm_.CommandText = "UPDATE zapozivanje_fakticko SET slucaj = 5 WHERE idpl IN ( SELECT KL.idpl FROM (( SELECT idpl FROM ( SELECT idpl, ukomasaciji FROM ( SELECT A.*, B.idpl FROM ( SELECT brparcelef, ukomasaciji, idParc FROM fs_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idpl FROM fs_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idpl ORDER BY idpl ) AS GG GROUP BY idpl HAVING count(*) = 1 ) AS KL INNER JOIN ( SELECT idpl, ukomasaciji FROM ( SELECT A.*, B.idpl FROM ( SELECT brparcelef, ukomasaciji, idParc FROM fs_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idpl FROM fs_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idpl ORDER BY idpl ) AS MM ON KL.idpl = MM.idpl ) WHERE ukomasaciji = 0 )"
            comm_.ExecuteNonQuery() : comm_.CommandText = "delete from zapozivanje_fakticko where slucaj=5" : comm_.ExecuteNonQuery()
        End If

        If My.Settings.pozivanje_kriterijumIzbaciIndustrijsku = 1 Then
            comm_.CommandText = "UPDATE zapozivanje_fakticko SET slucaj = 5 WHERE idpl IN ( SELECT KL.idpl FROM (( SELECT idpl FROM ( SELECT idpl, ukomasaciji FROM ( SELECT A.*, B.idpl FROM ( SELECT brparcelef, ukomasaciji, idParc FROM fs_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idpl FROM fs_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idpl ORDER BY idpl ) AS GG GROUP BY idpl HAVING count(*) = 1 ) AS KL INNER JOIN ( SELECT idpl, ukomasaciji FROM ( SELECT A.*, B.idpl FROM ( SELECT brparcelef, ukomasaciji, idParc FROM fs_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idpl FROM fs_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idpl ORDER BY idpl ) AS MM ON KL.idpl = MM.idpl ) WHERE ukomasaciji = 4 )"
            comm_.ExecuteNonQuery() : comm_.CommandText = "delete from zapozivanje_fakticko where slucaj=5" : comm_.ExecuteNonQuery()
        End If

        'ovo podeliti u dva dela - pa onda nema veze sto se tice prioriteta - njih postaviti na 1
        ' comm_.CommandText = "update zapozivanje_fakticko set slucaj=5 where slucaj=1"
        comm_.CommandText = "update zapozivanje_fakticko set rednibroj=0" : comm_.ExecuteNonQuery()

        comm_.CommandText = "Select distinct idvlasnika from zapozivanje_fakticko"
        Dim ds_ As New DataTable : Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(ds_)

        'fali mi polje rednibroj ! 
        Dim rednibroj As Integer = 1 : pb1.Value = 0 : pb1.Maximum = ds_.Rows.Count

        'sada idemo pocetak dokle i kraj 
        'treba pre toga videti sta je sa smenama jer onda imas dva cekinga!

        'kako sam to resio: ako je druga smena pocetak i kraj 

        Dim jednaSmena As Boolean = False
        'Dim g_, pp_
        Dim datumPocetka_ As DateTime

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then jednaSmena = True Else jednaSmena = False

        'ovde ima sate i minute!

        Dim g_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim pp_ = My.Settings.pozivanje_pocetakDatum.Split("/")
        If g_.Length = 1 Then datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), 0, 0) Else datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), g_(1), 0)


        Dim prethodnoVreme As Double = 0 : Dim trenutnoVreme As Double = 0 : Dim smena_ As Integer = 1

        For i = 0 To ds_.Rows.Count - 1
            'sada ide selekcija za svakog pojedinacno pa idemo da biramo sve posedovne 


            'sada mozes da proveris sta se desava dalje koliko imas cega - odnosno koliko ti vremena treba! - treba ti broj listova i treba ti 
            'datum kada pocinje i vreme da ga smestis odmah

            comm_.CommandText = "SELECT " & My.Settings.pozivanje_nultoVreme & "+ br_pl*" & My.Settings.pozivanje_vremePosedovni & " + br_parc*" & My.Settings.pozivanje_vremeBrojParcela & " + br_vlas*" & My.Settings.pozivanje_vremeVlasnik & " FROM ( SELECT count(*) AS br_pl FROM ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " ) AS E1 ) AS E2, (SELECT count(*) br_parc FROM ( SELECT idparcele AS br_parc FROM ( SELECT DISTINCT idparcele FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E3 ) AS E4 where br_parc in (select idparc from fs_parcele where deoparcele=0 and ukomasaciji=1)) as MM44, ( SELECT count(*) AS br_vlas FROM ( SELECT DISTINCT idVlasnika FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E5 ) AS E6"
            'sada imas vreme u minutima koliko ce da traje!
            Dim read_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)
            read_.Read() : trenutnoVreme = read_.GetValue(0) : read_.Close()

            Try
                conn_.Open()
            Catch ex As Exception
            End Try

            'ogranicenje po osobi primena:
            If trenutnoVreme > Val(My.Settings.ogranicenje_poStranci) Then
                trenutnoVreme = Val(My.Settings.ogranicenje_poStranci)
            End If

            'comm_.CommandText = "UPDATE zapozivanje_fakticko INNER JOIN ( SELECT * FROM zapozivanje_fakticko WHERE idpl IN ( SELECT idpl FROM zapozivanje_fakticko WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " AND redniBroj = 0 )) AS G ON zapozivanje_fakticko.idpl = G.idpl SET zapozivanje_fakticko.rednibroj = " & rednibroj & ", zapozivanje_fakticko.minuti_=" & trenutnoVreme

            comm_.CommandText = "UPDATE zapozivanje_fakticko SET zapozivanje_fakticko.rednibroj = " & rednibroj & ", zapozivanje_fakticko.minuti_ = " & trenutnoVreme & " WHERE idpl IN ( SELECT idpl FROM ( SELECT * FROM zapozivanje_fakticko ) aH WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " AND redniBroj = 0 )"
            Dim dalipostoji As Integer = comm_.ExecuteNonQuery()

            'proveris da li je vlasnik vec pozivan ako jeste ides na sledeci ne ulazis u racunicu

            If dalipostoji > 0 Then

                'sada mozes na datum!

                'sada idemo da kreiramo novi datum i dodamo ovo vreme

                datumPocetka_ = datumPocetka_.AddMinutes(trenutnoVreme)


                If jednaSmena = True Then
                    'sada treba videti kako da se zatvori na kraj poslednje radno vreme?!
                    Dim bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") : Dim dKraj As DateTime

                    'datum pozivanja, vreme pozivanja

                    If bg_.Length = 1 Then dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)

                    comm_.CommandText = "update zapozivanje_fakticko set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()
                    'comm_.CommandText = "update zapozivanje set datum_='" & datumPocetka_.ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()

                    If p_ > 0 Then
                        'sada resetujes datum koji poredis pre ovoga moras da proveris da li je u pitanju 

                        'sta se sada desava sa datumom!
                        datumPocetka_ = datumPocetka_.AddDays(1)
                        'sada treba proveriti da li je subora iuli nedelja

                        'ako je subota
                        If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                        'ako je nedelja
                        If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                        'ides na ponedeljak

                        'treba proveriti samo dali je praznik@!

                        'sada kada imas resetujes vreme!
                        Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                        If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)

                        datumPocetka_ = gtime_

                    Else
                        'ovde proveriti dali je razlika 

                    End If

                Else
                    'sada prvo vidis da li je ovaj datum veci od kraja prve smene -  ako nije onda mu dodas, ako jeste saltas ga na 4 
                    Dim dKraj, Spocetak As DateTime : Dim bg_

                    If smena_ = 1 Then bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") Else bg_ = My.Settings.pozivanje_smena2Kraj.Split(":")

                    If bg_.Length = 1 Then
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0)
                    Else
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    End If


                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)


                    comm_.CommandText = "update zapozivanje_fakticko set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj    'sada je zanimljivo pitanje sta pravis update!?"
                    Dim uradioUpdate As Integer = comm_.ExecuteNonQuery()


                    If p_ >= 0 Then
                        'dosao si na promenu smene - sto znaci sta! - menjas smenu u 2
                        bg_ = My.Settings.pozivanje_smena2Pocetak.Split(":")
                        If bg_.Length = 1 Then Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)

                        Dim q_ = DateTime.Compare(datumPocetka_, Spocetak)

                        'ako je q_ manje od  1 znaci da treba saltati na drugu smenu!
                        If q_ <= 0 Then
                            'saltas  na pocetak na drugu smenu!'sada postavljas na pocetak ono drugo!
                            If smena_ = 1 Then
                                datumPocetka_ = Spocetak
                                smena_ = 2
                            End If

                            'sda postavis na nesto sto mozes 
                        Else
                            'sada prelazis na sledeci nivo!

                            datumPocetka_ = datumPocetka_.AddDays(1)
                            'sada treba proveriti da li je subora iuli nedelja

                            'ako je subota
                            If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                            'ako je nedelja
                            If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                            'ides na ponedeljak

                            'treba proveriti samo dali je praznik@!

                            'sada kada imas resetujes vreme!
                            Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                            If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)
                            datumPocetka_ = gtime_
                            smena_ = 1

                        End If

                        'sada je fora! ako je 
                    End If

                End If


            End If
            pb1.Value = i : rednibroj += 1

        Next

        stsql_ = "update kom_parametri set opisText='Formula: " & My.Settings.pozivanje_nultoVreme & " + brojposedovnihListva*" & My.Settings.pozivanje_vremePosedovni & " + brojVlasnika*" & My.Settings.pozivanje_vremeVlasnik & " + brojParcela*" & My.Settings.pozivanje_vremeBrojParcela &
            ". Datum pocetka: " & My.Settings.pozivanje_pocetakDatum & " . Komisija pocinje sa radom " & My.Settings.pozivanje_smena1Pocetak

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then
            'imas samo jedan
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena1Kraj
        Else
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena2Kraj & ", sa pauzom od " & My.Settings.pozivanje_smena1Kraj & " do " & My.Settings.pozivanje_smena2Pocetak
        End If

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu su iskljuceni iz plana pozivanja."
        Else
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu nisu iskljuceni iz plana pozivanja."
        End If

        If My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Then
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali nisu ukljuceno u plan pozivanja."
        Else
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali jesu ukljuceno u plan pozivanja."
        End If

        stsql_ = stsql_ & "' where opis='Pozivanje'"
        comm_.CommandText = stsql_
        comm_.ExecuteNonQuery()

        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        pb1.Value = 0

        If My.Settings.pozivanje_stampamOdmah = 1 Then
            StampajPoziveFaktickoStanjeSvi()
        End If



        MsgBox("Kraj")
    End Sub

    Private Sub mnu_pozivi_fs_stampa_svi_Click(sender As Object, e As System.EventArgs) Handles mnu_pozivi_fs_stampa_svi.Click
        StampajPoziveFaktickoStanjeSvi()
    End Sub

    Private Sub mnu_pozivi_fs_stampa_pojedinacno_Click(sender As Object, e As System.EventArgs) Handles mnu_pozivi_fs_stampa_pojedinacno.Click
        Dim broj_ As Integer = InputBox("Unesi broj iskaza", "Unesi broj iskaza", "1")
        If Val(broj_) > 0 Then StampajPozivFaktickoStanjePojedancno(broj_) Else MsgBox("Uneli ste pogresan iskaz")
    End Sub

    Private Sub mnu_pozivi_nadela_generisi_Click(sender As Object, e As System.EventArgs) Handles mnu_pozivi_nadela_generisi.Click
        'prvi nades one listove nepokretnosti koji imaju samo gradevinski rejon - znaci to je ono sto se zove sifra 5

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        comm_.CommandText = "CREATE TABLE `zapozivanje_nadela` ( `idiskazzemljista` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
        conn_.Open()
        Try
            comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table zapozivanje_nadela"
            comm_.ExecuteNonQuery()
            comm_.CommandText = "CREATE TABLE `zapozivanje_nadela` ( `idiskazzemljista` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
            comm_.ExecuteNonQuery()
        End Try

        Dim stsql_ As String = "" '"INSERT INTO zapozivanje_nadela ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )( ) AS B "

        If MsgBox("Da li ima prioriteta definisanih po Iskazu?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            stsql_ = "INSERT INTO zapozivanje_nadela ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idIskaza, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT GG.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 1 ) GG UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 5 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idiskazzemljista, br_vlas, br_parc FROM ( SELECT idiskazzemljista, count(*) AS br_vlas FROM ( SELECT DISTINCT idiskazzemljista, idVlasnika FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B2 LEFT OUTER JOIN ( SELECT idiskazzemljista, count(*) AS br_parc FROM ( SELECT DISTINCT idiskazzemljista, idparcele FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B3 ON B2.idiskazzemljista = B3.idiskazzemljista ) AS C2 ON C1.idiskaza = C2.idiskazzemljista ORDER BY slucaj, udeo, idiskazzemljista)"
        Else
            stsql_ = "INSERT INTO zapozivanje_nadela ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idIskaza, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idiskazzemljista, br_vlas, br_parc FROM ( SELECT idiskazzemljista, count(*) AS br_vlas FROM ( SELECT DISTINCT idiskazzemljista, idVlasnika FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B2 LEFT OUTER JOIN ( SELECT idiskazzemljista, count(*) AS br_parc FROM ( SELECT DISTINCT idiskazzemljista, idparcele FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B3 ON B2.idiskazzemljista = B3.idiskazzemljista ) AS C2 ON C1.idiskaza = C2.idiskazzemljista ORDER BY slucaj, udeo, idiskazzemljista)"
        End If


        Try
            comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table zapozivanje_nadela" : comm_.ExecuteNonQuery() : comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        End Try

        'sada treba izdvojiti one koji su samo u gradevinkom i dajes im slucaj 5 a ovaj slucaj ne tretiras!

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            comm_.CommandText = "UPDATE zapozivanje_nadela SET slucaj = 5 WHERE idiskazzemljista IN ( SELECT KL.idiskazzemljista FROM (( SELECT idiskazzemljista FROM ( SELECT idiskazzemljista, ukomasaciji FROM ( SELECT A.*, B.idiskazzemljista FROM ( SELECT brparcelef, ukomasaciji, idParc FROM kom_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idiskazzemljista FROM kom_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idiskazzemljista ORDER BY idiskazzemljista ) AS GG GROUP BY idiskazzemljista HAVING count(*) = 1 ) AS KL INNER JOIN ( SELECT idiskazzemljista, ukomasaciji FROM ( SELECT A.*, B.idiskazzemljista FROM ( SELECT brparcelef, ukomasaciji, idParc FROM kom_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idiskazzemljista FROM kom_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idiskazzemljista ORDER BY idiskazzemljista ) AS MM ON KL.idiskazzemljista = MM.idiskazzemljista ) WHERE ukomasaciji = 0 )"
            comm_.ExecuteNonQuery() : comm_.CommandText = "delete from zapozivanje_nadela where slucaj=5" : comm_.ExecuteNonQuery()
        End If


        'ovo podeliti u dva dela - pa onda nema veze sto se tice prioriteta - njih postaviti na 1

        comm_.CommandText = "update zapozivanje_nadela set rednibroj=0" : comm_.ExecuteNonQuery()

        comm_.CommandText = "Select distinct idvlasnika from zapozivanje_nadela"
        Dim ds_ As New DataTable : Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(ds_)

        'fali mi polje rednibroj ! 
        Dim rednibroj As Integer = 1 : pb1.Value = 0 : pb1.Maximum = ds_.Rows.Count

        'sada idemo pocetak dokle i kraj 
        'treba pre toga videti sta je sa smenama jer onda imas dva cekinga!

        'kako sam to resio: ako je druga smena pocetak i kraj 

        Dim jednaSmena As Boolean = False
        'Dim g_, pp_
        Dim datumPocetka_ As DateTime

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then jednaSmena = True Else jednaSmena = False

        'ovde ima sate i minute!

        Dim g_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim pp_ = My.Settings.pozivanje_pocetakDatum.Split("/")
        If g_.Length = 1 Then datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), 0, 0) Else datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), g_(1), 0)


        Dim prethodnoVreme As Double = 0 : Dim trenutnoVreme As Double = 0 : Dim smena_ As Integer = 1

        For i = 0 To ds_.Rows.Count - 1
            'sada ide selekcija za svakog pojedinacno pa idemo da biramo sve posedovne 


            'sada mozes da proveris sta se desava dalje koliko imas cega - odnosno koliko ti vremena treba! - treba ti broj listova i treba ti 
            'datum kada pocinje i vreme da ga smestis odmah

            comm_.CommandText = "SELECT " & My.Settings.pozivanje_nultoVreme & "+ br_pl*" & My.Settings.pozivanje_vremePosedovni & " + br_parc*" & My.Settings.pozivanje_vremeBrojParcela & " + br_vlas*" & My.Settings.pozivanje_vremeVlasnik & " FROM ( SELECT count(*) AS br_pl FROM ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " ) AS E1 ) AS E2, ( SELECT count(*) AS br_parc FROM ( SELECT DISTINCT idparcele FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E3 ) AS E4, ( SELECT count(*) AS br_vlas FROM ( SELECT DISTINCT idVlasnika FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E5 ) AS E6"
            'sada imas vreme u minutima koliko ce da traje!
            Dim read_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)
            read_.Read() : trenutnoVreme = read_.GetValue(0) : read_.Close()

            Try
                conn_.Open()
            Catch ex As Exception
            End Try

            'ogranicenje po osobi primena:
            If trenutnoVreme > Val(My.Settings.ogranicenje_poStranci) Then
                trenutnoVreme = Val(My.Settings.ogranicenje_poStranci)
            End If

            'comm_.CommandText = "UPDATE zapozivanje_nadela INNER JOIN ( SELECT * FROM zapozivanje_nadela WHERE idiskazzemljista IN ( SELECT idiskazzemljista FROM zapozivanje_nadela WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " AND redniBroj = 0 )) AS G ON zapozivanje_nadela.idiskazzemljista = G.idiskazzemljista SET zapozivanje_nadela.rednibroj = " & rednibroj & ", zapozivanje_nadela.minuti_=" & trenutnoVreme

            comm_.CommandText = "UPDATE zapozivanje_nadela SET zapozivanje_nadela.rednibroj = " & rednibroj & ", zapozivanje_nadela.minuti_ = " & trenutnoVreme & " WHERE idiskazzemljista IN ( SELECT idiskazzemljista FROM ( SELECT * FROM zapozivanje_nadela ) aH WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " AND redniBroj = 0 )"
            Dim dalipostoji As Integer = comm_.ExecuteNonQuery()

            'proveris da li je vlasnik vec pozivan ako jeste ides na sledeci ne ulazis u racunicu

            If dalipostoji > 0 Then

                'sada mozes na datum!

                'sada idemo da kreiramo novi datum i dodamo ovo vreme

                datumPocetka_ = datumPocetka_.AddMinutes(trenutnoVreme)


                If jednaSmena = True Then
                    'sada treba videti kako da se zatvori na kraj poslednje radno vreme?!
                    Dim bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") : Dim dKraj As DateTime

                    'datum pozivanja, vreme pozivanja

                    If bg_.Length = 1 Then dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)

                    comm_.CommandText = "update zapozivanje_nadela set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()
                    'comm_.CommandText = "update zapozivanje set datum_='" & datumPocetka_.ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()

                    If p_ > 0 Then
                        'sada resetujes datum koji poredis pre ovoga moras da proveris da li je u pitanju 

                        'sta se sada desava sa datumom!
                        datumPocetka_ = datumPocetka_.AddDays(1)
                        'sada treba proveriti da li je subora iuli nedelja

                        'ako je subota
                        If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                        'ako je nedelja
                        If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                        'ides na ponedeljak

                        'treba proveriti samo dali je praznik@!

                        'sada kada imas resetujes vreme!
                        Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                        If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)

                        datumPocetka_ = gtime_

                    Else
                        'ovde proveriti dali je razlika 

                    End If

                Else
                    'sada prvo vidis da li je ovaj datum veci od kraja prve smene -  ako nije onda mu dodas, ako jeste saltas ga na 4 
                    Dim dKraj, Spocetak As DateTime : Dim bg_

                    If smena_ = 1 Then bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") Else bg_ = My.Settings.pozivanje_smena2Kraj.Split(":")

                    If bg_.Length = 1 Then
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0)
                    Else
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    End If


                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)


                    comm_.CommandText = "update zapozivanje_nadela set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj    'sada je zanimljivo pitanje sta pravis update!?"
                    Dim uradioUpdate As Integer = comm_.ExecuteNonQuery()


                    If p_ >= 0 Then
                        'dosao si na promenu smene - sto znaci sta! - menjas smenu u 2
                        bg_ = My.Settings.pozivanje_smena2Pocetak.Split(":")
                        If bg_.Length = 1 Then Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)

                        Dim q_ = DateTime.Compare(datumPocetka_, Spocetak)

                        'ako je q_ manje od  1 znaci da treba saltati na drugu smenu!
                        If q_ <= 0 Then
                            'saltas  na pocetak na drugu smenu!'sada postavljas na pocetak ono drugo!
                            If smena_ = 1 Then
                                datumPocetka_ = Spocetak
                                smena_ = 2
                            End If

                            'sda postavis na nesto sto mozes 
                        Else
                            'sada prelazis na sledeci nivo!

                            datumPocetka_ = datumPocetka_.AddDays(1)
                            'sada treba proveriti da li je subora iuli nedelja

                            'ako je subota
                            If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                            'ako je nedelja
                            If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                            'ides na ponedeljak

                            'treba proveriti samo dali je praznik@!

                            'sada kada imas resetujes vreme!
                            Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                            If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)
                            datumPocetka_ = gtime_
                            smena_ = 1

                        End If

                        'sada je fora! ako je 
                    End If

                End If


            End If
            pb1.Value = i : rednibroj += 1

        Next

        stsql_ = "update kom_parametri set opisText='Formula: " & My.Settings.pozivanje_nultoVreme & " + brojposedovnihListva*" & My.Settings.pozivanje_vremePosedovni & " + brojVlasnika*" & My.Settings.pozivanje_vremeVlasnik & " + brojParcela*" & My.Settings.pozivanje_vremeBrojParcela &
            ". Datum pocetka: " & My.Settings.pozivanje_pocetakDatum & " . Komisija pocinje sa radom " & My.Settings.pozivanje_smena1Pocetak

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then
            'imas samo jedan
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena1Kraj
        Else
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena2Kraj & ", sa pauzom od " & My.Settings.pozivanje_smena1Kraj & " do " & My.Settings.pozivanje_smena2Pocetak
        End If

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu su iskljuceni iz plana pozivanja."
        Else
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu nisu iskljuceni iz plana pozivanja."
        End If

        If My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Then
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali nisu ukljuceno u plan pozivanja."
        Else
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali jesu ukljuceno u plan pozivanja."
        End If

        stsql_ = stsql_ & "' where opis='Pozivanje'"
        comm_.CommandText = stsql_
        comm_.ExecuteNonQuery()

        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        pb1.Value = 0

        If My.Settings.pozivanje_stampamOdmah = 1 Then
            StampajPoziveFaktickoStanjeSvi()
        End If


        'sada ovo mozes da ucitas pa da idemo dalje!
        MsgBox("Kraj")
    End Sub

    Private Sub mnu_pozivi_nadela_stampa_svi_Click(sender As Object, e As System.EventArgs) Handles mnu_pozivi_nadela_stampa_svi.Click
        StampajPoziveNadela()
    End Sub

    Private Sub generisiRasporedPozivanja(izlaznaTabela As String)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        comm_.CommandText = "CREATE TABLE `" & izlaznaTabela & "` ( `idiskazzemljista` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
        conn_.Open()
        Try
            comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table " & izlaznaTabela
            comm_.ExecuteNonQuery()
            comm_.CommandText = "CREATE TABLE `" & izlaznaTabela & "` ( `idiskazzemljista` INT NULL, `udeo` CHAR (20) NULL, `indikacije` LONGTEXT NULL, `idvlasnika` INT NULL, `sifralica` INT NULL, `mesto` CHAR (150) NULL, `slucaj` INT NULL, `br_LN` INT NULL, `br_vlas` INT NULL, `br_parc` INT NULL, `minuti_` DOUBLE NULL, `rednibroj` INT NULL, `datum_` CHAR (20) NULL, `vreme_` CHAR (20) NULL );"
            comm_.ExecuteNonQuery()
        End Try

        Dim stsql_ As String = "" '"INSERT INTO " & izlaznaTabela & " ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )( ) AS B "

        If MsgBox("Da li ima prioriteta definisanih po Iskazu?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            'ima prioriteta
            stsql_ = "INSERT INTO " & izlaznaTabela & " ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idIskaza, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT GG.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 1 ) GG UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 5 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 AND kom_iskazzemljista.prioritet = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idiskazzemljista, br_vlas, br_parc FROM ( SELECT idiskazzemljista, count(*) AS br_vlas FROM ( SELECT DISTINCT idiskazzemljista, idVlasnika FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B2 LEFT OUTER JOIN ( SELECT idiskazzemljista, count(*) AS br_parc FROM ( SELECT DISTINCT idiskazzemljista, idparcele FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B3 ON B2.idiskazzemljista = B3.idiskazzemljista ) AS C2 ON C1.idiskaza = C2.idiskazzemljista ORDER BY slucaj, udeo, idiskazzemljista)"

        Else
            stsql_ = "INSERT INTO " & izlaznaTabela & " ( idiskazzemljista, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc )(SELECT C1.idIskaza, udeo, indikacije, idvlasnika, mesto, sifralica, slucaj, br_vlas, br_parc FROM ( SELECT * FROM ( SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = '" & My.Settings.pozivanje_MaticnoNaselje & "' UNION SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT kom_iskazzemljista.idIskaza, zeljeucesnika, vezaka, Udeo, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ', ', ifnull(MATBRGRA, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, kom_vlasnik.idVlasnika, mesto, SIFRALICA FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika LEFT OUTER JOIN kom_iskazzemljista ON kom_vezaparcelavlasnik.idiskazzemljista = kom_iskazzemljista.idIskaza WHERE kom_vezaparcelavlasnik.obrisan = 0 AND kom_vlasnik.obrisan = 0 AND kom_iskazzemljista.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> '" & My.Settings.pozivanje_MaticnoNaselje & "' ) AS B ) AS C1 LEFT OUTER JOIN ( SELECT B2.idiskazzemljista, br_vlas, br_parc FROM ( SELECT idiskazzemljista, count(*) AS br_vlas FROM ( SELECT DISTINCT idiskazzemljista, idVlasnika FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B2 LEFT OUTER JOIN ( SELECT idiskazzemljista, count(*) AS br_parc FROM ( SELECT DISTINCT idiskazzemljista, idparcele FROM kom_vezaparcelavlasnik ) AS B1 GROUP BY idiskazzemljista ) AS B3 ON B2.idiskazzemljista = B3.idiskazzemljista ) AS C2 ON C1.idiskaza = C2.idiskazzemljista ORDER BY slucaj, udeo, idiskazzemljista)"
            'nema prioriteta
        End If


        Try
            'comm_.CommandText = "CREATE TABLE zaPozivanje SELECT C1.*, br_vlas, br_parc, (" & My.Settings.pozivanje_nultoVreme & " + br_vlas * " & My.Settings.pozivanje_vremePosedovni & " + br_parc * " & My.Settings.pozivanje_vremeBrojParcela & ") AS min_,  0 as prosao_, 0 as rednibroj  FROM ( SELECT * FROM ( SELECT A.*, 1 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS indikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) = 'RADUJEVAC' UNION SELECT A.*, 2 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica = 2000 OR sifralica = 2001 ) AND UCASE(mesto) <> 'RADUJEVAC' UNION SELECT A.*, 3 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) = 'RADUJEVAC' UNION SELECT A.*, 4 AS slucaj FROM ( SELECT DISTINCT fs_raspravnizapisnik.idPL, zeljeucesnika, vezaka, brojpredmeta, concat( ifnull(PREZIME, ''), ' ', ifnull(IMEOCA, ''), ' ', ifnull(IME, ''), ' ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ, '')) AS idnikacije, fs_vlasnik.idVlasnika, mesto, SIFRALICA FROM fs_vezaparcelavlasnik LEFT OUTER JOIN fs_vlasnik ON fs_vezaparcelavlasnik.idVlasnika = fs_vlasnik.idVlasnika LEFT OUTER JOIN fs_raspravnizapisnik ON fs_vezaparcelavlasnik.idPL = fs_raspravnizapisnik.idPL WHERE fs_vezaparcelavlasnik.obrisan = 0 AND fs_vlasnik.obrisan = 0 AND fs_raspravnizapisnik.obrisan = 0 ) AS A WHERE ( sifralica <> 2000 AND sifralica <> 2001 ) AND UCASE(mesto) <> 'RADUJEVAC' ) AS B WHERE zeljeucesnika IS NULL AND brojpredmeta = '' ) AS C1 LEFT OUTER JOIN ( SELECT B2.idPL, br_vlas, br_parc FROM ( SELECT idPL, count(*) AS br_vlas FROM ( SELECT DISTINCT idpl, idVlasnika FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B2 LEFT OUTER JOIN ( SELECT idPL, count(*) AS br_parc FROM ( SELECT DISTINCT idpl, idparcele FROM fs_vezaparcelavlasnik ) AS B1 GROUP BY idpl ) AS B3 ON B2.idPL = B3.idPL ) AS C2 ON C1.idpl = C2.idPL ORDER BY idpl"
            comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        Catch ex As Exception
            comm_.CommandText = "drop table " & izlaznaTabela : comm_.ExecuteNonQuery() : comm_.CommandText = stsql_ : comm_.ExecuteNonQuery()
        End Try

        'sada treba izdvojiti one koji su samo u gradevinkom i dajes im slucaj 5 a ovaj slucaj ne tretiras!

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            comm_.CommandText = "UPDATE " & izlaznaTabela & " SET slucaj = 5 WHERE idiskazzemljista IN ( SELECT KL.idiskazzemljista FROM (( SELECT idiskazzemljista FROM ( SELECT idiskazzemljista, ukomasaciji FROM ( SELECT A.*, B.idiskazzemljista FROM ( SELECT brparcelef, ukomasaciji, idParc FROM kom_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idiskazzemljista FROM kom_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idiskazzemljista ORDER BY idiskazzemljista ) AS GG GROUP BY idiskazzemljista HAVING count(*) = 1 ) AS KL INNER JOIN ( SELECT idiskazzemljista, ukomasaciji FROM ( SELECT A.*, B.idiskazzemljista FROM ( SELECT brparcelef, ukomasaciji, idParc FROM kom_parcele WHERE DEOPARCELE = 0 AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idparcele, idiskazzemljista FROM kom_vezaparcelavlasnik WHERE obrisan = 0 ) AS B ON A.idparc = B.idparcele ) AS GG GROUP BY ukomasaciji, idiskazzemljista ORDER BY idiskazzemljista ) AS MM ON KL.idiskazzemljista = MM.idiskazzemljista ) WHERE ukomasaciji = 0 )"
            comm_.ExecuteNonQuery() : comm_.CommandText = "delete from " & izlaznaTabela & " where slucaj=5" : comm_.ExecuteNonQuery()
        End If


        'ovo podeliti u dva dela - pa onda nema veze sto se tice prioriteta - njih postaviti na 1

        comm_.CommandText = "update " & izlaznaTabela & " set rednibroj=0" : comm_.ExecuteNonQuery()

        comm_.CommandText = "Select distinct idvlasnika from " & izlaznaTabela
        Dim ds_ As New DataTable : Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter : adapter_.SelectCommand = comm_ : adapter_.Fill(ds_)

        'fali mi polje rednibroj ! 
        Dim rednibroj As Integer = 1 : pb1.Value = 0 : pb1.Maximum = ds_.Rows.Count

        'sada idemo pocetak dokle i kraj 
        'treba pre toga videti sta je sa smenama jer onda imas dva cekinga!

        'kako sam to resio: ako je druga smena pocetak i kraj 

        Dim jednaSmena As Boolean = False
        'Dim g_, pp_
        Dim datumPocetka_ As DateTime

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then jednaSmena = True Else jednaSmena = False

        'ovde ima sate i minute!

        Dim g_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim pp_ = My.Settings.pozivanje_pocetakDatum.Split("/")
        If g_.Length = 1 Then datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), 0, 0) Else datumPocetka_ = New Date(pp_(2), pp_(1), pp_(0), g_(0), g_(1), 0)


        Dim prethodnoVreme As Double = 0 : Dim trenutnoVreme As Double = 0 : Dim smena_ As Integer = 1

        For i = 0 To ds_.Rows.Count - 1
            'sada ide selekcija za svakog pojedinacno pa idemo da biramo sve posedovne 


            'sada mozes da proveris sta se desava dalje koliko imas cega - odnosno koliko ti vremena treba! - treba ti broj listova i treba ti 
            'datum kada pocinje i vreme da ga smestis odmah

            comm_.CommandText = "SELECT " & My.Settings.pozivanje_nultoVreme & "+ br_pl*" & My.Settings.pozivanje_vremePosedovni & " + br_parc*" & My.Settings.pozivanje_vremeBrojParcela & " + br_vlas*" & My.Settings.pozivanje_vremeVlasnik & " FROM ( SELECT count(*) AS br_pl FROM ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " ) AS E1 ) AS E2, ( SELECT count(*) AS br_parc FROM ( SELECT DISTINCT idparcele FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E3 ) AS E4, ( SELECT count(*) AS br_vlas FROM ( SELECT DISTINCT idVlasnika FROM fs_vezaparcelavlasnik WHERE obrisan = 0 AND idpl IN ( SELECT DISTINCT idPL FROM fs_vezaparcelavlasnik WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " )) AS E5 ) AS E6"
            'sada imas vreme u minutima koliko ce da traje!
            Dim read_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)
            read_.Read() : trenutnoVreme = read_.GetValue(0) : read_.Close()

            Try
                conn_.Open()
            Catch ex As Exception
            End Try

            'ogranicenje po osobi primena:
            If trenutnoVreme > Val(My.Settings.ogranicenje_poStranci) Then
                trenutnoVreme = Val(My.Settings.ogranicenje_poStranci)
            End If

            comm_.CommandText = "UPDATE " & izlaznaTabela & " SET " & izlaznaTabela & ".rednibroj = " & rednibroj & ", " & izlaznaTabela & ".minuti_ = " & trenutnoVreme & " WHERE idiskazzemljista IN ( SELECT idiskazzemljista FROM ( SELECT * FROM " & izlaznaTabela & " ) aH WHERE idVlasnika = " & ds_.Rows(i).Item(0).ToString & " AND redniBroj = 0 )"
            Dim dalipostoji As Integer = comm_.ExecuteNonQuery()

            'proveris da li je vlasnik vec pozivan ako jeste ides na sledeci ne ulazis u racunicu

            If dalipostoji > 0 Then

                'sada mozes na datum!

                'sada idemo da kreiramo novi datum i dodamo ovo vreme

                datumPocetka_ = datumPocetka_.AddMinutes(trenutnoVreme)


                If jednaSmena = True Then
                    'sada treba videti kako da se zatvori na kraj poslednje radno vreme?!
                    Dim bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") : Dim dKraj As DateTime

                    'datum pozivanja, vreme pozivanja

                    If bg_.Length = 1 Then dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)

                    comm_.CommandText = "update " & izlaznaTabela & " set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()
                    'comm_.CommandText = "update zapozivanje set datum_='" & datumPocetka_.ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.ToString("HH:mm") & "' where rednibroj=" & rednibroj : comm_.ExecuteNonQuery()

                    If p_ > 0 Then
                        'sada resetujes datum koji poredis pre ovoga moras da proveris da li je u pitanju 

                        'sta se sada desava sa datumom!
                        datumPocetka_ = datumPocetka_.AddDays(1)
                        'sada treba proveriti da li je subora iuli nedelja

                        'ako je subota
                        If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                        'ako je nedelja
                        If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                        'ides na ponedeljak

                        'treba proveriti samo dali je praznik@!

                        'sada kada imas resetujes vreme!
                        Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                        If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)

                        datumPocetka_ = gtime_

                    Else
                        'ovde proveriti dali je razlika 

                    End If

                Else
                    'sada prvo vidis da li je ovaj datum veci od kraja prve smene -  ako nije onda mu dodas, ako jeste saltas ga na 4 
                    Dim dKraj, Spocetak As DateTime : Dim bg_

                    If smena_ = 1 Then bg_ = My.Settings.pozivanje_smena1Kraj.Split(":") Else bg_ = My.Settings.pozivanje_smena2Kraj.Split(":")

                    If bg_.Length = 1 Then
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0)
                    Else
                        dKraj = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)
                    End If


                    dKraj = dKraj.AddMinutes(-20)
                    Dim p_ = DateTime.Compare(datumPocetka_, dKraj)


                    comm_.CommandText = "update " & izlaznaTabela & " set datum_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("dd/MM/yyyy") & "', vreme_='" & datumPocetka_.AddMinutes(-trenutnoVreme).ToString("HH:mm") & "' where rednibroj=" & rednibroj    'sada je zanimljivo pitanje sta pravis update!?"
                    Dim uradioUpdate As Integer = comm_.ExecuteNonQuery()


                    If p_ >= 0 Then
                        'dosao si na promenu smene - sto znaci sta! - menjas smenu u 2
                        bg_ = My.Settings.pozivanje_smena2Pocetak.Split(":")
                        If bg_.Length = 1 Then Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), 0, 0) Else Spocetak = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bg_(0), bg_(1), 0)

                        Dim q_ = DateTime.Compare(datumPocetka_, Spocetak)

                        'ako je q_ manje od  1 znaci da treba saltati na drugu smenu!
                        If q_ <= 0 Then
                            'saltas  na pocetak na drugu smenu!'sada postavljas na pocetak ono drugo!
                            If smena_ = 1 Then
                                datumPocetka_ = Spocetak
                                smena_ = 2
                            End If

                            'sda postavis na nesto sto mozes 
                        Else
                            'sada prelazis na sledeci nivo!

                            datumPocetka_ = datumPocetka_.AddDays(1)
                            'sada treba proveriti da li je subora iuli nedelja

                            'ako je subota
                            If datumPocetka_.DayOfWeek.ToString = "Saturday" Then datumPocetka_ = datumPocetka_.AddDays(2)
                            'ako je nedelja
                            If datumPocetka_.DayOfWeek.ToString = "Sunday" Then datumPocetka_ = datumPocetka_.AddDays(1)
                            'ides na ponedeljak

                            'treba proveriti samo dali je praznik@!

                            'sada kada imas resetujes vreme!
                            Dim bb_ = My.Settings.pozivanje_smena1Pocetak.Split(":") : Dim gtime_ As DateTime

                            If bb_.Length = 1 Then gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), 0, 0) Else gtime_ = New Date(datumPocetka_.Year, datumPocetka_.Month, datumPocetka_.Day, bb_(0), bb_(1), 0)
                            datumPocetka_ = gtime_
                            smena_ = 1

                        End If

                        'sada je fora! ako je 
                    End If

                End If


            End If
            pb1.Value = i : rednibroj += 1

        Next

        stsql_ = "update kom_parametri set opisText='Formula: " & My.Settings.pozivanje_nultoVreme & " + brojposedovnihListva*" & My.Settings.pozivanje_vremePosedovni & " + brojVlasnika*" & My.Settings.pozivanje_vremeVlasnik & " + brojParcela*" & My.Settings.pozivanje_vremeBrojParcela &
            ". Datum pocetka: " & My.Settings.pozivanje_pocetakDatum & " . Komisija pocinje sa radom " & My.Settings.pozivanje_smena1Pocetak

        If My.Settings.pozivanje_smena2Pocetak = My.Settings.pozivanje_smena2Kraj Then
            'imas samo jedan
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena1Kraj
        Else
            stsql_ = stsql_ & " , a zavrsava sa radom " & My.Settings.pozivanje_smena2Kraj & ", sa pauzom od " & My.Settings.pozivanje_smena1Kraj & " do " & My.Settings.pozivanje_smena2Pocetak
        End If

        If My.Settings.pozivanje_kriterijum_izbaciGradevinski = 1 Then
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu su iskljuceni iz plana pozivanja."
        Else
            stsql_ = stsql_ & ". Vlasnici (odnosno list nepokretnosti) koji imaju parcele samo u gradevinskom rejonu nisu iskljuceni iz plana pozivanja."
        End If

        If My.Settings.pozivanje_kriterijum_zeljeUcesnika = 1 Then
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali nisu ukljuceno u plan pozivanja."
        Else
            stsql_ = stsql_ & " Listovi nepokretnosti ciji su vlasnici delimicno ili u celosti pristustvovali jesu ukljuceno u plan pozivanja."
        End If

        stsql_ = stsql_ & "' where opis='Pozivanje'"
        comm_.CommandText = stsql_
        comm_.ExecuteNonQuery()

        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        pb1.Value = 0

        If My.Settings.pozivanje_stampamOdmah = 1 Then
            StampajPoziveFaktickoStanjeSvi()
        End If


        'sada ovo mozes da ucitas pa da idemo dalje!
        MsgBox("Kraj")

    End Sub

    Private Sub StampaToolStripMenuItem2_Click(sender As Object, e As System.EventArgs) Handles StampaToolStripMenuItem2.Click

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Try
            conn_.Open()
        Catch ex As Exception
            MsgBox("Nemoguce uspostaviti vezu sa bazom. Proverite.")
            Exit Sub
        End Try

        Dim odakle_ As Integer = InputBox("Od koje table krecem?", "", "1")
        Dim brTable As Integer = 0 'InputBox("Unesite broj table za generisanje spiska", "Unos podataka", 1)


        'sada idemo da selektujemo sve table koje se nalaze u kom_tablenadela
        Dim dSet_ As New DataSet
        'sada za ovo bi trebalo nesto uraditi 

        Dim myadapter_ As MySql.Data.MySqlClient.MySqlDataAdapter

        comm_.CommandText = "select DISTINCT idtable from kom_tablenadela where idtable>= " & odakle_ & " order by idtable"
        myadapter_ = New MySqlDataAdapter(comm_)
        myadapter_.Fill(dSet_, "spisakTabli")

        'sada bi trebalo da imas spisak tabli

        pb1.Value = 0 : pb1.Maximum = dSet_.Tables("spisakTabli").Rows.Count

        Dim folderPath_ As String = ""
        fbd_diag.ShowDialog()
        folderPath_ = fbd_diag.SelectedPath.ToString

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application

        pb1.Maximum = dSet_.Tables("spisakTabli").Rows.Count
        pb1.Value = 0

        For i = 0 To dSet_.Tables("spisakTabli").Rows.Count - 1
            'sada idemo upit za svaki broj table
            comm_.CommandText = "SELECT C.idiskazzemljista, D.indikacije, D.mesto, D.adresa, C.redniBrojNadele FROM ( SELECT B.idiskazzemljista, B.idvlasnika, A.redniBrojNadele FROM ( SELECT idIskazZemljista, redniBrojNadele FROM kom_tablenadela WHERE idtable = " & dSet_.Tables("spisakTabli").Rows(i).Item(0).ToString & " AND idko = " & My.Settings.pozivanje_idko & " ORDER BY redniBrojNadele ) AS A LEFT OUTER JOIN ( SELECT DISTINCT idvlasnika, idiskazzemljista FROM kom_vezaparcelavlasnik WHERE idko = " & My.Settings.pozivanje_idko & " AND obrisan = 0 ) AS B ON A.idIskazZemljista = B.idiskazzemljista ) AS C LEFT OUTER JOIN ( SELECT idvlasnika, concat( ifnull(prezime, ''), ' ', ifnull(imeoca, ''), ' ', ifnull(ime, '')) AS indikacije, ifnull(mesto, '') AS mesto, concat( ifnull(ulica, ''), ' ', ifnull(broj, ''), ' ', ifnull(uzbroj, '')) AS adresa FROM kom_vlasnik ) AS D ON C.idvlasnika = D.idvlasnika"
            'Dim myreader_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
            Dim dtable_ As New DataTable
            myadapter_.Fill(dtable_)

            docApp_.Visible = True


            For j = 0 To dtable_.Rows.Count - 1
                'sad ides jedan po jedan pa polako :)

                Dim wDoc_ As Microsoft.Office.Interop.Word.Document = docApp_.Documents.Open(My.Settings.pozivanje_wordFileTemplatePath)
                'otvoris word kao template

                wDoc_.SaveAs(folderPath_ & "\tabla_" & dSet_.Tables("spisakTabli").Rows(i).Item(0).ToString & "_" & dtable_.Rows(j).Item(4).ToString & "_" & dtable_.Rows(j).Item(0).ToString & "_" & dtable_.Rows(j).Item(1).ToString & ".doc")
                'sada mozes dalje!
                'idemo prvo na identifikaciju
                Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

                Try
                    bokMarks_.Item("indikacije_dole").Range.Text = dtable_.Rows(j).Item(1).ToString
                Catch ex As Exception

                End Try


                Try
                    bokMarks_.Item("indikacije_gore").Range.Text = dtable_.Rows(j).Item(1).ToString
                Catch ex As Exception

                End Try


                Try
                    bokMarks_.Item("mesto").Range.Text = dtable_.Rows(j).Item(2).ToString
                Catch ex As Exception

                End Try


                Try
                    bokMarks_.Item("ulica").Range.Text = dtable_.Rows(j).Item(3).ToString
                Catch ex As Exception

                End Try

                Try
                    bokMarks_.Item("brojtable").Range.Text = dSet_.Tables("spisakTabli").Rows(i).Item(0).ToString
                Catch ex As Exception

                End Try

                wDoc_.Save() : wDoc_.Close() : wDoc_ = Nothing

            Next

            pb1.Value = i

            dtable_ = Nothing
        Next


        pb1.Value = 0
        myadapter_ = Nothing : conn_.Close() : comm_ = Nothing : conn_ = Nothing : docApp_ = Nothing
        MsgBox("Kraj")
    End Sub
    Private Sub TableVrednostCSVToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles TableVrednostCSVToolStripMenuItem.Click

        'ides u petlji po fileovima

        Dim brOpstine As Integer = InputBox("Unesite idko, za feketic je 801461, a za lovcenac ", "Unos podataka", 801500)

        If brOpstine = 0 Then Exit Sub

        fbd_diag.SelectedPath = ""
        fbd_diag.ShowDialog()

        Me.Cursor = Cursors.WaitCursor

        sf_diag.FileName = ""
        sf_diag.Filter = "Text Files (*.txt)|*.txt"
        sf_diag.ShowDialog()

        If sf_diag.FileName = "" Then sf_diag.FileName = Path.GetTempPath() & "\csvKontrola.txt"
        Dim fileWrite_ As New StreamWriter(sf_diag.FileName)

        lbl_infoMain.Text = "Ucitavam podatke iz baze"
        My.Application.DoEvents()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "SELECT oznakaTable,VSuma from kom_kfmns where obrisan=0 and idko=" & brOpstine
        Dim dt_ As New DataTable

        Dim myadap_ As MySql.Data.MySqlClient.MySqlDataAdapter = New MySqlDataAdapter(comm_)
        myadap_.Fill(dt_)

        lbl_infoMain.Text = "Ucitavam podatke iz CSV-ova"
        My.Application.DoEvents()

        'Dim f_ As System.IO.File
        fileWrite_.WriteLine("brTable; nadeljenoCSV; kfmns")
        For Each f_ In System.IO.Directory.GetFiles(fbd_diag.SelectedPath, "*.csv")
            'sada za svaki file idemo redom ucitavanje
            Dim fileReader_ As New StreamReader(f_)
            Dim nadeljeno_ As Double = 0
            Do While fileReader_.Peek <> -1
                Dim a_ = fileReader_.ReadLine.Split(",")
                Try
                    nadeljeno_ += Val(a_(1))
                Catch ex As Exception
                End Try
            Loop
            'sada iz tabele treba naci ono sto ti treba a to je tabla!
            Dim p_ = Replace(Path.GetFileName(f_), ".csv", "")
            p_ = Replace(p_, "Tabla_", "T_")

            Dim izbaze_ As String = ""
            For g = 0 To dt_.Rows.Count - 1
                If dt_.Rows(g).Item(0).ToString = p_ Then
                    izbaze_ = dt_.Rows(g).Item(1).ToString
                    Exit For
                End If
            Next
            fileWrite_.WriteLine(Path.GetFileName(f_) & ";" & nadeljeno_ & ";" & izbaze_)
        Next

        'sada nesto uraditi sa ovim? ali sta 

        lbl_infoMain.Text = "Pisem podatke u file"
        My.Application.DoEvents()

        fileWrite_.Close()
        lbl_infoMain.Text = ""
        Me.Cursor = Cursors.Default
        MsgBox("Kraj")
    End Sub

    Private Sub ZaokruziGeomNa2DecLINIJEToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ZaokruziGeomNa2DecLINIJEToolStripMenuItem.Click
        'sada idemo ovo deluje kao mnogo elegantnije resenje nego sto je bilo ranije

        'gleda layer koji je definisan kao         My.Settings.layerName_ParceleNadela
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If MsgBox("Radim zaokruzivanje na layer-u: " & My.Settings.layerName_ParceleNadela & " ako zelite neki drugi layer promenite parametar: layerName_ParceleNadela", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then

            Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)

            'bilo bi dobro da napravi kopiju ovoga! ali u koji document
            Dim drwNew_ As Manifold.Interop.Drawing = doc.NewDrawing(My.Settings.layerName_ParceleNadela & "_2dec", drw_.CoordinateSystem)
            drw_.Copy()
            drwNew_.Paste()

            'sada mozes dalje

            Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("udff")
            qvr_.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[Geom (I)] as geom_,newArea_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToLine( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID],1 as broj_  FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[ID]=AA.id ) set geom_=newArea_"
            qvr_.RunEx(True)
            doc.ComponentSet.Remove("udff")
            doc.Save()

            MsgBox("Kraj")
        End If

        doc = Nothing
    End Sub

    Private Sub ZaokruziGeomNa2DecQueryPOINTToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ZaokruziGeomNa2DecQueryPOINTToolStripMenuItem.Click
        'sada idemo ovo deluje kao mnogo elegantnije resenje nego sto je bilo ranije

        'gleda layer koji je definisan kao         My.Settings.layerName_ParceleNadela
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If MsgBox("Radim zaokruzivanje na layer-u: " & My.Settings.layerName_ParceleNadela & " ako zelite neki drugi layer promenite parametar: layerName_ParceleNadela", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then

            Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)

            'bilo bi dobro da napravi kopiju ovoga! ali u koji document
            Dim drwNew_ As Manifold.Interop.Drawing = doc.NewDrawing(My.Settings.layerName_ParceleNadela & "_2dec", drw_.CoordinateSystem)
            drw_.Copy()
            drwNew_.Paste()

            'sada mozes dalje

            Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("udff")
            qvr_.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[Geom (I)] as geom_,newArea_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToPoint( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID],1 as broj_  FROM [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & (My.Settings.layerName_ParceleNadela & "_2dec") & "].[ID]=AA.id ) set geom_=newArea_"
            qvr_.RunEx(True)
            doc.ComponentSet.Remove("udff")
            doc.Save()

            MsgBox("Kraj")
        End If

        doc = Nothing
    End Sub
    Public Function getNumeric(value As String) As Object()
        Dim broj_ As StringBuilder = New StringBuilder
        Dim slovo_ As StringBuilder = New StringBuilder
        For i = 0 To value.Length - 1
            If IsNumeric(value(i)) Then
                broj_.Append(value(i))
            Else
                slovo_.Append(value(i))
            End If
        Next

        Dim a(1)
        a(0) = broj_.ToString : a(1) = slovo_.ToString
        Return a
    End Function

    Private Sub toolbarManuClearAll_Click(sender As Object, e As System.EventArgs) Handles toolbarManuClearAll.Click
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        Me.Cursor = Cursors.WaitCursor
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("brisi")
        pb1.Maximum = doc.ComponentSet.Count
        For i = 0 To doc.ComponentSet.Count - 1
            If doc.ComponentSet.Item(i).TypeName = "Drawing" Then
                qvr_.Text = "update [" & doc.ComponentSet.Item(i).Name & "] set [Selection (I)]=false"
                qvr_.RunEx(True)
            End If
            pb1.Value = i
        Next
        pb1.Value = 0
        ManifoldCtrl.Refresh()
        Me.Cursor = Cursors.Default

        doc = Nothing
    End Sub

    Private Sub UcitajSelektovaniToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles UcitajSelektovaniToolStripMenuItem.Click
        'sada treba napraviti zoom na objekat!
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        'uvek je u prvoj koloni ID
        Me.Cursor = Cursors.WaitCursor

        Try
            'ako je jedan selektovan to je ok idemo na njega!
            Dim iRowIndex = dgv_ManifoldData.SelectedRows.Item(0).Cells(0).Value
            Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("pronadi")
            qvr_.Text = "update [" & dgv_ManifoldData.Tag & "] set [Selection (I)]=true where [ID]=" & dgv_ManifoldData.SelectedRows.Item(0).Cells(0).Value
            qvr_.RunEx(True)
            Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet(dgv_ManifoldData.Tag)
            Dim objSet As Manifold.Interop.ObjectSet = drw_.Selection
            ManifoldCtrl.ZoomTo(objSet)
            ManifoldCtrl.Refresh()
        Catch ex As Exception

        End Try

        Me.Cursor = Cursors.Default
        doc = Nothing
    End Sub

    Private Sub tvSelektion_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvSelektion.AfterSelect
        If InStr(e.Node.Text, "http") > 0 Then
            Process.Start(e.Node.Text)
        End If
    End Sub
    Private Sub PreparcelacijaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles PreparcelacijaToolStripMenuItem.Click
        'imas dato seldece:
        'drawing u kome su parcele
        'u drowingu obavezna polja: idTable, PB, gde je idTable - redni broj grupe, PB povrsina iz baze

        Me.Cursor = Cursors.WaitCursor
        Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim drwParcele, drwTacke As Manifold.Interop.Drawing

        Try
            drwParcele = doc_.ComponentSet(My.Settings.layerName_ParceleNadela)
        Catch ex As Exception
            'ako nema mozes da se slikas
            Exit Sub
        End Try

        Try
            drwTacke = doc_.ComponentSet(My.Settings.layerName_nadelaSmer)
        Catch ex As Exception
            'ako nema mozes da se slikas
            Exit Sub
        End Try

        'sada mozemo po relativno poznato algortimu kao sto je bilo sa onim H
        Dim qvrTableListing As Manifold.Interop.Query = doc_.NewQuery("qvrRedosled")

        qvrTableListing.Text = "select distinct idtable from [" & My.Settings.layerName_ParceleNadela & "] where idtable<>0 order by [idtable]" : qvrTableListing.RunEx(True)

        Dim matTabli(-1) As Integer : Dim brMatTabli As Integer = 0

        For i = 0 To qvrTableListing.Table.RecordSet.Count - 1
            ReDim Preserve matTabli(brMatTabli)
            matTabli(brMatTabli) = qvrTableListing.Table.RecordSet.Item(i).DataText(1)
            brMatTabli += 1
        Next

        For i = 0 To matTabli.Length - 1
            'prvi korak je da sracunas kako ces da ih podelis 

            Dim qvrSumaP_ As Manifold.Interop.Query = doc_.NewQuery("sumaP")
            qvrSumaP_.Text = "SELECT  B.[ID], [PB], round([Area (I)]) PG, (round([Area (I)]) - [PB]) as DP, Round(Sqr([PB])*0.0007*2880) DO, Round([PB]*(SELECT sum(round([Area (I)])-[PB])/ sum([pb]) FROM (SELECT top 1 [Geom (I)] pnt_ FROM [" & My.Settings.layerName_nadelaSmer & "] where [idTable]=" & matTabli(i) & ") A, [" & My.Settings.layerName_ParceleNadela & "] B WHERE B.[idTable]=" & matTabli(i) & " ORDER by Distance(CentroidWeight(B.[Geom (I)]),pnt_))+[PB]) as PBNew,[Brparcelef] FROM (SELECT top 1  [Geom (I)] pnt_ FROM [" & My.Settings.layerName_nadelaSmer & "] where [idTable]=" & matTabli(i) & ") A, [" & My.Settings.layerName_ParceleNadela & "] B WHERE B.[idTable]=" & matTabli(i) & " ORDER by Distance(CentroidWeight(B.[Geom (I)]),pnt_)"
            qvrSumaP_.RunEx(True)
            'formiras geometriju
            Dim drwNewTable As Manifold.Interop.Drawing

            Try
                drwNewTable = doc_.NewDrawing("tablaPR_" & matTabli(i), drwParcele.CoordinateSystem, True)
            Catch ex As Exception
                'znaci da postoji 
                doc_.ComponentSet.Remove("tablaPR_" & matTabli(i))
                drwNewTable = doc_.NewDrawing("tablaPR_" & matTabli(i), drwParcele.CoordinateSystem, True)
            End Try

            'sada sve copiras i njega
            Dim qvrCopy As Manifold.Interop.Query = doc_.NewQuery("kopirajTablu")

            Dim col_ As Manifold.Interop.Column = doc_.Application.NewColumnSet.NewColumn
            col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
            col_.Name = "idTable"
            drwNewTable.OwnedTable.ColumnSet.Add(col_)
            col_.Name = "OldID"
            drwNewTable.OwnedTable.ColumnSet.Add(col_)
            col_.Name = "brparcelef"
            col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
            drwNewTable.OwnedTable.ColumnSet.Add(col_)
            col_ = Nothing

            qvrCopy.Text = "insert into [" & drwNewTable.Name & "] ([idtable],[Geom (I)]) (select [idtable],[Geom (I)] FROM [" & My.Settings.layerName_ParceleNadela & "] where [idTable]=" & matTabli(i) & ")"
            qvrCopy.RunEx(True)

            doc_.ComponentSet.Remove("kopirajTablu")
            qvrCopy = Nothing
            'sada idemo da grupisemo parcele !!!!

            Dim anal_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer

            anal_.Union(drwNewTable, drwNewTable, drwNewTable.ObjectSet)
            'sada ti je ovo objedinjeno - odnosno imas jednu jedinu parcelu koju treba deliti
            ' doc_.Save()

            'sada cepkas dobijeno

            Dim qvrDist As Manifold.Interop.Query = doc_.NewQuery("dist")
            qvrDist.Text = "SELECT top 1 distance(A.[geom (I)],B.[Geom (I)]) as dist_,atn2(CentroidX(A.[Geom (I)])-CentroidX(B.[Geom (I)]),CentroidY(A.[Geom (I)])-CentroidY(B.[Geom (I)])) as ugao_,CentroidX(A.[Geom (I)]) as x1,CentroidY(A.[Geom (I)]) as y1,CentroidX(B.[Geom (I)]) as x2,CentroidY(B.[Geom (I)]) as y2 FROM ((SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & matTabli(i) & ") as A,(SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & matTabli(i) & ") as B) WHERE A.[ID]<>B.[ID]" : qvrDist.RunEx(True)
            Dim qvrLastID As Manifold.Interop.Query = doc_.NewQuery("lastID")


            If qvrSumaP_.Table.RecordSet.Count > 0 Then

                Dim sumazaDeob_ As Double = 0
                pb1.Maximum = qvrSumaP_.Table.RecordSet.Count + 1
                pb1.Value = 0
                'insertujes nultu liniju!
                Dim pnt1(1), pnt2(1) As Double
                pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
                pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                Dim du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
                podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -10, pnt1, pnt2, doc_, drwNewTable, 2)

                Dim matLinija() As Integer : ReDim Preserve matLinija(0)

                qvrLastID.Text = "select top 1 [ID] from [" & drwNewTable.Name & "] order by [ID] Desc" : qvrLastID.RunEx(True)

                matLinija(0) = qvrLastID.Table.RecordSet(0).DataText(1) : doc_.Save()

                'PrintLine(freefile_, "Poceo sa obradom table u " & Now())

                For j = 0 To qvrSumaP_.Table.RecordSet.Count - 2  'ovde zamnei sa necim na ulazu!
                    pb1.Value = j
                    Dim kraj As Boolean = False
                    'treba ti rastojanje izmedu dve tacke
                    sumazaDeob_ += qvrSumaP_.Table.RecordSet.Item(j).DataText(6)
                    Dim h_ As Double = H_racunanjeDirektno(qvrDist.Table.RecordSet(0).DataText(1), qvrDist.Table.RecordSet(0).DataText(1), qvrSumaP_.Table.RecordSet.Item(j).DataText(6), sumazaDeob_)
                    'sada treba videti dali je to ok!
                    Dim interacija_ As Integer = -1
                    'pb2.Maximum = My.Settings.nadela_brInteracija

                    Do While Not kraj = True
                        interacija_ += 1

                        Dim drwTemp As Manifold.Interop.Drawing = doc_.NewDrawing("temp", drwNewTable.CoordinateSystem, True)

                        col_ = doc_.Application.NewColumnSet.NewColumn : col_.Name = "idTable" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                        col_.Name = "OldID" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                        col_.Name = "brpracelef" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : drwTemp.OwnedTable.ColumnSet.Add(col_)
                        col_ = Nothing

                        Dim q_ As Manifold.Interop.Query = doc_.NewQuery("dfas")
                        q_.Text = "insert into [temp] (idtable, [geom (i)]) (select idtable,[geom (i)] from [" & drwNewTable.Name & "])" : q_.RunEx(True) : doc_.ComponentSet.Remove("dfas")

                        Dim drwLine As Manifold.Interop.Drawing = doc_.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                        'Dim pnt1(1), pnt2(1) As Double
                        pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4) : pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                        du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1)) : podeliParceluViseDelova_KreirajPresecnuLiniju(du - 90, qvrDist.Table.RecordSet(0).DataText(1), h_, pnt1, pnt2, doc_, drwLine)
                        Dim lineID As Integer = drwLine.ObjectSet.Item(0).ID
                        'sada imas liniju i ide presek

                        anal_.Split(drwTemp, drwTemp, drwTemp.ObjectSet, drwLine.ObjectSet)
                        'doc_.Save()

                        'problem kako da selektujes poligone koji su nastali naknadno? izmedu linije i dve tacke?
                        podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -h_, pnt1, pnt2, doc_, drwLine, 2)
                        'doc_.Save()
                        'sada ti treba convechull da napravi poligon
                        Dim qvrDobijenaPov As Manifold.Interop.Query = doc_.NewQuery("koliko")
                        qvrDobijenaPov.Text = "insert into [linije] ([Geom (I)]) VALUES (SELECT ConvexHull(AllCoords([Geom (I)])) FROM [Linije])" : qvrDobijenaPov.RunEx(True)
                        'doc.Save()
                        'doc.Save()
                        qvrDobijenaPov.Text = "SELECT sum([temp].[Area (I)]) FROM [Temp],[Linije] WHERE IsArea([Temp].[ID]) and  Contains([Linije].[ID],[Temp].[ID])" : qvrDobijenaPov.RunEx(True)
                        'SADA treba skratiti celu pricu!
                        'doc.Save()
                        'ovde ide print

                        If interacija_ > My.Settings.nadela_brInteracija Then
                            MsgBox("Nesto nije u redu!? broj interacija je veci od onoga sto treba-prepostavljam da je u pitanju tabla koja se za malo razlikuje od onoga sto treba")
                            'pronasao! izlazim iz ovoga kreiras liniju u temp3
                            drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                            qvrLastID.RunEx(True)
                            ReDim Preserve matLinija(j + 1)
                            matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                            doc_.ComponentSet.Remove("temp") : doc_.ComponentSet.Remove("linije")
                            kraj = True
                            Exit Do
                        End If

                        lbl_infoMain.Text = "Deoba za iskaz " & qvrSumaP_.Table.RecordSet.Item(j).DataText(6) & " razlika> " & (Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - sumazaDeob_, 2))
                        My.Application.DoEvents()

                        Dim nesto_ As Double = Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))

                        lbl_infoMain.Text = "Tabla " & matTabli(i) & ", Parcela:" & qvrSumaP_.Table.RecordSet.Item(j).DataText(6) & "; Razlika: " & (Math.Round(nesto_, 2) - Math.Round(sumazaDeob_, 2)) : My.Application.DoEvents()

                        If Math.Round(nesto_, 2) = Math.Round(sumazaDeob_, 2) Then
                            'pronasao! izlazim iz ovoga kreiras liniju u temp3
                            drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                            qvrLastID.RunEx(True)
                            ReDim Preserve matLinija(j + 1)
                            matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                            doc_.ComponentSet.Remove("temp") : doc_.ComponentSet.Remove("linije")
                            kraj = True
                        Else
                            'idemo iz pocetka
                            h_ = h_ + ((sumazaDeob_ - Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))) / qvrDist.Table.RecordSet(0).DataText(1))
                            doc_.ComponentSet.Remove("temp") : doc_.ComponentSet.Remove("linije")
                            drwLine = Nothing : drwTemp = Nothing
                            'PrintLine(freefile_, "kraj interacije " & Now())
                            'interacija_ += 1
                        End If
                        doc_.ComponentSet.Remove("koliko")
                        qvrDobijenaPov = Nothing
                    Loop
                Next

                'PrintLine(freefile_, "Kraj " & Now())
                pb1.Value = 0 'pb2.Value = 0

                '// OVDE UBACIO SVE STO TREBA IZ PRETHODNOG

                'qvrDist.Text = "update [" & drwNewTable.Name & "] set [OldID]=[ID]" : qvrLastID.RunEx(True)
                'doc.Save()

                anal_.Split(drwNewTable, drwNewTable, drwNewTable.ObjectSet, drwNewTable.ObjectSet)

                For j = 0 To qvrSumaP_.Table.RecordSet.Count - 1
                    qvrDist.Text = "update [" & drwNewTable.Name & "] set [brparcelef]=" & Chr(34) & qvrSumaP_.Table.RecordSet.Item(j).DataText(7) & Chr(34) & " where [ID]=(select min([ID]) from [" & drwNewTable.Name & "] where IsArea([ID]) and [brparcelef]=" & Chr(34) & Chr(34) & ")" : qvrDist.RunEx(True)
                Next

                'qvrDist.Text = "delete from [" & drwNewTable.Name & "] where isline([id])" : qvrDist.RunEx(True)
                qvrDist.Text = "update [" & drwNewTable.Name & "] set [idtable]=" & matTabli(i) : qvrDist.RunEx(True)
                qvrDist.Text = "delete from [" & drwParcele.Name & "] where [idtable]=" & matTabli(i) : qvrDist.RunEx(True)
                qvrDist.Text = "insert into [" & drwParcele.Name & "] ([Brparcelef],[Geom (I)],[idtable]) select [Brparcelef],[Geom (I)],[idtable] FROM [" & drwNewTable.Name & "] where isarea([id])" : qvrDist.RunEx(True)

                'sada iz parcele brises ovu tablu 
                doc_.ComponentSet.Remove("Dist")
                doc_.ComponentSet.Remove("LastID")

                doc_.Save()


            Else
                'nema nista pa bude prolem

            End If

        Next
        lbl_infoMain.Text = ""
        Me.Cursor = Cursors.Default
        MsgBox("Kraj")
    End Sub
    Private Sub StampanjeResenjaPosleNadeleWordAllToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles StampanjeResenjaPosleNadeleWordAllToolStripMenuItem.Click

        If My.Settings.resenja_wordFileTemplatePath = "" Then MsgBox("Morate definisati template word dokument") : Exit Sub

        Dim brResenja As Integer = InputBox("Unesi broj resenja od kojeg generisem.", "Unos podataka", "1")

        Dim freefile_ As Integer = FreeFile()
        FileOpen(freefile_, Path.GetTempPath() & "\resenjaKojaNemajuStaroStanje.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        Dim tbl_ As New DataTable
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        Dim mysqladap_ As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT DISTINCT idIskaz FROM kom_novostanjeparcela where idIskaz>=" & brResenja & " ORDER BY idIskaz", conn_)
        mysqladap_.Fill(tbl_)

        'sada bi trebalo uneti folder pa njega proslediti dalje
        Dim folderPath_ As String = ""
        fbd_diag.ShowDialog()
        folderPath_ = fbd_diag.SelectedPath.ToString

        pb1.Value = 0
        pb1.Maximum = tbl_.Rows.Count

        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application : docApp_.Visible = True
        Dim wDoc_ As Microsoft.Office.Interop.Word.Document

        For i = 0 To tbl_.Rows.Count - 1
            wDoc_ = docApp_.Documents.Open(My.Settings.resenja_wordFileTemplatePath)
            wDoc_.SaveAs((folderPath_ & "\" & tbl_.Rows(i).Item(0).ToString & ".doc"), 0)

            stampajResenje(tbl_.Rows(i).Item(0).ToString(), docApp_, wDoc_)
            wDoc_.Save()
            wDoc_.Close()
            pb1.Value = i
        Next

        MsgBox("Kraj ")

    End Sub

    Private Sub StampanjeResenjaPojedinacnoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles StampanjeResenjaPojedinacnoToolStripMenuItem.Click

        If My.Settings.resenja_wordFileTemplatePath = "" Then MsgBox("Morate definisati template pre pocetka stampanja.") : Exit Sub

        Dim brResenja As Integer = InputBox("Unesi broj resenja koje generisem.", "Unos podataka", "1")

        'sada ga negde treba sacuvati
        sf_diag.FileName = "" : sf_diag.Filter = "Word Files (.doc)|*.doc"
        sf_diag.Title = "Odredite file u kome se cuva resenje" : sf_diag.FileName = brResenja & ".doc" : sf_diag.DefaultExt = "*.doc" : sf_diag.ShowDialog() : If sf_diag.FileName = "" Then Exit Sub
        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application : docApp_.Visible = True
        Dim wDoc_ As Microsoft.Office.Interop.Word.Document

        Try
            wDoc_ = docApp_.Documents.Open(My.Settings.resenja_wordFileTemplatePath)
        Catch ex As Exception
            MsgBox(ex.Message)
            wDoc_ = Nothing
            docApp_ = Nothing
            Exit Sub
        End Try


        Dim fileName_ As String = sf_diag.FileName
        'If InStr(fileName_, ".doc") = 0 Then fileName_ = fileName_ & ".doc"

        Try
            wDoc_.SaveAs(fileName_)
        Catch ex As Exception
            'znac da je problem jer je otvrren
            MsgBox(" File je otvoren. Sacuvajte pod drugim imenom.", ex.Message)
            sf_diag.ShowDialog()
            wDoc_.SaveAsQuickStyleSet(fileName_)
        End Try

        stampajResenje(brResenja, docApp_, wDoc_)
        wDoc_.Save()
        wDoc_.Close()

        MsgBox("Kraj ")

    End Sub
    Private Sub KreiranjeIPunjenjeTabeleParcelaNovogStanjaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KreiranjeIPunjenjeTabeleParcelaNovogStanjaToolStripMenuItem.Click
        'proveris da li tabela postoji
        'sada idemo manifold!
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        If ManifoldCtrl.get_Document.Name = "" Then
            MsgBox("Ucitaj map file")
            doc = Nothing
            Exit Sub
        Else
            doc = ManifoldCtrl.get_Document
        End If
        'ako ne postoji kreiras ako postoji birses i krerias novi

        'ubaci da proverti da li postoji tabela iskaza jer tu pravi problem

        Dim procRazred, drwDKPNew As Manifold.Interop.Drawing

        Try
            procRazred = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Thema ne postoji u drawingu - resite problem i probajte ponovo")
            Exit Sub
        End Try

        Try
            drwDKPNew = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        Catch ex As Exception
            'znaci da nepostoji treba izaci i kazi
            MsgBox("Thema ne postoji u drawingu - resite problem i probajte ponovo")
            Exit Sub
        End Try

        lbl_infoMain.Text = "Provera i kreiranje tabele."
        My.Application.DoEvents()

        Me.Cursor = Cursors.WaitCursor

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        'proveris da li postoji tabela kom_novostanjeparela
        comm_.CommandText = "show tables like 'kom_novoStanjaParcela%'"
        conn_.Open()
        Dim connReader As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
        If connReader.HasRows = True Then
            'sada brises
            connReader.Close() : conn_.Open()
            comm_.CommandText = "drop table kom_novoStanjaParcela"
            comm_.ExecuteNonQuery()
        Else
            connReader.Close()
        End If


        Try
            conn_.Open()
        Catch ex As Exception
        End Try

        comm_.CommandText = "select opistext from kom_parametri where opis=" & Chr(34) & "koCode" & Chr(34)
        Dim matbr_ As Integer = -1
        connReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
        If connReader.HasRows = True Then
            connReader.Read()
            matbr_ = connReader.GetValue(0)
        Else
            MsgBox("Nemate podesen parametar za maticni broj KO. Upisite ga u sledeci prozor ili izadite iz funckije.")
            matbr_ = InputBox("Upisite maticni broj KO")
            If matbr_ = "0" Then
                Exit Sub
            End If
        End If
        connReader.Close()
        connReader = Nothing
        'sada kreiras ovu tabelu

        Try
            conn_.Open()
        Catch ex As Exception

        End Try


        Try
            comm_.CommandText = "CREATE TABLE `kom_novoStanjaParcela` ( `idKO` INT NOT NULL, `idKfmsns` INT NOT NULL, `idIskaza` INT NULL, `idTable` INT NULL, `idPotes` INT NULL, `idKulture` INT NULL, `brParcele` CHAR (10) NULL, `deoParcele` INT NULL DEFAULT 1, `rednibrnadele` INT NULL DEFAULT 1, `brPlana` CHAR (10) NULL, `prazred_1` INT NULL, `prazred_2` INT NULL, `prazred_3` INT NULL, `prazred_4` INT NULL, `prazred_5` INT NULL, `prazred_6` INT NULL, `prazred_7` INT NULL, `prazred_8` INT NULL, `prazred_neplodno` INT NULL, `ukupno_povrsina` INT NULL, `ukupno_vrednost` DOUBLE NULL, `idTeret` INT NULL, PRIMARY KEY (`idKfmsns`), FOREIGN KEY (`idIskaza`) REFERENCES `kom_iskazzemljista` (`idIskaza`))"
            comm_.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("Problem " & comm_.CommandText)
        End Try

        lbl_infoMain.Text = "Odredivanje vrednosti novih parcela."
        My.Application.DoEvents()

        'ovde kreiras polje koje je ID2 i kopiras ga pa po njemu posle pravis sve sto ti treba a to je ujedno i id za bazu

        Dim tbl_ As Manifold.Interop.Table = drwDKPNew.OwnedTable

        Dim postoji As Boolean = False

        For i = 0 To tbl_.ColumnSet.Count - 1
            If tbl_.ColumnSet.Item(i).Name = "idzaBazu" Then
                postoji = True
                Exit For
            End If
        Next

        If postoji = False Then
            Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
            col_.Name = "idzaBazu"
            col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
            tbl_.ColumnSet.Add(col_)
            col_ = Nothing
        End If


        tbl_ = Nothing

        Dim kfmns As Manifold.Interop.Query = doc.NewQuery("kfmsns")
        kfmns.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [idzaBazu]=[ID]"
        kfmns.RunEx(True)

        'sada mozes overlap radi dobijanja povrsine po procembenim razredima i vrednosti

        Try
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception

        End Try

        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology
        topPRazredi.Bind(procRazred)
        topPRazredi.Build()

        Dim topTable As Manifold.Interop.Topology = doc.Application.NewTopology
        topTable.Bind(drwDKPNew)
        topTable.Build()

        topTable.DoIntersect(topPRazredi, "table_pr_razred")

        'sada idemo na upisivanje

        kfmns.Text = "TRANSFORM sum(round([Area (I)])) as suma_  SELECT [idzaBazu], sum(round([Area (I)])) as Povrsina, sum(round([area (I)])*[faktor]) as V FROM [table_pr_razred] group by [idzaBazu] PIVOT [table_pr_razred].[procembeni]" : kfmns.RunEx(True)

        doc.Save()
        'sada imas upis u bazu

        lbl_infoMain.Text = "Upisivanje parcela u bazu - klasifikacija" : My.Application.DoEvents()

        tbl_ = kfmns.Table
        pb1.Maximum = tbl_.RecordSet.Count + 1


        For i = 0 To tbl_.RecordSet.Count - 1

            pb1.Value = i
            'sada bi trebalo utvrditi broj procembenih razreda kojih ima u file-u pa na osnovu toga napraviti ovaj query!

            Dim stinsert_ As String = "insert into kom_novoStanjaParcela (idko, idKfmsns,"

            For p = 3 To tbl_.ColumnSet.Count - 2 'zbog neplodnog
                stinsert_ = stinsert_ & "prazred_" & tbl_.ColumnSet.Item(p).Name & ","
            Next
            stinsert_ = stinsert_ & "prazred_neplodno) values (" & matbr_ & "," & tbl_.RecordSet.Item(i).DataText(1)


            For j = 5 To tbl_.ColumnSet.Count
                ' Dim P_ = tbl_.ColumnSet.Item(j).Name
                If tbl_.RecordSet(i).DataText(j) = "" Then
                    stinsert_ = stinsert_ & ",0"
                Else
                    stinsert_ = stinsert_ & "," & Math.Round(Val(tbl_.RecordSet(i).DataText(j)), 2)
                End If
                If j = tbl_.ColumnSet.Count Then
                    stinsert_ = stinsert_ & ",0)"
                End If
            Next

            comm_.CommandText = stinsert_ : comm_.ExecuteNonQuery()
            stinsert_ = "" : pb1.Value = i
        Next

        lbl_infoMain.Text = "Upisivanje parcela u bazu - opsti podaci" : My.Application.DoEvents()

        kfmns.Text = "select [ID],[idVlasnika],[deoparcele],[potes],[brparcele],[idKulture],[idTable],[rednibrnadele],[brPlana] from [" & My.Settings.layerName_ParceleNadela & "]" : kfmns.RunEx(True)

        pb1.Maximum = kfmns.Table.RecordSet.Count + 1
        For i = 0 To kfmns.Table.RecordSet.Count - 1
            pb1.Value = i

            Dim stinsert_ As String = "update kom_novoStanjaParcela set idIskaza=" & kfmns.Table.RecordSet.Item(i).DataText(2) & ", deoparcele=" & kfmns.Table.RecordSet.Item(i).DataText(4) & ", idpotes=" & kfmns.Table.RecordSet.Item(i).DataText(5) & ", idKulture=" & kfmns.Table.RecordSet.Item(i).DataText(7) & ", brParcele=" & kfmns.Table.RecordSet.Item(i).DataText(6) & ", idTable=" & kfmns.Table.RecordSet.Item(i).DataText(8) & ", rednibrnadele=" & kfmns.Table.RecordSet.Item(i).DataText(9) & ", brPlana=" & Chr(34) & kfmns.Table.RecordSet.Item(i).DataText(10) & Chr(34) & " where idKfmsns=" & kfmns.Table.RecordSet.Item(i).DataText(1)
            'sada mozes insert
            comm_.CommandText = stinsert_
            Try
                comm_.ExecuteNonQuery()
            Catch ex As Exception
                MsgBox("problem " & ex.Message & " -- " & stinsert_)
            End Try
        Next

        lbl_infoMain.Text = ""

        comm_.CommandText = "update kom_novoStanjaParcela set prazred_neplodno=(prazred_1+prazred_2+prazred_3+prazred_4+prazred_5+prazred_6+prazred_7+prazred_neplodno),prazred_1=0,prazred_2=0,prazred_3=0,prazred_4=0,prazred_5=0,prazred_6=0,prazred_7=0  where (idkulture=315 or idkulture=366)"
        comm_.ExecuteNonQuery()


        conn_.Close()
        comm_ = Nothing
        conn_ = Nothing
        doc.ComponentSet.Remove("kfmsns")
        drwDKPNew = Nothing
        topPRazredi = Nothing
        procRazred = Nothing
        topTable = Nothing

        'pb1.Value = 0
        MsgBox("Kraj")
        Me.Cursor = Cursors.Default
    End Sub

    Public Sub OdrediListDetaljaZaParceluTablu(nazivDrawingaParceleTable As String, nazivIDPolja As String)

        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document


        Dim drwParcele As Manifold.Interop.Drawing
        Try
            drwParcele = doc.ComponentSet(nazivDrawingaParceleTable)
        Catch ex As Exception
            MsgBox("Proverite da li postoji drawing u map file i da li ste ga pravilno podesili u podesavanjima" & ex.Message)
            Exit Sub
        End Try

        Dim drwPodelaNaListove As Manifold.Interop.Drawing
        Try
            drwPodelaNaListove = doc.ComponentSet(My.Settings.layerName_podelaNaListove)
        Catch ex As Exception
            MsgBox("Proverite da li postoji drawing u map file i da li ste ga pravilno podesili u podesavanjima" & ex.Message)
            Exit Sub
        End Try

        'sada ti treba presek kao sa procembenim razredima!
        Dim i As Integer
        pb1.Value = 1
        Dim tbl_ As Manifold.Interop.Table

        Try
            doc.ComponentSet.Remove("podela_ListoveDetalja")
        Catch ex As Exception
            'drw = doc.NewDrawing("podela_ListoveDetalja", drwParcele.CoordinateSystem)
        End Try

        Try
            tbl_ = drwPodelaNaListove.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                End If
            Next
            tbl_ = Nothing
        Catch ex2 As Exception
            'MsgBox(ex2.Message)
        End Try

        Try
            tbl_ = drwParcele.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
            tbl_ = Nothing
        Catch ex1 As Exception
            'MsgBox(ex1.Message)
        End Try


        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology : topPRazredi.Bind(drwPodelaNaListove) : topPRazredi.Build()
        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology : topParcele.Bind(drwParcele) : topParcele.Build()

        Try
            topParcele.DoIntersect(topPRazredi, "podela_ListoveDetalja")
        Catch When Err.Number = -2147352567
            'sada treba pokrenuti normalizacju
            doc.ComponentSet.Remove("podela_ListoveDetalja")
            Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
            analizer_.NormalizeTopology(drwParcele, drwParcele.ObjectSet)
            topParcele.Bind(drwParcele)
            topParcele.Build()
            topParcele.DoIntersect(topPRazredi, "podela_ListoveDetalja")
            analizer_ = Nothing
        Catch
            MsgBox(Err.Description)
        End Try

        'sada treba kreirati polje u drwparcele
        tbl_ = drwParcele.OwnedTable
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "brLista"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_ = Nothing
        tbl_ = Nothing

        doc.Save()

        Dim qDodeli As Manifold.Interop.Query = doc.NewQuery("dodeli")
        qDodeli.Text = "UPDATE (SELECT [" & nazivDrawingaParceleTable & "].[" & nazivIDPolja & "],[" & nazivDrawingaParceleTable & "].[brLista],C.[NOMENKLATU] FROM [" & nazivDrawingaParceleTable & "], (SELECT B.[NOMENKLATU],A.[" & nazivIDPolja & "] FROM ((SELECT max([Area (I)]) as P,[" & nazivIDPolja & "] FROM [podela_ListoveDetalja] GROUP by [" & nazivIDPolja & "]) as A, (SELECT [" & nazivIDPolja & "],[NOMENKLATU],[Area (I)] as P1 FROM [podela_ListoveDetalja]) as B ) WHERE A.P=B.P1 and A.[" & nazivIDPolja & "]=B.[" & nazivIDPolja & "] ) as C WHERE [" & nazivDrawingaParceleTable & "].[" & nazivIDPolja & "]=C.[" & nazivIDPolja & "] ) set [brLista]=[NOMENKLATU]"
        qDodeli.RunEx(True)

        doc.ComponentSet.Remove("podela_ListoveDetalja")

        'sada bi odavde trebalo da ide pisanje u bazu a ne ovako!
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)


        'table
        qDodeli.Text = "select " & nazivIDPolja & ",brLista from [" & nazivDrawingaParceleTable & "]"
        qDodeli.RunEx(True)
        pb1.Value = 0
        pb1.Maximum = qDodeli.Table.RecordSet.Count
        Dim stsql_ As String
        For i = 0 To qDodeli.Table.RecordSet.Count - 1
            pb1.Value = i
            If nazivIDPolja = "idTable" Then
                stsql_ = "update kom_kfmns set brojplana='" & qDodeli.Table.RecordSet(i).DataText(2) & "' where " & nazivIDPolja & "=" & qDodeli.Table.RecordSet(i).DataText(1)
            Else
                stsql_ = "update kom_novostanjaparcela set brojplana='" & qDodeli.Table.RecordSet(i).DataText(2) & "' where " & nazivIDPolja & "=" & qDodeli.Table.RecordSet(i).DataText(1)
            End If
            conn_.Open()
            comm_.CommandText = stsql_
            comm_.ExecuteNonQuery()
            conn_.Close()
        Next

        'sada ti treba upis nad parcelama

        doc.ComponentSet.Remove("dodeli")
        qDodeli = Nothing
        topParcele = Nothing : topPRazredi = Nothing
        pb1.Value = 0
        doc.Save()

        MsgBox("Kraj")

    End Sub

    Private Sub OdrediListDetaljaZaTabluToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles OdrediListDetaljaZaTabluToolStripMenuItem.Click
        OdrediListDetaljaZaParceluTablu(My.Settings.layerName_table, "idTable")
    End Sub

    Private Sub OdrediListDetaljaZaParceluNovogStanjaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles OdrediListDetaljaZaParceluNovogStanjaToolStripMenuItem.Click
        OdrediListDetaljaZaParceluTablu(My.Settings.layerName_ParceleNadela, "brParcele")
    End Sub

    Private Sub mnu_komasacija_novoStanje_kfmns_Click(sender As Object, e As System.EventArgs) Handles mnu_komasacija_novoStanje_kfmns.Click
        'ovo puni kom_table!
        If ManifoldCtrl.get_Document.Name = "" Then
            MsgBox("Ucitaj map file")
            Dock = Nothing
            Exit Sub
        End If
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Try
            'sf_diag.FileName = ""
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            sf_diag.FileName = "KFMNS.map"
            sf_diag.Title = "Upisite naziv za izlazni Map File "
            sf_diag.ShowDialog()
            If sf_diag.FileName = "" Then
                MsgBox("Kraj operacije")
                doc = Nothing
                Exit Sub
            Else
                doc = ManifoldCtrl.get_Document
                doc.SaveAs(sf_diag.FileName)
                ManifoldCtrl.DocumentPath = sf_diag.FileName
            End If
            ManifoldCtrl.Refresh()
        Catch ex As Exception
            MsgBox("Dokument je read onlyu Zatvorite ga u Manifoldu i ponovo pokrenite ovu funkciju.")
            FileClose()
            doc = Nothing
            Exit Sub
        End Try
        doc = ManifoldCtrl.get_Document
        'sada pretpostavka je da u drawingu nove table imas dva polja: Tabla sa idtable i oznakaTable sa TIDTable

        'proveris da li postoje layer-i

        Dim drwTable As Manifold.Interop.Drawing
        Try
            drwTable = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("nije podesen drawing sa tablama. Podesite pa startujte ponovo funkciju.")
            Exit Sub
        End Try

        Dim drwProcRazredi As Manifold.Interop.Drawing
        Try
            drwProcRazredi = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("nije podesen drawing sa procembenim razredima. Podesite pa startujte ponovo funkciju.")
            Exit Sub
        End Try

        'brises ako postoji uradeno presecanje
        Try
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception

        End Try

        'prvi bi trebalo proveritii dali tabela postoji ako ne postoji da je kreiras pa onda ides dalje
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        'proveravas da li postoje tabele kom_kfmns 


        comm_.CommandText = "show tables like 'kom_kfmns%'"
        conn_.Open()
        Dim connReader As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
        If connReader.HasRows = False Then
            connReader.Close()
            connReader = Nothing
            conn_.Open()
            comm_.CommandText = "CREATE TABLE `kom_kfmns` ( `idKO` INT NULL, `idTable` INT NOT NULL AUTO_INCREMENT, `OznakaTable` VARCHAR (20) NULL, tipTable INT NULL, `BrojPlana` VARCHAR (20) NULL, `Prazred1` INT DEFAULT 0, `Prazred2` INT DEFAULT 0, `Prazred3` INT DEFAULT 0, `Prazred4` INT DEFAULT 0, `Prazred5` INT DEFAULT 0, `Prazred6` INT DEFAULT 0, `Prazred7` INT DEFAULT 0, `Prazred8` INT DEFAULT 0, `Prazred_Neplodno` INT DEFAULT 0, `Psuma` INT DEFAULT 0, `VSuma` DOUBLE DEFAULT 0, `obrisan` INT DEFAULT 0, PRIMARY KEY (`idTable`), INDEX (`idTable`));"
            comm_.ExecuteNonQuery()
            'comm_.CommandText = "CREATE TABLE `kom_tablenadela` (idNadeleTable int not null auto_increment,IdTable int not NUll, idIskazZemljista int not null, redniBrojNadele int null, teorijskaVrednost double null, nadeljenoVrednost double null, nadeljenoPovrsina double null, pocetakNadeleVrednost double null, krajNadeleVrednost double null, tipNadele int null, obrisan int NUll,primedba double null,uneo int null, datumunosa datetime null, PRIMARY KEY (idNadeleTable), FOREIGN KEY (`IdTable`) REFERENCES `kom_Table` (`idTable`), FOREIGN KEY (`idIskazZemljista`) REFERENCES `kom_iskazzemljista` (`idIskaza`), INDEX(idNadeleTable), INDEX(IdTable), INDEX(idIskazZemljista))"
            'comm_.ExecuteNonQuery()
            conn_.Close()
        Else
            'tabela postoji znaci treba obrisati sve iz nje!
            connReader.Close()
            connReader = Nothing
            'tabela postoji znaci treba svesti pocetno stanje a to je u sledecim koracima:
            conn_.Open()
            'comm_.CommandText = "drop table IF EXISTS kom_tableNadela"
            'comm_.ExecuteNonQuery()
            comm_.CommandText = "drop table IF EXISTS kom_kfmns"
            comm_.ExecuteNonQuery()
            comm_.CommandText = "CREATE TABLE `kom_kfmns` ( `idKO` INT NULL, `idTable` INT NOT NULL AUTO_INCREMENT, `OznakaTable` VARCHAR (20) NULL, tipTable INT NULL, `BrojPlana` VARCHAR (20) NULL, `Prazred1` INT DEFAULT 0, `Prazred2` INT DEFAULT 0, `Prazred3` INT DEFAULT 0, `Prazred4` INT DEFAULT 0, `Prazred5` INT DEFAULT 0, `Prazred6` INT DEFAULT 0, `Prazred7` INT DEFAULT 0, `Prazred8` INT DEFAULT 0, `Prazred_Neplodno` INT DEFAULT 0, `Psuma` INT DEFAULT 0, `VSuma` DOUBLE DEFAULT 0, `obrisan` INT DEFAULT 0, PRIMARY KEY (`idTable`), INDEX (`idTable`));"
            comm_.ExecuteNonQuery()
            'comm_.CommandText = "CREATE TABLE `kom_tablenadela` (idNadeleTable int not null auto_increment,IdTable int not NUll, idIskazZemljista int not null, redniBrojNadele int null, teorijskaVrednost double null, nadeljenoVrednost double null, nadeljenoPovrsina double null, pocetakNadeleVrednost double null, krajNadeleVrednost double null, tipNadele int null, obrisan int NUll,primedba double null,uneo int null, datumunosa datetime null, PRIMARY KEY (idNadeleTable), FOREIGN KEY (`IdTable`) REFERENCES `kom_Table` (`idTable`), FOREIGN KEY (`idIskazZemljista`) REFERENCES `kom_iskazzemljista` (`idIskaza`), INDEX(idNadeleTable), INDEX(IdTable), INDEX(idIskazZemljista))"
            'comm_.ExecuteNonQuery()
            conn_.Close()
        End If

        Dim tbl_ As Manifold.Interop.Table
        'podesavanje polja na copy-copy
        Dim i As Integer
        pb1.Value = 1
        Try
            tbl_ = drwTable.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                End If
            Next
        Catch ex2 As Exception
            'MsgBox(ex2.Message)
        End Try

        Try
            tbl_ = drwProcRazredi.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
        Catch ex1 As Exception
            'MsgBox(ex1.Message)
        End Try

        'kreiranje topologije
        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology
        topPRazredi.Bind(drwProcRazredi)
        topPRazredi.Build()

        Dim topTable As Manifold.Interop.Topology = doc.Application.NewTopology
        topTable.Bind(drwTable)
        topTable.Build()

        topTable.DoIntersect(topPRazredi, "table_pr_razred")

        'sada idemo na upisivanje
        doc.Save()

        'sada mozes da formiras i tabelu - recimo ovde 

        Dim qvrprocR As Manifold.Interop.Query = doc.NewQuery("procRaz_head")


        Dim qrKFMSS2 As Manifold.Interop.Query = doc.NewQuery("kfmss_povrsine")
        'qrKFMSS2.Text = "TRANSFORM sum([Area (I)]) as suma_ SELECT [idTable], sum([Area (I)]) as Povrsina,[klasatable] FROM [table_pr_razred] group by [idTable],[klasatable] PIVOT [table_pr_razred].[procembeni]"
        qrKFMSS2.Text = "TRANSFORM sum([Area (I)]) as suma_ SELECT [idTable], sum([Area (I)]) as Povrsina, case [klasatable] when 0 then (" & Chr(34) & "G_" & Chr(34) & " & [idtable])  when 1 then (" & Chr(34) & "T_" & Chr(34) & "  & [idtable])  when 2 then (" & Chr(34) & "T_" & Chr(34) & " & [idtable])  when 3 then (" & Chr(34) & "Put_" & Chr(34) & " & [idtable])  when 5 then (" & Chr(34) & "Kanal_" & Chr(34) & " & [idtable]) when 7 then (" & Chr(34) & "Suma_" & Chr(34) & " & [idtable]) when 8 then (" & Chr(34) & "Pruga_" & Chr(34) & " & [idtable]) when 9 then (" & Chr(34) & "Reka_" & Chr(34) & " & [idtable]) when 10 then (" & Chr(34) & "Nasip_" & Chr(34) & " &  [idtable]) end as oznakaTable, [klasatable] FROM [table_pr_razred] group by [idTable],[klasatable] PIVOT [table_pr_razred].[procembeni]"
        qrKFMSS2.RunEx(True)
        doc.Save()

        'sada treba upisati u bazu
        tbl_ = qrKFMSS2.Table
        pb1.Maximum = tbl_.RecordSet.Count + 1

        For i = 0 To tbl_.RecordSet.Count - 1

            pb1.Value = i
            'sada bi trebalo utvrditi broj procembenih razreda kojih ima u file-u pa na osnovu toga napraviti ovaj query!

            Dim stinsert_ As String = "insert into kom_kfmns (idTable,OznakaTable,tiptable,"

            For p = 4 To tbl_.ColumnSet.Count - 2 'zbog neplodnog
                stinsert_ = stinsert_ & "prazred" & tbl_.ColumnSet.Item(p).Name & ","
            Next
            stinsert_ = stinsert_ & "prazred_neplodno, obrisan) values (" & tbl_.RecordSet.Item(i).DataText(1) & "," & Chr(34) & tbl_.RecordSet.Item(i).DataText(3) & Chr(34) & "," & tbl_.RecordSet.Item(i).DataText(4)


            For j = 5 To tbl_.ColumnSet.Count
                ' Dim P_ = tbl_.ColumnSet.Item(j).Name
                If tbl_.RecordSet(i).DataText(j) = "" Then
                    stinsert_ = stinsert_ & ",0"
                Else
                    stinsert_ = stinsert_ & "," & Math.Round(Val(tbl_.RecordSet(i).DataText(j)), 2)
                End If
                If j = tbl_.ColumnSet.Count Then
                    stinsert_ = stinsert_ & ",0)"
                End If
            Next


            conn_.Open()
            comm_.CommandText = stinsert_
            comm_.ExecuteNonQuery()
            conn_.Close()
            stinsert_ = ""
            pb1.Value = i
        Next

        conn_.Open()
        comm_.CommandText = "UPDATE kom_kfmns set vsuma=(Prazred1+Prazred2*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=2) +Prazred3*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=3)+Prazred4*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=4)+Prazred5*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=5)+Prazred6*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=6)+Prazred7*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=7)+Prazred8*(SELECT VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=8))"
        comm_.ExecuteNonQuery()
        comm_.CommandText = "UPDATE kom_kfmns set psuma=(Prazred1+Prazred2+Prazred3+Prazred4+Prazred5+Prazred6+Prazred7+Prazred8+Prazred_neplodno)"
        comm_.ExecuteNonQuery()

        comm_.CommandText = "update kom_kfmns set prazred_neplodno=(prazred1+prazred2+prazred3+prazred4+prazred5+prazred6+prazred7+prazred_neplodno) where tiptable not in (1,2)"
        comm_.ExecuteNonQuery()
        comm_.CommandText = "update kom_kfmns set prazred1=0,prazred2=0,prazred3=0,prazred4=0,prazred5=0,prazred6=0,prazred7=0,vsuma=0 where tiptable not in (1,2)"
        comm_.ExecuteNonQuery()
        comm_.CommandText = "update kom_kfmns set idko=(select idko from kom_parcele limit 1)"
        comm_.ExecuteNonQuery()

        conn_.Close()

        comm_ = Nothing : conn_ = Nothing : pb1.Value = 0
        doc.Save()

        MsgBox("Kraj")
    End Sub

    Private Sub NumeracijaDelovaParcelaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles NumeracijaDelovaParcelaToolStripMenuItem.Click

        'drawing se nalazei u dkp_nadela
        'polje koje gada je BrDelaParc
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim dwg_ As Manifold.Interop.Drawing

        Try
            dwg_ = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        Catch ex As Exception
            MsgBox("Proverite podesavanje u map fileu.")
            Exit Sub
        End Try

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("preko")
        Dim qvrUpdate_ As Manifold.Interop.Query = doc.NewQuery("update_")

        qvr_.Text = "SELECT [id], [brparcele], [Povrsina], [Skulture] FROM [" & My.Settings.layerName_ParceleNadela & "] ORDER by [brparcele],[id_redosled], [Skulture],[Povrsina] desc"
        qvr_.RunEx(True)
        Dim parcela_ As String = ""
        Dim kultura_ As Integer = 0
        Dim redniBroj_ As Integer = 1

        pb1.Maximum = qvr_.Table.RecordSet.Count
        pb1.Value = 0
        For i = 0 To qvr_.Table.RecordSet.Count - 2
            pb1.Value = i
            If i = 0 Then
                parcela_ = qvr_.Table.RecordSet.Item(i).DataText(2)
                kultura_ = qvr_.Table.RecordSet.Item(i).DataText(4)
            End If

            If parcela_ = qvr_.Table.RecordSet.Item(i + 1).DataText(2) Then
                'znaci i sledeca je isti borj parcele
                'proveriti da li je i kultura ista
                If kultura_ = qvr_.Table.RecordSet.Item(i + 1).DataText(4) And qvr_.Table.RecordSet.Item(i + 1).DataText(4) <> 360 Then
                    'ista kultura upisjes isti brojac
                    qvrUpdate_.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [BrDelaParc]=" & redniBroj_ & " where [id]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
                    qvrUpdate_.RunEx(True)
                Else
                    'razicita kultura upisujes brojac i dodajes 1
                    qvrUpdate_.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [BrDelaParc]=" & redniBroj_ & " where [id]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
                    qvrUpdate_.RunEx(True)
                    redniBroj_ += 1
                End If

            Else
                'sledeci broj parcele nije isti upisujes trebutno stanje i resetujes brojac
                qvrUpdate_.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [BrDelaParc]=" & redniBroj_ & " where [id]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
                qvrUpdate_.RunEx(True)
                parcela_ = qvr_.Table.RecordSet.Item(i + 1).DataText(2)
                kultura_ = qvr_.Table.RecordSet.Item(i + 1).DataText(4)
                redniBroj_ = 1

            End If

        Next

        doc.ComponentSet.Remove("preko")
        doc.ComponentSet.Remove("update_")
        doc.Save()
        pb1.Value = 0
        MsgBox("Kraj")

    End Sub

    Private Sub ExportParcelaUBazuToolStripMenuItem_Click(sender As Object, e As System.EventArgs)

    End Sub

    Private Sub NumeracijaObjekataToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles NumeracijaObjekataToolStripMenuItem.Click
        'radi numeraciju delova parcela u okviru objekta na osnovu selekcije!
        'u map file- treba da imas drawing sa sledecim poljima: brparcele,povrsina,deoparcele, nacinkoriscenja
        'ovaj drawing u podesavanjima ide kao da je u pitanju parcele nadela
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        'Dim drwParcele As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("listing")
        Dim qvrupdate As Manifold.Interop.Query = doc.NewQuery("douodo")

        qvr_.Text = "select [ID],[brparcele],[Povrsina],[nacinkoriscenja], case [idKulture] when 360 then 1 when 361 then 2 when 370 then 3 else 4 end as redbr_  FROM [" & My.Settings.layerName_ParceleNadela & "] ORDER by  [brparcele],redbr_,[nacinkoriscenja],[Povrsina] desc"
        qvr_.RunEx(True)

        pb1.Value = 0
        pb1.Maximum = qvr_.Table.RecordSet.Count
        Dim poslednji_ As Integer = -1
        Dim redniBroj_ As Integer = 0
        For i = 0 To qvr_.Table.RecordSet.Count - 1
            If i = 0 Then
                poslednji_ = qvr_.Table.RecordSet.Item(0).DataText(2)
            End If
            If poslednji_ = Val(qvr_.Table.RecordSet.Item(i).DataText(2)) Then
                redniBroj_ += 1
            Else
                'sada ga resetujes!
                redniBroj_ = 1
                poslednji_ = Val(qvr_.Table.RecordSet.Item(i).DataText(2))
            End If

            qvrupdate.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [deoparcele]=" & redniBroj_ & " where [ID]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
            qvrupdate.RunEx(True)
            pb1.Value = i
        Next

        pb1.Value = 0

        doc.ComponentSet.Remove("listing")
        doc.ComponentSet.Remove("douodo")

        doc.Save()

        MsgBox("Kraj")
    End Sub

    Private Sub KontrolaPovršineIzKoordinataToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KontrolaPovršineIzKoordinataToolStripMenuItem.Click
        'trebaju ti dve stvari : layeru kome se nalaze definisane table 
        'iz baze ti treba broj lista-ovo bi trebalo da se doda
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        If doc.FullName = "" Then
            MsgBox("Greska : Ucitajte map file prvo")
            Exit Sub
        End If

        Try
            doc.Save()
        Catch ex As Exception

            MsgBox("File je otvoren u Manifold-u. Zatvoriter i probajte ponovo")
            doc = Nothing
            Exit Sub
        End Try

        sf_diag.FileName = ""
        sf_diag.Title = "Izlazni file za spisak Tacaka"
        sf_diag.DefaultExt = "txt"
        sf_diag.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then
            Exit Sub
        End If

        Dim freeFile_ As Integer = FreeFile() : FileOpen(freeFile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Dim freefile3_ As Integer = FreeFile() : FileOpen(freefile3_, Replace(sf_diag.FileName, ".txt", "1.txt"), OpenMode.Output, OpenAccess.Write, OpenShare.Shared)


        Dim freefile2_ As Integer = FreeFile()
        FileOpen(freefile2_, Path.GetTempPath() & "\parceleSpisakKontrola.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)


        'Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        'sada ti trebaju dva drawinga 
        Dim drwTable, pntTableObelezavanje As Manifold.Interop.Drawing
        Try
            drwTable = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
            pntTableObelezavanje = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Catch ex As Exception
            MsgBox("Proverite podesavanje ulaznih parametara")
            Exit Sub
        End Try

        Me.Cursor = Cursors.WaitCursor


        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("tableInfo")
        qvr_.Text = "select [idTable],[ID],[deoparcele],[brparcele] from [" & My.Settings.layerName_ParceleNadela & "] order by [brparcele],[deoparcele]" : qvr_.RunEx(True)
        pb1.Value = 0 : pb1.Maximum = qvr_.Table.RecordSet.Count

        lbl_infoMain.Text = "Ispisivanje listinga tacaka po parcelama u file"
        My.Application.DoEvents()

        Dim x(-1), y(-1) As Double
        Dim brojzaokruzivanje As Integer = My.Settings.zaokruzivanjeBrojDecMesta

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            pb1.Value = i
            Try
                lbl_infoMain.Text = "Ispisivanje listinga tacaka po parcelama u file. Obrada parcele broj = " & qvr_.Table.RecordSet.Item(i).DataText(4)
                My.Application.DoEvents()


                Dim qvrTacke As Manifold.Interop.Query = doc.NewQuery("pronadiTacke")
                'qvrTacke.Text = "select B.* FROM (SELECT CentroidX(pnt_) as x1, CentroidY(pnt_) as y1 FROM [" & My.Settings.layerName_table & "] WHERE [idTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & " SPLIT by Coords([Geom (I)]) as pnt_) as A LEFT OUTER JOIN (SELECT [idTacke],CentroidX([Geom (I)]) as x2,CentroidY([Geom (I)]) as y2 FROM [" & pntTableObelezavanje.Name & "]) as B on  round(A.x1,2)=round(B.x2,2) and round(A.y1,2)=round(B.y2,2)"
                qvrTacke.Text = "select * FROM (select B.* FROM (SELECT CentroidX(pnt_) as x1, CentroidY(pnt_) as y1 FROM [" & My.Settings.layerName_ParceleNadela & "] WHERE [brparcele]=" & qvr_.Table.RecordSet.Item(i).DataText(4) & " and [deoparcele]=" & qvr_.Table.RecordSet.Item(i).DataText(3) & " SPLIT by Coords([Geom (I)]) as pnt_) as A LEFT OUTER JOIN (SELECT [idTacke],CentroidX([Geom (I)]) as x2,CentroidY([Geom (I)]) as y2,[tipTacke] FROM [" & pntTableObelezavanje.Name & "]) as B on  round(A.x1,2)=round(B.x2,2) and round(A.y1,2)=round(B.y2,2)) as C where  [idTacke] IS NOT NULL"
                qvrTacke.RunEx(True)
                'doc.Save()
                PrintLine(freeFile_, "Parcela broj = " & qvr_.Table.RecordSet.Item(i).DataText(4) & " deoparcele= " & qvr_.Table.RecordSet.Item(i).DataText(3))
                ReDim x(qvrTacke.Table.RecordSet.Count), y(qvrTacke.Table.RecordSet.Count) 'da imas mesta i za poslednju tacku!
                For j = 0 To qvrTacke.Table.RecordSet.Count - 1
                    x(j) = qvrTacke.Table.RecordSet.Item(j).DataText(2) : y(j) = qvrTacke.Table.RecordSet.Item(j).DataText(3)
                    PrintLine(freeFile_, qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & FormatNumber(qvrTacke.Table.RecordSet.Item(j).DataText(2), brojzaokruzivanje, 0, 0, 0) & "," & FormatNumber(qvrTacke.Table.RecordSet.Item(j).DataText(3), brojzaokruzivanje, 0, 0, 0))
                    'PrintLine(freefile3_, qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3))
                    PrintLine(freefile2_, j & "," & qvrTacke.Table.RecordSet.Item(j).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(j).DataText(3) & "," & qvr_.Table.RecordSet.Item(i).DataText(4))
                Next
                x(qvrTacke.Table.RecordSet.Count) = qvrTacke.Table.RecordSet.Item(0).DataText(2) : y(qvrTacke.Table.RecordSet.Count) = qvrTacke.Table.RecordSet.Item(0).DataText(3)
                PrintLine(freefile2_, qvrTacke.Table.RecordSet.Count & "," & qvrTacke.Table.RecordSet.Item(0).DataText(1) & "," & qvrTacke.Table.RecordSet.Item(0).DataText(2) & "," & qvrTacke.Table.RecordSet.Item(0).DataText(3) & "," & qvr_.Table.RecordSet.Item(i).DataText(1))

                'sada mozes povrsinu!
                Dim p_ As Double = 0 : Dim p2_ As Double = 0 : Dim dp12_ As Double = 0
                For j = 0 To x.Length - 2
                    p_ += ((x(j + 1) - x(j)) * (y(j + 1) + y(j))) / 2
                    p2_ += ((Math.Round(x(j + 1), brojzaokruzivanje) - Math.Round(x(j), brojzaokruzivanje)) * (Math.Round(y(j + 1), brojzaokruzivanje) + Math.Round(y(j), brojzaokruzivanje))) / 2
                Next
                PrintLine(freeFile_, "")
                'sada imas i ovaj p2_ pa mozes da uradis nesto sa ovim kad vidim sta je laslo hteo
                PrintLine(freeFile_, "POVRSINA: " & Math.Abs(Math.Round(p_, 0)) & " м2, POVRSINA DOBIJENA IZ KOORDINATA NA " & brojzaokruzivanje & " DECIMALE: " & Math.Abs(p2_) & " razlika Pzaok-Psve=" & Math.Abs(Math.Round(p_, 0)) - Math.Abs(p2_))
                PrintLine(freeFile_, vbNewLine)
                PrintLine(freefile3_, qvr_.Table.RecordSet.Item(i).DataText(4) & "," & qvr_.Table.RecordSet.Item(i).DataText(3) & "," & Math.Abs(Math.Round(p_, 0)) & "," & Math.Abs(p2_))
                doc.ComponentSet.Remove("PronadiTacke") : qvrTacke = Nothing

            Catch ex As Exception

                'sta ako nema onda puca jos na upitu
                If MsgBox(ex.Message, MsgBoxStyle.OkCancel, "Pitanje: izlazim?") = MsgBoxResult.Ok Then
                    Exit Sub
                End If

            End Try

        Next
        pb1.Value = 0
        'conn_ = Nothing
        'comm_ = Nothing
        doc.ComponentSet.Remove("tableInfo")
        qvr_ = Nothing
        'qvrKopirajOK = Nothing
        'qvrOperat_ = Nothing
        FileClose()
        'doc.Save()
        Me.Cursor = Cursors.Default
        MsgBox("Kraj")
    End Sub

    Private Sub KomasacionoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KomasacionoToolStripMenuItem.Click
        'On Error Resume Next
        Try
            sf_diag.FileName = ""
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            'sf_diag.FileName = "nadela_tabla" & ddl_ttpSpisakTabli.SelectedValue & ".map"
            sf_diag.Title = "Upisite naziv za izlazni Map File - za STAMPU NADELE - PREGLEDNE TABLE"
            sf_diag.ShowDialog()
            If sf_diag.FileName = "" Then
                MsgBox("Kraj operacije")
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox("Dokument je read onlyu Zatvorite ga u Manifoldu i ponovo pokrenite ovu funkciju.")
            FileClose()
            Exit Sub
        End Try

        Dim manApp As Manifold.Interop.Application = New Manifold.Interop.Application
        Dim newDoc As Manifold.Interop.Document = manApp.NewDocument("", False)

        Try
            newDoc.SaveAs(sf_diag.FileName)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju")
            Exit Sub
        End Try

        'sada iz starog DKP_Nadela kopiras u novi map file

        Dim docOld As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim drwNadelaOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_ParceleNadela)
        Dim drwNadelaNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_ParceleNadela, drwNadelaOld.CoordinateSystem, True)

        drwNadelaOld.Copy(False)
        drwNadelaNew.Paste(True)
        drwNadelaOld.SelectNone()
        drwNadelaNew.SelectNone()

        Dim drwTackeOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_nadelaSmer)
        Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_nadelaSmer, drwNadelaOld.CoordinateSystem, True)

        drwTackeOld.Copy(False)
        drwTackeNew.Paste(True)
        drwTackeOld.SelectNone()
        drwTackeNew.SelectNone()

        Dim qvr_ As Manifold.Interop.Query = newDoc.NewQuery("analiza")
        Dim qvrUpdate As Manifold.Interop.Query = newDoc.NewQuery("update")
        Dim qvrTacke As Manifold.Interop.Query = newDoc.NewQuery("tacke")
        Dim qvrTackePoslednje As Manifold.Interop.Query = newDoc.NewQuery("poslednje_tacke")

        newDoc.Save()

        'sada dodajes i table jer ti to treba za apcisna odmeranja

        Dim drwTableOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_table)
        Dim drwTableNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_table, drwTableOld.CoordinateSystem, True)

        drwTableOld.Copy(False)
        drwTableNew.Paste(True)
        drwTableOld.SelectNone()
        drwTableNew.SelectNone()

        Dim drwPntNadelaOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Dim drwPntNadelaNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_pointTableObelezavanje, drwPntNadelaOld.CoordinateSystem, True)

        drwPntNadelaOld.Copy(False)
        drwPntNadelaNew.Paste(True)
        drwPntNadelaOld.SelectNone()
        drwPntNadelaNew.SelectNone()

        ' SADA IDU APCISNA ODMERANJA
        Dim labelApcisno_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Apcisno", drwTableNew.CoordinateSystem, True)
        labelApcisno_.LabelAlignX = LabelAlignX.LabelAlignXRight : labelApcisno_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelApcisno_.OptimizeLabelAlignX = False : labelApcisno_.OptimizeLabelAlignY = False
        labelApcisno_.ResolveOverlaps = False : labelApcisno_.PerLabelFormat = True
        Dim lblSetsApcisno_ As Manifold.Interop.LabelSet = labelApcisno_.LabelSet

        lbl_infoMain.Text = "Kreiram Label za Apcisno odmeranje" : My.Application.DoEvents()

        qvr_.Text = "select distinct [idTable] from [" & drwTableNew.Name & "] where [tipTable]=2 order by [idTable]"
        qvr_.RunEx(True)

        'ovde ti treba samo label i to je mozda najveci problem aj onda odma label!
        pb1.Value = 0
        pb1.Maximum = qvr_.Table.RecordSet.Count

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            lbl_infoMain.Text = "Kreiram Label za Apcisno odmeranje. Tabla " & qvr_.Table.RecordSet.Item(i).DataText(1) : My.Application.DoEvents()

            Dim pnt_ As Manifold.Interop.Point
            qvrUpdate.Text = "select idtacke FROM (SELECT pnts_ FROM [" & drwTableNew.Name & "] WHERE [tipTable]=2 and [idTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & " split by Coords([Geom (I)]) as pnts_) as A left join (SELECT [idTacke],[Geom (I)] FROM [" & drwPntNadelaNew.Name & "]) as B on A.pnts_=B.[Geom (I)]"
            'sada imas listing tacaka!
            qvrUpdate.RunEx(True)
            Dim listofPoint(qvrUpdate.Table.RecordSet.Count) As String

            'ovde cemo ovo u listu i dodamo na kraj prvu tacku i nema problema - lakse!
            For j = 0 To qvrUpdate.Table.RecordSet.Count - 1
                listofPoint(j) = qvrUpdate.Table.RecordSet.Item(j).DataText(1)
            Next
            'sada dodajes na poslednjem mestu prvu tacku
            listofPoint(listofPoint.Length - 1) = qvrUpdate.Table.RecordSet.Item(0).DataText(1)
            'listofPoint.Reverse()
            For j = listofPoint.Length - 1 To 1 Step -1
                Dim labb_ As Manifold.Interop.Label
                'MsgBox(listofPoint(j) & ", " & listofPoint(j - 1))
                qvrTacke.Text = "SELECT centroidx(pnt1) as xp,centroidy(pnt1) as yp,centroidx([Geom (I)]) as xd,CentroidY([Geom (I)]) as yd, Distance(pnt1,[Geom (I)]) as D FROM (SELECT NewLine(pnt1,pnt2) as line_,pnt1  FROM (select [Geom (I)] as pnt1 from [" & drwPntNadelaNew.Name & "] where [idTacke]=" & Chr(34) & listofPoint(j) & Chr(34) & "), (select [Geom (I)] as pnt2 from [" & drwPntNadelaNew.Name & "] where [idTacke]=" & Chr(34) & listofPoint(j - 1) & Chr(34) & ")) as A, [" & drwPntNadelaNew.Name & "] WHERE Touches(line_,[" & drwPntNadelaNew.Name & "].[Geom (I)]) and [tipTacke]=3"
                qvrTacke.RunEx(True)

                For k = 0 To qvrTacke.Table.RecordSet.Count - 1
                    pnt_ = newDoc.Application.NewPoint(qvrTacke.Table.RecordSet.Item(k).DataText(3), qvrTacke.Table.RecordSet.Item(k).DataText(4))
                    lblSetsApcisno_.Add(Math.Round(Val(qvrTacke.Table.RecordSet.Item(k).DataText(5)), 2), pnt_)
                    labb_ = lblSetsApcisno_.LastAdded
                    labb_.Size = 4
                    Dim ni_
                    Try
                        ni_ = NiAnaB(qvrTacke.Table.RecordSet.Item(k).DataText(1), qvrTacke.Table.RecordSet.Item(k).DataText(2), qvrTacke.Table.RecordSet.Item(k).DataText(3), qvrTacke.Table.RecordSet.Item(k).DataText(4))
                        labb_.Rotation = ni_
                    Catch ex As Exception
                    End Try
                    If k = 0 Then
                        pnt_ = newDoc.Application.NewPoint(qvrTacke.Table.RecordSet.Item(k).DataText(1), qvrTacke.Table.RecordSet.Item(k).DataText(2))
                        lblSetsApcisno_.Add("0.00", pnt_)
                        labb_ = lblSetsApcisno_.LastAdded
                        labb_.Size = 4
                        labb_.Rotation = ni_
                    End If
                    If k = qvrTacke.Table.RecordSet.Count - 1 Then
                        qvrTackePoslednje.Text = "SELECT xp,yp,xz,yz,distance(pnt1,pnt2) FROM (SELECT CentroidX([id]) as xp,CentroidY([id]) as yp ,[Geom (I)] as pnt1  FROM [" & drwPntNadelaNew.Name & "] WHERE [idTacke]=" & Chr(34) & listofPoint(j) & Chr(34) & ") , (SELECT CentroidX([id]) as xz,CentroidY([id]) as yz,[Geom (I)] as pnt2 FROM [" & drwPntNadelaNew.Name & "] WHERE [idTacke]=" & Chr(34) & listofPoint(j - 1) & Chr(34) & ")"
                        'qvrTackePoslednje.Text = " SELECT centroidx(pnt1) as xp,centroidy(pnt1) as yp FROM (SELECT [Geom (I)] as pnt1 from [" & drwPntNadelaNew.Name & "] where [idTacke]=" & Chr(34) & listofPoint(j - 1) & Chr(34)
                        qvrTackePoslednje.RunEx(True)
                        pnt_ = newDoc.Application.NewPoint(qvrTackePoslednje.Table.RecordSet.Item(0).DataText(3), qvrTackePoslednje.Table.RecordSet.Item(0).DataText(4))
                        lblSetsApcisno_.Add(Math.Round(Val(qvrTackePoslednje.Table.RecordSet.Item(0).DataText(5)), 2), pnt_)
                        labb_ = lblSetsApcisno_.LastAdded
                        labb_.Size = 4
                        labb_.Rotation = ni_
                    End If
                Next
                'ovde mislim da fali na poslednju al to cemo sad da vidimo!

                labb_ = Nothing
            Next

            'sada treba ovo za poslednju tacke!
            'sve isto kao gore al je drugaciji ulaz!
            'newDoc.Save()

            pnt_ = Nothing
            pb1.Value = i
        Next


        newDoc.Save() 'ovo posle izbaci!

        'sada pravis linije za export

        Dim drwParceleLinije As Manifold.Interop.Drawing
        Try
            drwParceleLinije = newDoc.NewDrawing("DKP_Nadela_Linije", drwNadelaNew.CoordinateSystem, True)
        Catch ex As Exception
            newDoc.ComponentSet.Remove("DKP_Nadela_Linije")
            drwParceleLinije = newDoc.NewDrawing("DKP_Nadela_Linije", drwNadelaNew.CoordinateSystem, True)
        End Try

        Dim analizer_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
        analizer_.Boundaries(drwNadelaNew, drwNadelaNew, drwNadelaNew.ObjectSet)

        drwNadelaNew.Cut(True)
        drwParceleLinije.Paste(True)

        analizer_.Explode(drwParceleLinije, drwParceleLinije.ObjectSet)
        analizer_.RemoveDuplicates(drwParceleLinije, drwParceleLinije.ObjectSet)

        analizer_ = Nothing

        'sada polje sa duzinom

        'Frontovi

        Dim tbl_ As Manifold.Interop.Table : tbl_ = drwParceleLinije.OwnedTable
        Dim col_ As Manifold.Interop.Column = newDoc.Application.NewColumnSet.NewColumn
        col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)

        qvr_.Text = "update [DKP_Nadela_Linije] set [Duzina]=round([Length (I)],2), [Ugao]=round([Bearing (I)],2)"
        qvr_.RunEx(True)

        newDoc.Save()

        'sada formiras label

        'kreiras tacke i sa ova dva polja treba da napravis label - odnosno duzina ti je label a ugao ti je rotacija! - samo se postavlja pitanje kako pojedinacnu da napravis
        qvr_.Text = "select centroidx(LinePoint([Geom (I)], [Length (I)]/2)),centroidy(LinePoint([Geom (I)], [Length (I)]/2)),[Ugao],[Duzina] from [DKP_Nadela_Linije]"
        qvr_.RunEx(True)

        Dim drwLabelFront As Manifold.Interop.Labels = newDoc.NewLabels("label_front", drwParceleLinije.CoordinateSystem, True)
        drwLabelFront.LabelAlignX = LabelAlignX.LabelAlignXLeft : drwLabelFront.LabelAlignY = LabelAlignY.LabelAlignYTop
        drwLabelFront.OptimizeLabelAlignX = False : drwLabelFront.OptimizeLabelAlignY = False
        drwLabelFront.ResolveOverlaps = False : drwLabelFront.PerLabelFormat = True

        Dim lblSets_ As Manifold.Interop.LabelSet = drwLabelFront.LabelSet
        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        lbl_infoMain.Text = "Kreiranje Label-a za frontove"
        My.Application.DoEvents()

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            'e sada da vidimo kreiras tacku i onda ide dalje
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(i).DataText(1), qvr_.Table.RecordSet.Item(i).DataText(2))
            lblSets_.Add(qvr_.Table.RecordSet.Item(i).DataText(4), pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets_.LastAdded
            labb_.Rotation = qvr_.Table.RecordSet.Item(i).DataText(3) - 90
            labb_.Size = 3.75
            pnt_ = Nothing
        Next
        newDoc.Save()
        pb1.Value = 0
        lblSets_ = Nothing

        If MsgBox("Da li radim frontove za stalne zasade?", MsgBoxStyle.OkCancel, "Pitanje") = MsgBoxResult.Ok Then

            'Frontovi stalni objekti


            Dim drwStalniZasadiNew As Manifold.Interop.Drawing = newDoc.NewDrawing("stalniZasadi", drwParceleLinije.CoordinateSystem, True)
            Dim drwStalniZasadiOld As Manifold.Interop.Drawing = docOld.ComponentSet("StalniZasadi")

            drwStalniZasadiOld.Copy(False)
            drwStalniZasadiNew.Paste(True)

            docOld = Nothing : drwNadelaOld = Nothing : drwStalniZasadiOld = Nothing

            tbl_ = drwStalniZasadiNew.OwnedTable
            col_ = newDoc.Application.NewColumnSet.NewColumn
            col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
            col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)


            qvr_.Text = "update [StalniZasadi] set [Duzina]=round([Length (I)],2), [Ugao]=round([Bearing (I)],2)"
            qvr_.RunEx(True)

            newDoc.Save()

            'sada formiras label

            'kreiras tacke i sa ova dva polja treba da napravis label - odnosno duzina ti je label a ugao ti je rotacija! - samo se postavlja pitanje kako pojedinacnu da napravis
            qvr_.Text = "select centroidx(LinePoint([Geom (I)], [Length (I)]/2)),centroidy(LinePoint([Geom (I)], [Length (I)]/2)),[Ugao],[Duzina] from [StalniZasadi]"
            qvr_.RunEx(True)

            Dim drwLabelFrontStalniZasadi As Manifold.Interop.Labels = newDoc.NewLabels("label_front_StalniZasadi", drwParceleLinije.CoordinateSystem, True)
            drwLabelFrontStalniZasadi.LabelAlignX = LabelAlignX.LabelAlignXLeft : drwLabelFrontStalniZasadi.LabelAlignY = LabelAlignY.LabelAlignYTop
            drwLabelFrontStalniZasadi.OptimizeLabelAlignX = False : drwLabelFrontStalniZasadi.OptimizeLabelAlignY = False
            drwLabelFrontStalniZasadi.ResolveOverlaps = False : drwLabelFrontStalniZasadi.PerLabelFormat = True

            Dim lblSetsSZ_ As Manifold.Interop.LabelSet = drwLabelFrontStalniZasadi.LabelSet
            pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

            lbl_infoMain.Text = "Kreiranje Label-a za frontove za stalne zasade"
            My.Application.DoEvents()

            For i = 0 To qvr_.Table.RecordSet.Count - 1
                pb1.Value = i
                'e sada da vidimo kreiras tacku i onda ide dalje
                Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(i).DataText(1), qvr_.Table.RecordSet.Item(i).DataText(2))
                lblSetsSZ_.Add(qvr_.Table.RecordSet.Item(i).DataText(4), pnt_)
                'sada treba nekako rotacija?
                Dim labb_ As Manifold.Interop.Label = lblSetsSZ_.LastAdded
                labb_.Rotation = qvr_.Table.RecordSet.Item(i).DataText(3) - 90
                labb_.Size = 3.75
                pnt_ = Nothing
            Next

            lblSetsSZ_ = Nothing
            newDoc.Save()
            pb1.Value = 0

        End If

        'sada ides na vlasnike!

        'kreiras polja

        tbl_ = drwNadelaNew.OwnedTable
        col_.Name = "Opis1" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        'sada update- ove kolone

        qvr_.Text = "select distinct [idvlasnika] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim myCommand As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()

        pb1.Maximum = qvr_.Table.RecordSet.Count
        pb1.Value = 0

        lbl_infoMain.Text = "Popunjavanje Vlasnika u polju Opis"
        My.Application.DoEvents()

        Dim maxPolja_ As Integer = -1
        For i = 0 To qvr_.Table.RecordSet.Count - 1
            myCommand.CommandText = "SELECT CONVERT(GROUP_concat(vlasnik_ SEPARATOR ';') USING utf8),count(*) as br FROM (SELECT  if(udeo='1/1',vlasnik_,concat(udeo,' ',vlasnik_)) as vlasnik_  FROM ((SELECT distinct idVlasnika,udeo FROM kom_vezaparcelavlasnik where obrisan=0 and idiskazzemljista=" & qvr_.Table.RecordSet.Item(i).DataText(1) & ") as A LEFT OUTER JOIN (SELECT idVlasnika, concat(PREZIME,if(isnull(IMEOCA),' ',concat(' (',imeoca,') ')),ime,if(isnull(mesto),' ', concat(' ',mesto,',')),if(isnull(ulica),' ',concat(CONVERT(' ul ' USING utf8),ulica,' ')),if(isnull(broj),' ',concat(' kbr ',broj,' ')),if(isnull(uzbroj),'',uzbroj) ) as vlasnik_  FROM kom_vlasnik) as B on A.idvlasnika=B.idVlasnika )) as C"
            Dim myreader_ As MySql.Data.MySqlClient.MySqlDataReader
            Try
                myreader_ = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myreader_ = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            pb1.Value = i
            If myreader_.HasRows Then
                myreader_.Read()
                If IsDBNull(myreader_.GetValue(0)) = False Then
                    Dim sta_ As String = myreader_.GetValue(0)
                    sta_ = Replace(sta_, "ul", "ул")
                    sta_ = Replace(sta_, "kbr", "кћ.бр.")

                    sta_ = konvertCirilicaULatinicu(sta_)

                    qvrUpdate.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [Opis1]=" & Chr(34) & sta_ & Chr(34) & " where [idvlasnika]=" & qvr_.Table.RecordSet.Item(i).DataText(1)

                    Try
                        qvrUpdate.RunEx(True)
                    Catch ex As Exception
                        MsgBox("Problem na : " & myCommand.CommandText)
                        Exit Sub
                    End Try


                    If maxPolja_ < myreader_.GetValue(1) Then
                        maxPolja_ = myreader_.GetValue(1)
                    End If
                Else
                    'problem
                End If
            Else
                'ovde je problem i treba prijaviti
                MsgBox("Nemam " & qvr_.Table.RecordSet.Item(i).DataText(1))
            End If
            myreader_.Close()
            myreader_ = Nothing
        Next
        conn_.Close()
        newDoc.Save()

        'sada ako treba prosiris ova polja!
        'MsgBox(maxPolja_)

        'sada kreiras nova polja da bi mogao dalje!
        tbl_ = drwNadelaNew.OwnedTable
        For i = 2 To maxPolja_
            col_.Name = "Opis" & i : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : tbl_.ColumnSet.Add(col_)
        Next

        'sada parsiras vlasnike!

        qvr_.Text = "select [opis1],[ID] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        Dim labelLinije_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Vlasnik", drwParceleLinije.CoordinateSystem, True)
        labelLinije_.LabelAlignX = LabelAlignX.LabelAlignXLeft : labelLinije_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelLinije_.OptimizeLabelAlignX = False : labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False : labelLinije_.PerLabelFormat = True

        Dim lblSets2_ As Manifold.Interop.LabelSet = labelLinije_.LabelSet

        lbl_infoMain.Text = "Kreiram Label Opis za svaku parcelu" : My.Application.DoEvents()

        qvr_.Text = "SELECT ax_,ay_,opis_,bx1_,by1_,bx2_,by2_ from (SELECT [X (I)] as ax_,[Y (I)] as ay_, [opis1] as opis_, [idtable] FROM [" & My.Settings.layerName_ParceleNadela & "]) as A LEFT JOIN (select C.[idTable],bx1_,by1_,bx2_,by2_ FROM ((SELECT [ID],[X (I)] as bx1_,[Y (I)] as by1_,[idtable] FROM [Tacke] ) as C JOIN (SELECT [ID],[X (I)] as bx2_,[Y (I)] as by2_,[idtable] FROM [Tacke]) as D on C.[idtable]=D.[idTable] and C.[ID]<>D.[ID] and (C.by1_/C.bx1_)<(D.by2_/D.bx2_))) as B on A.[idtable]=B.[idtable]"
        qvr_.RunEx(True)

        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            'za svaku parcelu racunas ugao

            'Dim P_ = 
            'a sad za svaku parcelu?
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(k).DataText(1), qvr_.Table.RecordSet.Item(k).DataText(2))
            Dim nesto_ = qvr_.Table.RecordSet.Item(k).DataText(3)
            If nesto_ = "" Then
                nesto_ = "prazan"
            End If
            lblSets2_.Add(nesto_, pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets2_.LastAdded
            labb_.Size = 7
            Try
                labb_.Rotation = NiAnaB(qvr_.Table.RecordSet.Item(k).DataText(4), qvr_.Table.RecordSet.Item(k).DataText(5), qvr_.Table.RecordSet.Item(k).DataText(6), qvr_.Table.RecordSet.Item(k).DataText(7)) - 90 + 180
            Catch ex As Exception

            End Try
            pnt_ = Nothing
            pb1.Value = k
        Next
        lblSets2_ = Nothing

        'sada ides na label briskaza
        pb1.Value = 0
        If MsgBox("Da li radim Iskaze?", MsgBoxStyle.OkCancel, "Pitanje") = MsgBoxResult.Ok Then

            Dim labelbrIskaza_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Iskaz", drwParceleLinije.CoordinateSystem, True)
            labelbrIskaza_.LabelAlignX = LabelAlignX.LabelAlignXCenter : labelbrIskaza_.LabelAlignY = LabelAlignY.LabelAlignYCenter
            labelbrIskaza_.OptimizeLabelAlignX = False : labelbrIskaza_.OptimizeLabelAlignY = False
            labelbrIskaza_.ResolveOverlaps = False : labelbrIskaza_.PerLabelFormat = True
            Dim lblSets3_ As Manifold.Interop.LabelSet = labelbrIskaza_.LabelSet

            lbl_infoMain.Text = "Kreiram Label Islaz za svaku parcelu" : My.Application.DoEvents()

            qvr_.Text = "SELECT ax_,ay_,opis_,bx1_,by1_,bx2_,by2_ from (SELECT [X (I)] as ax_,[Y (I)] as ay_, [idVlasnika] as opis_, [idtable] FROM [" & My.Settings.layerName_ParceleNadela & "]) as A LEFT JOIN (select C.[idTable],bx1_,by1_,bx2_,by2_ FROM ((SELECT [ID],[X (I)] as bx1_,[Y (I)] as by1_,[idtable] FROM [Tacke] ) as C JOIN (SELECT [ID],[X (I)] as bx2_,[Y (I)] as by2_,[idtable] FROM [Tacke]) as D on C.[idtable]=D.[idTable] and C.[ID]<>D.[ID] and (C.by1_/C.bx1_)<(D.by2_/D.bx2_))) as B on A.[idtable]=B.[idtable]"
            qvr_.RunEx(True)

            pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

            For k = 0 To qvr_.Table.RecordSet.Count - 1
                'a sad za svaku parcelu?
                Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(k).DataText(1), qvr_.Table.RecordSet.Item(k).DataText(2))
                lblSets3_.Add(qvr_.Table.RecordSet.Item(k).DataText(3), pnt_)
                'sada treba nekako rotacija?
                Dim labb_ As Manifold.Interop.Label = lblSets3_.LastAdded
                labb_.Size = 5
                Try
                    labb_.Rotation = NiAnaB(qvr_.Table.RecordSet.Item(k).DataText(4), qvr_.Table.RecordSet.Item(k).DataText(5), qvr_.Table.RecordSet.Item(k).DataText(6), qvr_.Table.RecordSet.Item(k).DataText(7)) - 90 + 180
                Catch ex As Exception

                End Try
                pnt_ = Nothing
                pb1.Value = k
            Next
            lblSets3_ = Nothing
            pb1.Value = 0
            lbl_infoMain.Text = "Kreiram Label Parcele za svaku parcelu" : My.Application.DoEvents()
            'za parcele

            Dim labelbrParcele_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Parcele", drwParceleLinije.CoordinateSystem, True)
            labelbrParcele_.LabelAlignX = LabelAlignX.LabelAlignXCenter : labelbrParcele_.LabelAlignY = LabelAlignY.LabelAlignYCenter
            labelbrParcele_.OptimizeLabelAlignX = False : labelbrParcele_.OptimizeLabelAlignY = False
            labelbrParcele_.ResolveOverlaps = False : labelbrParcele_.PerLabelFormat = True
            Dim lblSets4_ As Manifold.Interop.LabelSet = labelbrParcele_.LabelSet

            lbl_infoMain.Text = "Kreiram Label Islaz za svaku parcelu" : My.Application.DoEvents()

            qvr_.Text = "SELECT ax_,ay_,opis_,bx1_,by1_,bx2_,by2_ from (SELECT [X (I)] as ax_,[Y (I)] as ay_, [brParcele] as opis_, [idtable] FROM [" & My.Settings.layerName_ParceleNadela & "]) as A LEFT JOIN (select C.[idTable],bx1_,by1_,bx2_,by2_ FROM ((SELECT [ID],[X (I)] as bx1_,[Y (I)] as by1_,[idtable] FROM [Tacke] ) as C JOIN (SELECT [ID],[X (I)] as bx2_,[Y (I)] as by2_,[idtable] FROM [Tacke]) as D on C.[idtable]=D.[idTable] and C.[ID]<>D.[ID] and (C.by1_/C.bx1_)<(D.by2_/D.bx2_))) as B on A.[idtable]=B.[idtable]"
            qvr_.RunEx(True)

            pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

            For k = 0 To qvr_.Table.RecordSet.Count - 1
                'a sad za svaku parcelu?
                Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(k).DataText(1), qvr_.Table.RecordSet.Item(k).DataText(2))
                lblSets4_.Add(qvr_.Table.RecordSet.Item(k).DataText(3), pnt_)
                'sada treba nekako rotacija?
                Dim labb_ As Manifold.Interop.Label = lblSets4_.LastAdded
                labb_.Size = 4
                Try
                    Dim ni_ = NiAnaB(qvr_.Table.RecordSet.Item(k).DataText(4), qvr_.Table.RecordSet.Item(k).DataText(5), qvr_.Table.RecordSet.Item(k).DataText(6), qvr_.Table.RecordSet.Item(k).DataText(7)) - 90 + 180
                    labb_.Rotation = ni_
                Catch ex As Exception

                End Try
                pnt_ = Nothing
                pb1.Value = k
            Next
            lblSets4_ = Nothing
            pb1.Value = 0
        End If


        newDoc.ComponentSet.Remove("tacke")
        newDoc.ComponentSet.Remove("update")
        newDoc.ComponentSet.Remove("analiza")
        newDoc.ComponentSet.Remove("poslednje_tacke")
        newDoc.Save()

        qvr_ = Nothing : qvrUpdate = Nothing
        qvrTacke = Nothing
        newDoc = Nothing
        MsgBox("Kraj ")
    End Sub

    Private Sub GradevnskiToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles GradevnskiToolStripMenuItem.Click

        'sada je ovde problem sa vlasnistvom kako! - jer imas broj parcele a nemas 
        'koji je vlasnik pa na osnovu toga moras da nades broj iskaza pa tek onda da formiras celu pricu!


        Try
            sf_diag.FileName = ""
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            'sf_diag.FileName = "nadela_tabla" & ddl_ttpSpisakTabli.SelectedValue & ".map"
            sf_diag.Title = "Upisite naziv za izlazni Map File - za STAMPU NADELE - PREGLEDNE TABLE"
            sf_diag.ShowDialog()
            If sf_diag.FileName = "" Then
                MsgBox("Kraj operacije")
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox("Dokument je read onlyu Zatvorite ga u Manifoldu i ponovo pokrenite ovu funkciju.")
            FileClose()
            Exit Sub
        End Try

        Dim manApp As Manifold.Interop.Application = New Manifold.Interop.Application
        Dim newDoc As Manifold.Interop.Document = manApp.NewDocument("", False)

        Try
            newDoc.SaveAs(sf_diag.FileName)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju")
            Exit Sub
        End Try

        'sada iz starog DKP_Nadela kopiras u novi map file

        Dim docOld As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim drwNadelaOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_ParceleNadela)
        Dim drwNadelaNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_ParceleNadela, drwNadelaOld.CoordinateSystem, True)

        drwNadelaOld.Copy(False)
        drwNadelaNew.Paste(True)
        drwNadelaOld.SelectNone()
        drwNadelaNew.SelectNone()

        Dim drwTackeOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_nadelaSmer)
        Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing(My.Settings.layerName_nadelaSmer, drwNadelaOld.CoordinateSystem, True)

        drwTackeOld.Copy(False)
        drwTackeNew.Paste(True)
        drwTackeOld.SelectNone()
        drwTackeNew.SelectNone()

        newDoc.Save()



        'sada pravis linije za export

        Dim drwParceleLinije As Manifold.Interop.Drawing
        Try
            drwParceleLinije = newDoc.NewDrawing("DKP_Nadela_Linije", drwNadelaNew.CoordinateSystem, True)
        Catch ex As Exception
            newDoc.ComponentSet.Remove("DKP_Nadela_Linije")
            drwParceleLinije = newDoc.NewDrawing("DKP_Nadela_Linije", drwNadelaNew.CoordinateSystem, True)
        End Try

        Dim analizer_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
        analizer_.Boundaries(drwNadelaNew, drwNadelaNew, drwNadelaNew.ObjectSet)

        drwNadelaNew.Cut(True)
        drwParceleLinije.Paste(True)

        analizer_.Explode(drwParceleLinije, drwParceleLinije.ObjectSet)
        analizer_.RemoveDuplicates(drwParceleLinije, drwParceleLinije.ObjectSet)

        analizer_ = Nothing

        'sada polje sa duzinom

        'Frontovi

        Dim tbl_ As Manifold.Interop.Table
        tbl_ = drwParceleLinije.OwnedTable
        Dim col_ As Manifold.Interop.Column = newDoc.Application.NewColumnSet.NewColumn
        col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)


        Dim qvr_ As Manifold.Interop.Query = newDoc.NewQuery("analiza")
        qvr_.Text = "update [DKP_Nadela_Linije] set [Duzina]=round([Length (I)],2), [Ugao]=round([Bearing (I)])"
        qvr_.RunEx(True)

        newDoc.Save()

        'sada formiras label

        'kreiras tacke i sa ova dva polja treba da napravis label - odnosno duzina ti je label a ugao ti je rotacija! - samo se postavlja pitanje kako pojedinacnu da napravis
        qvr_.Text = "select centroidx(LinePoint([Geom (I)], [Length (I)]/2)),centroidy(LinePoint([Geom (I)], [Length (I)]/2)),[Ugao],[Duzina] from [DKP_Nadela_Linije]"
        qvr_.RunEx(True)

        Dim drwLabelFront As Manifold.Interop.Labels = newDoc.NewLabels("label_front", drwParceleLinije.CoordinateSystem, True)
        drwLabelFront.LabelAlignX = LabelAlignX.LabelAlignXLeft : drwLabelFront.LabelAlignY = LabelAlignY.LabelAlignYTop
        drwLabelFront.OptimizeLabelAlignX = False : drwLabelFront.OptimizeLabelAlignY = False
        drwLabelFront.ResolveOverlaps = False : drwLabelFront.PerLabelFormat = True

        Dim lblSets_ As Manifold.Interop.LabelSet = drwLabelFront.LabelSet
        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        lbl_infoMain.Text = "Kreiranje Label-a za frontove"
        My.Application.DoEvents()

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            'e sada da vidimo kreiras tacku i onda ide dalje
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(i).DataText(1), qvr_.Table.RecordSet.Item(i).DataText(2))
            lblSets_.Add(qvr_.Table.RecordSet.Item(i).DataText(4), pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets_.LastAdded
            labb_.Rotation = qvr_.Table.RecordSet.Item(i).DataText(3) - 90
            labb_.Size = 3.75
            pnt_ = Nothing
        Next
        newDoc.Save()
        pb1.Value = 0

        'sada ide druga prica - znaci u nekom polju moras da imas broj parcele: brParcele!
        tbl_ = drwNadelaNew.OwnedTable
        col_.Name = "Opis1" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        'col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)


        qvr_.Text = "select [brParcele],[ID] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim myCommand As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()

        Dim qvrInside_ As Manifold.Interop.Query = newDoc.NewQuery("update_")

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            'sada ides na server!
            'myCommand.CommandText = "SELECT CONVERT(GROUP_concat(vlasnik_ SEPARATOR ';') USING utf8) as vlasnik_ FROM (SELECT idVlasnika, concat(PREZIME,if(isnull(IMEOCA),' ',concat(' (',imeoca,') ')),ime,if(isnull(mesto),' ', concat(' ',mesto,',')),if(isnull(ulica),' ',concat(CONVERT(' ul ' USING utf8),ulica,' ')),if(isnull(broj),' ',concat(' kbr ',broj,' ')),if(isnull(uzbroj),'',uzbroj)) as vlasnik_ FROM kom_vlasnik where idVlasnika IN (SELECT distinct idVlasnika FROM kom_vezaparcelavlasnik where idParcele IN (SELECT idParc FROM kom_parcele where brParceleF='" & qvr_.Table.RecordSet(i).DataText(1) & "' and DEOPARCELE=0 and obrisan=0 ))) as E"
            myCommand.CommandText = "SELECT CONVERT(GROUP_concat(vlasnici_ SEPARATOR ';') USING utf8) as vlasnik_ FROM (SELECT if(A.udeo=" & Chr(34) & "1/1" & Chr(34) & ",B.vlasnik_,concat(A.udeo,' ',B.vlasnik_)) as vlasnici_ FROM ((SELECT idvlasnika,udeo FROM kom_vezaparcelavlasnik WHERE obrisan=0 and idParcele IN (SELECT idParc FROM kom_parcele where brParceleF='" & qvr_.Table.RecordSet(i).DataText(1) & "' and DEOPARCELE=0 and obrisan=0) ) as A LEFT JOIN (SELECT idVlasnika, concat(PREZIME,if(isnull(IMEOCA),' ',concat(' (',imeoca,') ')),ime,if(isnull(mesto),' ', concat(' ',mesto,',')),if(isnull(ulica),' ',concat(CONVERT(' ul ' USING utf8),ulica,' ')),if(isnull(broj),' ',concat(' kbr ',broj,' ')),if(isnull(uzbroj),'',uzbroj)) as vlasnik_ FROM kom_vlasnik ) as B on A.idvlasnika=B.idVlasnika ) )as E"
            Dim myreader_ As MySql.Data.MySqlClient.MySqlDataReader = myCommand.ExecuteReader

            If myreader_.HasRows = True Then
                myreader_.Read()
                qvrInside_.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [Opis1]=" & Chr(34) & myreader_.GetValue(0) & Chr(34) & " where [id]=" & qvr_.Table.RecordSet.Item(i).DataText(2)
                qvrInside_.RunEx(True)

            Else
                'nije nista selektovano

            End If

            myreader_.Close()
            myreader_ = Nothing

        Next
        conn_.Close()
        'sada ides na isertovanje!
        qvr_.Text = "select [opis1],[ID] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        Dim labelLinije_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Vlasnik", drwParceleLinije.CoordinateSystem, True)
        labelLinije_.LabelAlignX = LabelAlignX.LabelAlignXLeft : labelLinije_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelLinije_.OptimizeLabelAlignX = False : labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False : labelLinije_.PerLabelFormat = True

        Dim lblSets2_ As Manifold.Interop.LabelSet = labelLinije_.LabelSet

        lbl_infoMain.Text = "Kreiram Label Opis za svaku parcelu" : My.Application.DoEvents()

        qvr_.Text = "SELECT ax_,ay_,opis_,bx1_,by1_,bx2_,by2_ from (SELECT [X (I)] as ax_,[Y (I)] as ay_, [opis1] as opis_, [idtable] FROM [" & My.Settings.layerName_ParceleNadela & "]) as A LEFT JOIN (select C.[idTable],bx1_,by1_,bx2_,by2_ FROM ((SELECT [ID],[X (I)] as bx1_,[Y (I)] as by1_,[idtable] FROM [Tacke] ) as C JOIN (SELECT [ID],[X (I)] as bx2_,[Y (I)] as by2_,[idtable] FROM [Tacke]) as D on C.[idtable]=D.[idTable] and C.[ID]<>D.[ID] and (C.by1_/C.bx1_)<(D.by2_/D.bx2_))) as B on A.[idtable]=B.[idtable]"
        qvr_.RunEx(True)

        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            'za svaku parcelu racunas ugao

            'Dim P_ = 
            'a sad za svaku parcelu?
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(k).DataText(1), qvr_.Table.RecordSet.Item(k).DataText(2))
            Dim nesto_ = qvr_.Table.RecordSet.Item(k).DataText(3)
            If nesto_ = "" Then
                nesto_ = "prazan"
            End If
            lblSets2_.Add(nesto_, pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets2_.LastAdded
            labb_.Size = 7
            Try
                labb_.Rotation = NiAnaB(qvr_.Table.RecordSet.Item(k).DataText(4), qvr_.Table.RecordSet.Item(k).DataText(5), qvr_.Table.RecordSet.Item(k).DataText(6), qvr_.Table.RecordSet.Item(k).DataText(7)) - 90 + 180
            Catch ex As Exception

            End Try
            pnt_ = Nothing
            pb1.Value = k
        Next
        conn_.Close()
        newDoc.Save()

        'kreiras broj iskaza

        tbl_ = drwNadelaNew.OwnedTable
        col_.Name = "idIskaza" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)
        'col_.Name = "Ugao" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : tbl_.ColumnSet.Add(col_) ': tbl1_.ColumnSet.Add(col_)

        qvr_.Text = "select [brParcele],[ID] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        conn_.Open()

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            'sada ides na server!
            myCommand.CommandText = "SELECT idiskazzemljista from kom_vezaparcelavlasnik where obrisan=0 and idParcele IN (SELECT idParc FROM kom_parcele where brParceleF='" & qvr_.Table.RecordSet(i).DataText(1) & "' and DEOPARCELE=0 and obrisan=0)"
            Dim myreader_ As MySql.Data.MySqlClient.MySqlDataReader = myCommand.ExecuteReader

            If myreader_.HasRows = True Then
                myreader_.Read()
                qvrInside_.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [idIskaza]=" & myreader_.GetValue(0) & " where [id]=" & qvr_.Table.RecordSet.Item(i).DataText(2)
                qvrInside_.RunEx(True)

            Else
                'nije nista selektovano

            End If

            myreader_.Close()
            myreader_ = Nothing

        Next
        conn_.Close()

        'sada ides na isertovanje!
        qvr_.Text = "select [idIskaza],[ID] from [" & My.Settings.layerName_ParceleNadela & "]"
        qvr_.RunEx(True)

        Dim labelidIskaza_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Iskaz", drwParceleLinije.CoordinateSystem, True)
        labelidIskaza_.LabelAlignX = LabelAlignX.LabelAlignXLeft : labelidIskaza_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelidIskaza_.OptimizeLabelAlignX = False : labelidIskaza_.OptimizeLabelAlignY = False
        labelidIskaza_.ResolveOverlaps = False : labelidIskaza_.PerLabelFormat = True

        Dim lblSets3_ As Manifold.Interop.LabelSet = labelidIskaza_.LabelSet

        lbl_infoMain.Text = "Kreiram Label Iskaza za svaku parcelu" : My.Application.DoEvents()

        qvr_.Text = "SELECT ax_,ay_,opis_,bx1_,by1_,bx2_,by2_ from (SELECT [X (I)] as ax_,[Y (I)] as ay_, [idIskaza] as opis_, [idtable] FROM [" & My.Settings.layerName_ParceleNadela & "]) as A LEFT JOIN (select C.[idTable],bx1_,by1_,bx2_,by2_ FROM ((SELECT [ID],[X (I)] as bx1_,[Y (I)] as by1_,[idtable] FROM [Tacke] ) as C JOIN (SELECT [ID],[X (I)] as bx2_,[Y (I)] as by2_,[idtable] FROM [Tacke]) as D on C.[idtable]=D.[idTable] and C.[ID]<>D.[ID] and (C.by1_/C.bx1_)<(D.by2_/D.bx2_))) as B on A.[idtable]=B.[idtable]"
        qvr_.RunEx(True)

        pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            'za svaku parcelu racunas ugao

            'Dim P_ = 
            'a sad za svaku parcelu?
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvr_.Table.RecordSet.Item(k).DataText(1), qvr_.Table.RecordSet.Item(k).DataText(2))
            Dim nesto_ = qvr_.Table.RecordSet.Item(k).DataText(3)
            If nesto_ = "" Then
                nesto_ = "prazan"
            End If
            lblSets3_.Add(nesto_, pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets3_.LastAdded
            labb_.Size = 3
            Try
                labb_.Rotation = NiAnaB(qvr_.Table.RecordSet.Item(k).DataText(4), qvr_.Table.RecordSet.Item(k).DataText(5), qvr_.Table.RecordSet.Item(k).DataText(6), qvr_.Table.RecordSet.Item(k).DataText(7)) - 90 + 180
            Catch ex As Exception

            End Try
            pnt_ = Nothing
            pb1.Value = k
        Next

        newDoc.Save()

        'sada ides na label briskaza
        pb1.Value = 0

        conn_ = Nothing
        myCommand = Nothing

        drwTackeOld = Nothing : drwTackeNew = Nothing
        drwTackeOld = Nothing
        drwTackeNew = Nothing
        analizer_ = Nothing

        newDoc = Nothing
        docOld = Nothing
        'sada ostaje null da se dodeli
        lbl_infoMain.Text = ""
        MsgBox("Kraj")
    End Sub

    Private Sub OpisPoložajaPoligonskihTačakaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles OpisPoložajaPoligonskihTačakaToolStripMenuItem.Click
        'kreiras jedan drawinga :  linije
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim drwDetaljneTacke As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Dim drwPoligonskeTacke As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_poligonskeTacke)
        Dim drwLinOpisPolozaja As Manifold.Interop.Drawing '= doc.NewDrawing("drwOpisPolozaja")

        'drawing linije opisa
        Try
            doc.ComponentSet.Remove("linOpisPolozaja")
            drwLinOpisPolozaja = doc.NewDrawing("drwOpisPolozaja", drwDetaljneTacke.CoordinateSystem)
        Catch ex As Exception
            drwLinOpisPolozaja = doc.NewDrawing("drwOpisPolozaja", drwDetaljneTacke.CoordinateSystem)
        End Try

        Dim tbl_ As Manifold.Interop.Table = drwLinOpisPolozaja.OwnedTable
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "poligonska"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_.Name = "detaljna"
        tbl_.ColumnSet.Add(col_)
        col_.Name = "Rastojanje"
        tbl_.ColumnSet.Add(col_)

        'sada imas kreiran drawing aj sad query!
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("insertLines")
        If My.Settings.poligonske_brojOdmeranjaOpis = -1 Then
            qvr_.Text = "INSERT INTO [" & drwLinOpisPolozaja.Name & "] ([Geom (I)],[poligonska],[detaljna],[Rastojanje]) (SELECT lin_,pol_,det_,FormatNumber(d_,2,0,0,0) FROM (SELECT pol_,det_,d_,AssignCoordSys(lin_,COORDSYS(" & Chr(34) & drwDetaljneTacke.Name & Chr(34) & " as COMPONENT)) as lin_ FROM (SELECT t1.pol_,t1.det_,t1.d_,t1.lin_, Count(T2.[pol_]) as N FROM (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brt] as pol_,[" & My.Settings.layerName_pointTableObelezavanje & "].[idTacke] as det_,Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) as D_, NewLine([" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)],[" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)]) as Lin_ FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.poligonske_sirinaBaferZone & " ORDER by [" & My.Settings.layerName_poligonskeTacke & "].[brt],Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) asc ) as T1 LEFT join (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brt] as pol_,[" & My.Settings.layerName_pointTableObelezavanje & "].[idTacke] as det_,Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) as D_, NewLine([" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)],[" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)]) as Lin_ FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.poligonske_sirinaBaferZone & " ORDER by [" & My.Settings.layerName_poligonskeTacke & "].[brt],Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) asc ) as T2 on T1.[pol_]=T2.[Pol_] and T1.[D_]>T2.[D_] GROUP by t1.pol_,t1.det_,t1.D_,t1.lin_ ) as A ORDER by A.pol_,A.N ) as B )"
        Else
            qvr_.Text = "INSERT INTO [" & drwLinOpisPolozaja.Name & "] ([Geom (I)],[poligonska],[detaljna],[Rastojanje]) (SELECT lin_,pol_,det_,FormatNumber(d_,2,0,0,0) FROM (SELECT pol_,det_,d_,AssignCoordSys(lin_,COORDSYS(" & Chr(34) & drwDetaljneTacke.Name & Chr(34) & " as COMPONENT)) as lin_ FROM (SELECT t1.pol_,t1.det_,t1.d_,t1.lin_, Count(T2.[pol_]) as N FROM (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brt] as pol_,[" & My.Settings.layerName_pointTableObelezavanje & "].[idTacke] as det_,Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) as D_, NewLine([" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)],[" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)]) as Lin_ FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.poligonske_sirinaBaferZone & " ORDER by [" & My.Settings.layerName_poligonskeTacke & "].[brt],Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) asc ) as T1 LEFT join (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brt] as pol_,[" & My.Settings.layerName_pointTableObelezavanje & "].[idTacke] as det_,Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) as D_, NewLine([" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)],[" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)]) as Lin_ FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.poligonske_sirinaBaferZone & " ORDER by [" & My.Settings.layerName_poligonskeTacke & "].[brt],Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) asc ) as T2 on T1.[pol_]=T2.[Pol_] and T1.[D_]>T2.[D_] GROUP by t1.pol_,t1.det_,t1.D_,t1.lin_ ) as A WHERE N<=" & My.Settings.poligonske_brojOdmeranjaOpis & " ORDER by A.pol_,A.N ) as B )"
        End If

        qvr_.RunEx(True)
        doc.ComponentSet.Remove("insertLines")

        'sada imas linije mozes da ides na label - opis


        tbl_ = Nothing : col_ = Nothing
        drwDetaljneTacke = Nothing : drwLinOpisPolozaja = Nothing : drwPoligonskeTacke = Nothing
        qvr_ = Nothing : doc.Save() : doc = Nothing

        MsgBox("Kraj")
    End Sub

    Private Sub ProcesiranjeToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ProcesiranjeToolStripMenuItem.Click

        'txtQuery.Text = "Pocetak :" & Now() & vbNewLine & "Potrebni layer(i):" & vbNewLine & " -Poligonske tacke (DWG Poligonske tacke);" & vbNewLine & " -Detaljne tacke (DWG Tacke Obelezanja)" & vbNewLine & "  - Prepreke (DWG Ulice)"
        txt_Query.DocumentText = "Pocetak :" & Now() & vbNewLine & "Potrebni layer(i):" & vbNewLine & " -Poligonske tacke (DWG Poligonske tacke);" & vbNewLine & " -Detaljne tacke (DWG Tacke Obelezanja)" & vbNewLine & "  - Prepreke (DWG Ulice)"
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        'OPTIONS COORDSYS("Objedinjeno" as COMPONENT); SELECT [Poligona TACKE].[brTacke],[Objedinjeno].[ID],(Distance([Poligona TACKE].[ID],[Objedinjeno].[ID])) as D,NewLine([objedinjeno].[Geom (I)],[Poligona TACKE].[Geom (I)]) as line_ FROM [Poligona TACKE],[Objedinjeno] WHERE Distance([Poligona TACKE].[ID],[Objedinjeno].[ID])<300
        Dim q01 As Manifold.Interop.Query = doc.NewQuery("01")
        Dim drwDetaljne_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Dim drwNewTemp As Manifold.Interop.Drawing
        Try
            'moras da ga obrises ako postoji!
            doc.ComponentSet.Remove("01_Pravci")
            drwNewTemp = doc.NewDrawing("01_Pravci", drwDetaljne_.CoordinateSystem, True)
        Catch ex As Exception
            drwNewTemp = doc.NewDrawing("01_Pravci", drwDetaljne_.CoordinateSystem, True)
        End Try

        lbl_infoMain.Text = "Kreiram 01_Pravci" : My.Application.DoEvents()

        'txtQuery.Text = txtQuery.Text & vbNewLine & "Kreiranje polja"
        txt_Query.DocumentText += vbNewLine & "Kreiranje polja"
        'kreiras polja!
        Dim tbl_ As Manifold.Interop.Table = drwNewTemp.OwnedTable
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "PoligonskaIDTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_.Name = "DetaljnaID"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "Rastojanje"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        tbl_.ColumnSet.Add(col_)

        lbl_infoMain.Text = "Kreiram i upisujem pravce u 01_Pravci" : My.Application.DoEvents()
        'sada u njega ubacujes pravce sa tacaka
        pb1.Value = 0
        pb1.Maximum = 3
        q01.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_pointTableObelezavanje & Chr(34) & " as COMPONENT); insert into [01_Pravci] (PoligonskaIDTacke,DetaljnaID,Rastojanje,[Geom (I)]) (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brTacke],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID],(Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])) as D,NewLine([" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)],[" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)]) as line_ FROM [" & My.Settings.layerName_poligonskeTacke & "],[" & My.Settings.layerName_pointTableObelezavanje & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.tahimetrija_sirinaBaferZone & ")"
        q01.RunEx(True)

        pb1.Value = 1
        lbl_infoMain.Text = "Brisem pravce koji imaju presek" : My.Application.DoEvents()
        'sada ide pronalazenje preseka pravaca sa preprekama 
        q01.Text = "delete from [01_Pravci] where [ID] in (SELECT [01_Pravci].[ID] FROM [01_Pravci],[" & My.Settings.layerName_Ulice & "] WHERE Intersects([01_Pravci].[Geom (I)],[Prepreke].[Geom (I)]))" : q01.RunEx(True)
        pb1.Value = 2

        lbl_infoMain.Text = "Upisujem tacke mreze u bazu" : My.Application.DoEvents()
        q01.Text = "UPDATE (SELECT * FROM (SELECT A.[DetaljnaID],B.[PoligonskaIDTacke] FROM (SELECT [DetaljnaID],min(Rastojanje) as Rastojanje FROM [01_Pravci] GROUP by [DetaljnaID]) as A LEFT JOIN (SELECT [DetaljnaID],Rastojanje,[PoligonskaIDTacke] FROM 	[01_Pravci]) as B on A.[DetaljnaID]=B.[DetaljnaID] and A.Rastojanje=B.Rastojanje )) as C LEFT join (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[brTacke] FROM [" & My.Settings.layerName_pointTableObelezavanje & "]) as D on C.[DetaljnaID]=D.[ID] ) set brTacke=PoligonskaIDTacke" : q01.RunEx(True)

        pb1.Value = 3

        doc.Save()

        drwNewTemp = Nothing
        doc.ComponentSet.Remove("01")
        tbl_ = Nothing : q01 = Nothing : col_ = Nothing : doc = Nothing

        MsgBox("Kraj")
        pb1.Value = 0
    End Sub

    Private Sub NumerisanjeDetaljnihTačakaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles NumerisanjeDetaljnihTačakaToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim qvrMax_ As Manifold.Interop.Query
        Dim qvrUpdateNumber As Manifold.Interop.Query
        Dim qvrPoligone As Manifold.Interop.Query

        Try
            doc.ComponentSet.Remove("max_")
            qvrMax_ = doc.NewQuery("max_", True)
        Catch ex As Exception
            qvrMax_ = doc.NewQuery("max_", True)
        End Try

        Try
            doc.ComponentSet.Remove("updateNumber")
            qvrUpdateNumber = doc.NewQuery("updateNumber", True)
        Catch ex As Exception
            qvrUpdateNumber = doc.NewQuery("updateNumber")
        End Try

        Try
            doc.ComponentSet.Remove("listaPoligonih")
            qvrPoligone = doc.NewQuery("listaPoligonih", True)
        Catch ex As Exception
            qvrPoligone = doc.NewQuery("listaPoligonih", True)
        End Try
        Dim brojacPoligone_ As Integer = 0
        'sada ides za svaki poligonu posebno!
        qvrPoligone.Text = "SELECT [brTacke] FROM [" & My.Settings.layerName_poligonskeTacke & "] WHERE [tip] = 1 ORDER by [Y (I)] desc,[X (I)]"
        qvrPoligone.RunEx(True)
        pb1.Maximum = qvrPoligone.Table.RecordSet.Count
        pb1.Value = 0
        For i = 0 To qvrPoligone.Table.RecordSet.Count - 1
            'sada ides prvo pronalazenje pa onda 
            qvrUpdateNumber.Text = "UPDATE (select [BrDetaljne],(RedniBrojTacke_+" & brojacPoligone_ & ") as RedniBrojTacke_  FROM (SELECT A.id_,A.[BrDetaljne],B.* FROM (select NewLine([" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)],[" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)]) as line_,[" & My.Settings.layerName_pointTableObelezavanje & "].[ID] as id_,[" & My.Settings.layerName_pointTableObelezavanje & "].[BrDetaljne] FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] 	WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[brPoligonske]=brojPoligonskeTacke_ and [" & My.Settings.layerName_poligonskeTacke & "].brtacke=brojPoligonskeTacke_ ) as A INNER JOIN (SELECT [ID],[Geom (I)],[Bearing (I)] FROM [01_Pravci]) as B on A.line_=B.[Geom (I)] ORDER by B.[Bearing (I)] ) as G1 LEFT JOIN (SELECT (SELECT count(*) as RedniBrojTacke_ from (SELECT A.id_,A.[BrDetaljne],B.* FROM (select NewLine([" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)],[" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)]) as line_,[" & My.Settings.layerName_pointTableObelezavanje & "].[ID] as id_,[" & My.Settings.layerName_pointTableObelezavanje & "].[BrDetaljne] FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[brPoligonske]=brojPoligonskeTacke_ and [" & My.Settings.layerName_poligonskeTacke & "].brtacke=brojPoligonskeTacke_ ) as A INNER JOIN (SELECT [ID],[Geom (I)],[Bearing (I)] FROM [01_Pravci]) as B on A.line_=B.[Geom (I)] ORDER by B.[Bearing (I)] ) as Q2 where Q2.[Bearing (I)] <= T.[Bearing (I)]) , [Bearing (I)] from  (SELECT A.id_,A.[BrDetaljne],B.* FROM (select NewLine([" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)],[" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)]) as line_,[" & My.Settings.layerName_pointTableObelezavanje & "].[ID] as id_,[" & My.Settings.layerName_pointTableObelezavanje & "].[BrDetaljne] FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_poligonskeTacke & "] WHERE[" & My.Settings.layerName_pointTableObelezavanje & "].[brPoligonske]=brojPoligonskeTacke_ and [" & My.Settings.layerName_poligonskeTacke & "].brtacke=brojPoligonskeTacke_ ) as A INNER JOIN (SELECT [ID],[Geom (I)],[Bearing (I)] FROM [01_Pravci]) as B on A.line_=B.[Geom (I)] ORDER by B.[Bearing (I)] )  as T  ORDER BY [Bearing (I)]) as G2 ON G1.[Bearing (I)]=G2.[Bearing (I)] )  set [BrDetaljne]=RedniBrojTacke_"
            qvrUpdateNumber.RunEx(True)

            'sada 
            qvrMax_.Text = "select Max([BrDetaljne]) from [" & My.Settings.layerName_pointTableObelezavanje & "]"
            qvrMax_.RunEx(True)

            brojacPoligone_ = qvrMax_.Table.RecordSet.Item(0).DataText(1)

            pb1.Value = i
        Next
        'sada mozesd da ubijes sve ove

        doc.Save()

        doc = Nothing
    End Sub

    Private Sub ZapisnikToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ZapisnikToolStripMenuItem.Click
        'ovde ti treba tacka sa koje se startuje! - kako sledecu da nade? pitanje
        'txtQuery.Text = "Pocetak: " & Now()
        txt_Query.DocumentText = "Pocetak: " & Now()
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        'Dim freefile_ As Integer = FreeFile()

        'FileOpen(freefile_, "c:\redosed.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        Dim qvrPoligone As Manifold.Interop.Query
        Dim qvrDetaljne As Manifold.Interop.Query
        Dim qvrSlepe As Manifold.Interop.Query
        Dim qvrSlepeLevel1 As Manifold.Interop.Query
        Dim qvrUpdate As Manifold.Interop.Query
        Dim qvr_ As Manifold.Interop.Query
        'prvo ih obrises query!
        Try
            doc.ComponentSet.Remove("odrediRasporedPoligonih")
            qvrPoligone = doc.NewQuery("odrediRasporedPoligonih", True)
        Catch ex As Exception
            qvrPoligone = doc.NewQuery("odrediRasporedPoligonih", True)
        End Try

        Try
            doc.ComponentSet.Remove("odrediDetaljne")
            qvrDetaljne = doc.NewQuery("odrediDetaljne", True)
        Catch ex As Exception
            qvrDetaljne = doc.NewQuery("odrediDetaljne", True)
        End Try

        Try
            doc.ComponentSet.Remove("odrediSlepe")
            qvrSlepe = doc.NewQuery("odrediSlepe", True)
        Catch ex As Exception
            qvrSlepe = doc.NewQuery("odrediSlepe", True)
        End Try

        Try
            doc.ComponentSet.Remove("odrediSlepeLevel1")
            qvrSlepeLevel1 = doc.NewQuery("odrediSlepeLevel1", True)
        Catch ex As Exception
            qvrSlepeLevel1 = doc.NewQuery("odrediSlepeLevel1", True)
        End Try

        Try
            doc.ComponentSet.Remove("updatePredeno")
            qvrUpdate = doc.NewQuery("updatePredeno", True)
        Catch ex As Exception
            qvrUpdate = doc.NewQuery("updatePredeno", True)
        End Try

        Try
            doc.ComponentSet.Remove("ZaZapisnik")
            qvr_ = doc.NewQuery("ZaZapisnik")
        Catch ex As Exception
            qvr_ = doc.NewQuery("ZaZapisnik")
        End Try

        qvrUpdate.Text = "update [Poligona TACKE] set [tahimetrija]=0"
        qvrUpdate.RunEx(True)



        opf_diag.FileName = ""
        opf_diag.Filter = "Excel File|*.xls"
        opf_diag.Title = "Pronadite tempalate zapisnika"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        'sada otvoris excel !

        Dim xlsApp_ As Microsoft.Office.Interop.Excel.Application = New Microsoft.Office.Interop.Excel.Application
        Dim xlsWB_ As Microsoft.Office.Interop.Excel.Workbook
        xlsApp_.Visible = True
        'xlsWB_ = xlsApp_.Workbooks.Open("D:\Adorjan\Tahimetrija\Tahimetrija_template.xls")
        xlsWB_ = xlsApp_.Workbooks.Open(opf_diag.FileName)
        'MsgBox(xlsWB_.Sheets.Count)

        Dim xlsSheet As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.ActiveSheet

        qvrPoligone.Text = "SELECT [brTacke] FROM [Poligona TACKE] WHERE [tip] = 1 ORDER by [Y (I)] desc,[X (I)]"
        qvrPoligone.RunEx(True)

        pb1.Value = 0
        pb1.Maximum = qvrPoligone.Table.RecordSet.Count

        Dim brojacExcel_ As Integer = 5
        Dim visinaStanice As Double = -1

        For i = 0 To qvrPoligone.Table.RecordSet.Count - 1
            'txtQuery.Text = txtQuery.Text & vbNewLine & "     Poligonska: " & qvrPoligone.Table.RecordSet.Item(i).DataText(1)
            txt_Query.DocumentText += vbNewLine & "     Poligonska: " & qvrPoligone.Table.RecordSet.Item(i).DataText(1)
            'sada prvo utvrdis da li ima detaljnih tacaka snimljene sa ove tacke

            'qvr_.Text = "(select CStr([brdetaljne]) as brt,[X (I)] as Y_, [Y (I)] as X_, [Visina] as H_,3 as redosled, [brdetaljne] as brt1 from [Objedinjeno] WHERE  [brPoligonske]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " ) order by redosled,brt1"
            qvr_.Text = "SELECT * FROM (select CStr([brdetaljne]) as brt,[X (I)] as Y_, [Y (I)] as X_, [Visina] as H_,3 as redosled, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " ) UNION (SELECT CStr([Poligona TACKE].[brTacke]) as brt,[Poligona TACKE].[X (I)] as Y_,[Poligona TACKE].[Y (I)] as X_,[Poligona TACKE].[VISINA] as H_,2 as redosled,[Poligona TACKE].[brTacke] as brt1 	FROM [Poligona TACKE], (SELECT [Poligoni vlak Drawing].[Geom (I)] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE Contains([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID]) AND 	[Poligona TACKE].[brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " ) as A  WHERE Contains(A.[Geom (I)],[Poligona TACKE].[ID]) AND [tip]=2) order by redosled,brt1"
            qvr_.RunEx(True)

            'ako ima onda mozes dalje ako 
            If qvr_.Table.RecordSet.Count > 0 Then
                'pises u zapisnik!
                If brojacExcel_ > 5 Then
                    brojacExcel_ += My.Settings.tahimetrija_razmakIzmeduRedova
                End If
                visinaStanice = Math.Floor(185 - Rnd() * 15) / 100  'zameniti deo sql-a floor(185-Rnd*15)
                'qvr_.Text = "SELECT brtPol,brtViz," & visinaStanice & ", floor(ugaoHorizontalno) as stepenH, floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60) as minutH, floor((((ugaoHorizontalno-floor(ugaoHorizontalno))*60)-(floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60)))*60) as sekundH, floor(UgaoZenitno) as stepenZ, floor((UgaoZenitno-floor(UgaoZenitno) )*60 ) as minutZ, floor((((UgaoZenitno-floor(UgaoZenitno) )*60)-(floor((UgaoZenitno-floor(UgaoZenitno) )*60)))*60) as sekundZ, Dhor,Dred,visraz_,vissign,yviz_,xviz_,hviz_,brt1 FROM (SELECT top 2 brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,1 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & "), (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([Poligona TACKE].[brtacke])) as brtViz,[Poligona TACKE].[X (I)] as Yviz_,[Poligona TACKE].[Y (I)] as Xviz_,[Poligona TACKE].[Visina] as Hviz_,1.5 as visSign, [Poligona TACKE].[brTacke] as brt1 FROM [Poligona TACKE],(SELECT [Poligoni vlak Drawing].[Geom (I)],[Poligona TACKE].[brTacke] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brtacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " and [tip]=1 and Touches([Poligona TACKE].[ID],[Poligoni vlak Drawing].[ID])) as A WHERE Touches(A.[Geom (I)],[Poligona TACKE].[ID]) and A.[brTacke]<>[Poligona TACKE].[brTacke] and [tip]=1 )) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,2 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & "), (select CStr([brdetaljne]) as brtViz,[X (I)] as Yviz_, [Y (I)] as Xviz_, [Visina] as Hviz_, 1.5 as visSign, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " )) ORDER by tip_,brt1)"
                qvr_.Text = "SELECT brtPol,brtViz," & visinaStanice & ", floor(ugaoHorizontalno) as stepenH, floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60) as minutH, floor((((ugaoHorizontalno-floor(ugaoHorizontalno))*60)-(floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60)))*60) as sekundH, floor(UgaoZenitno) as stepenZ, floor((UgaoZenitno-floor(UgaoZenitno) )*60 ) as minutZ, floor((((UgaoZenitno-floor(UgaoZenitno) )*60)-(floor((UgaoZenitno-floor(UgaoZenitno) )*60)))*60) as sekundZ, Dhor,Dred,visraz_,vissign,yviz_,xviz_,hviz_,brt1 FROM (SELECT top 2 brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,1 as tip_  FROM  (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & "), (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([Poligona TACKE].[brtacke])) as brtViz,[Poligona TACKE].[X (I)] as Yviz_,[Poligona TACKE].[Y (I)] as Xviz_,[Poligona TACKE].[Visina] as Hviz_,1.5 as visSign, [Poligona TACKE].[brTacke] as brt1 FROM [Poligona TACKE], (SELECT [Poligoni vlak Drawing].[Geom (I)],[Poligona TACKE].[brTacke] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brtacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " and [tip]=1 and Touches([Poligona TACKE].[ID],[Poligoni vlak Drawing].[ID]) ) as A WHERE Touches(A.[Geom (I)],[Poligona TACKE].[ID]) and A.[brTacke]<>[Poligona TACKE].[brTacke] and [tip]=1 )) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,2 as tip_  FROM (SELECT " & Chr(34) & "P" & Chr(34) & " & cstr(A.brTacke) as brtPol,xpol_,ypol_,Hpol_," & visinaStanice & " as visstan_,cstr(" & Chr(34) & "P" & Chr(34) & " & brtViz) as brtViz,Xviz_,Yviz_,Hviz_,visSign,brtViz as brt1 FROM ((SELECT [Poligona TACKE].[brTacke], [Poligona TACKE].[X (I)] as xpol_, [Poligona TACKE].[Y (I)] as ypol_,[Poligona TACKE].[VISINA] as Hpol_ ,[Poligoni vlak Drawing].[ID] as vl1id_  FROM [Poligona TACKE],[Poligoni vlak Drawing]  WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " and [tahimetrija]=0 and Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID])  ORDER by [Poligoni vlak Drawing].[Bearing (I)] ) as A LEFT join (SELECT [Poligoni vlak Drawing].[ID] as vl2id_,[brTacke] as brtViz,[Poligona TACKE] .[X (I)] as Xviz_,[Poligona TACKE] .[Y (I)] as Yviz_,[Visina] as Hviz_,1.5 as visSign, [tip]  FROM [Poligoni vlak Drawing],[Poligona TACKE] WHERE Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID])  and [tahimetrija]=0 ) as B on A.vl1id_=B.vl2id_ and A.brTacke<>B.brtViz ) where [tip]=2 ) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,3 as tip_  FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & "), (select CStr([brdetaljne]) as brtViz,[X (I)] as Yviz_, [Y (I)] as Xviz_, [Visina] as Hviz_, 1.5 as visSign, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & ")) ORDER by tip_,brt1)"

                qvr_.RunEx(True)
                For pp = 0 To qvr_.Table.RecordSet.Count - 1
                    If pp = 0 Then
                        'imas prvo broj stanice
                        xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(1)
                        xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                        xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                        xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                        xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                        'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                        brojacExcel_ += 1
                    ElseIf pp = 1 Then

                        'preskaces broj stanice i upisujes samo visinu signala
                        xlsSheet.Cells(brojacExcel_, 1) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(3), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                        xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                        xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                        xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                        'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                        brojacExcel_ += 1

                    Else
                        'preskaces broj stanice 
                        'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(3)
                        xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                        xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                        xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                        xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                        'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                        'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                        brojacExcel_ += 1

                    End If
                Next
                'sada ponovis prvu!
                'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(0).DataText(1)
                xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(0).DataText(2)
                xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(0).DataText(4)
                xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(0).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(0).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(0).DataText(7)
                xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(0).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(0).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(13), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(12), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(14), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(15), 2, TriState.True)
                xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(16), 2, TriState.True)
                brojacExcel_ += 1
                'xlsWB_.Save()
            Else
                'prazna stanica

            End If

            qvrDetaljne.Text = "SELECT A.brTacke as Sa_,B.brTacke as Na_,[tip] FROM ((SELECT [Poligona TACKE].[brTacke],[Poligoni vlak Drawing].[ID] as vl1id_ FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brTacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1) & " and [tahimetrija]=0 and Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID]) ORDER by [Poligoni vlak Drawing].[Bearing (I)]) as A LEFT join (SELECT [Poligoni vlak Drawing].[ID] as vl2id_,[brTacke],[tip] FROM [Poligoni vlak Drawing],[Poligona TACKE] WHERE Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID])  and [tahimetrija]=0) as B on A.vl1id_=B.vl2id_ and A.brTacke<>B.brTacke ) where [tip]=2"
            qvrDetaljne.RunEx(True)

            For j = 0 To qvrDetaljne.Table.RecordSet.Count - 1
                'sada ides stampanje za svaku detaljnu
                txt_Query.DocumentText += vbNewLine & "           Detaljna : " & qvrDetaljne.Table.RecordSet.Item(j).DataText(2)
                'txtQuery.Text = txtQuery.Text & vbNewLine & "           Detaljna : " & qvrDetaljne.Table.RecordSet.Item(j).DataText(2)
                'sada pises u zapisnik

                If qvrDetaljne.Table.RecordSet.Count > 0 Then
                    'pises u zapisnik!
                    brojacExcel_ += My.Settings.tahimetrija_razmakIzmeduRedova
                    visinaStanice = Math.Floor(185 - Rnd() * 15) / 100 'zameniti deo sql-a floor(185-Rnd*15)
                    qvr_.Text = "SELECT brtPol,brtViz," & visinaStanice & ", floor(ugaoHorizontalno) as stepenH, floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60) as minutH, floor((((ugaoHorizontalno-floor(ugaoHorizontalno))*60)-(floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60)))*60) as sekundH, floor(UgaoZenitno) as stepenZ, floor((UgaoZenitno-floor(UgaoZenitno) )*60 ) as minutZ, floor((((UgaoZenitno-floor(UgaoZenitno) )*60)-(floor((UgaoZenitno-floor(UgaoZenitno) )*60)))*60) as sekundZ, Dhor,Dred,visraz_,vissign,yviz_,xviz_,hviz_,brt1 FROM (SELECT top 2  brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,1 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2) & "), (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([Poligona TACKE].[brtacke])) as brtViz,[Poligona TACKE].[X (I)] as Yviz_,[Poligona TACKE].[Y (I)] as Xviz_,[Poligona TACKE].[Visina] as Hviz_,1.5 as visSign, [Poligona TACKE].[brTacke] as brt1 FROM [Poligona TACKE],(SELECT [Poligoni vlak Drawing].[Geom (I)],[Poligona TACKE].[brTacke] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brtacke]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2) & " and Touches([Poligona TACKE].[ID],[Poligoni vlak Drawing].[ID])) as A WHERE Touches(A.[Geom (I)],[Poligona TACKE].[ID]) and A.[brTacke]<>[Poligona TACKE].[brTacke])) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,2 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2) & "), (select CStr([brdetaljne]) as brtViz,[X (I)] as Yviz_, [Y (I)] as Xviz_, [Visina] as Hviz_, 1.5 as visSign, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2) & " )) ORDER by tip_,brt1)"
                    qvr_.RunEx(True)
                    For pp = 0 To qvr_.Table.RecordSet.Count - 1
                        If pp = 0 Then
                            'imas prvo broj stanice
                            xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(1)
                            xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                            xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                            xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                            xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                            'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                            brojacExcel_ += 1
                        ElseIf pp = 1 Then

                            'preskaces broj stanice i upisujes samo visinu signala
                            xlsSheet.Cells(brojacExcel_, 1) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(3), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                            xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                            xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                            xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                            'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                            brojacExcel_ += 1

                        Else
                            'preskaces broj stanice 
                            'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(3)
                            xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                            xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                            xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                            xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                            'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                            'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                            brojacExcel_ += 1

                        End If
                    Next
                    'sada ponovis prvu!
                    'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(0).DataText(1)
                    xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(0).DataText(2)
                    xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(0).DataText(4)
                    xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(0).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                    xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(0).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                    xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(0).DataText(7)
                    xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(0).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                    xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(0).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                    'xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                    'xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(13), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(12), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(14), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(15), 2, TriState.True)
                    xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(16), 2, TriState.True)
                    brojacExcel_ += 1
                Else
                    'prazna stanica

                End If


                qvrUpdate.Text = "update [Poligona TACKE] set [tahimetrija]=1 where [brtacke]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2)
                qvrUpdate.RunEx(True)

                'sada proveris da li ova detaljna ima slepu!

                qvrSlepe.Text = "SELECT A.brTacke as Sa_,B.brTacke as Na_,[tip] FROM ((SELECT [Poligona TACKE].[brTacke],[Poligoni vlak Drawing].[ID] as vl1id_ FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brTacke]=" & qvrDetaljne.Table.RecordSet.Item(j).DataText(2) & " and Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID]) ORDER by [Poligoni vlak Drawing].[Bearing (I)]) as A LEFT join (SELECT [Poligoni vlak Drawing].[ID] as vl2id_,[brTacke],[tip] FROM [Poligoni vlak Drawing],[Poligona TACKE] WHERE Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID])  and tahimetrija=0) as B on A.vl1id_=B.vl2id_ and A.brTacke<>B.brTacke ) where [tip]=3"
                qvrSlepe.RunEx(True)

                For k = 0 To qvrSlepe.Table.RecordSet.Count - 1
                    ' txtQuery.Text = txtQuery.Text & vbNewLine & "                   Slepa : " & qvrSlepe.Table.RecordSet.Item(k).DataText(2)
                    txt_Query.DocumentText += vbNewLine & "                   Slepa : " & qvrSlepe.Table.RecordSet.Item(k).DataText(2)

                    If qvrSlepe.Table.RecordSet.Count > 0 Then
                        'pises u zapisnik!
                        brojacExcel_ += My.Settings.tahimetrija_razmakIzmeduRedova
                        visinaStanice = Math.Floor(185 - Rnd() * 15) / 100 'zameniti deo sql-a floor(185-Rnd*15)
                        qvr_.Text = "SELECT brtPol,brtViz," & visinaStanice & ", floor(ugaoHorizontalno) as stepenH, floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60) as minutH, floor((((ugaoHorizontalno-floor(ugaoHorizontalno))*60)-(floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60)))*60) as sekundH, floor(UgaoZenitno) as stepenZ, floor((UgaoZenitno-floor(UgaoZenitno) )*60 ) as minutZ, floor((((UgaoZenitno-floor(UgaoZenitno) )*60)-(floor((UgaoZenitno-floor(UgaoZenitno) )*60)))*60) as sekundZ, Dhor,Dred,visraz_,vissign,yviz_,xviz_,hviz_,brt1 FROM (SELECT top 2 brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,1 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2) & "), (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([Poligona TACKE].[brtacke])) as brtViz,[Poligona TACKE].[X (I)] as Yviz_,[Poligona TACKE].[Y (I)] as Xviz_,[Poligona TACKE].[Visina] as Hviz_,1.5 as visSign, [Poligona TACKE].[brTacke] as brt1 FROM [Poligona TACKE],(SELECT [Poligoni vlak Drawing].[Geom (I)],[Poligona TACKE].[brTacke] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brtacke]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2) & " and Touches([Poligona TACKE].[ID],[Poligoni vlak Drawing].[ID])) as A WHERE Touches(A.[Geom (I)],[Poligona TACKE].[ID]) and A.[brTacke]<>[Poligona TACKE].[brTacke])) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,2 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2) & "), (select CStr([brdetaljne]) as brtViz,[X (I)] as Yviz_, [Y (I)] as Xviz_, [Visina] as Hviz_, 1.5 as visSign, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2) & " )) ORDER by tip_,brt1)"
                        qvr_.RunEx(True)
                        For pp = 0 To qvr_.Table.RecordSet.Count - 1
                            If pp = 0 Then
                                'imas prvo broj stanice
                                xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(1)
                                xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                brojacExcel_ += 1
                            ElseIf pp = 1 Then

                                'preskaces broj stanice i upisujes samo visinu signala
                                xlsSheet.Cells(brojacExcel_, 1) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(3), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                brojacExcel_ += 1

                            Else
                                'preskaces broj stanice 
                                'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(3)
                                xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                brojacExcel_ += 1

                            End If
                        Next
                        'sada ponovis prvu!
                        'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(0).DataText(1)
                        xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(0).DataText(2)
                        xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(0).DataText(4)
                        xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(0).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(0).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(0).DataText(7)
                        xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(0).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(0).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                        xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(13), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(12), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(14), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(15), 2, TriState.True)
                        xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(16), 2, TriState.True)
                        brojacExcel_ += 1
                    Else
                        'prazna stanica

                    End If

                    qvrUpdate.Text = "update [Poligona TACKE] set [tahimetrija]=1 where [brtacke]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2)
                    qvrUpdate.RunEx(True)

                    'sada proveris dali ova slepa ima jos neku slepu!
                    qvrSlepeLevel1.Text = "SELECT A.brTacke as Sa_,B.brTacke as Na_,[tip] FROM ((SELECT [Poligona TACKE].[brTacke],[Poligoni vlak Drawing].[ID] as vl1id_ FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brTacke]=" & qvrSlepe.Table.RecordSet.Item(k).DataText(2) & " and Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID]) ORDER by [Poligoni vlak Drawing].[Bearing (I)]) as A LEFT join (SELECT [Poligoni vlak Drawing].[ID] as vl2id_,[brTacke],[tip] FROM [Poligoni vlak Drawing],[Poligona TACKE] WHERE Touches([Poligoni vlak Drawing].[ID],[Poligona TACKE].[ID])  and tahimetrija=0) as B on A.vl1id_=B.vl2id_ and A.brTacke<>B.brTacke ) where [tip]=3"
                    qvrSlepeLevel1.RunEx(True)

                    For p = 0 To qvrSlepeLevel1.Table.RecordSet.Count - 1

                        'txtQuery.Text = txtQuery.Text & vbNewLine & "                       Pod-Slepa : " & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2)
                        txt_Query.DocumentText = vbNewLine & "                       Pod-Slepa : " & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2)
                        If qvrSlepeLevel1.Table.RecordSet.Count > 0 Then
                            'pises u zapisnik!
                            brojacExcel_ += My.Settings.tahimetrija_razmakIzmeduRedova
                            visinaStanice = Math.Floor(185 - Rnd() * 15) / 100 'zameniti deo sql-a floor(185-Rnd*15)
                            qvr_.Text = "SELECT brtPol,brtViz," & visinaStanice & ", floor(ugaoHorizontalno) as stepenH, floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60) as minutH, floor((((ugaoHorizontalno-floor(ugaoHorizontalno))*60)-(floor((ugaoHorizontalno-floor(ugaoHorizontalno))*60)))*60) as sekundH, floor(UgaoZenitno) as stepenZ, floor((UgaoZenitno-floor(UgaoZenitno) )*60 ) as minutZ, floor((((UgaoZenitno-floor(UgaoZenitno) )*60)-(floor((UgaoZenitno-floor(UgaoZenitno) )*60)))*60) as sekundZ, Dhor,Dred,visraz_,vissign,yviz_,xviz_,hviz_,brt1 FROM (SELECT top 2 brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,1 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2) & "), (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([Poligona TACKE].[brtacke])) as brtViz,[Poligona TACKE].[X (I)] as Yviz_,[Poligona TACKE].[Y (I)] as Xviz_,[Poligona TACKE].[Visina] as Hviz_,1.5 as visSign, [Poligona TACKE].[brTacke] as brt1 FROM [Poligona TACKE],(SELECT [Poligoni vlak Drawing].[Geom (I)],[Poligona TACKE].[brTacke] FROM [Poligona TACKE],[Poligoni vlak Drawing] WHERE [brtacke]=" & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2) & " and Touches([Poligona TACKE].[ID],[Poligoni vlak Drawing].[ID])) as A WHERE Touches(A.[Geom (I)],[Poligona TACKE].[ID]) and A.[brTacke]<>[Poligona TACKE].[brTacke])) union SELECT brtPol,brtViz,visstan_, iif((iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))<0,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))+360,(iif((xviz_-xpol_)<0,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))+180,rad2deg(Atn((yviz_-ypol_)/(xviz_-xpol_)))))) as ugaoHorizontalno, rad2deg(Acos(((hviz_-hpol_)+vissign-visstan_)/(Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 ))))) as UgaoZenitno, Sqr((yviz_-ypol_)^2+(xviz_-xpol_)^2 ) as Dhor, Sqr((hviz_-hpol_)^2+((yviz_-ypol_)^2+(xviz_-xpol_)^2 )) as Dred, (hviz_-hpol_) as visraz_, vissign, yviz_,xviz_,hviz_,brt1,2 as tip_ FROM (SELECT * FROM (SELECT (" & Chr(34) & "P" & Chr(34) & " & CStr([brTacke])) as brtPol,[X (I)] as ypol_,[Y (I)] as xpol_,[Visina] as Hpol_," & visinaStanice & " as visstan_ FROM [Poligona TACKE] WHERE [brTacke]=" & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2) & "), (select CStr([brdetaljne]) as brtViz,[X (I)] as Yviz_, [Y (I)] as Xviz_, [Visina] as Hviz_, 1.5 as visSign, [brdetaljne] as brt1 from [Objedinjeno] WHERE [brPoligonske]=" & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2) & " )) ORDER by tip_,brt1)"
                            qvr_.RunEx(True)
                            For pp = 0 To qvr_.Table.RecordSet.Count - 1
                                If pp = 0 Then
                                    'imas prvo broj stanice
                                    xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(1)
                                    xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                    xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                    xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                    xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                    'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                    brojacExcel_ += 1
                                ElseIf pp = 1 Then

                                    'preskaces broj stanice i upisujes samo visinu signala
                                    xlsSheet.Cells(brojacExcel_, 1) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(3), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                    xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                    xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                    xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                    'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                    brojacExcel_ += 1

                                Else
                                    'preskaces broj stanice 
                                    'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(pp).DataText(3)
                                    xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(pp).DataText(2)
                                    xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(pp).DataText(4)
                                    xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(pp).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(pp).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(pp).DataText(7)
                                    xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(pp).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(pp).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                                    xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(10), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(11), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(13), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(12), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(14), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(15), 2, TriState.True)
                                    xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(16), 2, TriState.True)
                                    'xlsSheet.Cells(brojacExcel_, 16) = FormatNumber(qvr_.Table.RecordSet.Item(pp).DataText(17), 2, TriState.True)
                                    brojacExcel_ += 1

                                End If
                            Next
                            'sada ponovis prvu!
                            'xlsSheet.Cells(brojacExcel_, 1) = qvr_.Table.RecordSet.Item(0).DataText(1)
                            xlsSheet.Cells(brojacExcel_, 2) = qvr_.Table.RecordSet.Item(0).DataText(2)
                            xlsSheet.Cells(brojacExcel_, 3) = qvr_.Table.RecordSet.Item(0).DataText(4)
                            xlsSheet.Cells(brojacExcel_, 4) = Val(qvr_.Table.RecordSet.Item(0).DataText(5)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 5) = Val(qvr_.Table.RecordSet.Item(0).DataText(6)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 6) = qvr_.Table.RecordSet.Item(0).DataText(7)
                            xlsSheet.Cells(brojacExcel_, 7) = Val(qvr_.Table.RecordSet.Item(0).DataText(8)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 8) = Val(qvr_.Table.RecordSet.Item(0).DataText(9)).ToString("00", CultureInfo.InvariantCulture)
                            xlsSheet.Cells(brojacExcel_, 9) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(10), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 10) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(11), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 11) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(13), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 12) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(12), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 13) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(14), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 14) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(15), 2, TriState.True)
                            xlsSheet.Cells(brojacExcel_, 15) = FormatNumber(qvr_.Table.RecordSet.Item(0).DataText(16), 2, TriState.True)
                            brojacExcel_ += 1
                        Else
                            'prazna stanica

                        End If

                        qvrUpdate.Text = "update [Poligona TACKE] set [tahimetrija]=1 where [brtacke]=" & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2)
                        qvrUpdate.RunEx(True)

                        'sada printas dalje izlaz
                    Next
                Next
            Next
            qvrUpdate.Text = "update [Poligona TACKE] set [tahimetrija]=1 where [brtacke]=" & qvrPoligone.Table.RecordSet.Item(i).DataText(1)
            qvrUpdate.RunEx(True)

            pb1.Value = i
            'brojacExcel_ += korak_  ' ovde je problem kad ima tacaka koje se preskacu!
        Next
        FileClose()
        qvr_ = Nothing
        qvrDetaljne = Nothing
        qvrPoligone = Nothing
        qvrSlepe = Nothing
        qvrUpdate = Nothing
        pb1.Value = 0
        'txtQuery.Text = txtQuery.Text & vbNewLine & "Kraj: " & Now()
        txt_Query.DocumentText += vbNewLine & "Kraj: " & Now()
        MsgBox("Kraj")
    End Sub

    Private Sub To19To8To1ToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles To19To8To1ToolStripMenuItem.Click
        'ovde kao ulaz mora da ide spisak tacaka i nista vise!!!! - to je pocetak
        'znaci ulaz je file sa id, x, y (mozda z) - ovo verovatno moze iz map file-a
        'On Error Resume Next
        'spisak tacaka dobijas iz map-a kao query
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("fasdf")
        Dim qvr2_ As Manifold.Interop.Query = doc.NewQuery("unizu")

        opf_diag.FileName = ""
        opf_diag.Filter = "Excel File|*.xls"
        opf_diag.Title = "Pronadite tempalate zapisnika"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        'sada otvoris excel !

        Dim xlsApp_ As Microsoft.Office.Interop.Excel.Application = New Microsoft.Office.Interop.Excel.Application
        Dim xlsWB_ As Microsoft.Office.Interop.Excel.Workbook
        xlsApp_.Visible = True
        'xlsWB_ = xlsApp_.Workbooks.Open("D:\Adorjan\Tahimetrija\Tahimetrija_template.xls")
        xlsWB_ = xlsApp_.Workbooks.Open(opf_diag.FileName)
        'MsgBox(xlsWB_.Sheets.Count)

        Dim xlsSheet19 As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.Worksheets("TO19") : Dim xlsSheet8 As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.Worksheets("TO8") : Dim xlsSheet1 As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.Worksheets("TO1")

        Dim dokleStigaoXLS19 As Integer = 12 : Dim dokleStigaoXLS8 As Integer = 8 : Dim dokleStigaoXLS1 As Integer = 8

        qvr_.Text = "select [idvlaka] from [VlakoviMalihTacaka] where [idvlaka]<>0 order by [idvlaka] asc" : qvr_.RunEx(True)

        pb1.Value = 0 : pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            'Dim mat_dx(-1), mat_dy(-1) As Double

            Dim znak_ As Integer = CInt(Int((2 * Rnd()) + 1)) : Dim laznaPopravka_ = udec((CInt(Int((3 * Rnd()) + 1))) / 10000) : If znak_ = 2 Then laznaPopravka_ = -laznaPopravka_
            Dim znakDuzina_ As Integer = CInt(Int((2 * Rnd()) + 1))
            'sada idemo za svaki vlak spisak tacaka pa dalje delji!
            qvr2_.Text = "SELECT idVlaka,brtacke,round(x_,2),round(y_,2),tip FROM (SELECT [idVlaka], pnt_ FROM [VlakoviMalihTacaka] split by Coords([Geom (I)]) as pnt_ ) as A LEFT OUTER JOIN (SELECT [brTacke],[Geom (I)],[X (I)] as x_,[Y (I)] as y_,tip FROM [Poligona TACKE] ) as B on A.pnt_=B.[Geom (I)]  where idvlaka=" & qvr_.Table.RecordSet(i).DataText(1)
            qvr2_.RunEx(True)
            Dim dirTren_ As Double = -1
            'sada imas spisak tacaka!
            'idemo da im ispisemo koordinate i broj tacke!
            Dim sumad_, sumadx, sumady, sumaPrelomnih As Double
            pb1.Maximum = qvr2_.Table.RecordSet.Count

            sumad_ = 0 : sumadx = 0 : sumady = 0 : sumaPrelomnih = 0


            For j = 0 To qvr2_.Table.RecordSet.Count - 1
                'sada generises gresku za direkcione uglove! pa onda idemo dalje!

                If j = 0 Then

                    '19 OBRAZAC

                    'samo broj tacke i broj vlaka
                    xlsSheet19.Cells(dokleStigaoXLS19, 2) = qvr2_.Table.RecordSet.Item(j).DataText(2) : xlsSheet19.Cells(dokleStigaoXLS19, 1) = qvr2_.Table.RecordSet.Item(j).DataText(1)

                    'sada racunas direkcioni ugao!
                    Dim nini_ = NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j + 1).DataText(3), qvr2_.Table.RecordSet.Item(j + 1).DataText(4))

                    'ovde treba u stvari da uradis zaokruzivanje na 4 decimale
                    dirTren_ = udec(Math.Round(uste(nini_), 4)) : xlsSheet19.Cells(dokleStigaoXLS19, 4) = uUkras_izdec(nini_)
                    xlsSheet19.Cells(dokleStigaoXLS19, 5) = uUkras_izdec(nini_)
                    sumaPrelomnih = nini_
                    'KRAJ 19 OBRAZCA

                    If qvr2_.Table.RecordSet.Item(j).DataText(5) = 1 And qvr2_.Table.RecordSet.Item(j + 1).DataText(5) = 1 Then
                        '8 OBRAZAC

                        'ovde prvo gledas tip tacke ako nije jedan onda ne radis TO8!
                        'broj tacke
                        xlsSheet8.Cells(dokleStigaoXLS8, 2) = qvr2_.Table.RecordSet.Item(j).DataText(2) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 2) = qvr2_.Table.RecordSet.Item(j + 1).DataText(2)
                        'y koordinata
                        'xlsSheet8.Cells(dokleStigaoXLS8, 4) = qvr2_.Table.RecordSet.Item(j).DataText(3) : xlsSheet8.Cells(dokleStigaoXLS8, 6) = qvr2_.Table.RecordSet.Item(j + 1).DataText(3)
                        'xlsSheet8.Cells(dokleStigaoXLS8 + 1, 4) = qvr2_.Table.RecordSet.Item(j).DataText(4) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 6) = qvr2_.Table.RecordSet.Item(j + 1).DataText(4)

                        xlsSheet8.Cells(dokleStigaoXLS8, 4) = qvr2_.Table.RecordSet.Item(j).DataText(3) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 4) = qvr2_.Table.RecordSet.Item(j + 1).DataText(3)
                        xlsSheet8.Cells(dokleStigaoXLS8, 6) = qvr2_.Table.RecordSet.Item(j).DataText(4) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 6) = qvr2_.Table.RecordSet.Item(j + 1).DataText(4)

                        'sada mozes razliku
                        Dim dy8, dx8 As Double
                        dy8 = qvr2_.Table.RecordSet.Item(j).DataText(3) - qvr2_.Table.RecordSet.Item(j + 1).DataText(3) : dx8 = qvr2_.Table.RecordSet.Item(j).DataText(4) - qvr2_.Table.RecordSet.Item(j + 1).DataText(4)
                        xlsSheet8.Cells(dokleStigaoXLS8 + 2, 4) = dy8 : xlsSheet8.Cells(dokleStigaoXLS8 + 2, 6) = dx8
                        'sada imas dole sabiranje odnosno oduzimanje
                        xlsSheet8.Cells(dokleStigaoXLS8 + 3, 4) = dy8 + dx8 : xlsSheet8.Cells(dokleStigaoXLS8 + 3, 6) = dy8 - dx8

                        xlsSheet8.Cells(dokleStigaoXLS8, 10) = Math.Round(Math.Sin(urad(nini_)), 8) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 10) = Math.Round(Math.Cos(urad(nini_)), 8)

                        xlsSheet8.Cells(dokleStigaoXLS8 + 3, 9) = uUkras_izdec(nini_) : xlsSheet8.Cells(dokleStigaoXLS8 + 3, 10) = Math.Round((Math.Sqrt((dy8) ^ 2 + (dx8) ^ 2)), 2)

                        dokleStigaoXLS8 += 4
                        'KRAJ 8 OBRAZCA
                    End If


                    'ovde treba da otvara i 8 obrasac
                    dokleStigaoXLS19 += 3

                ElseIf j = qvr2_.Table.RecordSet.Count - 2 Then

                    'sada ga prekidas i nista ne upisujes osim poslenje koordinate
                    xlsSheet19.Cells(dokleStigaoXLS19, 2) = qvr2_.Table.RecordSet.Item(j).DataText(2)
                    xlsSheet19.Cells(dokleStigaoXLS19, 21) = qvr2_.Table.RecordSet.Item(j).DataText(3) : xlsSheet19.Cells(dokleStigaoXLS19, 24) = qvr2_.Table.RecordSet.Item(j).DataText(4)

                    'racuna zavrsni direkcioni
                    Dim nini_ = NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j + 1).DataText(3), qvr2_.Table.RecordSet.Item(j + 1).DataText(4)) - NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j - 1).DataText(3), qvr2_.Table.RecordSet.Item(j - 1).DataText(4)) : If nini_ < 0 Then nini_ = 360 + nini_

                    nini_ = udec(Math.Round(uste(nini_), 4))

                    'odreduje popravku

                    If znak_ = 1 Then laznaPopravka_ += udec(0.0001) Else laznaPopravka_ -= udec(0.0001)
                    'samo prikazujes umanjeno ni a ne i da ga 
                    sumaPrelomnih += nini_ + laznaPopravka_
                    nini_ += laznaPopravka_ : xlsSheet19.Cells(dokleStigaoXLS19 - 1, 4) = uste(-laznaPopravka_) * 10000

                    'ispisujes direkcioni ugao i popravku

                    xlsSheet19.Cells(dokleStigaoXLS19, 4) = uUkras_izdec(nini_)

                    dirTren_ = dirTren_ + nini_ : If dirTren_ > 180 Then dirTren_ = dirTren_ - 180

                    'sada ti treba direkcioni ugao kao poslednji korak!
                    nini_ = NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j + 1).DataText(3), qvr2_.Table.RecordSet.Item(j + 1).DataText(4))
                    ' i njega treba upisati negde

                    'ovo ide na mesto onoga!
                    xlsSheet19.Cells(dokleStigaoXLS19 + 2, 5) = uUkras_izdec(nini_) : xlsSheet19.Cells(dokleStigaoXLS19 + 5, 4) = "T: " & uUkras_izdec(nini_)

                    'SADA OSTAJE RAZLIKA

                    Dim kk_ = Fix(sumaPrelomnih / 180) : kk_ = sumaPrelomnih - kk_ * 180

                    Dim raz_ = nini_ - kk_ : Dim dozods_ = (20 * Math.Sqrt(j)) / 3600
                    xlsSheet19.Cells(dokleStigaoXLS19 + 5, 4) = "T: " & uUkras_izdec(nini_)
                    xlsSheet19.Cells(dokleStigaoXLS19 + 6, 4) = "M: " & uUkras_izdec(kk_)
                    If raz_ < 0 Then xlsSheet19.Cells(dokleStigaoXLS19 + 7, 4) = "f: -" & uUkras_izdecSkraceno(raz_) Else xlsSheet19.Cells(dokleStigaoXLS19 + 7, 4) = "f: " & uUkras_izdecSkraceno(raz_)

                    xlsSheet19.Cells(dokleStigaoXLS19 + 8, 4) = "d: " & uUkras_izdecSkraceno(dozods_)


                    'sada mozes da odstampas i sumuduzina u sumu dx i dy
                    xlsSheet19.Cells(dokleStigaoXLS19, 7) = Math.Round(sumad_, 2)
                    xlsSheet19.Cells(dokleStigaoXLS19 - 1, 11) = "My: " & Math.Round(sumadx, 2) : xlsSheet19.Cells(dokleStigaoXLS19 - 1, 16) = "Mx: " & Math.Round(sumady, 2)

                    'sada imas i razliku pocetna zavrsna tacka!
                    xlsSheet19.Cells(dokleStigaoXLS19, 11) = "Ty: " & Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(3) - qvr2_.Table.RecordSet.Item(1).DataText(3), 2)
                    xlsSheet19.Cells(dokleStigaoXLS19, 16) = "Tx: " & Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(4) - qvr2_.Table.RecordSet.Item(1).DataText(4), 2)

                    'idemo na odstupanja:
                    xlsSheet19.Cells(dokleStigaoXLS19 + 1, 11) = "fy: " & Math.Round((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(3) - qvr2_.Table.RecordSet.Item(1).DataText(3), 2) - Math.Round(sumadx, 2)) * 100)
                    xlsSheet19.Cells(dokleStigaoXLS19 + 1, 16) = "fx: " & Math.Round((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(4) - qvr2_.Table.RecordSet.Item(1).DataText(4), 2) - Math.Round(sumady, 2)) * 100)

                    xlsSheet19.Cells(dokleStigaoXLS19 + 2, 14) = "fd: " & Math.Round(Math.Sqrt(((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(3) - qvr2_.Table.RecordSet.Item(1).DataText(3), 2) - Math.Round(sumadx, 2))) ^ 2 + ((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(4) - qvr2_.Table.RecordSet.Item(1).DataText(4), 2) - Math.Round(sumady, 2))) ^ 2) * 100)

                    'sada mozes da sracunas dozvoljeno odstupanje:
                    xlsSheet19.Cells(dokleStigaoXLS19 + 3, 14) = "d: " & Math.Round((0.0045 * Math.Sqrt(sumad_) + 0.0003 * sumad_ + 0.005) * 100)

                    'sada ostaje da se poprave razlike!
                    Dim ddx, ddy As Integer
                    ddx = Math.Round((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(3) - qvr2_.Table.RecordSet.Item(1).DataText(3), 2) - Math.Round(sumadx, 2)) * 100)
                    ddy = Math.Round((Math.Round(qvr2_.Table.RecordSet.Item(qvr2_.Table.RecordSet.Count - 2).DataText(4) - qvr2_.Table.RecordSet.Item(1).DataText(4), 2) - Math.Round(sumady, 2)) * 100)

                    'ovo je mnogo veliko drkanje!!!!!!!!!!!!!!!!!!!!!!!!

                    Dim celija_ = dokleStigaoXLS19 - 2

                    'sada je veliko pitanje na kojoj je ovo strani! da li u plusu ili u minusu!
                    'kako to da provalim?
                    Dim perax_, peray_ As Integer : perax_ = -1 : peray_ = -1
                    If sumadx > 0 Then perax_ = 11 Else perax_ = 14
                    If sumady > 0 Then peray_ = 21 Else peray_ = 24
                    For k = 0 To qvr2_.Table.RecordSet.Count - 4
                        'sada idemo citas i radis popravke po x-u odnosno po y-nu!
                        Dim ll_ = DirectCast(xlsSheet19.Cells(celija_, 7), Excel.Range)
                        Dim kx_, ky_ As Double : kx_ = 0 : ky_ = 0
                        Dim px_ = Math.Round((ddx * (Val(ll_.Text) / sumad_)))
                        If px_ <> 0 Then
                            'sada ga pises
                            xlsSheet19.Cells(celija_ - 1, perax_) = px_
                            kx_ = Val(DirectCast(xlsSheet19.Cells(celija_, 21), Excel.Range).Text) - (px_ / 100)
                            xlsSheet19.Cells(celija_, perax_) = Math.Round(kx_, 2)
                        Else
                            kx_ = Val(DirectCast(xlsSheet19.Cells(celija_, 21), Excel.Range).Text)
                        End If


                        ll_ = DirectCast(xlsSheet19.Cells(celija_, 7), Excel.Range)
                        Dim py_ = Math.Round((ddy * (Val(ll_.Text) / sumad_)))
                        If py_ <> 0 Then
                            'sada ga pises
                            xlsSheet19.Cells(celija_ - 1, peray_) = py_
                            ky_ = Val(DirectCast(xlsSheet19.Cells(celija_, 24), Excel.Range).Text) - (py_ / 100)
                            xlsSheet19.Cells(celija_, peray_) = Math.Round(ky_, 2)
                        Else
                            ky_ = Val(DirectCast(xlsSheet19.Cells(celija_, 24), Excel.Range).Text)
                        End If
                        'duzina :
                        Dim dd_ = Math.Round(Math.Sqrt(kx_ * kx_ + ky_ * ky_), 2)
                        'sada pises ovu duzinu i to bi trebalo da je to!!!!!!!!!!!!!!
                        xlsSheet19.Cells(celija_, 7) = dd_
                        celija_ = celija_ - 4
                    Next


                    'sada treba poraviti i duzinu ! ali kako ?

                    dokleStigaoXLS19 += 4
                    xlsSheet19.Cells(dokleStigaoXLS19, 2) = qvr2_.Table.RecordSet.Item(j + 1).DataText(2)

                    If qvr2_.Table.RecordSet.Item(j).DataText(5) = 1 And qvr2_.Table.RecordSet.Item(j + 1).DataText(5) = 1 Then
                        'sada mozes da upises i u 8 obrazac!
                        'broj tacke
                        xlsSheet8.Cells(dokleStigaoXLS8, 2) = qvr2_.Table.RecordSet.Item(j).DataText(2) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 2) = qvr2_.Table.RecordSet.Item(j + 1).DataText(2)
                        'y koordinata
                        xlsSheet8.Cells(dokleStigaoXLS8, 4) = qvr2_.Table.RecordSet.Item(j).DataText(3) : xlsSheet8.Cells(dokleStigaoXLS8, 6) = qvr2_.Table.RecordSet.Item(j + 1).DataText(3)
                        xlsSheet8.Cells(dokleStigaoXLS8 + 1, 4) = qvr2_.Table.RecordSet.Item(j).DataText(4) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 6) = qvr2_.Table.RecordSet.Item(j + 1).DataText(4)
                        'sada mozes razliku
                        Dim dy8, dx8 As Double
                        dy8 = qvr2_.Table.RecordSet.Item(j).DataText(3) - qvr2_.Table.RecordSet.Item(j + 1).DataText(3) : dx8 = qvr2_.Table.RecordSet.Item(j).DataText(4) - qvr2_.Table.RecordSet.Item(j + 1).DataText(4)
                        xlsSheet8.Cells(dokleStigaoXLS8 + 2, 4) = dy8 : xlsSheet8.Cells(dokleStigaoXLS8 + 2, 6) = dx8
                        'sada imas dole sabiranje odnosno oduzimanje
                        xlsSheet8.Cells(dokleStigaoXLS8 + 3, 4) = dy8 + dx8 : xlsSheet8.Cells(dokleStigaoXLS8 + 3, 6) = dy8 - dx8

                        xlsSheet8.Cells(dokleStigaoXLS8, 10) = Math.Round(Math.Sin(urad(nini_)), 8) : xlsSheet8.Cells(dokleStigaoXLS8 + 1, 10) = Math.Round(Math.Cos(urad(nini_)), 8)

                        xlsSheet8.Cells(dokleStigaoXLS8 + 3, 9) = uUkras_izdec(nini_) : xlsSheet8.Cells(dokleStigaoXLS8 + 3, 10) = Math.Round((Math.Sqrt((dy8) ^ 2 + (dx8) ^ 2)), 2)

                        dokleStigaoXLS8 += 4
                        'kraj upisivanja u 8 obrazac
                    End If


                    Exit For

                Else

                    'stampa broj tacke i koordinate
                    xlsSheet19.Cells(dokleStigaoXLS19, 2) = qvr2_.Table.RecordSet.Item(j).DataText(2) : xlsSheet19.Cells(dokleStigaoXLS19, 21) = qvr2_.Table.RecordSet.Item(j).DataText(3) : xlsSheet19.Cells(dokleStigaoXLS19, 24) = qvr2_.Table.RecordSet.Item(j).DataText(4)

                    'sada racunas direkcioni ugao kao merenje sa ove tacke!
                    Dim nini_ = NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j + 1).DataText(3), qvr2_.Table.RecordSet.Item(j + 1).DataText(4)) - NiAnaB(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j - 1).DataText(3), qvr2_.Table.RecordSet.Item(j - 1).DataText(4)) : If nini_ < 0 Then nini_ = 360 + nini_
                    sumaPrelomnih += (nini_ + laznaPopravka_)
                    'PAKUJES PRVI
                    dokleStigaoXLS1 = upisi1obrazac(xlsSheet1, dokleStigaoXLS1, nini_, qvr2_.Table.RecordSet.Item(j).DataText(2), qvr2_.Table.RecordSet.Item(j - 1).DataText(2), qvr2_.Table.RecordSet.Item(j + 1).DataText(2))
                    'KRAJ PAKOVANJA PRVOG

                    'nini_ += laznaPopravka_
                    xlsSheet19.Cells(dokleStigaoXLS19 - 1, 4) = uste(-laznaPopravka_) * 10000

                    dirTren_ += nini_ : If dirTren_ > 180 Then dirTren_ = dirTren_ - 180

                    'pise direkcioni
                    xlsSheet19.Cells(dokleStigaoXLS19, 4) = uUkras_izdec(nini_ + laznaPopravka_)
                    xlsSheet19.Cells(dokleStigaoXLS19 + 2, 5) = uUkras_izdec(dirTren_)

                    Dim Duzina_ = Duzina(qvr2_.Table.RecordSet.Item(j).DataText(3), qvr2_.Table.RecordSet.Item(j).DataText(4), qvr2_.Table.RecordSet.Item(j + 1).DataText(3), qvr2_.Table.RecordSet.Item(j + 1).DataText(4))
                    'sada upisujes ovaj ugao!
                    'sada sredi duzinu pa tek onda idi na njenu sumu!
                    Dim popravkaDuzine = CInt(Int((2 * Rnd()) + 1)) / 100 : If znakDuzina_ = 1 Then popravkaDuzine = -popravkaDuzine
                    Duzina_ += popravkaDuzine : Duzina_ = Math.Round(Duzina_, 2)
                    sumad_ += Duzina_
                    Dim dx, dy, dx1, dy1 As Double

                    dx1 = Math.Round(qvr2_.Table.RecordSet.Item(j + 1).DataText(3) - qvr2_.Table.RecordSet.Item(j).DataText(3), 2)
                    dy1 = Math.Round(qvr2_.Table.RecordSet.Item(j + 1).DataText(4) - qvr2_.Table.RecordSet.Item(j).DataText(4), 2)

                    'sada i ovo mozes da upises

                    xlsSheet19.Cells(dokleStigaoXLS19 + 2, 21) = dx1 : xlsSheet19.Cells(dokleStigaoXLS19 + 2, 24) = dy1

                    dx = Math.Round(Duzina_ * Math.Sin(urad(dirTren_)), 2) : dy = Math.Round(Duzina_ * Math.Cos(urad(dirTren_)), 2)
                    sumadx += dx : sumady += dy
                    'sada ovo upises!

                    'stampa duzine
                    xlsSheet19.Cells(dokleStigaoXLS19 + 2, 7) = Duzina_

                    If dx > 0 Then
                        xlsSheet19.Cells(dokleStigaoXLS19 + 2, 11) = dx
                    Else
                        dx = Math.Abs(dx) : dx1 = Math.Abs(dx1)
                        xlsSheet19.Cells(dokleStigaoXLS19 + 2, 14) = dx
                    End If

                    If dy > 0 Then
                        xlsSheet19.Cells(dokleStigaoXLS19 + 2, 16) = dy
                    Else
                        dy = Math.Abs(dy) : dy1 = Math.Abs(dy1)
                        xlsSheet19.Cells(dokleStigaoXLS19 + 2, 19) = dy
                    End If

                    dokleStigaoXLS19 += 4
                End If
                pb1.Value = j
            Next

            dokleStigaoXLS19 += 9
            pb1.Value = i
        Next

        xlsSheet19 = Nothing
        xlsWB_ = Nothing
        xlsApp_.Quit()
        xlsApp_ = Nothing
        pb1.Value = 0
        pb1.Value = 0
        MsgBox("Kraj")
    End Sub

    Private Sub To18EToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles To18EToolStripMenuItem.Click
        'ovde kao ulaz mora da ide spisak tacaka i nista vise!!!! - to je pocetak
        'znaci ulaz je file sa id, x, y (mozda z) - ovo verovatno moze iz map file-a
        'On Error Resume Next
        'spisak tacaka dobijas iz map-a kao query
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("fasdf")
        Dim qvr2_ As Manifold.Interop.Query = doc.NewQuery("unizu")

        opf_diag.FileName = ""
        opf_diag.Filter = "Excel File|*.xls"
        opf_diag.Title = "Pronadite tempalate zapisnika"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        'sada otvoris excel !

        Dim xlsApp_ As Microsoft.Office.Interop.Excel.Application = New Microsoft.Office.Interop.Excel.Application
        Dim xlsWB_ As Microsoft.Office.Interop.Excel.Workbook
        xlsApp_.Visible = True
        'xlsWB_ = xlsApp_.Workbooks.Open("D:\Adorjan\Tahimetrija\Tahimetrija_template.xls")
        xlsWB_ = xlsApp_.Workbooks.Open(opf_diag.FileName)
        'MsgBox(xlsWB_.Sheets.Count)

        Dim xlsSheet18E As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.Worksheets("TO18E")
        Dim xlsSheetK As Microsoft.Office.Interop.Excel.Worksheet = xlsWB_.Worksheets("K")

        Dim dokleStigaoxlsSheet18E As Integer = 6 : Dim dokleStigaoXLSK As Integer = 5

        qvr_.Text = "select [idvlaka] from [VlakoviMalihTacaka] where [idvlaka]<>0 order by [idvlaka] asc" : qvr_.RunEx(True)

        pb1.Value = 0 : pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0
        Dim brojac3ki_ As Integer = 1

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            pb1.Value = i

            qvr2_.Text = "SELECT idVlaka,brtacke,round(x_,2),round(y_,2),visina,tip FROM (SELECT [idVlaka], pnt_ FROM [VlakoviMalihTacaka] split by Coords([Geom (I)]) as pnt_ ) as A LEFT OUTER JOIN (SELECT [brTacke],[Geom (I)],[X (I)] as x_,[Y (I)] as y_,visina,tip FROM [Poligona TACKE] ) as B on A.pnt_=B.[Geom (I)]  where idvlaka=" & qvr_.Table.RecordSet(i).DataText(1)
            qvr2_.RunEx(True)

            For j = 1 To qvr2_.Table.RecordSet.Count - 3
                'prva tacka ide glavna pa onda idu dalje!
                'sada upisujemo broj tacke i visinu


                xlsSheetK.Cells(dokleStigaoXLSK, 1) = qvr2_.Table.RecordSet(j).DataText(2)
                xlsSheetK.Cells(dokleStigaoXLSK, 10) = qvr2_.Table.RecordSet(j).DataText(5)



                'sada mi treba odavde sta da izvuce ako preracunava!?
                If j >= 2 Then
                    If qvr2_.Table.RecordSet(j).DataText(6) >= 2 Then

                        'sada mozes da citas ovo merenje
                        'Dim merenje1_, merenje2_ As Double

                        'duzina
                        Dim duzina_ = Math.Round(Math.Sqrt((qvr2_.Table.RecordSet(j - 1).DataText(3) - qvr2_.Table.RecordSet(j).DataText(3)) ^ 2 + (qvr2_.Table.RecordSet(j - 1).DataText(4) - qvr2_.Table.RecordSet(j).DataText(4)) ^ 2) / 100, 2)

                        xlsSheetK.Cells(dokleStigaoXLSK - 1, 2) = "18E." & i + 1 : xlsSheetK.Cells(dokleStigaoXLSK - 1, 11) = duzina_
                        'razlika dozvoljeog odstupanja recimo max 6
                        Dim raz_ As Integer = Rnd() * 6 : Dim znak_ As Integer = Rnd() * 1

                        If znak_ = 0 Then
                            'minus
                            xlsSheetK.Cells(dokleStigaoXLSK - 1, 6) = -Math.Round(Rnd() * 6, 0)
                        Else
                            'plus
                            xlsSheetK.Cells(dokleStigaoXLSK - 1, 6) = Math.Round(Rnd() * 6, 0)
                        End If
                        Dim g_ = xlsSheetK.Cells(dokleStigaoXLSK - 1, 3)

                        Dim pp_ As Excel.Range = xlsSheetK.Cells(dokleStigaoXLSK - 1, 3) : xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 34) = Val(pp_.Text) : pp_ = xlsSheetK.Cells(dokleStigaoXLSK - 1, 4)
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 5, 34) = Val(pp_.Text)


                        'ispisuje koordinate 
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E, 1) = qvr2_.Table.RecordSet(j - 1).DataText(2)
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E, 3) = qvr2_.Table.RecordSet(j).DataText(2)
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 3, 1) = qvr2_.Table.RecordSet(j).DataText(2)
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 3, 3) = qvr2_.Table.RecordSet(j - 1).DataText(2)

                        'Napred racunica:

                        duzina_ = Math.Round(Math.Sqrt((qvr2_.Table.RecordSet(j - 1).DataText(3) - qvr2_.Table.RecordSet(j).DataText(3)) ^ 2 + (qvr2_.Table.RecordSet(j - 1).DataText(4) - qvr2_.Table.RecordSet(j).DataText(4)) ^ 2), 3)
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 30) = duzina_
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 29) = ((((((Val(qvr2_.Table.RecordSet(j - 1).DataText(3)) + Val(qvr2_.Table.RecordSet(j).DataText(3))) / 2) - 7500000) / 1000) ^ 2) / 2 / 6377 ^ 2 - 0.0001) * duzina_ * 10 ^ 3
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 28) = -0.0001568 * duzina_ * ((Val(qvr2_.Table.RecordSet(j - 1).DataText(5)) + Val(qvr2_.Table.RecordSet(j).DataText(5))) / 2)

                        'nazad
                        Dim duzina2_ As Double = duzina_ + Rnd() * 0.001
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 5, 30) = duzina2_
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 5, 29) = ((((((Val(qvr2_.Table.RecordSet(j).DataText(3)) + Val(qvr2_.Table.RecordSet(j - 1).DataText(3))) / 2) - 7500000) / 1000) ^ 2) / 2 / 6377 ^ 2 - 0.0001) * duzina2_ * 10 ^ 3
                        xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 5, 28) = -0.0001568 * duzina2_ * ((Val(qvr2_.Table.RecordSet(j).DataText(5)) + Val(qvr2_.Table.RecordSet(j - 1).DataText(5))) / 2)


                        dokleStigaoxlsSheet18E += 6

                        brojac3ki_ += 1

                        If brojac3ki_ = 7 Then
                            brojac3ki_ = 1


                            'sada prode i u kolone upise prazan string
                            'prvi red imas na 23
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E, 24) = ""
                            For p = 1 To 20
                                xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 1, p + 4) = ""
                            Next
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 1, 34) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 24) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 26) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 27) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 31) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 32) = ""
                            xlsSheet18E.Cells(dokleStigaoxlsSheet18E + 2, 33) = ""

                            dokleStigaoxlsSheet18E += 4
                        End If

                        'dokleStigaoxlsSheet18E += 3
                    Else
                        'znaci dosao do kraja
                        dokleStigaoXLSK += 2 ': dokleStigaoxlsSheet18E += 3
                        ' 
                        'i da treba sledeci red da se anulira!
                        For k = 3 To 9
                            xlsSheetK.Cells(dokleStigaoXLSK - 1, k) = ""
                        Next
                        Exit For
                    End If

                    If j = qvr2_.Table.RecordSet.Count - 3 Then
                        For k = 3 To 9
                            xlsSheetK.Cells(dokleStigaoXLSK + 1, k) = ""
                        Next
                    End If

                End If
                dokleStigaoXLSK += 2
                'dokleStigaoxlsSheet18E += 3
            Next

        Next
        pb1.Value = 0
        MsgBox("Kraj")

    End Sub

    Private Sub GPSZapisnikToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles GPSZapisnikToolStripMenuItem.Click

        'koji je algoritam?

        'trebaju ti dve informacije: koji je krug i koji je pocetak u krugu! to je mnogo bitno - ovo su ti polja: krug: idtable 

        'neka su to dva polja krug i prvi gde je sa prvi oznacen onaj koji ima jedan!
        Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
        'treba ti pocetak merenja i datum kada je poceo
        Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("rastojanje")
        Dim qvr2_ As Manifold.Interop.Query = doc_.NewQuery("updateT")
        Dim qvr3_ As Manifold.Interop.Query = doc_.NewQuery("zaSpsiak")
        Dim qvr4_ As Manifold.Interop.Query = doc_.NewQuery("dajIzTabele")
        Dim pp_ = My.Settings.GPSMerenje_datumPocetka.Split("/")
        Dim gg_ = My.Settings.GPSMerenje_vremePocetka.Split(":")
        Dim pocetnoVreme As New Date(pp_(2), pp_(1), pp_(0), gg_(0), gg_(1), 0)
        Dim kraj As Boolean = False
        'sada ide do 
        Dim brojac_ As Integer = 1
        Dim idTrenutni As Integer = 52
        Dim brzina As Double = My.Settings.GPSMerenje_brzinaHoda
        Dim kolikoradim_ = My.Settings.GPSMerenje_duzinaRada * 3600 'posto je ovo u satima onda ides da ovo zbrojis! - odnosno prebacujes na sekunde!
        Dim kolikopresao = 0



        qvr_.Text = "update [" & My.Settings.layerName_pointTableObelezavanje & "] set [mh]=0, [mv]=0, [idmerenja]=0, [Vreme merenja]=null, [sesija]=0 where [tipTacke]=4"
        qvr_.RunEx(True)

        'qvr_.Text = "select count(*) from [" & My.Settings.layerName_pointTableObelezavanje & "] where [idmerenja]=0 and [tiptacke]=4"
        'qvr_.RunEx(True)

        'pb1.Maximum = qvr_.Table.RecordSet.Count : pb1.Value = 0


        qvr3_.Text = "select distinct [idtable] from [" & My.Settings.layerName_pointTableObelezavanje & "] where [tiptacke]=4 order by [idtable] asc"
        qvr3_.RunEx(True)
        pb1.Maximum = qvr3_.Table.RecordSet.Count + 2

        For i = 0 To qvr3_.Table.RecordSet.Count - 1

            pb1.Value = i

            If i <> 0 Then
                pocetnoVreme = pocetnoVreme.AddMinutes(60)
            End If

            'idTrenutni = InputBox("Unesite ID tacke od koje se polazi za poligon broj " & qvr3_.Table.RecordSet.Item(i).DataText(1), "")

            Try

                qvr4_.Text = "select idIDTacke from TabTabliTacke where idTable=" & qvr3_.Table.RecordSet.Item(i).DataText(1)
                qvr4_.RunEx(True)
                idTrenutni = qvr4_.Table.RecordSet.Item(0).DataText(1)

            Catch ex As Exception

                MsgBox("Morate formirati tabelu TabTabliTacke sa poljima: idtable i idIDTacke gde upisujete id od koga se pocinje za svaku tablu")

            End Try



            kraj = False

            Do While Not kraj = True
                'selektujes 
                qvr_.Text = "SELECT top 1 MIN(Distance([Geom (I)],(select [Geom (I)] from [" & My.Settings.layerName_pointTableObelezavanje & "] where [ID]=" & idTrenutni &
                    "))) as d_,[id] FROM [" & My.Settings.layerName_pointTableObelezavanje & "] WHERE [ID]<>" & idTrenutni & " and [mh]=0 and [idtable]=" & qvr3_.Table.RecordSet.Item(i).DataText(1) & " group by [ID] order by d_"
                qvr_.RunEx(True)

                'SADA MOZES DA PROCITAS OVAJ ID I DUZINU I U ODNOSU NA DUZINU DA POSTAVIS VREME I UPISES GA U REKORD
                Dim D_ As Double



                ' D_ = Math.Round((qvr_.Table.RecordSet.Item(0).DataText(1)) / brzina) + My.Settings.GPSMerenje_zadrzavanjeNaTacki 'dobijas sekunde!
                Try
                        D_ = Math.Round((qvr_.Table.RecordSet.Item(0).DataText(1)) * brzina) + My.Settings.GPSMerenje_zadrzavanjeNaTacki 'dobijas sekunde!
                        kolikopresao += D_


                        'Catch ex As Exception
                        '    MsgBox("Problem je na " & idTrenutni & "  " & qvr_.Text)
                        'Dim upis_ As String = qvr_.Text
                        'Dim qvr5_ As Manifold.Interop.Query = doc_.NewQuery("provera")
                        'qvr5_.Text = "select count(*) from [" & My.Settings.layerName_pointTableObelezavanje & "] where [mh]=0 and [idtable]=" & qvr3_.Table.RecordSet.Item(i).DataText(1)
                        'qvr5_.RunEx(True)
                        '' da li ovo znaci da je kraj!?
                        'If (qvr5_.Table.RecordSet.Item(0).DataText(1) = "0") Then
                        'znaci da nema nista! i da mozes na sledeci nivo!!!!
                        'kolikopresao = kolikoradim_ + 1
                        'End If

                        'End Try

                        'sada ovo daodas u vreme!


                        'sada proveravas dali je u istom danu pa ako jeste onda ovo a ako nije ides na novi dan!
                        If kolikopresao > kolikoradim_ Then
                            'ides na novi dan

                            pocetnoVreme = pocetnoVreme.AddDays(1)

                            'proveris dali je ovaj dan praznik odnosno vikend!


                            If My.Settings.GPSMerenje_preskacemPraznik = 1 Then
                                'znaci preskace praznik proveri ovo!
                                If My.Settings.GPSMerenje_preskacemVikend = 1 Then
                                    'znaci da preskace idi proveri dali je vikend
                                    pocetnoVreme = datumDrzavniPraznikPrviRadni(pocetnoVreme, True)
                                Else
                                    pocetnoVreme = datumDrzavniPraznikPrviRadni(pocetnoVreme, False)
                                End If
                            Else
                                If My.Settings.GPSMerenje_preskacemVikend = 1 Then
                                    'znaci da preskace idi proveri dali je vikend
                                    pocetnoVreme = datumVikendPrviRadniDan(pocetnoVreme, True)
                                Else
                                    pocetnoVreme = datumVikendPrviRadniDan(pocetnoVreme, False)
                                End If
                            End If
                            'sredio si datum!
                            'sada ostaje da resetujes vreme na pocetno!?
                            Dim tt_ = My.Settings.GPSMerenje_vremePocetka.Split(":")
                            'treba da ga resetujes ali koliko ja vidim ostaje jedino da se napravi novi datum!

                            Dim newdd_ As Date = New Date(pocetnoVreme.Year, pocetnoVreme.Month, pocetnoVreme.Day, tt_(0), tt_(1), 0)
                            pocetnoVreme = newdd_
                            kolikopresao = D_

                    Else

                            'ostajes u istom danu
                            pocetnoVreme = pocetnoVreme.AddSeconds(D_)
                            idTrenutni = qvr_.Table.RecordSet.Item(0).DataText(2)
                            'Vreme merenja
                            Dim p_ As New System.Random

                            Try
                                qvr2_.Text = "update [" & My.Settings.layerName_pointTableObelezavanje & "] set [mh]=" & Math.Round((Rnd() * 0.03), 3) & ", [mv]=" & Math.Round((Rnd() * 0.02), 3) &
                           ", [pdop]=" & (p_.Next(1359, 2455) / 1000) & ", [idmerenja]=" & brojac_ & ", [Vreme merenja]=" & Chr(34) & (pocetnoVreme.Year & "-" & pocetnoVreme.Month & "-" & pocetnoVreme.Day & " " & pocetnoVreme.Hour & ":" & pocetnoVreme.Minute & ":" & pocetnoVreme.Second) & Chr(34) & ", [sesija]=" & D_ & " where [id]=" & idTrenutni
                                qvr2_.RunEx(True)
                            Catch ex As Exception
                                MsgBox("Problem sa update  " & qvr2_.Text)
                            End Try


                            'sada idemo iz pocetka!
                            brojac_ += 1
                            'kolikopresao = 0
                        End If
                        'Try
                        '    pb1.Value += 1
                        'Catch ex As Exception

                        'End Try
                    Catch ex As Exception

                        'MsgBox(ex.Message & "  ovo je na kraju")
                        kraj = True
                    Exit Do

                End Try


            Loop



        Next

        doc_.ComponentSet.Remove("rastojanje") : doc_.ComponentSet.Remove("updateT") : doc_.ComponentSet.Remove("zaSpsiak")
        doc_.Save()
        'pa sada idemo da brisemo ako treba
        doc_ = Nothing
        pb1.Value = 0
        MsgBox("Kraj")
    End Sub

    Private Sub ZaokruziPovrsineNovoStanjeParcelaHorizontalnoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ZaokruziPovrsineNovoStanjeParcelaHorizontalnoToolStripMenuItem.Click
        'ovo zaokruzuje povrsine po procembeni razredima da se slozi sa povrsinom parcele

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("Select idnp, prazred_1, prazred_2, prazred_3, prazred_4, prazred_5, prazred_6, prazred_neplodno, (( prazred_1 + prazred_2 + prazred_3 + prazred_4 + prazred_5 + prazred_6 + prazred_neplodno ) - Povrsina ) Raz FROM kom_novostanjeparcela WHERE (( prazred_1 + prazred_2 + prazred_3 + prazred_4 + prazred_5 + prazred_6 + prazred_neplodno ) - Povrsina ) In (1,-1) ORDER BY brparcele, BrDelaParc", conn_)
        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ds_ As New DataTable
        conn_.Open()
        Dim cmd_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        adapter_.SelectCommand = comm_
        adapter_.Fill(ds_)

        pb1.Maximum = ds_.Rows.Count
        pb1.Value = 0


        For i = 0 To ds_.Rows.Count - 1

            Dim koliko_ As Integer = ds_.Rows(i).Item(8)
            'sada nadi gde imas prvo broj
            Dim gde_, najveci As Integer
            gde_ = 1
            najveci = ds_.Rows(i).Item(1)
            For j = 2 To 6
                If najveci < ds_.Rows(i).Item(j) Then
                    najveci = ds_.Rows(i).Item(j)
                    gde_ = j
                End If
            Next
            cmd_.CommandText = "update kom_novostanjeparcela Set prazred_" & gde_ & " = " & ds_.Rows(i).Item(gde_) - ds_.Rows(i).Item(8) & " where idnp=" & ds_.Rows(i).Item(0)
            cmd_.ExecuteNonQuery()
            pb1.Value = i

        Next

        MsgBox("Kraj")

    End Sub

    Private Sub PodelaParcelaStarogStanjaIzSpiskaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles PodelaParcelaStarogStanjaIzSpiskaToolStripMenuItem.Click

        'ulaz:
        'dkp - drawing sa parcelama - ovo ide u layerName_parcele
        'csv file u formatu brojstareparcele, brojnoveparcelegradevinski, brojnoveparcelekomasacija 
        'baza radi citanja podataka starih parcela

        'upisje nove parcele u kom_parcele i kom_vezaparcelavlasnik i brise stare parcele iz istih baza

        opf_diag.FileName = ""
        opf_diag.Filter = "CSV File|*.csv"
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then MsgBox("Pronadite csv file") : Exit Sub

        'proverimo da li postoji drawing

        Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document
        Dim drwParcele As Manifold.Interop.Drawing
        Try
            drwParcele = doc_.ComponentSet(My.Settings.layerName_parcele)
        Catch ex As Exception

            MsgBox("Provrite podesavanje-nemate layer")
            Exit Sub
        End Try

        Dim freeFile_ As Integer = FreeFile()

        FileOpen(freeFile_, opf_diag.FileName, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()

        Do While Not EOF(freeFile_)

            Dim a_ = LineInput(freeFile_).Split(",")

            lbl_infoMain.Text = "Obradujem parcelu : " & a_(0)
            My.Application.DoEvents()

            Try
                conn_.Open()
            Catch ex As Exception

            End Try
            'sada mi iz baze za treba povrsina za ovaj broj parcele
            connComm.CommandText = "select idparc, (hektari*10000+ari*100+metri) P from kom_parcele where deoparcele=0 and obrisan=0 and brparcelef='" & a_(0) & "'"
            Dim myreader_ As MySqlDataReader = connComm.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
            Dim idParcOld_, povrsinaSuma As Integer
            If myreader_.HasRows Then
                myreader_.Read() : idParcOld_ = myreader_.GetValue(0) : povrsinaSuma = myreader_.GetValue(1) : myreader_.Close()

                'sada ti treba da graficke povrsine
                Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("pronadiPovrsine")
                qvr_.Text = "SELECT [brparcelef], round( [Area (I)]*(SELECT ((" & povrsinaSuma & "-sum(round([Area (I)])))/" & povrsinaSuma & ") koef_ FROM [" & drwParcele.Name & "] WHERE [brparcelef] in (" & Chr(34) & a_(1) & Chr(34) & "," & Chr(34) & a_(2) & Chr(34) & "))) + round([Area (I)]) FROM [" & drwParcele.Name & "] WHERE [brparcelef] in  (" & Chr(34) & a_(1) & Chr(34) & "," & Chr(34) & a_(2) & Chr(34) & ")"
                qvr_.RunEx(True)
                'mora da ima dva rekorda iimas izravnate povrsine za deoparcele = 0
                For i = 0 To qvr_.Table.RecordSet.Count - 1
                    'sada ide sql sa upitom! i novom povrsinom
                    'kao ulaz treba mi - brojparcele, podbroj, hektari, ari, metri
                    Dim parc_ = qvr_.Table.RecordSet.Item(i).DataText(1).Split("/") ' sada imas broj parcele i podbroj
                    'sada da vidimo sta se desava sa povrsinom!
                    Dim povrs_ = povrsinaParceleUkras(qvr_.Table.RecordSet.Item(i).DataText(2)).Split(" ")
                    'sada da vidimo sta ima 
                    Dim hek_, ari_, met_ As Integer
                    Select Case povrs_.Length
                        Case 1
                            hek_ = 0 : ari_ = 0 : met_ = povrs_(0)
                        Case 2
                            hek_ = 0 : ari_ = povrs_(0) : met_ = povrs_(1)
                        Case 3
                            hek_ = povrs_(0) : ari_ = povrs_(1) : met_ = povrs_(2)
                    End Select
                    'sad amozes da sklopis celu pricu
                    Dim stsql_ = "INSERT into kom_parcele (SKATOPST, BROJPARC, PODBROJ, DEOPARCELE, BROJPLANA, SKICA, god, ULICAPOTES, SULICE, BROJ, UZBROJ, POTES, HEKTARI, ARI, METRI, idGradevinsko, MANUAL, BROJPOSLISTA, RASPRAVNIZAPISNIK, UKOMASACIJI, uneo, datumUnosa, brParceleF, obrisan) SELECT SKATOPST, " & parc_(0) & ", " & If(parc_.Length = 1, 0, parc_(1)) & ", DEOPARCELE, BROJPLANA, SKICA, god, ULICAPOTES, SULICE, BROJ, UZBROJ, POTES, " & hek_ & ", " & ari_ & ", " & met_ & ", idGradevinsko, MANUAL, BROJPOSLISTA, RASPRAVNIZAPISNIK, UKOMASACIJI, 2, datumUnosa, " & Chr(34) & If(parc_.Length = 2, parc_(0) & "/" & parc_(1), parc_(0)) & Chr(34) & ", 0 FROM kom_parcele WHERE brParceleF = " & Chr(34) & a_(0) & Chr(34) & " AND DEOPARCELE = 0"
                    connComm.CommandText = stsql_
                    Try
                        conn_.Open()
                    Catch ex As Exception

                    End Try
                    If connComm.ExecuteNonQuery() = 0 Then
                        'ove verivatno ima neki problem
                        MsgBox("nije napravio update: " & stsql_)

                    End If
                    'zavrseno sa update-om kom_parcele sada treba napraviti update u kom_vezaparcelavlasnik
                    'pronades poslednji koji treba da se uploadujem
                    connComm.CommandText = "select max(idparc) from kom_parcele"
                    'sada idemo dalje
                    myreader_ = connComm.ExecuteReader(System.Data.CommandBehavior.CloseConnection)

                    Dim poslednjiID_ As Integer = 0
                    If myreader_.HasRows Then
                        'ovde ne bi trebalo da 
                        myreader_.Read()
                        poslednjiID_ = myreader_.GetValue(0)
                    End If
                    myreader_.Close()
                    Try
                        conn_.Open()
                    Catch ex As Exception

                    End Try
                    'sada idemo dalje - odnosno mozemo da predemo na kom_tablevlasnik
                    stsql_ = "insert into kom_vezaparcelavlasnik (idParcele, idVlasnika, OBLIKSVOJINE, VRSTAPRAVA, OBIMPRAVA, Udeo, uneo ,datumUnosa, obrisan, koefUdeo, idiskazzemljista,idpl) SELECT " & poslednjiID_ & ", idVlasnika, OBLIKSVOJINE, VRSTAPRAVA, OBIMPRAVA, Udeo, uneo ,datumUnosa, 0, koefUdeo, idiskazzemljista, idpl FROM kom_vezaparcelavlasnik where idParcele=(SELECT idparc FROM kom_parcele WHERE brParceleF=" & Chr(34) & a_(0) & Chr(34) & " and DEOPARCELE=0)"
                    'sada imas ovo i mozes da izvrsis update
                    connComm.CommandText = stsql_
                    connComm.ExecuteNonQuery()
                    'sada idemo na sledeci korak a to je deo parcele!

                    Dim koeficijent_ = Val(qvr_.Table.RecordSet.Item(i).DataText(2).ToString) / povrsinaSuma

                    'stsql_ = "insert into kom_parcele (skatopst, brojparc, podbroj, SKULTURE, hektari, ari, metri, deoparcele, brparcelef) select skatopst, " & parc_(0) & ", " & If(parc_.Length = 1, 0, parc_(1)) & ", SKULTURE, hektari*" & koeficijent_ & ", ari*" & koeficijent_ & ", metri*" & koeficijent_ & ", 1, " & Chr(34) & If(parc_.Length = 2, parc_(0) & "/" & parc_(1), parc_(0)) & Chr(34) & " from kom_parcele where deoparcele=1 and brparcelef=" & Chr(34) & a_(0) & Chr(34)
                    stsql_ = "insert into kom_parcele (skatopst, brojparc, podbroj, SKULTURE, hektari, ari, metri, deoparcele, brparcelef) SELECT SKATOPST, " & parc_(0) & ", " & If(parc_.Length = 1, 0, parc_(1)) & ", SKULTURE, CASE length(P) WHEN 1 THEN '00' WHEN 2 THEN '00' WHEN 3 THEN '00' WHEN 4 THEN '00' ELSE LEFT (P, length(P) - 4) END AS hektari, CASE length(p) WHEN 1 THEN '00' WHEN 2 THEN '00' WHEN 3 THEN LEFT (P, 1) WHEN 4 THEN LEFT (P, 2) ELSE LEFT (RIGHT(P, 4), 2) END AS ari, RIGHT (P, 2) AS metri, 1, " & Chr(34) & If(parc_.Length = 2, parc_(0) & "/" & parc_(1), parc_(0)) & Chr(34) & " FROM ( SELECT skatopst, 322, 5, SKULTURE, round(( hektari * 10000 + ari * 100 + metri )*" & koeficijent_ & ") as P , 1, " & Chr(34) & If(parc_.Length = 2, parc_(0) & "/" & parc_(1), parc_(0)) & Chr(34) & " FROM kom_parcele WHERE deoparcele = 1 AND brparcelef = " & Chr(34) & a_(0) & Chr(34) & ") A"
                    connComm.CommandText = stsql_
                    connComm.ExecuteNonQuery()
                    'to bi bilo to

                    'sada idemmo da brisemo
                    connComm.CommandText = "update kom_parcele set obrisan=1 where brparcelef=" & Chr(34) & a_(0) & Chr(34)
                    connComm.ExecuteNonQuery()

                    'sada je pitanje kako da obrisem iz veza parcela vlasnik
                    connComm.CommandText = "update kom_vezaparcelavlasnik set obrisan=1 where idparcele=" & idParcOld_
                    connComm.ExecuteNonQuery()

                Next

            Else
                MsgBox("Za parcelu " & a_(0) & " nemate rekord u bazi proverite")
                myreader_.Close()
            End If

        Loop

        conn_.Close() : conn_ = Nothing
        connComm = Nothing

        FileClose()
        MsgBox("Kraj")

    End Sub

    Private Sub ZaokruziPovrsineKnjigaFondaMaseStarogStanjaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ZaokruziPovrsineKnjigaFondaMaseStarogStanjaToolStripMenuItem.Click
        'ovo zaokruzuje povrsine po procembeni razredima da se slozi sa povrsinom parcele

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("SELECT idParc AS idnp, prazred_1, prazred_2, prazred_3, prazred_4, prazred_5, prazred_6, prazred_7, prazred_8, prazred_neplodno, (( prazred_1 + prazred_2 + prazred_3 + prazred_4 + prazred_5 + prazred_6 + Prazred_7 + Prazred_8 + prazred_neplodno ) - Povrsina ) Raz FROM kom_kfmss WHERE (( prazred_1 + prazred_2 + prazred_3 + prazred_4 + prazred_5 + prazred_6 + Prazred_7 + Prazred_8 + prazred_neplodno ) - Povrsina ) IN (1, - 1, 2 ,- 2) ORDER BY brparcele", conn_)
        Dim adapter_ As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ds_ As New DataTable
        conn_.Open()
        Dim cmd_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        adapter_.SelectCommand = comm_
        adapter_.Fill(ds_)

        pb1.Maximum = ds_.Rows.Count
        pb1.Value = 0


        For i = 0 To ds_.Rows.Count - 1

            Dim koliko_ As Integer = ds_.Rows(i).Item(10)
            'sada nadi gde imas prvo broj
            Dim gde_, najveci As Integer
            gde_ = 1
            najveci = ds_.Rows(i).Item(1)
            For j = 2 To 8
                If najveci < ds_.Rows(i).Item(j) Then
                    najveci = ds_.Rows(i).Item(j)
                    gde_ = j
                End If
            Next
            cmd_.CommandText = "update kom_kfmss set prazred_" & gde_ & " = " & ds_.Rows(i).Item(gde_) - ds_.Rows(i).Item(10) & " where idparc=" & ds_.Rows(i).Item(0)
            cmd_.ExecuteNonQuery()
            pb1.Value = i

        Next

        MsgBox("Kraj")
    End Sub

    Private Sub ExportToDxfPointAnd3DPolylineToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ExportToDxfPointAnd3DPolylineToolStripMenuItem.Click

        'funckija napravljena za potrebe interpolacije na surface 3 dlinije
        'kada imporujes tacke pravi dva layer-a tac i tac st

        'layer sa tackama pntObelezavanje

        'layerName_ProcembeniRazredi  - breaklines linije
        'layerName_ParceleNadela- breaklines tacke
        '

        Dim freefile_ As Integer = FreeFile()
        sf_diag.FileName = ""
        sf_diag.DefaultExt = "DXF file *.dxf | *.dxc"
        sf_diag.ShowDialog()
        If sf_diag.FileName = "" Then MsgBox("morate defnisati izlazni file") : Exit Sub


        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        'sada idemo pisanje
        FileOpen(freefile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("zaBrisanje")


        'idemo prvo tacke
        qvr_.Text = "select [x (i)],[Y (i)], ([Height]+[Z]) from [" & My.Settings.layerName_parcele & "]"
        qvr_.RunEx(True)
        pb1.Maximum = qvr_.Table.RecordSet.Count
        pb1.Value = 0

        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMPOST")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMAPOST")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMALT")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMALTD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMALTF")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "25.3999999999999")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMLFAC")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMTOFL")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMTVP")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMTIX")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMSOXD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMSAH")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMBLK1")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMBLK2")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMSTYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "STANDARD")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMCLRD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMCLRE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMCLRT")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMTFAC")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$DIMGAP")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0.09")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$LUNITS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$LUPREC")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SKETCHINC")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0.1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$FILLETRAD")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$AUNITS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$AUPREC")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$MENU")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, ".")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$ELEVATION")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PELEVATION")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$THICKNESS")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$LIMCHECK")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$BLIPMODE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$CHAMFERA")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$CHAMFERB")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SKPOLY")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$TDCREATE")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "2457762.96957569")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$TDUPDATE")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "2457762.96996701")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$TDINDWG")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0.0003934954")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$TDUSRTIMER")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0.0003934722")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USRTIMER")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$ANGBASE")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$ANGDIR")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PDMODE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PDSIZE")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PLINEWID")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$COORDS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SPLFRAME")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SPLINETYPE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SPLINESEGS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "8")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$ATTDIA")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$ATTREQ")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$HANDLING")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$HANDSEED")
        PrintLine(freefile_, "5")
        PrintLine(freefile_, "308")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SURFTAB1")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SURFTAB2")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SURFTYPE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SURFU")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SURFV")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$UCSNAME")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$UCSORG")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$UCSXDIR")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$UCSYDIR")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PUCSNAME")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PUCSORG")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PUCSXDIR")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PUCSYDIR")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERI1")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERI2")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERI3")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERI4")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERI5")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERR1")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERR2")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERR3")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERR4")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$USERR5")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$WORLDVIEW")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SHADEDGE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$SHADEDIF")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$TILEMODE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$MAXACTVP")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "64")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PLIMCHECK")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PEXTMIN")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PEXTMAX")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PLIMMIN")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PLIMMAX")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "12")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$UNITMODE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$VISRETAIN")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PLINEGEN")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "9")
        PrintLine(freefile_, "$PSLTSCALE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDSEC")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "SECTION")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "TABLES")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "VPORT")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "VPORT")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "*ACTIVE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "11")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "21")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "12")
        PrintLine(freefile_, "16.8348750732359")
        PrintLine(freefile_, "22")
        PrintLine(freefile_, "11.7250831267482")
        PrintLine(freefile_, "13")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "23")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "14")
        PrintLine(freefile_, "0.5")
        PrintLine(freefile_, "24")
        PrintLine(freefile_, "0.5")
        PrintLine(freefile_, "15")
        PrintLine(freefile_, "0.5")
        PrintLine(freefile_, "25")
        PrintLine(freefile_, "0.5")
        PrintLine(freefile_, "16")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "26")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "36")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "17")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "27")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "37")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "7.26824793402298")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "1.69654289372599")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "43")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "44")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "51")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "72")
        PrintLine(freefile_, "1000")
        PrintLine(freefile_, "73")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "74")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "75")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "76")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "77")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "78")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "LTYPE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LTYPE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "CONTINUOUS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "Solid line")
        PrintLine(freefile_, "72")
        PrintLine(freefile_, "65")
        PrintLine(freefile_, "73")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LTYPE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "PHANTOM2")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "Phantom (.5x) ___ _ _ ___ _ _ ___ _ _ ___ _ _")
        PrintLine(freefile_, "72")
        PrintLine(freefile_, "65")
        PrintLine(freefile_, "73")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "1.25")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "0.625")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "-0.125")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "0.125")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "-0.125")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "0.125")
        PrintLine(freefile_, "49")
        PrintLine(freefile_, "-0.125")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "LAYER")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LAYER")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "62")
        PrintLine(freefile_, "7")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "CONTINUOUS")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LAYER")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "DEFPOINTS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "62")
        PrintLine(freefile_, "7")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "CONTINUOUS")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LAYER")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "LIN-1")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "62")
        PrintLine(freefile_, "92")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "CONTINUOUS")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "LAYER")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "TAC-1")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "62")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "CONTINUOUS")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "STYLE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "STYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "STANDARD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "0.2")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "txt")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "STYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ANNOTATIVE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "0.2")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "txt")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "STYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "LEGEND")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "50")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "0.2")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "txt")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "VIEW")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "UCS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "16")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACADANNOPO")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACADANNOTATIVE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD_DSTYLE_DIMJAG")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD_DSTYLE_DIMTALN")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD_MLEADERVER")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD_NAV_VCDISPLAY")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACMAPDMDISPLAYSTYLEREGAPP")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ADE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "DCO15")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ADE_PROJECTION")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "MAPGWS")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAD_PSEXT")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACAECLAYERSTANDARD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ACCMTRANSPARENCY")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "APPID")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "MapManagementAppName")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "TABLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "DIMSTYLE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "DIMSTYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "STANDARD")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "5")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "7")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "0.0625")
        PrintLine(freefile_, "43")
        PrintLine(freefile_, "0.38")
        PrintLine(freefile_, "44")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "45")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "46")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "47")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "48")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "140")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "141")
        PrintLine(freefile_, "0.09")
        PrintLine(freefile_, "142")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "143")
        PrintLine(freefile_, "25.3999999999999")
        PrintLine(freefile_, "144")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "145")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "146")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "147")
        PrintLine(freefile_, "0.09")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "72")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "73")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "74")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "75")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "76")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "77")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "78")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "170")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "171")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "172")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "173")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "174")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "175")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "176")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "177")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "178")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "DIMSTYLE")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ANNOTATIVE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "4")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "5")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "6")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "7")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "40")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "41")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "42")
        PrintLine(freefile_, "0.0625")
        PrintLine(freefile_, "43")
        PrintLine(freefile_, "0.38")
        PrintLine(freefile_, "44")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "45")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "46")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "47")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "48")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "140")
        PrintLine(freefile_, "0.18")
        PrintLine(freefile_, "141")
        PrintLine(freefile_, "0.09")
        PrintLine(freefile_, "142")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "143")
        PrintLine(freefile_, "25.3999999999999")
        PrintLine(freefile_, "144")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "145")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "146")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "147")
        PrintLine(freefile_, "0.09")
        PrintLine(freefile_, "71")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "72")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "73")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "74")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "75")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "76")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "77")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "78")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "170")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "171")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "172")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "173")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "174")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "175")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "176")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "177")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "178")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDTAB")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDSEC")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "SECTION")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "BLOCKS")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "BLOCK")
        PrintLine(freefile_, "8")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "$MODEL_SPACE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "$MODEL_SPACE")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDBLK")
        PrintLine(freefile_, "5")
        PrintLine(freefile_, "21")
        PrintLine(freefile_, "8")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "BLOCK")
        PrintLine(freefile_, "67")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "8")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "$PAPER_SPACE")
        PrintLine(freefile_, "70")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "10")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "20")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "30")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "3")
        PrintLine(freefile_, "$PAPER_SPACE")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDBLK")
        PrintLine(freefile_, "5")
        PrintLine(freefile_, "27A")
        PrintLine(freefile_, "67")
        PrintLine(freefile_, "1")
        PrintLine(freefile_, "8")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "ENDSEC")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "SECTION")
        PrintLine(freefile_, "2")
        PrintLine(freefile_, "ENTITIES")
        PrintLine(freefile_, "0")



        'TACKE
        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            PrintLine(freefile_, "POINT")
            'PrintLine(freefile_, "5")
            'PrintLine(freefile_, Hex(i))
            PrintLine(freefile_, "8")
            PrintLine(freefile_, "TAC-1")
            PrintLine(freefile_, "10")
            PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(1))
            PrintLine(freefile_, "20")
            PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(2))
            PrintLine(freefile_, "30")
            PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(3))
            PrintLine(freefile_, "0")
        Next

        'u sledeca dva podesavanja - procembeni razredi su linije
        'tacke u projektovane table

        '3DPOLYLINE
        Dim qvr2 As Manifold.Interop.Query = doc.NewQuery("dva")
        qvr2.Text = "select [id] from [" & My.Settings.layerName_ProcembeniRazredi & "] order by [id]"
        qvr2.RunEx(True)
        pb1.Maximum = qvr2.Table.RecordSet.Count : pb1.Value = 0
        For i = 0 To qvr2.Table.RecordSet.Count - 1
            PrintLine(freefile_, "POLYLINE")
            PrintLine(freefile_, 8)
            PrintLine(freefile_, "LIN-1")
            PrintLine(freefile_, 66)
            PrintLine(freefile_, 1)
            PrintLine(freefile_, 10)
            PrintLine(freefile_, 0.0)
            PrintLine(freefile_, 20)
            PrintLine(freefile_, 0.0)
            PrintLine(freefile_, 30)
            PrintLine(freefile_, 0.0)
            PrintLine(freefile_, 70)
            PrintLine(freefile_, 8)
            PrintLine(freefile_, 0)
            qvr_.Text = "select [x (i)],[Y (i)], ([Height]+[Z]) from [" & My.Settings.layerName_table & "] where [ID_Parent]=" & qvr2.Table.RecordSet.Item(i).DataText(1) & " order by [id]"
            qvr_.RunEx(True)
            For j = 0 To qvr_.Table.RecordSet.Count - 1
                PrintLine(freefile_, "VERTEX")
                PrintLine(freefile_, 8)
                PrintLine(freefile_, "LIN-1")
                PrintLine(freefile_, 10)
                PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(1))
                PrintLine(freefile_, 20)
                PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(2))
                PrintLine(freefile_, 30)
                PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(3))
                PrintLine(freefile_, 70)
                PrintLine(freefile_, 32)
                PrintLine(freefile_, 0)
            Next

            PrintLine(freefile_, "SEQEND")
            PrintLine(freefile_, 8)
            PrintLine(freefile_, "LIN-1")
            PrintLine(freefile_, 0)

            pb1.Value = i
        Next


        PrintLine(freefile_, "ENDSEC")
        PrintLine(freefile_, "0")
        PrintLine(freefile_, "EOF")
        FileClose()
        pb1.Value = 0
        qvr_ = Nothing : doc = Nothing
        MsgBox("Kraj")
    End Sub


    Private Sub ObjektiToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ObjektiToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Me.Cursor = Cursors.WaitCursor

        'u layeru table se nalaze objekti

        Dim drw_ As Manifold.Interop.Drawing

        Try
            drw_ = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox(ex.Message)
            doc = Nothing
            Me.Cursor = Cursors.Default
            Exit Sub
        End Try

        Dim velicinaBuffera As Double = InputBox("Unesite velicinu pranog prostora od tacke do pocetka geodetke linije ", "Unos", "1.25")

        'sada idemo redom

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("trenutni")
        Dim qvr2_ As Manifold.Interop.Query = doc.NewQuery("listing")
        Dim qvrLine As Manifold.Interop.Query = doc.NewQuery("nacrtaj liniju")
        qvr2_.Text = "select [id] from [" & My.Settings.layerName_table & "]"
        qvr2_.RunEx(True)

        pb1.Value = 0 : pb1.Maximum = qvr2_.Table.RecordSet.Count

        For i = 0 To qvr2_.Table.RecordSet.Count - 1
            qvr_.Text = "SELECT id1, Distance(pnt1_, pnt2_) as d_ , CentroidX(pnt1_) x1,CentroidY(pnt1_) y1,CentroidX(pnt2_) x2,CentroidY(pnt2_) y2 FROM (SELECT [ID] as id1,pnt1_ 	FROM [" & My.Settings.layerName_table & "] SPLIT by Coords([ID]) as pnt1_ ) A  , (SELECT [ID] as id2,pnt2_ 	FROM [" & My.Settings.layerName_table & "] SPLIT by Coords([ID]) as pnt2_ ) B  WHERE A.id1=B.id2 and A.pnt1_<>B.pnt2_ and A.id1=" & qvr2_.Table.RecordSet.Item(i).DataText(1) & " ORDER by id1,d_ desc "
            qvr_.RunEx(True)

            'sada idemo redom treba nam cetiri tacke - znaci treba da ima najmanje cetiri tacke

            If qvr_.Table.RecordSet.Count < 4 Then

                MsgBox("poligon sa [id]=" & qvr2_.Table.RecordSet.Item(i).DataText(1) & "ima manje od cetiri tacke")

            Else

                'sada mozemo da kartiramo ovo ! i to u isti drawing
                qvrLine.Text = "insert into [" & My.Settings.layerName_table & "] ([Geom (I)]) values (newline(AssignCoordSys(newpoint(" & qvr_.Table.RecordSet.Item(1).DataText(3) & "," & qvr_.Table.RecordSet.Item(1).DataText(4) & "),COORDSYS(" & Chr(34) & drw_.Name & Chr(34) & " as COMPONENT)), AssignCoordSys(newpoint(" & qvr_.Table.RecordSet.Item(1).DataText(5) & "," & qvr_.Table.RecordSet.Item(1).DataText(6) & "),COORDSYS(" & Chr(34) & drw_.Name & Chr(34) & " as COMPONENT))))"
                qvrLine.RunEx(True)
                qvrLine.Text = "insert into [" & My.Settings.layerName_table & "] ([Geom (I)]) values (newline(AssignCoordSys(newpoint(" & qvr_.Table.RecordSet.Item(3).DataText(3) & "," & qvr_.Table.RecordSet.Item(3).DataText(4) & "),COORDSYS(" & Chr(34) & drw_.Name & Chr(34) & " as COMPONENT)), AssignCoordSys(newpoint(" & qvr_.Table.RecordSet.Item(3).DataText(5) & "," & qvr_.Table.RecordSet.Item(3).DataText(6) & "),COORDSYS(" & Chr(34) & drw_.Name & Chr(34) & " as COMPONENT))))"
                qvrLine.RunEx(True)

            End If

            pb1.Value = i

        Next

        doc.Save()

        'sada idemo da sasecemo ovo

        'treba da kreiras tacke u svarkoj tacki polihona za sta ti treba 

        Dim analiz_ As Manifold.Interop.Analyzer = doc.NewAnalyzer


        Dim drwPnt_ As Manifold.Interop.Drawing = doc.NewDrawing("tackice", drw_.CoordinateSystem, True)

        Try
            analiz_.Points(drw_, drw_, drw_.ObjectSet)
            analiz_.RemoveDuplicates(drw_, drw_.ObjectSet)

            drw_.Copy(True)
            drwPnt_.Paste(False)

            analiz_.Buffers(drwPnt_, drwPnt_, drwPnt_.ObjectSet, velicinaBuffera)

            'brises tacke i poligone

            qvr_.Text = "delete from [" & My.Settings.layerName_table & "] where ispoint([id]) or isarea([id])"
            qvr_.RunEx(True)

            qvr_.Text = "update [tackice] set [Selection (I)]=false"
            qvr_.RunEx(True)

            analiz_.ClipSubtract(drw_, drw_.ObjectSet, drwPnt_.ObjectSet) 'nemam pojama sta ce da uradi

        Catch ex As Exception

        End Try

        doc.ComponentSet.Remove("trenutni")
        doc.ComponentSet.Remove("listing")
        doc.ComponentSet.Remove("nacrtaj liniju")
        doc.Save()
        doc = Nothing
        Me.Cursor = Cursors.Default
        pb1.Value = 0
        MsgBox("Kraj")

    End Sub

    Private Sub podelaNaListoveGKTS_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGKTS.Click
        podelaNaListoveGK(50000)
    End Sub

    Private Sub podelaNaListoveGK5000_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGK5000.Click
        podelaNaListoveGK(5000)
    End Sub

    Private Sub podelaNaListoveGK2500_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGK2500.Click
        podelaNaListoveGK(2500)
    End Sub

    Private Sub podelaNaListoveGK2000_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGK2000.Click
        podelaNaListoveGK(2000)
    End Sub

    Private Sub podelaNaListoveGK1000_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGK1000.Click
        podelaNaListoveGK(1000)
    End Sub

    Private Sub podelaNaListoveGK500_Click(sender As Object, e As System.EventArgs) Handles podelaNaListoveGK500.Click
        podelaNaListoveGK(500)
    End Sub

    'Private Sub Export3DPolylineToDXFToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles Export3DPolylineToDXFToolStripMenuItem.Click
    '    'funckija napravljena za potrebe interpolacije na surface 3 dlinije
    '    'kada imporujes tacke pravi dva layer-a tac i tac st

    '    'layer sa tackama pntObelezavanje

    '    'layerName_ProcembeniRazredi  - breaklines linije
    '    'layerName_ParceleNadela- breaklines tacke
    '    '

    '    Dim freefile_ As Integer = FreeFile()
    '    sf_diag.FileName = ""
    '    sf_diag.DefaultExt = "DXF file *.dxf | *.dxc"
    '    sf_diag.ShowDialog()
    '    If sf_diag.FileName = "" Then MsgBox("morate defnisati izlazni file") : Exit Sub


    '    Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

    '    'sada idemo pisanje
    '    FileOpen(freefile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

    '    Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("zaBrisanje")


    '    'idemo prvo tacke
    '    qvr_.Text = "select [x (i)],[Y (i)], ([Height]+[Z]) from [" & My.Settings.layerName_parcele & "]"
    '    qvr_.RunEx(True)
    '    pb1.Maximum = qvr_.Table.RecordSet.Count
    '    pb1.Value = 0

    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMPOST")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMAPOST")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMALT")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMALTD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMALTF")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "25.3999999999999")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMLFAC")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMTOFL")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMTVP")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMTIX")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMSOXD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMSAH")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMBLK1")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMBLK2")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMSTYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "STANDARD")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMCLRD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMCLRE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMCLRT")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMTFAC")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$DIMGAP")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0.09")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$LUNITS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$LUPREC")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SKETCHINC")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0.1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$FILLETRAD")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$AUNITS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$AUPREC")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$MENU")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, ".")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$ELEVATION")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PELEVATION")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$THICKNESS")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$LIMCHECK")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$BLIPMODE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$CHAMFERA")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$CHAMFERB")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SKPOLY")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$TDCREATE")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "2457762.96957569")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$TDUPDATE")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "2457762.96996701")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$TDINDWG")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0.0003934954")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$TDUSRTIMER")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0.0003934722")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USRTIMER")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$ANGBASE")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$ANGDIR")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PDMODE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PDSIZE")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PLINEWID")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$COORDS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SPLFRAME")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SPLINETYPE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SPLINESEGS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "8")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$ATTDIA")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$ATTREQ")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$HANDLING")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$HANDSEED")
    '    PrintLine(freefile_, "5")
    '    PrintLine(freefile_, "308")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SURFTAB1")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SURFTAB2")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SURFTYPE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SURFU")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SURFV")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$UCSNAME")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$UCSORG")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$UCSXDIR")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$UCSYDIR")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PUCSNAME")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PUCSORG")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PUCSXDIR")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PUCSYDIR")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERI1")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERI2")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERI3")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERI4")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERI5")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERR1")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERR2")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERR3")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERR4")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$USERR5")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$WORLDVIEW")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SHADEDGE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$SHADEDIF")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$TILEMODE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$MAXACTVP")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "64")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PLIMCHECK")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PEXTMIN")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PEXTMAX")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PLIMMIN")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PLIMMAX")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "12")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$UNITMODE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$VISRETAIN")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PLINEGEN")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "9")
    '    PrintLine(freefile_, "$PSLTSCALE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDSEC")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "SECTION")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "TABLES")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "VPORT")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "VPORT")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "*ACTIVE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "11")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "21")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "12")
    '    PrintLine(freefile_, "16.8348750732359")
    '    PrintLine(freefile_, "22")
    '    PrintLine(freefile_, "11.7250831267482")
    '    PrintLine(freefile_, "13")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "23")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "14")
    '    PrintLine(freefile_, "0.5")
    '    PrintLine(freefile_, "24")
    '    PrintLine(freefile_, "0.5")
    '    PrintLine(freefile_, "15")
    '    PrintLine(freefile_, "0.5")
    '    PrintLine(freefile_, "25")
    '    PrintLine(freefile_, "0.5")
    '    PrintLine(freefile_, "16")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "26")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "36")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "17")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "27")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "37")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "7.26824793402298")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "1.69654289372599")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "43")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "44")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "51")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "72")
    '    PrintLine(freefile_, "1000")
    '    PrintLine(freefile_, "73")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "74")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "75")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "76")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "77")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "78")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "LTYPE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LTYPE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "CONTINUOUS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "Solid line")
    '    PrintLine(freefile_, "72")
    '    PrintLine(freefile_, "65")
    '    PrintLine(freefile_, "73")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LTYPE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "PHANTOM2")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "Phantom (.5x) ___ _ _ ___ _ _ ___ _ _ ___ _ _")
    '    PrintLine(freefile_, "72")
    '    PrintLine(freefile_, "65")
    '    PrintLine(freefile_, "73")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "1.25")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "0.625")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "-0.125")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "0.125")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "-0.125")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "0.125")
    '    PrintLine(freefile_, "49")
    '    PrintLine(freefile_, "-0.125")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "LAYER")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LAYER")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "62")
    '    PrintLine(freefile_, "7")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "CONTINUOUS")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LAYER")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "DEFPOINTS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "62")
    '    PrintLine(freefile_, "7")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "CONTINUOUS")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LAYER")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "LIN-1")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "62")
    '    PrintLine(freefile_, "92")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "CONTINUOUS")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "LAYER")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "TAC-1")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "62")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "CONTINUOUS")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "STYLE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "STYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "STANDARD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "0.2")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "txt")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "STYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ANNOTATIVE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "0.2")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "txt")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "STYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "LEGEND")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "50")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "0.2")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "txt")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "VIEW")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "UCS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "16")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACADANNOPO")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACADANNOTATIVE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD_DSTYLE_DIMJAG")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD_DSTYLE_DIMTALN")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD_MLEADERVER")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD_NAV_VCDISPLAY")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACMAPDMDISPLAYSTYLEREGAPP")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ADE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "DCO15")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ADE_PROJECTION")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "MAPGWS")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAD_PSEXT")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACAECLAYERSTANDARD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ACCMTRANSPARENCY")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "APPID")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "MapManagementAppName")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "TABLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "DIMSTYLE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "DIMSTYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "STANDARD")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "5")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "7")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "0.0625")
    '    PrintLine(freefile_, "43")
    '    PrintLine(freefile_, "0.38")
    '    PrintLine(freefile_, "44")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "45")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "46")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "47")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "48")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "140")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "141")
    '    PrintLine(freefile_, "0.09")
    '    PrintLine(freefile_, "142")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "143")
    '    PrintLine(freefile_, "25.3999999999999")
    '    PrintLine(freefile_, "144")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "145")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "146")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "147")
    '    PrintLine(freefile_, "0.09")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "72")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "73")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "74")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "75")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "76")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "77")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "78")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "170")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "171")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "172")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "173")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "174")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "175")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "176")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "177")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "178")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "DIMSTYLE")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ANNOTATIVE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "4")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "5")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "6")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "7")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "40")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "41")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "42")
    '    PrintLine(freefile_, "0.0625")
    '    PrintLine(freefile_, "43")
    '    PrintLine(freefile_, "0.38")
    '    PrintLine(freefile_, "44")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "45")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "46")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "47")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "48")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "140")
    '    PrintLine(freefile_, "0.18")
    '    PrintLine(freefile_, "141")
    '    PrintLine(freefile_, "0.09")
    '    PrintLine(freefile_, "142")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "143")
    '    PrintLine(freefile_, "25.3999999999999")
    '    PrintLine(freefile_, "144")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "145")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "146")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "147")
    '    PrintLine(freefile_, "0.09")
    '    PrintLine(freefile_, "71")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "72")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "73")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "74")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "75")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "76")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "77")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "78")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "170")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "171")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "172")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "173")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "174")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "175")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "176")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "177")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "178")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDTAB")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDSEC")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "SECTION")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "BLOCKS")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "BLOCK")
    '    PrintLine(freefile_, "8")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "$MODEL_SPACE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "$MODEL_SPACE")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDBLK")
    '    PrintLine(freefile_, "5")
    '    PrintLine(freefile_, "21")
    '    PrintLine(freefile_, "8")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "BLOCK")
    '    PrintLine(freefile_, "67")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "8")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "$PAPER_SPACE")
    '    PrintLine(freefile_, "70")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "10")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "20")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "30")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "3")
    '    PrintLine(freefile_, "$PAPER_SPACE")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDBLK")
    '    PrintLine(freefile_, "5")
    '    PrintLine(freefile_, "27A")
    '    PrintLine(freefile_, "67")
    '    PrintLine(freefile_, "1")
    '    PrintLine(freefile_, "8")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "ENDSEC")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "SECTION")
    '    PrintLine(freefile_, "2")
    '    PrintLine(freefile_, "ENTITIES")
    '    PrintLine(freefile_, "0")

    '    ''TACKE
    '    'For i = 0 To qvr_.Table.RecordSet.Count - 1
    '    '    pb1.Value = i
    '    '    PrintLine(freefile_, "POINT")
    '    '    'PrintLine(freefile_, "5")
    '    '    'PrintLine(freefile_, Hex(i))
    '    '    PrintLine(freefile_, "8")
    '    '    PrintLine(freefile_, "TAC-1")
    '    '    PrintLine(freefile_, "10")
    '    '    PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(1))
    '    '    PrintLine(freefile_, "20")
    '    '    PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(2))
    '    '    PrintLine(freefile_, "30")
    '    '    PrintLine(freefile_, qvr_.Table.RecordSet.Item(i).DataText(3))
    '    '    PrintLine(freefile_, "0")
    '    'Next

    '    '3DPOLYLINE
    '    Dim qvr2 As Manifold.Interop.Query = doc.NewQuery("dva")
    '    qvr2.Text = "select [id] from [" & My.Settings.layerName_ProcembeniRazredi & "] order by [id]"
    '    qvr2.RunEx(True)
    '    pb1.Maximum = qvr2.Table.RecordSet.Count : pb1.Value = 0
    '    For i = 0 To qvr2.Table.RecordSet.Count - 1
    '        PrintLine(freefile_, "POLYLINE")
    '        PrintLine(freefile_, 8)
    '        PrintLine(freefile_, "LIN-1")
    '        PrintLine(freefile_, 66)
    '        PrintLine(freefile_, 1)
    '        PrintLine(freefile_, 10)
    '        PrintLine(freefile_, 0.0)
    '        PrintLine(freefile_, 20)
    '        PrintLine(freefile_, 0.0)
    '        PrintLine(freefile_, 30)
    '        PrintLine(freefile_, 0.0)
    '        PrintLine(freefile_, 70)
    '        PrintLine(freefile_, 8)
    '        PrintLine(freefile_, 0)
    '        qvr_.Text = "select [x (i)],[Y (i)], ([Height]+[Z]) from [" & My.Settings.layerName_ParceleNadela & "] where [ID_Parent]=" & qvr2.Table.RecordSet.Item(i).DataText(1) & " order by [id]"
    '        qvr_.RunEx(True)
    '        For j = 0 To qvr_.Table.RecordSet.Count - 1
    '            PrintLine(freefile_, "VERTEX")
    '            PrintLine(freefile_, 8)
    '            PrintLine(freefile_, "LIN-1")
    '            PrintLine(freefile_, 10)
    '            PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(1))
    '            PrintLine(freefile_, 20)
    '            PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(2))
    '            PrintLine(freefile_, 30)
    '            PrintLine(freefile_, qvr_.Table.RecordSet.Item(j).DataText(3))
    '            PrintLine(freefile_, 70)
    '            PrintLine(freefile_, 32)
    '            PrintLine(freefile_, 0)
    '        Next

    '        PrintLine(freefile_, "SEQEND")
    '        PrintLine(freefile_, 8)
    '        PrintLine(freefile_, "LIN-1")
    '        PrintLine(freefile_, 0)

    '        pb1.Value = i
    '    Next


    '    PrintLine(freefile_, "ENDSEC")
    '    PrintLine(freefile_, "0")
    '    PrintLine(freefile_, "EOF")
    '    FileClose()
    '    pb1.Value = 0
    '    qvr_ = Nothing : doc = Nothing
    '    MsgBox("Kraj")
    'End Sub

    Private Sub KomNovoStanjeProcembeniRazrediDeoParceleToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KomNovoStanjeProcembeniRazrediDeoParceleToolStripMenuItem.Click

        'ulaz: predpostavlja se da postoji drawing DKP_deoParcele, i procembeni razredi koje treba definisati u podesavanjima
        'i da postoji tabela u bazi kom_novostanjeparcela

        'prvo ovo pustis pa tek onda zaorkuzujes na jednu decimalu!!!!!!!!!!!!!!!!!!! 


        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        'prvo vidi koje se parcele ne slazu
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        conn_.Open()
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        Dim dtable_ As New DataTable
        comm_.CommandText = "SELECT id, brparcele, BrDelaParc FROM kom_novostanjeparcela WHERE abs(Povrsina - ( prazred_1 + prazred_2 + prazred_3 + prazred_4 + prazred_5 + prazred_6 + prazred_7 + prazred_8 + prazred_neplodno ))>2 ORDER BY brparcele"

        Dim adap_ As MySql.Data.MySqlClient.MySqlDataAdapter = New MySqlDataAdapter(comm_.CommandText, conn_)
        'selektuj sve parcele iz dkp_deoparcele koje su u ovom spisku i izkopiraj u novi drawing
        adap_.Fill(dtable_)

        'sada da vidimo kako da slozis spisak parcela 

        Dim drwProcembeni As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("procena")
        Dim drw_ As Manifold.Interop.Drawing

        Try
            drw_ = doc.NewDrawing("tempProcembeni", drwProcembeni.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove("tempProcembeni")
            drw_ = doc.NewDrawing("tempProcembeni", drwProcembeni.CoordinateSystem, True)
        End Try



        Try
            Dim tbl_ As Manifold.Interop.Table = drwProcembeni.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
        Catch ex1 As Exception
            'MsgBox(ex1.Message)
        End Try

        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology
        topPRazredi.Bind(drwProcembeni)
        topPRazredi.Build()

        pb1.Maximum = dtable_.Rows.Count
        pb1.Value = 0
        For i = 0 To dtable_.Rows.Count - 1
            'sada recimo ovo kopiras u neki drawing
            qvr_.Text = "delete from [tempProcembeni]"
            qvr_.RunEx(True)

            'sada bi trebalo iz dkp_deoparcele da kopiras ovde parcelu
            qvr_.Text = "insert into [tempProcembeni] ([Geom (I)]) (SELECT [Geom (i)] FROM [DKP_DeoParcele] where [brparcele]=" & dtable_.Rows(i).Item(1).ToString & " and [BrDelaParc]=" & dtable_.Rows(i).Item(2).ToString & ")"

            qvr_.RunEx(True)

            'kreiranje topologije

            Dim topTable As Manifold.Interop.Topology = doc.Application.NewTopology
            topTable.Bind(drw_)
            topTable.Build()

            topTable.DoIntersect(topPRazredi, "table_pr_razred")

            ' doc.Save() 'samo radi kontrole inace brisi

            'sada idemo na upisivanje

            'Dim drwRez_ As Manifold.Interop.Drawing = doc.ComponentSet("table_pr_razred")

            qvr_.Text = "select sum(round([area (i)])) as P, [procembeni] from [table_pr_razred] group by [procembeni]"
            qvr_.RunEx(True)
            'sada mozemo i update ne treba ti mnogo toga

            'sada bi prvo trebalo obrisati sve iz procembenih razreda za svaki slucaj
            comm_.CommandText = "update kom_novostanjeparcela set prazred_1=0,prazred_2=0,prazred_3=0,prazred_4=0,prazred_5=0,prazred_6=0,prazred_7=0,prazred_8=0,prazred_neplodno=0 where id=" & dtable_.Rows(i).Item(0).ToString
            comm_.ExecuteNonQuery()

            comm_.CommandText = "update kom_novostanjeparcela set "
            Dim poceo As Boolean = True
            For j = 0 To qvr_.Table.RecordSet.Count - 1
                Select Case qvr_.Table.RecordSet.Item(j).DataText(2)
                    Case "NP"
                        If poceo = True Then
                            poceo = False
                            comm_.CommandText = comm_.CommandText & " prazred_neplodno=" & qvr_.Table.RecordSet.Item(j).DataText(1)
                        Else
                            comm_.CommandText = comm_.CommandText & ", prazred_neplodno=" & qvr_.Table.RecordSet.Item(j).DataText(1)
                        End If


                    Case Else
                        If poceo = True Then
                            poceo = False
                            comm_.CommandText = comm_.CommandText & " prazred_" & qvr_.Table.RecordSet.Item(j).DataText(2) & "=" & qvr_.Table.RecordSet.Item(j).DataText(1)
                        Else
                            comm_.CommandText = comm_.CommandText & ", prazred_" & qvr_.Table.RecordSet.Item(j).DataText(2) & "=" & qvr_.Table.RecordSet.Item(j).DataText(1)
                        End If
                End Select
            Next
            'sada mozes da zatvrosi

            comm_.CommandText = comm_.CommandText & " where id=" & dtable_.Rows(i).Item(0).ToString
            comm_.ExecuteNonQuery()

            doc.ComponentSet.Remove("table_pr_razred")

            pb1.Value = i
        Next

        'napravis presek izdvojenih delova i procembenih razreda
        doc.ComponentSet.Remove("procena")

        'sracunavas povrsine i za onaj spisak upisujes u bazu!
        doc.Save()
        conn_.Close()
        adap_ = Nothing
        conn_ = Nothing
        comm_ = Nothing
        dtable_ = Nothing
        doc = Nothing
        '
        MsgBox("kraj")
    End Sub

    Private Sub PresekLinijaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles PresekLinijaToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        'pretpostavka sledeca u tackama su tacke koje mi trebaju
        'u linijama su linije kojre proveravam

        Dim drwPoints_ As Manifold.Interop.Drawing
        Dim drwLines As Manifold.Interop.Drawing

        Try
            drwPoints_ = doc.ComponentSet("Tacke")
        Catch ex As Exception
            MsgBox(ex.Message, vbOK)
            Exit Sub
        End Try

        Try
            drwLines = doc.ComponentSet("Linije")
        Catch ex As Exception
            MsgBox(ex.Message, vbOK)
            Exit Sub
        End Try

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("obrda")

        Dim qvr2_ As Manifold.Interop.Query = doc.NewQuery("unutra")

        qvr2_.Text = "SELECT [ID] FROM [Tacke] INNER join (SELECT b FROM (SELECT [Linije].[ID] a,[Tacke].[ID] b FROM [Linije],[Tacke] WHERE Contains([Linije].[ID],[Tacke].[ID]) ) GROUP by b having count(*)=2 ) C on [Tacke].[ID]=C.b"
        qvr2_.RunEx(True)

        pb1.Maximum = qvr2_.Table.RecordSet.Count
        pb1.Value = 0

        For i = 0 To qvr2_.Table.RecordSet.Count - 1

            qvr_.Text = "SELECT [Linije].[ID],[Linije].[Geom (I)] FROM [Linije],[Tacke] WHERE Touches([Linije].[ID],[Tacke].[ID]) AND [Tacke].[ID]=" & qvr2_.Table.RecordSet.Item(i).DataText(0) & " AND (select count(*) FROM (SELECT [Linije].* FROM [Linije],[Tacke] WHERE Touches([Linije].[ID],[Tacke].[ID]) AND [Tacke].[ID]=270321) AA GROUP by AA.[LAYER])=2"
            qvr_.RunEx(True)

            If qvr_.Table.RecordSet.Count <> 0 Then


            End If

        Next

    End Sub

    Private Sub TabelaPozivaLatinicaCirilicaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles TabelaPozivaLatinicaCirilicaToolStripMenuItem.Click

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        conn_.Open()
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Q','Љ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'W','Њ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'LJ','Љ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'NJ','Њ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'A','А');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'B','Б');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'V','В');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'G','Г');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'D','Д');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'E','Е');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Z','З');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'I','И');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'J','Ј');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'L','Л');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'M','М');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'N','Н');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'O','О');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'P','П');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'R','Р');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'S','С');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Т','Т');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'U','У');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'F','ф');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'H','Х');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'C','Ц');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'[','Ш');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,']','Ћ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'^','Ч');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'@','Ж');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Š','Ш');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Ć','Ћ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Č','Ч');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Ž','Ж');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'Đ','Ђ');" : comm_.ExecuteNonQuery()
        'comm_.CommandText = "update zapozivanje_fakticko set indikacije =replace(indikacije,'\','Ђ');" : comm_.ExecuteNonQuery()

        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Q','Љ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'W','Њ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'LJ','Љ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'NJ','Њ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'A','А');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'B','Б');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'V','В');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'G','Г');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'D','Д');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'E','Е');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Z','З');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'I','И');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'J','Ј');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'L','Л');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'M','М');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'N','Н');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'O','О');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'P','П');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'R','Р');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'S','С');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Т','Т');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'U','У');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'F','ф');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'H','Х');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'C','Ц');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'[','Ш');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,']','Ћ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'^','Ч');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'@','Ж');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Š','Ш');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Ć','Ћ');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Č','Ч');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Ž','Ж');" : comm_.ExecuteNonQuery()
        comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'Đ','Ђ');" : comm_.ExecuteNonQuery()
        'comm_.CommandText = "update zapozivanje_fakticko set mesto =replace(mesto,'\','Ђ');" : comm_.ExecuteNonQuery()

        If InputBox("Da promenim i tabelu fs_vlasnik?", "Pitanje", "1") = "1" Then

            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set prezime =replace(prezime,'@','Ж');" : comm_.ExecuteNonQuery()


            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ime =replace(ime,'@','Ж');" : comm_.ExecuteNonQuery()

            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set imeoca =replace(imeoca,'@','Ж');" : comm_.ExecuteNonQuery()

            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set mesto =replace(mesto,'@','Ж');" : comm_.ExecuteNonQuery()

            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set uzbroj =replace(uzbroj,'@','Ж');" : comm_.ExecuteNonQuery()

            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'Q','Љ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'W','Њ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'A','А');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'B','Б');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'V','В');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'G','Г');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'D','Д');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'E','Е');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'Z','З');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'I','И');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'J','Ј');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'L','Л');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'M','М');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'N','Н');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'O','О');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'P','П');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'R','Р');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'S','С');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'Т','Т');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'U','У');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'F','ф');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'H','Х');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'C','Ц');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'[','Ш');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,']','Ћ');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'^','Ч');" : comm_.ExecuteNonQuery()
            comm_.CommandText = "update fs_vlasnik set ulica =replace(ulica,'@','Ж');" : comm_.ExecuteNonQuery()

        End If

        conn_.Close()

        MsgBox("Kraj")

    End Sub

    Public Sub podelaNaListoveGK(razmera_ As Integer)

        'granica ti je data u layer-u centri moci jer dok radis postavi da je podela na listove u setting-u
        'radis podelu na listove za layer podela na listove i to za selektovano! a moramo da vidimo gde cemo to da smestimo :)

        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Me.Cursor = Cursors.WaitCursor

        lbl_infoMain.Text = "Brisem prethodne podele ako postoje"
        My.Application.DoEvents()

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podelaTS")
            doc.ComponentSet.Remove("podelaTS")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela5000")
            doc.ComponentSet.Remove("podela5000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2500")
            doc.ComponentSet.Remove("podela2500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2000")
            doc.ComponentSet.Remove("podela2000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela1000")
            doc.ComponentSet.Remove("podela1000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela500")
            doc.ComponentSet.Remove("podela500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Dim yts1_, xts1_, yts2_, xts2_ As Integer
        Dim y50001_, x50001_, y50002_, x50002_ As Integer
        Dim y25001_, x25001_, y25002_, x25002_ As Integer
        Dim y10001_, x10001_, y10002_, x10002_ As Integer
        Dim y5001_, x5001_, y5002_, x5002_ As Integer

        Dim NOMENKLATU_ As String = "7 "

        yts1_ = 0 : xts1_ = 0 : yts2_ = 0 : xts2_ = 0
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("cetiriTacke")
        qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]  WHERE [Selection (I)]=true ) SPLIT by Coords(geom_) as pnt_"
        qvr_.RunEx(True)

        If qvr_.Table.RecordSet.Count = 0 Then
            'ovo znaci da nema nista selektovano - uzimas sve sto je u drawingu i nastavljas dalje
            qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]) SPLIT by Coords(geom_) as pnt_"
            qvr_.RunEx(True)

        End If

        lbl_infoMain.Text = "Radim podelu za TS"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = 7320000 To 7680000 Step 22500
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 22500)) Then
                    For j = 4500000 To 5130000 Step 15000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 15000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                yts1_ = i : xts1_ = j
                                yts2_ = i + 22500 : xts2_ = j + 15000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > yts2_ Then yts2_ = i + 22500
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > xts2_ Then xts2_ = j + 15000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'ovo je za razmeru 50000 iz koje ide dalje podela
        Dim drwUTM_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_podelaNaListove)

        Dim drw_ As Manifold.Interop.Drawing = doc.NewDrawing("podelaTS", drwUTM_.CoordinateSystem, True)
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "NOMENKLATU"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText
        drw_.OwnedTable.ColumnSet.Add(col_)
        'ovde treba napraviti update to je najbolje
        Dim listaSlova = {"A2", "A1", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "N1", "N2"}
        'Dim listaSlova()


        Dim qvrInsert As Manifold.Interop.Query = doc.NewQuery("insertQ")
        ' NOMENKLATU_ = 

        For i = yts1_ To yts2_ - 22500 Step 22500
            For j = xts1_ To xts2_ - 15000 Step 15000

                Dim slovo_ = ""

                Try
                    slovo_ = "7 " & listaSlova(((i - 7320000) / 22500))
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try

                qvrInsert.Text = "insert into [podelaTS] ([NOMENKLATU],[Geom (I)]) values (" & Chr(34) & slovo_ & " " & (((j - 4500000) / 15000) + 1) & Chr(34) & ", AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 22500 & " " & j & "," & i + 22500 & " " & j + 15000 & "," & i & " " & j + 15000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"

                qvrInsert.RunEx(True)
                'ovde mora drugacije!
            Next
        Next

        If razmera_ = 50000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 5000"
        My.Application.DoEvents()

        'sada moze na 5000

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 22500 Step 2250
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 2250)) Then
                    For j = xts1_ To xts2_ Step 3000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 3000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y50001_ = i : x50001_ = j
                                y50002_ = i + 2250 : x50002_ = j + 3000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y50002_ Then y50002_ = i + 2250
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x50002_ Then x50002_ = j + 3000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela5000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        'ovde je veci problem za nomenklaturu jer moze da bude vise od jedne trig sekcije!!!
        For i = y50001_ To y50002_ - 2250 Step 2250
            For j = x50001_ To x50002_ - 3000 Step 3000

                qvrInsert.Text = "insert into [podela5000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 2250 & " " & j & "," & i + 2250 & " " & j + 3000 & "," & i & " " & j + 3000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1125 & "," & j + 1500 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 2250) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 3000
                    If n > 1 Then n = (n - 1) * 10 Else n = 0
                    qvrInsert.Text = "update [podela5000] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela5000])"
                    qvrInsert.RunEx(True)
                End If
            Next
        Next

        If razmera_ = 5000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        y25001_ = Nothing : y25002_ = Nothing : x25001_ = Nothing : x25002_ = Nothing
        'sada moze na 2500
        lbl_infoMain.Text = "Radim podelu za 2500"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 2250 Step 2250
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 2250)) Then
                    For j = xts1_ To xts2_ Step 1500
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1500)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y25001_ = i : x25001_ = j
                                y25002_ = i + 2250 : x25002_ = j + 1500
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 2250
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1500
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela2500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        'NOMENKLATU_ = NOMENKLATU_ & " " & ((y50002_ - y50001_) / 3000) * ((x50002_ - x50001_) / 2000)
        For i = y25001_ To y25002_ - 2250 Step 2250
            For j = x25001_ To x25002_ - 1500 Step 1500
                qvrInsert.Text = "insert into [podela2500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 2250 & " " & j & "," & i + 2250 & " " & j + 1500 & "," & i & " " & j + 1500 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1125 & "," & j + 750 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 2250) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1500
                    If n >= 1 Then n = (n - 1) * 10 Else n = 0
                    qvrInsert.Text = "update [podela2500] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela2500])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 2500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        'sada moze na 2000
        lbl_infoMain.Text = "Radim podelu za 2000"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 1500 Step 1500
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 1500)) Then
                    For j = xts1_ To xts2_ Step 1000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y25001_ = i : x25001_ = j
                                y25002_ = i + 1500 : x25002_ = j + 1000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 1500
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela2000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y25001_ To y25002_ - 1500 Step 1500
            For j = x25001_ To x25002_ - 1000 Step 1000
                qvrInsert.Text = "insert into [podela2000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 1500 & " " & j & "," & i + 1500 & " " & j + 1000 & "," & i & " " & j + 1000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 750 & "," & j + 500 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 1500) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1000
                    If n >= 1 Then n = (n - 1) * 15 Else n = 0
                    qvrInsert.Text = "update [podela2000] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela2000])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 2000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 1000 750x500"
        My.Application.DoEvents()

        'sada moze na 1000 ide iz 5000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y50001_ To y50002_ + 750 Step 750
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 750)) Then
                    For j = x50001_ To x50002_ Step 500
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 500)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y10001_ = i : x10001_ = j
                                y10002_ = i + 750 : x10002_ = j + 500
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y10002_ Then y10002_ = i + 750
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x10002_ Then x10002_ = j + 500
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela1000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y10001_ To y10002_ - 750 Step 750
            For j = x10001_ To x10002_ - 500 Step 500
                qvrInsert.Text = "insert into [podela1000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 750 & " " & j & "," & i + 750 & " " & j + 500 & "," & i & " " & j + 500 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 250 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 750) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 500
                    If n > 1 Then n = (n - 1) * 3 Else n = 0

                    qvrInsert.Text = "update [podela1000] set [NOMENKLATU]=" & Chr(34) & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela1000])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 1000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 500"
        My.Application.DoEvents()
        'sada moze na 500 koja ide iz 1000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y10001_ To y10002_ + 375 Step 375
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 375)) Then
                    For j = x10001_ To x10002_ Step 250
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 250)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y5001_ = i : x5001_ = j
                                y5002_ = i + 375 : x5002_ = j + 250
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y5002_ Then y5002_ = i + 375
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x5002_ Then x5002_ = j + 250
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y5001_ To y5002_ - 375 Step 375
            For j = x5001_ To x5002_ - 250 Step 250
                qvrInsert.Text = "insert into [podela500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 375 & " " & j & "," & i + 375 & " " & j + 250 & "," & i & " " & j + 250 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela500" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela1000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 375 & "," & j + 250 & "), COORDSYS(" & Chr(34) & "Podela1000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 375) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 250
                    If n > 1 Then n = (n - 1) * 2 Else n = 0
                    qvrInsert.Text = "update [podela500] set [NOMENKLATU]=" & Chr(34) & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela500])"
                    qvrInsert.RunEx(True)
                End If


            Next
        Next

        If razmera_ = 500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        doc.ComponentSet.Remove("cetiriTacke")
        doc.ComponentSet.Remove("insertQ")
        doc.Save()

        lbl_infoMain.Text = ""
        Cursor = Cursors.Default

        MsgBox("Kraj")

    End Sub

    Private Sub TSToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles TSToolStripMenuItem.Click
        podelaNaListoveGK_Zona6(50000)
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As System.EventArgs) Handles ToolStripMenuItem2.Click
        podelaNaListoveGK_Zona6(5000)
    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As System.EventArgs) Handles ToolStripMenuItem3.Click
        podelaNaListoveGK_Zona6(2500)
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As System.EventArgs) Handles ToolStripMenuItem4.Click
        podelaNaListoveGK_Zona6(2000)
    End Sub

    Private Sub ToolStripMenuItem5_Click(sender As Object, e As System.EventArgs) Handles ToolStripMenuItem5.Click
        podelaNaListoveGK_Zona6(1000)
    End Sub

    Private Sub ToolStripMenuItem6_Click(sender As Object, e As System.EventArgs) Handles ToolStripMenuItem6.Click
        podelaNaListoveGK_Zona6(500)
    End Sub

    Private Sub GPSZapisnikPrenumerisiBrojeveTacakaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles GPSZapisnikPrenumerisiBrojeveTacakaToolStripMenuItem.Click

        ' sada je znaimljivo sta ti treba! 
        'file u kome 
        'ulaz su ti:
        'drawing sa tackama koje su kao pntObelezavanje
        ''drawing sa tackama koje si importovao iz autocad-a i promenio tip polja u text string
        ''ono sto treba uraditi pre ovoga je da se ociste tacke kojih nema ovako ili tamo odnosno da se naprvi iner join i obrisan==0
        ''pitanje dokle da se ponavlja procedura!?

        Dim doc_ As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("listing")
        Dim qvr2_ As Manifold.Interop.Query = doc_.NewQuery("updateT")
        ' Dim qvr3_ As Manifold.Interop.Query = doc_.NewQuery("razlika")

        ''pretpostavka je da ces u jednom da brises a to je autocad!


        '' prvo idemo kntrou!
        If MessageBox.Show("Da li kontrolisem isti broj merenja u oba drawinga?", "Pitanje", MessageBoxButtons.YesNo) = DialogResult.OK Then
            Dim sql_ As String = String.Format("SELECT * FROM (SELECT count(*) FROM [{0}]), (SELECT  count(*) FROM [{0}] INNER join [{1}] ON [{0}].[idmerenja]=[{1}].[textstring]), (select count(*) FROM [{1}]) ", My.Settings.layerName_ProcembeniRazredi, My.Settings.layerName_parcele)
            qvr_.Text = sql_
            qvr_.RunEx(True)

            If ((qvr_.Table.RecordSet.Item(0).DataText(1) <> qvr_.Table.RecordSet.Item(0).DataText(2)) Or (qvr_.Table.RecordSet.Item(0).DataText(1) <> qvr_.Table.RecordSet.Item(0).DataText(3)) Or (qvr_.Table.RecordSet.Item(0).DataText(2) <> qvr_.Table.RecordSet.Item(0).DataText(3))) Then

                ''idemo napolje jer nisu jednake tako da treba proveriti!
                MsgBox("Nije identican broj rekorda koji se medusobno slaze, sa pojedinacnim brojem rekorda po tabelama - proverite pa pokrenite proceduru jos jednom")
                Exit Sub
            End If

        End If

        Dim dosaoDoKraja As Boolean = False
        Dim staoNegde As Integer = 1


        Do While Not (dosaoDoKraja)
            dosaoDoKraja = True
            qvr_.Text = "select [textstring] from [" & My.Settings.layerName_parcele & "] order by [textstring] asc"
            qvr_.RunEx(True)
            pb1.Maximum = qvr_.Table.RecordSet.Count - 2
            pb1.Minimum = 0
            For i = staoNegde To qvr_.Table.RecordSet.Count - 2
                pb1.Value = i
                ''sada da vidimo sta trazimo
                If (qvr_.Table.RecordSet.Item(i + 1).DataText(1) - qvr_.Table.RecordSet.Item(i).DataText(1)) > 1 Then
                    ''znaci postoji razlika ! aj da vidimo kolika je
                    Dim razlika_ = qvr_.Table.RecordSet.Item(i + 1).DataText(1) - qvr_.Table.RecordSet.Item(i).DataText(1)
                    Dim odakle_ = qvr_.Table.RecordSet.Item(i + 1).DataText(1)
                    ''update jednog crteza
                    qvr2_.Text = "update [" & My.Settings.layerName_parcele & "] set [textstring]=[textstring] - " & razlika_ - 1 & " where [textstring]>=" & odakle_
                    qvr2_.RunEx(True)
                    ''update drugog crteza
                    ''qvr2_.Text = "update [" & My.Settings.layerName_ProcembeniRazredi & "] set [idmerenja]=[idmerenja] - " & razlika_ - 1 & " where [obrisan]=0 and [idmerenja]>=" & odakle_
                    ''qvr2_.RunEx(True)
                    dosaoDoKraja = False
                    If (i - 2) < 0 Then staoNegde = 0 Else staoNegde = i - 2
                    System.Diagnostics.Debug.WriteLine("Dosao do sledeceg " & odakle_)


                    Exit For
                End If
            Next
        Loop

        dosaoDoKraja = False
        staoNegde = 1
        '' sada po ostom principu za drugi
        Do While Not (dosaoDoKraja)
            dosaoDoKraja = True
            qvr_.Text = "select [idmerenja] from [" & My.Settings.layerName_ProcembeniRazredi & "] order by [idmerenja] asc"
            qvr_.RunEx(True)
            pb1.Maximum = qvr_.Table.RecordSet.Count - 2
            pb1.Minimum = 0
            For i = staoNegde To qvr_.Table.RecordSet.Count - 2
                pb1.Value = i
                ''sada da vidimo sta trazimo
                If (qvr_.Table.RecordSet.Item(i + 1).DataText(1) - qvr_.Table.RecordSet.Item(i).DataText(1)) > 1 Then
                    ''znaci postoji razlika ! aj da vidimo kolika je
                    Dim razlika_ = qvr_.Table.RecordSet.Item(i + 1).DataText(1) - qvr_.Table.RecordSet.Item(i).DataText(1)
                    Dim odakle_ = qvr_.Table.RecordSet.Item(i + 1).DataText(1)
                    ''update jednog crteza
                    qvr2_.Text = "update [" & My.Settings.layerName_ProcembeniRazredi & "] set [idmerenja]=[idmerenja] - " & razlika_ - 1 & " where [idmerenja]>=" & odakle_
                    qvr2_.RunEx(True)
                    ''update drugog crteza
                    ''qvr2_.Text = "update [" & My.Settings.layerName_ProcembeniRazredi & "] set [idmerenja]=[idmerenja] - " & razlika_ - 1 & " where [obrisan]=0 and [idmerenja]>=" & odakle_
                    ''qvr2_.RunEx(True)
                    dosaoDoKraja = False
                    If (i - 2) < 0 Then staoNegde = 0 Else staoNegde = i - 2
                    System.Diagnostics.Debug.WriteLine("Dosao do sledeceg " & odakle_)
                    Exit For
                End If
            Next
        Loop



        doc_.ComponentSet.Remove("listing")
        doc_.ComponentSet.Remove("updateT")
        doc_.Save()


        MsgBox("Kraj")

    End Sub

    Public Sub podelaNaListoveGK_Zona6(razmera_ As Integer)

        'granica ti je data u layer-u centri moci jer dok radis postavi da je podela na listove u setting-u
        'radis podelu na listove za layer podela na listove i to za selektovano! a moramo da vidimo gde cemo to da smestimo :)

        Dim doc As Manifold.Interop.Document = ManifoldCtrl.get_Document

        Me.Cursor = Cursors.WaitCursor

        lbl_infoMain.Text = "Brisem prethodne podele ako postoje"
        My.Application.DoEvents()

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podelaTS")
            doc.ComponentSet.Remove("podelaTS")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela5000")
            doc.ComponentSet.Remove("podela5000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2500")
            doc.ComponentSet.Remove("podela2500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela2000")
            doc.ComponentSet.Remove("podela2000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela1000")
            doc.ComponentSet.Remove("podela1000")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Try
            Dim pp_ As Manifold.Interop.Drawing = doc.ComponentSet("podela500")
            doc.ComponentSet.Remove("podela500")
            pp_ = Nothing
        Catch ex As Exception

        End Try

        Dim yts1_, xts1_, yts2_, xts2_ As Integer
        Dim y50001_, x50001_, y50002_, x50002_ As Integer
        Dim y25001_, x25001_, y25002_, x25002_ As Integer
        Dim y10001_, x10001_, y10002_, x10002_ As Integer
        Dim y5001_, x5001_, y5002_, x5002_ As Integer

        Dim NOMENKLATU_ As String = "6 "

        yts1_ = 0 : xts1_ = 0 : yts2_ = 0 : xts2_ = 0
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("cetiriTacke")
        qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]  WHERE [Selection (I)]=true ) SPLIT by Coords(geom_) as pnt_"
        qvr_.RunEx(True)

        If qvr_.Table.RecordSet.Count = 0 Then
            'ovo znaci da nema nista selektovano - uzimas sve sto je u drawingu i nastavljas dalje
            qvr_.Text = "select centroidx(pnt_),CentroidY(pnt_) from (SELECT BoundingBox( UnionAll([ID])) as geom_ FROM  [" & My.Settings.layerName_podelaNaListove & "]) SPLIT by Coords(geom_) as pnt_"
            qvr_.RunEx(True)

        End If

        lbl_infoMain.Text = "Radim podelu za TS"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = 6320000 To 6680000 Step 22500
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 22500)) Then
                    For j = 4635000 To 5175000 Step 15000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 15000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                yts1_ = i : xts1_ = j
                                yts2_ = i + 22500 : xts2_ = j + 15000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > yts2_ Then yts2_ = i + 22500
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > xts2_ Then xts2_ = j + 15000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'ovo je za razmeru 50000 iz koje ide dalje podela
        Dim drwUTM_ As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_podelaNaListove)

        Dim drw_ As Manifold.Interop.Drawing = doc.NewDrawing("podelaTS", drwUTM_.CoordinateSystem, True)
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "NOMENKLATU"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText
        drw_.OwnedTable.ColumnSet.Add(col_)
        'ovde treba napraviti update to je najbolje
        Dim listaSlova = {"A2", "A1", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "L1", "L2"}
        'Dim listaSlova()


        Dim qvrInsert As Manifold.Interop.Query = doc.NewQuery("insertQ")
        ' NOMENKLATU_ = 

        For i = yts1_ To yts2_ - 22500 Step 22500
            For j = xts1_ To xts2_ - 15000 Step 15000

                Dim slovo_ = ""

                Try
                    slovo_ = "6 " & listaSlova(((i - 6320000) / 22500))
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try

                qvrInsert.Text = "insert into [podelaTS] ([NOMENKLATU],[Geom (I)]) values (" & Chr(34) & slovo_ & " " & (((j - 4635000) / 15000) + 1) & Chr(34) & ", AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 22500 & " " & j & "," & i + 22500 & " " & j + 15000 & "," & i & " " & j + 15000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"

                qvrInsert.RunEx(True)
                'ovde mora drugacije!
            Next
        Next

        If razmera_ = 50000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 5000"
        My.Application.DoEvents()

        'sada moze na 5000

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 22500 Step 2250
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 2250)) Then
                    For j = xts1_ To xts2_ Step 3000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 3000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y50001_ = i : x50001_ = j
                                y50002_ = i + 2250 : x50002_ = j + 3000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y50002_ Then y50002_ = i + 2250
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x50002_ Then x50002_ = j + 3000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela5000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        'ovde je veci problem za nomenklaturu jer moze da bude vise od jedne trig sekcije!!!
        For i = y50001_ To y50002_ - 2250 Step 2250
            For j = x50001_ To x50002_ - 3000 Step 3000

                qvrInsert.Text = "insert into [podela5000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 2250 & " " & j & "," & i + 2250 & " " & j + 3000 & "," & i & " " & j + 3000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1125 & "," & j + 1500 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 2250) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 3000
                    If n > 1 Then n = (n - 1) * 10 Else n = 0
                    qvrInsert.Text = "update [podela5000] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela5000])"
                    qvrInsert.RunEx(True)
                End If
            Next
        Next

        If razmera_ = 5000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        y25001_ = Nothing : y25002_ = Nothing : x25001_ = Nothing : x25002_ = Nothing
        'sada moze na 2500
        lbl_infoMain.Text = "Radim podelu za 2500"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 2250 Step 2250
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 2250)) Then
                    For j = xts1_ To xts2_ Step 1500
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1500)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y25001_ = i : x25001_ = j
                                y25002_ = i + 2250 : x25002_ = j + 1500
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 2250
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1500
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela2500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        'NOMENKLATU_ = NOMENKLATU_ & " " & ((y50002_ - y50001_) / 3000) * ((x50002_ - x50001_) / 2000)
        For i = y25001_ To y25002_ - 2250 Step 2250
            For j = x25001_ To x25002_ - 1500 Step 1500
                qvrInsert.Text = "insert into [podela2500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 2250 & " " & j & "," & i + 2250 & " " & j + 1500 & "," & i & " " & j + 1500 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 1125 & "," & j + 750 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 2250) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1500
                    If n >= 1 Then n = (n - 1) * 10 Else n = 0
                    qvrInsert.Text = "update [podela2500] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela2500])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 2500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        'sada moze na 2000
        lbl_infoMain.Text = "Radim podelu za 2000"
        My.Application.DoEvents()

        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = yts1_ To yts2_ + 1500 Step 1500
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 1500)) Then
                    For j = xts1_ To xts2_ Step 1000
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 1000)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y25001_ = i : x25001_ = j
                                y25002_ = i + 1500 : x25002_ = j + 1000
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y25002_ Then y25002_ = i + 1500
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x25002_ Then x25002_ = j + 1000
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next

        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela2000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y25001_ To y25002_ - 1500 Step 1500
            For j = x25001_ To x25002_ - 1000 Step 1000
                qvrInsert.Text = "insert into [podela2000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 1500 & " " & j & "," & i + 1500 & " " & j + 1000 & "," & i & " " & j + 1000 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [PodelaTS] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 750 & "," & j + 500 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 1500) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 1000
                    If n >= 1 Then n = (n - 1) * 15 Else n = 0
                    qvrInsert.Text = "update [podela2000] set [NOMENKLATU]=" & Chr(34) & qvrInsert.Table.RecordSet(0).DataText(3) & " - " & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela2000])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 2000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 1000 750x500"
        My.Application.DoEvents()

        'sada moze na 1000 ide iz 5000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y50001_ To y50002_ + 750 Step 750
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 750)) Then
                    For j = x50001_ To x50002_ Step 500
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 500)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y10001_ = i : x10001_ = j
                                y10002_ = i + 750 : x10002_ = j + 500
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y10002_ Then y10002_ = i + 750
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x10002_ Then x10002_ = j + 500
                                Exit For
                            End If

                        End If

                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela1000", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y10001_ To y10002_ - 750 Step 750
            For j = x10001_ To x10002_ - 500 Step 500
                qvrInsert.Text = "insert into [podela1000] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 750 & " " & j & "," & i + 750 & " " & j + 500 & "," & i & " " & j + 500 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela5000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 300 & "," & j + 250 & "), COORDSYS(" & Chr(34) & "PodelaTS" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 750) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 500
                    If n > 1 Then n = (n - 1) * 3 Else n = 0

                    qvrInsert.Text = "update [podela1000] set [NOMENKLATU]=" & Chr(34) & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela1000])"
                    qvrInsert.RunEx(True)
                End If

            Next
        Next

        If razmera_ = 1000 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        lbl_infoMain.Text = "Radim podelu za 500"
        My.Application.DoEvents()
        'sada moze na 500 koja ide iz 1000
        For k = 0 To qvr_.Table.RecordSet.Count - 1
            For i = y10001_ To y10002_ + 375 Step 375
                If (Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > i And Val(qvr_.Table.RecordSet.Item(k).DataText(1) < i + 375)) Then
                    For j = x10001_ To x10002_ Step 250
                        'sada mogu unutra po drugoj osi
                        If (Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > j And Val(qvr_.Table.RecordSet.Item(k).DataText(2) < j + 250)) Then
                            'sada si ga nasao!!!!!
                            If k = 0 Then
                                y5001_ = i : x5001_ = j
                                y5002_ = i + 375 : x5002_ = j + 250
                                Exit For
                            Else
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(1)) > y5002_ Then y5002_ = i + 375
                                If Val(qvr_.Table.RecordSet.Item(k).DataText(2)) > x5002_ Then x5002_ = j + 250
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        Next
        'sada ides u krug od do pa iscrtavas poligone za 5000
        drw_ = doc.NewDrawing("podela500", drwUTM_.CoordinateSystem, True)
        drw_.OwnedTable.ColumnSet.Add(col_)

        For i = y5001_ To y5002_ - 375 Step 375
            For j = x5001_ To x5002_ - 250 Step 250
                qvrInsert.Text = "insert into [podela500] ([Geom (I)]) values (AssignCoordSys(CGeom(CGeomWKB(" & Chr(34) & "POLYGON((" & i & " " & j & ", " & i + 375 & " " & j & "," & i + 375 & " " & j + 250 & "," & i & " " & j + 250 & "," & i & " " & j & "))" & Chr(34) & ")), COORDSYS(" & Chr(34) & "Podela500" & Chr(34) & " as COMPONENT)))"
                qvrInsert.RunEx(True)

                'sada za ovaj list treba da nades trigsekciju
                qvrInsert.Text = "SELECT min(centroidx(pnt_)), max(CentroidY(pnt_)), First([NOMENKLATU]) FROM [Podela1000] WHERE Contains([ID],AssignCoordSys( NewPoint(" & i + 375 & "," & j + 250 & "), COORDSYS(" & Chr(34) & "Podela1000" & Chr(34) & " as COMPONENT))) split by Coords([ID]) as pnt_ "
                'sada ti treba gorenji levi ugao trig sekcije pa do njegga douzimas
                qvrInsert.RunEx(True)

                If qvrInsert.Table.RecordSet.Count <> 1 Then
                    'nesto nije ok
                Else
                    'sada ga imas - ide ti x,y, NOMENKLATU

                    Dim m, n As Integer
                    m = (Math.Abs(i - Val(qvrInsert.Table.RecordSet(0).DataText(1))) / 375) + 1
                    n = Math.Abs(j - Val(qvrInsert.Table.RecordSet(0).DataText(2))) / 250
                    If n > 1 Then n = (n - 1) * 2 Else n = 0
                    qvrInsert.Text = "update [podela500] set [NOMENKLATU]=" & Chr(34) & (m + n) & Chr(34) & " where [id]=(select max([id]) from [podela500])"
                    qvrInsert.RunEx(True)
                End If


            Next
        Next

        If razmera_ = 500 Then
            doc.ComponentSet.Remove("cetiriTacke")
            doc.ComponentSet.Remove("insertQ")
            doc.Save()
            MsgBox("Kraj ")
            Exit Sub
        End If

        doc.ComponentSet.Remove("cetiriTacke")
        doc.ComponentSet.Remove("insertQ")
        doc.Save()

        lbl_infoMain.Text = ""
        Cursor = Cursors.Default

        MsgBox("Kraj")

    End Sub


    Private Sub StampajResenjaIzListeToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles StampajResenjaIzListeToolStripMenuItem.Click

        If My.Settings.resenja_wordFileTemplatePath = "" Then MsgBox("Morate definisati template word dokument") : Exit Sub

        Dim freefile_ As Integer = FreeFile()
        FileOpen(freefile_, Path.GetTempPath() & "\resenjaKojaNemajuStaroStanje.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        Dim freefileUlaz_ As Integer = FreeFile()

        opf_diag.FileName = ""
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then MsgBox("Morate izabrati ulazni file") : Exit Sub
        'ulaz file je jednostavan po jedan broj iskaza u redu
        'sada bi trebalo uneti folder pa njega proslediti dalje
        Dim folderPath_ As String = ""
        fbd_diag.ShowDialog()
        folderPath_ = fbd_diag.SelectedPath.ToString

        FileOpen(freefileUlaz_, opf_diag.FileName, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)


        Dim docApp_ As Microsoft.Office.Interop.Word.Application = New Microsoft.Office.Interop.Word.Application : docApp_.Visible = True
        Dim wDoc_ As Microsoft.Office.Interop.Word.Document

        Do While Not EOF(freefileUlaz_)
            Dim brIskaza As Integer = LineInput(freefileUlaz_)
            wDoc_ = docApp_.Documents.Open(My.Settings.resenja_wordFileTemplatePath)
            wDoc_.SaveAs2(folderPath_ & "\" & brIskaza & ".doc")
            stampajResenje(brIskaza, docApp_, wDoc_)
            wDoc_.Save()
            wDoc_.Close()
        Loop

        MsgBox("Kraj ")

    End Sub
End Class