﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Public NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("parcele")>  _
        Public Property layerName_parcele() As String
            Get
                Return CType(Me("layerName_parcele"),String)
            End Get
            Set
                Me("layerName_parcele") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("table")>  _
        Public Property layerName_table() As String
            Get
                Return CType(Me("layerName_table"),String)
            End Get
            Set
                Me("layerName_table") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("procembeni razredi")>  _
        Public Property layerName_ProcembeniRazredi() As String
            Get
                Return CType(Me("layerName_ProcembeniRazredi"),String)
            End Get
            Set
                Me("layerName_ProcembeniRazredi") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Tacke za nadelu")>  _
        Public Property layerName_nadelaSmer() As String
            Get
                Return CType(Me("layerName_nadelaSmer"),String)
            End Get
            Set
                Me("layerName_nadelaSmer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Linije ulice")>  _
        Public Property layerName_Ulice() As String
            Get
                Return CType(Me("layerName_Ulice"),String)
            End Get
            Set
                Me("layerName_Ulice") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("CentriMoci")>  _
        Public Property layerName_centar() As String
            Get
                Return CType(Me("layerName_centar"),String)
            End Get
            Set
                Me("layerName_centar") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("brParcele")>  _
        Public Property parcele_fieldName_brParcele() As String
            Get
                Return CType(Me("parcele_fieldName_brParcele"),String)
            End Get
            Set
                Me("parcele_fieldName_brParcele") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("rastojanjeCM")>  _
        Public Property parcele_fieldName_rastojanjeCentarMoci() As String
            Get
                Return CType(Me("parcele_fieldName_rastojanjeCentarMoci"),String)
            End Get
            Set
                Me("parcele_fieldName_rastojanjeCentarMoci") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("katPovrsina")>  _
        Public Property parcele_fieldName_katPovrsina() As String
            Get
                Return CType(Me("parcele_fieldName_katPovrsina"),String)
            End Get
            Set
                Me("parcele_fieldName_katPovrsina") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("idVlasnika")>  _
        Public Property parcele_fieldName_Vlasnik() As String
            Get
                Return CType(Me("parcele_fieldName_Vlasnik"),String)
            End Get
            Set
                Me("parcele_fieldName_Vlasnik") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Ekonomska_dvorista")>  _
        Public Property layerName_ekonomskaDvorista() As String
            Get
                Return CType(Me("layerName_ekonomskaDvorista"),String)
            End Get
            Set
                Me("layerName_ekonomskaDvorista") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0.25*P+0.25*R")>  _
        Public Property funkcijaRaspodele() As String
            Get
                Return CType(Me("funkcijaRaspodele"),String)
            End Get
            Set
                Me("funkcijaRaspodele") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("-10000000")>  _
        Public Property maxP() As String
            Get
                Return CType(Me("maxP"),String)
            End Get
            Set
                Me("maxP") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("-10000000")>  _
        Public Property maxR() As String
            Get
                Return CType(Me("maxR"),String)
            End Get
            Set
                Me("maxR") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("-100000000")>  _
        Public Property maxZ() As String
            Get
                Return CType(Me("maxZ"),String)
            End Get
            Set
                Me("maxZ") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0.95")>  _
        Public Property koeficijentUmanjenja() As String
            Get
                Return CType(Me("koeficijentUmanjenja"),String)
            End Get
            Set
                Me("koeficijentUmanjenja") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("User ID=root;Password=azra220;Host=localhost;Port=3306;Database=adorjan;")>  _
        Public Property mysqlConnString() As String
            Get
                Return CType(Me("mysqlConnString"),String)
            End Get
            Set
                Me("mysqlConnString") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("75")>  _
        Public Property nadela_brInteracija() As String
            Get
                Return CType(Me("nadela_brInteracija"),String)
            End Get
            Set
                Me("nadela_brInteracija") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1.5")>  _
        Public Property nadela_duzina() As String
            Get
                Return CType(Me("nadela_duzina"),String)
            End Get
            Set
                Me("nadela_duzina") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("DKP_Nadela")>  _
        Public Property layerName_ParceleNadela() As String
            Get
                Return CType(Me("layerName_ParceleNadela"),String)
            End Get
            Set
                Me("layerName_ParceleNadela") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("PntTableObelezavanje")>  _
        Public Property layerName_pointTableObelezavanje() As String
            Get
                Return CType(Me("layerName_pointTableObelezavanje"),String)
            End Get
            Set
                Me("layerName_pointTableObelezavanje") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Podela2500")>  _
        Public Property layerName_podelaNaListove() As String
            Get
                Return CType(Me("layerName_podelaNaListove"),String)
            End Get
            Set
                Me("layerName_podelaNaListove") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Poligonske")>  _
        Public Property layerName_poligonskeTacke() As String
            Get
                Return CType(Me("layerName_poligonskeTacke"),String)
            End Get
            Set
                Me("layerName_poligonskeTacke") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("4")>  _
        Public Property poligonske_brojOdmeranjaOpis() As String
            Get
                Return CType(Me("poligonske_brojOdmeranjaOpis"),String)
            End Get
            Set
                Me("poligonske_brojOdmeranjaOpis") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("50")>  _
        Public Property poligonske_sirinaBaferZone() As String
            Get
                Return CType(Me("poligonske_sirinaBaferZone"),String)
            End Get
            Set
                Me("poligonske_sirinaBaferZone") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("300")>  _
        Public Property tahimetrija_sirinaBaferZone() As String
            Get
                Return CType(Me("tahimetrija_sirinaBaferZone"),String)
            End Get
            Set
                Me("tahimetrija_sirinaBaferZone") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property tahimetrija_razmakIzmeduRedova() As String
            Get
                Return CType(Me("tahimetrija_razmakIzmeduRedova"),String)
            End Get
            Set
                Me("tahimetrija_razmakIzmeduRedova") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("""""")>  _
        Public Property openFiles() As String
            Get
                Return CType(Me("openFiles"),String)
            End Get
            Set
                Me("openFiles") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1")>  _
        Public Property komponentShowTable() As String
            Get
                Return CType(Me("komponentShowTable"),String)
            End Get
            Set
                Me("komponentShowTable") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("23/12/2013")>  _
        Public Property GPSMerenje_datumPocetka() As String
            Get
                Return CType(Me("GPSMerenje_datumPocetka"),String)
            End Get
            Set
                Me("GPSMerenje_datumPocetka") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("8:00")>  _
        Public Property GPSMerenje_vremePocetka() As String
            Get
                Return CType(Me("GPSMerenje_vremePocetka"),String)
            End Get
            Set
                Me("GPSMerenje_vremePocetka") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0.85")>  _
        Public Property GPSMerenje_brzinaHoda() As String
            Get
                Return CType(Me("GPSMerenje_brzinaHoda"),String)
            End Get
            Set
                Me("GPSMerenje_brzinaHoda") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("20")>  _
        Public Property GPSMerenje_zadrzavanjeNaTacki() As String
            Get
                Return CType(Me("GPSMerenje_zadrzavanjeNaTacki"),String)
            End Get
            Set
                Me("GPSMerenje_zadrzavanjeNaTacki") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("8")>  _
        Public Property GPSMerenje_duzinaRada() As String
            Get
                Return CType(Me("GPSMerenje_duzinaRada"),String)
            End Get
            Set
                Me("GPSMerenje_duzinaRada") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property GPSMerenje_preskacemPraznik() As String
            Get
                Return CType(Me("GPSMerenje_preskacemPraznik"),String)
            End Get
            Set
                Me("GPSMerenje_preskacemPraznik") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property GPSMerenje_preskacemVikend() As String
            Get
                Return CType(Me("GPSMerenje_preskacemVikend"),String)
            End Get
            Set
                Me("GPSMerenje_preskacemVikend") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("60")>  _
        Public Property GPSMerenje_vremePreseljenjeBaze() As String
            Get
                Return CType(Me("GPSMerenje_vremePreseljenjeBaze"),String)
            End Get
            Set
                Me("GPSMerenje_vremePreseljenjeBaze") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property zaokruzivanjeBrojDecMesta() As String
            Get
                Return CType(Me("zaokruzivanjeBrojDecMesta"),String)
            End Get
            Set
                Me("zaokruzivanjeBrojDecMesta") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("drawing")>  _
        Public Property drawing_streetHouseNumbers() As String
            Get
                Return CType(Me("drawing_streetHouseNumbers"),String)
            End Get
            Set
                Me("drawing_streetHouseNumbers") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("naselje")>  _
        Public Property id_field_municipality() As String
            Get
                Return CType(Me("id_field_municipality"),String)
            End Get
            Set
                Me("id_field_municipality") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("broj")>  _
        Public Property id_field_streetNumber() As String
            Get
                Return CType(Me("id_field_streetNumber"),String)
            End Get
            Set
                Me("id_field_streetNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("podbroj")>  _
        Public Property id_field_streetLetter() As String
            Get
                Return CType(Me("id_field_streetLetter"),String)
            End Get
            Set
                Me("id_field_streetLetter") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("ulica")>  _
        Public Property id_field_streetName() As String
            Get
                Return CType(Me("id_field_streetName"),String)
            End Get
            Set
                Me("id_field_streetName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("iduliceskr")>  _
        Public Property id_field_streetCode() As String
            Get
                Return CType(Me("id_field_streetCode"),String)
            End Get
            Set
                Me("id_field_streetCode") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1")>  _
        Public Property resenja_stampaProcembeniRazredi() As String
            Get
                Return CType(Me("resenja_stampaProcembeniRazredi"),String)
            End Get
            Set
                Me("resenja_stampaProcembeniRazredi") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("20")>  _
        Public Property pozivanje_nultoVreme() As String
            Get
                Return CType(Me("pozivanje_nultoVreme"),String)
            End Get
            Set
                Me("pozivanje_nultoVreme") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property pozivanje_vremePosedovni() As String
            Get
                Return CType(Me("pozivanje_vremePosedovni"),String)
            End Get
            Set
                Me("pozivanje_vremePosedovni") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0.5")>  _
        Public Property pozivanje_vremeBrojParcela() As String
            Get
                Return CType(Me("pozivanje_vremeBrojParcela"),String)
            End Get
            Set
                Me("pozivanje_vremeBrojParcela") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property pozivanje_pocetakDatum() As String
            Get
                Return CType(Me("pozivanje_pocetakDatum"),String)
            End Get
            Set
                Me("pozivanje_pocetakDatum") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("8:00")>  _
        Public Property pozivanje_smena1Pocetak() As String
            Get
                Return CType(Me("pozivanje_smena1Pocetak"),String)
            End Get
            Set
                Me("pozivanje_smena1Pocetak") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("13")>  _
        Public Property pozivanje_smena1Kraj() As String
            Get
                Return CType(Me("pozivanje_smena1Kraj"),String)
            End Get
            Set
                Me("pozivanje_smena1Kraj") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("17:15")>  _
        Public Property pozivanje_smena2Pocetak() As String
            Get
                Return CType(Me("pozivanje_smena2Pocetak"),String)
            End Get
            Set
                Me("pozivanje_smena2Pocetak") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("20")>  _
        Public Property pozivanje_smena2Kraj() As String
            Get
                Return CType(Me("pozivanje_smena2Kraj"),String)
            End Get
            Set
                Me("pozivanje_smena2Kraj") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property pozivanje_kriterijum_zeljeUcesnika() As String
            Get
                Return CType(Me("pozivanje_kriterijum_zeljeUcesnika"),String)
            End Get
            Set
                Me("pozivanje_kriterijum_zeljeUcesnika") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property pozivanje_kriterijum_izbaciGradevinski() As String
            Get
                Return CType(Me("pozivanje_kriterijum_izbaciGradevinski"),String)
            End Get
            Set
                Me("pozivanje_kriterijum_izbaciGradevinski") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("RADUJEVAC")>  _
        Public Property pozivanje_MaticnoNaselje() As String
            Get
                Return CType(Me("pozivanje_MaticnoNaselje"),String)
            End Get
            Set
                Me("pozivanje_MaticnoNaselje") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property pozivanje_wordFileTemplatePath() As String
            Get
                Return CType(Me("pozivanje_wordFileTemplatePath"),String)
            End Get
            Set
                Me("pozivanje_wordFileTemplatePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property pozivanje_stampamOdmah() As String
            Get
                Return CType(Me("pozivanje_stampamOdmah"),String)
            End Get
            Set
                Me("pozivanje_stampamOdmah") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1")>  _
        Public Property pozivanje_stampamSpisakParcelaUPozivu() As String
            Get
                Return CType(Me("pozivanje_stampamSpisakParcelaUPozivu"),String)
            End Get
            Set
                Me("pozivanje_stampamSpisakParcelaUPozivu") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property pozivanje_vremeVlasnik() As String
            Get
                Return CType(Me("pozivanje_vremeVlasnik"),String)
            End Get
            Set
                Me("pozivanje_vremeVlasnik") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property pozivanje_pisemSamoImenaBezVremena() As String
            Get
                Return CType(Me("pozivanje_pisemSamoImenaBezVremena"),String)
            End Get
            Set
                Me("pozivanje_pisemSamoImenaBezVremena") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("801500")>  _
        Public Property pozivanje_idko() As String
            Get
                Return CType(Me("pozivanje_idko"),String)
            End Get
            Set
                Me("pozivanje_idko") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("55")>  _
        Public Property ogranicenje_poStranci() As String
            Get
                Return CType(Me("ogranicenje_poStranci"),String)
            End Get
            Set
                Me("ogranicenje_poStranci") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property resenja_wordFileTemplatePath() As String
            Get
                Return CType(Me("resenja_wordFileTemplatePath"),String)
            End Get
            Set
                Me("resenja_wordFileTemplatePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property resenja_pismo() As String
            Get
                Return CType(Me("resenja_pismo"),String)
            End Get
            Set
                Me("resenja_pismo") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property resenje_koeficijent0() As String
            Get
                Return CType(Me("resenje_koeficijent0"),String)
            End Get
            Set
                Me("resenje_koeficijent0") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("100")>  _
        Public Property resenje_vjedinice_din() As String
            Get
                Return CType(Me("resenje_vjedinice_din"),String)
            End Get
            Set
                Me("resenje_vjedinice_din") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property pozivanje_kriterijumIzbaciIndustrijsku() As String
            Get
                Return CType(Me("pozivanje_kriterijumIzbaciIndustrijsku"),String)
            End Get
            Set
                Me("pozivanje_kriterijumIzbaciIndustrijsku") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.JLCG.My.MySettings
            Get
                Return Global.JLCG.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
