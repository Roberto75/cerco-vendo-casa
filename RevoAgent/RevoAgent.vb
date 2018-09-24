Option Strict On
Imports System.Diagnostics
Imports MyManagerCSharp


Public Class RevoAgent
    Implements IDisposable

    'Private _urlAnnunci As String = "http://revo1.revoagent.com/portali/cvc.xml"
    Private _urlAnnunci As String = "http://partner.miogest.com/040_cvc.xml"
    Public _documentXML As System.Xml.XmlDocument
    Private _temp As String

    Private _manager As ImmobiliareManager
    Private _managerRegioneProvinciaComune As MyManager.RegioniProvinceComuniManager
    Private _managerLog As MyManager.LogManager
    'Private _managerPhoto As MyManager.PhotoManager

    ' Private _externalId As String
    Private _dataTable As Data.DataTable


    Private _message As String

    '*** CONTATORI ***
    Private _contaRecordLetti As Long = 0
    Private _contaRecordInseriti As Long = 0
    Private _contaRecordDaAggiornare As Long = 0
    Private _contaRecordAggiornatiPrezzo As Long = 0
    Private _contaRecordAggiornatiNota As Long = 0
    Private _contaRecordAgenzieDisabled As Long = 0
    Private _contaRecordAgenzieMancanti As Long = 0
    Private _contaRecordCancellati As Long = 0
    Public _contaRecordConErrori As Long = 0


    Public _pathCategorieXML As String
    Private _pathLogFile As String
    Public _pathStatusFile As String



    Private _documentXMLCategorie As System.Xml.XmlDocument


    Public Sub New(pathLogFile As String)
        _pathLogFile = pathLogFile

        'Cancello il log precedente!
        If (System.IO.File.Exists(_pathLogFile)) Then
            System.IO.File.Delete(_pathLogFile)
        End If

        MyLog._init(_pathLogFile)

    End Sub


    Protected Overloads Sub Dispose() Implements IDisposable.Dispose

        MyLog.Flush()
        MyLog.Close()

    End Sub


    Public Function _processXML(myPathXmlFile As String, modeTest As Boolean, forceUpdate As Boolean) As String


        '24/04/2013 OutOfMemory
        '_documentXML = New System.Xml.XmlDocument()
        'Try
        '    _documentXML.LoadXml(IO.File.ReadAllText(myPathXmlFile))
        'Catch ex As Exception
        '    Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
        'End Try


        'Dim reader As New System.IO.StreamReader(myPathXmlFile, System.Text.Encoding.UTF8)
        '_documentXML = New System.Xml.XmlDocument()
        'Try
        '    _documentXML.Load(reader)
        'Catch ex As Exception


        '    Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
        'Finally
        '    If (Not reader Is Nothing) Then
        '        reader.Close()
        '        reader.Dispose()
        '    End If

        '    MyLog.Flush()
        '    MyLog.Close()
        'End Try




        'Dim reader As New System.IO.StreamReader(myResponse.GetResponseStream, System.Text.Encoding.UTF8)
        'Dim html As String
        'html = reader.ReadToEnd
        'myResponse.Close()
        '_documentXML = New System.Xml.XmlDocument()
        'Try
        '    _documentXML.LoadXml(html)
        'Catch ex As Exception
        '    Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
        'End Try

        If (_documentXML Is Nothing) Then



            MyLog.Info("Inizio lettura del file XML")
            '24/04/2013 OutOfMemory
            Dim reader As New System.IO.StreamReader(myPathXmlFile, System.Text.Encoding.UTF8)
            _documentXML = New System.Xml.XmlDocument()
            Try
                _documentXML.Load(reader)
            Catch ex As Exception
                MyLog.Exception("Load del file xml: ", ex)
                Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
            Finally
                If (Not reader Is Nothing) Then
                    reader.Close()
                    reader.Dispose()
                End If
                MyLog.Flush()
                MyLog.Close()
            End Try



            Debug.WriteLine("Lettura del file XML conclusa con successo")
            MyLog.Info("Lettura del file XML conclusa con successo")
            MyLog.Flush()
        End If





        _documentXMLCategorie = New System.Xml.XmlDocument()
        Try
            _documentXMLCategorie.LoadXml(IO.File.ReadAllText(_pathCategorieXML, System.Text.Encoding.UTF8))
        Catch ex As Exception
            Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML delle CATEGORIE.")
        End Try

        _message = ""


        If _manager Is Nothing Then
            _manager = New ImmobiliareManager()
            _manager.openConnection()
        End If

        Dim dateStart As DateTime = Now

        Dim nodeList As System.Xml.XmlNodeList
        Dim dt As Data.DataTable

        'IO.File.WriteAllText(_pathStatusFile, "Check Agenzie ... ")
        '_message &= "Check Agenzie ... " & vbCrLf

        ''verifico la presenza di tutte le agenzie da caricare
        'nodeList = _documentXML.SelectNodes("//Annunci/Annuncio/AgenziaId[not(.=preceding::AgenziaId)]")

        'For Each node As System.Xml.XmlNode In nodeList
        '    _temp = node.FirstChild.Value
        '    Console.WriteLine(_temp)

        '    dt = _manager.getUtenteByExternalId(_temp, MyManager.ImmobiliareManager.SourceId.RevoAgent)
        '    If dt.Rows.Count = 0 Then
        '        _message &= String.Format("L'agenzia {0} non è associata a nessun utente del sito.", _temp) & vbCrLf
        '        _contaRecordConErrori += 1
        '    End If

        '    If dt.Rows.Count > 1 Then
        '        _message &= String.Format("L'agenzia {0} è presente + di una volta tra gli utenti del sito.", _temp) & vbCrLf
        '        _contaRecordConErrori += 1
        '    End If
        'Next

        'cambio lo stato di tutti gli annunci
        If modeTest = False Then
            '  _manager.updateStatoAnnunciByUserId(Long.Parse(dt.Rows(0)("user_id").ToString), MyManager.MercatinoManager.StatoAnnuncio.DaCancellare, MyManager.ImmobiliareManager.SourceId.RevoAgent)
            _manager.updateStatoAnnunciBySourceId(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.DaCancellare)
        End If


        'If _contaRecordConErrori > 0 Then
        '    '  _manager.closeConnection()
        '    _message = "Check agenzie FAILED" & vbCrLf

        '    _managerLog = New MyManager.LogManager()
        '    Try
        '        _managerLog.openConnection()
        '        Dim t As String
        '        If modeTest Then
        '            t = "RevoAgent-TEST"
        '        Else
        '            t = "RevoAgent"
        '        End If
        '        _managerLog.insert(t, _message, "", "Agenzie mancanti", MyManager.LogManager.MyLevel.MyError)

        '    Finally
        '        _manager.closeConnection()
        '    End Try
        '    '*** ESCO DALLA PROCEDURA ***
        '    ' Return _message
        'End If


        If _managerRegioneProvinciaComune Is Nothing Then
            _managerRegioneProvinciaComune = New MyManager.RegioniProvinceComuniManager("RegioniProvinceComuni")
            _managerRegioneProvinciaComune.openConnection()
        End If

        If _managerLog Is Nothing Then
            _managerLog = New MyManager.LogManager()
            _managerLog.openConnection()
        End If


        Dim annuncioId As Long
        Dim userId As Long
        Dim nota As String
        Dim agenziaIsDisabled As Boolean
        Dim dataModifica As DateTime
        Dim elencoAgenzieMancanti As New Hashtable
        Dim contatore As Integer


        _message &= vbCrLf

        If modeTest Then
            _message &= "Start TEST: " & dateStart & vbCrLf
        Else
            _message &= "Start: " & dateStart & vbCrLf
        End If

        nodeList = _documentXML.SelectNodes("//Annunci/Annuncio")
        Try

            For Each node As System.Xml.XmlNode In nodeList
                _contaRecordLetti += 1
                nota = ""
                '26/04/2013 disabiitato perchè sul server mi dato un errore che il file era già in uso da un altro processo
                'IO.File.WriteAllText(_pathStatusFile, String.Format("{0}/{1}", _contaRecordLetti, nodeList.Count))

                Debug.WriteLine(String.Format("{0:N0}/{1:N0}", _contaRecordLetti, nodeList.Count))
                MyLog.Info(String.Format("{0:N0}/{1:N0}", _contaRecordLetti, nodeList.Count))

                Try
                    'Ottengo l'utente dall'agenzia!
                    _temp = node.Item("AgenziaId").FirstChild.Value
                    dt = _manager.getUtenteByExternalId(_temp, Annunci.Models.Immobile.TipoSourceId.RevoAgent)

                    If dt.Rows.Count = 0 Then
                        'rientra tra le agenzie mancanti
                        If elencoAgenzieMancanti.ContainsKey(_temp) Then
                            contatore = CInt(elencoAgenzieMancanti(_temp))
                            elencoAgenzieMancanti.Remove(_temp)
                            elencoAgenzieMancanti.Add(_temp, contatore + 1)
                        Else
                            elencoAgenzieMancanti.Add(_temp, 1)
                        End If
                    Else
                        userId = Long.Parse(dt.Rows(0)("user_id").ToString)

                        If Integer.Parse(dt.Rows(0)("stato_id").ToString) = Annunci.Models.Immobile.StatoUtente.Disabled Then
                            agenziaIsDisabled = True
                        Else
                            agenziaIsDisabled = False
                        End If

                        If agenziaIsDisabled Then
                            _contaRecordAgenzieDisabled += 1
                        Else
                            _temp = node.Item("AnnuncioId").FirstChild.Value
                            With _manager
                                ._sourceId = Annunci.Models.Immobile.TipoSourceId.RevoAgent
                                ._externalId = node.Item("AgenziaId").FirstChild.Value & "-" & node.Item("AnnuncioId").FirstChild.Value

                                If ._externalId = "1593-363" _
                                        OrElse ._externalId = "1593-366" _
                                        OrElse ._externalId = "1593-367" Then
                                    Dim debugg As String
                                    debugg = "OK"
                                End If

                                If node.Item("Tipologia2") Is Nothing OrElse node.Item("Tipologia2").FirstChild Is Nothing Then
                                    ._categoria = decodeTipoAnnuncio(node.Item("Tipologia").FirstChild.Value, "")
                                Else
                                    ._categoria = decodeTipoAnnuncio(node.Item("Tipologia").FirstChild.Value, node.Item("Tipologia2").FirstChild.Value)
                                End If

                                If node.Item("CategoriaId") Is Nothing OrElse node.Item("CategoriaId").FirstChild Is Nothing Then
                                    Throw New MyManager.ManagerException("Categoria non valorizzata")
                                Else
                                    ._tipoImmobile = decodeTipoImmobile(node.Item("Tipologia").FirstChild.Value, Integer.Parse(node.Item("CategoriaId").FirstChild.Value))
                                End If


                                If (node.Item("Cod_Istat") Is Nothing OrElse node.Item("Cod_Istat").FirstChild Is Nothing) And (node.Item("Nazione") Is Nothing OrElse node.Item("Nazione").FirstChild Is Nothing) Then
                                    Throw New MyManager.ManagerException("Nazione e Cod_Istat non valorizzati.")
                                End If


                                'REGIONE - PROVINCIA - COMUNE 
                                If node.Item("Nazione").FirstChild.Value = "Italia" Then

                                    If node.Item("Regione").FirstChild Is Nothing Then
                                        Throw New MyManager.ManagerException("Regione non valorizzata.")
                                    End If

                                    ._regione = node.Item("Regione").FirstChild.Value
                                    If ._regione = "Trentino - Alto Adige" Then
                                        ._regione = "Trentino-Alto Adige"
                                    ElseIf ._regione = "Friuli- Venezia Giulia" Then
                                        ._regione = "Friuli Venezia Giulia"
                                    End If

                                    dt = _managerRegioneProvinciaComune.getRegioneByLabel(._regione)
                                    If dt.Rows.Count <> 1 Then
                                        Throw New MyManager.ManagerException("Regione non valida: " & ._regione)
                                    End If
                                    ._regioneId = Long.Parse(dt.Rows(0)("id").ToString)

                                    dt = _managerRegioneProvinciaComune.getProvinciaBySigla(node.Item("Prov").FirstChild.Value)
                                    If dt.Rows.Count <> 1 Then
                                        Throw New MyManager.ManagerException("Provincia non valida: " & node.Item("Prov").FirstChild.Value)
                                    End If
                                    ._provincia = dt.Rows(0)("valore").ToString
                                    '._provinciaId = Long.Parse(dt.Rows(0)("id").ToString)
                                    ._provinciaId = dt.Rows(0)("id").ToString

                                    dt = _managerRegioneProvinciaComune.getComuneByCodiceISTAT(node.Item("Cod_Istat").FirstChild.Value)
                                    If dt.Rows.Count <> 1 Then
                                        Throw New MyManager.ManagerException("Codice ISTAT non valido: " & node.Item("Cod_Istat").FirstChild.Value)
                                    End If

                                    ._comune = dt.Rows(0)("valore").ToString
                                    '._comuneId = Long.Parse(dt.Rows(0)("id").ToString)
                                    ._comuneId = dt.Rows(0)("id").ToString

                                    If node.Item("Citta").FirstChild.Value <> ._comune Then
                                        _message &= "Warning: " & node.Item("Citta").FirstChild.Value & " <> " & ._comune & " (codice istat: " & ._comuneId & ") " & vbCrLf
                                    End If

                                Else
                                    ._regione = "Stato Estero: " & node.Item("Nazione").FirstChild.Value
                                    ._regioneId = 21

                                    dt = _managerRegioneProvinciaComune.getProvinciaBySigla("EE")
                                    ._provincia = dt.Rows(0)("valore").ToString
                                    '._provinciaId = Long.Parse(dt.Rows(0)("id").ToString)
                                    ._provinciaId = "EE"

                                    dt = _managerRegioneProvinciaComune.getNazioneByDescrizione(node.Item("Nazione").FirstChild.Value)
                                    If dt.Rows.Count <> 1 Then
                                        Throw New MyManager.ManagerException("NAZIONE non valida: " & node.Item("Nazione").FirstChild.Value)
                                    End If

                                    ._comune = dt.Rows(0)("valore").ToString
                                    ' ._comuneId = Long.Parse(dt.Rows(0)("id").ToString)
                                    ._comuneId = dt.Rows(0)("id").ToString
                                End If

                                If Not node.Item("Zona").FirstChild Is Nothing Then
                                    ._quartiere = node.Item("Zona").FirstChild.Value
                                Else
                                    ._quartiere = ""
                                End If

                                If Not node.Item("Indirizzo").FirstChild Is Nothing Then
                                    ._indirizzo = node.Item("Indirizzo").FirstChild.Value
                                Else
                                    ._indirizzo = ""
                                End If

                                If Not node.Item("Civico").FirstChild Is Nothing AndAlso Not String.IsNullOrEmpty(node.Item("Civico").FirstChild.Value) Then
                                    ._indirizzo &= ", " & node.Item("Civico").FirstChild.Value
                                End If

                                If Not node.Item("Latitudine").FirstChild Is Nothing Then
                                    ._latitude = Double.Parse(node.Item("Latitudine").FirstChild.Value.Replace(".", ","))
                                Else
                                    ._latitude = 0
                                End If

                                If Not node.Item("Longitudine").FirstChild Is Nothing Then
                                    ._longitude = Double.Parse(node.Item("Longitudine").FirstChild.Value.Replace(".", ","))
                                Else
                                    ._longitude = 0
                                End If

                                If Not node.Item("Prezzo").FirstChild Is Nothing Then
                                    Try
                                        ._prezzo = Decimal.Parse(node.Item("Prezzo").FirstChild.Value.Replace(".", ","))
                                    Catch ex As Exception
                                        Throw New MyManager.ManagerException(String.Format("prezzo non valido: {0} ", node.Item("Prezzo").FirstChild.Value))
                                    End Try
                                Else
                                    ._prezzo = 0
                                End If

                                If Not node.Item("Anno").FirstChild Is Nothing Then
                                    ._anno = Integer.Parse(node.Item("Anno").FirstChild.Value)
                                Else
                                    ._anno = 0
                                End If

                                If Not node.Item("Mq").FirstChild Is Nothing Then
                                    ._mq = Convert.ToInt32(Decimal.Parse(node.Item("Mq").FirstChild.Value))
                                Else
                                    ._mq = 0
                                End If

                                If Not node.Item("MqGiardino").FirstChild Is Nothing Then
                                    ._giardinoMq = Convert.ToInt32(Decimal.Parse(node.Item("MqGiardino").FirstChild.Value))
                                Else
                                    ._giardinoMq = 0
                                End If

                                If Not node.Item("Camere").FirstChild Is Nothing Then
                                    ._camereDaLetto = Integer.Parse(node.Item("Camere").FirstChild.Value)
                                Else
                                    ._camereDaLetto = 0
                                End If


                                If Not node.Item("Bagni").FirstChild Is Nothing Then
                                    ._bagni = Integer.Parse(node.Item("Bagni").FirstChild.Value)
                                Else
                                    ._bagni = 0
                                End If

                                If Not node.Item("Box").FirstChild Is Nothing Then
                                    ._box = decodeBoxAuto(node.Item("Box").FirstChild.Value)
                                Else
                                    ._box = 0
                                End If

                                If Not node.Item("PostiAuto").FirstChild Is Nothing Then
                                    ._postoAuto = decodePostoAuto(node.Item("PostiAuto").FirstChild.Value)
                                Else
                                    ._postoAuto = 0
                                End If

                                If Not node.Item("Scheda_StatoImmobile") Is Nothing AndAlso Not node.Item("Scheda_StatoImmobile").FirstChild Is Nothing Then
                                    ._statoImmobile = decodeStatoImmobile(node.Item("Scheda_StatoImmobile").FirstChild.Value)
                                Else
                                    ._statoImmobile = Annunci.Models.Immobile.TipoStatoImmobile.Undefined
                                End If

                                If Not node.Item("Piano").FirstChild Is Nothing Then
                                    '._piano = Integer.Parse(node.Item("Piano").FirstChild.Value)

                                    'in alcuni casi abbiamo il piano = 1,5 lo trasformo in 1
                                    Dim d As Decimal
                                    d = Decimal.Parse(node.Item("Piano").FirstChild.Value)

                                    'If (d - Convert.ToInt32(d)) <> 0 Then
                                    '._piano = Convert.ToInt32(Decimal.Parse(node.Item("Piano").FirstChild.Value) - 1)
                                    'Else
                                    '   ._piano = Convert.ToInt32(Decimal.Parse(node.Item("Piano").FirstChild.Value))
                                    'End If
                                    ._piano = Convert.ToInt32(System.Math.Truncate(d))

                                Else
                                    ._piano = 0
                                End If

                                If Not node.Item("Scheda_TotalePiani") Is Nothing AndAlso Not node.Item("Scheda_TotalePiani").FirstChild Is Nothing Then
                                    ._pianiTotali = decodePianiTotali(node.Item("Scheda_TotalePiani").FirstChild.Value)
                                Else
                                    ._pianiTotali = Integer.MinValue
                                End If

                                If Not node.Item("Scheda_TipoCucina") Is Nothing AndAlso Not node.Item("Scheda_TipoCucina").FirstChild Is Nothing Then
                                    ._cucina = decodeTipoCucina(node.Item("Scheda_TipoCucina").FirstChild.Value)
                                Else
                                    ._cucina = Annunci.Models.Immobile.TipoCucina.Undefined
                                End If

                                If Not node.Item("Scheda_Balconi") Is Nothing AndAlso Not node.Item("Scheda_Balconi").FirstChild Is Nothing Then
                                    ._balconi = decodeBalconi(node.Item("Scheda_Balconi").FirstChild.Value)
                                Else
                                    ._balconi = -1
                                End If

                                If Not node.Item("Scheda_Terrazzi") Is Nothing AndAlso Not node.Item("Scheda_Terrazzi").FirstChild Is Nothing Then
                                    ._terrazzi = decodeTerrazzi(node.Item("Scheda_Terrazzi").FirstChild.Value)
                                Else
                                    ._terrazzi = -1
                                End If

                                If Not node.Item("Scheda_CantinaSolaio") Is Nothing AndAlso Not node.Item("Scheda_CantinaSolaio").FirstChild Is Nothing Then
                                    ._cantina = decodeCantina(node.Item("Scheda_CantinaSolaio").FirstChild.Value)
                                Else
                                    ._cantina = -1
                                End If

                                If Not node.Item("Scheda_Riscaldamento") Is Nothing AndAlso Not node.Item("Scheda_Riscaldamento").FirstChild Is Nothing Then
                                    ._riscaldamento = decodeTipoRiscaldamento(node.Item("Scheda_Riscaldamento").FirstChild.Value)
                                Else
                                    ._riscaldamento = Annunci.Models.Immobile.TipoRiscaldamento.Undefined
                                End If

                                nota = "<b>" & getCategoriaFromXML(node.Item("Tipologia").FirstChild.Value, Integer.Parse(node.Item("CategoriaId").FirstChild.Value)) & "</b> <br />"
                                nota &= "<ul>"
                                If Not node.Item("Spese") Is Nothing AndAlso Not node.Item("Spese").FirstChild Is Nothing AndAlso Decimal.Parse(node.Item("Spese").FirstChild.Value) > 0 Then
                                    nota &= "<li>Spese: " & node.Item("Spese").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Taverna") Is Nothing AndAlso Not node.Item("Scheda_Taverna").FirstChild Is Nothing AndAlso node.Item("Scheda_Taverna").FirstChild.Value <> "No" Then
                                    nota &= "<li>Taverna: " & node.Item("Scheda_Taverna").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Giardino") Is Nothing AndAlso Not node.Item("Scheda_Giardino").FirstChild Is Nothing AndAlso node.Item("Scheda_Giardino").FirstChild.Value <> "No" Then
                                    nota &= "<li>Giardino: " & node.Item("Scheda_Giardino").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Cauzione") Is Nothing AndAlso Not node.Item("Scheda_Cauzione").FirstChild Is Nothing Then
                                    nota &= "<li>Cauzione: " & node.Item("Scheda_Cauzione").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_CanoniAnticipati") Is Nothing AndAlso Not node.Item("Scheda_CanoniAnticipati").FirstChild Is Nothing Then
                                    nota &= "<li>Canoni Anticipati: " & node.Item("Scheda_CanoniAnticipati").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_TipoContratto") Is Nothing AndAlso Not node.Item("Scheda_TipoContratto").FirstChild Is Nothing Then
                                    nota &= "<li>Tipo Contratto: " & node.Item("Scheda_TipoContratto").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Arredi") Is Nothing AndAlso Not node.Item("Scheda_Arredi").FirstChild Is Nothing Then
                                    nota &= "<li>Arredi: " & node.Item("Scheda_Arredi").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_ElettricitaGas") Is Nothing AndAlso Not node.Item("Scheda_ElettricitaGas").FirstChild Is Nothing Then
                                    nota &= "<li>Elettricità e Gas: " & node.Item("Scheda_ElettricitaGas").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Tv") Is Nothing AndAlso Not node.Item("Scheda_Tv").FirstChild Is Nothing Then
                                    nota &= "<li>TV: " & node.Item("Scheda_Tv").FirstChild.Value & "</li>"
                                End If
                                If Not node.Item("Scheda_Internet") Is Nothing AndAlso Not node.Item("Scheda_Internet").FirstChild Is Nothing Then
                                    nota &= "<li>Internet: " & node.Item("Scheda_Internet").FirstChild.Value & "</li>"
                                End If

                                nota &= "</ul>"
                                If Not node.Item("Descrizione") Is Nothing AndAlso Not node.Item("Descrizione").FirstChild Is Nothing Then
                                    nota &= "<p>" & node.Item("Descrizione").FirstChild.Value & "</p>"
                                End If
                                ._nota = nota

                            End With


                            'verico che l'annuncio non sia già presente
                            _dataTable = _manager.getAnnuncioByExternalId(_manager._externalId)

                            'verifico la data di  modifica
                            _temp = node.Item("Modifica").FirstChild.Value
                            dataModifica = New DateTime(Integer.Parse(_temp.Substring(0, 4)), Integer.Parse(_temp.Substring(4, 2)), Integer.Parse(_temp.Substring(6, 2)), Integer.Parse(_temp.Substring(8, 2)), Integer.Parse(_temp.Substring(10, 2)), Integer.Parse(_temp.Substring(12, 2)))

                            If _dataTable Is Nothing OrElse _dataTable.Rows.Count = 0 Then
                                'insert
                                '20/02/2012 inserisco l'annuncio con la data di modifica
                                annuncioId = _manager.insertAnnuncio(userId, modeTest, MyManager.MercatinoManager.StatoAnnuncio.OffLine, dataModifica)

                                _contaRecordInseriti += 1
                            Else

                                annuncioId = Long.Parse(_dataTable.Rows(0)("annuncio_id").ToString)

                                'in fase di inserimento di un annuncio imposto la data di modifica = data inserimento
                                If dataModifica > DateTime.Parse(_dataTable.Rows(0)("DATE_MODIFIED").ToString()) OrElse forceUpdate Then
                                    'se l'annuncio è stato modificato dall'ultima volta che ho caricato...
                                    'allora lo devo aggiornare

                                    '*** verifico se il prezzo è stato modificato
                                    If _manager._prezzo <> Decimal.Parse(_dataTable.Rows(0)("PREZZO").ToString()) Then
                                        If _manager.updateAnnuncioPrezzo(annuncioId, _manager._prezzo, modeTest) > 0 Then
                                            'invio email modifica del PREZZO annuncio
                                            Dim dtEmail As Data.DataTable
                                            dtEmail = _manager.getEmailUtentiInTrattativa(annuncioId)

                                            'Rel 1.0.0.7 del 08/01/2012
                                            'in ogni caso invio un'email a noi
                                            'se ci sono anche delle persone in trattativa, avviso anche loro
                                            Dim email As New MyManager.MailMessage(System.Configuration.ConfigurationManager.AppSettings("application.url"), System.Configuration.ConfigurationManager.AppSettings("application.name"))

                                            email._Subject = System.Configuration.ConfigurationManager.AppSettings("application.name") & " - Aggiornamento prezzo"
                                            email._Body = email.getBodyImmobiliareAggiornamentoPrezzoAnnuncio(annuncioId, CType(_manager._categoria, Annunci.Models.Immobile.Categorie).ToString & " - " & CType(_manager._tipoImmobile, Annunci.Models.Immobile.TipoImmobile).ToString, Decimal.Parse(_dataTable.Rows(0)("prezzo").ToString), _manager._prezzo)

                                            'MY-DEBUGG
                                            email._Bcc(System.Configuration.ConfigurationManager.AppSettings("mail.To.Ccn"))

                                            If dtEmail.Rows.Count > 0 Then
                                                For Each emailTo As Data.DataRow In dtEmail.Rows
                                                    email._Bcc(emailTo("email").ToString)
                                                Next

                                                email.send()
                                            End If
                                            'Rutigliano 04/03/2013
                                            'email.send()
                                        End If

                                        _contaRecordAggiornatiPrezzo += 1
                                    End If


                                    If _manager._nota <> _dataTable.Rows(0)("DESCRIZIONE").ToString() Then
                                        If _manager.updateAnnuncioDescrizione(annuncioId, _manager._nota, modeTest) > 0 Then

                                            'invio email modifica annuncio
                                            Dim dtEmail As Data.DataTable
                                            dtEmail = _manager.getEmailUtentiInTrattativa(annuncioId)

                                            If dtEmail.Rows.Count > 0 Then
                                                'Dim email As New MyMailMessage
                                                Dim email As New MyManager.MailMessage(System.Configuration.ConfigurationManager.AppSettings("application.url"), System.Configuration.ConfigurationManager.AppSettings("application.name"))

                                                email._Subject = System.Configuration.ConfigurationManager.AppSettings("application.name") & " - Modifica annuncio"
                                                email._Body = email.getBodyImmobiliareModificaTestoAnnuncio(annuncioId, CType(_manager._categoria, Annunci.Models.Immobile.Categorie).ToString & " - " & CType(_manager._tipoImmobile, Annunci.Models.Immobile.TipoImmobile).ToString)
                                                'MY-DEBUGG
                                                email._Bcc(System.Configuration.ConfigurationManager.AppSettings("mail.To.Ccn"))

                                                For Each emailTo As Data.DataRow In dtEmail.Rows
                                                    email._Bcc(emailTo("email").ToString)
                                                Next

                                                email.send()
                                            End If
                                        End If
                                        _contaRecordAggiornatiNota += 1
                                    End If


                                    '28/02/2012
                                    'Aggiorno il piano a tutti
                                    ' If modeTest = False Then
                                    '_manager.updatePianoAnnuncio(annuncioId, _manager._piano)
                                    ' End If

                                    _manager.updateAnnuncio(annuncioId, modeTest)

                                    _contaRecordDaAggiornare += 1
                                End If

                                If modeTest = False Then
                                    _manager.updateStatoAnnuncio(annuncioId, MyManager.MercatinoManager.StatoAnnuncio.OffLine)
                                End If
                            End If


                            '*** PHOTO ***
                            If modeTest = False AndAlso annuncioId <> -1 Then
                                'cancello le vecchie Photo 
                                _manager.deleteAllPhoto(annuncioId)

                                'inserimento delle Photo 
                                Dim i As Integer = 1
                                While Not node.Item("Foto" & i) Is Nothing
                                    _manager.insertPhoto(node.Item("Foto" & i).FirstChild.Value, "", annuncioId, userId)
                                    i = i + 1
                                End While
                            End If
                        End If 'dell IF agenzia is disabled
                    End If
                Catch ex As Exception

#If DEBUG Then
                    If (ex.Message = "Regione non valorizzata.") Then

                    ElseIf (ex.Message.StartsWith("Provincia non valida:")) Then

                    ElseIf (ex.Message.StartsWith("NAZIONE non valida:")) Then

                    ElseIf (ex.Message.StartsWith("TipoImmobile non gestito:")) Then

                    ElseIf (ex.Message.StartsWith("prezzo non valido:")) Then

                    ElseIf (ex.Message = "Categoria non valorizzata") Then

                    ElseIf (ex.Message = "Input string was not in a correct format.") Then

                    Else
                        Debug.WriteLine("EXCEPTION: " + ex.Message)
                    End If
#End If

                    _message &= "[Agenzia-Annuncio " & _manager._externalId & "] " & ex.Message & vbCrLf
                    _contaRecordConErrori += 1
                    If Not _manager._getTransaction Is Nothing Then
                        _manager._transactionRollback()
                    End If
                    _manager.openConnection()

                    MyLog.Exception("[Agenzia-Annuncio " & _manager._externalId & "]", ex)
                    MyLog.Flush()
                    ' _managerLog.insert("RevoAgent", ex.Message, _manager._externalId, "Annuncio id", MyManager.LogManager.MyLevel.MyError)
                End Try
            Next

            If modeTest = False Then
                _message &= vbCrLf
                '*** Cancello gli annunci nello stato DaCancellare ***
                _message &= deleteAnnunciDaCancellareByRevoAgent(_manager) & vbCrLf

                '*** Pubblico gli annunci ****
                Dim conta As Integer
                conta = _manager.updateStatoAnnunciBySourceIdAndStatoIniziale(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.OffLine, MyManager.MercatinoManager.StatoAnnuncio.Pubblicato)
                _message &= String.Format("Sono stati pubblicati {0:N0} annunci", conta) & vbCrLf & vbCrLf

            End If


            If modeTest Then
                _message &= "End TEST: " & Now & vbCrLf
            Else
                _message &= "End: " & Now & vbCrLf
            End If

            _message &= "TIME: " & MyManager.ConsoleManager.calcolaTempoDiElaborazione(dateStart, Now) & vbCrLf & vbCrLf

            If elencoAgenzieMancanti.Count > 0 Then
                _message &= "*** Agenzie mancanti ***" & vbCrLf
                Dim en As IDictionaryEnumerator
                en = elencoAgenzieMancanti.GetEnumerator
                _contaRecordAgenzieMancanti = 0
                While en.MoveNext
                    _message &= String.Format("L'agenzia {0} non è associata a nessun utente del sito. N° di annunci {1:N0}", en.Key, en.Value) & vbCrLf

                    _contaRecordAgenzieMancanti += CInt(en.Value)
                End While
                _message &= vbCrLf
            End If

            _message &= String.Format("Record Letti: {0}", _contaRecordLetti) & vbCrLf
            _message &= String.Format("Record con data da aggiornare: {0}", _contaRecordDaAggiornare) & vbCrLf
            _message &= String.Format("Record Aggiornati PREZZO: {0}", _contaRecordAggiornatiPrezzo) & vbCrLf
            _message &= String.Format("Record Aggiornati NOTA: {0}", _contaRecordAggiornatiNota) & vbCrLf
            _message &= String.Format("Record Cancellati: {0}", _contaRecordCancellati) & vbCrLf
            _message &= String.Format("Record con Errori: {0}", _contaRecordConErrori) & vbCrLf
            _message &= String.Format("Record Inseriti: {0}", _contaRecordInseriti) & vbCrLf
            _message &= String.Format("Record di agenzie disabilitate: {0}", _contaRecordAgenzieDisabled) & vbCrLf
            _message &= String.Format("Record di agenzie mancanti: {0}", _contaRecordAgenzieMancanti) & vbCrLf


            '*** Info stato annunci ****
            _message &= vbCrLf & "*** Stato annunci RevoAgent ***" & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySource(Annunci.Models.Immobile.TipoSourceId.RevoAgent)
            _message &= "Totale annunci RevoAgent (EXTERNAL):" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.OffLine)
            _message &= "Offline:" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.Pubblicato)
            _message &= "Pubblicati:" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.DaCancellare)
            _message &= "DaCancellare:" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.ConclusoConSuccesso)
            _message &= "ConclusoConSuccesso:" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.OggettoNonPiuDisponibile)
            _message &= "OggettoNonPiuDisponibile:" & _dataTable.Rows.Count & vbCrLf

            _dataTable = _manager.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.Altro)
            _message &= "Altro:" & _dataTable.Rows.Count & vbCrLf

            Dim t As String
            If modeTest Then
                t = "RevoAgent-TEST"
            Else
                t = "RevoAgent"
            End If

            If _contaRecordConErrori > 0 Then
                _managerLog.insert(t, _message, "", "Import RevoAgent", MyManager.LogManager.MyLevel.MyError)
            Else
                _managerLog.insert(t, _message, "", "Import RevoAgent", MyManager.LogManager.MyLevel.MyInfo)
            End If

            MyLog.Info(vbCrLf & vbCrLf & _message)
        Finally
            _manager.closeConnection()
            _managerLog.closeConnection()
            _managerRegioneProvinciaComune.closeConnection()

            MyLog.Flush()
            MyLog.Close()

            'If (IO.File.Exists(_pathStatusFile)) Then
            '    IO.File.Delete(_pathStatusFile)
            'End If

        End Try


        Return _message
    End Function



    Public Function processV2() As String

        '        var reader = XmlReader.Create(filename);
        'reader.WhitespaceHandling = WhitespaceHandling.None;
        '        While (reader.Read())
        '{
        '    // your code here.
        '}

        Return ""
    End Function



    Public Function _downloadFileXML(fullPathSaveFile As String) As Boolean
        'eseguo il download del file degli annunci
        Dim myRequest As System.Net.WebRequest
        Dim contaTentativi As Integer = 0
STEP1:
        myRequest = System.Net.WebRequest.Create(_urlAnnunci)
        myRequest.Timeout = 15000

        MyLog.Info("Inizio download del file XML")


        Dim myResponse As System.Net.WebResponse = Nothing

        Try
            myResponse = myRequest.GetResponse()
        Catch ex As System.Net.WebException
            'connection time out
            If contaTentativi <= 3 Then
                System.Threading.Thread.Sleep(5000)
                contaTentativi = contaTentativi + 1
                GoTo STEP1
            End If
            MyLog.Exception("Download del file XML", ex)

            MyManager.MailMessage.send(ex, "RevoAgent - connection error")
            Return False

        End Try

        '


        ''Dim reader As New System.IO.StreamReader(myResponse.GetResponseStream, System.Text.Encoding.UTF8)
        ''Dim html As String
        ''html = reader.ReadToEnd
        ''myResponse.Close()
        ''_documentXML = New System.Xml.XmlDocument()
        ''Try
        ''    _documentXML.LoadXml(html)
        ''Catch ex As Exception
        ''    Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
        ''End Try


        '24/04/2013 OutOfMemory
        Dim reader As New System.IO.StreamReader(myResponse.GetResponseStream, System.Text.Encoding.UTF8)
        _documentXML = New System.Xml.XmlDocument()
        Try
            MyLog.Info("Inizio lettura del file XML")
            _documentXML.Load(reader)
            MyLog.Info("Lettura del file XML conclusa con succeso")


            If (System.IO.File.Exists(fullPathSaveFile)) Then
                System.IO.File.Delete(fullPathSaveFile)
            End If


            _documentXML.Save(fullPathSaveFile)
            MyLog.Info("Download del file XML conluso con successo")
            MyLog.Flush()

            Return True

        Catch ex As Exception
            MyLog.Exception("RevoAgent - Errore nel parsing del file XML.", ex)
            Throw New MyManager.ManagerException("RevoAgent - Errore nel parsing del file XML.", ex)
        Finally
            If (Not reader Is Nothing) Then
                reader.Close()
                reader.Dispose()
            End If
            myResponse.Close()
        End Try
    End Function

    Public Function deleteAnnunciALLByRevoAgent() As String
        Dim managerLocal As ImmobiliareManager

        managerLocal = New ImmobiliareManager()
        managerLocal.openConnection()

        ' Dim dt As Data.DataTable
        Try
            _dataTable = managerLocal.getAnnunciExternalBySource(Annunci.Models.Immobile.TipoSourceId.RevoAgent)

            For Each row As Data.DataRow In _dataTable.Rows

                ''prendo tutti gli utenti in trattativa
                'dt = _manager.getEmailUtentiInTrattativa(Long.Parse(row("annuncio_id").ToString))
                'If dt.Rows.Count > 0 Then
                '    Dim mail As New MyMailMessage
                '    mail._Subject = System.Configuration.ConfigurationManager.AppSettings("application.name") & " - Cancellazione annuncio"
                '    mail._Body = mail.getBodyImmobiliareCancellaAnnuncio(row("tipo").ToString & " " & row("comune").ToString & " " & row("indirizzo").ToString)

                '    For Each email As Data.DataRow In dt.Rows
                '        mail._Bcc(email("email").ToString)
                '    Next

                '    'MY-DEBUGG
                '    'mail._ToClearField()
                '    ' mail._To("roberto.rutigliano@gmail.com")

                '    mail._Bcc(System.Configuration.ConfigurationManager.AppSettings("mail.To.Ccn"))

                '    mail.send()
                'End If

                managerLocal.deleteAnnuncio(Long.Parse(row("annuncio_id").ToString), "")
            Next


        Finally
            managerLocal.closeConnection()
        End Try

        Return "Sono stati cancellati " & _dataTable.Rows.Count & " annunci."
    End Function


    Public Function deleteAnnunciDaCancellareByRevoAgent() As String
        Return deleteAnnunciDaCancellareByRevoAgent(Nothing)
    End Function
    Public Function deleteAnnunciDaCancellareByRevoAgent(ByRef managerLocal As ImmobiliareManager) As String

        Dim flag As Boolean = False


        If managerLocal Is Nothing Then
            managerLocal = New ImmobiliareManager
            managerLocal.openConnection()
            flag = True
        End If

        Dim dt As Data.DataTable
        Try
            _dataTable = managerLocal.getAnnunciExternalBySourceAndStato(Annunci.Models.Immobile.TipoSourceId.RevoAgent, MyManager.MercatinoManager.StatoAnnuncio.DaCancellare)

            For Each row As Data.DataRow In _dataTable.Rows

                'prendo tutti gli utenti in trattativa
                dt = managerLocal.getEmailUtentiInTrattativa(Long.Parse(row("annuncio_id").ToString))
                If dt.Rows.Count > 0 Then

                    Dim mail As New Annunci.ImmobiliareMailMessageManager(System.Configuration.ConfigurationManager.AppSettings("application.url"), System.Configuration.ConfigurationManager.AppSettings("application.name"))


                    'Dim mail As New MyManager.MailMessage(System.Configuration.ConfigurationManager.AppSettings("application.url"), System.Configuration.ConfigurationManager.AppSettings("application.name"))

                    mail.Subject = System.Configuration.ConfigurationManager.AppSettings("application.name") & " - Cancellazione annuncio"
                    mail.Body = mail.getBodyCancellaAnnuncio(row("tipo").ToString & " " & row("comune").ToString & " " & row("indirizzo").ToString, "Immobiliare\MyTrattative")

                    If (dt.Rows.Count > 0) Then
                        For Each email As Data.DataRow In dt.Rows
                            mail.Bcc(email("email").ToString)
                        Next

                        'MY-DEBUGG
                        'mail._ToClearField()
                        ' mail._To("roberto.rutigliano@gmail.com")
                        mail.Bcc(System.Configuration.ConfigurationManager.AppSettings("mail.To.Ccn"))
                        mail.send()
                    End If
                    'cancellazione logica
                    managerLocal.deleteAnnuncioLogic(Long.Parse(row("annuncio_id").ToString), "")
                Else

                    'cancellazione fisica
                    managerLocal.deleteAnnuncio(Long.Parse(row("annuncio_id").ToString), "")
                End If
            Next


        Finally
            If flag Then
                managerLocal.closeConnection()
            End If
        End Try

        Return "Sono stati cancellati " & _dataTable.Rows.Count & " annunci."
    End Function

    Private Function getCategoriaFromXML(tipoContratto As String, id As Integer) As String
        Dim node As System.Xml.XmlNode
        node = _documentXMLCategorie.SelectSingleNode("//revo/cat[contratto='" & tipoContratto & "' and id='" & id & "']")
        If node Is Nothing Then
            Throw New MyManager.ManagerException("Categoria non trovata nel file XML. TipoContratto:" & tipoContratto & " id: " & id)
        End If

        Return node.Item("nome").FirstChild.Value
    End Function

    Private Function decodeTipoAnnuncio(value As String, tipologia2 As String) As Annunci.Models.Immobile.Categorie
        Select Case value
            Case "A"
                Return Annunci.Models.Immobile.Categorie.Affitto
            Case "V"
                Return Annunci.Models.Immobile.Categorie.Vendita
            Case "I"  'Cessione
                Return Annunci.Models.Immobile.Categorie.Vendita
            Case Else
                Throw New MyManager.ManagerException("Categoria annuncio non gestito: " & value)
        End Select
    End Function

    Private Function decodeTipoImmobile(tipoContratto As String, id As Integer) As Annunci.Models.Immobile.TipoImmobile
        Dim value As String
        value = getCategoriaFromXML(tipoContratto, id)
        Select Case value.Trim()
            Case "2 locali", "3 locali", "4 o più locali", "Casa singola", "Duplex", "Contratto breve durata"
                Return Annunci.Models.Immobile.TipoImmobile.Appartamento
            Case "1 locale", "Porzione di Casa"
                Return Annunci.Models.Immobile.TipoImmobile.Monolocale
            Case "Villa", "Porzione Villa", "Villetta schiera", "Villa Bifamiliare", "Maisonette", "Baita", "Villa Bifalimiare"
                Return Annunci.Models.Immobile.TipoImmobile.Villa
            Case "Ufficio"
                Return Annunci.Models.Immobile.TipoImmobile.Ufficio
            Case "Attico"
                Return Annunci.Models.Immobile.TipoImmobile.Attico
            Case "Rustico/Casale", "Tenuta", "Azienda Agricola", "Agriturismo"
                Return Annunci.Models.Immobile.TipoImmobile.Casale
            Case "Negozio", "Magazzino", "Bar", "Ristorante", "Abbigliamento", "Non Alimentari" _
                , "Alimentari", "Parrucchiere", "Cartoleria / Art. Regalo", "Fiorista", "Estetica / Solarium" _
                , "Panetteria", "Pizza D'Asporto", "Lavanderia", "Pub", "Pizzeria", "Informatica", "Palestre / Fitness", "Giochi / Scommesse" _
                , "Enoteca", "Caffetteria", "Gioielleria / Bigiotteria", "Tabaccheria", "Edicola", "Gelateria", "Pasticceria", "Fast Food/Kebab", "Auto Officina"
                Return Annunci.Models.Immobile.TipoImmobile.Negozio
            Case "Garage", "Posto Auto"
                Return Annunci.Models.Immobile.TipoImmobile.Box
            Case "Terreno Edificabile", "Terreno Agricolo", "Terreno"
                Return Annunci.Models.Immobile.TipoImmobile.Terreno
            Case "Capannone"
                Return Annunci.Models.Immobile.TipoImmobile.Capannone
            Case "Loft"
                Return Annunci.Models.Immobile.TipoImmobile.Loft
            Case "Mansarda"
                Return Annunci.Models.Immobile.TipoImmobile.Mansarda
            Case "Stabile / Palazzo", "Hotel", "Clinica / Casa di Riposo", "Castello", "Laboratorio", "Bed and Breakfast", "Discoteca", "Stab. Balneare", "Impianti Sportivi"
                Return Annunci.Models.Immobile.TipoImmobile.Altro
            Case Else
                Throw New MyManager.ManagerException("TipoImmobile non gestito: " & value)
        End Select

        Return Nothing
    End Function

    Private Function decodeBoxAuto(value As String) As Annunci.Models.Immobile.TipoBoxAuto
        Select Case value
            Case "0"
                Return Annunci.Models.Immobile.TipoBoxAuto.No
            Case "1"
                Return Annunci.Models.Immobile.TipoBoxAuto.Singolo
            Case "2"
                Return Annunci.Models.Immobile.TipoBoxAuto.Doppio
            Case "3"
                Return Annunci.Models.Immobile.TipoBoxAuto.Triplo
            Case Else
                Return Annunci.Models.Immobile.TipoBoxAuto.Altro
                MyManager.MailManager.send(New Exception("decodeBoxAuto: " & value))
        End Select
    End Function

    Private Function decodePostoAuto(value As String) As Annunci.Models.Immobile.TipoPostoAuto
        Select Case value
            Case "0"
                Return Annunci.Models.Immobile.TipoPostoAuto.No
            Case "1"
                Return Annunci.Models.Immobile.TipoPostoAuto.Singolo_scoperto
            Case "2"
                Return Annunci.Models.Immobile.TipoPostoAuto.Doppio_scoperto
            Case "3"
                Return Annunci.Models.Immobile.TipoPostoAuto.Triplo_scoperto
            Case Else
                Return Annunci.Models.Immobile.TipoPostoAuto.Altro
                MyManager.MailManager.send(New Exception("decodePostoAuto: " & value))
        End Select
    End Function

    Private Function decodeStatoImmobile(value As String) As Annunci.Models.Immobile.TipoStatoImmobile
        Select Case value
            Case "In Costruzione"
                Return Annunci.Models.Immobile.TipoStatoImmobile.In_costruzione
            Case "Nuovo", "Buono", "Ristrutturato", "Ottimo", "Abitabile"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Ristrutturato
            Case "Da Ristrutturare", "Rustico", "Discrete Condizioni", "Mediocre"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Da_ristrutturare
            Case "Come da Origini"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Undefined
            Case Else
                Return Annunci.Models.Immobile.TipoStatoImmobile.Undefined
                MyManager.MailManager.send(New Exception("decodeStatoImmobile: " & value))
        End Select
    End Function

    Private Function decodePiano(value As String) As Integer
        '<valore>Interrato</valore> <valore>Semi Interrato</valore> 
        '<valore>Piano Terra</valore> <valore>Primo</valore> <valore>Secondo</valore> 
        '<valore>Terzo</valore> <valore>Quarto</valore> <valore>Quinto</valore> 
        '<valore>Sesto</valore> <valore>Settimo</valore> 
        '<valore>Ottavo</valore> <valore>Otto o Più</valore> 
        '<valore>Ultimo</valore> <valore>Piano Rialzato</valore> 
        '<valore>Su Più Livelli</valore> <valore>Unico Livello</valore> <valore>Duplex</valore> <valore>Attico</valore>

        Select Case value
            Case "In Costruzione"
                Return Annunci.Models.Immobile.TipoStatoImmobile.In_costruzione
            Case "Nuovo", "Buono", "Ristrutturato", "Ottimo", "Abitabile"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Ristrutturato
            Case "Da Ristrutturare", "Rustico", "Discrete Condizioni", "Mediocre"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Da_ristrutturare
            Case "Come da Origini"
                Return Annunci.Models.Immobile.TipoStatoImmobile.Undefined
            Case Else
                Return Annunci.Models.Immobile.TipoStatoImmobile.Undefined
                MyManager.MailManager.send(New Exception("decodePiano: " & value))
        End Select
    End Function

    Private Function decodePianiTotali(value As String) As Integer
        Select Case value
            Case "Uno"
                Return 1
            Case "Due"
                Return 2
            Case "Tre"
                Return 3
            Case "Quattro"
                Return 4
            Case "Cinque"
                Return 5
            Case "Sei"
                Return 6
            Case "Sette"
                Return 7
            Case "Otto"
                Return 8
            Case "Nove o più"
                Return 9
            Case Else
                Return Integer.MinValue
                MyManager.MailManager.send(New Exception("decodePianiTotali: " & value))
        End Select
    End Function

    Private Function decodeTipoCucina(value As String) As Annunci.Models.Immobile.TipoCucina
        Select Case value
            Case "Abitabile"
                Return Annunci.Models.Immobile.TipoCucina.Abitabile
            Case "Semi Abitabile"
                Return Annunci.Models.Immobile.TipoCucina.Semi_abitabile
            Case "Angolo Cottura", "A Vista"
                Return Annunci.Models.Immobile.TipoCucina.Angolo_cottura
            Case "Cucinotto"
                Return Annunci.Models.Immobile.TipoCucina.Con_tinello
            Case Else
                Return Annunci.Models.Immobile.TipoCucina.Altro
                MyManager.MailManager.send(New Exception("decodeTipoCucina: " & value))
        End Select
    End Function

    Private Function decodeBalconi(value As String) As Integer
        Select Case value
            Case "No"
                Return 0
            Case "Uno"
                Return 1
            Case "Due"
                Return 2
            Case "Tre"
                Return 3
            Case "Quattro"
                Return 4
            Case "Cinque o più"
                Return 5
            Case Else
                Return -1
                MyManager.MailManager.send(New Exception("decodeBalconi: " & value))
        End Select
    End Function

    Private Function decodeTerrazzi(value As String) As Integer
        Select Case value
            Case "No"
                Return 0
            Case "Uno"
                Return 1
            Case "Due"
                Return 2
            Case "Tre"
                Return 3
            Case "Quattro"
                Return 4
            Case "Cinque o più"
                Return 5
            Case Else
                Return -1
                MyManager.MailManager.send(New Exception("decodeTerrazzi: " & value))
        End Select
    End Function

    Private Function decodeCantina(value As String) As Integer
        Select Case value
            Case "Cantina"
                Return 1
            Case Else
                Return 0
                'MyManager.MailManager.send(New Exception("decodeTerrazzi: " & value))
        End Select
    End Function

    Private Function decodeTipoRiscaldamento(value As String) As Annunci.Models.Immobile.TipoRiscaldamento
        Select Case value
            Case "Autonomo", "A Pavimento", "Tele Riscaldamento"
                Return Annunci.Models.Immobile.TipoRiscaldamento.Autonomo
            Case "Centralizzato"
                Return Annunci.Models.Immobile.TipoRiscaldamento.Centralizzato
            Case "Conta Calorie"
                Return Annunci.Models.Immobile.TipoRiscaldamento.Centralizzato_ripartito
            Case "Da Creare", "No"
                Return Annunci.Models.Immobile.TipoRiscaldamento.No
            Case Else
                Return Annunci.Models.Immobile.TipoRiscaldamento.Undefined
                MyManager.MailManager.send(New Exception("decodeTipoRiscaldamento: " & value))
        End Select
    End Function
End Class
