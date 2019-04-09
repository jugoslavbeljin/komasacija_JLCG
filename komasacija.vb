Imports Manifold.Interop
Imports Microsoft.Office.Interop

Module komasacija
    Structure csvKontrola
        Public idIskaza As Integer
        Public idtable As Integer
        Public iskazStatus As Integer
        Public iskazVrednost As Double
        Public iskazNadeljenCSV As Double
        Public listingFileova As String
    End Structure

    '//KONVERT
    Public Function konvertCirilicaULatinicu(ulaz_ As String, Optional prvoVeliko As Boolean = True) As String

        konvertCirilicaULatinicu = ulaz_
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "A", "А")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "B", "Б")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "V", "В")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "G", "Г")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "D", "Д")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "DJ", "Ђ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "E", "Е")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "Ž", "Ж")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "Z", "З")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "I", "И")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "J", "Ј")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "K", "К")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "L", "Л")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "LJ", "Љ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "M", "М")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "N", "Н")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "NJ", "Њ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "O", "О")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "P", "П")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "R", "Р")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "S", "С")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "T", "Т")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "U", "У")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "F", "Ф")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "H", "Х")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "C", "Ц")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "Ć", "Ћ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "Č", "Ч")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "Š", "Ш")

        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "a", "а")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "b", "б")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "v", "в")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "g", "г")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "d", "д")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "đ", "ђ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "e", "е")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "ž", "ж")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "z", "з")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "i", "и")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "j", "ј")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "k", "к")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "l", "л")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "lj", "љ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "m", "м")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "n", "н")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "nj", "њ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "o", "о")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "p", "п")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "r", "р")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "s", "с")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "t", "т")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "u", "у")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "f", "ф")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "h", "х")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "c", "ц")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "ć", "ћ")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "č", "ч")
        konvertCirilicaULatinicu = Replace(konvertCirilicaULatinicu, "š", "ш")

        If prvoVeliko = True Then
            konvertCirilicaULatinicu = StrConv(konvertCirilicaULatinicu, VbStrConv.ProperCase)
        End If

        Return konvertCirilicaULatinicu

    End Function
    '//DIREKCIONI

    Public Function NiAnaB(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
        'samo napomena x ti je 7
        Dim dx, dy As Double
        dy = x2 - x1 : dx = y2 - y1
        'sada proveravas koji je slucaj
        If dy = 0 And dx > 0 Then
            NiAnaB = 0 '90
        ElseIf dy = 0 And dx < 0 Then
            NiAnaB = Math.PI
        ElseIf dy > 0 And dx = 0 Then
            NiAnaB = Math.PI / 2
        ElseIf dy < 0 And dx = 0 Then
            NiAnaB = (Math.PI / 2) * 3
        ElseIf dy > 0 And dx > 0 Then
            'znaci da ne dodajes nista
            NiAnaB = udec_r(Math.Atan(dy / dx))
        ElseIf dy > 0 And dx < 0 Then
            NiAnaB = udec_r(Math.Abs(Math.Atan(dx / dy))) + 90
        ElseIf dy < 0 And dx < 0 Then
            NiAnaB = udec_r(Math.Atan(dy / dx)) + 180
        ElseIf dy < 0 And dx > 0 Then
            NiAnaB = udec_r(Math.Abs(Math.Atan(dx / dy))) + 270
        End If

        Return NiAnaB

    End Function

    Public Function NiAnaB(ByVal tackaA As PointF, ByVal tackaB As PointF) As Double
        'samo napomena x ti je 7
        Dim dx, dy As Double
        dy = tackaB.X - tackaA.X : dx = tackaB.Y - tackaA.Y
        If dy > 0 And dx > 0 Then
            'znaci da ne dodajes nista
            NiAnaB = Math.Atan(dy / dx)
        ElseIf dy > 0 And dx < 0 Then
            NiAnaB = Math.Abs(Math.Atan(dy / dx)) + 90
        ElseIf dy < 0 And dx < 0 Then
            NiAnaB = Math.Atan(dy / dx) + 180
        ElseIf dy < 0 And dx > 0 Then
            NiAnaB = Math.Abs(Math.Atan(dy / dx)) + 270
        End If

        Return NiAnaB

    End Function

    Public Function Duzina(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
        Duzina = Math.Sqrt((x1 - x2) ^ 2 + (y1 - y2) ^ 2)

        Return Duzina

    End Function

    Public Function dopuna_nule(broj_ As Integer) As String

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

    Public Function upisi1obrazac(xls_ As Microsoft.Office.Interop.Excel.Worksheet, odakle_ As Integer, ugao_ As Double, stanica_ As String, brPoc_ As String, brKraj_ As String) As Integer

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

    Public Function formatirajUgaoZaPrikaz(ugao_ As Double) As String

        Dim a_ = Math.Round(uste(ugao_), 4) & "0"

        formatirajUgaoZaPrikaz = Fix(a_) & "    " & Mid(a_, InStr(a_, ".") + 1, 2) & "    " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return formatirajUgaoZaPrikaz
    End Function

    Public Function formirajUgaoZaPrikazBezStepeni(ugao_ As Double) As String
        Dim a_ = Math.Round(uste(ugao_), 4) & "0"

        formirajUgaoZaPrikazBezStepeni = Mid(a_, InStr(a_, ".") + 1, 2) & "    " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return formirajUgaoZaPrikazBezStepeni
    End Function

    Public Function vratiUgaoIzFormataUDec(ugao_ As String) As Double

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
    '//UGLOVI 
    Public Function udec(ByVal F As Double) As Double
        'iz stepeni,minuti, sekunde u doubleni zapis
        'izdvojmo prvo stepene

        Dim p1, p2 As Double
        p1 = (F - Fix(F))
        p2 = (Fix(p1 * 100)) / 60 + Fix(F)
        udec = ((p1 * 100 - Fix(p1 * 100)) * 100) / 3600 + p2

    End Function

    Public Function uste(ByVal F As Double) As Double
        'iz doublenog zapisa u s.m.s
        Dim p1, p2
        p1 = ((F - Fix(F)) * 0.6) * 100
        p2 = (p1 - Fix(p1)) * 60
        uste = Fix(F) + Fix(p1) / 100 + p2 / 10000
    End Function

    Public Function urad(ByVal F As Double) As Double
        'iz doublenog zapisa u radijane
        urad = (F * Math.PI) / 180
    End Function

    Public Function udec_r(ByVal F As Double) As Double
        'iz radijana u doubleni zapis
        udec_r = (F * 180) / Math.PI
    End Function

    Public Function uUkras_izdec(ugao_ As Double) As String
        'ulaz je ugao u decimalnom zapisu
        Dim a_ = Math.Round(uste(ugao_), 4).ToString
        uUkras_izdec = Fix(a_) & " " & Mid(a_, InStr(a_, ".") + 1, 2) & " " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return uUkras_izdec
    End Function

    Public Function uUkras_izdecSkraceno(ugao_ As Double) As String
        'ulaz je ugao u decimalnom zapisu
        Dim a_ = Math.Round(uste(ugao_), 4).ToString

        uUkras_izdecSkraceno = If(Fix(a_) = "0", "", Fix(a_)) & " " & IIf(Mid(a_, InStr(a_, ".") + 1, 2) = "00", "", Mid(a_, InStr(a_, ".") + 1, 2)) & " " & Mid(a_, InStr(a_, ".") + 3, 2)

        Return uUkras_izdecSkraceno
    End Function

    '//DATUMI

    Public Function datumDrzavniPraznikPrviRadni(ByVal datum_ As Date, ByVal vikendJeRadni As Boolean) As Date
        'ako je vikend ukljucen onda ne gleda vikend u suprotnom gleda!

        Dim drzavniPraznici(10) As String
        drzavniPraznici(0) = "1/1"
        drzavniPraznici(1) = "2/1"
        drzavniPraznici(2) = "7/1"
        drzavniPraznici(3) = "8/1"
        drzavniPraznici(4) = "14/1"
        drzavniPraznici(5) = "15/2"
        drzavniPraznici(6) = "16/2"
        drzavniPraznici(7) = "1/5"
        drzavniPraznici(8) = "2/5"
        drzavniPraznici(9) = "11/11"

        datum_ = datum_.AddDays(1) 'odmah prebacis na sledeci jer si poceo sa radnim danom!

        Dim jestePraznik As Boolean = False
        For i = 0 To drzavniPraznici.Length - 1
            If drzavniPraznici(i) = (datum_.Day & "/" & datum_.Month) Then
                jestePraznik = True
                Exit For
            End If
        Next

        If jestePraznik = True Then
            'sada ides na sledeci da vidis da li je radni dan!@
            If vikendJeRadni = True Then
                'prebacujes na sledeci dan i to je to!
                datum_ = datum_.AddDays(1)

            Else
                'sada prvoeris koji je dan!
                If datum_.DayOfWeek = DayOfWeek.Friday Then
                    'prvoeris dalli vodis racuna o vikendima
                    datum_ = datum_.AddDays(2)
                End If
            End If
        End If

        Return datum_
    End Function

    Public Function datumVikendPrviRadniDan(ByVal datum_ As Date, ByVal vikendJeRadni As Boolean)

        If vikendJeRadni = True Then
            'prebacujes na sledeci dan i to je to!
            datum_ = datum_.AddDays(1)
        Else
            'sada prvoeris koji je dan!
            If datum_.DayOfWeek = DayOfWeek.Friday Then
                'prvoeris dalli vodis racuna o vikendima
                datum_ = datum_.AddDays(2)
            End If
        End If

        'ovde odmah resetujes vreme na pocetno radno! i to je to!


        Return datum_
    End Function


    '//komasacija
    Public Function H_racunanjeDirektno(ByVal a_ As Double, ByVal b_ As Double, ByVal P_ As Double, ByVal Pdod_ As Double) As Double
        If a_ = b_ Then
            a_ = a_ + 0.5
        End If
        Dim K_ As Double = ((a_ - b_) * (a_ + b_)) / (2 * P_)
        Dim h1 As Double = (a_ - Math.Sqrt((a_ ^ 2) - (2 * K_ * Pdod_))) / K_
        Return h1

    End Function
    Public Function H_racunanjePribliznoPetlja(ByVal Pdod_ As Double, ByVal a_ As Double, ByVal K_ As Double) As Double
        Dim x As Double = 0 : Dim h1 As Double = (2 * Pdod_) / (2 * a_ - x) : Dim kraj As Boolean = False
        'ulazis u interaktivni proces
        Dim hpred As Double
        Dim kraj_ As Boolean = False
        Do While Not kraj_ = True
            'racunas
            h1 = (2 * Pdod_) / (2 * a_ - x) : x = K_ * h1
            If h1 - hpred < 0.00000001 Then
                kraj_ = True
            End If
            hpred = h1
        Loop
        H_racunanjePribliznoPetlja = h1
    End Function

    Public Sub podeliParceluViseDelovaNadela(ByVal doc_ As Manifold.Interop.Document, ByVal ID_ As Integer, ByVal ID2_ As Integer, ByVal napred_ As Boolean, ByVal freeFile_ As Integer, ByVal drwNadelaProcRazred As Manifold.Interop.Drawing)
        'ulaz ti je doc_
        'id objekta iz izdvojenog seta!
        'za ove tacke cemo da vidimo
        'RADIS SAMO ZA TAJ SEGMENT!
        PrintLine(freeFile_, "")
        PrintLine(freeFile_, "Poceo deobu i nadelu segmenara u " & Now)
        PrintLine(freeFile_, "Segment sa ID:" & ID_)
        PrintLine(freeFile_, "")
        Dim drwNadela As Manifold.Interop.Drawing = doc_.ComponentSet("deoba")
        'Dim drwNadelaProcRazred As Manifold.Interop.Drawing = doc_.ComponentSet("deoba_proc_razred")
        Dim drwO, drwL, drwS, drwNT As Manifold.Interop.Drawing
        Try
            drwO = doc_.NewDrawing("tempOriginal", drwNadela.CoordinateSystem, True) 'stavlja tekuca deljenja 
            drwL = doc_.NewDrawing("tempLinije", drwNadela.CoordinateSystem, True) 'stavlja tekuca deljenja 
            'Dim drwC As Manifold.Interop.Drawing = doc_.NewDrawing("TempCopy", drwNadela.CoordinateSystem, True) 'stavlja tekuca deljenja 
            drwS = doc_.NewDrawing("tempSeci", drwNadela.CoordinateSystem, True) 'ovo je za jedan poligon
            drwNT = doc_.NewDrawing("tempNormTopo", drwNadela.CoordinateSystem, True) 'ovo je za jedan poligon
            Dim col_ As Manifold.Interop.Column = doc_.Application.NewColumnSet.NewColumn
            col_.Type = ColumnType.ColumnTypeInt32
            col_.Name = "idTable"
            Dim tbl_ As Manifold.Interop.Table = drwNT.OwnedTable
            tbl_.ColumnSet.Add(col_)
            'tbl_ = drwO.OwnedTable

            'col_.Name = "ID2"
            'col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32

            'tbl_.ColumnSet.Add(col_)

            col_ = Nothing
            tbl_ = Nothing
        Catch ex As Exception
            drwO = doc_.ComponentSet("tempOriginal") : drwO.ObjectSet.RemoveAll()
            drwL = doc_.ComponentSet("tempLinije") : drwL.ObjectSet.RemoveAll()
            drwS = doc_.ComponentSet("tempSeci") : drwS.ObjectSet.RemoveAll()
            drwNT = doc_.ComponentSet("tempNormTopo") : drwNT.ObjectSet.RemoveAll()
        End Try

        'pa sad brises ono sto ti ne treba
        PrintLine(freeFile_, "Poceo sa deobom u " & Now)

        Dim qvrBrisi As Query = doc_.NewQuery("brisivisak")
        'dali sada moze copy paste
        drwNadela.Copy()
        drwO.Paste()
        qvrBrisi.Text = "delete from [tempOriginal] where not [ID2]=" & ID2_
        qvrBrisi.RunEx(True)
        'doc_.Save()
        qvrBrisi.Text = "INSERT INTO [TempNormTopo] ([Geom (I)],[idTable]) SELECT [Geom (I)],[idTable] FROM [TempOriginal]"
        qvrBrisi.RunEx(True)

        qvrBrisi.Text = "select [idTable] from [TempNormTopo]"
        qvrBrisi.RunEx(True)
        'If qvrBrisi.Table.RecordSet.Item(0).DataText(1) = 5 Then
        '    MsgBox("sad si na cetvrtoj vidi sta nije u redu")
        'End If

        qvrBrisi = Nothing
        doc_.ComponentSet.Remove("brisivisak")

        Dim drwPR As Manifold.Interop.Drawing = doc_.ComponentSet(My.Settings.layerName_ProcembeniRazredi)
        Dim topProcembeniRazredi As Manifold.Interop.Topology = doc_.Application.NewTopology
        topProcembeniRazredi.Bind(drwPR)
        topProcembeniRazredi.Build()

        Dim analizer1_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
        analizer1_.NormalizeTopology(drwNT, drwNT.ObjectSet)
        analizer1_ = Nothing

        doc_.Save()
        'sada bi trebalo da je ok
        If drwNT.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Count = 4 Then
            Dim povrsine_ As Manifold.Interop.Query = doc_.NewQuery("povrsine_")

            povrsine_.Text = "select [vrednost],[deoba] from [deoba] where [ID2]=" & ID2_
            povrsine_.RunEx(True)
            Dim povrsina_ As Double = povrsine_.Table.RecordSet.Item(0).DataText(1)
            PrintLine(freeFile_, "Vrednost koju nadeljujem je: " & povrsina_)
            Dim popMattext_ = povrsine_.Table.RecordSet.Item(0).DataText(2)
            Dim popMattextm_ = popMattext_.Split("/")
            Dim vlasnici(popMattextm_.Length - 1) As Integer
            Dim popPovrsine(popMattextm_.Length - 1) As Double

            For j = 0 To popMattextm_.Length - 1
                Dim a_ = popMattextm_(j).Split(":")
                vlasnici(j) = a_(0)
                popPovrsine(j) = a_(1)
                PrintLine(freeFile_, "Vlasnik : " & a_(0) & " dobija : " & a_(1))
            Next

            For j = 0 To vlasnici.Length - 1
                Dim duzine_ As Manifold.Interop.Query = doc_.NewQuery("duzine_")
                If napred_ = True Then
                    duzine_.Text = "SELECT duzina_,cstr(cgeomwkb(StartPoint([g]))) as pocetna_,cstr(cgeomwkb(EndPoint([g]))) as krajnja_,Dodl FROM (SELECT top 2 [g],Length([g]) as duzina_,Distance([g],[Tacke].[Geom (I)]) as Dodl from [TempNormTopo],[Tacke] WHERE [Tacke].[idTable]=[TempNormTopo].[idTable] SPLIT BY Branches(IntersectLine(Boundary([TempNormTopo].[Geom (I)]),Boundary([TempNormTopo].[Geom (I)]))) as [g]  order by Length([g]) DESC ) order by Dodl ASC"
                Else
                    duzine_.Text = "SELECT duzina_,cstr(cgeomwkb(StartPoint([g]))) as pocetna_,cstr(cgeomwkb(EndPoint([g]))) as krajnja_,Dodl FROM (SELECT top 2 [g],Length([g]) as duzina_,Distance([g],[Tacke].[Geom (I)]) as Dodl from [TempNormTopo],[Tacke] WHERE [Tacke].[idTable]=[TempNormTopo].[idTable] SPLIT BY Branches(IntersectLine(Boundary([TempNormTopo].[Geom (I)]),Boundary([TempNormTopo].[Geom (I)]))) as [g]  order by Length([g]) DESC ) order by Dodl DESC"
                End If
                PrintLine(freeFile_, "Racunanje visine h")
                PrintLine(freeFile_, "Duzina trapeza a: " & duzine_.Table.RecordSet.Item(0).DataText(1) & ", druzina stranice b=" & duzine_.Table.RecordSet.Item(1).DataText(1) & " vrednost koja se trazi je: " & popPovrsine(j))
                duzine_.RunEx(True)
                'doc_.Save()
                Dim h1 As Double = H_racunanjeDirektno(duzine_.Table.RecordSet.Item(0).DataText(1), duzine_.Table.RecordSet.Item(1).DataText(1), povrsina_, popPovrsine(j))
                PrintLine(freeFile_, "h1 trapeza je: " & h1)
                'sada ulazis u interativni!
                Dim pnt1(1), pnt2(1) As Double
                pnt1 = pointXYfromWKT(duzine_.Table.RecordSet.Item(0).DataText(2)) : pnt2 = pointXYfromWKT(duzine_.Table.RecordSet.Item(0).DataText(3))
                'prvo kreiras presecnu liniju

                'izgleda da kad je napred mora da ide upravan ugao a kad je nazad da je ok!
                'ponovo_:
                PrintLine(freeFile_, "Poceo interaktivni metod")
                Dim preseceno As Boolean = False
                Dim brojInteracija As Integer = 0
                Do While Not preseceno = True
                    brojInteracija += 1
                    PrintLine(freeFile_, "Interacija " & brojInteracija)
                    If brojInteracija > 20 Then
                        Exit For
                    End If
                    'doc_.Save()
                    drwL.ObjectSet.RemoveAll()
                    Dim d_ = direkcioniUgaoUpravnaUPolygonu(pnt1, pnt2, doc_)
                    podeliParceluViseDelova_KreirajPresecnuLiniju(d_(0), duzine_.Table.RecordSet(0).DataText(1), h1, pnt1, pnt2, doc_, drwL)
                    'sada mzoes da seces ali da vidimo sta sa kim!                    'selis ovaj poligon u tempseci
                    drwS.ObjectSet.RemoveAll()
                    drwS.ObjectSet.Add(drwO.ObjectSet.Item(0).Geom)

                    'ovde mi treba polje sa recimo ID-om da bi mogao da sumiram!
                    Dim analizer_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
                    analizer_.Split(drwS, drwS, drwS.ObjectSet, drwL.ObjectSet)
                    doc_.Save()

                    Dim tbl2_ As Manifold.Interop.Table = drwS.OwnedTable
                    If tbl2_.ColumnSet.ItemByName("ID2") = -1 Then
                        Dim col2_ As Manifold.Interop.Column = doc_.Application.NewColumnSet.NewColumn
                        col2_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32 : col2_.Name = "ID2" : tbl2_.ColumnSet.Add(col2_)
                        col2_ = Nothing : tbl2_ = Nothing
                    End If


                    Dim qvrSrediSeci As Manifold.Interop.Query = doc_.NewQuery("srediSeci")
                    qvrSrediSeci.Text = "update [tempSeci] set [ID2]=[ID]"
                    qvrSrediSeci.RunEx(True)

                    Dim topotest_ As Manifold.Interop.Topology = doc_.Application.NewTopology
                    topotest_.Bind(drwS)
                    topotest_.Build()

                    'sada mzoes presek
                    topProcembeniRazredi.DoIntersect(topotest_, "tempSPR")
                    doc_.Save()

                    If napred_ = True Then
                        qvrSrediSeci.Text = "SELECT sum([Area (I)]*[Faktor]) as vrednost_ FROM [TempSPR] WHERE [ID2]=(SELECT [ID] from (SELECT top 1 Distance([TempSeci].[ID],[Tacke].[ID]) as D_,[TempSeci].[ID] FROM [TempSeci],[Tacke] WHERE [Tacke].[idTable]=(SELECT [idTable] FROM [TempOriginal]) order by D_ ASC))"
                    Else
                        qvrSrediSeci.Text = "SELECT sum([Area (I)]*[Faktor]) as vrednost_ FROM [TempSPR] WHERE [ID2]=(SELECT [ID] from (SELECT top 1 Distance([TempSeci].[ID],[Tacke].[ID]) as D_,[TempSeci].[ID] FROM [TempSeci],[Tacke] WHERE [Tacke].[idTable]=(SELECT [idTable] FROM [TempOriginal]) order by D_ DESC))"
                    End If

                    qvrSrediSeci.RunEx(True)
                    'doc_.Save()
                    'sada imas sta ti treba
                    Dim postignutaVrednost As Double = qvrSrediSeci.Table.RecordSet.Item(0).DataText(1)
                    PrintLine(freeFile_, "Postignuta vrednost: " & postignutaVrednost)
                    qvrSrediSeci = Nothing
                    doc_.ComponentSet.Remove("srediSeci")
                    PrintLine(freeFile_, "Razlika u interaciji " & brojInteracija & " je " & (postignutaVrednost - popPovrsine(j)))
                    If Math.Round(postignutaVrednost, 1) <> Math.Round(popPovrsine(j), 1) Then
                        Dim p_ As Double = (popPovrsine(j) - postignutaVrednost) / duzine_.Table.RecordSet.Item(0).DataText(1)
                        'sada mora ispocetka
                        h1 = h1 + p_
                        'sada treba obrisati brdo toga da vidimo sta!
                        doc_.ComponentSet.Remove("TempSPR")
                    Else
                        'prosao i mozes da nastavis
                        preseceno = True
                        'doc_.Save()
                        'sta sada radis?
                        'prvo iz temp seci prebacujes u deobu temu
                        Dim qvrRasporedi As Manifold.Interop.Query = doc_.NewQuery("rasporedujem")
                        If napred_ = True Then
                            qvrRasporedi.Text = "SELECT top 1 Distance([TempSeci].[ID],[Tacke].[ID]) as D_,[TempSeci].[ID] FROM [TempSeci],[Tacke] WHERE [Tacke].[idTable]=(SELECT [idTable] FROM [TempOriginal]) order by D_ ASC"
                            'sredi one dve tacke
                        Else
                            qvrRasporedi.Text = "SELECT top 1 Distance([TempSeci].[ID],[Tacke].[ID]) as D_,[TempSeci].[ID] FROM [TempSeci],[Tacke] WHERE [Tacke].[idTable]=(SELECT [idTable] FROM [TempOriginal]) order by D_ DESC"
                            'sredi dve tacke mislim da je tu trenutno problem! - mozda ovo ne moram ako idem na varijantu da mi 
                        End If

                        'zatim drugi odsecak prebacujes u temporiginal
                        qvrRasporedi.RunEx(True)
                        'ovaj ide u nadelu
                        'doc_.Save()
                        drwNadela.ObjectSet.Add(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRasporedi.Table.RecordSet.Item(0).DataText(2))).Geom)
                        drwS.ObjectSet.Remove(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRasporedi.Table.RecordSet.Item(0).DataText(2))))
                        'doc_.Save()
                        If napred_ = True Then
                            qvrRasporedi.Text = "update [deoba] set [NoviVlasnikNapred]=" & vlasnici(j) & " where [ID]=(select top 1 [ID] from [deoba] order by [ID] Desc)"
                        Else
                            qvrRasporedi.Text = "update [deoba] set [NoviVlasnikNazad]=" & vlasnici(j) & " where [ID]=(select top 1 [ID] from [deoba] order by [ID] Desc)"
                        End If

                        qvrRasporedi.RunEx(True)
                        'prebacujes poslednji iz seci u temp original
                        qvrRasporedi.Text = "update [tempOriginal] set [Geom (I)]=(select top 1 [Geom (I)] from TempSeci)"
                        qvrRasporedi.RunEx(True)
                        qvrRasporedi.Text = "delete from [TempNormTopo]"
                        qvrRasporedi.RunEx(True)
                        qvrRasporedi.Text = "INSERT INTO [TempNormTopo] ([Geom (I)],[idTable]) SELECT [Geom (I)],[idTable] FROM [TempOriginal]"
                        qvrRasporedi.RunEx(True)
                        Dim analizer2_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
                        analizer2_.NormalizeTopology(drwNT, drwNT.ObjectSet)
                        analizer2_ = Nothing
                        doc_.ComponentSet.Remove("TempSPR")
                        qvrRasporedi = Nothing
                        doc_.ComponentSet.Remove("rasporedujem")
                        'treba pomeriti tacke!
                        'povrsina_ = povrsina_ - popPovrsine(j)

                        If j = popPovrsine.Length - 2 Then
                            'znaci da je poslednji pa mozes da ga dokrajcis!
                            PrintLine(freeFile_, "nadela poslednjem vlasniku i to vrednost " & drwS.ObjectSet.Item(0).Geom.Area)
                            'ovde bi trebalo proveriti da li odgovara vrednsot onome sto je preostalo!

                            'DODATI!!!!!!!!!!!!!!!!

                            drwNadela.ObjectSet.Add(drwS.ObjectSet.Item(0).Geom)
                            Dim qvrposlednji As Manifold.Interop.Query = doc_.NewQuery("poslenji")

                            If napred_ = True Then
                                qvrposlednji.Text = "update [deoba] set [NoviVlasnikNapred]=" & vlasnici(j + 1) & " where [ID]=(select top 1 [ID] from [deoba] order by [ID] Desc)"
                            Else
                                qvrposlednji.Text = "update [deoba] set [NoviVlasnikNazad]=" & vlasnici(j + 1) & " where [ID]=(select top 1 [ID] from [deoba] order by [ID] Desc)"
                            End If

                            'zatim drugi odsecak prebacujes u temporiginal
                            qvrposlednji.RunEx(True)
                            qvrposlednji = Nothing
                            doc_.ComponentSet.Remove("poslenji")
                            drwS.ObjectSet.RemoveAll() : drwL.ObjectSet.RemoveAll() : drwO.ObjectSet.RemoveAll() : drwNT.ObjectSet.RemoveAll()
                            drwNadela = Nothing : drwNadelaProcRazred = Nothing : drwS = Nothing : drwL = Nothing : drwNT = Nothing : drwO = Nothing
                            j = popPovrsine.Length - 1
                            doc_.Save()
                            Exit Do
                        End If
                    End If
                Loop
                duzine_ = Nothing
                doc_.ComponentSet.Remove("duzine_")

            Next

            povrsine_ = Nothing
            doc_.ComponentSet.Remove("povrsine_")
        Else
            'opet imas neko sranje pa treba videti koje!
            PrintLine(freeFile_, "Ovde je verovatno trougao i to treba resiti nekako!")
        End If
        If napred_ = False Then
            'znaci mozes da brisess ove sto su ti visak
            doc_.ComponentSet.Remove("TempLinije")
            doc_.ComponentSet.Remove("TempNormTopo")
            doc_.ComponentSet.Remove("TempOriginal")
            doc_.ComponentSet.Remove("TempSeci")
        End If
        doc_.Save()
        PrintLine(freeFile_, "")
        PrintLine(freeFile_, "Zavrsio obradu segmenta u " & Now)
        PrintLine(freeFile_, "")
    End Sub

    Public Function podeliParceluViseDelova(ByVal brParc_ As String, ByVal idVlasnika_() As Integer, ByVal povrsina_() As Double, ByVal polygonwkt_ As String, ByVal prvaTackawkt_ As String, ByVal drugaTackewkt_ As String, ByVal doc_ As Manifold.Interop.Document, ByVal idIzvestaj As Integer)

        'ulazi ti poligon parcele i to je jedino sto te zanima i ulazi ti broj vlasnika ali ti trebaju i povrsine!
        Dim drw As Manifold.Interop.Drawing = doc_.ComponentSet.Item(My.Settings.layerName_parcele)
        Dim drwO As Manifold.Interop.Drawing = doc_.NewDrawing("tempOriginal", drw.CoordinateSystem, True) 'stavlja tekuca deljenja 
        Dim drwL As Manifold.Interop.Drawing = doc_.NewDrawing("tempLinije", drw.CoordinateSystem, True) 'stavlja tekuca deljenja 
        Dim drwC As Manifold.Interop.Drawing = doc_.NewDrawing("TempCopy", drw.CoordinateSystem, True) 'stavlja tekuca deljenja 
        Dim drwS As Manifold.Interop.Drawing = doc_.NewDrawing("tempSeci", drw.CoordinateSystem, True) 'ovo je za jedan poligon
        drwO.ObjectSet.Add(doc_.Application.NewGeomFromTextWKT(polygonwkt_))

        Dim pnt1_ = pointXYfromWKT(prvaTackawkt_) : Dim pnt2_ = pointXYfromWKT(drugaTackewkt_)
        Dim ugaoduz_() As Double = direkcioniUgaoUpravnaUPolygonu(pnt1_, pnt2_, polygonwkt_, doc_)
        Dim ugao_ As Double = ugaoduz_(0) : Dim d_ As Double = ugaoduz_(1)
        podeliPolygon2Dela_KreirajLinijeUSvakojTacki_I_PodeliPoligonNaDelove(ugao_, d_, doc_)
        'doc_.Save()
        Dim mAnalyzer As Manifold.Interop.Analyzer = doc_.NewAnalyzer

        Try
            For i = 0 To povrsina_.Length - 1
                Dim prvi, drugi As Integer
                Dim qvrPrvi As Manifold.Interop.Query = doc_.NewQuery("prvi")
                Dim qvrDrugi As Manifold.Interop.Query = doc_.NewQuery("drugi")
                doc_.Save()

                Try
                    qvrPrvi.Text = "SELECT top 1 A.[ID],cstr(cgeomwkb(A.[Geom (I)])), A.P, B.broj_ from (SELECT [ID],[Geom (I)],[Area (I)] as P FROM(SELECT  AssignCoordSys(NewLine(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "),NewPoint(" & pnt2_(0) & "," & pnt2_(1) & ")),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)) as line_, [Geom (I)],[ID],[Area (I)] FROM [tempOriginal] ) WHERE Touches(line_,[Geom (I)]) and IsArea([Geom (I)]) ) as A INNER JOIN (SELECT count(*) as broj_,G.[ID],G.[Geom (I)] FROM ((SELECT [id],[Geom (I)]  FROM [TempOriginal] ) as G INNER JOIN 	(SELECT [ID],[Geom (I)] FROM [TempOriginal]) as V on G.[ID]<>V.[ID] ) WHERE Touches(G.[Geom (I)],V.[Geom (I)]) and IsArea(G.[Geom (I)]) GROUP BY G.[ID],G.[Geom (I)] ) as B on A.[ID]=B.[ID] order by B.broj_"
                    qvrPrvi.RunEx(True)
                    prvi = qvrPrvi.Table.RecordSet.Item(0).DataText(1)
                    qvrDrugi.Text = "SELECT A.[ID],A.[Geom (I)],A.[Area (I)],cstr(cgeomwkb(A.[Geom (I)])) FROM (SELECT [ID],[Geom (I)],[Area (I)] FROM [TempOriginal] ) as A, (SELECT [Geom (I)] FROM [TempOriginal] WHERE [ID]=" & prvi & ") as B WHERE Touches(A.[Geom (I)],B.[Geom (I)]) and  A.[ID]<>" & prvi
                    qvrDrugi.RunEx(True)
                    drugi = qvrDrugi.Table.RecordSet.Item(0).DataText(1)
                Catch ex As Exception
                    'ovim bi trebalo da resis kada ima jedan poligon!
                    qvrPrvi.Text = "SELECT  [tempOriginal].[ID],cstr(cgeomwkb([tempOriginal].[Geom (I)])), [tempOriginal].[Area (I)] from [tempOriginal]"
                    qvrPrvi.RunEx(True)
                    prvi = qvrPrvi.Table.RecordSet.Item(0).DataText(1)
                    drugi = prvi
                End Try
                PrintLine(idIzvestaj, "Prvi segment: " & prvi & ",Drugi segment: " & drugi)
                'doc_.Save()

                If i = (povrsina_.Length - 1) Then

                    mAnalyzer.Union(drwO, drwO, drwO.ObjectSet) 'sada ih samo spojis i izlistas njihovu povrsinu u zapisnik
                    drw.ObjectSet.Add(drwO.ObjectSet.Item(0).Geom)

                    podeliParceluViseDelova_UpdateRecordParcele(doc_, brParc_, idVlasnika_(i))

                    PrintLine(idIzvestaj, "Poslednji Segment koji smo prebacili: " & drwO.ObjectSet.Item(0).Geom.AreaNative & " razlika: " & (drwO.ObjectSet.Item(0).Geom.AreaNative - povrsina_(i)))
                    'prepusujes vlasnika i brises poslednji segment
                    PrintLine(idIzvestaj, "")

                Else
                    For j = 0 To drwO.ObjectSet.Count - 1
                        'doc_.Save()
                        If qvrPrvi.Table.RecordSet.Item(0).DataText(3) > povrsina_(i) Then

                            If drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))).Geom.BranchSet.Item(0).PointSet.Count = 3 Then
                                'trougao
                                Dim drwTmp As Drawing = doc_.NewDrawing("Pnt2pnt", drw.CoordinateSystem, True)
                                drwL.ObjectSet.RemoveAll()
                                Dim pntNL2 As PointSet = doc_.Application.NewPointSet
                                pntNL2.Add(doc_.Application.NewPoint(pnt1_(0), pnt1_(1)))
                                pntNL2.Add(doc_.Application.NewPoint(pnt2_(0), pnt2_(1)))
                                Dim nl2_ As Geom = doc_.Application.NewGeom(GeomType.GeomPoint, doc_.Application.NewPoint(pnt1_(0), pnt1_(1)))
                                drwTmp.ObjectSet.Add(nl2_)
                                nl2_ = doc_.Application.NewGeom(GeomType.GeomPoint, doc_.Application.NewPoint(pnt2_(0), pnt2_(1)))
                                drwTmp.ObjectSet.Add(nl2_)

                                nl2_ = Nothing
                                pntNL2 = Nothing

                                drwS.ObjectSet.RemoveAll()
                                'prebacujes ga u seci da bi imao kontinuitet sa polkigonom!
                                drwS.ObjectSet.Add(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))).Geom)

                                Dim qvrtr1 As Query = doc_.NewQuery("trougaoAH")
                                qvrtr1.Text = "SELECT pnt_,line_,h_,d FROM ((SELECT pnt_,line_,Distance(pnt_,line_) as h_,Length(line_) as d FROM ((SELECT pnt_ FROM [TempSeci] split by Coords([Geom (I)]) as pnt_) as A, (SELECT line_ FROM [TempSeci] SPLIT BY Branches(IntersectLine(Boundary([Geom (I)]),Boundary([Geom (I)]))) as line_) as B ) WHERE NOT Touches(pnt_,line_) ) as C, (SELECT top 1 max(Distance(pnt1,pnt2)) as d_,pnt2  FROM ((SELECT [Geom (I)] as pnt1 FROM [Pnt2pnt]) as A, (SELECT pnt2 FROM [TempSeci] split by Coords([Geom (I)]) as pnt2) as B ) GROUP by pnt2 order by d_ desc) as D ) WHERE C.d=D.d_"
                                qvrtr1.RunEx(True)

                                Dim h1 As Double = (2 * povrsina_(1) * Val(qvrtr1.Table.RecordSet.Item(0).DataText(3))) / Val(qvrtr1.Table.RecordSet.Item(0).DataText(4)) 'treba da se dobije 4.5 oko 5!
                                h1 = Math.Sqrt(h1)
                                h1 = Val(qvrtr1.Table.RecordSet.Item(0).DataText(3)) - h1 ' visina trapeza

                                'kreiras liniju sada ti treba query!
                                Dim qvrtr2 As Manifold.Interop.Query = doc_.NewQuery("paralelna")
                                qvrtr2.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT);INSERT INTO [templinije] ([Geom (I)]) SELECT MoveHorizontally(MoveVertically(line_,sin(ugao)*" & h1 & "),cos(ugao)*" & h1 & ") FROM (SELECT CentroidX(StartPoint(line_)) as x1,CentroidY(StartPoint(line_)) as y1,CentroidX(EndPoint(line_)) as x2,CentroidY(EndPoint(line_)) as y2, Atn2((CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_))),(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_)))) as ugao, Length(line_) as d,line_ FROM ((SELECT pnt_,line_ FROM ((SELECT pnt_ FROM [TempSeci] split by Coords([Geom (I)]) as pnt_) as A, (SELECT line_ FROM [TempOriginal] SPLIT BY Branches(IntersectLine(Boundary([Geom (I)]),Boundary([Geom (I)]))) as line_) as B ) WHERE NOT Touches(pnt_,line_) ) as C, (SELECT top 1 min(Distance(pnt1,pnt2)) as d_,pnt2 FROM ((SELECT [Geom (I)] as pnt1 FROM [Pnt2pnt]) as A, (SELECT pnt2 FROM [TempSeci] split by Coords([Geom (I)]) as pnt2) as B ) GROUP by pnt2 order by d_ desc) as D ) WHERE C.pnt_=D.pnt2 )"
                                qvrtr2.RunEx(True)
                                'doc_.Save()
                                'sada ti treba da vidis dali postoji presek sa poligonom a ako nepostoji ide na drugu stranu
                                qvrtr2.Text = "SELECT line_ FROM ((select Boundary([Geom (I)]) as pol_ from [TempOriginal]) as A, (SELECT [Geom (I)] as line_ from [TempLinije]) as B) WHERE Touches(A.pol_,B.line_)"
                                qvrtr2.RunEx(True)
                                'doc_.Save()

                                If qvrtr2.Table.RecordSet.Count = 0 Then
                                    'sada ti treba sa obrnutim znakoom
                                    drwL.ObjectSet.RemoveAll()
                                    qvrtr2.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT);INSERT INTO [templinije] ([Geom (I)]) SELECT MoveHorizontally(MoveVertically(line_,sin(ugao)*" & h1 & "),cos(ugao)*" & h1 & ") FROM (SELECT CentroidX(StartPoint(line_)) as x1,CentroidY(StartPoint(line_)) as y1,CentroidX(EndPoint(line_)) as x2,CentroidY(EndPoint(line_)) as y2, Atn2((CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_))),(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_)))) as ugao, Length(line_) as d,line_ FROM ((SELECT pnt_,line_ FROM ((SELECT pnt_ FROM [TempSeci] split by Coords([Geom (I)]) as pnt_) as A, (SELECT line_ FROM [TempSeci] SPLIT BY Branches(IntersectLine(Boundary([Geom (I)]),Boundary([Geom (I)]))) as line_) as B ) WHERE NOT Touches(pnt_,line_) ) as C, (SELECT top 1 min(Distance(pnt1,pnt2)) as d_,pnt2 FROM ((SELECT [Geom (I)] as pnt1 FROM [Pnt2pnt]) as A, (SELECT pnt2 FROM [TempSeci] split by Coords([Geom (I)]) as pnt2) as B ) GROUP by pnt2 order by d_ desc) as D ) WHERE C.pnt_=D.pnt2 )"
                                    qvrtr2.RunEx(True)
                                End If

                                drwO.ObjectSet.Remove(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))))
                                'sada mozes na presek sa poligonom!
                                mAnalyzer.Split(drwS, drwS, drwS.ObjectSet, drwL.ObjectSet)
                                'ostaje da odradis prebacivanje jednog i drugog
                                'odredis ih po rstojanju!

                                qvrtr2.Text = "select Distance(C.line_,AssignCoordSys(NewPoint(CentroidX([Geom (I)]),CentroidY([Geom (I)])),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT))) as D_,[ID] FROM [TempSeci],(select top 1 AssignCoordSys(newline(pnt1,pnt2),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)) as line_ FROM ((SELECT [Geom (I)] as pnt1,[ID] from [Pnt2pnt]) as A INNER JOIN (SELECT [Geom (I)] as pnt2,[ID] from [Pnt2pnt]) as B ON A.[ID]<>B.[ID] )) as C ORDER BY D_ ASC"
                                qvrtr2.RunEx(True)

                                'sada treba prebaciti prvi ! jer je ostatak za deobu!
                                drw.ObjectSet.Add(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrtr2.Table.RecordSet.Item(0).DataText(2))).Geom)

                                podeliParceluViseDelova_UpdateRecordParcele(doc_, brParc_, idVlasnika_(i))

                                drwO.ObjectSet.Add(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrtr2.Table.RecordSet.Item(1).DataText(2))).Geom)

                                'sada znas koji ti ostaje ostaje ti onaj drugi za njega treba odrediti tacke
                                Dim qvrUpdateParc As Manifold.Interop.Query = doc_.NewQuery("pnts")
                                qvrUpdateParc.Text = "SELECT CentroidX(B.pnt3_) as x_,CentroidY(B.pnt3_) as y_ from ((SELECT min(Distance(pnt1_,[Pnt2pnt].[Geom (I)])) as dtr_,[Pnt2pnt].[ID] FROM [TempSeci],[Pnt2pnt] WHERE [TempSeci].[ID]=" & qvrtr2.Table.RecordSet.Item(0).DataText(2) & " SPLIT by Coords([TempSeci].[Geom (I)]) as pnt1_ group by [Pnt2pnt].[ID]) as A, (SELECT Distance([Pnt2pnt].[Geom (I)], pnt3_) as di_,pnt3_ FROM [TempSeci],[Pnt2pnt] WHERE [TempSeci].[ID]=" & qvrtr2.Table.RecordSet.Item(0).DataText(2) & " SPLIT by Coords([TempSeci].[Geom (I)]) as pnt3_) as B ) WHERE A.dtr_=B.di_"
                                qvrUpdateParc.RunEx(True)
                                'doc_.Save()
                                pnt1_(0) = qvrUpdateParc.Table.RecordSet.Item(0).DataText(1) : pnt1_(1) = qvrUpdateParc.Table.RecordSet.Item(0).DataText(2)
                                pnt2_(0) = qvrUpdateParc.Table.RecordSet.Item(1).DataText(1) : pnt2_(1) = qvrUpdateParc.Table.RecordSet.Item(1).DataText(2)

                                'doc_.Save()

                                drwS.ObjectSet.RemoveAll()

                                'treba odrediti koordinate za sledece dve tacke !

                                drwTmp = Nothing
                                qvrtr1 = Nothing
                                qvrtr2 = Nothing
                                qvrUpdateParc = Nothing
                                doc_.ComponentSet.Remove("Pnt2pnt")
                                doc_.ComponentSet.Remove("trougaoAH")
                                doc_.ComponentSet.Remove("paralelna")
                                doc_.ComponentSet.Remove("pnts")

                            Else

                                'paralelogaram
                                'seces
                                PrintLine(idIzvestaj, "Povrsina segmenta koji se ispituje: " & qvrPrvi.Table.RecordSet.Item(0).DataText(3))
                                PrintLine(idIzvestaj, "Povrsina koju trazimo: " & povrsina_(i))
                                'Dodajes u sece n ti segment
                                drwS.ObjectSet.RemoveAll()
                                drwS.ObjectSet.Add(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))).Geom)

                                'doc_.Save()
                                Dim strB, strA As Double 'odredivanje strana a i b

                                Try
                                    If prvi <> drugi Then
                                        Dim strAB() As Double = podeliParceluViseDelova_strAstrB_verzijaPrviRazlicitDrugi(prvi, drugi, doc_)
                                        strA = strAB(0) : strB = strAB(1)
                                    Else
                                        Dim strAB() As Double = podeliParceluViseDelova_strAstrB_verzijaPrviJednakDrugi(prvi, pnt1_, pnt2_, doc_)
                                        strA = strAB(0) : strB = strAB(1)
                                    End If
                                Catch ex As Exception
                                    PrintLine(idIzvestaj, "GRESKA: kod odredivanja strana za parcelu " & brParc_ & "prekird operacije")
                                    Exit Function
                                End Try

                                Dim htemp = H_racunanjeDirektno(strA, strB, drwS.ObjectSet.Item(0).Geom.AreaNative, povrsina_(i))

                                PrintLine(idIzvestaj, "Visina: " & htemp)

                                podeliParceluViseDelova_KreirajPresecnuLiniju(ugao_, d_, htemp, pnt1_, pnt2_, doc_, drwL)

                                'sklonis  ovaj segment iz Original
                                drwO.ObjectSet.Remove(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1)))) 'sada ti treba presek ove linije i poligona!

                                'delis segment u Seci
                                mAnalyzer.Split(drwS, drwS, drwS.ObjectSet, drwL.ObjectSet) ' ovde pravi dve nove parcele - kljucan momenat!


                                If drwS.ObjectSet.Count <> 2 Then
                                    'imas problem ovde ga treba vratiti na nesto ali na sta!
                                    PrintLine(idIzvestaj, "Greska: " & brParc_ & " nije presecena kako treba proveri ovo rucno")
                                    Exit Function
                                End If

                                PrintLine(idIzvestaj, "Povrsine podeljenih segmenata: ")
                                For l = 0 To drwS.ObjectSet.Count - 1
                                    PrintLine(idIzvestaj, "Segment: " & drwS.ObjectSet.Item(l).Geom.AreaNative)
                                Next


                                'treba odvojiti segmente onaj koji ide u original i onaj koji ide u copy!
                                Dim qvrRazdvoj As Query = doc_.NewQuery("razdvoj")

                                Try
                                    qvrRazdvoj.Text = "SELECT [TempSeci].[ID] FROM [TempSeci], [TempOriginal] WHERE Touches([TempSeci].[Geom (I)],[TempOriginal].[Geom (I)])"
                                    qvrRazdvoj.RunEx(True)
                                    drwO.ObjectSet.Add(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRazdvoj.Table.RecordSet.Item(0).DataText(1))).Geom) 'prvo dodajes ovaj sto je ostao
                                    drwS.ObjectSet.Remove(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRazdvoj.Table.RecordSet.Item(0).DataText(1))))
                                Catch ex As Exception
                                    'javlja se u slucaju da je poslednji segment u original
                                    'ovde mi fali NOT!
                                    doc_.Save()
                                    qvrRazdvoj.Text = "SELECT [ID] FROM [TempSeci] WHERE Touches([ID],AssignCoordSys(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT))) or Touches([ID],AssignCoordSys(NewPoint(" & pnt2_(0) & "," & pnt2_(1) & "),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)))"
                                    qvrRazdvoj.RunEx(True)
                                    drwO.ObjectSet.Add(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRazdvoj.Table.RecordSet.Item(0).DataText(1))).Geom) 'prvo dodajes ovaj sto je ostao
                                    drwS.ObjectSet.Remove(drwS.ObjectSet.Item(drwS.ObjectSet.ItemByID(qvrRazdvoj.Table.RecordSet.Item(0).DataText(1))))
                                End Try
                                'doc_.Save()

                                'iseceni objekat selis u copy i pravis union
                                drwC.ObjectSet.Add(drwS.ObjectSet.Item(0).Geom)
                                mAnalyzer.Union(drwC, drwC, drwC.ObjectSet)

                                'pronalazis dve nove tacke
                                Dim updateKoord_ As Manifold.Interop.Query = doc_.NewQuery("updatepnts")
                                'updateKoord_.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.parcele_layerName & Chr(34) & " as COMPONENT); SELECT CentroidX(GG),CentroidY(GG) from (SELECT distinct GG FROM (SELECT (IntersectLine(A.L,B.P)) as L from (SELECT [Geom (I)] as L FROM [TempLinije]) as A, (SELECT [Geom (I)] as P FROM [TempSeci] SPLIT BY Branches(IntersectLine(Boundary([Geom (I)]),Boundary([Geom (I)]))) as P) as B ) split by Coords(L) as GG) as C WHERE C.GG not in (SELECT MM FROM [TempLinije] SPLIT BY Coords([Geom (I)]) as MM) "
                                updateKoord_.Text = "SELECT CentroidX(pnt_),CentroidY(pnt_) FROM (SELECT pnt_ FROM [TempSeci] SPLIT BY Coords([Geom (I)]) as pnt_ ),[TempLinije] WHERE Touches(pnt_,[TempLinije].[Geom (I)]) "
                                updateKoord_.RunEx(True)
                                doc_.Save()
                                pnt1_(0) = updateKoord_.Table.RecordSet.Item(0).DataText(1) : pnt1_(1) = updateKoord_.Table.RecordSet.Item(0).DataText(2)
                                pnt2_(0) = updateKoord_.Table.RecordSet.Item(1).DataText(1) : pnt2_(1) = updateKoord_.Table.RecordSet.Item(1).DataText(2)

                                PrintLine(idIzvestaj, "Nove koordinate dve tacke: " & pnt1_(0) & "," & pnt1_(1) & "," & pnt2_(0) & "," & pnt2_(1))

                                qvrRazdvoj = Nothing : doc_.ComponentSet.Remove("razdvoj")
                                updateKoord_ = Nothing : doc_.ComponentSet.Remove("updatepnts")

                                'prebacujes u parcele i dodajes broj vlasnika i broj parcele
                                PrintLine(idIzvestaj, "Segment koji smo prebacili: " & drwC.ObjectSet.Item(0).Geom.AreaNative & " razlika: " & (drwC.ObjectSet.Item(0).Geom.AreaNative - povrsina_(i)))

                                drw.ObjectSet.Add(drwC.ObjectSet.Item(0).Geom)
                                'sada ubacujes sve to u parcele

                                podeliParceluViseDelova_UpdateRecordParcele(doc_, brParc_, idVlasnika_(i))

                                drwC.ObjectSet.RemoveAll()


                            End If

                            doc_.Save()

                            Exit For
                        Else
                            doc_.Save()
                            'sada ga samo iskopiras!
                            drwC.ObjectSet.Add(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))).Geom)
                            povrsina_(i) = povrsina_(i) - drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))).Geom.AreaNative
                            Dim updateKoord_ As Query = doc_.NewQuery("updatepnts")

                            drwO.ObjectSet.Remove(drwO.ObjectSet.Item(drwO.ObjectSet.ItemByID(qvrPrvi.Table.RecordSet.Item(0).DataText(1))))
                            doc_.Save() 'izgleda da mora

                            If drwO.ObjectSet.Count = 0 Then
                                drwO.ObjectSet.Add(drwS.ObjectSet.Item(0).Geom)
                            End If

                            updateKoord_.Text = "OPTIONS COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT); SELECT CentroidX(pnt1),CentroidY(pnt1) FROM ((SELECT pnt1 FROM [TempCopy] SPLIT BY coords([Geom (I)]) pnt1) as A, (SELECT [Geom (I)] as lin1 FROM [TempOriginal]) as B ) WHERE Touches(A.pnt1,B.lin1)"
                            updateKoord_.RunEx(True)
                            doc_.Save()

                            Try
                                pnt1_(0) = updateKoord_.Table.RecordSet.Item(0).DataText(1) : pnt1_(1) = updateKoord_.Table.RecordSet.Item(0).DataText(2)
                                pnt2_(0) = updateKoord_.Table.RecordSet.Item(1).DataText(1) : pnt2_(1) = updateKoord_.Table.RecordSet.Item(1).DataText(2)
                            Catch ex As Exception
                                'MsgBox("Proveri sta nije u redu")
                                PrintLine(idIzvestaj, "Greska: na parceli : " & brParc_ & " greska kod prenosenja copy")
                            End Try
                            updateKoord_ = Nothing
                            doc_.ComponentSet.Remove("updatepnts")

                            'doc_.Save()
                            i = i - 1
                            Exit For
                        End If
                    Next
                End If

                doc_.ComponentSet.Remove("prvi")
                doc_.ComponentSet.Remove("drugi")
            Next

        Catch ex As Exception
            PrintLine(idIzvestaj, "Greska: na parceli : " & brParc_)
        End Try

        drwO = Nothing
        drwS = Nothing
        drw = Nothing
        drwL = Nothing
        doc_.ComponentSet.Remove("tempCopy")
        doc_.ComponentSet.Remove("tempLinije")
        doc_.ComponentSet.Remove("tempOriginal")
        doc_.ComponentSet.Remove("TempSeci")
        doc_.Save()
    End Function
    Public Sub podeliParceluViseDelova_KreirajPresecnuLiniju2(ByVal ugao_ As Double, ByVal d_ As Double, ByVal htemp As Double, ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal doc_ As Manifold.Interop.Document, ByVal drwL As Manifold.Interop.Drawing, ByVal kofStrane As Double)
        Dim xupr, yupr, x1, x2, y1, y2 As Double
        xupr = Math.Sin(urad(ugao_)) * htemp + ((pnt1_(0) + pnt2_(0)) / 2) : yupr = Math.Cos(urad(ugao_)) * htemp + ((pnt1_(1) + pnt2_(1)) / 2)
        'sada imas paralelnu liniju
        x1 = xupr + Math.Sin(urad(ugao_ - 90)) * (-kofStrane * d_)
        y1 = yupr + Math.Cos(urad(ugao_ - 90)) * (-kofStrane * d_)
        x2 = xupr + Math.Sin(urad(ugao_ - 90)) * (kofStrane * d_)
        y2 = yupr + Math.Cos(urad(ugao_ - 90)) * (kofStrane * d_)
        Dim pntNL As PointSet = doc_.Application.NewPointSet
        pntNL.Add(doc_.Application.NewPoint(x1, y1))
        pntNL.Add(doc_.Application.NewPoint(x2, y2))
        Dim nl_ As Geom = doc_.Application.NewGeom(GeomType.GeomLine, pntNL)
        drwL.ObjectSet.Add(nl_)
    End Sub
    Public Sub podeliParceluViseDelova_KreirajPresecnuLiniju(ByVal ugao_ As Double, ByVal d_ As Double, ByVal htemp As Double, ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal doc_ As Manifold.Interop.Document, ByVal drwL As Manifold.Interop.Drawing, ByVal kofStrane As Double)
        Dim xupr, yupr, x1, x2, y1, y2 As Double
        xupr = Math.Sin(urad(ugao_)) * htemp + ((pnt1_(0) + pnt2_(0)) / 2) : yupr = Math.Cos(urad(ugao_)) * htemp + ((pnt1_(1) + pnt2_(1)) / 2)
        'sada imas paralelnu liniju
        x1 = xupr + Math.Sin(urad(ugao_ - 90)) * (-kofStrane * d_)
        y1 = yupr + Math.Cos(urad(ugao_ - 90)) * (-kofStrane * d_)
        x2 = xupr + Math.Sin(urad(ugao_ - 90)) * (kofStrane * d_)
        y2 = yupr + Math.Cos(urad(ugao_ - 90)) * (kofStrane * d_)
        Dim pntNL As PointSet = doc_.Application.NewPointSet
        pntNL.Add(doc_.Application.NewPoint(x1, y1))
        pntNL.Add(doc_.Application.NewPoint(x2, y2))
        Dim nl_ As Geom = doc_.Application.NewGeom(GeomType.GeomLine, pntNL)
        drwL.ObjectSet.RemoveAll() 'zajebava ovde! ali ne znam zasto 
        drwL.ObjectSet.Add(nl_)
    End Sub
    Public Sub podeliParceluViseDelova_KreirajPresecnuLiniju(ByVal ugao_ As Double, ByVal d_ As Double, ByVal htemp As Double, ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal doc_ As Manifold.Interop.Document, ByVal drwL As Manifold.Interop.Drawing)
        Dim xupr, yupr, x1, x2, y1, y2 As Double
        xupr = Math.Sin(urad(ugao_)) * htemp + ((pnt1_(0) + pnt2_(0)) / 2) : yupr = Math.Cos(urad(ugao_)) * htemp + ((pnt1_(1) + pnt2_(1)) / 2)
        'sada imas paralelnu liniju
        x1 = xupr + Math.Sin(urad(ugao_ - 90)) * (-2 * d_)
        y1 = yupr + Math.Cos(urad(ugao_ - 90)) * (-2 * d_)
        x2 = xupr + Math.Sin(urad(ugao_ - 90)) * (2 * d_)
        y2 = yupr + Math.Cos(urad(ugao_ - 90)) * (2 * d_)
        Dim pntNL As PointSet = doc_.Application.NewPointSet
        pntNL.Add(doc_.Application.NewPoint(x1, y1))
        pntNL.Add(doc_.Application.NewPoint(x2, y2))
        Dim nl_ As Geom = doc_.Application.NewGeom(GeomType.GeomLine, pntNL)
        drwL.ObjectSet.RemoveAll() 'zajebava ovde! ali ne znam zasto 
        drwL.ObjectSet.Add(nl_)
    End Sub

    Public Sub podeliParceluViseDelova_UpdateRecordParcele(ByVal doc_ As Manifold.Interop.Document, ByVal brParc_ As String, ByVal idVlasnika_ As Integer)
        Dim qvrUpdateParc As Manifold.Interop.Query
        qvrUpdateParc = doc_.NewQuery("updateP")
        qvrUpdateParc.Text = "update [" & My.Settings.layerName_parcele & "] set [" & My.Settings.parcele_fieldName_brParcele & "]=" & Chr(34) & brParc_ & Chr(34) & ", [" & My.Settings.parcele_fieldName_Vlasnik & "]=" & idVlasnika_ & " where [ID]=(select top 1 [ID] from [" & My.Settings.layerName_parcele & "] order by [ID] Desc)"
        qvrUpdateParc.RunEx(True)
        qvrUpdateParc = Nothing : doc_.ComponentSet.Remove("updateP")
    End Sub

    Private Function podeliParceluViseDelova_strAstrB_verzijaPrviJednakDrugi(ByVal idPrvi_ As Integer, ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal doc_ As Manifold.Interop.Document) As Double()
        Dim stranaA, stranaB As Query
        Dim rez(1) As Double
        doc_.Save()
        stranaA = doc_.NewQuery("stranaA")
        'stranaA.Text = "SELECT length(line_),cstr(CGeomWKB(line_)),(CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_)))/(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_))) as ugao_,cstr(cgeomwkb(startpoint(line_))),cstr(cgeomWKB(endPoint(line_)))  FROM (SELECT line_ FROM ( SELECT ConvexHull(AllBranches([Geom (I)])) as P FROM [TempOriginal] WHERE [ID]=" & prvi & ") split by Branches(IntersectLine(Boundary(P),Boundary(P))) as line_ ),[TempOriginal] WHERE ((EndPoint(line_)=AssignCoordSys(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "),COORDSYS(" & Chr(34) & My.Settings.parcele_layerName & Chr(34) & " as COMPONENT)) AND StartPoint(line_)=AssignCoordSys(NewPoint(" & pnt2_(0) & "," & pnt2_(1) & "),COORDSYS(" & Chr(34) & My.Settings.parcele_layerName & Chr(34) & " as COMPONENT))) OR (StartPoint(line_)=AssignCoordSys(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "),COORDSYS(" & Chr(34) & My.Settings.parcele_layerName & Chr(34) & " as COMPONENT)) AND endpoint(line_)=AssignCoordSys(NewPoint(" & pnt2_(0) & "," & pnt2_(1) & "),COORDSYS(" & Chr(34) & My.Settings.parcele_layerName & Chr(34) & " as COMPONENT))))"
        stranaA.Text = "SELECT top 1 d_,lwkb,ugao_,spntwkb,epntwkb,lineDato,line_,abs(d_-Distance(endpoint (lineDato),endpoint(line_))) as raz_ FROM (SELECT length(line_) as d_,cstr(CGeomWKB(line_)) as lwkb,(CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_)))/(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_))) as ugao_,cstr(cgeomwkb(startpoint(line_))) as spntwkb,cstr(cgeomWKB(endPoint(line_))) as epntwkb,AssignCoordSys(newline(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "),NewPoint(" & pnt2_(0) & "," & pnt2_(1) & ")),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)) as lineDato,line_  FROM (SELECT line_ FROM ( SELECT ConvexHull(AllBranches([Geom (I)])) as P FROM [TempOriginal] WHERE [ID]=" & idPrvi_ & ") split by Branches(IntersectLine(Boundary(P),Boundary(P))) as line_ )) order by raz_ asc"
        'odredis stranu b
        stranaA.RunEx(True)
        rez(1) = stranaA.Table.RecordSet.Item(0).DataText(1)
        stranaB = doc_.NewQuery("stranaB")
        stranaB.Text = "SELECT length(line_),cstr(CGeomWKB(line_)),(CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_)))/(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_))) as ugao_,cstr(cgeomwkb(startpoint(line_))),cstr(cgeomWKB(endPoint(line_))) FROM (SELECT line_ FROM ( SELECT ConvexHull(AllBranches([Geom (I)])) as P FROM [TempOriginal] WHERE [ID]=" & idPrvi_ & ") split by Branches(IntersectLine(Boundary(P),Boundary(P))) as line_ ),[TempOriginal] WHERE not Touches (line_, AssignCoordSys(NewLine(NewPoint(" & pnt1_(0) & "," & pnt1_(1) & "), NewPoint(" & pnt2_(0) & "," & pnt2_(1) & ")),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT)))"
        stranaB.RunEx(True)
        rez(0) = stranaB.Table.RecordSet.Item(0).DataText(1) 'ako ovde prijavi gresku dali znaci da je trougao - samo kada je trougao?
        doc_.ComponentSet.Remove("stranaA") : doc_.ComponentSet.Remove("stranaB") : stranaA = Nothing : stranaB = Nothing
        Return rez
    End Function

    Private Function podeliParceluViseDelova_strAstrB_verzijaPrviRazlicitDrugi(ByVal prvi As Integer, ByVal drugi As Integer, ByVal doc_ As Manifold.Interop.Document)
        Dim stranaA, stranaB As Query
        Dim rez(1) As Double
        stranaA = doc_.NewQuery("stranaA")
        stranaA.Text = "SELECT length(line_),cstr(CGeomWKB(line_)),(CentroidX(StartPoint(line_))-CentroidX(EndPoint(line_)))/(CentroidY(StartPoint(line_))-CentroidY(EndPoint(line_))) as ugao_,cstr(cgeomwkb(startpoint(line_))),cstr(cgeomWKB(endPoint(line_))) FROM ( SELECT line_ FROM ( SELECT ConvexHull(AllBranches([Geom (I)])) as P FROM [TempOriginal] WHERE [ID]=" & drugi & ") split by Branches(IntersectLine(Boundary(P),Boundary(P))) as line_ ),[TempOriginal] WHERE [TempOriginal].[ID]=" & prvi & " AND  ClipIntersect(line_,[TempOriginal].[Geom (I)]) is not null"
        'odredis stranu b
        stranaA.RunEx(True)
        rez(1) = stranaA.Table.RecordSet.Item(0).DataText(1)
        stranaB = doc_.NewQuery("stranaB")
        stranaB.Text = "SELECT A.line_, Length(A.line_) as b_,CentroidX(StartPoint(A.line_)),CentroidY(StartPoint(A.line_)),CentroidX(EndPoint(A.line_)),CentroidY(EndPoint(A.line_)) FROM (SELECT line_ FROM (SELECT ConvexHull(AllBranches([Geom (I)])) as P FROM [TempOriginal] WHERE [ID]=" & prvi & " ) split by Branches(IntersectLine(Boundary(P),Boundary(P))) as line_ ) as A WHERE not Touches(A.line_,AssignCoordSys(CGeomWKB(" & Chr(34) & stranaA.Table.RecordSet.Item(0).DataText(2) & Chr(34) & "),COORDSYS(" & Chr(34) & My.Settings.layerName_parcele & Chr(34) & " as COMPONENT))) ORDER by b_ DESC"
        stranaB.RunEx(True)
        rez(0) = stranaB.Table.RecordSet.Item(0).DataText(2) 'ako ovde prijavi gresku dali znaci da je trougao - samo kada je trougao?
        doc_.ComponentSet.Remove("stranaA") : doc_.ComponentSet.Remove("stranaB") : stranaA = Nothing : stranaB = Nothing
        Return rez
    End Function

    Public Sub podeliPolygon2Dela_KreirajLinijeUSvakojTacki_I_PodeliPoligonNaDelove(ByVal ugao_ As Double, ByVal d_ As Double, ByVal doc_ As Manifold.Interop.Document)
        Dim x1, x2, y1, y2 As Double
        Dim drwP As Drawing = doc_.ComponentSet.Item("tempOriginal")
        Dim drwL As Drawing = doc_.ComponentSet.Item("tempLinije")
        For i = 0 To drwP.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Count - 1
            x1 = drwP.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Item(i).X + Math.Sin(urad(ugao_ - 90)) * (-2 * d_) : y1 = drwP.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Item(i).Y + Math.Cos(urad(ugao_ - 90)) * (-2 * d_)
            x2 = drwP.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Item(i).X + Math.Sin(urad(ugao_ - 90)) * (2 * d_) : y2 = drwP.ObjectSet.Item(0).Geom.BranchSet.Item(0).PointSet.Item(i).Y + Math.Cos(urad(ugao_ - 90)) * (2 * d_)
            'mozes da kreiras linije
            Dim pntsetNovaLinija As PointSet = doc_.Application.NewPointSet
            pntsetNovaLinija.Add(doc_.Application.NewPoint(x1, y1)) : pntsetNovaLinija.Add(doc_.Application.NewPoint(x2, y2))
            Dim novaLinija_ As Geom = doc_.Application.NewGeom(GeomType.GeomLine, pntsetNovaLinija)
            drwL.ObjectSet.Add(novaLinija_)
            pntsetNovaLinija = Nothing : novaLinija_ = Nothing
        Next

        Dim mAnalyzer As Manifold.Interop.Analyzer = doc_.NewAnalyzer : mAnalyzer.Split(drwP, drwP, drwP.ObjectSet, drwL.ObjectSet)
        drwP = Nothing
        drwL = Nothing
        mAnalyzer = Nothing
        ' doc_.Save()
    End Sub

    Public Sub topologijaKreirajOsnovno(ByVal doc_ As Manifold.Interop.Document)

        Dim drw_parcele As Manifold.Interop.Drawing
        Dim drw_procRazredi As Manifold.Interop.Drawing
        Dim drw_Lamele As Manifold.Interop.Drawing

        Try
            drw_parcele = doc_.ComponentSet.Item(My.Settings.layerName_parcele)
            drw_procRazredi = doc_.ComponentSet.Item(My.Settings.layerName_ProcembeniRazredi)
            drw_Lamele = doc_.ComponentSet.Item(My.Settings.layerName_table)
        Catch ex As Exception
            MsgBox("Nema svih drawinga. potrebni su> table,parcele i procembeni razredi")
            drw_parcele = Nothing
            drw_procRazredi = Nothing
            drw_Lamele = Nothing
            Exit Sub
        End Try


        'sada prvo kontrola! i to kontrola ide ovde a ne tamo!
        Dim tbl_ As Manifold.Interop.Table


        'Dim col_ As Manifold.Interop.Column
        tbl_ = drw_Lamele.OwnedTable

        If tbl_.ColumnSet.ItemByName("StatusTable") = -1 Then
            MsgBox("Nedostaje polje StatusTable - kreirajte ga pa onda idemo dalje.")
            'ovo mora da moze da se kreira
            tbl_ = Nothing
            drw_Lamele = Nothing
            drw_procRazredi = Nothing
            drw_parcele = Nothing
            Exit Sub
        End If

        tbl_ = drw_parcele.OwnedTable
        If tbl_.ColumnSet.ItemByName(My.Settings.parcele_fieldName_Vlasnik) = -1 Then
            Try
                Dim col_ As Manifold.Interop.Column = doc_.Application.NewColumnSet.NewColumn
                tbl_ = drw_parcele.OwnedTable
                col_.Name = My.Settings.parcele_fieldName_Vlasnik
                col_.Type = Manifold.Interop.ColumnType.ColumnTypeInt32
                'col_.TransferMul 
                'col_.TransferDiv 
                tbl_.ColumnSet.Add(col_)

                'sada mozes da napravis vezu sa bazom i da ukljucis tabelu koja ti treba a treba ti u stvari qvery!
                'sada treba linkovati kako to bese !
                Dim conn_ As Manifold.Interop.DataSource = doc_.NewDataSource
                conn_.ConnectionType = "ODBC"
                conn_.ConnectionString = "DSN=adorjan;SERVER=localhost;UID=root;PWD=azra220;DATABASE=adorjan;PORT=3306"
                conn_.LinkTable("kom_parcele")
                conn_.LinkTable("kom_vezaparcelavlasnik")
                'sada pokreces query koji radi update
                Dim qvrUpdateIdVlasnika As Manifold.Interop.Query = doc_.NewQuery("qvrUpdateIdVlasnika")
                qvrUpdateIdVlasnika.Text = "UPDATE (SELECT brParcele," & My.Settings.parcele_fieldName_Vlasnik & ",brParceleF,idiskazzemljista FROM (SELECT [" & My.Settings.layerName_parcele & "].[brParcele],[" & My.Settings.layerName_parcele & "].[" & My.Settings.parcele_fieldName_Vlasnik & "] FROM [" & My.Settings.layerName_parcele & "] ) as B LEFT JOIN (SELECT distinct brParceleF,idiskazzemljista FROM kom_parcele,kom_vezaparcelavlasnik WHERE kom_parcele.DEOPARCELE=0 and kom_parcele.idParc=kom_vezaparcelavlasnik.idParcele and kom_vezaparcelavlasnik.obrisan=0  ) as A on B.brparcele=A.brparceleF ) set idVlasnika=idiskazzemljista"
                doc_.Save()
                qvrUpdateIdVlasnika.RunEx(True)
                'sada mozes sve da pobrises!
                tbl_ = Nothing
                col_ = Nothing
                conn_ = Nothing
                qvrUpdateIdVlasnika = Nothing
                doc_.ComponentSet.Remove("kom_parcele")
                doc_.ComponentSet.Remove("kom_vezaparcelavlasnik")
                doc_.ComponentSet.Remove("qvrUpdateIdVlasnika")
                'dodata kolona mozda treba ono da copy
            Catch ex As Exception
                MsgBox("Nedostaje polje idVlasnika - kreirajte ga pa onda idemo dalje.Definitivno rucno - ubacite layer sa parcelama!" & ex.Message)
            End Try
        End If

        Dim i As Integer

        Try
            tbl_ = drw_Lamele.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                End If
            Next
        Catch ex As Exception

        End Try
        Try
            tbl_ = drw_procRazredi.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                End If
            Next
        Catch ex As Exception

        End Try

        Try
            tbl_ = drw_parcele.OwnedTable
            For i = 0 To tbl_.ColumnSet.Count - 1
                If Not tbl_.ColumnSet.Item(i).IsIntrinsic() And Not tbl_.ColumnSet.Item(i).Identity And Not tbl_.ColumnSet.Item(i).IsForeign Then
                    tbl_.ColumnSet.Item(i).TransferDiv = Manifold.Interop.TransferRuleDiv.TransferDivCopy
                    tbl_.ColumnSet.Item(i).TransferMul = Manifold.Interop.TransferRuleMul.TransferMulCopy
                End If
            Next
        Catch ex As Exception

        End Try


        Dim topPRazredi As Manifold.Interop.Topology = doc_.Application.NewTopology
        topPRazredi.Bind(drw_procRazredi)
        topPRazredi.Build()

        Dim topParcele As Manifold.Interop.Topology = doc_.Application.NewTopology
        topParcele.Bind(drw_parcele)
        topParcele.Build()

        Dim topLamele As Manifold.Interop.Topology = doc_.Application.NewTopology
        topLamele.Bind(drw_Lamele)
        topLamele.Build()

        Try
            topParcele.DoIntersect(topPRazredi, "parc_pr_razred")
        Catch When Err.Number = -2147352567
            'sada treba pokrenuti normalizacju
            doc_.ComponentSet.Remove("parc_pr_razred")
            Dim analizer_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
            analizer_.NormalizeTopology(drw_parcele, drw_parcele.ObjectSet)
            topParcele.Bind(drw_parcele)
            topParcele.Build()
            topParcele.DoIntersect(topPRazredi, "parc_pr_razred")
            analizer_ = Nothing
        Catch
            MsgBox(Err.Description)
        End Try

        Try
            topPRazredi.DoIntersect(topLamele, "tab_proc_raz")
        Catch When Err.Number = -2147352567
            'sada treba pokrenuti normalizacju
            doc_.ComponentSet.Remove("tab_proc_raz")
            Dim analizer_ As Manifold.Interop.Analyzer = doc_.NewAnalyzer
            analizer_.NormalizeTopology(topPRazredi, topPRazredi.ObjectSet)
            topPRazredi.Bind(drw_procRazredi)
            topPRazredi.Build()
            topPRazredi.DoIntersect(topLamele, "tab_proc_raz")
            analizer_ = Nothing
        Catch
            MsgBox(Err.Description)
        End Try

        'BITNO: TABLE MORAJU DA BUDU DEFINISANE SA 0 AKO NE UCESTVUJU U KOMASACIJI!

        Dim topParcPrRaz As Manifold.Interop.Topology = doc_.Application.NewTopology
        topParcPrRaz.Bind(doc_.ComponentSet.Item(doc_.ComponentSet.ItemByName("parc_pr_razred")))
        topParcPrRaz.Build()

        topParcPrRaz.DoIntersect(topLamele, "par_pr_raz_tab")
        topParcPrRaz = Nothing
        'doc.Save()
        tbl_ = Nothing
        drw_Lamele = Nothing
        drw_procRazredi = Nothing
        drw_parcele = Nothing
        doc_.Save()
        MsgBox("Topologija kreirana")
    End Sub

    Public Sub ObrisiSveMoguce(ByVal doc_ As Manifold.Interop.Document)
        On Error Resume Next

        doc_.ComponentSet.Remove("prvi")
        doc_.ComponentSet.Remove("drugi")
        doc_.ComponentSet.Remove("razdvoj")
        doc_.ComponentSet.Remove("tempCopy")
        doc_.ComponentSet.Remove("tempLinije")
        doc_.ComponentSet.Remove("tempOriginal")
        doc_.ComponentSet.Remove("TempSeci")
        doc_.ComponentSet.Remove("updateP")
        doc_.ComponentSet.Remove("updatepnts")
        doc_.ComponentSet.Remove("stranaA")
        doc_.ComponentSet.Remove("stranaB")
        doc_.ComponentSet.Remove("Pnt2pnt")
        doc_.ComponentSet.Remove("TrougaoAH")


    End Sub

    Public Function redusePolygonPointNumberTo4(ByVal mat_A_(,) As Double) As Double(,)
        On Error Resume Next

        'uf sada bi trebalo da 
        'logika je da je prva tacka u nizu istih ona koja treba!
        Dim bb_ As Integer = (mat_A_.Length / (UBound(mat_A_) + 1))
        Dim nizUglova(-1) As Double
        'sada moze povrsdina
        Dim suma_ As Double = 0
        Dim i As Integer
        For i = 0 To bb_ - 1
            ReDim Preserve nizUglova(i)
            If (i = (bb_ - 1)) Then
                nizUglova(i) = NiAnaB(mat_A_(1, i), mat_A_(2, i), mat_A_(1, 0), mat_A_(2, 0))
            Else
                nizUglova(i) = NiAnaB(mat_A_(1, i), mat_A_(2, i), mat_A_(1, i + 1), mat_A_(2, i + 1))
            End If

        Next

        For i = 0 To UBound(nizUglova) - 1
            'uf sta sad
            For j = i + 1 To UBound(nizUglova)
                If Math.Round(nizUglova(i), 5) = Math.Round(nizUglova(j), 5) Then
                    nizUglova(j) = -1
                End If
            Next
        Next
        Dim matB_(3, 3) As Double
        Dim brojac_ As Integer = 0
        For i = 0 To UBound(nizUglova)
            If nizUglova(i) <> -1 Then
                'tacka ostaje
                matB_(0, brojac_) = mat_A_(0, i) : matB_(1, brojac_) = mat_A_(1, i) : matB_(2, brojac_) = mat_A_(2, i) : matB_(3, brojac_) = mat_A_(3, i)
                brojac_ += 1
            End If
        Next
        Return matB_
    End Function

    Public Function direkcioniUgaoUpravnaUPolygonu(ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal doc As Manifold.Interop.Document) As Double()
        Dim izlaz_(1) As Double
        Dim ugao_ As Double = NiAnaB(pnt1_(0), pnt1_(1), pnt2_(0), pnt2_(1)) + 90
        Dim d_ As Double = Math.Sqrt((pnt1_(0) - pnt2_(0)) ^ 2 + (pnt1_(1) - pnt2_(1)) ^ 2)
        Dim pnt(1) As Double
        Dim okrenuo As Boolean = False
ovde_:
        pnt(0) = Math.Sin(urad(ugao_)) * 2 + ((pnt1_(0) + pnt2_(0)) / 2) : pnt(1) = Math.Cos(urad(ugao_)) * 2 + ((pnt1_(1) + pnt2_(1)) / 2)
        Dim pntUpr As Manifold.Interop.Point = doc.Application.NewPoint(pnt(0), pnt(1)) : Dim pntGeom As Manifold.Interop.Geom = doc.Application.NewGeom(GeomType.GeomPoint, pntUpr)
        Dim drw_ As Manifold.Interop.Drawing = doc.ComponentSet("tempNormTopo")
        Dim obj2 As Manifold.Interop.GeomSet = doc.NewGeomSet(drw_.ObjectSet.Item(0).Geom)
        'proveris dali je strana dobra
        If obj2.Item(0).CheckContains(pntGeom) = False Then 'ovako mozes da proveris dali je ok tacka!
            ugao_ += 180
            If okrenuo = False Then
                okrenuo = True
                GoTo ovde_
            End If
        End If
        pntGeom = Nothing : pntUpr = Nothing : obj2 = Nothing
        izlaz_(0) = ugao_ : izlaz_(1) = d_
        Return izlaz_
    End Function

    Public Function direkcioniUgaoUpravnaUPolygonu(ByVal pnt1_() As Double, ByVal pnt2_() As Double, ByVal polygonwkt As String, ByVal doc As Manifold.Interop.Document) As Double()
        Dim izlaz_(1) As Double
        Dim ugao_ As Double = NiAnaB(pnt1_(0), pnt1_(1), pnt2_(0), pnt2_(1)) + 90
        Dim d_ As Double = Math.Sqrt((pnt1_(0) - pnt2_(0)) ^ 2 + (pnt1_(1) - pnt2_(1)) ^ 2)
        Dim pnt(1) As Double
        Dim okrenuo As Boolean = False
ovde_:
        pnt(0) = Math.Sin(urad(ugao_)) * 2 + ((pnt1_(0) + pnt2_(0)) / 2) : pnt(1) = Math.Cos(urad(ugao_)) * 2 + ((pnt1_(1) + pnt2_(1)) / 2)
        Dim pntUpr As Manifold.Interop.Point = doc.Application.NewPoint(pnt(0), pnt(1)) : Dim pntGeom As Manifold.Interop.Geom = doc.Application.NewGeom(GeomType.GeomPoint, pntUpr)
        Dim pol_ As Manifold.Interop.Geom = doc.Application.NewGeomFromTextWKT(polygonwkt) : Dim obj2 As Manifold.Interop.GeomSet = doc.NewGeomSet(pol_)
        'proveris dali je strana dobra
        If obj2.Item(0).CheckContains(pntGeom) = False Then 'ovako mozes da proveris dali je ok tacka!
            ugao_ += 180
            If okrenuo = False Then
                okrenuo = True
                GoTo ovde_
            End If
        End If
        pntGeom = Nothing : pntUpr = Nothing : obj2 = Nothing
        izlaz_(0) = ugao_ : izlaz_(1) = d_
        Return izlaz_
    End Function
    Public Function pointXYfromWKT(ByVal pnt_ As String) As Double()
        pnt_ = pnt_.Replace("POINT", "")
        pnt_ = pnt_.Replace("(", "")
        pnt_ = pnt_.Replace(")", "")
        Dim a_ = Split(pnt_, " ")
        Dim pera_(1) As Double
        pera_(0) = a_(0)
        pera_(1) = a_(1)
        Return pera_
    End Function

    'Public Function NiAnaB(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
    '    'samo napomena x ti je 7
    '    Dim dx, dy As Double
    '    dy = x2 - x1 : dx = y2 - y1
    '    'sada proveravas koji je slucaj
    '    If dy = 0 And dx > 0 Then
    '        NiAnaB = 0 '90
    '    ElseIf dy = 0 And dx < 0 Then
    '        NiAnaB = udec_r(Math.PI)
    '    ElseIf dy > 0 And dx = 0 Then
    '        NiAnaB = udec_r(Math.PI / 2)
    '    ElseIf dy < 0 And dx = 0 Then
    '        NiAnaB = udec_r((Math.PI / 2) * 3)
    '    ElseIf dy > 0 And dx > 0 Then
    '        'znaci da ne dodajes nista
    '        NiAnaB = udec_r(Math.Atan(dy / dx))
    '    ElseIf dy > 0 And dx < 0 Then
    '        NiAnaB = udec_r(Math.Abs(Math.Atan(dx / dy))) + 90
    '    ElseIf dy < 0 And dx < 0 Then
    '        NiAnaB = udec_r(Math.Atan(dy / dx)) + 180
    '    ElseIf dy < 0 And dx > 0 Then
    '        NiAnaB = udec_r(Math.Abs(Math.Atan(dx / dy))) + 270
    '    End If

    'End Function

    Public Function NiAnaBACAD(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
        'samo napomena x ti je 7
        Dim dx, dy As Double
        dx = x2 - x1 : dy = y2 - y1
        'sada proveravas koji je slucaj
        NiAnaBACAD = udec_r(Math.Atan(dy / dx))

        If NiAnaBACAD < 0 Then
            NiAnaBACAD = 360 + NiAnaBACAD
        End If

        'If dx > 0 And dy > 0 Then
        '    NiAnaBACAD = udec_r(Math.Atan(dy / dx))
        'ElseIf dx > 0 And dy < 0 Then
        '    NiAnaBACAD = udec_r(Math.Atan(dx / dy)) + 270
        'ElseIf dx < 0 And dy < 0 Then
        '    NiAnaBACAD = udec_r(Math.Atan(dy / dx)) + 180
        'Else
        '    NiAnaBACAD = udec_r(Math.Atan(dx / dy)) + 90
        'End If


    End Function

    Public Sub stampajResenje(brResenja As String, docApp_ As Microsoft.Office.Interop.Word.Application, wDoc_ As Microsoft.Office.Interop.Word.Document)

        Dim conn_ As New MySql.Data.MySqlClient.MySqlConnection(My.Settings.mysqlConnString) : Dim comm_ As New MySql.Data.MySqlClient.MySqlCommand("", conn_)
        conn_.Open()
        comm_.CommandText = ""

        Dim freefile_ As Integer = FreeFile() : Dim mysqladap_ As New MySql.Data.MySqlClient.MySqlDataAdapter("", conn_)
        'otvoris word kao template


        Dim prelom_ = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak
        Dim sumaP_, sumaV_

        Dim putujuciText_ As String = ""
        'idemo prvo na identifikaciju
        If My.Settings.resenja_pismo = "Latinica" Then putujuciText_ = "Iskaz broj " Else putujuciText_ = "Исказ број "

        Dim bokMarks_ As Word.Bookmarks = wDoc_.Bookmarks

        Dim myreader_ As MySqlDataReader ' = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        comm_.CommandText = "SELECT fs_raspravnizapisnik.brojPredmeta FROM kom_izt INNER JOIN fs_raspravnizapisnik on kom_izt.idpl=fs_raspravnizapisnik.idPL where autoid=" & brResenja

        Try
            conn_.Open()
        Catch ex As Exception

        End Try

        myreader_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        If myreader_.HasRows Then
            'znaci da ima nesto
            myreader_.Read()
            Try
                bokMarks_.Item("brPredmeta").Range.Text = myreader_.GetValue(0)
            Catch ex As Exception

            End Try

        End If
        myreader_.Close()




        Try
            bokMarks_.Item("brIskaza_01").Range.Text = putujuciText_ & brResenja
        Catch ex As Exception

        End Try


        For i = 2 To 9

            Try
                bokMarks_.Item("brIskaza_0" & i).Range.Text = brResenja
            Catch ex As Exception

            End Try

        Next

        If My.Settings.resenja_pismo = "Latinica" Then
            'comm_.CommandText = "SELECT DISTINCT concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') = '', '', concat('(', IMEOCA, ')')), ' ', ifnull(IME, ''), ', ', ifnull(MESTO, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ,''), ', ЈМБГ: ', ifnull(MATBRGRA, ''), ', sa pravom svojine i idealnim udelom ', Udeo ) AS indikacije_ FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika WHERE idiskazzemljista = " & brResenja & " AND kom_vezaparcelavlasnik.obrisan = 0"
            comm_.CommandText = "SELECT DISTINCT concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') = '', '', concat('(', IMEOCA, ')')), ' ', ifnull(IME, ''), ', ', ifnull(MESTO, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ,''), ', ЈМБГ: ', ifnull(MATBRGRA, ''), ', ', (SELECT naziv from kat_svojina where sifra=OBLIKSVOJINE) ,', ', (select naziv from kat_pravovrsta where SIFRA=VRSTAPRAVA) ,' i ',(select naziv from kat_pravoobim where sifra=OBIMPRAVA),':', Udeo ) AS indikacije_ FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika WHERE idiskazzemljista = " & brResenja & " AND kom_vezaparcelavlasnik.obrisan = 0"
        Else
            'comm_.CommandText = "SELECT DISTINCT concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') = '', '', concat('(', IMEOCA, ')')), ' ', ifnull(IME, ''), ', ', ifnull(MESTO, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ,''), ', ЈМБГ: ', ifnull(MATBRGRA, ''), ', са правом:', (select naziv from kat_pravovrsta where SIFRA=VRSTAPRAVA) ,' и ',(select naziv from kat_pravoobim where sifra=OBIMPRAVA),':', Udeo ) AS indikacije_ FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika WHERE idiskazzemljista = " & brResenja & " AND kom_vezaparcelavlasnik.obrisan = 0"
            comm_.CommandText = "SELECT DISTINCT concat( ifnull(PREZIME, ''), ' ', IF ( ifnull(IMEOCA, '') = '', '', concat('(', IMEOCA, ')')), ' ', ifnull(IME, ''), ', ', ifnull(MESTO, ''), ', ', ifnull(ULICA, ''), ' ', ifnull(BROJ, ''), ' ', ifnull(UZBROJ,''), ', ЈМБГ: ', ifnull(MATBRGRA, ''), ', ', (SELECT naziv from kat_svojina where sifra=OBLIKSVOJINE) ,', ', (select naziv from kat_pravovrsta where SIFRA=VRSTAPRAVA) ,' и ',(select naziv from kat_pravoobim where sifra=OBIMPRAVA),':', Udeo ) AS indikacije_ FROM kom_vezaparcelavlasnik LEFT OUTER JOIN kom_vlasnik ON kom_vezaparcelavlasnik.idVlasnika = kom_vlasnik.idVlasnika WHERE idiskazzemljista = " & brResenja & " AND kom_vezaparcelavlasnik.obrisan = 0"
        End If

        Try
            conn_.Open()
        Catch ex As Exception

        End Try

        myreader_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        Dim txt_ = ""
        Do While myreader_.Read
            txt_ = txt_ & myreader_.GetValue(0) & vbNewLine
        Loop
        myreader_.Close()

        'sada mozes da uradis nesto al sta?
        Try
            bokMarks_.Item("indikacijeFull_01").Range.Text = txt_
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("indikacijeFull_02").Range.Text = txt_
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("indikacijeFull_03").Range.Text = txt_
        Catch ex As Exception

        End Try

        bokMarks_.Item("pageBreak_01").Range.InsertBreak(prelom_)

        'SADA IDEMO NA STARO STANJE!!!!!!!!!!!

        'STARO STANJE

        Try
            conn_.Open()
        Catch ex As Exception

        End Try

        Dim brPolja As Integer = 7
        If My.Settings.resenja_pismo = "Latinica" Then
            mysqladap_.SelectCommand.CommandText = "SELECT brparcelef, naziv, concat(ucase(substring(skulture,1,1)),lcase(substring(skulture,2))) as skulture, Pkat P, round((pkat / povrsina) * v, 2) AS V, round((pkat / povrsina) * dv, 2) AS dv, round((pkat / povrsina) * Vnad, 2) AS vnad, 1 redosled FROM ( SELECT D.brparcelef, potes, skulture, Pkat, povrsina, round(vsuma, 2) AS V, ( round(vsuma, 2) - round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 )) AS dV, round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 ) AS Vnad FROM ( SELECT DISTINCT idParcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & "  AND obrisan = 0 ) AS A INNER JOIN ( SELECT brParceleF, POTES, idparc FROM kom_parcele WHERE obrisan = 0 AND UKOMASACIJI <> 0 ) AS B ON A.idParcele = B.idparc LEFT JOIN ( SELECT ( prazred_1 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 1 ) + prazred_2 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 2 ) + prazred_3 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 3 ) + prazred_4 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 4 ) + prazred_5 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 5 ) + prazred_6 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 6 ) + prazred_7 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 7 ) + prazred_8 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 8 )) AS vsuma, Povrsina, idparc FROM kom_kfmss WHERE obrisan = 0 ) AS C ON A.idparcele = C.idparc LEFT JOIN ( SELECT NAZIVKULT AS skulture, ( hektari * 10000 + ari * 100 + metri ) AS Pkat, brParceleF FROM kom_parcele LEFT JOIN kat_kultura ON SKULTURE = idKulture WHERE deoparcele = 1 AND obrisan = 0 ) AS D ON B.brparcelef = D.brparcelef ) AS GG LEFT JOIN kat_potesi ON GG.potes = SIFRA UNION ( SELECT NULL, NULL, 'Ukupno u iskazu:', sum(povrsina) PP, sum(vsuma) VV, NULL, round(sum(vsuma)*(select opisText from kom_parametri where opis='koeficijent_umanjenja'),2) VV, 2 AS redosled FROM ( SELECT DISTINCT idparcele FROM kom_vezaparcelavlasnik WHERE obrisan = 0 AND idiskazzemljista = " & brResenja & "  ) AA INNER JOIN ( SELECT ( prazred_1 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 1 ) + prazred_2 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 2 ) + prazred_3 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 3 ) + prazred_4 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 4 ) + prazred_5 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 5 ) + prazred_6 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 6 ) + prazred_7 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 7 ) + prazred_8 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 8 )) AS vsuma, Povrsina, idparc FROM kom_kfmss WHERE obrisan = 0 ) HH ON idparcele = HH.idparc INNER JOIN 	(select idparc from kom_parcele where DEOPARCELE=0 and UKOMASACIJI<>0 and obrisan=0) as KK on idparcele=KK.idparc ) ORDER BY redosled, naziv, brparcelef"
        Else
            mysqladap_.SelectCommand.CommandText = "SELECT brparcelef, naziv, concat(ucase(substring(skulture,1,1)),lcase(substring(skulture,2))) as skulture, Pkat P, round((pkat / povrsina) * v, 2) AS V, round((pkat / povrsina) * dv, 2) AS dv, round((pkat / povrsina) * Vnad, 2) AS vnad, 1 redosled FROM ( SELECT D.brparcelef, potes, skulture, Pkat, povrsina, round(vsuma, 2) AS V, ( round(vsuma, 2) - round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 )) AS dV, round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 ) AS Vnad FROM ( SELECT DISTINCT idParcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & "  AND obrisan = 0 ) AS A INNER JOIN ( SELECT brParceleF, POTES, idparc FROM kom_parcele WHERE obrisan = 0 AND UKOMASACIJI <> 0 ) AS B ON A.idParcele = B.idparc LEFT JOIN ( SELECT ( prazred_1 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 1 ) + prazred_2 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 2 ) + prazred_3 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 3 ) + prazred_4 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 4 ) + prazred_5 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 5 ) + prazred_6 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 6 ) + prazred_7 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 7 ) + prazred_8 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 8 )) AS vsuma, Povrsina, idparc FROM kom_kfmss WHERE obrisan = 0 ) AS C ON A.idparcele = C.idparc LEFT JOIN ( SELECT NAZIVKULT AS skulture, ( hektari * 10000 + ari * 100 + metri ) AS Pkat, brParceleF FROM kom_parcele LEFT JOIN kat_kultura ON SKULTURE = idKulture WHERE deoparcele = 1 AND obrisan = 0 ) AS D ON B.brparcelef = D.brparcelef ) AS GG LEFT JOIN kat_potesi ON GG.potes = SIFRA UNION ( SELECT NULL, NULL, 'Укупно y исказу:', sum(povrsina) PP, sum(vsuma) VV, NULL, round(sum(vsuma)*(select opisText from kom_parametri where opis='koeficijent_umanjenja'),2) VV, 2 AS redosled FROM ( SELECT DISTINCT idparcele FROM kom_vezaparcelavlasnik WHERE obrisan = 0 AND idiskazzemljista = " & brResenja & "  ) AA INNER JOIN ( SELECT ( prazred_1 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 1 ) + prazred_2 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 2 ) + prazred_3 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 3 ) + prazred_4 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 4 ) + prazred_5 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 5 ) + prazred_6 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 6 ) + prazred_7 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 7 ) + prazred_8 * ( SELECT vrednostkoeficijenta FROM kom_koeficijenti WHERE brojkoeficijenta = 8 )) AS vsuma, Povrsina, idparc FROM kom_kfmss WHERE obrisan = 0 ) HH ON idparcele = HH.idparc INNER JOIN 	(select idparc from kom_parcele where DEOPARCELE=0 and UKOMASACIJI<>0 and obrisan=0) as KK on idparcele=KK.idparc ) ORDER BY redosled, naziv, brparcelef"
        End If

        If My.Settings.resenje_koeficijent0 = 1 Then
            'skracena verzija
            brPolja = 5
        Else
            'duga verzija
            brPolja = 7
        End If

        Dim tbl0Stanje As New DataTable : mysqladap_.Fill(tbl0Stanje)
        Dim wTable, wtable1, wtableTereti As Word.Table : Dim koje_ As Integer

        wTable = wDoc_.Tables.Add(bokMarks_.Item("Spisak_parcela_staro_stanje").Range, tbl0Stanje.Rows.Count + 2, brPolja)
        wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow)

        With wTable
            .Borders.Enable = True
            .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
            .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
            .Range.Font.Name = "Times New Roman"
            .Range.Font.Size = 9
        End With

        'sada header
        For j = 1 To brPolja
            For k = 1 To tbl0Stanje.Rows.Count
                If j = 4 Then
                    wTable.Cell(k + 1, j).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(k - 1).Item(j - 1).ToString)
                Else
                    wTable.Cell(k + 1, j).Range.Text = tbl0Stanje.Rows(k - 1).Item(j - 1).ToString
                End If
                wTable.Cell(k + 1, j).Range.Font.Bold = False
            Next
        Next

        For j = 1 To brPolja
            wTable.Cell(1, j).Range.Font.Size = 11
        Next

        wTable.Rows(1).Range.Font.Bold = True : wTable.Rows(wTable.Rows.Count-1).Range.Font.Bold = True

        If My.Settings.resenja_pismo = "Latinica" Then
            wTable.Cell(1, 1).Range.Text = "Broj parcele" : wTable.Cell(1, 2).Range.Text = "Potes"
            wTable.Cell(1, 3).Range.Text = "Kultura" : wTable.Cell(1, 4).Range.Text = "Površina" : wTable.Cell(1, 5).Range.Text = "Vrednost"
            Try
                wTable.Cell(1, 6).Range.Text = "Umanjenje vrednosti" : wTable.Cell(1, 7).Range.Text = "Vrednost za nadelu"
            Catch ex As Exception

            End Try

        Else
            wTable.Cell(1, 1).Range.Text = "Број парцеле" : wTable.Cell(1, 2).Range.Text = "Потес"
            wTable.Cell(1, 3).Range.Text = "Култура" : wTable.Cell(1, 4).Range.Text = "Површина" : wTable.Cell(1, 5).Range.Text = "Вредност"
            Try
                wTable.Cell(1, 6).Range.Text = "Умањење вредности" : wTable.Cell(1, 7).Range.Text = "Вредност за наделу"
            Catch ex As Exception

            End Try

        End If


        'wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

        'sada idemo da uradimo po procembenim razredima 
        tbl0Stanje.Rows.Clear() : tbl0Stanje.Columns.Clear()

        If My.Settings.resenja_stampaProcembeniRazredi = True Then
            'znaci stampam i procembene razrede
            mysqladap_.SelectCommand.CommandText = "SELECT sum(p1) AS p1, sum(v1) AS v1, sum(p2) AS p2, sum(v2) AS v2, sum(p3) AS p3, sum(v3) AS v3, sum(p4) AS p4, sum(v4) AS v4, sum(p5) AS p5, sum(v5) AS v5, sum(p6) AS p6, sum(v6) AS v6, sum(p7) AS p7, sum(v7) AS v7, sum(prazred_neplodno) AS p8, 0 AS v8 FROM ( SELECT DISTINCT idParcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & "  AND obrisan = 0 ) AS B INNER JOIN ( SELECT prazred_1 AS P1, prazred_1 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 1 ) AS V1, Prazred_2 AS P2, prazred_2 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 2 ) AS V2, Prazred_3 AS P3, prazred_3 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 3 ) AS V3, Prazred_4 AS P4, prazred_4 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 4 ) AS V4, Prazred_5 AS P5, prazred_5 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 5 ) AS V5, Prazred_6 AS P6, prazred_6 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 6 ) AS V6, Prazred_7 AS P7, prazred_7 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 7 ) AS V7, prazred_neplodno, idparc FROM kom_kfmss WHERE obrisan = 0 and SR<>0) AS A ON B.idParcele = A.idparc"
            mysqladap_.Fill(tbl0Stanje)

            brPolja = 3

            wTable = Nothing

            wtable1 = wDoc_.Tables.Add(bokMarks_.Item("Spisak_procembenih_razreda_staro_stanje").Range, 10, brPolja)

            With wtable1
                .Borders.Enable = True
                .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Font.Name = "Times New Roman"
                .Range.Font.Size = 9
            End With

            For p = 1 To brPolja
                wtable1.Columns.Item(p).Width = docApp_.CentimetersToPoints(3.8)
            Next

            'sada header
            'wtable1.Columns(1).Width = 25 : wtable1.Columns(2).Width = 35 : wtable1.Columns(3).Width = 35
            If My.Settings.resenja_pismo = "Latinica" Then
                wtable1.Cell(1, 1).Range.Text = "PROCEMBENI RAZRED" : wtable1.Cell(1, 2).Range.Text = "POVRŠINA" : wtable1.Cell(1, 3).Range.Text = "VREDNOST"
                wtable1.Cell(9, 1).Range.Text = "Bez razreda" : wtable1.Cell(10, 1).Range.Text = "Učesnik uneo u komasacionu masu"
            Else
                wtable1.Cell(1, 1).Range.Text = "ПРОЦЕМБЕНИ РАЗРЕД" : wtable1.Cell(1, 2).Range.Text = "ПОВРШИНА" : wtable1.Cell(1, 3).Range.Text = "ВРЕДНОСТ"
                wtable1.Cell(9, 1).Range.Text = "Без разреда" : wtable1.Cell(10, 1).Range.Text = "Учесник унео у комасациону масу"
            End If

            wtable1.Cell(2, 1).Range.Text = "1." : wtable1.Cell(2, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(0).ToString) : wtable1.Cell(2, 3).Range.Text = tbl0Stanje.Rows(0).Item(1).ToString
            wtable1.Cell(3, 1).Range.Text = "2." : wtable1.Cell(3, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(2).ToString) : wtable1.Cell(3, 3).Range.Text = tbl0Stanje.Rows(0).Item(3).ToString
            wtable1.Cell(4, 1).Range.Text = "3." : wtable1.Cell(4, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(4).ToString) : wtable1.Cell(4, 3).Range.Text = tbl0Stanje.Rows(0).Item(5).ToString
            wtable1.Cell(5, 1).Range.Text = "4." : wtable1.Cell(5, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(6).ToString) : wtable1.Cell(5, 3).Range.Text = tbl0Stanje.Rows(0).Item(7).ToString
            wtable1.Cell(6, 1).Range.Text = "5." : wtable1.Cell(6, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(8).ToString) : wtable1.Cell(6, 3).Range.Text = tbl0Stanje.Rows(0).Item(9).ToString
            wtable1.Cell(7, 1).Range.Text = "6." : wtable1.Cell(7, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(10).ToString) : wtable1.Cell(7, 3).Range.Text = tbl0Stanje.Rows(0).Item(11).ToString
            wtable1.Cell(8, 1).Range.Text = "7." : wtable1.Cell(8, 2).Range.Text = povrsinaParceleUkras(tbl0Stanje.Rows(0).Item(12).ToString) : wtable1.Cell(8, 3).Range.Text = tbl0Stanje.Rows(0).Item(13).ToString
            wtable1.Cell(9, 2).Range.Text = tbl0Stanje.Rows(0).Item(14).ToString : wtable1.Cell(9, 3).Range.Text = tbl0Stanje.Rows(0).Item(15).ToString


            wtable1.Cell(10, 2).Range.Text = sumaP_ ' tbl0Stanje.Rows(0).Item(0) + tbl0Stanje.Rows(0).Item(2) + tbl0Stanje.Rows(0).Item(4) + tbl0Stanje.Rows(0).Item(6) + tbl0Stanje.Rows(0).Item(8) + tbl0Stanje.Rows(0).Item(10) + tbl0Stanje.Rows(0).Item(12)
            wtable1.Cell(10, 3).Range.Text = sumaV_ 'tbl0Stanje.Rows(0).Item(1) + tbl0Stanje.Rows(0).Item(3) + tbl0Stanje.Rows(0).Item(5) + tbl0Stanje.Rows(0).Item(7) + tbl0Stanje.Rows(0).Item(9) + tbl0Stanje.Rows(0).Item(11) + tbl0Stanje.Rows(0).Item(13)
            wtable1.Rows(1).Range.Font.Bold = True : wtable1.Rows(10).Range.Font.Bold = True

            For j = 1 To brPolja
                wtable1.Cell(1, j).Range.Font.Size = 11
            Next

        Else
            bokMarks_.Item("Spisak_procembenih_razreda_staro_stanje").Range.Text = ""
        End If

        wTable = Nothing : wtable1 = Nothing

        'ovde treba dodati terete u starom stanju
        Dim tbl0StanjeTereti As New DataTable
        'mysqladap_.SelectCommand.CommandText = "SELECT DISTINCT IF( PODBROJ = 0, concat(BROJPARC), concat(BROJPARC, '/', PODBROJ)) AS brp, cast(OPISTERETA as CHAR) AS OPISTERETA, ( SELECT DISTINCT concat( prezime, ' ', ifnull(imeoca, ''), ' ', ifnull(ime,''), ', ', MATBRGRA ) FROM fs_lica WHERE fs_lica.MATBRGRA = kom_tereti.MATBRGRA AND obrisan = 0 ) AS indef_ FROM ( SELECT DISTINCT idparcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & " ) A LEFT OUTER JOIN kom_tereti ON A.idParcele = kom_tereti.idParcele ORDER BY BROJPARC"
        mysqladap_.SelectCommand.CommandText = "SELECT brParceleF AS brp, cast(OPISTERETA AS CHAR) AS OPISTERETA, ( SELECT replace(replace(indikacije_,'LJ','Q'),'NJ','W') as indikacije_ FROM ( SELECT concat( prezime, ' ', ifnull(imeoca, ''), ' ', ifnull(ime, ''), ', ', matbrgra ) AS indikacije_, matbrgra FROM kom_vlasnik UNION SELECT DISTINCT concat( prezime, ' ', ifnull(imeoca, ''), ' ', ifnull(ime, ''), ', ', MATBRGRA ), matbrgra FROM fs_lica ) GG WHERE GG.MATBRGRA = kom_tereti.MATBRGRA ) AS indef_ FROM kom_tereti INNER JOIN kom_parcele ON kom_tereti.idParcele = kom_parcele.idParc AND (UKOMASACIJI > 0) AND kom_tereti.idParcele IN ( SELECT idparcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & " and obrisan=0) AND kom_tereti.obrisan=0 ORDER BY brp"
        mysqladap_.Fill(tbl0StanjeTereti)

        'prvo treba videti da li ima tereta

        If tbl0StanjeTereti.Rows.Count > 0 Then
            brPolja = 3

            'wTable = Nothing
            wtable1 = wDoc_.Tables.Add(bokMarks_.Item("Spisak_tereta_staro_stanje").Range, tbl0StanjeTereti.Rows.Count + 1, brPolja)
            wtable1.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow)
            With wtable1
                .Borders.Enable = True
                .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                '.Range.Font.Name = "Times New Roman"
                '.Range.Font.Name = "Times Roman Cirilica"
                .Range.Font.Name = "Times New Roman"
                .Range.Font.Size = 9
            End With

            For j = 1 To 3
                For k = 1 To tbl0StanjeTereti.Rows.Count
                    wtable1.Cell(k + 1, j).Range.Text = tbl0StanjeTereti.Rows(k - 1).Item(j - 1).ToString
                    wtable1.Cell(k + 1, j).Range.Font.Bold = False
                Next
            Next

            'sada header
            'wtable1.Columns(1).Width = 25 : wtable1.Columns(2).Width = 35 : wtable1.Columns(3).Width = 35
            If My.Settings.resenja_pismo = "Latinica" Then
                wtable1.Cell(1, 1).Range.Text = "Broj parcele" : wtable1.Cell(1, 2).Range.Text = "Opis tereta" : wtable1.Cell(1, 3).Range.Text = "U korist"
            Else
                wtable1.Cell(1, 1).Range.Text = "Број парцеле" : wtable1.Cell(1, 2).Range.Text = "Опис терета" : wtable1.Cell(1, 3).Range.Text = "У корист"
            End If

            wtable1.Rows(1).Range.Font.Bold = True

            For j = 1 To brPolja
                wtable1.Cell(1, j).Range.Font.Size = 11
                Select Case j
                    Case 1
                        wtable1.Columns(j).Width = docApp_.CentimetersToPoints(1.94)
                    Case 2
                        wtable1.Columns(j).Width = docApp_.CentimetersToPoints(11)
                    Case 3
                        wtable1.Columns(j).Width = docApp_.CentimetersToPoints(3.25)
                End Select
            Next

        Else
            'nema tereta
            bokMarks_.Item("Spisak_tereta_staro_stanje").Range.Text = ""
        End If

        bokMarks_.Item("pageBreak_02").Range.InsertBreak(prelom_)
        'da li da ga obrise

        'SADA IDEMO NA NOVO STANJE
        If My.Settings.resenja_pismo = "Latinica" Then
            mysqladap_.SelectCommand.CommandText = "SELECT brParcele, brdelaparc, kom_novostanjeparcela.idTable, NAZIV, concat(ucase(substring(NAZIVKULT,1,1)),lcase(substring(NAZIVKULT,2))) as NAZIVKULT, Povrsina AS ukupno_povrsina, NULL AS V, 1 redosled FROM kom_novostanjeparcela LEFT OUTER JOIN kat_potesi ON idPotes = SIFRA LEFT OUTER JOIN kat_kultura ON kom_novostanjeparcela.SKulture = kat_kultura.idKulture WHERE idIskaz = " & brResenja & " UNION SELECT brParcele, NULL brdelaparc, kom_novostanjeparcela.idTable, NULL NAZIV, 'Ukupno:' NAZIVKULT, sum(Povrsina) AS ukupno_povrsina, nadeljenoVrednost AS V, 2 redosled FROM kom_novostanjeparcela LEFT OUTER JOIN kom_tablenadela ON kom_novostanjeparcela.idtable = kom_tablenadela.IdTable AND kom_novostanjeparcela.rednibrNadele = kom_tablenadela.redniBrojNadele WHERE idiskaz = " & brResenja & " GROUP BY brparcele UNION SELECT 100000 AS brParcele, NULL brdelaparc, 100000 AS idTable, NULL NAZIV, 'Ukupno u iskazu:' NAZIVKULT, sum(Povrsina) AS ukupno_povrsina, ( SELECT sum(nadeljenoVrednost) FROM kom_tablenadela WHERE idIskazZemljista = " & brResenja & " ) AS V, 3 redosled FROM kom_novostanjeparcela WHERE idiskaz = " & brResenja & " ORDER BY idTable, brParcele, redosled, brdelaparc"
        Else
            mysqladap_.SelectCommand.CommandText = "SELECT brParcele, brdelaparc, kom_novostanjeparcela.idTable, NAZIV, concat(ucase(substring(NAZIVKULT,1,1)),lcase(substring(NAZIVKULT,2))) as NAZIVKULT, Povrsina AS ukupno_povrsina, NULL AS V, 1 redosled FROM kom_novostanjeparcela LEFT OUTER JOIN kat_potesi ON idPotes = SIFRA LEFT OUTER JOIN kat_kultura ON kom_novostanjeparcela.SKulture = kat_kultura.idKulture WHERE idIskaz = " & brResenja & " UNION SELECT brParcele, NULL brdelaparc, kom_novostanjeparcela.idTable, NULL NAZIV, 'Укупно:' NAZIVKULT, sum(Povrsina) AS ukupno_povrsina, nadeljenoVrednost AS V, 2 redosled FROM kom_novostanjeparcela LEFT OUTER JOIN kom_tablenadela ON kom_novostanjeparcela.idtable = kom_tablenadela.IdTable AND kom_novostanjeparcela.rednibrNadele = kom_tablenadela.redniBrojNadele WHERE idiskaz = " & brResenja & " GROUP BY brparcele UNION SELECT 100000 AS brParcele, NULL brdelaparc, 100000 AS idTable, NULL NAZIV, 'Укупно у исказу:' NAZIVKULT, sum(Povrsina) AS ukupno_povrsina, ( SELECT sum(nadeljenoVrednost) FROM kom_tablenadela WHERE idIskazZemljista = " & brResenja & " ) AS V, 3 redosled FROM kom_novostanjeparcela WHERE idiskaz = " & brResenja & " ORDER BY idTable, brParcele, redosled, brdelaparc"
        End If

        brPolja = 7

        Dim tblNovoStanje As New DataTable : mysqladap_.Fill(tblNovoStanje)

        'novo stanje!
        wTable = wDoc_.Tables.Add(bokMarks_.Item("Spisak_parcela_novo_stanje").Range, tblNovoStanje.Rows.Count + 2, brPolja)
        wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow)
        With wTable
            .Borders.Enable = True
            .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
            .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
            .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
            .Range.Font.Name = "Times New Roman"
            .Range.Font.Size = 9
        End With

        'sada header

        For j = 1 To brPolja
            For k = 1 To tblNovoStanje.Rows.Count
                If j = 6 Then
                    wTable.Cell(k + 1, j).Range.Text = povrsinaParceleUkras(tblNovoStanje.Rows(k - 1).Item(j - 1).ToString)
                Else
                    wTable.Cell(k + 1, j).Range.Text = tblNovoStanje.Rows(k - 1).Item(j - 1).ToString
                End If

                wTable.Cell(k + 1, j).Range.Font.Bold = False
            Next
        Next

        wTable.Rows(1).Range.Font.Bold = True
        If My.Settings.resenja_pismo = "Latinica" Then
            wTable.Cell(1, 1).Range.Text = "Broj parcele" : wTable.Cell(1, 2).Range.Text = "Deo parcele" : wTable.Cell(1, 3).Range.Text = "Broj table" : wTable.Cell(1, 4).Range.Text = "Potes" : wTable.Cell(1, 5).Range.Text = "Kultura" : wTable.Cell(1, 6).Range.Text = "Nadeljena površina" : wTable.Cell(1, 7).Range.Text = "Nadeljena vrednost"
        Else
            wTable.Cell(1, 1).Range.Text = "Број парцеле" : wTable.Cell(1, 2).Range.Text = "Део парцеле" : wTable.Cell(1, 3).Range.Text = "Број табле" : wTable.Cell(1, 4).Range.Text = "Потес" : wTable.Cell(1, 5).Range.Text = "Култура" : wTable.Cell(1, 6).Range.Text = "Надељено површина" : wTable.Cell(1, 7).Range.Text = "Надељено вредност"
        End If

        wTable.Rows(wTable.Rows.Count).Range.Bold = True

        For k = 1 To tblNovoStanje.Rows.Count
            If Val(wTable.Cell(k + 1, 2).Range.Text) = 0 Then
                'sada bi trebalo da obrises i sa leve i sa desne strane 
                wTable.Cell(k + 1, 3).Range.Text = "" : wTable.Cell(k + 1, 1).Range.Text = ""
                wTable.Rows(k + 1).Range.Bold = True
            End If
        Next

        For j = 1 To brPolja
            wTable.Cell(1, j).Range.Font.Size = 11
        Next

        wTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent)

        If My.Settings.resenja_stampaProcembeniRazredi = True Then

            Dim tbl2stanje As New DataTable
            mysqladap_.SelectCommand.CommandText = "SELECT sum(prazred_1) AS p1, (sum(prazred_1)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 1 ) AS v1, sum(prazred_2) AS p2, (sum(prazred_2)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 2 ) AS v2, sum(prazred_3) AS p3, (sum(prazred_3)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 3 ) AS v3, sum(prazred_4) AS p4, (sum(prazred_4)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 4 ) AS v4, sum(prazred_5) AS p5, (sum(prazred_5)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 5 ) AS v5, sum(prazred_6) AS p6, (sum(prazred_6)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 6 ) AS v6, sum(prazred_7) AS p7, (sum(prazred_7)) * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE kom_koeficijenti.brojKoeficijenta = 7 ) AS v7, sum(prazred_neplodno) AS pNPL, 0 AS vNPL, sum(ukupno_povrsina) AS PSuma, ( SELECT sum(nadeljenoVrednost) FROM kom_tablenadela WHERE idIskazZemljista = " & brResenja & " ) AS vsuma FROM kom_dkp_parcela WHERE idIskaza = " & brResenja
            mysqladap_.Fill(tbl2stanje)

            brPolja = 3 : wtable1 = wDoc_.Tables.Add(bokMarks_.Item("Spisak_procembenih_razreda_novo_stanje").Range, 10, brPolja)

            With wtable1
                .Borders.Enable = True
                .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Font.Name = "Times New Roman"
                .Range.Font.Size = 9
            End With

            For p = 1 To brPolja
                wtable1.Columns.Item(p).Width = docApp_.CentimetersToPoints(3.8)
            Next

            'sada header
            'wtable1.Columns(1).Width = 25 : wtable1.Columns(2).Width = 35 : wtable1.Columns(3).Width = 35
            If My.Settings.resenja_pismo = "Latinica" Then
                wtable1.Cell(1, 1).Range.Text = "PROCEMBENI RAZRED" : wtable1.Cell(1, 2).Range.Text = "POVRŠINA" : wtable1.Cell(1, 3).Range.Text = "VREDNOST" : wtable1.Cell(9, 1).Range.Text = "Bez razreda" : wtable1.Cell(10, 1).Range.Text = "Učesnik dobio iz komasacione mase:"
            Else
                wtable1.Cell(1, 1).Range.Text = "ПРОЦЕМБЕНИ РАЗРЕД" : wtable1.Cell(1, 2).Range.Text = "ПОВРШИНА" : wtable1.Cell(1, 3).Range.Text = "ВРЕДНОСТ" : wtable1.Cell(9, 1).Range.Text = "Без разреда" : wtable1.Cell(10, 1).Range.Text = "Учесник добио из комасационе масе:"
            End If

            wtable1.Cell(2, 1).Range.Text = "1." : wtable1.Cell(2, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(0).ToString) : wtable1.Cell(2, 3).Range.Text = tbl2stanje.Rows(0).Item(1).ToString
            wtable1.Cell(3, 1).Range.Text = "2." : wtable1.Cell(3, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(2).ToString) : wtable1.Cell(3, 3).Range.Text = tbl2stanje.Rows(0).Item(3).ToString
            wtable1.Cell(4, 1).Range.Text = "3." : wtable1.Cell(4, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(4).ToString) : wtable1.Cell(4, 3).Range.Text = tbl2stanje.Rows(0).Item(5).ToString
            wtable1.Cell(5, 1).Range.Text = "4." : wtable1.Cell(5, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(6).ToString) : wtable1.Cell(5, 3).Range.Text = tbl2stanje.Rows(0).Item(7).ToString
            wtable1.Cell(6, 1).Range.Text = "5." : wtable1.Cell(6, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(8).ToString) : wtable1.Cell(6, 3).Range.Text = tbl2stanje.Rows(0).Item(9).ToString
            wtable1.Cell(7, 1).Range.Text = "6." : wtable1.Cell(7, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(10).ToString) : wtable1.Cell(7, 3).Range.Text = tbl2stanje.Rows(0).Item(11).ToString
            wtable1.Cell(8, 1).Range.Text = "7." : wtable1.Cell(8, 2).Range.Text = povrsinaParceleUkras(tbl2stanje.Rows(0).Item(12).ToString) : wtable1.Cell(8, 3).Range.Text = tbl2stanje.Rows(0).Item(13).ToString
            wtable1.Cell(9, 2).Range.Text = tbl2stanje.Rows(0).Item(14).ToString : wtable1.Cell(9, 3).Range.Text = tbl2stanje.Rows(0).Item(15).ToString
            wtable1.Cell(10, 2).Range.Text = tbl2stanje.Rows(0).Item(16).ToString : wtable1.Cell(10, 3).Range.Text = tbl2stanje.Rows(0).Item(17).ToString

            wtable1.Rows(1).Range.Font.Bold = True : wtable1.Rows(10).Range.Font.Bold = True

            For j = 1 To brPolja
                wtable1.Cell(1, j).Range.Font.Size = 11
            Next

        Else
            bokMarks_.Item("Spisak_procembenih_razreda_novo_stanje").Range.Text = ""
        End If

        'sada idu TERETI

        'mysqladap_.SelectCommand.CommandText = "SELECT if(podbroj=0,concat(brojparc),concat(brojparc,'/',podbroj)), BrNoveParcele, cast(OPISTERETA as char), udeonovibrojparc FROM kom_tereti WHERE obrisan = 0 AND idParcele IN ( SELECT DISTINCT idparcele FROM kom_vezaparcelavlasnik WHERE obrisan=0 and idiskazzemljista = " & brResenja & " )"
        'ovaj query treba izmeniti i to je to!!!!
        'mysqladap_.SelectCommand.CommandText = "SELECT IF ( kom_tereti.PODBROJ = 0, concat(kom_tereti.BROJPARC), concat( kom_tereti.BROJPARC, '/', kom_tereti.PODBROJ )) AS brp, BrNoveParcele, cast(OPISTERETA AS CHAR) AS OPISTERETA, ifnull(udeoNoviBrojParc, '1/1') FROM kom_tereti INNER JOIN kom_parcele ON kom_tereti.idParcele = kom_parcele.idParc AND kom_tereti.obrisan = 0 and kom_tereti.BrNoveParcele<>-1 AND ( UKOMASACIJI <> 0 ) AND kom_tereti.idParcele IN ( SELECT idparcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & " )"
        mysqladap_.SelectCommand.CommandText = "SELECT bb.aa_, brnoveparcele, aa.opistereta, udeo_, indef_ FROM ( SELECT IF ( kom_tereti.PODBROJ = 0, concat(kom_tereti.BROJPARC), concat( kom_tereti.BROJPARC, '/', kom_tereti.PODBROJ )) AS brp, BrNoveParcele, cast(OPISTERETA AS CHAR) AS OPISTERETA, ifnull(udeoNoviBrojParc, '1/1') AS udeo_, ( SELECT REPLACE ( REPLACE (indikacije_, 'LJ', 'Q'), 'NJ', 'W' ) AS indikacije_ FROM ( SELECT concat( prezime, ' ', ifnull(imeoca, ''), ' ', ifnull(ime, ''), ', ', matbrgra, ', ', ifnull(mesto, ''), ', ', ifnull(ulica, ''), ' ', ifnull(broj, ''), ifnull(uzbroj, '')) AS indikacije_, matbrgra FROM kom_vlasnik UNION SELECT DISTINCT concat( prezime, ' ', ifnull(imeoca, ''), ' ', ifnull(ime, ''), ', ', MATBRGRA, ', ', ifnull(mesto, ''), ', ', ifnull(adresa, ''), ' ', ifnull(broj, ''), ifnull(uzbroj, '')), matbrgra FROM fs_lica ) GG WHERE GG.MATBRGRA = kom_tereti.MATBRGRA ) AS indef_ FROM kom_tereti INNER JOIN kom_parcele ON kom_tereti.idParcele = kom_parcele.idParc AND kom_tereti.obrisan = 0 AND kom_tereti.BrNoveParcele <>- 1 AND (UKOMASACIJI <> 0) AND kom_tereti.idParcele IN ( SELECT idparcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & " AND obrisan = 0 )) AA LEFT OUTER JOIN ( SELECT group_concat(distinct brparcelef SEPARATOR ', ') aa_, opistereta FROM ( SELECT idParcele, IF ( PODBROJ = 0, concat(BROJPARC), concat(BROJPARC, '/', PODBROJ)) AS brparcelef, OPISTERETA FROM kom_tereti WHERE kom_tereti.obrisan = 0 AND idParcele IN ( SELECT idparcele FROM kom_vezaparcelavlasnik WHERE obrisan = 0 AND kom_vezaparcelavlasnik.idiskazzemljista = " & brResenja & " )) A GROUP BY opistereta ) BB ON AA.opistereta = bb.opistereta ORDER BY brnoveparcele"


        Dim tblTereti As New DataTable : mysqladap_.Fill(tblTereti)

        'novo stanje!

        'sada da vidimo sta se desava sa teretima!
        brPolja = 5
        If tblTereti.Rows.Count <> 0 Then

            wtableTereti = wDoc_.Tables.Add(bokMarks_.Item("Spisak_tereta_novo_stanje").Range, tblTereti.Rows.Count + 2, brPolja)
            wtableTereti.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow)
            With wtableTereti
                .Borders.Enable = True
                .Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Borders(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle
                .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                .Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                '.Range.Font.Name = "Times New Roman"
                '.Range.Font.Name = "Times Roman Cirilica"
                .Range.Font.Name = "Times New Roman"
                .Range.Font.Size = 9
            End With
            If My.Settings.resenja_pismo = "Latinica" Then
                wtableTereti.Cell(1, 1).Range.Text = "Sa parcele" : wtableTereti.Cell(1, 2).Range.Text = "Na parcelu" : wtableTereti.Cell(1, 3).Range.Text = "Opis tereta" : wtableTereti.Cell(1, 4).Range.Text = "Udeo" : wtableTereti.Cell(1, 5).Range.Text = "U korist"
            Else
                wtableTereti.Cell(1, 1).Range.Text = "Са парцеле" : wtableTereti.Cell(1, 2).Range.Text = "На парцелу" : wtableTereti.Cell(1, 3).Range.Text = "Опис терета" : wtableTereti.Cell(1, 4).Range.Text = "Удео" : wtableTereti.Cell(1, 5).Range.Text = "У корист"
            End If

            wtableTereti.Cell(1, 1).Range.Bold = True : wtableTereti.Cell(1, 2).Range.Bold = True : wtableTereti.Cell(1, 3).Range.Bold = True : wtableTereti.Cell(1, 4).Range.Bold = True : wtableTereti.Cell(1, 5).Range.Bold = True
            For j = 1 To brPolja
                For k = 1 To tblTereti.Rows.Count
                    wtableTereti.Cell(k + 1, j).Range.Text = tblTereti.Rows(k - 1).Item(j - 1).ToString
                    wtableTereti.Cell(k + 1, j).Range.Font.Bold = False
                Next
            Next

            For j = 1 To brPolja
                wtableTereti.Cell(1, j).Range.Font.Size = 11
                Select Case j
                    Case 1
                        wtableTereti.Columns(j).Width = docApp_.CentimetersToPoints(1.79)
                    Case 2
                        wtableTereti.Columns(j).Width = docApp_.CentimetersToPoints(1.79)
                    Case 3
                        wtableTereti.Columns(j).Width = docApp_.CentimetersToPoints(8.6)
                    Case 4
                        wtableTereti.Columns(j).Width = docApp_.CentimetersToPoints(1.36)
                    Case 5
                        wtableTereti.Columns(j).Width = docApp_.CentimetersToPoints(2.75)
                End Select
            Next

        Else
            bokMarks_.Item("Spisak_tereta_novo_stanje").Range.Text = ""
        End If

        bokMarks_.Item("pageBreak_03").Range.InsertBreak(prelom_)

        'kraj tereta

        'sada mi treba koliko je vise odnosno manje nadeljen i to je onda to! zavrsio sam sa ovim!

        comm_.CommandText = "SELECT ifnull(vtreba - vnadeljen,0) FROM ( SELECT sum(Vnad) AS Vtreba FROM ( SELECT round(vsuma, 2) AS V, ( round(vsuma, 2) - round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 )) AS dV, round( vsuma * ( SELECT vrednost FROM kom_parametri WHERE opis = 'koeficijent_umanjenja' ), 2 ) AS Vnad FROM ( SELECT DISTINCT idParcele FROM kom_vezaparcelavlasnik WHERE idiskazzemljista = " & brResenja & " AND obrisan = 0 ) AS A LEFT OUTER JOIN ( SELECT ( prazred_1 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 1 ) + prazred_2 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 2 ) + prazred_3 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 3 ) + prazred_4 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 4 ) + prazred_5 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 5 ) + prazred_6 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 6 ) + prazred_7 * ( SELECT VrednostKoeficijenta FROM kom_koeficijenti WHERE brojKoeficijenta = 7 )) AS vsuma, idparc FROM kom_kfmss ) AS C ON A.idparcele = C.idparc ) AS ff ) AS GG1, ( SELECT round(sum(nadeljenoVrednost), 2) AS Vnadeljen FROM kom_tablenadela WHERE idIskazZemljista = " & brResenja & " ) AS GG2"

        Try
            conn_.Open()
        Catch ex As Exception

        End Try

        myreader_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        myreader_.Read()
        Dim odbitak_ = myreader_.GetValue(0)
        myreader_.Close()

        'If Val(odbitak_) > 0 Then
        '    wtable1.Cell(12, 3).Range.Text = odbitak_
        'Else
        '    wtable1.Cell(11, 3).Range.Text = Math.Abs(Val(odbitak_))
        'End If
        Try
            bokMarks_.Item("odbitak_01").Range.Text = Math.Abs(Val(odbitak_))
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("odbitak_02").Range.Text = Math.Abs(Val(odbitak_))
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("odbitak_03").Range.Text = Math.Abs(Val(odbitak_))
        Catch ex As Exception

        End Try

        'bokMarks_.Item("odbitak_02").Range.Text = Math.Abs(Val(odbitak_))
        'више-мање

        Try
            If Val(odbitak_) < 0 Then
                bokMarks_.Item("vise_manje_01").Range.Text = "више"
            Else
                bokMarks_.Item("vise_manje_01").Range.Text = "мање"
            End If
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("pare_01").Range.Text = Math.Abs(Val(odbitak_)) * My.Settings.resenje_vjedinice_din
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("pare_02").Range.Text = Math.Abs(Val(odbitak_)) * My.Settings.resenje_vjedinice_din
        Catch ex As Exception

        End Try

        Try
            bokMarks_.Item("pare_03").Range.Text = Math.Abs(Val(odbitak_)) * My.Settings.resenje_vjedinice_din
        Catch ex As Exception

        End Try

        Try
            If Val(odbitak_) = 0 Then
                bokMarks_.Item("feketic_01").Range.Text = "Нема разлике у вредносним јединицама између унетог и надељеног земљишта."
            End If
        Catch ex As Exception

        End Try

        Try
            If Val(odbitak_) = 0 Then
                bokMarks_.Item("nadoknadaManje").Range.Text = ""
                bokMarks_.Item("nadoknadaVise").Range.Text = ""
            Else
                If Val(odbitak_) < 0 Then
                    bokMarks_.Item("nadoknadaManje").Range.Text = ""
                Else
                    bokMarks_.Item("nadoknadaVise").Range.Text = ""
                    Try
                        bokMarks_.Item("brIskaza_lovcenac").Range.Text = ""
                    Catch ex As Exception

                    End Try
                End If
            End If


        Catch ex As Exception

        End Try

        'sada mozemo da opravimo ovo sa tackom 5!
        'SELECT concat( 'учесник комасације ', indik_, ' се обавезује да Комисији за спровођење комасације у КО Адорјан, ', udeo, 'накнаде из тачке 4. овог Решења уплати на рачун број 840-467845-86 са позивом на број 8836-', MATBRGRA ) FROM ( SELECT IF ( imeoca IS NULL, concat(PREZIME, ' ', IME), CONCAT( PREZIME, ' (', IMEOCA, ') ', IME )) AS indik_, MATBRGRA, A.udeo, A.koefUdeo FROM kom_vlasnik INNER JOIN ( SELECT DISTINCT idvlasnika, udeo, koefUdeo FROM kom_vezaparcelavlasnik WHERE obrisan = 0 AND idiskazzemljista = 23 ) AS A ON kom_vlasnik.idVlasnika = A.idvlasnika ) AS AA
        'comm_.CommandText = "SELECT concat( 'учесник комасације ', indik_, ' се обавезује да Комисији за спровођење комасације у КО Адорјан, ', udeo, 'накнаде из тачке 4. овог Решења уплати на рачун број 840-467845-86 са позивом на број 8836-', MATBRGRA ) FROM ( SELECT IF ( imeoca IS NULL, concat(PREZIME, ' ', IME), CONCAT( PREZIME, ' (', IMEOCA, ') ', IME )) AS indik_, MATBRGRA, A.udeo, A.koefUdeo FROM kom_vlasnik INNER JOIN ( SELECT DISTINCT idvlasnika, udeo, koefUdeo FROM kom_vezaparcelavlasnik WHERE obrisan = 0 AND idiskazzemljista = " & brResenja & " ) AS A ON kom_vlasnik.idVlasnika = A.idvlasnika ) AS AA"
        'Try
        '    conn_.Open()
        'Catch ex As Exception

        'End Try

        'myreader_ = comm_.ExecuteReader(CommandBehavior.CloseConnection)

        'Dim tekstZaPlacanje_ As String = ""

        'Do While myreader_.Read

        '    If tekstZaPlacanje_ = "" Then
        '        tekstZaPlacanje_ = myreader_.GetValue(0)
        '    Else
        '        tekstZaPlacanje_ = tekstZaPlacanje_ & " и " & myreader_.GetValue(0)
        '    End If


        'Loop

        'myreader_.Close()

        'bokMarks_.Item("JMBG_01").Range.Text = tekstZaPlacanje_ & " "
        'sda mozes nesto da uradis u stvri da promenis do kraja to

        'wDoc_.Save()
        'wDoc_.Close()
        'docApp_.Quit()
        'pb1.Value = i
        'wDoc_ = Nothing
        bokMarks_ = Nothing
        conn_ = Nothing : myreader_ = Nothing
        'docApp_ = Nothing

    End Sub

    Public Function povrsinaParceleUkras(povrsina_ As String) As String

        Dim p_ As String
        If povrsina_ = "" Then
            p_ = "0"
        Else
            Select Case povrsina_.Length
                Case 1
                    p_ = povrsina_
                Case 2
                    p_ = povrsina_
                Case 3
                    p_ = Left(povrsina_, 1) & " " & Right(povrsina_, 2)
                Case 4
                    p_ = Left(povrsina_, 2) & " " & Right(povrsina_, 2)
                Case Else
                    p_ = Left(povrsina_, povrsina_.Length - 4) & " " & Mid(povrsina_, povrsina_.Length - 3, 2) & " " & Right(povrsina_, 2)
            End Select
        End If

        Return p_

    End Function

    Public Function povrsinaParceleUkras(povrsina_ As Integer) As String
        Dim p_ As String
        Dim povrsina1 As String = povrsina_

        Select Case povrsina1.Length
            Case 1
                p_ = povrsina1
            Case 2
                p_ = povrsina1
            Case 3
                p_ = Left(povrsina1, 1) & " " & Right(povrsina1, 2)
            Case 4
                p_ = Left(povrsina1, 2) & " " & Right(povrsina1, 2)
            Case Else
                p_ = Left(povrsina1, povrsina1.Length - 4) & " " & Mid(povrsina1, povrsina1.Length - 4, 2) & " " & Right(povrsina1, 2)
        End Select

        Return p_
    End Function

End Module
