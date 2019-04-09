Imports System.Globalization
Imports Microsoft.Office.Interop
Imports MySql.Data.MySqlClient
Imports System.IO

Public Class frmDKP
    Private Sub KomasacionoPoducjeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles KomasacionoPoducjeToolStripMenuItem.Click
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

        Dim docOld As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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

        frmMain.lbl_infoMain.Text = "Kreiram Label za Apcisno odmeranje" : My.Application.DoEvents()

        qvr_.Text = "select distinct [idTable] from [" & drwTableNew.Name & "] where [tipTable]=2 order by [idTable]"
        qvr_.RunEx(True)

        'ovde ti treba samo label i to je mozda najveci problem aj onda odma label!
        pb1.Value = 0
        pb1.Maximum = qvr_.Table.RecordSet.Count

        For i = 0 To qvr_.Table.RecordSet.Count - 1
            frmMain.lbl_infoMain.Text = "Kreiram Label za Apcisno odmeranje. Tabla " & qvr_.Table.RecordSet.Item(i).DataText(1) : My.Application.DoEvents()

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

        frmMain.lbl_infoMain.Text = "Kreiranje Label-a za frontove"
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

            frmMain.lbl_infoMain.Text = "Kreiranje Label-a za frontove za stalne zasade"
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

        frmMain.lbl_infoMain.Text = "Popunjavanje Vlasnika u polju Opis"
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

        frmMain.lbl_infoMain.Text = "Kreiram Label Opis za svaku parcelu" : My.Application.DoEvents()

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

            frmMain.lbl_infoMain.Text = "Kreiram Label Islaz za svaku parcelu" : My.Application.DoEvents()

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
            frmMain.lbl_infoMain.Text = "Kreiram Label Parcele za svaku parcelu" : My.Application.DoEvents()
            'za parcele

            Dim labelbrParcele_ As Manifold.Interop.Labels = newDoc.NewLabels("label_Parcele", drwParceleLinije.CoordinateSystem, True)
            labelbrParcele_.LabelAlignX = LabelAlignX.LabelAlignXCenter : labelbrParcele_.LabelAlignY = LabelAlignY.LabelAlignYCenter
            labelbrParcele_.OptimizeLabelAlignX = False : labelbrParcele_.OptimizeLabelAlignY = False
            labelbrParcele_.ResolveOverlaps = False : labelbrParcele_.PerLabelFormat = True
            Dim lblSets4_ As Manifold.Interop.LabelSet = labelbrParcele_.LabelSet

            frmMain.lbl_infoMain.Text = "Kreiram Label Islaz za svaku parcelu" : My.Application.DoEvents()

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

    Private Sub GradevinskiRejonToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GradevinskiRejonToolStripMenuItem.Click

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

        Dim docOld As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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

        frmMain.lbl_infoMain.Text = "Kreiranje Label-a za frontove"
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

        frmMain.lbl_infoMain.Text = "Kreiram Label Opis za svaku parcelu" : My.Application.DoEvents()

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

        frmMain.lbl_infoMain.Text = "Kreiram Label Iskaza za svaku parcelu" : My.Application.DoEvents()

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
        frmMain.lbl_infoMain.Text = ""
        MsgBox("Kraj")
    End Sub
    Private Sub OpisPolozajaPoligonskeTackeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpisPolozajaPoligonskeTackeToolStripMenuItem.Click
        'kreiras jedan drawinga :  linije
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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

    Private Sub ProcesiranjeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ProcesiranjeToolStripMenuItem.Click

        'txtQuery.Text = "Pocetak :" & Now() & vbNewLine & "Potrebni layer(i):" & vbNewLine & " -Poligonske tacke (DWG Poligonske tacke);" & vbNewLine & " -Detaljne tacke (DWG Tacke Obelezanja)" & vbNewLine & "  - Prepreke (DWG Ulice)"
        frmMain.txt_Query.DocumentText = "Pocetak :" & Now() & vbNewLine & "Potrebni layer(i):" & vbNewLine & " -Poligonske tacke (DWG Poligonske tacke);" & vbNewLine & " -Detaljne tacke (DWG Tacke Obelezanja)" & vbNewLine & "  - Prepreke (DWG Ulice)"
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
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

        frmMain.lbl_infoMain.Text = "Kreiram 01_Pravci" : My.Application.DoEvents()

        'txtQuery.Text = txtQuery.Text & vbNewLine & "Kreiranje polja"
        frmMain.txt_Query.DocumentText += vbNewLine & "Kreiranje polja"
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

        frmMain.lbl_infoMain.Text = "Kreiram i upisujem pravce u 01_Pravci" : My.Application.DoEvents()
        'sada u njega ubacujes pravce sa tacaka
        pb1.Value = 0
        pb1.Maximum = 3
        q01.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_pointTableObelezavanje & Chr(34) & " as COMPONENT); insert into [01_Pravci] (PoligonskaIDTacke,DetaljnaID,Rastojanje,[Geom (I)]) (SELECT [" & My.Settings.layerName_poligonskeTacke & "].[brTacke],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID],(Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])) as D,NewLine([" & My.Settings.layerName_pointTableObelezavanje & "].[Geom (I)],[" & My.Settings.layerName_poligonskeTacke & "].[Geom (I)]) as line_ FROM [" & My.Settings.layerName_poligonskeTacke & "],[" & My.Settings.layerName_pointTableObelezavanje & "] WHERE Distance([" & My.Settings.layerName_poligonskeTacke & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID])<" & My.Settings.tahimetrija_sirinaBaferZone & ")"
        q01.RunEx(True)

        pb1.Value = 1
        frmMain.lbl_infoMain.Text = "Brisem pravce koji imaju presek" : My.Application.DoEvents()
        'sada ide pronalazenje preseka pravaca sa preprekama 
        q01.Text = "delete from [01_Pravci] where [ID] in (SELECT [01_Pravci].[ID] FROM [01_Pravci],[" & My.Settings.layerName_Ulice & "] WHERE Intersects([01_Pravci].[Geom (I)],[Prepreke].[Geom (I)]))" : q01.RunEx(True)
        pb1.Value = 2

        frmMain.lbl_infoMain.Text = "Upisujem tacke mreze u bazu" : My.Application.DoEvents()
        q01.Text = "UPDATE (SELECT * FROM (SELECT A.[DetaljnaID],B.[PoligonskaIDTacke] FROM (SELECT [DetaljnaID],min(Rastojanje) as Rastojanje FROM [01_Pravci] GROUP by [DetaljnaID]) as A LEFT JOIN (SELECT [DetaljnaID],Rastojanje,[PoligonskaIDTacke] FROM 	[01_Pravci]) as B on A.[DetaljnaID]=B.[DetaljnaID] and A.Rastojanje=B.Rastojanje )) as C LEFT join (SELECT [" & My.Settings.layerName_pointTableObelezavanje & "].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[brTacke] FROM [" & My.Settings.layerName_pointTableObelezavanje & "]) as D on C.[DetaljnaID]=D.[ID] ) set brTacke=PoligonskaIDTacke" : q01.RunEx(True)

        pb1.Value = 3

        doc.Save()

        drwNewTemp = Nothing
        doc.ComponentSet.Remove("01")
        tbl_ = Nothing : q01 = Nothing : col_ = Nothing : doc = Nothing

        MsgBox("Kraj")
        pb1.Value = 0
    End Sub

    Private Sub BrDetaljnihTacakaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BrDetaljnihTacakaToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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

    Private Sub ZapisnikExcelToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ZapisnikExcelToolStripMenuItem.Click
        'ovde ti treba tacka sa koje se startuje! - kako sledecu da nade? pitanje
        'txtQuery.Text = "Pocetak: " & Now()
        frmMain.txt_Query.DocumentText = "Pocetak: " & Now()
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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
            frmMain.txt_Query.DocumentText += vbNewLine & "     Poligonska: " & qvrPoligone.Table.RecordSet.Item(i).DataText(1)
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
                frmMain.txt_Query.DocumentText += vbNewLine & "           Detaljna : " & qvrDetaljne.Table.RecordSet.Item(j).DataText(2)
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
                    frmMain.txt_Query.DocumentText += vbNewLine & "                   Slepa : " & qvrSlepe.Table.RecordSet.Item(k).DataText(2)

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
                        frmMain.txt_Query.DocumentText = vbNewLine & "                       Pod-Slepa : " & qvrSlepeLevel1.Table.RecordSet.Item(p).DataText(2)
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
        frmMain.txt_Query.DocumentText += vbNewLine & "Kraj: " & Now()
        MsgBox("Kraj")
    End Sub

    Public Sub zaokruziPovrsineKFMNS()
        'ovde je problem ! zato st ne mora da znaci da je povrsina pod procembenim razredima ista!!!!!!!!!!!!!!1
        'i to komplikuje stvar!

        'aj da probamo ovako:

        'ako suma zaokruzenih vrednosti odgovara povrsini iz tabli onda je ok i ne moras dalje da se zajebavas 
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "SELECT distinct idtable from kom_table where obrisan=0"

        Dim dl_ As New DataTable : Dim myadap_ As New MySqlDataAdapter(comm_)
        myadap_.Fill(dl_) : Dim freeFile_ As Integer = FreeFile()
        FileOpen(freeFile_, Path.GetTempPath() & "\izvestajzaokruzvanjaParcela2.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        frmMain.pb1.Value = 0
        frmMain.pb1.Maximum = dl_.Rows.Count

        frmMain.lbl_infoMain.Text = "Zaokruzivanje povrsine"
        My.Application.DoEvents()

        For i = 0 To dl_.Rows.Count - 1
            'sada idemo za svaku tablu!
            Dim txt_ As String = ""
            frmMain.pb1.Value = i

            comm_.CommandText = "SELECT sum( round(prazred_1) + round(prazred_2) + round(prazred_3) + round(prazred_4) + round(prazred_5) + round(prazred_6) + round(prazred_7) + round(prazred_neplodno)) - ( SELECT sum(prazred1+prazred2+prazred3+prazred4+prazred5+prazred6+prazred7+prazred_neplodno) FROM kom_table WHERE idtable = " & dl_.Rows(i).Item(0) & "  AND obrisan = 0 ) FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0)
            'sada ti treba reader_
            Dim myread_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)

            If myread_.HasRows Then
                myread_.Read()
                Dim razlika_ As Integer = myread_.GetValue(0) 'uf ova razlika mi nista ne znaci jer mi treba razlika po razredu!

                txt_ = dl_.Rows(i).Item(0) & " , " & razlika_ & ","

                myread_.Close()
                Try
                    conn_.Open()
                Catch ex As Exception

                End Try
                If razlika_ = 0 Then
                    'ako je nula znaci da mozes sve da zaokruzs i da nemas problem!
                    comm_.CommandText = "update kom_novostanjaparcela set prazred_1=round(prazred_1), prazred_2=round(prazred_2),prazred_3=round(prazred_3), prazred_4=round(prazred_4), prazred_5=round(prazred_5), prazred_6=round(prazred_6), prazred_7=round(prazred_7), prazred_neplodno=round(prazred_neplodno) where idtable=" & dl_.Rows(i).Item(0)
                    comm_.ExecuteNonQuery()
                Else
                    'sada ovo treba videti sta dalje!!!!!!!!!!!! ovde je najveci problem jer koliko ovoga moze da bude !
                    'sada ides za svaki procembeni razred po jedno!
                    'If dl_.Rows(i).Item(0) = 28 Then
                    '    MsgBox("9")
                    'End If

                    For j = 1 To 8
                        'sada da probamo isti sistem kao i kod sume za celu tablu!

                        Dim gde_ As String = j : If j = 8 Then gde_ = "neplodno"

                        comm_.CommandText = "SELECT round((sum(prazred_" & gde_ & ")-sum(round(prazred_" & gde_ & ")))) FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0)
                        'comm_.CommandText = "SELECT idKfmsns, abs(round(prazred_3) - prazred_3) AS raz_ FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " ORDER BY raz_ DESC LIMIT " & Math.Abs(razlika_)
                        'sta sad da radim! opet idem ona reader!
                        Dim ggread_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
                        ggread_.Read()
                        Dim raz2_ As Integer = ggread_.GetValue(0)
                        ggread_.Close()
                        Try
                            conn_.Open()
                        Catch ex As Exception

                        End Try

                        If raz2_ = 0 Then
                            'ovo je dobro
                            comm_.CommandText = "update kom_novostanjaparcela set prazred_" & gde_ & "=round(prazred_" & gde_ & ") where idtable=" & dl_.Rows(i).Item(0)
                            comm_.ExecuteNonQuery()
                            txt_ = txt_ & ",0"
                        Else

                            txt_ = txt_ & "," & raz2_

                            'ovo ide na zaokruzvanje!
                            comm_.CommandText = "SELECT idKfmsns, abs(round(prazred_" & gde_ & " ) - prazred_" & gde_ & ") AS raz_ FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " and abs(round(prazred_" & gde_ & " ) - prazred_" & gde_ & ")" & "<>0 ORDER BY raz_ DESC LIMIT " & Math.Abs(raz2_)
                            'svali ili dodajes ili oduzimas u odnosu na raz!
                            Dim tbl2_ As New DataTable
                            Dim myadap2_ As New MySql.Data.MySqlClient.MySqlDataAdapter(comm_)
                            myadap2_.Fill(tbl2_)

                            'sada mozes dalje aj da vidimo sta cemo a ovim 

                            Dim suma_ As Double = 0 : Dim skidam_ As Integer = 0

                            For q = 0 To tbl2_.Rows.Count - 1
                                suma_ += tbl2_.Rows(q).Item(1)
                            Next

                            'mozda ovo bude drugacije: ako sumiras sve razlike i onda reciprocno delis! to moze da bude ok

                            If raz2_ > 0 Then skidam_ = 1 Else skidam_ = -1 'ovde treba mozda veca prica al aj
                            Dim ostalo_ As Integer = 0

                            For g = 0 To tbl2_.Rows.Count - 1
                                'sada idemo redom i skidamo po jedan odnosno -1 to moram da vidim jos!
                                comm_.CommandText = "update kom_novostanjaparcela set prazred_" & gde_ & "=round(prazred_" & gde_ & ",0)+" & skidam_ & " where idKfmsns=" & tbl2_.Rows(g).Item(0)
                                'Math.Round(tbl2_.Rows(g).Item(1) / suma_, 0)
                                comm_.ExecuteNonQuery()
                                'ostalo_ += Math.Round(tbl2_.Rows(g).Item(1) / suma_, 0)
                                'ovo bi trebalo da je to to!
                            Next

                            comm_.CommandText = "update kom_novostanjaparcela set prazred_" & gde_ & "=round(prazred_" & gde_ & ") where idtable=" & dl_.Rows(i).Item(0)
                            comm_.ExecuteNonQuery()
                        End If
                    Next

                End If

            Else
                'ovde imas problem ali koji
                MsgBox("problem korak 1")
            End If

            PrintLine(freeFile_, txt_)
        Next

        'idemo da zaokruzimo sumu povrisna i ponovo da sracunamo vrednost!
        comm_.CommandText = "update kom_novostanjaparcela set ukupno_povrsina=(prazred_1+prazred_2+prazred_3+prazred_4+prazred_5+prazred_6+prazred_7+prazred_neplodno)"
        comm_.ExecuteNonQuery()

        comm_.CommandText = "update kom_novostanjaparcela set ukupno_vrednost=(prazred_1+prazred_2*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=2)+prazred_3*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=3)+prazred_4*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=4)+prazred_5*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=5)+prazred_6*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=6)+prazred_7*(select VrednostKoeficijenta from kom_koeficijenti where brojKoeficijenta=7))"
        comm_.ExecuteNonQuery()

        comm_ = Nothing
        conn_.Close()
        conn_ = Nothing

        Close()
        MsgBox("Kraj zaokriuzivanja")
    End Sub

    Public Sub zaokruziPUKFMNS()

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = "SELECT distinct idtable from kom_table where obrisan=0"

        Dim dl_ As New DataTable : Dim myadap_ As New MySqlDataAdapter(comm_)
        myadap_.Fill(dl_) : Dim freeFile_ As Integer = FreeFile()
        FileOpen(freeFile_, Path.GetTempPath() & "\izvestajzaokruzvanjaParcela.txt", OpenMode.Output, OpenAccess.Write, OpenShare.Shared)
        frmMain.pb1.Value = 0
        frmMain.pb1.Maximum = dl_.Rows.Count

        frmMain.lbl_infoMain.Text = "Zaokruzivanje povrsine"
        My.Application.DoEvents()

        For i = 0 To dl_.Rows.Count - 1
            For j = 1 To 8 'za svaki procembeni razred posebno!
                If j <> 8 Then
                    comm_.CommandText = "SELECT idKfmsns, round(prazred_" & j & ") AS A, abs(round(prazred_" & j & ") - prazred_" & j & ") AS B, ( SELECT prazred" & j & " FROM kom_table WHERE idTable = " & dl_.Rows(i).Item(0) & " AND obrisan = 0 ) AS G, ( SELECT sum(round(prazred_" & j & ")) FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " ) AS M FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " GROUP BY brParcele ORDER BY B DESC "
                Else
                    comm_.CommandText = "SELECT idKfmsns, round(prazred_neplodno) AS A, abs(round(prazred_neplodno) - prazred_neplodno) AS B, ( SELECT prazred_neplodno FROM kom_table WHERE idTable = " & dl_.Rows(i).Item(0) & " AND obrisan = 0 ) AS G, ( SELECT sum(round(prazred_neplodno)) FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " ) AS M FROM kom_novostanjaparcela WHERE idTable = " & dl_.Rows(i).Item(0) & " GROUP BY brParcele ORDER BY B DESC "
                End If
                'sada mozes da pakujes ono dalje!
                myadap_.SelectCommand = comm_ : Dim tbl2 As New DataTable
                myadap_.Fill(tbl2)

                Dim razlika_ As Integer = tbl2.Rows(0).Item(3) - tbl2.Rows(0).Item(4)
                Dim g_ As Integer


                For k = 0 To tbl2.Rows.Count - 1
                    If razlika_ = 0 Then
                        g_ = 0
                    ElseIf razlika_ > 0 Then
                        g_ = 1
                    Else
                        g_ = -1
                    End If
                    If j <> 8 Then
                        comm_.CommandText = "update kom_novostanjaparcela set prazred_" & j & " = round(prazred_" & j & ") + " & g_ & " where idkfmsns=" & tbl2.Rows(k).Item(0)
                    Else
                        comm_.CommandText = "update kom_novostanjaparcela set prazred_neplodno = round(prazred_neplodno) + " & g_ & " where idkfmsns=" & tbl2.Rows(k).Item(0)
                    End If

                    comm_.ExecuteNonQuery()
                    razlika_ = razlika_ - g_
                Next

                'sada mizes i sve ostale u ovom razredu
                If j <> 8 Then
                    comm_.CommandText = "update kom_novostanjaparcela set prazred_" & j & "= round(prazred_" & j & ")"
                    comm_.ExecuteNonQuery()
                Else
                    comm_.CommandText = "update kom_novostanjaparcela set prazred_neplodno = round(prazred_neplodno)"
                    comm_.ExecuteNonQuery()
                End If
                pb1.Value = j
            Next
            frmMain.pb1.Value = i
        Next

        'sada ide update za sumu proce

        'sada ostaje da update napravis za vrednosti!
        comm_.CommandText = "update kom_novostanjaparcela set ukupno_vrednost=(prazred_1+prazred_2*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=2)+prazred_3*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=3)+prazred_4*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=4)+prazred_5*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=5)+prazred_6*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=6)+prazred_7*(select vrednostkoeficijenta from kom_koeficijenti where brojkoeficijenta=7))"
        comm_.ExecuteNonQuery()

        'ds_ = Nothing
        comm_ = Nothing
        conn_.Close()
        pb1.Value = 0
        MsgBox("Kraj")
        FileClose()
    End Sub

    Private Sub ZapisnikToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ZapisnikToolStripMenuItem.Click

        'koji je algoritam?

        'trebaju ti dve informacije: koji je krug i koji je pocetak u krugu! to je mnogo bitno - ovo su ti polja: krug: idtable 

        'neka su to dva polja krug i prvi gde je sa prvi oznacen onaj koji ima jedan!
        Dim doc_ As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
        'treba ti pocetak merenja i datum kada je poceo
        Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("rastojanje")
        Dim qvr2_ As Manifold.Interop.Query = doc_.NewQuery("updateT")
        Dim qvr3_ As Manifold.Interop.Query = doc_.NewQuery("zaSpsiak")
        Dim pp_ = My.Settings.GPSMerenje_datumPocetka.Split("/")
        Dim gg_ = My.Settings.GPSMerenje_vremePocetka.Split(":")
        Dim pocetnoVreme As New Date(pp_(2), pp_(1), pp_(0), gg_(0), gg_(1), 0)
        Dim kraj As Boolean = False
        'sada ide do 
        Dim brojac_ As Integer = 0
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

        For i = 0 To qvr3_.Table.RecordSet.Count - 1

            If i <> 0 Then
                pocetnoVreme = pocetnoVreme.AddMinutes(60)
            End If

            idTrenutni = InputBox("Unesite ID tacke od koje se polazi za poligon broj " & qvr3_.Table.RecordSet.Item(i).DataText(1), "")
            kraj = False

            Do While Not kraj = True
                'selektujes 
                qvr_.Text = "SELECT top 1 MIN(Distance([Geom (I)],(select [Geom (I)] from [" & My.Settings.layerName_pointTableObelezavanje & "] where [ID]=" & idTrenutni & _
                    "))) as d_,[id] FROM [" & My.Settings.layerName_pointTableObelezavanje & "] WHERE [ID]<>" & idTrenutni & " and [mh]=0 and [idtable]=" & qvr3_.Table.RecordSet.Item(i).DataText(1) & " group by [ID] order by d_"
                qvr_.RunEx(True)

                'SADA MOZES DA PROCITAS OVAJ ID I DUZINU I U ODNOSU NA DUZINU DA POSTAVIS VREME I UPISES GA U REKORD
                Dim D_ As Double

                Try

                    D_ = Math.Round((qvr_.Table.RecordSet.Item(0).DataText(1)) / brzina) + My.Settings.GPSMerenje_zadrzavanjeNaTacki 'dobijas sekunde!
                    'sada ovo daodas u vreme!
                    kolikopresao += D_

                    'sada proveravas dali je u istom danu pa ako jeste onda ovo a ako nije ides na novi dan!
                    If kolikopresao > kolikoradim_ Then
                        'ides na novi dan

                        pocetnoVreme.AddDays(1)

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

                        qvr2_.Text = "update [" & My.Settings.layerName_pointTableObelezavanje & "] set [mh]=" & Math.Round((Rnd() * 0.03), 3) & ", [mv]=" & Math.Round((Rnd() * 0.02), 3) & _
                            ", [pdop]=" & (p_.Next(1359, 2455) / 1000) & ", [idmerenja]=" & brojac_ & ", [Vreme merenja]=cdate(" & Chr(34) & (pocetnoVreme.Year & "-" & pocetnoVreme.Month & "-" & pocetnoVreme.Day & " " & pocetnoVreme.Hour & ":" & pocetnoVreme.Minute & ":" & pocetnoVreme.Second) & Chr(34) & "), [sesija]=" & D_ & " where [id]=" & idTrenutni
                        qvr2_.RunEx(True)
                        'sada idemo iz pocetka!
                        brojac_ += 1
                        'kolikopresao = 0
                    End If
                    'Try
                    '    pb1.Value += 1
                    'Catch ex As Exception

                    'End Try
                Catch ex As Exception

                    kraj = True
                    'Exit Do

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
    Private Sub GranicneLinijeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GranicneLinijeToolStripMenuItem.Click
        'treba ti layer sa parcelama i
        'layer sa linijama koje su uvucene iz autocad-a

        'prvi korak ti je da napravis linije od parcela da ih exportujes u novi layer i da napravis drowing clean up!
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Dim drwPovrsine As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        'sada idemoda proverimo da li postoji ako postoji onda brisemo i ponovo  ga pravimo!

        Dim drwLin As Manifold.Interop.Drawing

        pb1.Value = 0
        pb1.Maximum = 9

        'aj da vidimo kako da kreiram folder!? i da dodam dwg u njega!?
        Dim drwFolder As Manifold.Interop.Folder

        Try
            drwFolder = doc.ComponentSet("DKP_Komplet")
        Catch ex As Exception
            drwFolder = doc.NewFolder("DKP_Komplet")
        End Try

        Try
            drwLin = doc.NewDrawing("DKP_GranicnaLinija", drwPovrsine.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove("DKP_GranicnaLinija")
            drwLin = doc.NewDrawing("DKP_GranicnaLinija", drwPovrsine.CoordinateSystem, True)
        End Try
        drwLin.Folder = drwFolder

        pb1.Value = 1
        'sada mozes polja da mu kreiras!



        Dim tbl_ As Manifold.Interop.Table = drwLin.OwnedTable

        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "PR_KO"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)
        col_.Name = "PR_PAR"
        tbl_.ColumnSet.Add(col_)
        col_.Name = "PR_DEOPAR"
        tbl_.ColumnSet.Add(col_)
        col_.Name = "PR_RAZ"
        tbl_.ColumnSet.Add(col_)
        col_.Name = "PRIPADNOST"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 4
        tbl_.ColumnSet.Add(col_)
        col_.Name = "TkKod"
        col_.Size = 6
        tbl_.ColumnSet.Add(col_)
        col_ = Nothing
        pb1.Value = 2
        'sada idemo da kreiramo linije to je verovatno analizer!

        Dim analizerF_ As Manifold.Interop.Analyzer = doc.NewAnalyzer

        analizerF_.Boundaries(drwPovrsine, drwPovrsine, drwPovrsine.ObjectSet)
        analizerF_.Explode(drwPovrsine, drwPovrsine.ObjectSet)
        analizerF_.RemoveDuplicates(drwPovrsine, drwPovrsine.ObjectSet)
        analizerF_ = Nothing
        pb1.Value = 3
        'sada iz selectujes i kopiras linije!
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("selekcijaJa")
        qvr_.Text = "INSERT INTO [DKP_GranicnaLinija] ([Geom (I)]) (SELECT [Geom (I)] FROM [" & drwPovrsine.Name & "] WHERE [Type (I)]=2)"
        qvr_.RunEx(True)
        qvr_.Text = "delete from [" & drwPovrsine.Name & "] where  [Type (I)]=2"
        qvr_.RunEx(True)
        pb1.Value = 4
        'sada idemo da napravimo udpdate za jedno po jedno polje!
        qvr_.Text = "update [DKP_GranicnaLinija] set [tkkod]=" & Chr(34) & "L20-01" & Chr(34)
        qvr_.RunEx(True)


        '1. KO
        qvr_.Text = "UPDATE [DKP_GranicnaLinija] set [PR_KO]=1 WHERE [DKP_GranicnaLinija].[ID] in (SELECT [DKP_GranicnaLinija].[ID] FROM [DKP_GranicnaLinija],[" & My.Settings.layerName_pointTableObelezavanje & "] WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[tipTacke]=1 and Contains([DKP_GranicnaLinija].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]) And (InStr([idTacke]," & Chr(34) & "GR" & Chr(34) & ")=0))"
        qvr_.RunEx(True)
        pb1.Value = 5

        '2 parcela
        qvr_.Text = "UPDATE [DKP_GranicnaLinija] set [PR_PAR]=1"
        qvr_.RunEx(True)
        qvr_.Text = "update (SELECT [PR_PAR],[tkKod] from [DKP_GranicnaLinija] WHERE [id] in (SELECT id_  FROM (SELECT [brparcele],[ID] as id_,count(*) as brt FROM [DKP_GranicnaLinija],( SELECT [Geom (I)] as ggr_,[brparcele] FROM [DKP_Nadela_topo] WHERE [idKultureNew]<>360 ) as AA  WHERE Contains(AA.ggr_,[ID])  GROUP by [brparcele],[ID] ) as AB WHERE brt>1) ) set [PR_PAR]=0, [tkKod]=" & Chr(34) & "L30-01" & Chr(34)
        qvr_.RunEx(True)
        pb1.Value = 6

        'deoparcele
        qvr_.Text = "UPDATE (SELECT [ID],[PR_DEOPAR] FROM [DKP_GranicnaLinija], (SELECT [Geom (I)] AS grr_ FROM [" & drwPovrsine.Name & "] WHERE [brparcele] IN (SELECT [brparcele] FROM [" & drwPovrsine.Name & "] GROUP BY [brparcele] HAVING Count(*) > 1 )) AS AA WHERE CONTAINS (AA.grr_ ,[DKP_GranicnaLinija].[ID])) set [PR_DEOPAR]=1"
        qvr_.RunEx(True)
        pb1.Value = 7

        'razmera - ja bi ovde stavio sve linije koje su granice odnosno granica ko i granica GR!
        qvr_.Text = "UPDATE [DKP_GranicnaLinija] set [PR_RAZ]=1 WHERE [DKP_GranicnaLinija].[ID] in (SELECT [DKP_GranicnaLinija].[ID] FROM [DKP_GranicnaLinija],[" & My.Settings.layerName_pointTableObelezavanje & "] WHERE [" & My.Settings.layerName_pointTableObelezavanje & "].[tipTacke]=1 and Contains([DKP_GranicnaLinija].[ID],[" & My.Settings.layerName_pointTableObelezavanje & "].[ID]))"
        qvr_.RunEx(True)
        pb1.Value = 8

        'sada je ostalo to da prikazes kao jedan!
        qvr_.Text = "UPDATE [DKP_GranicnaLinija] set [PRIPADNOST]= cstr([PR_KO]) & cstr([PR_RAZ]) & cstr([PR_PAR]) & cstr([PR_DEOPAR])"
        qvr_.RunEx(True)
        pb1.Value = 9

        'ovo po defaiultu stavljas za codnu stranu!

        qvr_.Text = "update [DKP_GranicnaLinija] set [tkkod]=" & Chr(34) & "L70-09" & Chr(34) & " where [PR_KO]=1"
        qvr_.RunEx(True)

        qvr_.Text = "UPDATE (SELECT [TKKOD] FROM [DKP_GranicnaLinija], (SELECT [Geom (I)] as ggr_ FROM [DKP_Nadela_topo] WHERE [idKultureNew]=360) AA WHERE Contains(AA.ggr_,[DKP_GranicnaLinija].[ID])) SET [TKKOD]=" & Chr(34) & "L40-01" & Chr(34)
        qvr_.RunEx(True)

        doc.ComponentSet.Remove("selekcijaJa")

        doc.Save()
        MsgBox("Kraj")
        pb1.Value = 0
    End Sub

    Private Sub DetaljneIPomocneTackeDKPToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DetaljneIPomocneTackeDKPToolStripMenuItem.Click
        'za pocetak ovde ti treba samo layer sa tackama

        'treba ti layer sa parcelama i
        'layer sa linijama koje su uvucene iz autocad-a

        'prvi korak ti je da napravis linije od parcela da ih exportujes u novi layer i da napravis drowing clean up!
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Dim drwPoints As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_pointTableObelezavanje)
        'sada idemoda proverimo da li postoji ako postoji onda brisemo i ponovo  ga pravimo!

        Dim drwPnt As Manifold.Interop.Drawing

        pb1.Value = 0
        pb1.Maximum = 9

        'aj da vidimo kako da kreiram folder!? i da dodam dwg u njega!?
        Dim drwFolder As Manifold.Interop.Folder

        Try
            drwFolder = doc.ComponentSet("DKP_Komplet")
        Catch ex As Exception
            drwFolder = doc.NewFolder("DKP_Komplet")
        End Try

        Try
            drwPnt = doc.NewDrawing("DKP_Tacka", drwPoints.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove("DKP_Tacka")
            drwPnt = doc.NewDrawing("DKP_Tacka", drwPoints.CoordinateSystem, True)
        End Try
        drwPnt.Folder = drwFolder

        pb1.Value = 1
        'sada mozes polja da mu kreiras!


        Dim tbl_ As Manifold.Interop.Table = drwPnt.OwnedTable : Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn

        col_.Name = "BrojTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 20
        tbl_.ColumnSet.Add(col_)

        col_.Name = "VrstaTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)

        col_.Name = "TkKod"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 6
        tbl_.ColumnSet.Add(col_)

        col_.Name = "Skica"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 20
        tbl_.ColumnSet.Add(col_)


        col_.Name = "Metoda"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)

        col_.Name = "Izvor"
        tbl_.ColumnSet.Add(col_)

        col_.Name = "Epoha"
        tbl_.ColumnSet.Add(col_)

        col_.Name = "Razmera"
        tbl_.ColumnSet.Add(col_)

        col_ = Nothing
        pb1.Value = 2

        'da vidimo dali nesto moze uopsteno i odmah!

        'sada iskopiras sve iz pnt po...
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("pntSaS")
        qvr_.Text = "insert into [DKP_Tacka] ([Geom (I)],[brojtacke],[vrstatacke],[metoda],[izvor],[epoha],[razmera],[TkKod]) (select [Geom (I)],cstr([idTacke]),1,4,5,2012,4, case when [tipTacke]=1 then " & Chr(34) & "T20-06" & Chr(34) & " else " & Chr(34) & "T20-02" & Chr(34) & " end from [" & My.Settings.layerName_pointTableObelezavanje & "])"
        qvr_.RunEx(True)

        'ovde fale tacke koje se dobijaju iz klasa i kultura! to jos fali i onda je kompletno
        'ovo cemo po broju tacke ali to treba srediti zasebno! ovo sutra pa tek onda idemo da ga ubacimo!
        'ali mislim da ce biti ok

        doc.Save()

        MsgBox("Kraj")

    End Sub

    Private Sub DefinicijaGeodetskeMrezeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs)


        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        'za ulaz ti treba layer sa tackama poligone mreze - to vec imas u setings-ima
        Dim drwPoints As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_poligonskeTacke)
        'sada idemoda proverimo da li postoji ako postoji onda brisemo i ponovo  ga pravimo!

        Dim drwPnt As Manifold.Interop.Drawing

        pb1.Value = 0
        pb1.Maximum = 9

        'aj da vidimo kako da kreiram folder!? i da dodam dwg u njega!?
        Dim drwFolder As Manifold.Interop.Folder

        Try
            drwFolder = doc.ComponentSet("DKP_Komplet")
        Catch ex As Exception
            drwFolder = doc.NewFolder("DKP_Komplet")
        End Try

        Try
            drwPnt = doc.NewDrawing("DKP_TackaGeodetskeOsnove", drwPoints.CoordinateSystem, True)
        Catch ex As Exception
            doc.ComponentSet.Remove("DKP_TackaGeodetskeOsnove")
            drwPnt = doc.NewDrawing("DKP_TackaGeodetskeOsnove", drwPoints.CoordinateSystem, True)
        End Try

        drwPnt.Folder = drwFolder

        pb1.Value = 1
        'sada mozes polja da mu kreiras!

        Dim tbl_ As Manifold.Interop.Table = drwPnt.OwnedTable : Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn

        col_.Name = "MaticniBrojKo"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)

        col_.Name = "BrojTacke"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 20
        tbl_.ColumnSet.Add(col_)

        col_.Name = "VisinaTackeH"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        tbl_.ColumnSet.Add(col_)

        col_.Name = "TkKod"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 6
        tbl_.ColumnSet.Add(col_)

        col_.Name = "AzimutSimbola"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        tbl_.ColumnSet.Add(col_)

        col_.Name = "mx"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeFloat64
        tbl_.ColumnSet.Add(col_)

        col_.Name = "my"
        tbl_.ColumnSet.Add(col_)

        col_.Name = "mz"
        tbl_.ColumnSet.Add(col_)

        col_.Name = "Epoha"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
        tbl_.ColumnSet.Add(col_)

        col_.Name = "OpisPolozaja"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        col_.Size = 250
        tbl_.ColumnSet.Add(col_)

        col_ = Nothing
        pb1.Value = 2

        'da vidimo dali nesto moze uopsteno i odmah!

        'sada iskopiras sve iz pnt po...
        Dim qvr_ As Manifold.Interop.Query = doc.NewQuery("pntSaS")
        qvr_.Text = "insert into [DKP_Tacka] ([Geom (I)],[brojtacke],[vrstatacke],[metoda],[izvor],[epoha],[razmera],[TkKod]) (select [Geom (I)],cstr([idTacke]),1,4,5,2012,4, case when [tipTacke]=1 then " & Chr(34) & "T20-06" & Chr(34) & " else " & Chr(34) & "T20-02" & Chr(34) & " end from [" & My.Settings.layerName_pointTableObelezavanje & "])"
        qvr_.RunEx(True)

        'ovde fale tacke koje se dobijaju iz klasa i kultura! to jos fali i onda je kompletno
        'ovo cemo po broju tacke ali to treba srediti zasebno! ovo sutra pa tek onda idemo da ga ubacimo!
        'ali mislim da ce biti ok

        doc.Save()

        MsgBox("Kraj")
    End Sub
    Private Sub DopuniParceleUBaziZaBrojDetaljnogListaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DopuniParceleUBaziZaBrojDetaljnogListaToolStripMenuItem.Click
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Try
            sf_diag.FileName = ""
            sf_diag.DefaultExt = "map"
            sf_diag.Filter = "Manifold Map file (*.map)|*.map"
            sf_diag.FileName = "podelaNaListoveTable.map"
            sf_diag.Title = "Upisite naziv za izlazni Map File"
            sf_diag.ShowDialog()
            If sf_diag.FileName = "" Then
                MsgBox("Kraj operacije")
                doc = Nothing
                Exit Sub
            Else
                doc = frmMain.ManifoldCtrl.get_Document
                doc.SaveAs(sf_diag.FileName)
                frmMain.ManifoldCtrl.DocumentPath = sf_diag.FileName
            End If
            frmMain.ManifoldCtrl.Refresh()
        Catch ex As Exception
            MsgBox("Dokument je read onlyu Zatvorite ga u Manifoldu i ponovo pokrenite ovu funkciju.")
            FileClose()
            doc = Nothing
            Exit Sub
        End Try

        doc = frmMain.ManifoldCtrl.get_Document

        Dim drwParcele As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_ParceleNadela)
        Dim drwPodelaNaListove As Manifold.Interop.Drawing = doc.ComponentSet(My.Settings.layerName_podelaNaListove)

        'sada ti treba presek kao sa procembenim razredima!
        Dim i As Integer
        pb1.Value = 1
        Dim tbl_ As Manifold.Interop.Table

        Try

            Dim drw As Manifold.Interop.Drawing = doc.ComponentSet("podela_Table")

        Catch ex As Exception

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

        End Try


        Dim topPRazredi As Manifold.Interop.Topology = doc.Application.NewTopology
        topPRazredi.Bind(drwPodelaNaListove)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc.Application.NewTopology
        topParcele.Bind(drwParcele)
        topParcele.Build()

        Try
            topParcele.DoIntersect(topPRazredi, "podela_Table")
        Catch When Err.Number = -2147352567
            'sada treba pokrenuti normalizacju
            doc.ComponentSet.Remove("podela_Table")
            Dim analizer_ As Manifold.Interop.Analyzer = doc.NewAnalyzer
            analizer_.NormalizeTopology(drwParcele, drwParcele.ObjectSet)
            topParcele.Bind(drwParcele)
            topParcele.Build()
            topParcele.DoIntersect(topPRazredi, "podela_Table")
            analizer_ = Nothing
        Catch
            MsgBox(Err.Description)
        End Try

        'sada treba kreirati polje u drwparcele
        tbl_ = drwParcele.OwnedTable
        Dim col_ As Manifold.Interop.Column = doc.Application.NewColumnSet.NewColumn
        col_.Name = "brPlana"
        col_.Type = Manifold.Interop.ColumnType.ColumnTypeAText
        tbl_.ColumnSet.Add(col_)
        col_ = Nothing
        tbl_ = Nothing
        doc.Save()
        Dim qDodeli As Manifold.Interop.Query = doc.NewQuery("dodeli")
        'qDodeli.Text = "UPDATE (SELECT [" & My.Settings.layerName_table & "].[idTable],[" & My.Settings.layerName_table & "].[brLista],C.[NOMENKLATU] FROM [" & My.Settings.layerName_table & "], (SELECT B.[NOMENKLATU],A.[idTable] FROM ((SELECT max([Area (I)]) as P,[idTable] FROM [podela_Table] GROUP by [idTable]) as A, (SELECT [idTable],[NOMENKLATU],[Area (I)] as P1 FROM [podela_Table]) as B ) WHERE A.P=B.P1 and A.[idTable]=B.[idTable] ) as C WHERE [" & My.Settings.layerName_table & "].[idTable]=C.[idTable] ) set [brLista]=[NOMENKLATU]"
        qDodeli.Text = "UPDATE (SELECT [" & My.Settings.layerName_ParceleNadela & "].[brparcele],[" & My.Settings.layerName_ParceleNadela & "].[brplana],C.[NOMENKLATU] FROM [" & My.Settings.layerName_ParceleNadela & "], (SELECT B.[NOMENKLATU],A.[brparcele] FROM ((SELECT max([Area (I)]) as P,[brparcele] FROM [podela_Table] GROUP by [brparcele]) as A, (SELECT [brparcele],[NOMENKLATU],[Area (I)] as P1 FROM [podela_Table]) as B ) WHERE A.P=B.P1 and A.[brparcele]=B.[brparcele] ) as C WHERE [" & My.Settings.layerName_ParceleNadela & "].[brparcele]=C.[brparcele] ) set [brplana]=[NOMENKLATU]"
        qDodeli.RunEx(True)

        doc.ComponentSet.Remove("podela_Table")

        'sada bi odavde trebalo da ide pisanje u bazu a ne ovako!
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        'sada ti treba upis nad parcelama
        qDodeli.Text = "select brparcele,brplana from [" & My.Settings.layerName_ParceleNadela & "]"
        qDodeli.RunEx(True)
        pb1.Value = 0
        pb1.Maximum = qDodeli.Table.RecordSet.Count
        For i = 0 To qDodeli.Table.RecordSet.Count - 1
            pb1.Value = i
            Dim stsql_ As String = "update kom_kfmns set brplana='" & qDodeli.Table.RecordSet(i).DataText(2) & "' where brparcele='" & qDodeli.Table.RecordSet(i).DataText(1) & "'"
            conn_.Open()
            comm_.CommandText = stsql_
            comm_.ExecuteNonQuery()
            conn_.Close()
        Next

        doc.ComponentSet.Remove("dodeli")
        qDodeli = Nothing
        topParcele = Nothing : topPRazredi = Nothing
        pb1.Value = 0
        doc.Save()
        MsgBox("Kraj")
    End Sub

    Private Sub NumeracijaObjekataToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NumeracijaObjekataToolStripMenuItem.Click
        'radi numeraciju delova parcela u okviru objekta na osnovu selekcije!
        'u map file- treba da imas drawing sa sledecim poljima: brparcele,povrsina,deoparcele, nacinkoriscenja
        'ovaj drawing u podesavanjima ide kao da je u pitanju parcele nadela
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
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

    Private Sub ExportParceladelovaUBazuToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExportParceladelovaUBazuToolStripMenuItem.Click

        'aj da vidimo kako ovo ovde da uradimo!
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        'sada bi odavde trebalo da ide pisanje u bazu a ne ovako!
        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)

        'prvi korak bi trebao da bude da brise sve kod kojih je idpotesa gradevinski rejon!
        'prvi problem je sto je u dve baze razlicito definisan idpotesa gradevinskog rejona! - recimo da je nula sto je mozda bolje
        'resenje!
        conn_.Open()
        comm_.CommandText = "delete from kom_novostanjaparcela where idpotesa=0"
        comm_.ExecuteNonQuery()

        'sada mozes dalje odnosno da ides redom i ovu selekciju!


    End Sub

    Private Sub TO19PakovanjeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TO19PakovanjeToolStripMenuItem.Click
        'ovde kao ulaz mora da ide spisak tacaka i nista vise!!!! - to je pocetak
        'znaci ulaz je file sa id, x, y (mozda z) - ovo verovatno moze iz map file-a
        'On Error Resume Next
        'spisak tacaka dobijas iz map-a kao query
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
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

        pb1.Value = 0 : frmMain.pb1.Maximum = qvr_.Table.RecordSet.Count : frmMain.pb1.Value = 0

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
            frmMain.pb1.Value = i
        Next

        xlsSheet19 = Nothing
        xlsWB_ = Nothing
        xlsApp_.Quit()
        xlsApp_ = Nothing
        pb1.Value = 0
        frmMain.pb1.Value = 0
        MsgBox("Kraj")
    End Sub

    Private Function dopuna_nule(broj_ As Integer) As String

        Dim negativan_ As Boolean = False

        If broj_ < 0 Then
            negativan_ = True
            broj_ = Math.Abs(broj_)
        End If


        Dim p_ As String = "00" & broj_
        dopuna_nule = p_.Substring(p_.Length - 2)

        If negativan_ = True Then
            dopuna_nule = "-" & dopuna_nule
        End If

        Return dopuna_nule
    End Function

    Private Function upisi1obrazac(xls_ As Microsoft.Office.Interop.Excel.Worksheet, odakle_ As Integer, ugao_ As Double, stanica_ As String, brPoc_ As String, brKraj_ As String) As Integer

        'e sada je zanimljivo!
        xls_.Cells(odakle_, 12) = "0        0       0" : xls_.Cells(odakle_ + 1, 12) = formatirajUgaoZaPrikaz(ugao_)
        xls_.Cells(odakle_, 1) = stanica_ : xls_.Cells(odakle_ + 4, 1) = stanica_
        xls_.Cells(odakle_, 2) = brPoc_ : xls_.Cells(odakle_ + 5, 2) = brPoc_
        xls_.Cells(odakle_ + 1, 2) = brKraj_ : xls_.Cells(odakle_ + 4, 2) = brKraj_


        'ovo nece ovako!
        'idemo od kolone 11!
        'sada dajemo krajnje za giruse:
        Dim prviGirus, drugiGirus As Double

        Dim razlikaGirusi_ As Double = udec(CInt(Int((15 * Rnd()) + 0)) / 10000)

        prviGirus = ugao_ + razlikaGirusi_ : drugiGirus = ugao_ - razlikaGirusi_

        xls_.Cells(odakle_, 9) = "0       0" : xls_.Cells(odakle_ + 1, 9) = formirajUgaoZaPrikazBezStepeni(prviGirus)
        xls_.Cells(odakle_, 10) = "0       0" : xls_.Cells(odakle_ + 1, 10) = formirajUgaoZaPrikazBezStepeni(drugiGirus)

        'pakujemo kolonu 6!
        xls_.Cells(odakle_, 6) = "0        0       0" : xls_.Cells(odakle_ + 1, 6) = formatirajUgaoZaPrikaz(prviGirus)
        xls_.Cells(odakle_ + 4, 6) = "0        0       0" : xls_.Cells(odakle_ + 5, 6) = formatirajUgaoZaPrikaz(drugiGirus)

        'pakujemo kolonu 7 - kolimacione greske!

        Dim kolimaciona_, znak_ As Integer
        For j = 0 To 4 Step 4
            For i = 0 To 1
                znak_ = CInt(Int((2 * Rnd()) + 0))
                kolimaciona_ = CInt(Int((15 * Rnd()) + 2))

                If znak_ = 1 Then
                    xls_.Cells(odakle_ + i + j, 7) = "+"
                    xls_.Cells(odakle_ + i + j, 8) = kolimaciona_
                Else
                    xls_.Cells(odakle_ + i + j, 7) = "-"
                    xls_.Cells(odakle_ + i + j, 8) = kolimaciona_
                End If
            Next
        Next

        'sada idemo na kolonu sredina iz oba polozaja 5
        Dim pp1_ As Double = udec(CInt(Int((60 * Rnd()) + 1)) / 10000)
        xls_.Cells(odakle_, 5) = formatirajUgaoZaPrikaz(pp1_) : xls_.Cells(odakle_ + 1, 5) = formatirajUgaoZaPrikaz(prviGirus + pp1_)
        xls_.Cells(odakle_ + 2, 5) = formirajUgaoZaPrikazBezStepeni(pp1_ + prviGirus + pp1_)  'kontrola

        Dim pp2_ As Double = udec(90 + (CInt(Int((60 * Rnd()) + 1)) / 10000))
        xls_.Cells(odakle_ + 4, 5) = formatirajUgaoZaPrikaz(pp2_) : xls_.Cells(odakle_ + 5, 5) = formatirajUgaoZaPrikaz(drugiGirus + pp2_)
        xls_.Cells(odakle_ + 6, 5) = formirajUgaoZaPrikazBezStepeni(pp2_ + drugiGirus + pp2_)  'kontrola
        'sada idemo na kolone 3 i 4!

        'oni gore
        Dim greska_ = udec((xls_.Cells(odakle_, 8).value.ToString / 2) / 10000) : Dim ufznak_ As String = xls_.Cells(odakle_, 7).value.ToString : If ufznak_ = "-" Then greska_ = -greska_
        xls_.Cells(odakle_, 3) = formatirajUgaoZaPrikaz(pp1_ - greska_) : xls_.Cells(odakle_, 4) = formatirajUgaoZaPrikaz(pp1_ + greska_) 'prva kolimanciona

        greska_ = udec((xls_.Cells(odakle_ + 1, 8).value.ToString / 2) / 10000) : ufznak_ = xls_.Cells(odakle_ + 1, 7).value.ToString : If ufznak_ = "-" Then greska_ = -greska_
        xls_.Cells(odakle_ + 1, 3) = formatirajUgaoZaPrikaz(prviGirus + pp1_ - greska_) : xls_.Cells(odakle_ + 1, 4) = formatirajUgaoZaPrikaz(prviGirus + pp1_ + greska_)

        ' sad moze da predes na one dole!
        greska_ = udec((xls_.Cells(odakle_ + 4, 8).value.ToString / 2) / 10000) : ufznak_ = xls_.Cells(odakle_ + 4, 7).value.ToString : If ufznak_ = "-" Then greska_ = -greska_
        xls_.Cells(odakle_ + 4, 3) = formatirajUgaoZaPrikaz(pp2_ - greska_) : xls_.Cells(odakle_ + 4, 4) = formatirajUgaoZaPrikaz(pp2_ + greska_) 'treca kolimanciona
        'kontrola

        greska_ = udec((xls_.Cells(odakle_ + 5, 8).value.ToString / 2) / 10000) : ufznak_ = xls_.Cells(odakle_ + 5, 7).value.ToString : If ufznak_ = "-" Then greska_ = -greska_
        xls_.Cells(odakle_ + 5, 3) = formatirajUgaoZaPrikaz(drugiGirus + pp2_ - greska_) : xls_.Cells(odakle_ + 5, 4) = formatirajUgaoZaPrikaz(drugiGirus + pp2_ + greska_) 'cetvrta kolimanciona
        'kontrola
        'sada mozemo kontrole da sprovedemo!

        'sada ti treba da uradis kontrolu za 3 4 i 6
        Dim zaon_
        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_, 3).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 1, 3).value.ToString)
        xls_.Cells(odakle_ + 2, 3) = formirajUgaoZaPrikazBezStepeni(zaon_)
        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_, 4).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 1, 4).value.ToString)
        xls_.Cells(odakle_ + 2, 4) = formirajUgaoZaPrikazBezStepeni(zaon_)
        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_, 6).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 1, 6).value.ToString)
        xls_.Cells(odakle_ + 2, 6) = formirajUgaoZaPrikazBezStepeni(zaon_)
        'vracas dokle si stigao

        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 4, 3).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 5, 3).value.ToString)
        xls_.Cells(odakle_ + 6, 3) = formirajUgaoZaPrikazBezStepeni(zaon_)
        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 4, 4).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 5, 4).value.ToString)
        xls_.Cells(odakle_ + 6, 4) = formirajUgaoZaPrikazBezStepeni(zaon_)
        zaon_ = vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 4, 6).value.ToString) + vratiUgaoIzFormataUDec(xls_.Cells(odakle_ + 5, 6).value.ToString)
        xls_.Cells(odakle_ + 6, 6) = formirajUgaoZaPrikazBezStepeni(zaon_)

        odakle_ += 8
        Return odakle_
    End Function

    Private Function formatirajUgaoZaPrikaz(ugao_ As Double) As String

        Dim a_ = Math.Round(uste(ugao_), 4) & "0"

        formatirajUgaoZaPrikaz = Fix(a_) & "    " & Mid(a_, InStr(a_, ".") + 1, 2) & "    " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return formatirajUgaoZaPrikaz
    End Function

    Private Function formirajUgaoZaPrikazBezStepeni(ugao_ As Double) As String
        Dim a_ = Math.Round(uste(ugao_), 4) & "0"

        formirajUgaoZaPrikazBezStepeni = Mid(a_, InStr(a_, ".") + 1, 2) & "    " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return formirajUgaoZaPrikazBezStepeni
    End Function

    Private Function vratiUgaoIzFormataUDec(ugao_ As String) As Double

        Dim a_ = ugao_.Split(" ")
        Dim b_(2)
        'sada mi treba novi da g
        Dim j = 0
        For i = 0 To a_.Length - 1
            If a_(i) <> "" Then
                b_(j) = a_(i)
                j = j + 1
            End If
        Next

        vratiUgaoIzFormataUDec = udec(b_(0) & "." & b_(1) & b_(2))

        Return vratiUgaoIzFormataUDec
    End Function

    Private Sub PovrsineNaPovrsineIzDKPaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PovrsineNaPovrsineIzDKPaToolStripMenuItem.Click

        If MsgBox("Pegla povrsine na povrsine iz DKP-a: Moras da imas manifold otvoren i u njemu themu layerName_parcele / podesavanje, a u okviru theme dva polja: brparcelef i povrsina, u bazi gada fs_parcele", MsgBoxStyle.OkCancel, "Peglanje povrsina na DKP") = MsgBoxResult.Ok Then
            'sada mozemo da radimo
            Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : conn_.Open()
            Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
            Dim doc_ As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

            ' Dim drw_ As Manifold.Interop.Drawing = doc_.ComponentSet(My.Settings.layerName_parcele)

            'ok mozemo sada da idemo redom napravimo query - broj parcele i povrsina
            Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("povi")
            qvr_.Text = "select brparcelef,povrsina from [" & My.Settings.layerName_parcele & "]"
            qvr_.RunEx(True)

            pb1.Value = 0
            pb1.Maximum = qvr_.Table.RecordSet.Count

            For i = 0 To qvr_.Table.RecordSet.Count - 1
                'sada za svaku parcelu idemo redom: imas glupave slucajeve!
                comm_.CommandText = "select count(*) from fs_parcele where obrisan=0 and brparcelef='" & qvr_.Table.RecordSet.Item(i).DataText(1) & "'"
                'sada je problem gde ovo smestiti ako ide u reader onda ima problem ako ne ide onda nema!
                Dim read_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)

                read_.Read()
                Dim koliko_ As Integer = read_.GetValue(0)

                read_.Close()
                conn_.Open()
                'razlikujemo razlicite slucajeve



                If koliko_ = 2 Then
                    'ovo je idealan slucaj jer onda povrsinu zatvaras bez problema! imas jednostavan update osnovne i pod parcele
                    'parsiraj povrisnu parcele
                    Dim g_ As Integer = qvr_.Table.RecordSet.Item(i).DataText(2)
                    Dim met_ As Integer = g_ - Fix(g_ / 100) * 100 'sada imas povrsinu 
                    Dim ar_ = Fix(g_ / 100)
                    Dim ari_ As Integer = ar_ - Fix(ar_ / 100) * 100
                    'sada ostaje jos samo hektari
                    Dim he_ As Integer = Fix(ar_ / 100)
                    'pises he_,ari_ i met_
                    'sa ovim querijem menja oba!
                    comm_.CommandText = "update fs_parcele set hektari=" & he_ & ", ari=" & ari_ & ", metri=" & met_ & " where brparcelef='" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' and obrisan=0"
                    comm_.ExecuteNonQuery()

                Else

                    'sta sada radis? najbolje da razliku odbije od nekog dela koji je njiva ili tako nesto jer ako je 360- sta onda@!? ili da pocepas ravnomerno po delovima?
                    'to mozda i nije lose!?
                    Dim g_ As Integer = qvr_.Table.RecordSet.Item(i).DataText(2)
                    'sada mozes da poredis ove dve povrsine i da podjednako napravis update za sve njih! 

                    comm_.CommandText = "select (hektari*10000+ari*100+metri)-(" & g_ & ") from fs_parcele where deoparcele=0 and obrisan=0 and brparcelef='" & qvr_.Table.RecordSet.Item(i).DataText(1) & "'"
                    Dim read2_ As MySql.Data.MySqlClient.MySqlDataReader = comm_.ExecuteReader(CommandBehavior.CloseConnection)
                    read2_.Read()
                    Dim povrsina As Integer = read2_.GetValue(0)
                    read2_.Close()
                    conn_.Open()

                    g_ = povrsina / (koliko_ - 1)
                    Dim met_ As Integer = g_ - Fix(g_ / 100) * 100 'sada imas povrsinu 
                    Dim ar_ = Fix(g_ / 100)
                    Dim ari_ As Integer = ar_ - Fix(ar_ / 100) * 100
                    'sada ostaje jos samo hektari
                    Dim he_ As Integer = Fix(ar_ / 100)
                    'sada za ovo napravis update !
                    comm_.CommandText = " update fs_parcele set hektari=hektari-" & he_ & ", ari=IF((METRI-" & met_ & ")<0,ARI-1-" & ari_ & ",ARI-" & ari_ & "), metri=IF((METRI-" & met_ & ")<0,METRI+100-" & met_ & ",METRI-" & met_ & ") where obrisan=0 and deoparcele=1 and brparcelef='" & qvr_.Table.RecordSet.Item(i).DataText(1) & "'"
                    comm_.ExecuteNonQuery()
                    'sada ides na sumu da bu napravio update za deoparcele=0 
                    comm_.CommandText = "UPDATE fs_parcele INNER JOIN ( SELECT TRUNCATE (ukupno_povrsina / 10000, 0) AS hektari_, TRUNCATE (( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100, 0 ) ari_, round((( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100 - TRUNCATE (( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100, 0 )) * 100 ) metri_, '" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' AS brparcelef FROM ( SELECT sum( hektari * 10000 + ari * 100 + metri ) AS ukupno_povrsina FROM fs_parcele WHERE DEOPARCELE = 1 AND obrisan = 0 AND brParceleF = '" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' ) AS A ) AS B ON fs_parcele.brparcelef = B.brparcelef SET hektari = HEKTARI_, ari = ari_, metri = metri_ WHERE fs_parcele.deoparcele = 0"
                    comm_.ExecuteNonQuery()

                    'sada moze da se dogodo da je zbog zaokruzivanja ovde nastane problem! 
                    'to cemo da resimo tako sto poslednjem delu koji je tipa njiva ili nesto slicno dodamo razliku i opet saberemo povrsine!
                    comm_.CommandText = "select (hektari*10000+ari*100+metri)-(" & qvr_.Table.RecordSet.Item(i).DataText(2) & ") from fs_parcele where deoparcele=0 and obrisan=0 and brparcelef='" & qvr_.Table.RecordSet.Item(i).DataText(1) & "'"
                    read2_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)
                    read2_.Read()
                    povrsina = read2_.GetValue(0)
                    read2_.Close()
                    conn_.Open()
                    read2_ = Nothing

                    'kolika god da je oduzimas od metara!
                    comm_.CommandText = "UPDATE fs_parcele INNER JOIN ( SELECT idParc FROM fs_parcele WHERE brParceleF = '" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' AND DEOPARCELE = 1 ORDER BY SKULTURE LIMIT 1 ) AS A ON fs_parcele.idparc = A.idparc SET metri = metri - " & povrsina
                    comm_.ExecuteNonQuery()
                    comm_.CommandText = "UPDATE fs_parcele INNER JOIN ( SELECT TRUNCATE (ukupno_povrsina / 10000, 0) AS hektari_, TRUNCATE (( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100, 0 ) ari_, round((( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100 - TRUNCATE (( ukupno_povrsina - TRUNCATE (ukupno_povrsina / 10000, 0) * 10000 ) / 100, 0 )) * 100 ) metri_, '" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' AS brparcelef FROM ( SELECT sum( hektari * 10000 + ari * 100 + metri ) AS ukupno_povrsina FROM fs_parcele WHERE DEOPARCELE = 1 AND obrisan = 0 AND brParceleF = '" & qvr_.Table.RecordSet.Item(i).DataText(1) & "' ) AS A ) AS B ON fs_parcele.brparcelef = B.brparcelef SET hektari = HEKTARI_, ari = ari_, metri = metri_ WHERE fs_parcele.deoparcele = 0"
                    comm_.ExecuteNonQuery()



                End If

                read_ = Nothing
                pb1.Value = i

            Next
            doc_ = Nothing
            comm_ = Nothing
            conn_.Close()
            conn_ = Nothing
            MsgBox("Kraj")
        End If



    End Sub

    Private Sub LinijeParceleDeoParceleVsGranicnaLinijaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles LinijeParceleDeoParceleVsGranicnaLinijaToolStripMenuItem.Click

        'idemo redom prvo kontrolisemo ono sto sam opisa:
        'da sve linije moraju da budu unutra - linije iz parcela i delova parcela u granicnimlinijama!
        Cursor = Cursors.WaitCursor
        Dim doc_ As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Dim drwGranicneLinije As Manifold.Interop.Drawing

        Try
            drwGranicneLinije = doc_.ComponentSet("GranicnaLinija")
        Catch ex As Exception

            MsgBox("Nepostoji GranicnaLinija drawing")
            Exit Sub
        End Try

        Try
            doc_.ComponentSet.Remove("K1Greske")
        Catch ex As Exception
        End Try

        Try
            doc_.ComponentSet.Remove("KontrolaLinija")
        Catch ex As Exception
        End Try

        Try
            'doc_.ComponentSet.Remove("KontrolaLinija")
        Catch ex As Exception

        End Try

        Dim drwNew_ As Manifold.Interop.Drawing = doc_.NewDrawing("kontrolaLinija", drwGranicneLinije.CoordinateSystem, True)

        Dim qvr_ As Manifold.Interop.Query = doc_.NewQuery("pokrime")
        qvr_.Text = "OPTIONS  COORDSYS(" & Chr(34) & "deoparcele" & Chr(34) & " as COMPONENT); insert  into [kontrolaLinija] ([Geom (I)]) (SELECT DISTINCT line_ FROM [Parcela] split by Branches(IntersectLine(ConvertToLine([Geom (I)]), ConvertToLine([Geom (I)]))) as line_ ) Union (SELECT DISTINCT line_  FROM [DeoParcele] split by  Branches(IntersectLine(ConvertToLine([Geom (I)]), ConvertToLine([Geom (I)]))) as line_)"
        qvr_.RunEx(True)

        'sada mozes remove duplicates
        Dim anal_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
        anal_.RemoveDuplicates(drwNew_, drwNew_.ObjectSet)

        doc_.ComponentSet.Remove("pokrime")
        'drwNew_.SelectNone()
        'sledeci korak je selekcija

        qvr_ = doc_.NewQuery("selectkKK")

        qvr_.Text = "update [GranicnaLinija] set [Selection (I)]=true where [ID] in (SELECT [GranicnaLinija].[ID] FROM [kontrolaLinija],[GranicnaLinija] WHERE [kontrolaLinija].[Geom (I)]=[GranicnaLinija].[Geom (I)])"
        qvr_.RunEx(True)


        drwGranicneLinije.SelectInverse()


        If drwGranicneLinije.Selection.Count = 0 Then
            MsgBox("Kotrola linija je prosla - nema gresaka")
        Else
            Dim drwGreskeK1 As Manifold.Interop.Drawing = doc_.NewDrawing("K1Greske", drwGranicneLinije.CoordinateSystem, True)
            MsgBox("Kontrola linija nije prosla gresne linije su u drawingu K1Greske")
            drwGranicneLinije.Copy(True)
            drwGreskeK1.Paste(True)
            doc_.Save()
            drwGreskeK1 = Nothing
        End If
        doc_.ComponentSet.Remove("selectkKK")
        'sada mozes da izades iz programa mada mozes da ides na tacke- ali bi trebalo da su tacke ok jer si ih sredio ranije

        'idemo na sledecu odnosno intersect
        Dim qvrInter As Manifold.Interop.Query = doc_.NewQuery("lineIntersect")
        qvrInter.Text = "update [GranicnaLinija] set [Selection (I)]=true where [ID] in (SELECT A.[ID] FROM [GranicnaLinija] as A, [GranicnaLinija] as  B WHERE Intersects(A.[Geom (I)],B.[Geom (I)]) and  A.[ID]<>B.[ID])"
        qvrInter.RunEx(True)
        If drwGranicneLinije.Selection.Count = 0 Then
            MsgBox("Nema linija koje se seku - ova kontrola je prosla")
        Else
            MsgBox("Ima linija koje se seku - proverite u drawingu ")
            Dim drwInt As Manifold.Interop.Drawing = doc_.NewDrawing("kontrolaIntersect", drwGranicneLinije.CoordinateSystem, True)
            drwGranicneLinije.Copy(True)
            drwInt.Paste(True)
            drwInt = Nothing
            doc_.Save()
        End If
        doc_.ComponentSet.Remove("lineIntersect")

        'sada mozemo na tacke!?
        'ista procedura!

        doc_.Save()

        MsgBox("Kraj")
        Cursor = Cursors.Default
    End Sub

    Private Sub TackeParceleDeoParceleVsTackeToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles TackeParceleDeoParceleVsTackeToolStripMenuItem.Click
        Cursor = Cursors.WaitCursor
        Dim doc_ As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

        Dim drwTacka, drwParc, drwParcDeo As Manifold.Interop.Drawing

        Try
            drwTacka = doc_.ComponentSet("Tacka")
        Catch ex As Exception
            MsgBox("Nepostoji ParcDelParcTacke drawing")
            Cursor = Cursors.Default
            Exit Sub
        End Try

        Try
            drwParc = doc_.ComponentSet("Parcela")
        Catch ex As Exception
            MsgBox("Drawing parcele ne postoji")
            Cursor = Cursors.Default
            Exit Sub
        End Try

        Try
            drwParcDeo = doc_.ComponentSet("DeoParcele")
        Catch ex As Exception
            MsgBox("Drawing deoparcele ne postoji")
            Cursor = Cursors.Default
            Exit Sub
        End Try

        Try
            doc_.ComponentSet.Remove("K1Greske")
        Catch ex As Exception
        End Try

        Try
            doc_.ComponentSet.Remove("kontrolaTacke")
        Catch ex As Exception
        End Try

        Try
            'doc_.ComponentSet.Remove("KontrolaLinija")
        Catch ex As Exception

        End Try

        Dim drwNew_ As Manifold.Interop.Drawing = doc_.NewDrawing("kontrolaTacke", drwTacka.CoordinateSystem, True)

        Dim anal_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer

        'pa idemo iz parcela u tacke
        Dim objSet_ As Manifold.Interop.ObjectSet
        objSet_ = anal_.Points(drwParc, drwParc, drwNew_.ObjectSet)
        objSet_ = anal_.Points(drwParcDeo, drwParcDeo, drwNew_.ObjectSet)

        anal_.RemoveDuplicates(drwNew_, drwNew_.ObjectSet)

        anal_ = Nothing

        doc_.Save()

        MsgBox("Kraj")
        Cursor = Cursors.Default
    End Sub

    Private Sub TO18EToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles TO18EToolStripMenuItem.Click
        'ovde kao ulaz mora da ide spisak tacaka i nista vise!!!! - to je pocetak
        'znaci ulaz je file sa id, x, y (mozda z) - ovo verovatno moze iz map file-a
        'On Error Resume Next
        'spisak tacaka dobijas iz map-a kao query
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
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

        pb1.Value = 0 : frmMain.pb1.Maximum = qvr_.Table.RecordSet.Count : frmMain.pb1.Value = 0
        Dim brojac3ki_ As Integer = 1

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            frmMain.pb1.Value = i

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
        frmMain.pb1.Value = 0
        MsgBox("Kraj")

    End Sub

    Private Sub KontrolaPARKORGLIVLIRGZToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KontrolaPARKORGLIVLIRGZToolStripMenuItem.Click

        'potrebno je da znam koje su ovo tabele? - kako cemo ovo?
        Dim nazPar_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03PAR")
        Dim nazKOR_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03KOR")
        Dim nazGLI_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03GLI")
        Dim nazVLI_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03VLI")
        Dim nazLica_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03LICA")
        ' Dim nazPar_ As String = InputBox("uneiste naziv za PAR bazu", "Unos podataka", "ADO03PAR")
        'pretpostavka je da su parcele ulica i ppotesa u tabelama ulice i potesi

        'kotrola PAR

        Dim freefile_ As Integer = FreeFile()

        sf_diag.FileName = "" : sf_diag.Filter = "Text Files | *.txt" : sf_diag.DefaultExt = "txt" : sf_diag.ShowDialog()

        If sf_diag.FileName = "" Then Exit Sub

        FileOpen(freefile_, sf_diag.FileName, OpenMode.Output, OpenAccess.Write, OpenShare.Shared)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
        Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM " & nazPar_ & " WHERE cast(brojparc AS UNSIGNED) = 0 OR ( cast(podbroj AS UNSIGNED) = 0 AND podbroj <> '000' ) OR brojparc IS NULL OR podbroj IS NULL OR length(brojparc) <> 5 OR length(podbroj) <> 3"
        Dim header_ As String = " Oznaka za broj i podbroj parcele.................................Par / 001"

        Dim mysqlreader_ As MySql.Data.MySqlClient.MySqlDataReader
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)
        'mysqlreader_.Read()

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM " & nazPar_ & " WHERE brposlis IS NULL OR cast(brposlis AS UNSIGNED) = 0 OR LENGTH(brposlis) <> 5"
        header_ = " Oznaka za broj B_lista ..........................................Par / 002"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM " & nazPar_ & " WHERE (skica = '' OR skica IS NULL) AND (cast(god AS UNSIGNED) <> 0) OR brojplana IS NULL OR brojplana = ''"
        header_ = "Oznaka za plan, skicu/god. ili manual/god........................Par / 004"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)


        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM ( SELECT brojparc, podbroj, ulicapotes, " & nazPar_ & ".sulice, 'ulica' FROM " & nazPar_ & " LEFT OUTER JOIN ulice ON " & nazPar_ & ".sulice = ulice.sulice WHERE ulicapotes = 1 ) AS A WHERE sulice IS NULL UNION SELECT * FROM ( SELECT brojparc, podbroj, ulicapotes, " & nazPar_ & ".sulice, 'potes' FROM " & nazPar_ & " LEFT OUTER JOIN potesi ON " & nazPar_ & ".sulice = potesi.spotesa WHERE ulicapotes = 9 ) AS A WHERE sulice IS NULL"
        header_ = " Oznaka i {ifra ulice ili potesa..................................Par / 006"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)


        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2) & " , " & mysqlreader_.GetValue(3) & " ," & mysqlreader_.GetValue(4))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM " & nazPar_ & " WHERE length(hektari) <> 5 AND length(ari) <> 3 AND length(metri) <> 3 AND ( cast(hektari AS UNSIGNED) * 10000 + cast(ari AS UNSIGNED) * 100 + cast(metri AS UNSIGNED)) <= 0"
        header_ = " Oznaka za povr{inu parcele (hektari,ari,metri)...................Par / 008"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)


        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM ( SELECT " & nazPar_ & ".*, NAZIV FROM " & nazPar_ & " LEFT OUTER JOIN kat_gradjevi ON " & nazPar_ & ".ggradjzem = kat_gradjevi.SIFRA ) A WHERE naziv IS NULL"
        header_ = " Oznaka za gra|evinsko zemqi{te...................................Par / 010"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)


        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM ( SELECT * FROM " & nazPar_ & " LEFT OUTER JOIN kat_kultura ON " & nazPar_ & ".skulture = kat_kultura.idKulture ) A WHERE nazivkult IS NULL"
        header_ = "[ifra na~ina kori{}ewa zemqi{ta (kultura)........................Par / 011"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)


        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, brstavke FROM " & nazPar_ & " WHERE brstavke = 0 OR brstavke = '' OR brstavke IS NULL"
        header_ = " Oznaka za broj stavke na parceli.................................Par / 013"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        'sada idemo na KOR ___________________________________________________________________________________________
        connComm.CommandText = "SELECT prezime, imeoca, ime FROM " & nazKOR_ & " WHERE brposlis = '' OR brposlis IS NULL OR brposlis = 0 OR cast(brposlis AS UNSIGNED) = 0"
        header_ = "Oznaka za broj B_lista...........................................Kor / 015"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM " & nazKOR_ & " WHERE prezime IS NULL OR prezime = ''"
        header_ = " Oznaka za prezime ili naziv pravnog lica.........................Kor / 017"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM " & nazKOR_ & " WHERE mesto IS NULL OR mesto = ''"
        header_ = " Oznaka za mesto stanovawa ili sedi{te pravnog lica...............Kor / 018"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " WHERE ( cast(broj AS UNSIGNED) = 0 AND uzbroj <> '' ) OR ( cast(broj AS UNSIGNED) = 0 AND LENGTH(broj) > 0 )"
        header_ = "  Oznaka za ku}ni broj i podbroj...................................Kor / 019"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " WHERE cast(matbrgra AS UNSIGNED) = 0 OR matbrgra = '' OR matbrgra IS NULL OR LENGTH(matbrgra) > 13"
        header_ = " Oznaka za mati~ni broj gra|ana ili pravnog lica..................Kor / 020"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " WHERE ( cast(brojilac AS UNSIGNED) > cast(imenilac AS UNSIGNED)) AND ( obimprava <> 4 AND obimprava <> 1 )"
        header_ = " Oznaka za udeo (brojilac/imenilac)...............................Kor / 021 -- kontrola 1"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " WHERE ( cast(brojilac AS UNSIGNED) > cast(imenilac AS UNSIGNED)) AND ( obimprava <> 4 AND obimprava <> 1 )"
        header_ = " Oznaka za udeo (brojilac/imenilac)...............................Kor / 021 -- kontrola 1"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " WHERE obimprava = 4 AND brojilac = '0000Z' AND imenilac = '0000S'"
        header_ = " Oznaka za udeo (brojilac/imenilac)...............................Kor / 021 -- kontrola 2"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM " & nazKOR_ & " where obimprava=1 and (brojilac<>'00001' or imenilac<>'00001')"
        header_ = " Oznaka za udeo (brojilac/imenilac)...............................Kor / 021 -- kontrola 3"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM ( SELECT * FROM " & nazKOR_ & " LEFT OUTER JOIN kat_svojina ON " & nazKOR_ & ".ds_ps = kat_svojina.SIFRA ) A WHERE naziv IS NULL"
        header_ = " Oznaka za vrstu svojine..........................................Kor / 022"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brposlis, sifralica, matbrgra FROM ( SELECT * FROM " & nazKOR_ & " LEFT OUTER JOIN kat_pravovrsta ON " & nazKOR_ & ".vrstaprava = kat_pravovrsta.SIFRA ) A WHERE naziv IS NULL"
        header_ = " Oznaka za vrstu svojine..........................................Kor / 024 Kontrola 1 "

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT ds_ps, vrstaprava FROM " & nazKOR_ & " WHERE vrstaprava = 5 AND ds_ps <> 2"
        header_ = " Oznaka za vrstu svojine..........................................Kor / 024 Kontrola 2"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM ( SELECT * FROM " & nazKOR_ & " LEFT OUTER JOIN kat_pravoobim ON " & nazKOR_ & ".obimprava = kat_pravoobim.SIFRA ) A WHERE naziv IS NULL"
        header_ = " Oznaka za vrstu svojine..........................................Kor / 025"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT * FROM ( SELECT * FROM " & nazKOR_ & " LEFT OUTER JOIN kat_nosiocip ON " & nazKOR_ & ".sifralica = kat_nosiocip.SIFRA ) A WHERE naziv IS NULL"
        header_ = "Oznaka za {ifru lica.............................................Kor/ 026"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Vlasnik: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        ' -- IDEMO ZA VLI SADA JE PAR I KOR GOTOVO

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br FROM " & nazVLI_ & " WHERE cast(brojparc AS UNSIGNED) = 0 OR ( cast(podbroj AS UNSIGNED) = 0 AND podbroj <> '000' ) OR brojparc IS NULL OR podbroj IS NULL OR length(brojparc) <> 5 OR length(podbroj) <> 3"
        header_ = " Oznaka za broj i podbroj parcele.................................Vli / 028"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br, nacinkor, evidencija FROM " & nazVLI_ & " WHERE ( IF ( nacinkor = 3001 OR nacinkor = 3002 OR nacinkor = 3003, 1, 0 ) - evidencija ) <> 0"
        header_ = "Oznaka za objekat ili poseban deo objekta........................Vli / 029"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br FROM " & nazVLI_ & " WHERE zk_br = 0 OR zk_br IS NULL"
        header_ = " Oznaka za redni broj objekta.....................................Vli / 030"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br FROM " & nazVLI_ & " WHERE pstatuso = 9 AND nacinkor NOT IN ( '10040', '10050', '10049', '10059', '20103' )"
        header_ = " Oznaka za pravni status objekta..................................Vli / 031 -- Kontrola 1"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br FROM " & nazVLI_ & " LEFT OUTER JOIN kat_objektiosnovizg ON " & nazVLI_ & ".pstatuso = kat_objektiosnovizg.SIFRA WHERE OSNOVIZG IS NULL"
        header_ = " Oznaka za pravni status objekta..................................Vli / 031 -- Kontrola 2"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        connComm.CommandText = "SELECT brojparc, podbroj, zk_br FROM " & nazVLI_ & " WHERE pstatuso <> 0 AND nacinkor IN ('3001', '3002', '3003')"
        header_ = " Oznaka za pravni status objekta..................................Vli / 031 -- Kontrola 3"

        conn_.Open()
        mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)

        If mysqlreader_.HasRows Then
            'printas gresku
            PrintLine(freefile_, header_)
            While mysqlreader_.Read
                'sada mozes da printas
                PrintLine(freefile_, "Parcela: " & mysqlreader_.GetValue(0) & "/" & mysqlreader_.GetValue(1) & " deo>" & mysqlreader_.GetValue(2))
            End While
        Else
            'znaci nema greske
        End If
        mysqlreader_.Close()

        conn_.Close() : FileClose()
        mysqlreader_ = Nothing : connComm = Nothing : conn_ = Nothing

        MsgBox("Kraj")
    End Sub

    Private Sub UpdatePromeneBrparcelefOldPLNewPLToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles UpdatePromeneBrparcelefOldPLNewPLToolStripMenuItem.Click

        Dim msg_ = "Funckija vrsi promene vlasnistva nad parcelom tako sto seli iz jednog Ln u drugi LN. Potrebna tabela sa sledecim poljima: brparcelef, oldLN, newLN koja se zove fs_promeneLNFS"

        If MsgBox(msg_, MsgBoxStyle.OkCancel, "Pitanje") = MsgBoxResult.Ok Then
            'sada mozemo dalje

            Cursor = Cursors.WaitCursor

            Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString)
            Dim connComm As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
            conn_.Open()

            connComm.CommandText = "select brparcelef, oldln, newLN from fs_promeneLNFS"

            Dim mysqlreader_ As MySql.Data.MySqlClient.MySqlDataReader
            mysqlreader_ = connComm.ExecuteReader(CommandBehavior.CloseConnection)
            'mysqlreader_.Read()
            Dim sqlTODO(-1) As String
            Dim brojac_ As Integer = 0

            If mysqlreader_.HasRows Then
                'printas gresku
                While mysqlreader_.Read
                    'sada mozes da napravis novi sql i da izvrsis potreban update
                    ReDim Preserve sqlTODO(brojac_)
                    sqlTODO(brojac_) = "insert into fs_vezaparcelavlasnik (idparcele, idvlasnika, obliksvojine, vrstaprava, obimprava, udeo, uneo,idpl,obrisan,koefudeo) SELECT distinct (select idparc FROM fs_parcele where DEOPARCELE=0 and brParceleF=" & Chr(34) & mysqlreader_.GetValue(0) & Chr(34) & " ) as idparc , idVlasnika, OBLIKSVOJINE, VRSTAPRAVA, OBIMPRAVA, Udeo, uneo, idPL, 0,koefUdeo FROM fs_vezaparcelavlasnik where idPL=" & mysqlreader_.GetValue(2)
                    brojac_ += 1
                    ReDim Preserve sqlTODO(brojac_)
                    sqlTODO(brojac_) = "update fs_vezaparcelavlasnik set obrisan=1 where idpl=" & mysqlreader_.GetValue(1).ToString & " and idparcele=(select idparc FROM fs_parcele where DEOPARCELE=0 and brParceleF=" & Chr(34) & mysqlreader_.GetValue(0) & Chr(34) & ")"
                    brojac_ += 1
                End While
            Else
                'znaci nema greske
            End If
            mysqlreader_.Close()
            mysqlreader_ = Nothing

            conn_.Open()
            For i = 0 To sqlTODO.Length - 1
                connComm.CommandText = sqlTODO(i)
                connComm.ExecuteNonQuery()
            Next

            conn_.Close()
            conn_ = Nothing
            connComm = Nothing

            MsgBox("Kraj")
            Cursor = Cursors.Default
        End If


    End Sub

    Private Sub KontrolaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles KontrolaToolStripMenuItem.Click
        'trebaju ti dve stvari : layeru kome se nalaze definisane table 
        'iz baze ti treba broj lista-ovo bi trebalo da se doda
        Dim doc As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document

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

        frmMain.lbl_infoMain.Text = "Ispisivanje listinga tacaka po parcelama u file"
        My.Application.DoEvents()

        Dim x(-1), y(-1) As Double
        Dim brojzaokruzivanje As Integer = My.Settings.zaokruzivanjeBrojDecMesta

        For i = 0 To qvr_.Table.RecordSet.Count - 1

            pb1.Value = i
            Try
                frmMain.lbl_infoMain.Text = "Ispisivanje listinga tacaka po parcelama u file. Obrada parcele broj = " & qvr_.Table.RecordSet.Item(i).DataText(4)
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

    Private Sub UpisiDeobeParcelaGranicaGradevinskogReonaToolStripMenuItem_Click(sender As Object, e As System.EventArgs) Handles UpisiDeobeParcelaGranicaGradevinskogReonaToolStripMenuItem.Click

        'ulaz:
        'dkp - drawing sa parcelama - ovo ide u layerName_parcele
        'csv file u formatu brojstareparcele, brojnoveparcelegradevinski, brojnoveparcelekomasacija 
        'baza radi citanja podataka starih parcela

        'upisje nove parcele u kom_parcele i kom_vezaparcelavlasnik i brise stare parcele iz istih baza

        opf_diag.FileName = ""
        opf_diag.ShowDialog()
        If opf_diag.FileName = "" Then MsgBox("Pronadite csv file") : Exit Sub

        'proverimo da li postoji drawing

        Dim doc_ As Manifold.Interop.Document = frmMain.ManifoldCtrl.get_Document
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

            'sada mi iz baze za treba povrsina za ovaj broj parcele
            connComm.CommandText = "select idparc, (hektari*10000+ari*100+metri) from kom_parcele where deoparcele=0 and obrisan=0 and brparcelef='" & a_(0) & "')"
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
                    Dim stsql_ = "INSERT into kom_parcele (SKATOPST, BROJPARC, PODBROJ, DEOPARCELE, BROJPLANA, SKICA, god, ULICAPOTES, SULICE, BROJ, UZBROJ, POTES, HEKTARI, ARI, METRI, idGradevinsko, MANUAL, BROJPOSLISTA, RASPRAVNIZAPISNIK, UKOMASACIJI, uneo, datumUnosa, brParceleF, obrisan) SELECT SKATOPST, " & parc_(0) & ", " & If(parc_.Length = 1, 0, parc_(1)) & ", DEOPARCELE, BROJPLANA, SKICA, god, ULICAPOTES, SULICE, BROJ, UZBROJ, POTES, " & hek_ & ", " & ari_ & ", " & met_ & ", idGradevinsko, MANUAL, BROJPOSLISTA, RASPRAVNIZAPISNIK, UKOMASACIJI, 2, datumUnosa, " & If(parc_.Length = 2, parc_(0) & "/" & parc_(1), parc_(0)) & ", 0 FROM kom_parcele WHERE brParceleF = " & Chr(34) & a_(0) & Chr(34) & " AND DEOPARCELE = 0"
                    connComm.CommandText = stsql_
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
                    'sada idemo dalje - odnosno mozemo da predemo na kom_tablevlasnik
                    stsql_ = "insert into kom_vezaparcelavlasnik (idParcele, idVlasnika, OBLIKSVOJINE, VRSTAPRAVA, OBIMPRAVA, Udeo, uneo ,datumUnosa, obrisan, koefUdeo, idiskazzemljista) SELECT " & poslednjiID_ & ", idVlasnika, OBLIKSVOJINE, VRSTAPRAVA, OBIMPRAVA, Udeo, uneo ,datumUnosa, 0, koefUdeo, idiskazzemljista FROM kom_vezaparcelavlasnik where idParcele=(SELECT idparc FROM kom_parcele WHERE brParceleF=" & Chr(34) & a_(0) & Chr(34) & " and DEOPARCELE=0)"
                    'sada imas ovo i mozes da izvrsis update
                    connComm.CommandText = stsql_
                    connComm.ExecuteNonQuery()
                    'sada idemo na sledeci korak a to je deo parcele!
                    Dim koeficijent_ = Val(qvr_.Table.RecordSet.Item(i).DataText(2).ToString) / povrsinaSuma
                    stsql_ = "insert into kom_parcele (skatopst, brojparc, podbroj, skulutre, hektari, ari, metri, deoparcele) select skatopst, " & parc_(0) & ", " & If(parc_.Length = 1, 0, parc_(1)) & ", skulutre, hektari*" & koeficijent_ & ", ari*" & koeficijent_ & ", metri*" & koeficijent_ & ", 1 where deoparcele=1 and brparcelef=" & Chr(34) & a_(0) & Chr(34)
                    connComm.ExecuteNonQuery()
                    'to bi bilo to
                Next

            Else
                MsgBox("Za parcelu " & a_(0) & " nemate rekord u bazi proverite")
                myreader_.Close()
            End If

        Loop

    End Sub
End Class