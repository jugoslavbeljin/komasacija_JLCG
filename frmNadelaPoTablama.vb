Imports MySql.Data.MySqlClient
Imports Manifold.Interop
Imports System.IO

Public Class frmNadelaPoTablama

    Public doc_ As Manifold.Interop.Document

    Private Sub frmNadelaPoTablama_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
            conn_.Open()
            'Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("select idTable,oznakaTable from kom_table where obrisan=0", conn_)
            Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("select idTable,oznakaTable from kom_kfmns where obrisan=0", conn_)
            Dim myAdapter As New MySqlDataAdapter
            myAdapter.SelectCommand = connComm

            Dim dsTable As New DataTable
            myAdapter.Fill(dsTable)

            ddl_ttpSpisakTabli.DataSource = dsTable
            ddl_ttpSpisakTabli.DisplayMember = "oznakaTable"
            ddl_ttpSpisakTabli.ValueMember = "idTable"

            dsTable = Nothing
            myAdapter = Nothing
            connComm = Nothing
            conn_.Close()
            conn_ = Nothing

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ddl_ttpSpisakTabli_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddl_ttpSpisakTabli.SelectedIndexChanged
        Try
            Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
            conn_.Open()
            Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("select idiskazzemljista,rednibrojnadele,nadeljenoVrednost,nadeljenopovrsina from kom_tablenadela where obrisan=0 and idTable=" & Replace(ddl_ttpSpisakTabli.SelectedValue, "T", "") & " order by rednibrojnadele ASC", conn_)
            'Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("select idTable,idiskazzemljista,rednibrojnadele,nadeljenoVrednost,nadeljenopovrsina from kom_tablenadela where obrisan=0 and idTable=" & Replace(ddl_ttpSpisakTabli.SelectedValue, "T", "") & " order by rednibrojnadele ASC", conn_)
            Dim myAdapter As New MySql.Data.MySqlClient.MySqlDataAdapter(connComm.CommandText, conn_)
            Dim ds_ As New DataTable
            myAdapter.Fill(ds_)

            gridTableNadela.DataSource = ds_
            conn_.Close()
            myAdapter = Nothing
            conn_ = Nothing
            connComm = Nothing
            ds_ = Nothing
        Catch ex As Exception

        End Try

    End Sub
    Public Sub priprema_formiranjeRaspodele(ByVal doc As Manifold.Interop.Document)

        ', ByVal topParcele As Manifold.Interop.Topology
        Dim drw_Lamele As Manifold.Interop.Drawing = doc.ComponentSet.Item(My.Settings.layerName_table)
        Dim drw_parcele As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_parcele)

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drw_parcele)
        topParcele.Build()


        Dim tbl_ As Manifold.Interop.Table
        Dim col_ As Manifold.Interop.Column
        'napravi se kopija dve tabele pre ovoga: tab_proc_raz i OglednoPolje_Table zbog intersekta
        Dim drw1 As Manifold.Interop.Drawing = doc.NewDrawing("tab_proc_raz2", drw_Lamele.CoordinateSystem, True)
        Dim drw_tab_proc_raz As Manifold.Interop.Drawing
        Try
            drw_tab_proc_raz = doc.ComponentSet("tab_proc_raz")
        Catch ex As Exception
            topologijaKreirajOsnovno(doc)
            doc.Save()
            drw_tab_proc_raz = doc.ComponentSet("tab_proc_raz")
        End Try

        drw_tab_proc_raz.Copy()
        drw1.Paste()
        drw1 = doc.NewDrawing((My.Settings.layerName_table & "2"), drw_Lamele.CoordinateSystem, True)
        drw_Lamele.Copy()
        drw1.Paste()

        Dim drwLinije As Manifold.Interop.Drawing = doc.NewDrawing("LinijeSegmenti", drw_tab_proc_raz.CoordinateSystem, True)
        tbl_ = drwLinije.OwnedTable
        col_ = doc.Application.NewColumnSet.NewColumn
        col_.Name = "dID"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)

        Dim qvr As Manifold.Interop.Query = doc.NewQuery("upisiLinije")
        qvr.Text = "INSERT INTO [LinijeSegmenti] ([Geom (I)],[dID]) (SELECT [Line],[dID] from (SELECT distinct ClipIntersect([L].[LineObj], Geom_) AS [Line], [L].[dID] FROM ( SELECT UnionAll([Geom (I)]) as Geom_,[idTable] as idLam_ from [tab_proc_raz] GROUP by [idTable]) INNER JOIN (SELECT distinct AssignCoordSys(newline(AssignCoordSys(NewPoint(XS,YS),CoordSys(" & Chr(34) & "tab_proc_raz" & Chr(34) & " as COMPONENT)), [gg]) ,CoordSys(" & Chr(34) & "tab_proc_raz" & Chr(34) & " as COMPONENT)) as LineObj,[dID] FROM (SELECT  IIf(Touches([tab_proc_raz].[ID],AssignCoordSys(NewPoint((XS+cos(ugao)*50),(YS+sin(ugao)*50)),CoordSys(" & Chr(34) & "tab_proc_raz" & Chr(34) & " as COMPONENT))),AssignCoordSys(NewPoint((XS+cos(ugao)*sirina_final),(YS+sin(ugao)*sirina_final)),CoordSys(" & Chr(34) & "tab_proc_raz" & Chr(34) & " as COMPONENT)),AssignCoordSys(NewPoint((XS-cos(ugao)*sirina_final),(YS-sin(ugao)*sirina_final)),CoordSys(" & Chr(34) & "tab_proc_raz" & Chr(34) & " as COMPONENT))) as [gg], YS, XS, dID, ugao, sirina_final FROM (SELECT distinct YS, XS, dID, ugao, sirina_final FROM ( SELECT distinct YS, XS, ugao, dID FROM (SELECT XS,YS, dID, pgID, rastojanje,X1,Y1,X2,Y2,ugao FROM ((SELECT CentroidX(pnt_) as XS, CentroidY(pnt_) as YS, ggID FROM (SELECT distinct [pnt_],[idTable] as ggID from [tab_proc_raz] Split BY Coords([ID]) AS pnt_)) LEFT JOIN (SELECT rastojanje1 as rastojanje, dID, pgID, X1,Y1,X2, Y2, Atn2((Y2-Y1),(X2-X1)) as ugao from (SELECT min(distance([Segment],[" & My.Settings.layerName_nadelaSmer & "].[ID])) as rastojanje2 FROM [tab_proc_raz],[" & My.Settings.layerName_nadelaSmer & "] WHERE [tab_proc_raz].[idTable]=[" & My.Settings.layerName_nadelaSmer & "].[idTable] SPLIT BY Branches(IntersectLine(Boundary([tab_proc_raz].[Geom (I)]), Boundary([tab_proc_raz].[Geom (I)]))) AS [Segment] group by [tab_proc_raz].[idTable]),(SELECT (distance([Segment],[" & My.Settings.layerName_nadelaSmer & "].[ID])) as rastojanje1,[tab_proc_raz].[idTable] as dID, [tab_proc_raz].[ID] as pgID, CentroidX(Coord([Segment],0)) as X1,CentroidY(Coord([Segment],0)) as Y1, CentroidX(Coord([Segment],1)) as X2,CentroidY(Coord([Segment],1)) as Y2 FROM [tab_proc_raz],[" & My.Settings.layerName_nadelaSmer & "] WHERE [tab_proc_raz].[idTable]=[" & My.Settings.layerName_nadelaSmer & "].[idTable] SPLIT BY Branches(IntersectLine(Boundary([tab_proc_raz].[Geom (I)]), Boundary([tab_proc_raz].[Geom (I)]))) AS [Segment]) where rastojanje1=rastojanje2) on ggID=dID )))	LEFT JOIN (SELECT dpID, CASE WHEN RectWidth(EnclosingRectangle(geom_)," & Chr(34) & "m" & Chr(34) & ") > RectHeight(EnclosingRectangle(geom_)," & Chr(34) & "m" & Chr(34) & ") THEN  RectWidth(EnclosingRectangle(geom_)," & Chr(34) & "m" & Chr(34) & ") ELSE RectHeight(EnclosingRectangle(geom_)," & Chr(34) & "m" & Chr(34) & ") END AS sirina_final FROM (SELECT UnionAll([Geom (I)]) as geom_,[idTable] dpID from [tab_proc_raz] GROUP by [idTable])) ON dID=dpID ), [tab_proc_raz] WHERE [tab_proc_raz].[idTable]=[dID] )) as [L] ON idLam_=[L].[dID]) where [Line] is NOT NULL)"
        'linija iznad moze da bude problem zbog toga sto ne hendluje status table!
        qvr.RunEx(True)

        'sada ti trebaju dve teme jedna je ulaz_1 a druga ulaz_2

        'kreiras dva drawinga
        Dim ulaz_1 As Manifold.Interop.Drawing = doc.NewDrawing("ulaz_1", drw_tab_proc_raz.CoordinateSystem, True)
        Dim ulaz_2 As Manifold.Interop.Drawing = doc.NewDrawing("ulaz_2", drw_tab_proc_raz.CoordinateSystem, True)

        Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        analizer_.Split(ulaz_1, ulaz_1, drw_tab_proc_raz.ObjectSet, drwLinije.ObjectSet)
        analizer_.Split(ulaz_2, ulaz_2, drw_Lamele.ObjectSet, drwLinije.ObjectSet)

        doc.ComponentSet.Remove("upisiLinije")


        Dim drwRaspodela As Manifold.Interop.Drawing = doc.NewDrawing("Raspodela", drw_tab_proc_raz.CoordinateSystem, True)
        tbl_ = drwRaspodela.OwnedTable
        col_ = doc.Application.NewColumnSet.NewColumn
        col_.Name = "idTable"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "vrednost"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        tbl_.ColumnSet.Add(col_)
        col_.Name = "ID2"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "maxVlasnik"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32

        tbl_.ColumnSet.Add(col_)
        For i = 0 To tbl_.ColumnSet.Count - 1
            If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
            End If
        Next

        Dim qvrRaspodela As Manifold.Interop.Query = doc.NewQuery("upisiraspodela")
        qvrRaspodela.Text = "insert into [raspodela] ([Geom (I)],[idTable],[vrednost]) (SELECT [ulaz_2].[Geom (I)],[ulaz_2].[idTable],vrednost FROM [ulaz_2] LEFT JOIN (SELECT sum([Ulaz_1].[Faktor]*[Ulaz_1].[Area (I)]) as Vrednost,[ulaz_2].[ID] as gID FROM [Ulaz_1],[ulaz_2] WHERE Contains([Ulaz_2].[ID],[ulaz_1].[ID]) GROUP BY [ulaz_2].[ID] ) as [p] on [ulaz_2].[ID]=[p].gID )"
        qvrRaspodela.RunEx(True)

        'sada preslikas id u id2
        qvrRaspodela.Text = "update [Raspodela] set [ID2]=[ID]"
        qvrRaspodela.RunEx(True)

        'sada ti opet treba neki presek - raspodela i parcele - raspodela 2 odnosno max min

        Dim topRaspodela As Manifold.Interop.Topology = doc.Application.NewTopology
        topRaspodela.Bind(drwRaspodela)
        topRaspodela.Build()

        topRaspodela.DoIntersect(topParcele, "maxVlasnikParcelaSegment")

        'sada ostaje da napravis update u odnosu na ovaj drawing
        qvrRaspodela.Text = "update [raspodela] SET [maxVlasnik]=(SELECT idMax FROM (SELECT gID,[maxVlasnikParcelaSegment].[idVlasnika] as idMax from [maxVlasnikParcelaSegment],(SELECT [ID2] as gID, max([Area (I)]) as max_ from [maxVlasnikParcelaSegment] GROUP by [ID2]) WHERE gID=[maxVlasnikParcelaSegment].[ID2] AND max_=[maxVlasnikParcelaSegment].[Area (I)]) WHERE gID=[ID2])"
        qvrRaspodela.RunEx(True)


        doc.ComponentSet.Remove("upisiraspodela")
        'ovde ima drw-a koje treba obrisati ali sta i u kom momentu
        If MsgBox("Brisem drw koji su nastali u procesu racunanja?", MsgBoxStyle.OkCancel) = 1 Then
            doc.ComponentSet.Remove("LinijeSegmenti")
            doc.ComponentSet.Remove("MaxVlasnikParcelaSegment")
            doc.ComponentSet.Remove("Tab_proc_raz")
            doc.ComponentSet.Item(doc.ComponentSet.ItemByName("Tab_proc_raz2")).Name = "Tab_proc_raz"
            doc.ComponentSet.Remove("Table")
            doc.ComponentSet.Item(doc.ComponentSet.ItemByName("Table2")).Name = "Table"
            doc.ComponentSet.Remove("Ulaz_1")
            doc.ComponentSet.Remove("Ulaz_2")
        End If

        doc.Save()
    End Sub
    Private Sub UpisDeobe(ByVal doc As Manifold.Interop.Document, ByVal tekst_ As String, ByVal id_ As Integer, ByVal napredNazad_ As Integer)
        Dim qvr3_ As Manifold.Interop.Query = doc.NewQuery("upisi")
        If napredNazad_ = 1 Then
            qvr3_.Text = "update [Raspodela] set [DeobaNapred]= ([DeobaNapred] + " & Chr(34) & tekst_ & Chr(34) & ") where ID= " & id_
        Else
            qvr3_.Text = "update [Raspodela] set [DeobaNazad]= ([DeobaNazad] + " & Chr(34) & tekst_ & Chr(34) & ") where ID= " & id_
        End If
        qvr3_.RunEx(True)
        qvr3_ = Nothing
        doc.ComponentSet.Remove("upisi")
        doc.Save()
    End Sub
    Private Function findOwnersInLandTable(ByVal doc As Manifold.Interop.Document, ByVal ulaznaMatrica(,) As Double, ByVal red_ As Integer) As List(Of Integer)
        'vraca vektor index-a vlasnika za tablu
        Dim i As Integer
        Dim temp(-1) As Integer
        Dim brojac_ As Integer = 0
        Dim qvrVlasnici As Manifold.Interop.Query = doc.NewQuery("spisakVlasnika")
        qvrVlasnici.Text = "select distinct [" & My.Settings.parcele_fieldName_Vlasnik & "] from [" & My.Settings.layerName_parcele & "] order by [" & My.Settings.parcele_fieldName_Vlasnik & "]"
        qvrVlasnici.RunEx(True)

        'kontrola da li je jedanka matrica i broj vlasnika
        If (qvrVlasnici.Table.RecordSet.Count <> ulaznaMatrica.GetLength(1)) Then
            MsgBox("Nesto nije u redu - broj vlasnika nije saglasan")
            'exit 
        End If

        'prvo ih prepises sve koji su razliciti od nule
        For i = 0 To ulaznaMatrica.GetLength(1) - 1
            If ulaznaMatrica(red_, i) <> 0 Then
                ReDim Preserve temp(brojac_)
                'temp(brojac_) = i + 1
                temp(brojac_) = qvrVlasnici.Table.RecordSet.Item(i).DataText(1)
                'ovde mora i upit u file ne moze samo ovako!
                brojac_ += 1
            End If
        Next
        'ovde nemas duplikata znaci gotovo
        doc.ComponentSet.Remove("spisakVlasnika")
        Dim pera_ As List(Of Integer) = temp.ToList
        Return pera_
    End Function

    Private Sub UpisiNovogVlasnika(ByVal doc As Manifold.Interop.Document, ByVal noviVlasnik_ As Integer, ByVal id_ As Integer, ByVal napredNazad_ As Integer)
        Dim qvr3_ As Manifold.Interop.Query = doc.NewQuery("upisi")
        If napredNazad_ = 1 Then
            qvr3_.Text = "update [Raspodela] set [NoviVlasnikNapred]=" & noviVlasnik_ & " where ID= " & id_
        Else
            qvr3_.Text = "update [Raspodela] set [NoviVlasnikNazad]=" & noviVlasnik_ & " where ID= " & id_
        End If
        qvr3_.RunEx(True)
        qvr3_ = Nothing
        doc.ComponentSet.Remove("upisi")
        doc.Save()
    End Sub
    Private Function daliPostojiElementUMatrici(ByVal mat_() As Integer, ByVal elem_ As Integer) As Boolean
        daliPostojiElementUMatrici = False

        For i = 0 To mat_.Length - 1
            If mat_(i) = elem_ Then
                daliPostojiElementUMatrici = True
                Exit Function
            End If
        Next
        Return daliPostojiElementUMatrici
    End Function
    Private Sub nadela_izTable_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs)
        txt_Help.Text = "Ova opcija daje mogucnost nadele iz baze podataka. Neophodne ulazni Drawing-zi su:" & vbCrLf & "1. Drawing sa tablama, " & vbCrLf & "2. Drawing sa tackama koje definisu pravac nadele, " & vbCrLf & "3. Drawing sa tackama koje se koriste za obelezavanje na terenu." & _
            "proces rada je: " & vbCrLf & "Prvo izaberite tablu koju zelite da nadelite, a pre toga ste trebali da ucitate crtez, odnosno map file." & vbCrLf & "U slucaju da je vec izvrsena nadela za ovu tablu bicete upitani da li zelite da brisete stare podatke." & vbCrLf & "Nakon zavrsene procedure (i recimo da ste izabrali tablu sa ID-om 19) bice kreirana dva drawinga: TablaPR_15 i TablaPR_15_dissolve, i bice modifikovani Drawing-zi: DKP_Nadela - gde ce biti upisane novo formirane parcele, i PntTableObelezavanje - gde ce biti upisane novo formirane tacke koje definisu parcelu sa jedinstvenim brojem." & _
            vbCrLf & "Svi kreirani podaci se cuvaju u Map file koji je ucitan na pocetku."
    End Sub

    Private Sub btn_dataVrednost_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs)
        txt_Help.Text = "Ova opcija daje mogucnost nadele za zadatu konstantnu vrednost i koristi se prilikom kreiranja skica tabli za rad - odnosno nadelu.  Neophodne ulazni Drawing-zi su:" & vbCrLf & "1. Drawing sa tablama, " & vbCrLf & "2. Drawing sa tackama koje definisu pravac nadele." & vbCrLf & "Funkcija formira linije - lamele na zadatu vrednost koju upisujete u prozor koji se pojavljuje nakon starta ove funkcije. Funcija formira lamele samo za table ciji je statusTable podesen na 1 - osnovna ideja je da otvorite map file u manifold-u selektujete sve table koje zelite da izvrsite 'virtualnu nadelu' i upisete 1 u table view za ovaj drawing." & _
        vbCrLf & ("Funckija za svaki tablu formira dva nova drawing-a i to: Tabla (broj table) i Tabla(broj table)")

        'mozda sad da napuni table? sa cek boxom? a za to mi treba lista sto je opet zajebancija!?
        Try

            If gridTableNadela.RowCount = 0 Then
                Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
                conn_.Open()
                Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("select idTable,oznakatable,0 as status from kom_table where obrisan=0", conn_)
                Dim myAdapter As New MySql.Data.MySqlClient.MySqlDataAdapter(connComm.CommandText, conn_)
                Dim ds_ As New DataTable
                myAdapter.Fill(ds_)

                gridTableNadela.DataSource = ds_
                conn_.Close()
                myAdapter = Nothing
                conn_ = Nothing
                connComm = Nothing
                ds_ = Nothing
            End If
        Catch ex As Exception

        End Try



    End Sub

    Private Sub btn_izFile_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs)
        txt_Help.Text = "Ova opcija daje mogucnost nadele iz datoteke-file(a). Neophodne ulazni Drawing-zi su:" & vbCrLf & "1. Drawing sa tablama, " & vbCrLf & "2. Drawing sa tackama koje definisu pravac nadele, " & vbCrLf & "3. Drawing sa tackama koje se koriste za obelezavanje na terenu." & _
           vbCrLf & "Proces rada je: " & vbCrLf & "Izaberite file u kome se nalazi nadela koju zelite da izvrsite - format file-a je idiskaza, vrednost iskaza." & vbCrLf & " zatim upisite ime izlaznog map file-a / kreira se novi file" & vbCrLf & "unesite broj table."

    End Sub

    Private Sub btnPrintTable_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs)
        txt_Help.Text = "Ova opcija daje mogucnost stampanja nadeljene table." & vbCrLf & "Ulazni podaci: " & vbCrLf & "1. Drawing za nadeljenu tablu sa prefiksom PR na primer TablaPR 17, i Drawing sa sumiranim procembenim razredima po ovoj tabli na primer TablaPR 17 dissolve" & vbCrLf & "Proces rada:" & _
        vbCrLf & "Izaberite ima novog map file u kome ce se generisati prikaz za stampu"
    End Sub

    Private Sub btn_stampaObelezavanje_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs)
        txt_Help.Text = "Ova opcija daje mogucnost stampanja nadeljene table ali za obelezavanje." & vbCrLf & "Ulazni podaci: " & vbCrLf & "1. Drawing za nadeljenu tablu sa prefiksom PR na primer TablaPR 17, i Drawing sa sumiranim procembenim razredima po ovoj tabli na primer TablaPR 17 dissolve" & vbCrLf & "Proces rada:" & _
       vbCrLf & "Izaberite ima novog map file u kome ce se generisati prikaz za stampu"
    End Sub
    Public Sub stampajNadelu(ByVal brojTable As Integer, ByVal prikaziShowDialog As Boolean)

        Dim putanjaDoIzlaza_ As String = ""

        If prikaziShowDialog = True Then
            Try
                sf_diag.FileName = "nadela_tabla_" & brojTable
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
            putanjaDoIzlaza_ = sf_diag.FileName
        Else

            putanjaDoIzlaza_ = "c:\tablaNadela_" & brojTable & ".map"

        End If

        Dim manApp As Manifold.Interop.Application = New Manifold.Interop.Application : Dim newDoc As Manifold.Interop.Document = manApp.NewDocument("", False)

        Try
            newDoc.SaveAs(putanjaDoIzlaza_)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju")
            Exit Sub
        End Try


        'drwNewTable - bez disolva
        'drwnewDisolve sa disolve 
        'mislim da bi i ovo trebalo izbaciti! drwnewtable jer ti netreba posle!

        'sada prvo moras da importujes drawing koji ti treba a to je disolve!
        Dim docOld As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        '& ddl_ttpSpisakTabli.SelectedValue &
        'TablaPR 21 dissolve

        'ovde treba promeniti da ide iz DKP_Nadela a ne iz disolve

        Dim tri_ As String = "TablaPR_" & CStr(brojTable) & "_dissolve"
        Dim drwOriginal As Manifold.Interop.Drawing
        'Try
        '    drwOriginal = docOld.ComponentSet(tri_)
        'Catch ex As Exception
        '    MsgBox("Nemate tablu pod nazivom " & tri_)
        '    Exit Sub
        'End Try

        Dim qvrNest As Manifold.Interop.Query = docOld.NewQuery("sfdfd")
        qvrNest.Text = "select * from [" & My.Settings.layerName_ParceleNadela & "] where [idTable]=" & CStr(brojTable)
        qvrNest.RunEx(True)

        If qvrNest.Table.RecordSet.Count > 0 Then
            drwOriginal = docOld.ComponentSet(My.Settings.layerName_ParceleNadela)
            qvrNest.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [Selection (I)]=true where [idTable]=" & CStr(brojTable)
            qvrNest.RunEx(True)
        Else
            MsgBox("Nemate tablu u DKP_Nadela. Proveriti")
            Exit Sub
        End If


        Dim analizerF_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
        Dim drwnewDisolve As Manifold.Interop.Drawing = newDoc.NewDrawing(tri_, drwOriginal.CoordinateSystem, True)

        'sada copy - paste da ga preneses!
        drwOriginal.Copy(True) : drwnewDisolve.Paste(True) : drwOriginal.SelectNone() : drwnewDisolve.SelectNone()

        Dim drwTackeOld As Manifold.Interop.Drawing = docOld.ComponentSet("Tacke") : Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing("Tacke", drwTackeOld.CoordinateSystem, True)

        drwTackeOld.Copy(False) : drwTackeNew.Paste(True) : drwTackeOld.SelectNone() : drwTackeNew.SelectNone()

        'sada mozes dalje nemas vise problema

        Dim drwLinije As Manifold.Interop.Drawing = newDoc.NewDrawing("tablePR " & brojTable & " linije", drwnewDisolve.CoordinateSystem, True)
        'sada treba na osnovu ovoga da uradi nesto da kreira na primer linije koji opisuju zbog rastojanja - odnosno 

        'da dobijes linije
        analizerF_.Boundaries(drwnewDisolve, drwnewDisolve, drwnewDisolve.ObjectSet) : analizerF_.Explode(drwnewDisolve, drwnewDisolve.ObjectSet)
        analizerF_.RemoveDuplicates(drwnewDisolve, drwnewDisolve.ObjectSet) : analizerF_ = Nothing

        newDoc.Save()

        Dim qvrSelect As Manifold.Interop.Query = newDoc.NewQuery("selekcijaLinija")
        qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Selection (i)]=true where isLine([ID])=true"
        qvrSelect.RunEx(True) : drwnewDisolve.Cut(True) : drwLinije.Paste()

        drwnewDisolve.SelectNone() : drwLinije.SelectNone()

        Dim prazno_ As Manifold.Interop.Color = newDoc.Application.NewColor("proba", 0, 0, 1) : drwnewDisolve.AreaBackground.Set(prazno_)

        Dim tbl_ As Manifold.Interop.Table : Dim col_ As Manifold.Interop.Column

        tbl_ = drwLinije.OwnedTable : col_ = newDoc.Application.NewColumnSet.NewColumn
        'col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat32 : tbl_.ColumnSet.Add(col_)

        tbl_ = drwnewDisolve.OwnedTable
        col_.Name = "Opis1" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText : tbl_.ColumnSet.Add(col_)
        col_.Name = "Opis2" : tbl_.ColumnSet.Add(col_)

        col_ = Nothing : tbl_ = Nothing

        qvrSelect.Text = "SELECT CentroidX([Geom (I)]),CentroidY([Geom (I)]),round([Length (I)],2),[Bearing (I)] FROM [" & "tablePR " & brojTable & " linije" & "]" : qvrSelect.RunEx(True)

        Dim drwLabel As Manifold.Interop.Labels = newDoc.NewLabels(drwLinije.Name & "_label", drwLinije.CoordinateSystem, True)
        drwLabel.LabelAlignX = LabelAlignX.LabelAlignXCenter : drwLabel.LabelAlignY = LabelAlignY.LabelAlignYCenter
        drwLabel.OptimizeLabelAlignX = False : drwLabel.OptimizeLabelAlignY = False : drwLabel.ResolveOverlaps = False : drwLabel.PerLabelFormat = True

        Dim lblSets_ As Manifold.Interop.LabelSet = drwLabel.LabelSet

        For i = 0 To qvrSelect.Table.RecordSet.Count - 1
            'e sada da vidimo kreiras tacku i onda ide dalje
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvrSelect.Table.RecordSet.Item(i).DataText(1), qvrSelect.Table.RecordSet.Item(i).DataText(2))
            lblSets_.Add(qvrSelect.Table.RecordSet.Item(i).DataText(3), pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets_.LastAdded
            labb_.Rotation = qvrSelect.Table.RecordSet.Item(i).DataText(4) - 90
            labb_.Size = 5
            pnt_ = Nothing
        Next

        Dim map_ As Manifold.Interop.Map = newDoc.NewMap("TablaPR_" & brojTable & " map", drwnewDisolve, drwnewDisolve.CoordinateSystem, True)
        Dim pLayer As Manifold.Interop.Layer = newDoc.NewLayer(drwLabel)
        map_.LayerSet.Add(pLayer)

        'sad ide zanimljiviji deo ! znaci treba za ovu tabelu da selektujes sve ono sto ti treba a to je:
        'idNadele, vrednost, povrsina, ime i prezime

        Dim qvrZaNadeluInfo As Manifold.Interop.Query = newDoc.NewQuery("nadelaInfo")
        qvrZaNadeluInfo.Text = "select [idVlasnika],[ID],[redniBrNadele] from [tablaPR_" & brojTable & "_dissolve]"
        qvrZaNadeluInfo.RunEx(True)
        ' conn_.Open()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim myCommand As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        Dim myReader As MySqlDataReader

        For i = 0 To qvrZaNadeluInfo.Table.RecordSet.Count - 1
            Dim stsql_ As String = "select rednibrojnadele, nadeljenoPovrsina,nadeljenoVrednost from kom_tableNadela where obrisan=0 and idTable=" & brojTable & " and idIskazZemljista=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1) & " and rednibrojnadele=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(3)
            myCommand.CommandText = stsql_
            'conn_.Open()
            'sada ti treba reader da bi ovo sastavio kao text 
            Try
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            myReader.Read()
            If myReader.HasRows Then
                'sada imas sve sto ti treba - sto se teksta tice
                Dim text_ As String = "L=" & myReader.GetValue(0) & " V=" & myReader.GetValue(2) & " P=" & Math.Round(Val(myReader.GetValue(1)), 0) & " I=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1)
                qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis1]=" & Chr(34) & text_ & Chr(34) & " where [ID]=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(2)
                qvrSelect.RunEx(True)
                myReader.Close()
                myReader = Nothing
            Else
                'imas problem
                myReader.Close()
                myReader = Nothing
            End If
            'sada mozes na drugi deo a to su vlasnici
        Next

        'sada vlasnikk 
        For i = 0 To qvrZaNadeluInfo.Table.RecordSet.Count - 1
            Dim stsql_ As String = "SELECT group_concat( distinct concat(PREZIME, if(isnull(IMEOCA),'',concat(' (',IMEOCA,') ')),IME,' ',kom_vezaparcelavlasnik.Udeo) SEPARATOR ';') FROM kom_vlasnik,kom_vezaparcelavlasnik WHERE kom_vezaparcelavlasnik.idVlasnika=kom_vlasnik.idVlasnika and kom_vezaparcelavlasnik.obrisan=0 and idiskazzemljista=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1)
            myCommand.CommandText = stsql_

            Try
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            If myReader.HasRows Then
                myReader.Read()
                'sada imas sve sto ti treba - sto se teksta tice
                'Dim sta_ As String = myReader.GetValue(0)
                'sta_ = Replace(sta_, ",", vbCrLf)
                Try
                    qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis2]=" & Chr(34) & myReader.GetValue(0) & Chr(34) & " where [ID]=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(2)
                    qvrSelect.RunEx(True)
                    qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis2]=Replace([Opis2]," & Chr(34) & ";" & Chr(34) & ",chr(10) & chr(13))"
                    qvrSelect.RunEx(True)
                Catch ex As Exception
                    MsgBox("Proverite da u nazivu nemate znake navoda - u bazi, jer onda generisem gresku kao sada.")
                End Try
                

                myReader.Close()
                myReader = Nothing
            Else
                'imas problem
                myReader.Close()
                myReader = Nothing
            End If
        Next

        'sada od ova dva pravis novi label koji dodajes jos to da vidimo kako i gotovo!
        Dim labelLinije_ As Manifold.Interop.Labels
        labelLinije_ = newDoc.NewLabels("Opisi", drwnewDisolve, True, True)
        labelLinije_.Text = "[Opis2]" & vbCrLf & "[Opis1]"

        labelLinije_.LeftToRight = False
        labelLinije_.MultipleLabelsPerBranch = False
        labelLinije_.LineOffset = 3
        labelLinije_.OptimizeLabelAlignX = False
        labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False
        labelLinije_.Synchronized = False
        labelLinije_.Synchronized = True
        labelLinije_.PerLabelFormat = True

        Dim lbl_ As Manifold.Interop.Labels
        Dim labelSet_ As Manifold.Interop.LabelSet
        lbl_ = newDoc.ComponentSet("Opisi")
        labelSet_ = lbl_.LabelSet

        qvrSelect.Text = "SELECT CentroidX([Geom (I)]) as X,CentroidY([Geom (I)]) as Y FROM [" & My.Settings.layerName_nadelaSmer & "] WHERE [idtable]=" & brojTable
        qvrSelect.RunEx(True)

        Dim P_

        Try
            P_ = NiAnaB(qvrSelect.Table.RecordSet.Item(0).DataText(1), qvrSelect.Table.RecordSet.Item(0).DataText(2), qvrSelect.Table.RecordSet.Item(1).DataText(1), qvrSelect.Table.RecordSet.Item(1).DataText(2))
        Catch ex As Exception
            MsgBox("Problem sa tackama moje definisu pravac nadele!")
            'treba videti sta je napravio i sta treba obrisati
            Exit Sub
        End Try
        For Each lab_ In labelSet_
            lab_.rotation = P_ - 90
            lab_.size = 7
        Next
        pLayer = newDoc.NewLayer(labelLinije_)
        map_.LayerSet.Add(pLayer)

        Dim layout_ As Manifold.Interop.Layout = newDoc.NewLayout("Tabla" & brojTable & " Stampa", map_)
        Dim entitySet_ As Manifold.Interop.LayoutEntrySet = layout_.EntrySet
        'fali broj table - gde to ubaciti?
        entitySet_.Add(LayoutType.LayoutTypeText)
        entitySet_.Item(entitySet_.Count - 1).Text = "TABLA " & ddl_ttpSpisakTabli.SelectedText
        entitySet_.Item(entitySet_.Count - 1).TextAlignX = LabelAlignX.LabelAlignXLeft
        entitySet_.Item(entitySet_.Count - 1).TextAlignY = LabelAlignY.LabelAlignYTop

        'pitanje kako postaviti velicinu fonta!!!!!!!!!!!! veliko pitanje 

        qvrZaNadeluInfo = Nothing
        qvrSelect = Nothing
        newDoc.ComponentSet.Remove("nadelaInfo")
        newDoc.ComponentSet.Remove("selekcijaLinija")
        'Dim qvrNest As Manifold.Interop.Query = docOld.NewQuery("sfdfd")
        docOld.ComponentSet.Remove("sfdfd")
        qvrNest = Nothing
        labelLinije_ = Nothing
        labelSet_ = Nothing
        lbl_ = Nothing
        pLayer = Nothing
        'map_.Open()
        map_ = Nothing
        newDoc.Save()
        conn_ = Nothing
        drwnewDisolve = Nothing
        myCommand = Nothing
        newDoc = Nothing
        docOld = Nothing
        manApp = Nothing
        MsgBox("Kraj pripreme za stampu Nadele Table")

    End Sub

    Public Sub stampaObelezavanje(ByVal brojTable As Integer, ByVal prikaziSaveDialog As Boolean)

        Dim putanjaDoFile_ As String = ""

        If prikaziSaveDialog = True Then

            Try
                sf_diag.FileName = "obelezavanje_table_" & brojTable
                sf_diag.DefaultExt = "map"
                sf_diag.Filter = "Manifold Map file (*.map)|*.map"
                'sf_diag.FileName = "nadela_tabla" & ddl_ttpSpisakTabli.SelectedValue & ".map"
                sf_diag.Title = "Upisite naziv za izlazni Map File za stampu OBELEZAVANJA"
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

            putanjaDoFile_ = sf_diag.FileName

        Else

            putanjaDoFile_ = "c:\obelezavanjeTable" & brojTable & ".map"

        End If

        Dim manApp As Manifold.Interop.Application = New Manifold.Interop.Application : Dim newDoc As Manifold.Interop.Document = manApp.NewDocument("", False)

        Try
            newDoc.SaveAs(putanjaDoFile_)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju") : Exit Sub
        End Try

        Dim docOld As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        'idemo 
        'trebaju ti dva drawinga iz DKP_Nadela - skidas parcele i iz pntobelezavanje skidas tacke

        Dim drwNadelaOld, drwTackeObelezavanjeOld As Manifold.Interop.Drawing
        Try
            drwNadelaOld = docOld.ComponentSet(My.Settings.layerName_ParceleNadela)
        Catch ex As Exception
            MsgBox("Niste uradili nadelu - uradite pa onda stampa.") : Exit Sub
        End Try

        drwNadelaOld.SelectNone()

        Try
            drwTackeObelezavanjeOld = docOld.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Catch ex As Exception
            MsgBox("Ne postoji drawing za tackama za obelezavanje - formirajte ga pa ponovo.") : Exit Sub
        End Try

        drwTackeObelezavanjeOld.SelectNone()

        Dim drwNadelaNew As Manifold.Interop.Drawing = newDoc.NewDrawing("Parcele", drwNadelaOld.CoordinateSystem, True)
        Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing("Tacke", drwTackeObelezavanjeOld.CoordinateSystem, True)

        'sada selectujes u jednom i selektujes u drugom
        Dim qvrSelekt As Manifold.Interop.Query = docOld.NewQuery("kopiranje") : qvrSelekt.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [Selection (I)]=True where [idTable]=" & brojTable : qvrSelekt.RunEx(True)
        'prveoris da li ima nesto selektovano

        qvrSelekt.Text = "select count(*) from [" & My.Settings.layerName_ParceleNadela & "] where [Selection (I)]=true" : qvrSelekt.RunEx(True)

        If qvrSelekt.Table.RecordSet.Item(0).DataText(1) > 0 Then
            'sada mozes prosto copy paste!
            drwNadelaOld.Copy(True) : drwNadelaNew.Paste(True) : drwNadelaNew.SelectNone() : drwNadelaOld.SelectNone()
        Else
            'problem
            MsgBox("Nista nije selektovano u parcelama - verovatno niste uradili nadelu")
            Exit Sub
        End If

        Dim prazno_ As Manifold.Interop.Color = newDoc.Application.NewColor("proba", 0, 0, 1) : drwNadelaNew.AreaBackground.Set(prazno_)

        Dim drwMap As Manifold.Interop.Map = newDoc.NewMap("obelezavanjeTable" & brojTable, drwNadelaNew, drwNadelaNew.CoordinateSystem)
        'sada idemo na labele duzina frontova!

        'kreiras novi drawing za linije_
        Dim drwFront_ As Manifold.Interop.Drawing = newDoc.NewDrawing("LinijeFront", drwNadelaNew.CoordinateSystem, True)

        Dim qvrJJ As Manifold.Interop.Query = newDoc.NewQuery("fasdf")
        qvrJJ.Text = "insert into [LinijeFront] ([Geom (I)]) select ([Geom (I)]) from [Parcele]"
        qvrJJ.RunEx(True)

        'sada uvodis analizer
        Dim analizerF_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
        analizerF_.Boundaries(drwFront_, drwFront_, drwFront_.ObjectSet) : analizerF_.Explode(drwFront_, drwFront_.ObjectSet)
        analizerF_.RemoveDuplicates(drwFront_, drwFront_.ObjectSet) : analizerF_ = Nothing

        'newDoc.Save()

        'sada kreiras polje!
        Dim tbl_ As Manifold.Interop.Table : Dim col_ As Manifold.Interop.Column : tbl_ = drwFront_.OwnedTable
        col_ = newDoc.Application.NewColumnSet.NewColumn : col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat32 : tbl_.ColumnSet.Add(col_)
        col_ = Nothing : tbl_ = Nothing

        qvrJJ.Text = "delete from [linijefront] where isarea([id])=true" : qvrJJ.RunEx(True)
        qvrJJ.Text = "UPDATE [LinijeFront] set [Duzina]=round([Length (I)],2)" : qvrJJ.RunEx(True)


        newDoc.ComponentSet.Remove("fasdf")

        'sada kreiraas label za BROJEVI TACAKA !
        Dim labelFront_ As Manifold.Interop.Labels = newDoc.NewLabels("lbl_front", drwFront_, True, True)
        labelFront_.Text = "[Duzina]"
        labelFront_.LabelAlignX = LabelAlignX.LabelAlignXCenter : labelFront_.LabelAlignY = LabelAlignY.LabelAlignYCenter
        labelFront_.LeftToRight = False : labelFront_.MultipleLabelsPerBranch = False : labelFront_.OptimizeLabelAlignX = False
        labelFront_.OptimizeLabelAlignY = False : labelFront_.ResolveOverlaps = False : labelFront_.LabelEachBranch = False
        labelFront_.PerLabelFormat = False : labelFront_.LineOffset = 2


        Dim pLayer2 As Manifold.Interop.Layer = newDoc.NewLayer(labelFront_) : drwMap.LayerSet.Add(pLayer2)

        'newDoc.Save()
        qvrSelekt.Text = "UPDATE [" & My.Settings.layerName_pointTableObelezavanje & "] set [selection (I)]=true WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] IN (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] from [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_ParceleNadela & "] WHERE [" & My.Settings.layerName_ParceleNadela & "].[idTable]=" & brojTable & " and Contains([" & My.Settings.layerName_ParceleNadela & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]))" : qvrSelekt.RunEx(True)
        qvrSelekt.Text = "select count(*) from [" & My.Settings.layerName_pointTableObelezavanje & "] where [Selection (I)]=true" : qvrSelekt.RunEx(True)


        If qvrSelekt.Table.RecordSet.Item(0).DataText(1) > 0 Then
            'sada mozes prosto copy paste!
            drwTackeObelezavanjeOld.Copy(True) : drwTackeNew.Paste(True) : drwTackeObelezavanjeOld.SelectNone() : drwTackeNew.SelectNone()
        Else
            'problem
            MsgBox("Nista nije selektovano u parcelama - verovatno niste uradili nadelu") : Exit Sub
        End If


        'sada kreiraas label za BROJEVI TACAKA !
        Dim labelLinije_ As Manifold.Interop.Labels = newDoc.NewLabels("lbl_tacke", drwTackeNew, True, True)
        labelLinije_.Text = "[idTacke]"
        labelLinije_.LabelAlignX = LabelAlignX.LabelAlignXLeft
        labelLinije_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelLinije_.LeftToRight = False
        labelLinije_.MultipleLabelsPerBranch = False
        labelLinije_.OptimizeLabelAlignX = False
        labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False
        labelLinije_.LabelEachBranch = False
        labelLinije_.PerLabelFormat = True
        labelLinije_.LineOffset = 2

        Dim lbl_ As Manifold.Interop.Labels = newDoc.ComponentSet("lbl_tacke")

        Dim pLayer As Manifold.Interop.Layer = newDoc.NewLayer(drwTackeNew) : drwMap.LayerSet.Add(pLayer)
        Dim lLayer As Manifold.Interop.Layer = newDoc.NewLayer(lbl_) : drwMap.LayerSet.Add(lLayer)

        'sada layout
        Dim layout_ As Manifold.Interop.Layout = newDoc.NewLayout("obelezavanje", drwMap)
        Dim entitySet_ As Manifold.Interop.LayoutEntrySet = layout_.EntrySet

        entitySet_.Add(LayoutType.LayoutTypeText)

        entitySet_.Item(entitySet_.Count - 1).Text = "TABLA " & brojTable
        entitySet_.Item(entitySet_.Count - 1).TextAlignX = LabelAlignX.LabelAlignXLeft
        entitySet_.Item(entitySet_.Count - 1).TextAlignY = LabelAlignY.LabelAlignYTop

        newDoc.Save()

        Dim qvrExcel As Manifold.Interop.Query = newDoc.NewQuery("zaExcel")
        'treba ti excel file-e sa tackama za obelezavanje
        qvrExcel.Text = "SELECT [Tacke].[idTacke] as [BrTacke],[Tacke].[X (I)] as Yproj,[Tacke].[Y (I)] as Xproj, case when [tipTacke]=1 then " & Chr(34) & "Katastarska opstina" & Chr(34) & " when [tipTacke]=2 then " & Chr(34) & "Putevi Kanali" & Chr(34) & " when [tipTacke]=3 then " & Chr(34) & "Parcele" & Chr(34) & " end as TipTacke into [tmpList] from [Tacke]"
        qvrExcel.RunEx(True)
        Dim compTable As Manifold.Interop.Table = newDoc.ComponentSet("tmpList")

        'sada promenis - tip
        For i = 0 To compTable.ColumnSet.Count - 1
            Try
                compTable.ColumnSet.Item(i).Type = ColumnType.ColumnTypeWText
            Catch ex As Exception

            End Try
        Next

        newDoc.Save()
        qvrExcel.Text = "select [brTacke],[Yproj],[Xproj],[TipTacke] from [tmpList]"
        qvrExcel.Run()

        If prikaziSaveDialog = True Then
            sf_diag.FileName = "obelezavanje_tabla_" & brojTable
            sf_diag.Filter = "CSV file (*.csv)|*.csv"
            sf_diag.Title = "Upisite csv file za koordinate"
            sf_diag.ShowDialog()
            If sf_diag.FileName <> "" Then

                Dim freefile_ As Integer = FreeFile()
                FileOpen(freefile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
                PrintLine(freefile_, "brojtacke,y,x,tiptacke")
                For kk_ = 0 To qvrExcel.Table.RecordSet.Count - 1
                    PrintLine(freefile_, qvrExcel.Table.RecordSet.Item(kk_).DataText(1) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(2) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(3) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(4))
                Next

            End If
        End If

        docOld.ComponentSet.Remove("kopiranje") : newDoc.ComponentSet.Remove("zaExcel") ': newDoc.ComponentSet.Remove("zaExcel")

        qvrSelekt = Nothing
        drwMap = Nothing
        drwNadelaNew = Nothing
        drwNadelaOld = Nothing
        drwTackeNew = Nothing
        drwTackeObelezavanjeOld = Nothing

        newDoc.Save()
        'sve ovo bi trebalo da ide u nekom layout-u!?
        newDoc = Nothing
        manApp = Nothing

        If prikaziSaveDialog = True Then
            MsgBox("Kraj pripreme za stampu Obelezavanja")
        End If


    End Sub

    Public Sub stampajNadeluIObelezavanje(ByVal brojTable As Integer, ByVal prikaziShowDialog As Boolean)
         Dim putanjaDoIzlaza_ As String = ""

        If prikaziShowDialog = True Then
            Try
                sf_diag.FileName = "nadela_tabla_" & brojTable
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
            putanjaDoIzlaza_ = sf_diag.FileName
        Else

            putanjaDoIzlaza_ = "c:\tablaNadela_" & brojTable & ".map"

        End If

        Dim manApp As Manifold.Interop.Application = New Manifold.Interop.Application : Dim newDoc As Manifold.Interop.Document = manApp.NewDocument("", False)

        Try
            newDoc.SaveAs(putanjaDoIzlaza_)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju")
            Exit Sub
        End Try


        'drwNewTable - bez disolva
        'drwnewDisolve sa disolve 
        'mislim da bi i ovo trebalo izbaciti! drwnewtable jer ti netreba posle!

        'sada prvo moras da importujes drawing koji ti treba a to je disolve!
        Dim docOld As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        '& ddl_ttpSpisakTabli.SelectedValue &
        'TablaPR 21 dissolve

        'ovde treba promeniti da ide iz DKP_Nadela a ne iz disolve

        Dim tri_ As String = "TablaPR_" & CStr(brojTable) & "_dissolve"
        Dim drwOriginal As Manifold.Interop.Drawing
        'Try
        '    drwOriginal = docOld.ComponentSet(tri_)
        'Catch ex As Exception
        '    MsgBox("Nemate tablu pod nazivom " & tri_)
        '    Exit Sub
        'End Try

        Dim qvrNest As Manifold.Interop.Query = docOld.NewQuery("sfdfd")
        qvrNest.Text = "select * from [" & My.Settings.layerName_ParceleNadela & "] where [idTable]=" & CStr(brojTable)
        qvrNest.RunEx(True)

        If qvrNest.Table.RecordSet.Count > 0 Then
            drwOriginal = docOld.ComponentSet(My.Settings.layerName_ParceleNadela)
            qvrNest.Text = "update [" & My.Settings.layerName_ParceleNadela & "] set [Selection (I)]=true where [idTable]=" & CStr(brojTable)
            qvrNest.RunEx(True)
        Else
            MsgBox("Nemate tablu u DKP_Nadela. Proveriti")
            Exit Sub
        End If


        Dim analizerF_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
        Dim drwnewDisolve As Manifold.Interop.Drawing = newDoc.NewDrawing(tri_, drwOriginal.CoordinateSystem, True)

        'sada copy - paste da ga preneses!
        drwOriginal.Copy(True) : drwnewDisolve.Paste(True) : drwOriginal.SelectNone() : drwnewDisolve.SelectNone()

        Dim drwTackeOld As Manifold.Interop.Drawing = docOld.ComponentSet("Tacke") : Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing("Tacke", drwTackeOld.CoordinateSystem, True)

        drwTackeOld.Copy(False) : drwTackeNew.Paste(True) : drwTackeOld.SelectNone() : drwTackeNew.SelectNone()

        Dim drwTackeObelezavanjeOld As Manifold.Interop.Drawing = docOld.ComponentSet(My.Settings.layerName_pointTableObelezavanje)

        'sada mozes dalje nemas vise problema

        Dim drwLinije As Manifold.Interop.Drawing = newDoc.NewDrawing("tablePR " & brojTable & " linije", drwnewDisolve.CoordinateSystem, True)
        'sada treba na osnovu ovoga da uradi nesto da kreira na primer linije koji opisuju zbog rastojanja - odnosno 

        'da dobijes linije
        analizerF_.Boundaries(drwnewDisolve, drwnewDisolve, drwnewDisolve.ObjectSet) : analizerF_.Explode(drwnewDisolve, drwnewDisolve.ObjectSet)
        analizerF_.RemoveDuplicates(drwnewDisolve, drwnewDisolve.ObjectSet) : analizerF_ = Nothing

        newDoc.Save()

        Dim qvrSelect As Manifold.Interop.Query = newDoc.NewQuery("selekcijaLinija")
        qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Selection (i)]=true where isLine([ID])=true"
        qvrSelect.RunEx(True) : drwnewDisolve.Cut(True) : drwLinije.Paste()

        drwnewDisolve.SelectNone() : drwLinije.SelectNone()

        Dim prazno_ As Manifold.Interop.Color = newDoc.Application.NewColor("proba", 0, 0, 1) : drwnewDisolve.AreaBackground.Set(prazno_)

        Dim tbl_ As Manifold.Interop.Table : Dim col_ As Manifold.Interop.Column

        tbl_ = drwLinije.OwnedTable : col_ = newDoc.Application.NewColumnSet.NewColumn
        'col_.Name = "Duzina" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat32 : tbl_.ColumnSet.Add(col_)

        tbl_ = drwnewDisolve.OwnedTable
        col_.Name = "Opis1" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeWText : tbl_.ColumnSet.Add(col_)
        col_.Name = "Opis2" : tbl_.ColumnSet.Add(col_)

        col_ = Nothing : tbl_ = Nothing

        qvrSelect.Text = "SELECT CentroidX([Geom (I)]),CentroidY([Geom (I)]),round([Length (I)],2),[Bearing (I)] FROM [" & "tablePR " & brojTable & " linije" & "]" : qvrSelect.RunEx(True)

        Dim drwLabel As Manifold.Interop.Labels = newDoc.NewLabels(drwLinije.Name & "_label", drwLinije.CoordinateSystem, True)
        drwLabel.LabelAlignX = LabelAlignX.LabelAlignXCenter : drwLabel.LabelAlignY = LabelAlignY.LabelAlignYCenter
        drwLabel.OptimizeLabelAlignX = False : drwLabel.OptimizeLabelAlignY = False : drwLabel.ResolveOverlaps = False : drwLabel.PerLabelFormat = True

        Dim lblSets_ As Manifold.Interop.LabelSet = drwLabel.LabelSet

        For i = 0 To qvrSelect.Table.RecordSet.Count - 1
            'e sada da vidimo kreiras tacku i onda ide dalje
            Dim pnt_ As Manifold.Interop.Point = newDoc.Application.NewPoint(qvrSelect.Table.RecordSet.Item(i).DataText(1), qvrSelect.Table.RecordSet.Item(i).DataText(2))
            lblSets_.Add(qvrSelect.Table.RecordSet.Item(i).DataText(3), pnt_)
            'sada treba nekako rotacija?
            Dim labb_ As Manifold.Interop.Label = lblSets_.LastAdded
            labb_.Rotation = qvrSelect.Table.RecordSet.Item(i).DataText(4) - 90
            labb_.Size = 5
            pnt_ = Nothing
        Next

        Dim map_ As Manifold.Interop.Map = newDoc.NewMap("TablaPR_" & brojTable & " map", drwnewDisolve, drwnewDisolve.CoordinateSystem, True)
        Dim pLayer As Manifold.Interop.Layer = newDoc.NewLayer(drwLabel)
        map_.LayerSet.Add(pLayer)

        'sad ide zanimljiviji deo ! znaci treba za ovu tabelu da selektujes sve ono sto ti treba a to je:
        'idNadele, vrednost, povrsina, ime i prezime

        Dim qvrZaNadeluInfo As Manifold.Interop.Query = newDoc.NewQuery("nadelaInfo")
        qvrZaNadeluInfo.Text = "select [idVlasnika],[ID],[redniBrNadele] from [tablaPR_" & brojTable & "_dissolve]"
        qvrZaNadeluInfo.RunEx(True)
        ' conn_.Open()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim myCommand As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        Dim myReader As MySqlDataReader

        For i = 0 To qvrZaNadeluInfo.Table.RecordSet.Count - 1
            Dim stsql_ As String = "select rednibrojnadele, nadeljenoPovrsina,nadeljenoVrednost from kom_tableNadela where obrisan=0 and idTable=" & brojTable & " and idIskazZemljista=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1) & " and rednibrojnadele=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(3)
            myCommand.CommandText = stsql_
            'conn_.Open()
            'sada ti treba reader da bi ovo sastavio kao text 
            Try
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            myReader.Read()
            If myReader.HasRows Then
                'sada imas sve sto ti treba - sto se teksta tice
                Dim text_ As String = "L=" & myReader.GetValue(0) & " V=" & myReader.GetValue(2) & " P=" & Math.Round(Val(myReader.GetValue(1)), 0) & " I=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1)
                qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis1]=" & Chr(34) & text_ & Chr(34) & " where [ID]=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(2)
                qvrSelect.RunEx(True)
                myReader.Close()
                myReader = Nothing
            Else
                'imas problem
                myReader.Close()
                myReader = Nothing
            End If
            'sada mozes na drugi deo a to su vlasnici
        Next

        'sada vlasnikk 
        For i = 0 To qvrZaNadeluInfo.Table.RecordSet.Count - 1
            Dim stsql_ As String = "SELECT group_concat( distinct concat(PREZIME, if(isnull(IMEOCA),'',concat(' (',IMEOCA,') ')),IME,' ',kom_vezaparcelavlasnik.Udeo) SEPARATOR ';') FROM kom_vlasnik,kom_vezaparcelavlasnik WHERE kom_vezaparcelavlasnik.idVlasnika=kom_vlasnik.idVlasnika and kom_vezaparcelavlasnik.obrisan=0 and idiskazzemljista=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(1)
            myCommand.CommandText = stsql_

            Try
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            If myReader.HasRows Then
                myReader.Read()
                'sada imas sve sto ti treba - sto se teksta tice
                'Dim sta_ As String = myReader.GetValue(0)
                'sta_ = Replace(sta_, ",", vbCrLf)
                Try
                    qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis2]=" & Chr(34) & myReader.GetValue(0) & Chr(34) & " where [ID]=" & qvrZaNadeluInfo.Table.RecordSet.Item(i).DataText(2)
                    qvrSelect.RunEx(True)
                    qvrSelect.Text = "update [tablaPR_" & brojTable & "_dissolve] set [Opis2]=Replace([Opis2]," & Chr(34) & ";" & Chr(34) & ",chr(10) & chr(13))"
                    qvrSelect.RunEx(True)
                Catch ex As Exception
                    MsgBox("Proverite da u nazivu nemate znake navoda - u bazi, jer onda generisem gresku kao sada.")
                End Try
                myReader.Close()
                myReader = Nothing
            Else
                'imas problem
                myReader.Close()
                myReader = Nothing
            End If
        Next

        'sada od ova dva pravis novi label koji dodajes jos to da vidimo kako i gotovo!
        Dim labelLinije_ As Manifold.Interop.Labels
        labelLinije_ = newDoc.NewLabels("Opisi", drwnewDisolve, True, True)
        labelLinije_.Text = "[Opis2]" & vbCrLf & "[Opis1]"

        labelLinije_.LeftToRight = False
        labelLinije_.MultipleLabelsPerBranch = False
        labelLinije_.LineOffset = 3
        labelLinije_.OptimizeLabelAlignX = False
        labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False
        labelLinije_.Synchronized = False
        labelLinije_.Synchronized = True
        labelLinije_.PerLabelFormat = True

        Dim lbl_ As Manifold.Interop.Labels
        Dim labelSet_ As Manifold.Interop.LabelSet
        lbl_ = newDoc.ComponentSet("Opisi")
        labelSet_ = lbl_.LabelSet

        qvrSelect.Text = "SELECT CentroidX([Geom (I)]) as X,CentroidY([Geom (I)]) as Y FROM [" & My.Settings.layerName_nadelaSmer & "] WHERE [idtable]=" & brojTable
        qvrSelect.RunEx(True)

        Dim P_

        Try
            P_ = NiAnaB(qvrSelect.Table.RecordSet.Item(0).DataText(1), qvrSelect.Table.RecordSet.Item(0).DataText(2), qvrSelect.Table.RecordSet.Item(1).DataText(1), qvrSelect.Table.RecordSet.Item(1).DataText(2))
        Catch ex As Exception
            MsgBox("Problem sa tackama moje definisu pravac nadele!")
            'treba videti sta je napravio i sta treba obrisati
            Exit Sub
        End Try

        For Each lab_ In labelSet_
            lab_.rotation = P_ - 90
            lab_.size = 7
        Next
        pLayer = newDoc.NewLayer(labelLinije_)
        map_.LayerSet.Add(pLayer)

        Dim qvrNestoBrisi As Manifold.Interop.Query = docOld.NewQuery("nesoBrisiKao")

        qvrNestoBrisi.Text = "update  [" & My.Settings.layerName_pointTableObelezavanje & "] set [selection (I)]=false"
        qvrNestoBrisi.RunEx(True)
        qvrNestoBrisi.Text = "UPDATE [" & My.Settings.layerName_pointTableObelezavanje & "] set [selection (I)]=true WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] IN (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] from [" & My.Settings.layerName_pointTableObelezavanje & "],[" & My.Settings.layerName_ParceleNadela & "] WHERE [" & My.Settings.layerName_ParceleNadela & "].[idTable]=" & brojTable & " and Contains([" & My.Settings.layerName_ParceleNadela & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]))"
        qvrNestoBrisi.RunEx(True)
        qvrNestoBrisi.Text = "select count(*) from [" & My.Settings.layerName_pointTableObelezavanje & "] where [Selection (I)]=true"
        qvrNestoBrisi.RunEx(True)


        'SADA bi trebalo  da drawing tacle nije potreban - aj da probamo da obirsemo sve i da kopiramo u njega
        Dim qvrBrisi As Manifold.Interop.Query = newDoc.NewQuery("brisiOdmah")
        qvrBrisi.Text = "delete from [Tacke]"
        qvrBrisi.RunEx(True)
        newDoc.ComponentSet.Remove("brisiOdmah")

        If qvrNestoBrisi.Table.RecordSet.Item(0).DataText(1) > 0 Then
            'sada mozes prosto copy paste!
            drwTackeObelezavanjeOld.Copy(True) : drwTackeNew.Paste(True) : drwTackeObelezavanjeOld.SelectNone() : drwTackeNew.SelectNone()
        Else
            'problem
            MsgBox("Nista nije selektovano u parcelama - verovatno niste uradili nadelu") : Exit Sub
        End If


        'sada kreiraas label za BROJEVI TACAKA !
        labelLinije_ = newDoc.NewLabels("lbl_tacke", drwTackeNew, True, True)
        labelLinije_.Text = "[idTacke]"
        labelLinije_.LabelAlignX = LabelAlignX.LabelAlignXLeft
        labelLinije_.LabelAlignY = LabelAlignY.LabelAlignYTop
        labelLinije_.LeftToRight = False
        labelLinije_.MultipleLabelsPerBranch = False
        labelLinije_.OptimizeLabelAlignX = False
        labelLinije_.OptimizeLabelAlignY = False
        labelLinije_.ResolveOverlaps = False
        labelLinije_.LabelEachBranch = False
        labelLinije_.PerLabelFormat = True
        labelLinije_.LineOffset = 2

        labelSet_ = labelLinije_.LabelSet
        For Each lab_ In labelSet_
            lab_.rotation = P_ - 90
        Next

        lbl_ = newDoc.ComponentSet("lbl_tacke")

        pLayer = newDoc.NewLayer(drwTackeNew) : map_.LayerSet.Add(pLayer)
        pLayer = newDoc.NewLayer(lbl_) : map_.LayerSet.Add(pLayer)

        newDoc.Save()

        Dim qvrExcel As Manifold.Interop.Query = newDoc.NewQuery("zaExcel")
        'treba ti excel file-e sa tackama za obelezavanje
        qvrExcel.Text = "SELECT [Tacke].[idTacke] as [BrTacke],[Tacke].[X (I)] as Yproj,[Tacke].[Y (I)] as Xproj, case when [tipTacke]=1 then " & Chr(34) & "Katastarska opstina" & Chr(34) & " when [tipTacke]=2 then " & Chr(34) & "Putevi Kanali" & Chr(34) & " when [tipTacke]=3 then " & Chr(34) & "Parcele" & Chr(34) & " end as TipTacke into [tmpList] from [Tacke]"
        qvrExcel.RunEx(True)
        Dim compTable As Manifold.Interop.Table = newDoc.ComponentSet("tmpList")

        'sada promenis - tip
        For i = 0 To compTable.ColumnSet.Count - 1
            Try
                compTable.ColumnSet.Item(i).Type = ColumnType.ColumnTypeWText
            Catch ex As Exception

            End Try
        Next

        newDoc.Save()
        qvrExcel.Text = "select [brTacke],[Yproj],[Xproj],[TipTacke] from [tmpList]"
        qvrExcel.Run()

        If prikaziShowDialog = True Then
            sf_diag.FileName = "obelezavanje_tabla_" & brojTable
            sf_diag.Filter = "CSV file (*.csv)|*.csv"
            sf_diag.Title = "Upisite csv file za koordinate"
            sf_diag.ShowDialog()
            If sf_diag.FileName <> "" Then

                Dim freefile_ As Integer = FreeFile()
                FileOpen(freefile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
                PrintLine(freefile_, "brojtacke,y,x,tiptacke")
                For kk_ = 0 To qvrExcel.Table.RecordSet.Count - 1
                    PrintLine(freefile_, qvrExcel.Table.RecordSet.Item(kk_).DataText(1) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(2) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(3) & "," & qvrExcel.Table.RecordSet.Item(kk_).DataText(4))
                Next

            End If
        End If

        Dim layout_ As Manifold.Interop.Layout = newDoc.NewLayout("Tabla" & brojTable & " Stampa", map_)
        Dim entitySet_ As Manifold.Interop.LayoutEntrySet = layout_.EntrySet
        'fali broj table - gde to ubaciti?
        entitySet_.Add(LayoutType.LayoutTypeText)
        entitySet_.Item(entitySet_.Count - 1).Text = "TABLA " & ddl_ttpSpisakTabli.SelectedText
        entitySet_.Item(entitySet_.Count - 1).TextAlignX = LabelAlignX.LabelAlignXLeft
        entitySet_.Item(entitySet_.Count - 1).TextAlignY = LabelAlignY.LabelAlignYTop

        newDoc.ComponentSet.Remove("zaExcel") ': newDoc.ComponentSet.Remove("zaExcel")


        'pitanje kako postaviti velicinu fonta!!!!!!!!!!!! veliko pitanje 

        qvrZaNadeluInfo = Nothing
        qvrSelect = Nothing
        newDoc.ComponentSet.Remove("nadelaInfo")
        newDoc.ComponentSet.Remove("selekcijaLinija")
        'Dim qvrNest As Manifold.Interop.Query = docOld.NewQuery("sfdfd")
        docOld.ComponentSet.Remove("sfdfd")
        qvrNest = Nothing
        labelLinije_ = Nothing
        labelSet_ = Nothing
        lbl_ = Nothing
        pLayer = Nothing
        'map_.Open()
        map_ = Nothing
        newDoc.Save()
        conn_ = Nothing
        drwnewDisolve = Nothing
        myCommand = Nothing
        newDoc = Nothing
        docOld = Nothing
        manApp = Nothing
        MsgBox("Kraj pripreme za stampu Nadele Table")

    End Sub

    Private Sub stampajObelezavanje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        stampaObelezavanje(ddl_ttpSpisakTabli.SelectedValue, True)
    End Sub

    Private Sub IzBazeKomtableNadelaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles IzBazeKomtableNadelaToolStripMenuItem.Click

        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        tss_label.Text = "Priprema potrebnih drawing-a"
        My.Application.DoEvents()
        'prvo proveris da li postoje podeseni drawing-i

        Dim drw_table As Manifold.Interop.Drawing
        Try
            drw_table = doc.ComponentSet.Item(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Proverite definiciju ulaznih parametara pa pokrenite jos jednom")
            Exit Sub
        End Try

        Dim drwDKP As Manifold.Interop.Drawing
        Try
            drwDKP = doc.NewDrawing(My.Settings.layerName_ParceleNadela, drw_table.CoordinateSystem, True)
        Catch ex As Exception
            drwDKP = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        End Try

        Dim drw_procRazredi As Manifold.Interop.Drawing
        Try
            drw_procRazredi = doc.ComponentSet.Item(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing sa procenom. Proverite.")
            Exit Sub
        End Try

        Dim g_ As Manifold.Interop.Drawing
        Try
            g_ = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing sa detaljnim tackama. Proverite.")
            Exit Sub
        End Try
        'prvo proveris dali u gridu ima nesto

        If gridTableNadela.RowCount = 0 Then
            MsgBox("Izabeite tablu u kojoj ima nesto za nadelu.")
            Exit Sub
        End If

        '//ucitava nadelu iz tabele kom_tableNadela
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim myCommand As New MySql.Data.MySqlClient.MySqlCommand("select idIskazZemljista,nadeljenoVrednost,redniBrojNadele from kom_tableNadela where idTable=" & ddl_ttpSpisakTabli.SelectedValue & " order by rednibrojnadele", conn_)
        conn_.Open()
        Dim myReader As MySqlDataReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
        Dim sumaVrednosti As Double = 0 : Dim matVlasnika(-1) As Integer : Dim matVrednosti(-1) As Double : Dim matRbNadele(-1) As Integer : Dim brojac_ As Integer = 0

        If myReader.HasRows = False Then
            MsgBox("Nadela nije definisana za ovu tablu!")
            Exit Sub
        Else
            While myReader.Read
                ReDim Preserve matVlasnika(brojac_) : ReDim Preserve matVrednosti(brojac_) : ReDim Preserve matRbNadele(brojac_)
                matVlasnika(brojac_) = myReader.GetValue(0) : matVrednosti(brojac_) = myReader.GetValue(1) : matRbNadele(brojac_) = myReader.GetValue(2)
                sumaVrednosti += myReader.GetValue(1) : brojac_ += 1
            End While
        End If
        myReader.Close()

        Dim vrednostTableKomTable As Double = -1
        myCommand.CommandText = "SELECT VSuma FROM kom_kfmns where obrisan=0 and idTable=" & ddl_ttpSpisakTabli.SelectedValue

        Try
            conn_.Open()
        Catch ex As Exception

        End Try

        myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection) : myReader.Read() : vrednostTableKomTable = myReader.GetValue(0) : myReader.Close() : myReader = Nothing
        conn_.Close()

        tss_label.Text = "Priprema potrebnih drawing-a"
        My.Application.DoEvents()

        'napravis prvo presek procembenih razreda i tabli
        Dim tbl_ As Manifold.Interop.Table
        'ovde bi trebalo obrisati table_pr_razred i ponovo ga napraviti
        Try
            Dim drw As Manifold.Interop.Drawing = doc.ComponentSet("table_pr_razred")
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception
        End Try

        'sada prvo proveris dali postoji! ako postoji onda pitas dali brises e a ako brises onda treba brisati na vise mesta 
        'í to je sad pitanje na koliko!?

        Dim name_ = "tablaPR_" & ddl_ttpSpisakTabli.SelectedValue.ToString & "_dissolve"
        Dim name2_ = "tablaPR_" & ddl_ttpSpisakTabli.SelectedValue.ToString
        If doc.ComponentSet.ItemByName(name_) <> -1 Or doc.ComponentSet.ItemByName(name2_) <> -1 Then
            'postoji idemo sa pitanjem dali brisem i ako je potvrdan odgovor izlaz napolje
            Dim odgovor = MsgBox("Nadela za ovu tablu je vec radena, da li je brisem i radim ispocetka?", MsgBoxStyle.OkCancel, "Pitanje?")

            If odgovor = MsgBoxResult.Ok Then
                'uf sad brisem - e al sta i kako ?
                Try
                    'brises drawinge
                    Try
                        doc.ComponentSet.Remove(name_)
                    Catch ex As Exception

                    End Try

                    Try
                        doc.ComponentSet.Remove(name2_)
                    Catch ex As Exception

                    End Try
                Catch ex As Exception

                End Try
            Else

                'brises sta je u memoriji jer je odgovor ne!
                doc = Nothing
                drw_table = Nothing
                drwDKP = Nothing
                Exit Sub

            End If

        End If

        'podesis da je samo ova tabla status1

        Dim freefile_ As Integer = FreeFile()
        FileOpen(freefile_, Path.GetTempPath() & "\nadela_table_" & ddl_ttpSpisakTabli.SelectedValue & ".txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        '// pravi presek tabli i procembenih razreda koji ti treba kasnije kod pravljenja parcela
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
            tbl_ = drw_table.OwnedTable
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
        topPRazredi.Bind(drw_procRazredi)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drw_table)
        topParcele.Build()

        topParcele.DoIntersect(topPRazredi, "table_pr_razred")

        Dim topParcPrRaz As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcPrRaz.Bind(doc.ComponentSet.Item(doc.ComponentSet.ItemByName("table_pr_razred")))
        topParcPrRaz.Build()

        topPRazredi = Nothing : topParcPrRaz = Nothing : topParcele = Nothing

        doc.Save()

        Dim drwFR As Manifold.Interop.Drawing = doc.ComponentSet.Item("table_pr_razred")
        'sada bi trebalo svaku tablu da delis na isti segment! kako 
        Dim qvr_ As Manifold.Interop.Query
        Try
            qvr_ = doc.NewQuery("brojTabli", True)
        Catch ex As Exception
            qvr_ = doc.ComponentSet("brojTabli")
        End Try


        'ovde negde ti treba selekcija iz baze
        qvr_.Text = "select sum([table_pr_razred].[Area (I)]*[Faktor]) as vrednost from [table_pr_razred] WHERE [idTable]=" & ddl_ttpSpisakTabli.SelectedValue
        qvr_.RunEx(True)

        tss_label.Text = "Suma ulaznih vrednosti je: " & sumaVrednosti & ", a ukupna vrednost za nadelu u tabli je: " & qvr_.Table.RecordSet(0).DataText(1) & " dok je u kom_kfmns=" & vrednostTableKomTable
        My.Application.DoEvents()

        Dim brojacNer_ As Integer = 0

        doc.Save()

        Dim drwNewTable As Manifold.Interop.Drawing = doc.NewDrawing(name2_, drw_table.CoordinateSystem, True)
        'sada sve copiras i njega ali cemo ovo preko queryja ne ovako!
        Dim qvrCopy As Manifold.Interop.Query = doc.NewQuery("kopirajTablu", True)

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "OldID"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idVlasnika"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "redniBrNadele"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idTable"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        'col_.Name = "tipTable"
        'drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "procembeni"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "faktor"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_ = Nothing

        qvrCopy.Text = "insert into [" & drwNewTable.Name & "] ([idtable],[procembeni],[faktor],[Geom (I)]) (select [idtable],[procembeni],[faktor],[Geom (I)] FROM [table_pr_razred] where [idTable]=" & ddl_ttpSpisakTabli.SelectedValue & ")"
        'qvrCopy.Text = "insert into [" & drwNewTable.Name & "] ([idtable],[tipTable],[procembeni],[faktor],[Geom (I)]) (select [idtable],[tipTable],[procembeni],[faktor],[Geom (I)] FROM [table_pr_razred] where [idTable]=" & ddl_ttpSpisakTabli.SelectedValue & ")"
        qvrCopy.RunEx(True)


        doc.ComponentSet.Remove("kopirajTablu")
        qvrCopy = Nothing
        'sada mozes odmah deobu a mozes i u sledecem krugu

        'doc.Save()

        Dim qvrDist As Manifold.Interop.Query
        Try
            qvrDist = doc.NewQuery("dist", True)
        Catch ex As Exception
            qvrDist = doc.ComponentSet("dist")
        End Try


        qvrDist.Text = "SELECT top 1 distance(A.[geom (I)],B.[Geom (I)]) as dist_,atn2(CentroidX(A.[Geom (I)])-CentroidX(B.[Geom (I)]),CentroidY(A.[Geom (I)])-CentroidY(B.[Geom (I)])) as ugao_,CentroidX(A.[Geom (I)]) as x1,CentroidY(A.[Geom (I)]) as y1,CentroidX(B.[Geom (I)]) as x2,CentroidY(B.[Geom (I)]) as y2 FROM ((SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & ddl_ttpSpisakTabli.SelectedValue & ") as A,(SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & ddl_ttpSpisakTabli.SelectedValue & ") as B) WHERE A.[ID]<>B.[ID]" : qvrDist.RunEx(True)

        Dim sumazaDeob_ As Double
        pb1.Maximum = UBound(matVlasnika) : pb1.Value = 0
        'insertujes nultu liniju!
        Dim pnt1(1), pnt2(1) As Double

        Dim analizerF_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        Dim qvrLastID As Manifold.Interop.Query
        Try
            qvrLastID = doc.NewQuery("lastID", True)
        Catch ex As Exception
            qvrLastID = doc.ComponentSet("lastID")
        End Try

        If qvrDist.Table.RecordSet.Count > 0 Then

            pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
            pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
            Dim du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))

            podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -10, pnt1, pnt2, doc, drwNewTable, 2)
            Dim matLinija() As Integer : ReDim Preserve matLinija(0)

            qvrLastID.Text = "select top 1 [ID] from [" & drwNewTable.Name & "] order by [ID] Desc" : qvrLastID.RunEx(True)
            matLinija(0) = qvrLastID.Table.RecordSet(0).DataText(1)
            'doc.Save()
            PrintLine(freefile_, "Poceo sa obradom table u " & Now())

            For j = 0 To UBound(matVlasnika) - 1  'ovde zamnei sa necim na ulazu!
                pb1.Value = j
                gridTableNadela.Rows(j).Selected = True
                Dim kraj As Boolean = False
                'treba ti rastojanje izmedu dve tacke
                sumazaDeob_ += matVrednosti(j)

                'treba ga sjebati kod poslednjeg jer ovde pravi problem !
                'sumazaDeob_ = matVrednosti(j)
                Dim h_ As Double = H_racunanjeDirektno(qvrDist.Table.RecordSet(0).DataText(1), qvrDist.Table.RecordSet(0).DataText(1), qvr_.Table.RecordSet.Item(0).DataText(1), sumazaDeob_)
                'sada treba videti dali je to ok!
                Dim interacija_ As Integer = -1
                'pb2.Maximum = 300
                Do While Not kraj = True
                    interacija_ += 1
                    'pb2.Value = pb2.Value + 1
                    PrintLine(freefile_, "Poceo interaciju " & interacija_)

                    Dim drwTemp As Manifold.Interop.Drawing = doc.NewDrawing("temp", drwNewTable.CoordinateSystem, True)
                    'drwNewTable.Copy() : drwTemp.Paste() : My.Computer.Clipboard.Clear() 'ocisti clipboard ali kako?
                    col_ = doc.Application.NewColumnSet.NewColumn
                    col_.Name = "OldID"
                    col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idVlasnika"
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "redniBrNadele"
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idTable"
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    'col_.Name = "tipTable"
                    'drwNewTable.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "procembeni"
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "faktor"
                    col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
                    drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_ = Nothing

                    Dim q_ As Manifold.Interop.Query = doc.NewQuery("dfas")
                    q_.Text = "insert into [temp] (OldID,idVlasnika,redniBrNadele,idTable,procembeni,faktor,[geom (i)]) (select OldID,idVlasnika,redniBrNadele,idTable,procembeni,faktor,[geom (i)] from [" & drwNewTable.Name & "])"
                    'q_.Text = "insert into [temp] (OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)]) (select OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)] from [" & drwNewTable.Name & "])"
                    q_.RunEx(True)
                    doc.ComponentSet.Remove("dfas")
                    'doc.Save()

                    Dim drwLine As Manifold.Interop.Drawing = doc.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                    'Dim pnt1(1), pnt2(1) As Double
                    pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
                    pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                    du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
                    podeliParceluViseDelova_KreirajPresecnuLiniju(du - 90, qvrDist.Table.RecordSet(0).DataText(1), h_, pnt1, pnt2, doc, drwLine)
                    Dim lineID As Integer = drwLine.ObjectSet.Item(0).ID
                    'sada imas liniju i ide presek

                    'Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
                    analizerF_.Split(drwTemp, drwTemp, drwTemp.ObjectSet, drwLine.ObjectSet)
                    'doc.Save()
                    'analizer_ = Nothing
                    'problem kako da selektujes poligone koji su nastali naknadno? izmedu linije i dve tacke?
                    podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -h_, pnt1, pnt2, doc, drwLine, 2)
                    'sada ti treba convechull da napravi poligon
                    Dim qvrDobijenaPov As Manifold.Interop.Query = doc.NewQuery("koliko")
                    qvrDobijenaPov.Text = "insert into [linije] ([Geom (I)]) VALUES (SELECT ConvexHull(AllCoords([Geom (I)])) FROM [Linije])"
                    qvrDobijenaPov.RunEx(True)
                    'sada radis update faktora! za svaki slucaj! ovo bi trebalo da napravis i gore
                    qvrDobijenaPov.Text = "update (SELECT [temp].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([Temp], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [Temp].[ID])) set out_ = in_"
                    qvrDobijenaPov.RunEx(True)
                    qvrDobijenaPov.Text = "SELECT sum([temp].[Area (I)]*[temp].[Faktor]) FROM [Temp],[Linije] WHERE IsArea([Temp].[ID]) and  Contains([Linije].[ID],[Temp].[ID])"
                    qvrDobijenaPov.RunEx(True)
                    'SADA treba skratiti celu pricu!
                    'doc.Save()
                    If interacija_ > My.Settings.nadela_brInteracija Then
                        MsgBox("Nesto nije u redu!?")
                        Exit Sub
                    End If

                    tss_label.Text = "Deoba za iskaz " & matVlasnika(j) & " razlika> " & (Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - sumazaDeob_, 2))
                    My.Application.DoEvents()

                    If Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - sumazaDeob_, 2) = 0 Then
                        'pronasao! izlazim iz ovoga kreiras liniju u temp3
                        drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                        qvrLastID.RunEx(True)
                        ReDim Preserve matLinija(j + 1)
                        matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        kraj = True
                    Else
                        'idemo iz pocetka
                        h_ = h_ + ((sumazaDeob_ - Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))) / qvrDist.Table.RecordSet(0).DataText(1))
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        drwLine = Nothing : drwTemp = Nothing
                        PrintLine(freefile_, "kraj interacije " & Now())
                        interacija_ = -1
                    End If
                    doc.ComponentSet.Remove("koliko")
                    qvrDobijenaPov = Nothing
                Loop

            Next

            gridTableNadela.Rows(matVlasnika.Length - 1).Selected = True
            PrintLine(freefile_, "Kraj " & Now()) : pb1.Value = 0

            'sada bi trebalo kreirati poslednju liniju koja je negde u pizdicima i koju bi trebalo da doda poslednji id!
            'sada ostaje da se isece i dodele ID
            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [OldID]=[ID]" : qvrLastID.RunEx(True)
            'doc.Save()

            analizerF_.Split(drwNewTable, drwNewTable, drwNewTable.ObjectSet, drwNewTable.ObjectSet)

            For i = 0 To matLinija.Length - 2
                qvrLastID.Text = "update (SELECT [" & My.Settings.parcele_fieldName_Vlasnik & "],[redniBrNadele] from [" & drwNewTable.Name & "] WHERE Contains((SELECT ConvexHull(AllCoords([Geom (I)])) FROM [" & drwNewTable.Name & "] WHERE [OldID]=" & matLinija(i) & " or [OldID]=" & matLinija(i + 1) & "),[ID]) AND IsArea([ID])) set " & My.Settings.parcele_fieldName_Vlasnik & "=" & matVlasnika(i) & ", redniBrNadele=" & matRbNadele(i)
                qvrLastID.RunEx(True)
            Next

            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & matVlasnika(matVlasnika.Length - 1) & ", [redniBrNadele]=" & matRbNadele(matRbNadele.Length - 1) & " where [idVlasnika]=0 and [redniBrNadele]=0 and IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

            doc.Save()
        Else

            'sada treba da update napravis ovog jednog kojeg imasa!
            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & matVlasnika(matVlasnika.Length - 1) & ", [redniBrNadele]=1 where IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

        End If


        Dim drwnewDisolve As Manifold.Interop.Drawing = doc.NewDrawing(drwNewTable.Name & "_dissolve", drwNewTable.CoordinateSystem, True)
        'sada kreiras polje za pocetak rednibrnadele i idtable i idvlasnika
        tbl_ = drwnewDisolve.OwnedTable
        col_ = doc.Application.NewColumnSet.NewColumn
        col_.Name = "redniBrNadele"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idtable"
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idvlasnika"
        tbl_.ColumnSet.Add(col_)
        'sada jedino merge na osnovu selekcije koja ide izmedu linija jer drugacije nece da radi - na primer disolve 
        'ili recimo redni broj nadele? - kako ovo da dobijes? isto 

        tbl_ = Nothing : col_ = Nothing
        'doc.Save()
        qvrDist.Text = "INSERT INTO [" & drwnewDisolve.Name & "] ([Geom (I)],[redniBrNadele]) (SELECT * FROM (SELECT UnionAll([ID]) as pera_,[redniBrNadele] FROM [" & drwNewTable.Name & "] GROUP BY [redniBrNadele] ) where pera_ IS NOT NULL )"
        qvrDist.RunEx(True)
        qvrDist.Text = "update (SELECT [" & drwNewTable.Name & "].[idVlasnika] as in_,[" & drwnewDisolve.Name & "].[idVlasnika] as out_ FROM [" & drwNewTable.Name & "],[" & drwnewDisolve.Name & "] WHERE [" & drwNewTable.Name & "].[redniBrNadele]=[" & drwnewDisolve.Name & "].[redniBrNadele] ) set out_=in_ "
        qvrDist.RunEx(True)
        qvrDist.Text = "update [" & drwnewDisolve.Name & "] set [idTable]=" & ddl_ttpSpisakTabli.SelectedValue.ToString
        qvrDist.RunEx(True)
        'sada ostaje da napravis update za idtable i vlasnika!
        analizerF_.NormalizeTopology(drwnewDisolve, drwnewDisolve.ObjectSet)

        'brises parcele koje definisu tablu!
        Try
            Dim qvrBrisi2 As Manifold.Interop.Query = doc.NewQuery("brisiizDKP")
            qvrBrisi2.Text = "delete from [" & My.Settings.layerName_ParceleNadela & "] where [idTable]=" & ddl_ttpSpisakTabli.SelectedValue.ToString
            Try
                qvrBrisi2.RunEx(True)
            Catch ex As Exception

            End Try
            doc.ComponentSet.Remove("brisiizDKP")
            qvrBrisi2 = Nothing
        Catch ex As Exception

        End Try

        'sada iz ove table kopiras parcele u DKP_Nadela!

        'zaokruzis parcele na dve decimale i odavde mozes gde hoces tako da nemas kasnije problem sa ovim povrsinama 
        'u zapisniku

        'qvrDist.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & drwnewDisolve.Name & "].[Geom (I)] as geom_,newArea_ FROM [" & drwnewDisolve.Name & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToArea( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & drwnewDisolve.Name & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & drwnewDisolve.Name & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID],1 as broj_  FROM [" & drwnewDisolve.Name & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & drwnewDisolve.Name & "].[ID]=AA.id ) set geom_=newArea_"
        'qvrDist.RunEx(True)

        'pre ovoga treba zaokruziti na dve decimale!

        'mozda moze drugacije da ne pravi ovoliku zajebanciju nego da uradi u jednom cugu!
        qvrDist.Text = "insert into [" & My.Settings.layerName_ParceleNadela & "] ([idtable],[idvlasnika],[redniBrNadele],[Geom (I)]) (SELECT [idtable],[idvlasnika],[redniBrNadele],[Geom (I)] FROM [" & drwnewDisolve.Name & "] WHERE IsArea([id]))"
        qvrDist.RunEx(True)


        'sada bi trebalo da obrise poslednjih n linija da ostavi samo poslednju!

        'napravis update tabele kom_tableNadela: sa nadeljenom povrsinom i vrednoscu(za sada ostavi ovo vrednost!?
        'ovde bi trebalo dodati i ime file-a da nebi bilo kasnije problema ! promeni u web aplikaciji takode
        Dim qvrP As Manifold.Interop.Query = doc.NewQuery("racunanjePovrsine")

        'ovde bi trebalo uvesti i vrednost i to upisati u tabelu u redu je nadeljena teorijski ali treba da stoji kolika je ona 
        'i prakticno! - ovo iziskuje da se promeni polje prilikom kreiranja tabele kom_tableNadela, i da se proveri u 
        'kojim sve SQL-ovima postoji polje pa da se zameni!
        'ovo mozda moze drugacije!

        'sada bi trbalo da se zaokruzi pa tek onda da se upise!

        qvrP.Text = "select [idtable],[idvlasnika],[rednibrnadele],round([Area (I)]) from [" & drwnewDisolve.Name & "]" : qvrP.RunEx(True)

        For i = 0 To qvrP.Table.RecordSet.Count - 1
            conn_.Open()
            myCommand.CommandText = "update kom_tableNadela set nadeljenoPovrsina= " & Math.Round(Val(qvrP.Table.RecordSet.Item(i).DataText(4)), 0) & " where idTable=" & qvrP.Table.RecordSet.Item(i).DataText(1) & " and idiskazZemljista=" & qvrP.Table.RecordSet.Item(i).DataText(2) & " and rednibrojnadele=" & qvrP.Table.RecordSet.Item(i).DataText(3)
            myCommand.ExecuteNonQuery()
            conn_.Close()
        Next

        doc.ComponentSet.Remove("racunanjePovrsine")

        '// RACUNANJE TACAKA POCINJE SADA

        'TREBA kreirati tacke iz nadele sada i proveriti koje tacke se slazu sa onim sto ima u pntzaobelezavanje i njih obrisati iz file disolve
        'treba probaci tacke u pnt nadela kojih nema po geometriji u pntobelezavanje i obrisati
        '
        analizerF_.Points(drwnewDisolve, drwnewDisolve, drwnewDisolve.ObjectSet) : analizerF_.RemoveDuplicates(drwnewDisolve, drwnewDisolve.ObjectSet)


        'sada mozes query - brises tacke koje ne pripadaju konturi table u pntoobelezavanje
        qvrDist.Text = "DELETE FROM [" & My.Settings.layerName_pointTableObelezavanje & "] WHERE [ID] in (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] FROM [" & My.Settings.layerName_pointTableObelezavanje & "],[" & drwnewDisolve.Name & "] WHERE Contains([" & drwnewDisolve.Name & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) and  [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] not in (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID] FROM (SELECT [id],CentroidX([Geom (I)]) as Y_,CentroidY([Geom (I)]) as X_ FROM [" & drwnewDisolve.Name & "] WHERE IsPoint([id])) as A INNER JOIN  [" & My.Settings.layerName_pointTableObelezavanje & "] on  round(A.Y_,2)=round(CentroidX([" & My.Settings.layerName_pointTableObelezavanje & "].[ID]),2) and round(A.X_,2)=round(CentroidY([" & My.Settings.layerName_pointTableObelezavanje & "].[ID]),2)))"
        qvrDist.RunEx(True)

        qvrDist.Text = "SELECT [" & drwnewDisolve.Name & "].[ID] FROM [" & drwNewTable.Name & "],[" & drwnewDisolve.Name & "] WHERE Contains([" & drwNewTable.Name & "].[ID],[" & drwnewDisolve.Name & "].[ID]) AND isline([" & drwNewTable.Name & "].[ID]) and IsPoint([" & drwnewDisolve.Name & "].[ID]) ORDER by [" & drwNewTable.Name & "].[ID] "
        qvrDist.RunEx(True)

        doc.Save()
        'sada ides jedan po jedan i pises brojeve tacaka recim da je pocetna 15000 ovo moras iz crteza da izvuces!
        'mora da ima po dve tacke! to je primarno!
        qvrLastID.Text = "SELECT max(cint([idTacke])) FROM [" & My.Settings.layerName_pointTableObelezavanje & "] where [tipTacke]<>1"
        qvrLastID.RunEx(True)
        Dim maxbr_ As Integer
        Try
            maxbr_ = qvrLastID.Table.RecordSet.Item(0).DataText(1) + 1
        Catch ex As Exception
            'ako prijavu gresku znaci da je ovo prva tabla koja je radena pa prema tome treba da krene od 2!
            qvrLastID.Text = "SELECT max(cint([idTacke])) FROM [" & My.Settings.layerName_pointTableObelezavanje & "] where [tipTacke]=2"
            qvrLastID.RunEx(True)
            maxbr_ = qvrLastID.Table.RecordSet.Item(0).DataText(1) + 1
        End Try

        If qvrDist.Table.RecordSet.Count = 0 Then

            qvrDist.Text = "SELECT [" & drwnewDisolve.Name & "].[ID] FROM [" & drwNewTable.Name & "],[" & drwnewDisolve.Name & "] WHERE Contains([" & drwNewTable.Name & "].[ID],[" & drwnewDisolve.Name & "].[ID]) AND isline([" & drwNewTable.Name & "].[ID]) and IsPoint([" & drwnewDisolve.Name & "].[ID]) ORDER by [" & drwNewTable.Name & "].[ID] "
            qvrDist.RunEx(True)


        End If

        For i = 0 To qvrDist.Table.RecordSet.Count - 1
            qvrLastID.Text = "INSERT INTO [" & My.Settings.layerName_pointTableObelezavanje & "] ([Geom (I)],[idTable],[idTacke],[tipTacke]) (SELECT [Geom (I)]," & ddl_ttpSpisakTabli.SelectedValue.ToString & "," & maxbr_ & ",3 FROM [" & drwnewDisolve.Name & "] where [ID]=" & qvrDist.Table.RecordSet.Item(i).DataText(1) & ")"
            qvrLastID.RunEx(True)
            maxbr_ += 1
        Next

        qvrLastID.Text = "delete from [" & drwnewDisolve.Name & "] where ispoint([Geom (I)])"
        qvrLastID.RunEx(True)

        '// RACUNANJE POVRSINA ZAVRSAVA SADA

        'sada treba napaviti skicu za obelezavanje u odnosu na ovo

        tss_label.Text = "Stampa"
        My.Application.DoEvents()

        doc.ComponentSet.Remove(name2_) : doc.ComponentSet.Remove("Table_pr_razred")
        doc.ComponentSet.Remove("LastID") : qvrLastID = Nothing
        doc.ComponentSet.Remove("Dist") : qvrDist = Nothing
        doc.ComponentSet.Remove("BrojTabli") : qvr_ = Nothing

        doc.Save()

        drwFR = Nothing
        drwNewTable = Nothing


        stampajNadelu(ddl_ttpSpisakTabli.SelectedValue, True)
        stampaObelezavanje(ddl_ttpSpisakTabli.SelectedValue, True)

        For i = 0 To gridTableNadela.Rows.Count - 1
            gridTableNadela.Rows(i).Selected = False
        Next

        FileClose()
        tss_label.Text = ""
        MsgBox("Kraj Nadele za tablu" & ddl_ttpSpisakTabli.SelectedValue)

    End Sub

    Private Sub DataVrednostToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles DataVrednostToolStripMenuItem.Click


        'kreiramo novi file i u njega kopiramo 
        'drawing sa tablama i drawing sa procembenim razredima 
        'sto prakticno znaci da mogu ovde da kreiram za sve table koje su u crtezu!

        'proveris da li crtezi postoje

        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Dim drwTableOld As Manifold.Interop.Drawing
        Try
            drwTableOld = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing table, proverite pa pokrenite proceduru ponovo.")
            Exit Sub
        End Try

        Dim drwProcenaOld As Manifold.Interop.Drawing
        Try
            drwProcenaOld = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing procene, proverite pa pokrenite proceduru ponovo.")
            Exit Sub
        End Try

        Dim drwTackeOld As Manifold.Interop.Drawing
        Try
            drwTackeOld = doc.ComponentSet(My.Settings.layerName_nadelaSmer)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing tacaka smera nadele, proverite pa pokrenite proceduru ponovo.")
            Exit Sub
        End Try

        Try
            sf_diag.FileName = "nadela_tabli_datavrednost"
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            'sf_diag.FileName = "nadela_tabla" & ddl_ttpSpisakTabli.SelectedValue & ".map"
            sf_diag.Title = "Upisite naziv za izlazni Map File - NADELA TABLI ZA KONSTANTNU VREDNOST"
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

        Dim putanjaDoIzlaza_ As String = sf_diag.FileName
        
        'podesavanje novog dokumenta
        Dim newApp As Manifold.Interop.Application = New Manifold.Interop.Application
        Dim newDoc As Manifold.Interop.Document = newApp.NewDocument("", False)

        Try
            newDoc.SaveAs(putanjaDoIzlaza_)
        Catch ex As Exception
            MsgBox("Map file je otvoren u Manifold-u. Zatvorite ga tamo pa ponovo pokrenite celu operaciju")
            Exit Sub
        End Try

        tss_label.Text = "Kreiranje topologije."
        My.Application.DoEvents()

        'sada podesis nove drawinge - kreiras

        Dim drwTableNew As Manifold.Interop.Drawing = newDoc.NewDrawing("table", drwProcenaOld.CoordinateSystem)
        Dim drwProcembeniNew As Manifold.Interop.Drawing = newDoc.NewDrawing("procembeni", drwProcenaOld.CoordinateSystem)
        Dim drwTackeNew As Manifold.Interop.Drawing = newDoc.NewDrawing("tacke", drwProcenaOld.CoordinateSystem)

        'sada mozemo da napravimo copy - paste
        Dim qvr1_ As Manifold.Interop.Query = doc.NewQuery("selektuj")

        qvr1_.Text = "update [" & My.Settings.layerName_table & "] set [Selection (I)]=true where [tiptable]=1"
        qvr1_.RunEx(True)

        drwTableOld.Copy(True) : drwTableNew.Paste(False) : drwTableOld.SelectNone() : drwTableNew.SelectNone()
        drwProcenaOld.Copy(False) : drwProcembeniNew.Paste(False) : drwProcembeniNew.SelectNone() : drwProcenaOld.SelectNone()
        drwTackeOld.Copy(False) : drwTackeNew.Paste(False) : drwTackeNew.SelectNone() : drwTackeOld.SelectNone()

        'sada mozes da se diskonektujes sa starih drawinga!
        drwProcenaOld = Nothing : drwTableOld = Nothing : drwTackeOld = Nothing : doc = Nothing
        qvr1_ = Nothing
        'sada idemo na topologiju
        Dim tbl_ As Manifold.Interop.Table
        Dim i As Integer
        pb1.Value = 1
        Try
            tbl_ = drwProcembeniNew.OwnedTable
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
            tbl_ = drwTableNew.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
        Catch ex1 As Exception
            'MsgBox(ex1.Message)
        End Try

        Dim topPRazredi As Manifold.Interop.Topology = newDoc.Application.NewTopology
        topPRazredi.Bind(drwProcembeniNew)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = newDoc.Application.NewTopology
        topParcele.Bind(drwTableNew)
        topParcele.Build()

        topParcele.DoIntersect(topPRazredi, "FiktivnaRaspodela")

        Dim topParcPrRaz As Manifold.Interop.Topology = newDoc.Application.NewTopology
        topParcPrRaz.Bind(newDoc.ComponentSet.Item(newDoc.ComponentSet.ItemByName("FiktivnaRaspodela")))
        topParcPrRaz.Build()

        topPRazredi = Nothing : topParcPrRaz = Nothing : topParcele = Nothing

        'sada mozemo dalje a dalje je sledece

        Dim korak_ As Integer = InputBox("Upisite korak deobe", "Unos podataka", "4000")

        Dim drwFR As Manifold.Interop.Drawing = newDoc.ComponentSet.Item("FiktivnaRaspodela")
        'sada bi trebalo svaku tablu da delis na isti segment! kako 
        Dim qvr_ As Manifold.Interop.Query = newDoc.NewQuery("selektuj")

        'kako podesiti style za povrsinu
        qvr_.Text = "select [FiktivnaRaspodela].[idTable],sum([FiktivnaRaspodela].[Area (I)]*[Faktor]) as vrednost from [FiktivnaRaspodela] group by [idTable] order by [idTable] asc"
        qvr_.RunEx(True)
        Dim brojacNer_ As Integer = 0
        pb1.Maximum = qvr_.Table.RecordSet.Count

        tss_label.Text = "Kreiranje tabli sa procenom za svaku tablu."
        My.Application.DoEvents()
        Dim tmp As Manifold.Interop.Drawing

        Dim qvrCopy As Manifold.Interop.Query = newDoc.NewQuery("kopirajTablu")
        Dim col_ As Manifold.Interop.Column = newDoc.Application.NewColumnSet.NewColumn
        'za svaku tablu u crtezu kreiras novu tablu
        For i = 0 To qvr_.Table.RecordSet.Count - 1
            tmp = newDoc.NewDrawing("tabla_" & qvr_.Table.RecordSet.Item(i).DataText(1), drwTableNew.CoordinateSystem, True)
            'sada sve copiras i njega
            qvrCopy.Text = "update (select * from [FiktivnaRaspodela] where [idTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & ") set [Selection (I)]=True"
            qvrCopy.RunEx(True)
            drwFR.Copy(True)
            tmp.Paste(True)
            My.Computer.Clipboard.Clear() 'ocisti clipboard ali kako?
            qvrCopy.Text = "update [FiktivnaRaspodela] set [Selection (I)]=false"
            qvrCopy.RunEx(True)
            col_.Name = "zaLabel"
            col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
            tmp.OwnedTable.ColumnSet.Add(col_)
            tmp.AreaBackground.Set(newApp.NewColor("", 255, 255, 255, 0))
            pb1.Value = i
        Next

        newDoc.Save()
        ''sad kreiras polje

        'kreiras map u koji ces sve kreirano da smestis!
        Dim map_ As Manifold.Interop.Map = newDoc.NewMap("SveTable", drwTableNew, drwTableNew.CoordinateSystem, True)
        Dim layout_ As Manifold.Interop.Layout = newDoc.NewLayout("Nadela zadata vrednost Stampa", map_)

        Dim freefile_ As Integer = FreeFile() : FileOpen(freefile_, Path.GetTempPath() & "\izvesta_tableAuto.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared) : pb1.Maximum = qvr_.Table.RecordSet.Count

        Dim j As Integer
        tss_label.Text = "Obrada tabli."
        My.Application.DoEvents()

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            PrintLine(freefile_, "poceo tablu " & qvr_.Table.RecordSet.Item(i).DataText(1) & " vreme: " & Now())
            PrintLine(freefile_, vbCrLf)
            pb1.Value = i

            tss_label.Text = "Obrada table " & qvr_.Table.RecordSet.Item(i).DataText(1)
            My.Application.DoEvents()

            System.Threading.Thread.Sleep(100)

            tmp = newDoc.ComponentSet("tabla_" & qvr_.Table.RecordSet.Item(i).DataText(1))
            Dim qvrDist As Manifold.Interop.Query = newDoc.NewQuery("dist")
            qvrDist.Text = "SELECT top 1 distance(A.[geom (I)],B.[Geom (I)]) as dist_,atn2(CentroidX(A.[Geom (I)])-CentroidX(B.[Geom (I)]),CentroidY(A.[Geom (I)])-CentroidY(B.[Geom (I)])) as ugao_,CentroidX(A.[Geom (I)]) as x1,CentroidY(A.[Geom (I)]) as y1,CentroidX(B.[Geom (I)]) as x2,CentroidY(B.[Geom (I)]) as y2 FROM ((SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & ") as A,(SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & qvr_.Table.RecordSet.Item(i).DataText(1) & ") as B) WHERE A.[ID]<>B.[ID]"
            qvrDist.RunEx(True)

            pb2.Maximum = qvr_.Table.RecordSet.Item(i).DataText(2) + 1

            For j = korak_ To qvr_.Table.RecordSet.Item(i).DataText(2) Step korak_ 'ovde zamnei sa necim na ulazu!
                Dim kraj As Boolean = False
                'treba ti rastojanje izmedu dve tacke
                pb2.Value = j
                System.Threading.Thread.Sleep(100)
                Dim h_ As Double = H_racunanjeDirektno(qvrDist.Table.RecordSet(0).DataText(1), qvrDist.Table.RecordSet(0).DataText(1), qvr_.Table.RecordSet.Item(i).DataText(2), j)
                'sada treba videti dali je to ok!
                Dim interacija_ As Integer = 0

                PrintLine(freefile_, "korak=" & j) : PrintLine(freefile_, vbCrLf)

                Do While Not kraj = True
                    PrintLine(freefile_, "interacija=" & interacija_ & " poceo: " & Now())

                    Dim drwTemp As Manifold.Interop.Drawing = newDoc.NewDrawing("temp", drwTableNew.CoordinateSystem, True)

                    'aj da probamo ovo preko query-ja!

                    Try
                        tmp.Copy() : drwTemp.Paste(False) 'ocisti clipboard ali kako?
                    Catch ex As Exception
                    End Try


                    Try
                        My.Computer.Clipboard.Clear()
                    Catch ex As Exception
                    End Try

                    Dim drwLine As Manifold.Interop.Drawing = newDoc.NewDrawing("linije", drwTableNew.CoordinateSystem, True)
                    Dim pnt1(1), pnt2(1) As Double
                    pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
                    pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                    Dim du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
                    podeliParceluViseDelova_KreirajPresecnuLiniju(du - 90, qvrDist.Table.RecordSet(0).DataText(1), h_, pnt1, pnt2, newDoc, drwLine, My.Settings.nadela_duzina)
                    Dim lineID As Integer = drwLine.ObjectSet.Item(0).ID
                    'sada imas liniju i ide presek
                    'doc.Save()
                    Dim analizer_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
                    analizer_.Split(drwTemp, drwTemp, drwTemp.ObjectSet, drwLine.ObjectSet)
                    'doc.Save()
                    'problem kako da selektujes poligone koji su nastali naknadno? izmedu linije i dve tacke?
                    podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -h_, pnt1, pnt2, newDoc, drwLine, My.Settings.nadela_duzina)
                    'sada ti treba convechull da napravi poligon

                    Dim qvrDobijenaPov As Manifold.Interop.Query
                    Try
                        qvrDobijenaPov = newDoc.ComponentSet.Item("koliko")
                    Catch ex As Exception
                        qvrDobijenaPov = newDoc.NewQuery("koliko")
                    End Try

                    qvrDobijenaPov.Text = "insert into [linije] ([Geom (I)]) VALUES (SELECT ConvexHull(AllCoords([Geom (I)])) FROM [Linije])"
                    qvrDobijenaPov.RunEx(True)
                    qvrDobijenaPov.Text = "SELECT sum([temp].[Area (I)]*[temp].[Faktor]) FROM [Temp],[Linije] WHERE IsArea([Temp].[ID]) and  Contains([Linije].[ID],[Temp].[ID])"
                    qvrDobijenaPov.RunEx(True)
                    'SADA treba skratiti celu pricu!
                    'doc.Save()
                    If qvrDobijenaPov.Table.RecordSet(0).DataText(1) = "0" Then
                        MsgBox("proveri copy - copy u polju faktor!")
                    End If

                    If interacija_ > My.Settings.nadela_brInteracija Then
                        PrintLine(freefile_, "ovde sam ga prekunuo jer je iz nekog razloga dosao do " & My.Settings.nadela_brInteracija & " interacije")
                        drwLine = Nothing : drwTemp = Nothing
                        newDoc.ComponentSet.Remove("temp") : newDoc.ComponentSet.Remove("linije")
                        Exit For
                    End If
                    tss_label.Text = "Tabla: " & qvr_.Table.RecordSet.Item(i).DataText(1) & " njena vrednost:" & qvr_.Table.RecordSet.Item(i).DataText(2) & " Interacija: " & j & " razlika=" & (Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - j, 2))
                    My.Application.DoEvents()
                    If Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)), 2) = j Then
                        PrintLine(freefile_, "razlika: " & Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)), 2))
                        'pronasao! izlazim iz ovoga kreiras liniju u temp3
                        tmp.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                        newDoc.ComponentSet.Remove("temp") : newDoc.ComponentSet.Remove("linije")
                        kraj = True
                    Else
                        'idemo iz pocetka
                        h_ = h_ + ((j - Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))) / qvrDist.Table.RecordSet(0).DataText(1))
                        newDoc.ComponentSet.Remove("temp") : newDoc.ComponentSet.Remove("linije")
                        drwLine = Nothing : drwTemp = Nothing
                    End If
                    newDoc.ComponentSet.Remove("koliko")
                    qvrDobijenaPov = Nothing
                    PrintLine(freefile_, "interacija=" & interacija_ & " kraj: " & Now())
                    interacija_ += 1
                    drwTemp = Nothing : drwLine = Nothing : analizer_ = Nothing
                    Try
                        My.Computer.Clipboard.Clear()
                    Catch ex As Exception

                    End Try

                Loop

                'drwNewTable = Nothing
                Dim brojanje As Manifold.Interop.Query = newDoc.NewQuery("broj")
                Dim ostalo_ As Manifold.Interop.Query = newDoc.NewQuery("punjenjeLabela")
                brojanje.Text = "SELECT top 1 [ID] FROM [" & tmp.Name & "] order by [ID] DESC"
                brojanje.RunEx(True)
                'sada dodajes linije koje ti trebaju
                ostalo_.Text = "options coordsys(" & Chr(34) & tmp.Name & Chr(34) & " as Component); INSERT INTO [" & tmp.Name & "] ([Geom (I)])  (SELECT CGeom(ClipIntersect(F,G)) FROM ((SELECT G FROM (SELECT  UnionAll([" & tmp.Name & "].[Geom (I)]) as G FROM [" & tmp.Name & "] WHERE IsArea([ID]) )) as A, (SELECT [Geom (I)] as F FROM [" & tmp.Name & "] WHERE IsLine([id])) as B))"
                ostalo_.RunEx(True)
                'sada je sve upisano 
                'ostaje da obrises ostatak linija!
                Dim poslednji_ As Integer = -1
                poslednji_ = brojanje.Table.RecordSet.Item(0).DataText(1)
                'proveris sta je upisao u poslednji
                ostalo_.Text = "delete from [" & tmp.Name & "] where [ID]<=" & poslednji_ & " and isLine([ID])"
                ostalo_.RunEx(True)

                ostalo_ = Nothing
                brojanje = Nothing
                newDoc.ComponentSet.Remove("broj")
                newDoc.ComponentSet.Remove("punjenjeLabela")
                brojanje = Nothing
              
            Next

            newDoc.Save()
            qvrDist.Text = "select [ID] from [" & tmp.Name & "] where isLine([ID]) order by [ID] ASC"
            qvrDist.RunEx(True)

            Dim updatemat(-1) As String : Dim brojac_ As Integer = -1

            For k = 0 To qvrDist.Table.RecordSet.Count - 1
                ReDim Preserve updatemat(k)
                updatemat(k) = "update [" & tmp.Name & "] set [zaLabel]=" & Chr(34) & (korak_ * (k + 1)) & Chr(34) & " where [ID]=" & qvrDist.Table.RecordSet.Item(k).DataText(1)
            Next

            For k = 0 To updatemat.Length - 1
                qvrDist.Text = updatemat(k)
                qvrDist.RunEx(True)
            Next

            qvrDist.Text = "insert into [" & tmp.Name & "] ([Geom (I)]) (select endPoint([ID]) from [" & tmp.Name & "] where isLine([ID]) and IsPoint(endPoint([ID])))"
            qvrDist.RunEx(True)
            qvrDist.Text = "update (SELECT A.in_,A.line_,B.out_,B.pnt_ FROM ((SELECT [" & tmp.Name & "].[zaLabel] as in_,[Geom (I)] as line_  FROM [" & tmp.Name & "] WHERE isline([ID])) as A, (SELECT [" & tmp.Name & "].[zaLabel] as out_, [Geom (I)] as pnt_ FROM [" & tmp.Name & "] WHERE IsPoint([id])) as b ) WHERE Touches(A.line_,B.pnt_) ) set out_=in_"
            qvrDist.RunEx(True)
            qvrDist.Text = "update [" & tmp.Name & "] set [zaLabel]=" & Chr(34) & Chr(34) & " where isline([ID])"
            qvrDist.RunEx(True)

            Dim analizer2_ As Manifold.Interop.Analyzer = newDoc.NewAnalyzer
            analizer2_.Union(tmp, tmp, tmp.ObjectSet)
            analizer2_ = Nothing

            'sada mozes da kreiras label za ovaj crtez
            Dim labelLinije_ As Manifold.Interop.Labels = newDoc.NewLabels((tmp.Name & "_vrednost"), tmp, True, True)
            labelLinije_.Text = "[zaLabel]"
            labelLinije_.LeftToRight = False : labelLinije_.MultipleLabelsPerBranch = False
            labelLinije_.OptimizeLabelAlignX = False : labelLinije_.OptimizeLabelAlignY = False
            labelLinije_.ResolveOverlaps = False : labelLinije_.LabelEachBranch = False
            labelLinije_.PerLabelFormat = True : labelLinije_.LabelAlignX = Manifold.Interop.LabelAlignX.LabelAlignXLeft
            labelLinije_.LabelAlignY = Manifold.Interop.LabelAlignY.LabelAlignYTop
            labelLinije_.LineOffset = 3

            Dim lbl_ As Manifold.Interop.Labels = newDoc.ComponentSet((tmp.Name & "_vrednost"))

            Dim labelSet_ As Manifold.Interop.LabelSet = lbl_.LabelSet

            qvrDist.Text = "SELECT CentroidX([Geom (I)]) as X,CentroidY([Geom (I)]) as Y FROM [" & My.Settings.layerName_nadelaSmer & "] WHERE [idtable]=" & qvr_.Table.RecordSet.Item(i).DataText(1)
            qvrDist.RunEx(True)
            Dim P_ = NiAnaB(qvrDist.Table.RecordSet.Item(0).DataText(1), qvrDist.Table.RecordSet.Item(0).DataText(2), qvrDist.Table.RecordSet.Item(1).DataText(1), qvrDist.Table.RecordSet.Item(1).DataText(2))

            For Each lab_ In labelSet_
                lab_.rotation = P_ - 90
                lab_.size = 8
            Next

            Dim pLayer As Manifold.Interop.Layer = newDoc.NewLayer(tmp)
            pLayer.Visible = False
            map_.LayerSet.Add(pLayer)
            pLayer = newDoc.NewLayer(labelLinije_)
            pLayer.Visible = False
            map_.LayerSet.Add(pLayer)

            labelLinije_ = Nothing
            newDoc.ComponentSet.Remove("dist")
            qvrDist = Nothing
            newDoc.Save()
        Next

        'sada bi trebalo kreirati jedan map i layout pa da imas kompletnu procu

        drwFR = Nothing
        newDoc.ComponentSet.Remove("KopirajTablu") : newDoc.ComponentSet.Remove("Selektuj")
        qvr_ = Nothing
        'MsgBox("kraj")
        tss_label.Text = ""
        pb1.Value = 0 : pb2.Value = 0
        MsgBox("Kraj")
    End Sub

    Private Sub IzFileaDirektnoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles IzFileaDirektnoToolStripMenuItem.Click

        'prvo da proverimo da li postoje drawing-zi

        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        'provera postojanja drawing-a

        Dim drw_table As Manifold.Interop.Drawing

        Try
            drw_table = doc.ComponentSet.Item(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Proverite podesanje drawinga za table, pa pokretnite proceduru ponovo.")
            Exit Sub
        End Try

        Dim drw_procRazredi As Manifold.Interop.Drawing
        Try
            drw_procRazredi = doc.ComponentSet.Item(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Proverite podesanje drawinga za procenom, pa pokretnite proceduru ponovo.")
            Exit Sub
        End Try

        'prvo proveris dali u gridu ima nesto
        Dim sumaVrednosti As Double = 0 : Dim matVlasnika(-1) As String : Dim matVrednosti(-1) As Double


        opf_diag.FileName = ""
        opf_diag.DefaultExt = "csv"
        opf_diag.Filter = "CSV file (*.csv)|*.csv|Txt file (*.txt)|*.txt|All files (*.*)|*.*"
        opf_diag.RestoreDirectory = True
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        Dim brojac_ As Integer = 0
        Dim freefileIN_ As Integer = FreeFile()
        FileOpen(freefileIN_, opf_diag.FileName, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)

        Do While Not EOF(freefileIN_)
            Dim a_ = LineInput(freefileIN_)
            Dim b_ = Split(a_, ",")
            If b_.Length = 2 And b_(0) <> "" And Val(b_(1)) > 0 Then
                ReDim Preserve matVlasnika(brojac_)
                ReDim Preserve matVrednosti(brojac_)
                matVlasnika(brojac_) = b_(0)
                matVrednosti(brojac_) = b_(1)
                sumaVrednosti += b_(1)
                brojac_ += 1
            End If
        Loop

        'conn_ = Nothing

        Dim brTable_ As Integer
        Try
            brTable_ = InputBox("Upisite broj table?", "Unos table", ddl_ttpSpisakTabli.SelectedValue)
        Catch ex As Exception
            brTable_ = InputBox("Upisite broj table?", "Unos table", "1")
        End Try


        'napravis prvo presek procembenih razreda i tabli
        Dim tbl_ As Manifold.Interop.Table
        Try
            Dim drw As Manifold.Interop.Drawing = doc.ComponentSet("table_pr_razred")
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception

        End Try

        'podesis da je samo ova tabla status1


        Dim freefile_ As Integer = FreeFile()
        FileOpen(freefile_, Path.GetTempPath() & "\nadela_table_" & brTable_ & ".txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared) 

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
            tbl_ = drw_table.OwnedTable
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
        topPRazredi.Bind(drw_procRazredi)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drw_table)
        topParcele.Build()

        topParcele.DoIntersect(topPRazredi, "table_pr_razred")

        Dim topParcPrRaz As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcPrRaz.Bind(doc.ComponentSet.Item(doc.ComponentSet.ItemByName("table_pr_razred")))
        topParcPrRaz.Build()

        topPRazredi = Nothing : topParcPrRaz = Nothing : topParcele = Nothing

        'doc.Save()

        Dim drwFR As Manifold.Interop.Drawing = doc.ComponentSet.Item("table_pr_razred")

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("brojTabli")
        qvr_.Text = "select sum([table_pr_razred].[Area (I)]*[Faktor]) as vrednost from [table_pr_razred] WHERE [idTable]=" & brTable_
        qvr_.RunEx(True)

        If qvr_.Table.RecordSet(0).DataText(1) = "" Then
            MsgBox("Nesto nije u redu sa podesavanjima - ili drawing ili baza komasacije - proverite jos jednom.")
            Exit Sub
        End If

        doc.Save()

        MsgBox("Suma ulaznih vrednosti je: " & sumaVrednosti & ", a ukupna vrednost za nadelu u tabli je: " & qvr_.Table.RecordSet(0).DataText(1))

        Dim brojacNer_ As Integer = 0

        'doc.Save()
        Dim drwNewTable As Manifold.Interop.Drawing
        Try
            drwNewTable = doc.NewDrawing("tablaPR_" & brTable_, drw_table.CoordinateSystem, True)
        Catch ex As Exception
            'znaci da postoji 
            doc.ComponentSet.Remove("tablaPR_" & brTable_)
            drwNewTable = doc.NewDrawing("tablaPR_" & brTable_, drw_table.CoordinateSystem, True)
        End Try

        'sada sve copiras i njega
        Dim qvrCopy As Manifold.Interop.Query = doc.NewQuery("kopirajTablu")

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "OldID"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idVlasnika"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "redniBrNadele"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idTable"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "tipTable"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "procembeni"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "faktor"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_ = Nothing


        qvrCopy.Text = "insert into [" & drwNewTable.Name & "] ([idtable],[procembeni],[faktor],[Geom (I)]) (select [idtable],[procembeni],[faktor],[Geom (I)] FROM [table_pr_razred] where [idTable]=" & brTable_ & ")"
        qvrCopy.RunEx(True)

        doc.ComponentSet.Remove("kopirajTablu")
        qvrCopy = Nothing
        'sada mozes odmah deobu a mozes i u sledecem krugu

        'doc.Save()

        Dim qvrDist As Manifold.Interop.Query = doc.NewQuery("dist")
        qvrDist.Text = "SELECT top 1 distance(A.[geom (I)],B.[Geom (I)]) as dist_,atn2(CentroidX(A.[Geom (I)])-CentroidX(B.[Geom (I)]),CentroidY(A.[Geom (I)])-CentroidY(B.[Geom (I)])) as ugao_,CentroidX(A.[Geom (I)]) as x1,CentroidY(A.[Geom (I)]) as y1,CentroidX(B.[Geom (I)]) as x2,CentroidY(B.[Geom (I)]) as y2 FROM ((SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & brTable_ & ") as A,(SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & brTable_ & ") as B) WHERE A.[ID]<>B.[ID]" : qvrDist.RunEx(True)
        Dim qvrLastID As Manifold.Interop.Query = doc.NewQuery("lastID")
        Dim analizerF_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        If qvrDist.Table.RecordSet.Count > 0 Then


            Dim sumazaDeob_ As Double
            pb1.Maximum = UBound(matVlasnika)
            pb1.Value = 0
            'insertujes nultu liniju!
            Dim pnt1(1), pnt2(1) As Double
            pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
            pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
            Dim du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
            podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -10, pnt1, pnt2, doc, drwNewTable, 2)

            Dim matLinija() As Integer : ReDim Preserve matLinija(0)


            qvrLastID.Text = "select top 1 [ID] from [" & drwNewTable.Name & "] order by [ID] Desc" : qvrLastID.RunEx(True)

            matLinija(0) = qvrLastID.Table.RecordSet(0).DataText(1)

            'doc.Save()

            PrintLine(freefile_, "Poceo sa obradom table u " & Now())

            For j = 0 To UBound(matVlasnika)  'ovde zamnei sa necim na ulazu!
                pb1.Value = j
                Dim kraj As Boolean = False
                'treba ti rastojanje izmedu dve tacke
                sumazaDeob_ += matVrednosti(j)
                Dim h_ As Double = H_racunanjeDirektno(qvrDist.Table.RecordSet(0).DataText(1), qvrDist.Table.RecordSet(0).DataText(1), qvr_.Table.RecordSet.Item(0).DataText(1), sumazaDeob_)
                'sada treba videti dali je to ok!
                Dim interacija_ As Integer = -1
                pb2.Maximum = My.Settings.nadela_brInteracija

                Do While Not kraj = True
                    interacija_ += 1
                    Try
                        pb2.Value = interacija_
                    Catch ex As Exception

                    End Try

                    PrintLine(freefile_, "Poceo interaciju " & interacija_)

                    Dim drwTemp As Manifold.Interop.Drawing
                    Try
                        drwTemp = doc.NewDrawing("temp", drwNewTable.CoordinateSystem, True)
                    Catch ex As Exception
                        doc.ComponentSet.Remove("temp")
                        drwTemp = doc.NewDrawing("temp", drwNewTable.CoordinateSystem, True)
                    End Try

                    col_ = doc.Application.NewColumnSet.NewColumn : col_.Name = "OldID" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idVlasnika" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : col_.Name = "redniBrNadele" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idTable" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "tipTable" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "procembeni" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "faktor" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_ = Nothing

                    Dim q_ As Manifold.Interop.Query = doc.NewQuery("dfas")
                    q_.Text = "insert into [temp] (OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)]) (select OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)] from [" & drwNewTable.Name & "])"
                    q_.RunEx(True)
                    doc.ComponentSet.Remove("dfas")

                    Dim drwLine As Manifold.Interop.Drawing

                    Try
                        drwLine = doc.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                    Catch ex As Exception
                        doc.ComponentSet.Remove("linije")
                        drwLine = doc.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                    End Try

                    'Dim pnt1(1), pnt2(1) As Double
                    pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
                    pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                    du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
                    podeliParceluViseDelova_KreirajPresecnuLiniju(du - 90, qvrDist.Table.RecordSet(0).DataText(1), h_, pnt1, pnt2, doc, drwLine)
                    Dim lineID As Integer = drwLine.ObjectSet.Item(0).ID
                    'sada imas liniju i ide presek

                    Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
                    analizer_.Split(drwTemp, drwTemp, drwTemp.ObjectSet, drwLine.ObjectSet)
                    'doc.Save()
                    analizer_ = Nothing
                    'problem kako da selektujes poligone koji su nastali naknadno? izmedu linije i dve tacke?
                    podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -h_, pnt1, pnt2, doc, drwLine, 2)
                    'doc.Save()
                    'sada ti treba convechull da napravi poligon
                    Dim qvrDobijenaPov As Manifold.Interop.Query = doc.NewQuery("koliko")
                    qvrDobijenaPov.Text = "insert into [linije] ([Geom (I)]) VALUES (SELECT ConvexHull(AllCoords([Geom (I)])) FROM [Linije])"
                    qvrDobijenaPov.RunEx(True)
                    'doc.Save()
                    'sada radis update faktora! za svaki slucaj! ovo bi trebalo da napravis i gore
                    qvrDobijenaPov.Text = "update (SELECT [temp].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([Temp], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [Temp].[ID])) set out_ = in_"
                    qvrDobijenaPov.RunEx(True)
                    'doc.Save()
                    qvrDobijenaPov.Text = "SELECT sum([temp].[Area (I)]*[temp].[Faktor]) FROM [Temp],[Linije] WHERE IsArea([Temp].[ID]) and  Contains([Linije].[ID],[Temp].[ID])"
                    qvrDobijenaPov.RunEx(True)
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
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        kraj = True
                        Exit Do
                    End If

                    tss_label.Text = "Deoba za iskaz " & matVlasnika(j) & " razlika> " & (Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - sumazaDeob_, 2))
                    My.Application.DoEvents()

                    Dim nesto_ As Double = Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))

                    tss_label.Text = "Obrada iskaza " & matVlasnika(j) & ", razlika: " & (Math.Round(nesto_, 2) - Math.Round(sumazaDeob_, 2)) : My.Application.DoEvents()
                    My.Application.DoEvents()

                    If Math.Round(nesto_, 2) = Math.Round(sumazaDeob_, 2) Then
                        'pronasao! izlazim iz ovoga kreiras liniju u temp3
                        drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                        qvrLastID.RunEx(True)
                        ReDim Preserve matLinija(j + 1)
                        matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        kraj = True
                    Else
                        'idemo iz pocetka
                        h_ = h_ + ((sumazaDeob_ - Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))) / qvrDist.Table.RecordSet(0).DataText(1))
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        drwLine = Nothing : drwTemp = Nothing
                        PrintLine(freefile_, "kraj interacije " & Now())
                        'interacija_ += 1
                    End If
                    doc.ComponentSet.Remove("koliko")
                    qvrDobijenaPov = Nothing
                Loop

            Next

            PrintLine(freefile_, "Kraj " & Now())
            pb1.Value = 0 : pb2.Value = 0

            '// OVDE UBACIO SVE STO TREBA IZ PRETHODNOG

            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [OldID]=[ID]" : qvrLastID.RunEx(True)
            'doc.Save()

            analizerF_.Split(drwNewTable, drwNewTable, drwNewTable.ObjectSet, drwNewTable.ObjectSet)

            'doc.Save()

            For i = 0 To matLinija.Length - 2
                qvrLastID.Text = "update (SELECT [" & My.Settings.parcele_fieldName_Vlasnik & "],[redniBrNadele] from [" & drwNewTable.Name & "] WHERE Contains((SELECT ConvexHull(AllCoords([Geom (I)])) FROM [" & drwNewTable.Name & "] WHERE [OldID]=" & matLinija(i) & " or [OldID]=" & matLinija(i + 1) & "),[ID]) AND IsArea([ID])) set " & My.Settings.parcele_fieldName_Vlasnik & "=" & Chr(34) & matVlasnika(i) & Chr(34) & ", redniBrNadele=" & i + 1
                qvrLastID.RunEx(True)
            Next

            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & Chr(34) & matVlasnika(matVlasnika.Length - 1) & Chr(34) & ", [redniBrNadele]=" & matVrednosti.Length - 1 & " where [idVlasnika]=" & Chr(34) & "0" & Chr(34) & " and [redniBrNadele]=0 and IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

        Else

            MsgBox("Proverite da li ste upisali dobar broj table u drawingu tacke!!!!")
            'sada treba da update napravis ovog jednog kojeg imasa!
            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & matVlasnika(matVlasnika.Length - 1) & ", [redniBrNadele]=1 where IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

        End If

        Dim drwnewDisolve As Manifold.Interop.Drawing
        Try
            drwnewDisolve = doc.NewDrawing(drwNewTable.Name & "_dissolve", drwNewTable.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove(drwNewTable.Name & "_dissolve")
            drwnewDisolve = doc.NewDrawing(drwNewTable.Name & "_dissolve", drwNewTable.CoordinateSystem, True)
        End Try

        'sada kreiras polje za pocetak rednibrnadele i idtable i idvlasnika
        tbl_ = drwnewDisolve.OwnedTable
        col_ = doc.Application.NewColumnSet.NewColumn
        col_.Name = "redniBrNadele"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idtable"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idvlasnika"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_.Name = "povrsina"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        'sada jedino merge na osnovu selekcije koja ide izmedu linija jer drugacije nece da radi - na primer disolve 
        'ili recimo redni broj nadele? - kako ovo da dobijes? isto 

        tbl_ = Nothing : col_ = Nothing

        'sada zaokrizis to na dve decimale pa kopiras i racunas povrisnu
        'doc.Save()

        qvrDist.Text = "INSERT INTO [" & drwnewDisolve.Name & "] ([Geom (I)],[redniBrNadele]) (SELECT * FROM (SELECT UnionAll([ID]) as pera_,[redniBrNadele] FROM [" & drwNewTable.Name & "] GROUP BY [redniBrNadele] ) where pera_ IS NOT NULL )"
        qvrDist.RunEx(True)

        qvrDist.Text = "update (SELECT [" & drwNewTable.Name & "].[idVlasnika] as in_,[" & drwnewDisolve.Name & "].[idVlasnika] as out_ FROM [" & drwNewTable.Name & "],[" & drwnewDisolve.Name & "] WHERE [" & drwNewTable.Name & "].[redniBrNadele]=[" & drwnewDisolve.Name & "].[redniBrNadele] ) set out_=in_ "
        qvrDist.RunEx(True)
        qvrDist.Text = "update [" & drwnewDisolve.Name & "] set [idTable]=" & brTable_
        qvrDist.RunEx(True)
        'sada ostaje da napravis update za idtable i vlasnika!
        'drwnewDisolve.SelectNone()

        analizerF_.NormalizeTopology(drwnewDisolve, drwnewDisolve.ObjectSet)

        analizerF_ = Nothing

        qvrDist.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_ParceleNadela & Chr(34) & " as COMPONENT); UPDATE (SELECT [" & drwnewDisolve.Name & "].[Geom (I)] as geom_,newArea_ FROM [" & drwnewDisolve.Name & "], (SELECT AllBranches(forArea_) as newArea_ ,id from (SELECT ConvertToArea( AllCoords(pnt1)) as forArea_,id,rbr FROM (SELECT AssignCoordSys( NewPoint(round(centroidx(pnt_),2),round(centroidy(pnt_),2)), COORDSYS(" & Chr(34) & drwnewDisolve.Name & Chr(34) & " as COMPONENT)) as pnt1,id,rbr FROM (SELECT t1.brnc_,t1.id,count(t2.brnc_) as rbr FROM ((SELECT brnc_, [ID],1 as broj_ FROM [" & drwnewDisolve.Name & "] SPLIT by Branches([Geom (I)]) as brnc_) as T1 LEFT JOIN (SELECT brnc_, [ID],1 as broj_  FROM [" & drwnewDisolve.Name & "] SPLIT by Branches([Geom (I)]) as brnc_) as T2 on T1.[id]=T2.[id] and T1.brnc_>T2.brnc_ ) GROUP by t1.id,t1.brnc_ ) SPLIT by Coords(brnc_) as pnt_ ) GROUP by id,rbr ) GROUP by id ) as AA WHERE [" & drwnewDisolve.Name & "].[ID]=AA.id ) set geom_=newArea_"
        qvrDist.RunEx(True)

        doc.ComponentSet.Remove("lastID") : qvrLastID = Nothing : drwNewTable = Nothing
        doc.ComponentSet.Remove("dist") : qvrDist = Nothing : drwFR = Nothing
        doc.ComponentSet.Remove("brojTabli") : qvr_ = Nothing
        doc.Save()
        Close()
        MsgBox("kraj")


    End Sub

    Private Sub ProveraPovrsinaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ProveraPovrsinaToolStripMenuItem.Click
        'On Error Resume Next
        'proverava povrsine u bazi i manifoldu!
        'treba mu putanja do baza ali to je u stvari otvoren dokument
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        doc = frmMain.ManifoldCtrl.get_Document
        tss_label.Text = "Priprema potrebnih drawing-a" : My.Application.DoEvents()

        Dim drwDKPNadela As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("pojedinacnoPovrsina")

        Dim freeFile_ As Integer = FreeFile()
        ' Try
        FileOpen(freeFile_, Path.GetTempPath() & "\poredenje.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        'Catch ex As Exception
        'MsgBox("zatvorite c:\poredenje.txt i pokrenite ponovo proceduru")
        ' Exit Sub
        ' End Try
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim mycommand As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        'aj ovo malo drugacije:
        'ti treba da napravis update pa treba!

        Dim qvrStart As Manifold.Interop.Query = doc.NewQuery("poetak")
        qvrStart.Text = "SELECT [idtable],[redniBrNadele],[idVlasnika],[Area (I)] FROM [" & My.Settings.layerName_ParceleNadela & "] ORDER BY [idtable],[redniBrNadele]"
        qvrStart.RunEx(True)
        pb2.Maximum = qvrStart.Table.RecordSet.Count
        For i = 0 To qvrStart.Table.RecordSet.Count - 1
            'sada za svaki proveris ili jednostavno upises?
            mycommand.CommandText = "select nadeljenoPovrsina from kom_tablenadela where idtable=" & qvrStart.Table.RecordSet.Item(i).DataText(1) & " and rednibrojnadele=" & qvrStart.Table.RecordSet.Item(i).DataText(2) & " and idiskazzemljista=" & qvrStart.Table.RecordSet.Item(i).DataText(3)
            Dim myReader As MySqlDataReader
            Try
                myReader = mycommand.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                conn_.Open()
                myReader = mycommand.ExecuteReader(CommandBehavior.CloseConnection)
            End Try

            'proveravas dali ima !
            If myReader.HasRows Then
                myReader.Read()
                Dim povrsina_ As Double = myReader.GetValue(0)
                myReader.Close()
                If Math.Round(povrsina_, 2) <> Math.Round(Val(qvrStart.Table.RecordSet.Item(i).DataText(4)), 2) Then
                    'sada ga upisujes!
                    mycommand.CommandText = "update kom_tableNadela set nadeljenoPovrsina=" & qvrStart.Table.RecordSet.Item(i).DataText(4) & " where idtable=" & qvrStart.Table.RecordSet.Item(i).DataText(1) & " and rednibrojnadele=" & qvrStart.Table.RecordSet.Item(i).DataText(2) & " and idiskazzemljista=" & qvrStart.Table.RecordSet.Item(i).DataText(3)
                    Try
                        mycommand.ExecuteNonQuery()
                    Catch ex As Exception
                        conn_.Open()
                        mycommand.ExecuteNonQuery()
                    End Try
                    'sada upisujes da je promenjen
                    PrintLine(freeFile_, "Promenjen rekord :  idtable=" & qvrStart.Table.RecordSet.Item(i).DataText(1) & " and rednibrojnadele=" & qvrStart.Table.RecordSet.Item(i).DataText(2) & " and idiskazzemljista=" & qvrStart.Table.RecordSet.Item(i).DataText(3))
                End If
            Else
                'ovaj record ne postoji u bazi sa ovakvim elementima
                myReader.Close()
                PrintLine(freeFile_, "nema ovog rekorda u bazi: " & qvrStart.Table.RecordSet.Item(i).DataText(1) & "," & qvrStart.Table.RecordSet.Item(i).DataText(2) & "," & qvrStart.Table.RecordSet.Item(i).DataText(3))
            End If
            pb2.Value = i

        Next

        mycommand = Nothing
        Try
            conn_.Close()
        Catch ex As Exception

        End Try

        conn_ = Nothing
        FileClose()

        MsgBox("Kraj")
    End Sub

    Private Sub NadelaIObelezavanjeTableToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles NadelaIObelezavanjeTableToolStripMenuItem.Click
        stampajNadelu(ddl_ttpSpisakTabli.SelectedValue, True)
        stampaObelezavanje(ddl_ttpSpisakTabli.SelectedValue, True)
    End Sub

    Private Sub NadelaToolStripMenuItem1_Click(sender As Object, e As System.EventArgs) Handles NadelaToolStripMenuItem1.Click
        stampajNadelu(ddl_ttpSpisakTabli.SelectedValue, True)
    End Sub

    Private Sub ObelezavanjeToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ObelezavanjeToolStripMenuItem.Click
        stampaObelezavanje(ddl_ttpSpisakTabli.SelectedValue, True)
    End Sub

    Private Sub OdrediVrednostPoligonaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles OdrediVrednostPoligonaToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        'pretpostavka sledeca da ste importovali poligon koji ima polje idTable  i u njemu broj
        'imate u drawingu procembene razrede i to je sve sto vam treba

        'poligon se nalazi u podesavanjima

        Dim drwProcembeni_ As Manifold.Interop.Drawing

        Try
            drwProcembeni_ = doc.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Nemate procembene razrede u map file, ili niste napravili dobro podesavanje.")
            Exit Sub
        End Try

        Dim drwPrivremeneTable As Manifold.Interop.Drawing
        Try
            drwPrivremeneTable = doc.ComponentSet(My.Settings.layerName_parcele)
        Catch ex As Exception
            MsgBox("Nemate definisane table za koje odredujem vrednost u map file, ili niste napravili dobro podesavanje.")
            Exit Sub
        End Try

        Try
            Dim drw As Manifold.Interop.Drawing = doc.ComponentSet("temp_table_pr_razred")
            doc.ComponentSet.Remove("temp_table_pr_razred")
        Catch ex As Exception

        End Try

        'formira topologiju i uslov za presek
        Dim tbl_ As Manifold.Interop.Table

        Try
            tbl_ = drwProcembeni_.OwnedTable
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
            tbl_ = drwPrivremeneTable.OwnedTable
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
        topPRazredi.Bind(drwProcembeni_)
        topPRazredi.Build()

        Dim topPrivremeneTable As Manifold.Interop.Topology = doc.Application.NewTopology
        topPrivremeneTable.Bind(drwPrivremeneTable)
        topPrivremeneTable.Build()

        topPrivremeneTable.DoIntersect(topPRazredi, "temp_table_pr_razred")

        Dim topParcPrRaz As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcPrRaz.Bind(doc.ComponentSet.Item(doc.ComponentSet.ItemByName("temp_table_pr_razred")))
        topParcPrRaz.Build()

        topPRazredi = Nothing
        topParcPrRaz = Nothing
        topPrivremeneTable = Nothing

        'sada titreba da kreiras polje u originalnog atbeli tabli koje se zove VSUMA - double na dve decimale

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "VSuma"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        drwPrivremeneTable.OwnedTable.ColumnSet.Add(col_)
        col_ = Nothing

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("sracunajVrednost")
        'racuna vrednost svake table i upisuje u polje vsuma koje je prethodno kreirano u drawingu sa privremenim tablama
        qvr_.Text = "UPDATE (SELECT round(V,2) V, VSuma FROM (SELECT sum([Area (I)]*[faktor]) as V,[idtable] FROM [Temp_table_pr_razred] GROUP by [idtable]) A, [" & My.Settings.layerName_parcele & "] WHERE A.idtable=[" & My.Settings.layerName_parcele & "].[idtable]) set vsuma=v"
        qvr_.RunEx(True)


        doc.ComponentSet.Remove("sracunajVrednost")
        doc.ComponentSet.Remove("temp_table_pr_razred")
        doc.Save()

        doc = Nothing
        MsgBox("Kraj")

    End Sub

    Private Sub ImportujUDKPNadeluToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles ImportujUDKPNadeluToolStripMenuItem.Click

        'procedura iz drawinga u kojem je rezultat - parcele ubacuje u DKP nadela i pntobelezavanje

        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        'idemo sada redom 
        Me.Cursor = Cursors.WaitCursor

        Dim dwg_ As Manifold.Interop.Drawing
        Try
            dwg_ = doc.ComponentSet(My.Settings.layerName_parcele)
        Catch ex As Exception
            MsgBox("Drawing nije definisan! - prbajte ponovo")
            Exit Sub
        End Try

        Dim drwTest As Manifold.Interop.Drawing
        'provera postojanja drawinga iz podesavanja
        Try
            drwTest = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing sa parcelama Nadele. Izaberite odgovarajuci drawing i pokrenite operaciju ponovo.")
            Exit Sub
        End Try

        Try
            drwTest = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing sa detaljnim tackama parcela Nadele. Izaberite odgovarajuci drawing i pokrenite operaciju ponovo.")
            Exit Sub
        End Try

        Try
            drwTest = doc.ComponentSet(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Nije pravilno definisan drawing sa tablama Nadele. Izaberite odgovarajuci drawing i pokrenite operaciju ponovo.")
            Exit Sub
        End Try

        'sada mi treba spisak parcela iz drawinga

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("jedan")

        Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer : analizer_.Points(dwg_, dwg_, dwg_.ObjectSet) : analizer_.RemoveDuplicates(dwg_, dwg_.ObjectSet)

        'sada mozes da ides na centralnu tacku!
        Dim qvr2_ As Manifold.Interop.Query = doc.NewQuery("dva")

        frmMain.lbl_infoMain.Text = "Pronalazenje tacaka i brisanje"
        My.Application.DoEvents()

        'sada ide pronalazenje tacaka i brisanje
        qvr2_.Text = "DELETE FROM [" & My.Settings.layerName_parcele & "] WHERE IsPoint([ID]) AND AssignCoordSys (NewPoint(round(CentroidX([ID]),2),round(CentroidY([ID]),2))  , COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)) in (SELECT [Geom (I)] FROM [" & My.Settings.layerName_pointTableObelezavanje & "])"
        qvr2_.RunEx(True)


        frmMain.lbl_infoMain.Text = "Numeracija tacaka"
        My.Application.DoEvents()

        qvr2_.Text = "SELECT distinct [" & My.Settings.layerName_parcele & "].[ID] FROM [" & My.Settings.layerName_table & "],[" & My.Settings.layerName_parcele & "] WHERE IsPoint([" & My.Settings.layerName_parcele & "].[ID]) ORDER BY [" & My.Settings.layerName_table & "].[idtable],Atn2((CentroidX([" & My.Settings.layerName_table & "].[id])-CentroidX([" & My.Settings.layerName_parcele & "].[id])),(CentroidY([" & My.Settings.layerName_table & "].[ID])-CentroidY([" & My.Settings.layerName_parcele & "].[ID])))"
        qvr2_.RunEx(True)


        qvr_.Text = "select max([idtacke]) from [" & My.Settings.layerName_pointTableObelezavanje & "] where tiptacke=3"
        qvr_.RunEx(True)


        Dim brojac_ As Integer = qvr_.Table.RecordSet.Item(0).DataText(1)

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "brTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        dwg_.OwnedTable.ColumnSet.Add(col_)
        col_ = Nothing

        For i = 0 To qvr2_.Table.RecordSet.Count - 1
            qvr_.Text = "update [" & My.Settings.layerName_parcele & "] set [brTacke]=" & brojac_ + i + 1 & "  where [id]=" & qvr2_.Table.RecordSet.Item(i).DataText(1)
            qvr_.RunEx(True)
            doc.Save()
        Next

        frmMain.lbl_infoMain.Text = "Kopiranje u pntobelezavanje"
        My.Application.DoEvents()

        'u pntobelezavanje kopiras tacke iz rucnatabla
        qvr_.Text = "insert INTO [" & My.Settings.layerName_pointTableObelezavanje & "] ([Idtacke],[idtable],[Tiptacke],[Geom (I)]) SELECT [brTacke],[idtable],3,AssignCoordSys(NewPoint(round(CentroidX([id]),2),round(CentroidY([ID]),2)),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT )) FROM [" & My.Settings.layerName_parcele & "] where IsPoint([ID])"
        qvr_.RunEx(True)

        'brises iz nadele parcele sa idtable koji se radi
        qvr_.Text = "delete from [" & My.Settings.layerName_ParceleNadela & "] where [idTable] in (select distinct [idtable] from [" & My.Settings.layerName_parcele & "])"
        qvr_.RunEx(True)

        'sada idemo da kopiramo!
        qvr_.Text = "insert into [" & My.Settings.layerName_ParceleNadela & "] ([geom (I)],[idtable],[rednibrnadele],[idvlasnika]) select [geom (I)],[idtable],[rednibrnadele],[idvlasnika] from [" & My.Settings.layerName_parcele & "]  where IsArea([id])"
        qvr_.RunEx(True)

        'sada treba da upise povrisne u kom_tableNadela

        frmMain.lbl_infoMain.Text = "Kopiranje parcela i upisivanje povrsina"
        My.Application.DoEvents()

        qvr_.Text = "select [idtable],[rednibrnadele],[idvlasnika],round([Area (I)],0) from [" & My.Settings.layerName_parcele & "] where isarea([id])"
        qvr_.RunEx(True)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_) : conn_.Open()

        pb1.Maximum = qvr_.Table.RecordSet.Count

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            pb1.Value = i
            Try
                comm_.CommandText = "UPDATE kom_tablenadela set nadeljenoPovrsina=" & Math.Round(Val(qvr_.Table.RecordSet.Item(i).DataText(4)), 0) & " where IdTable=" & qvr_.Table.RecordSet.Item(i).DataText(1) & " and idIskazZemljista=" & qvr_.Table.RecordSet.Item(i).DataText(3) & " and rednibrojnadele=" & qvr_.Table.RecordSet.Item(i).DataText(2)
                comm_.ExecuteNonQuery()
            Catch ex As Exception
                MsgBox("Problem sa update kom_tablenadela - nadeljenoPovrsina")
            End Try

        Next

        doc.Save()
        pb1.Value = 0
        qvr_ = Nothing : qvr2_ = Nothing
        doc.ComponentSet.Remove("jedan") : doc.ComponentSet.Remove("dva")
        Me.Cursor = Cursors.Default
        MsgBox("Kraj ")
    End Sub

    Private Sub DfasdfToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles DfasdfToolStripMenuItem.Click

        'prvo da proverimo da li postoje drawing-zi

        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        'provera postojanja drawing-a

        Dim drw_table As Manifold.Interop.Drawing

        Try
            drw_table = doc.ComponentSet.Item(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Proverite podesanje drawinga za table, pa pokretnite proceduru ponovo.")
            Exit Sub
        End Try

        Dim drw_procRazredi As Manifold.Interop.Drawing
        Try
            drw_procRazredi = doc.ComponentSet.Item(My.Settings.layerName_ProcembeniRazredi)
        Catch ex As Exception
            MsgBox("Proverite podesanje drawinga za procenom, pa pokretnite proceduru ponovo.")
            Exit Sub
        End Try

        'prvo proveris dali u gridu ima nesto
        Dim sumaVrednosti As Double = 0 : Dim matVlasnika(-1) As String : Dim matVrednosti(-1) As Double


        opf_diag.FileName = ""
        opf_diag.DefaultExt = "csv"
        opf_diag.Filter = "CSV file (*.csv)|*.csv|Txt file (*.txt)|*.txt|All files (*.*)|*.*"
        opf_diag.RestoreDirectory = True
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then Exit Sub
        Dim brojac_ As Integer = 0
        Dim freefileIN_ As Integer = FreeFile()
        FileOpen(freefileIN_, opf_diag.FileName, OpenMode.Input, OpenAccess.Read, OpenShare.Shared)

        Do While Not EOF(freefileIN_)
            Dim a_ = LineInput(freefileIN_)
            Dim b_ = Split(a_, ",")
            If b_.Length = 2 And b_(0) <> "" And Val(b_(1)) > 0 Then
                ReDim Preserve matVlasnika(brojac_)
                ReDim Preserve matVrednosti(brojac_)
                matVlasnika(brojac_) = b_(0)
                matVrednosti(brojac_) = b_(1)
                sumaVrednosti += b_(1)
                brojac_ += 1
            End If
        Loop

        'conn_ = Nothing

        Dim brTable_ As Integer
        Try
            brTable_ = InputBox("Upisite broj table?", "Unos table", ddl_ttpSpisakTabli.SelectedValue)
        Catch ex As Exception
            brTable_ = InputBox("Upisite broj table?", "Unos table", "1")
        End Try


        'napravis prvo presek procembenih razreda i tabli
        Dim tbl_ As Manifold.Interop.Table
        Try
            Dim drw As Manifold.Interop.Drawing = doc.ComponentSet("table_pr_razred")
            doc.ComponentSet.Remove("table_pr_razred")
        Catch ex As Exception

        End Try

        'podesis da je samo ova tabla status1


        Dim freefile_ As Integer = FreeFile()
        Try
            FileOpen(freefile_, Path.GetTempPath() & "\nadela_table_" & brTable_ & ".txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        Catch ex As Exception
            FileClose()
            FileOpen(freefile_, Path.GetTempPath() & "\nadela_table_" & brTable_ & ".txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        End Try
        'FileOpen(freefile_, Path.GetTempPath() & "\nadela_table_" & brTable_ & ".txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

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
            tbl_ = drw_table.OwnedTable
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
        topPRazredi.Bind(drw_procRazredi)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drw_table)
        topParcele.Build()

        topParcele.DoIntersect(topPRazredi, "table_pr_razred")

        Dim topParcPrRaz As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcPrRaz.Bind(doc.ComponentSet.Item(doc.ComponentSet.ItemByName("table_pr_razred")))
        topParcPrRaz.Build()

        topPRazredi = Nothing : topParcPrRaz = Nothing : topParcele = Nothing

        doc.Save()

        Dim drwFR As Manifold.Interop.Drawing = doc.ComponentSet.Item("table_pr_razred")

        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("brojTabli")
        qvr_.Text = "select sum([table_pr_razred].[Area (I)]*[Faktor]) as vrednost from [table_pr_razred] WHERE [idTable]=" & brTable_
        qvr_.RunEx(True)

        If qvr_.Table.RecordSet(0).DataText(1) = "" Then
            MsgBox("Nesto nije u redu sa podesavanjima - ili drawing ili baza komasacije - proverite jos jednom.")
            Exit Sub
        End If

        MsgBox("Suma ulaznih vrednosti je: " & sumaVrednosti & ", a ukupna vrednost za nadelu u tabli je: " & qvr_.Table.RecordSet(0).DataText(1))

        Dim brojacNer_ As Integer = 0

        doc.Save()
        Dim drwNewTable As Manifold.Interop.Drawing
        Try
            drwNewTable = doc.NewDrawing("tablaPR_" & brTable_, drw_table.CoordinateSystem, True)
        Catch ex As Exception
            'znaci da postoji 
            doc.ComponentSet.Remove("tablaPR_" & brTable_)
            drwNewTable = doc.NewDrawing("tablaPR_" & brTable_, drw_table.CoordinateSystem, True)
        End Try

        'sada sve copiras i njega
        Dim qvrCopy As Manifold.Interop.Query = doc.NewQuery("kopirajTablu")

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "OldID"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idVlasnika"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "redniBrNadele"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "idTable"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "tipTable"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "procembeni"
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_.Name = "faktor"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        drwNewTable.OwnedTable.ColumnSet.Add(col_)
        col_ = Nothing

        qvrCopy.Text = "insert into [" & drwNewTable.Name & "] ([idtable],[procembeni],[faktor],[Geom (I)]) (select [idtable],[procembeni],[faktor],[Geom (I)] FROM [table_pr_razred] where [idTable]=" & brTable_ & ")"
        qvrCopy.RunEx(True)

        doc.ComponentSet.Remove("kopirajTablu")
        qvrCopy = Nothing
        'sada mozes odmah deobu a mozes i u sledecem krugu

        doc.Save()

        Dim qvrDist As Manifold.Interop.Query = doc.NewQuery("dist")
        qvrDist.Text = "SELECT top 1 distance(A.[geom (I)],B.[Geom (I)]) as dist_,atn2(CentroidX(A.[Geom (I)])-CentroidX(B.[Geom (I)]),CentroidY(A.[Geom (I)])-CentroidY(B.[Geom (I)])) as ugao_,CentroidX(A.[Geom (I)]) as x1,CentroidY(A.[Geom (I)]) as y1,CentroidX(B.[Geom (I)]) as x2,CentroidY(B.[Geom (I)]) as y2 FROM ((SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & brTable_ & ") as A,(SELECT [Geom (I)],[ID] FROM [" & My.Settings.layerName_nadelaSmer & "] where [IDTable]=" & brTable_ & ") as B) WHERE A.[ID]<>B.[ID]" : qvrDist.RunEx(True)
        Dim qvrLastID As Manifold.Interop.Query = doc.NewQuery("lastID")
        Dim analizerF_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        If qvrDist.Table.RecordSet.Count > 0 Then


            Dim sumazaDeob_ As Double
            pb1.Maximum = UBound(matVlasnika)
            pb1.Value = 0
            'insertujes nultu liniju!
            Dim pnt1(1), pnt2(1) As Double
            pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
            pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
            Dim du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
            podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -10, pnt1, pnt2, doc, drwNewTable, 2)

            Dim matLinija() As Integer : ReDim Preserve matLinija(0)


            qvrLastID.Text = "select top 1 [ID] from [" & drwNewTable.Name & "] order by [ID] Desc" : qvrLastID.RunEx(True)

            matLinija(0) = qvrLastID.Table.RecordSet(0).DataText(1)

            'doc.Save()

            PrintLine(freefile_, "Poceo sa obradom table u " & Now())
            Dim prethodnaPovrsina As Double = 0
            For j = 0 To UBound(matVlasnika)  'ovde zamnei sa necim na ulazu!
                pb1.Value = j
                Dim kraj As Boolean = False
                'treba ti rastojanje izmedu dve tacke
                sumazaDeob_ += matVrednosti(j)
                Dim h_ As Double = H_racunanjeDirektno(qvrDist.Table.RecordSet(0).DataText(1), qvrDist.Table.RecordSet(0).DataText(1), qvr_.Table.RecordSet.Item(0).DataText(1), sumazaDeob_)
                'sada treba videti dali je to ok!
                Dim interacija_ As Integer = -1
                pb2.Maximum = My.Settings.nadela_brInteracija

                Do While Not kraj = True
                    interacija_ += 1
                    Try
                        pb2.Value = interacija_
                    Catch ex As Exception

                    End Try

                    PrintLine(freefile_, "Poceo interaciju " & interacija_)

                    Dim drwTemp As Manifold.Interop.Drawing
                    Try
                        drwTemp = doc.NewDrawing("temp", drwNewTable.CoordinateSystem, True)
                    Catch ex As Exception
                        doc.ComponentSet.Remove("temp")
                        drwTemp = doc.NewDrawing("temp", drwNewTable.CoordinateSystem, True)
                    End Try

                    col_ = doc.Application.NewColumnSet.NewColumn : col_.Name = "OldID" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idVlasnika" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : col_.Name = "redniBrNadele" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "idTable" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "tipTable" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "procembeni" : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_.Name = "faktor" : col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64 : drwTemp.OwnedTable.ColumnSet.Add(col_)
                    col_ = Nothing

                    Dim q_ As Manifold.Interop.Query = doc.NewQuery("dfas")
                    q_.Text = "insert into [temp] (OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)]) (select OldID,idVlasnika,redniBrNadele,idTable,tipTable,procembeni,faktor,[geom (i)] from [" & drwNewTable.Name & "])"
                    q_.RunEx(True)
                    doc.ComponentSet.Remove("dfas")

                    Dim drwLine As Manifold.Interop.Drawing

                    Try
                        drwLine = doc.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                    Catch ex As Exception
                        doc.ComponentSet.Remove("linije")
                        drwLine = doc.NewDrawing("linije", drwNewTable.CoordinateSystem, True)
                    End Try

                    'Dim pnt1(1), pnt2(1) As Double
                    pnt1(0) = qvrDist.Table.RecordSet(0).DataText(3) : pnt1(1) = qvrDist.Table.RecordSet(0).DataText(4)
                    pnt2(0) = qvrDist.Table.RecordSet(0).DataText(5) : pnt2(1) = qvrDist.Table.RecordSet(0).DataText(6)
                    du = NiAnaB(pnt1(0), pnt1(1), pnt2(0), pnt2(1))
                    podeliParceluViseDelova_KreirajPresecnuLiniju(du - 90, qvrDist.Table.RecordSet(0).DataText(1), h_, pnt1, pnt2, doc, drwLine)
                    Dim lineID As Integer = drwLine.ObjectSet.Item(0).ID
                    'sada imas liniju i ide presek

                    Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
                    analizer_.Split(drwTemp, drwTemp, drwTemp.ObjectSet, drwLine.ObjectSet)

                    'mozda ovde da napravi zaokruzivanje!

                    'doc.Save()
                    analizer_ = Nothing
                    'problem kako da selektujes poligone koji su nastali naknadno? izmedu linije i dve tacke?
                    podeliParceluViseDelova_KreirajPresecnuLiniju2(du - 90, qvrDist.Table.RecordSet(0).DataText(1), -h_, pnt1, pnt2, doc, drwLine, 2)
                    'doc.Save()
                    'sada ti treba convechull da napravi poligon
                    Dim qvrDobijenaPov As Manifold.Interop.Query = doc.NewQuery("koliko")
                    qvrDobijenaPov.Text = "insert into [linije] ([Geom (I)]) VALUES (SELECT ConvexHull(AllCoords([Geom (I)])) FROM [Linije])"
                    qvrDobijenaPov.RunEx(True)
                    'doc.Save()
                    'sada radis update faktora! za svaki slucaj! ovo bi trebalo da napravis i gore
                    qvrDobijenaPov.Text = "update (SELECT [temp].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([Temp], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [Temp].[ID])) set out_ = in_"
                    qvrDobijenaPov.RunEx(True)
                    'doc.Save()
                    qvrDobijenaPov.Text = "SELECT sum(round([temp].[Area (I)])*[temp].[Faktor]) FROM [Temp],[Linije] WHERE IsArea([Temp].[ID]) and  Contains([Linije].[ID],[Temp].[ID])"
                    qvrDobijenaPov.RunEx(True)
                    'SADA treba skratiti celu pricu!
                    'doc.Save()
                    'ovde ide print

                    If interacija_ > My.Settings.nadela_brInteracija Then
                        'MsgBox("Nesto nije u redu!? broj interacija je veci od onoga sto treba-prepostavljam da je u pitanju tabla koja se za malo razlikuje od onoga sto treba")
                        'pronasao! izlazim iz ovoga kreiras liniju u temp3
                        drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                        qvrLastID.RunEx(True)
                        ReDim Preserve matLinija(j + 1)
                        matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        doc.ComponentSet.Remove("koliko")
                        kraj = True
                        Exit Do
                    End If

                    tss_label.Text = "Deoba za iskaz " & matVlasnika(j) & " razlika> " & (Math.Round(Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1)) - sumazaDeob_, 2))
                    My.Application.DoEvents()

                    Dim nesto_ As Double = Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))

                    tss_label.Text = "Obrada iskaza " & matVlasnika(j) & ", razlika: " & (Math.Round(nesto_, 2) - Math.Round(sumazaDeob_, 2)) : My.Application.DoEvents()
                    My.Application.DoEvents()

                    'ovde u stvari treba gledati da li je i prethodna razlika bila ista jer ako jeste onda mozes da prekines


                    If Math.Round(nesto_, 2) = Math.Round(sumazaDeob_, 2) Or (nesto_ = prethodnaPovrsina) Then
                        prethodnaPovrsina = 0
                        'pronasao! izlazim iz ovoga kreiras liniju u temp3
                        drwNewTable.ObjectSet.Add(drwLine.ObjectSet.Item(drwLine.ObjectSet.ItemByID(lineID)).Geom) 'ovde nastaje problem kako da prepoznas liniju!
                        qvrLastID.RunEx(True)
                        ReDim Preserve matLinija(j + 1)
                        matLinija(j + 1) = qvrLastID.Table.RecordSet(0).DataText(1)
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        kraj = True
                    Else
                        prethodnaPovrsina = nesto_
                        'idemo iz pocetka
                        h_ = h_ + ((sumazaDeob_ - Val(qvrDobijenaPov.Table.RecordSet(0).DataText(1))) / qvrDist.Table.RecordSet(0).DataText(1))
                        doc.ComponentSet.Remove("temp") : doc.ComponentSet.Remove("linije")
                        drwLine = Nothing : drwTemp = Nothing
                        PrintLine(freefile_, "kraj interacije " & Now())
                        'interacija_ += 1
                    End If
                    doc.ComponentSet.Remove("koliko")
                    qvrDobijenaPov = Nothing
                Loop

            Next

            PrintLine(freefile_, "Kraj " & Now())
            pb1.Value = 0 : pb2.Value = 0

            '// OVDE UBACIO SVE STO TREBA IZ PRETHODNOG

            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [OldID]=[ID]" : qvrLastID.RunEx(True)
            'doc.Save()

            analizerF_.Split(drwNewTable, drwNewTable, drwNewTable.ObjectSet, drwNewTable.ObjectSet)

            'doc.Save()

            For i = 0 To matLinija.Length - 2
                qvrLastID.Text = "update (SELECT [" & My.Settings.parcele_fieldName_Vlasnik & "],[redniBrNadele] from [" & drwNewTable.Name & "] WHERE Contains((SELECT ConvexHull(AllCoords([Geom (I)])) FROM [" & drwNewTable.Name & "] WHERE [OldID]=" & matLinija(i) & " or [OldID]=" & matLinija(i + 1) & "),[ID]) AND IsArea([ID])) set " & My.Settings.parcele_fieldName_Vlasnik & "=" & Chr(34) & matVlasnika(i) & Chr(34) & ", redniBrNadele=" & i + 1
                qvrLastID.RunEx(True)
            Next

            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & Chr(34) & matVlasnika(matVlasnika.Length - 1) & Chr(34) & ", [redniBrNadele]=" & matVrednosti.Length - 1 & " where [idVlasnika]=" & Chr(34) & "0" & Chr(34) & " and [redniBrNadele]=0 and IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

        Else

            MsgBox("Proverite da li ste upisali dobar broj table u drawingu tacke!!!!")
            'sada treba da update napravis ovog jednog kojeg imasa!
            qvrLastID.Text = "update [" & drwNewTable.Name & "] set [idVlasnika]=" & matVlasnika(matVlasnika.Length - 1) & ", [redniBrNadele]=1 where IsArea([ID])"
            qvrLastID.RunEx(True)
            qvrLastID.Text = "update (SELECT [" & drwNewTable.Name & "].[Faktor] as out_,[Table_pr_razred].[Faktor] as in_ FROM([" & drwNewTable.Name & "], [Table_pr_razred]) WHERE Contains([Table_pr_razred].[ID], [" & drwNewTable.Name & "].[ID])) set out_ = in_"
            qvrLastID.RunEx(True)

        End If

        Dim drwnewDisolve As Manifold.Interop.Drawing
        Try
            drwnewDisolve = doc.NewDrawing(drwNewTable.Name & "_dissolve", drwNewTable.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove(drwNewTable.Name & "_dissolve")
            drwnewDisolve = doc.NewDrawing(drwNewTable.Name & "_dissolve", drwNewTable.CoordinateSystem, True)
        End Try

        'sada kreiras polje za pocetak rednibrnadele i idtable i idvlasnika
        tbl_ = drwnewDisolve.OwnedTable
        col_ = doc.Application.NewColumnSet.NewColumn
        col_.Name = "redniBrNadele"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idtable"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "idvlasnika"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        'sada jedino merge na osnovu selekcije koja ide izmedu linija jer drugacije nece da radi - na primer disolve 
        'ili recimo redni broj nadele? - kako ovo da dobijes? isto 

        tbl_ = Nothing : col_ = Nothing
        'doc.Save()
        qvrDist.Text = "INSERT INTO [" & drwnewDisolve.Name & "] ([Geom (I)],[redniBrNadele]) (SELECT * FROM (SELECT UnionAll([ID]) as pera_,[redniBrNadele] FROM [" & drwNewTable.Name & "] GROUP BY [redniBrNadele] ) where pera_ IS NOT NULL )"
        qvrDist.RunEx(True)
        qvrDist.Text = "update (SELECT [" & drwNewTable.Name & "].[idVlasnika] as in_,[" & drwnewDisolve.Name & "].[idVlasnika] as out_ FROM [" & drwNewTable.Name & "],[" & drwnewDisolve.Name & "] WHERE [" & drwNewTable.Name & "].[redniBrNadele]=[" & drwnewDisolve.Name & "].[redniBrNadele] ) set out_=in_ "
        qvrDist.RunEx(True)
        qvrDist.Text = "update [" & drwnewDisolve.Name & "] set [idTable]=" & brTable_
        qvrDist.RunEx(True)
        'sada ostaje da napravis update za idtable i vlasnika!
        analizerF_.NormalizeTopology(drwnewDisolve, drwnewDisolve.ObjectSet)

        analizerF_ = Nothing
        doc.ComponentSet.Remove("Table_pr_razred")
        doc.ComponentSet.Remove("lastID") : qvrLastID = Nothing : drwNewTable = Nothing
        doc.ComponentSet.Remove("dist") : qvrDist = Nothing : drwFR = Nothing
        doc.ComponentSet.Remove("brojTabli") : qvr_ = Nothing
        doc.Save()
        Close()
        MsgBox("kraj")

    End Sub

    Private Sub NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles NadelaIObelezavanjeTableUJednomMapFileuToolStripMenuItem.Click
        stampajNadeluIObelezavanje(ddl_ttpSpisakTabli.SelectedValue, True)
    End Sub
End Class